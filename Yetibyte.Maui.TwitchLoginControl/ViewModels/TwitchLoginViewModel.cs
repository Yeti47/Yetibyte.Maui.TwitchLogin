using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yetibyte.Maui.TwitchLoginControl.Core;
using Yetibyte.Maui.TwitchLoginControl.Services;

namespace Yetibyte.Maui.TwitchLoginControl.ViewModels
{
    internal class TwitchLoginViewModel : ViewModel
    {
        #region Constants

        public const string DEFAULT_REDIRECT_URI = "https://localhost";

        public const string TWITCH_HOST_NAME = "twitch.tv";
        public const string TWITCH_BASE_URI = "https://twitch.tv";
        public const string TWITCH_ID_BASE_URI = "https://id.twitch.tv";

        private const string FRAGMENT_PARAMETER_NAME_ACCESS_TOKEN = "access_token";
        private const string FRAGMENT_PARAMETER_NAME_ERROR= "error";
        private const string FRAGMENT_PARAMETER_NAME_ERROR_DESCR = "error_description";
        private const string FRAGMENT_PARAMETER_NAME_STATE = "state";

        private static readonly string[] TWITCH_LOGIN_COOKIE_NAMES = new[] { "last_login", "name", "login", "twitch.lohp.countryCode", "api_token", "auth-token", "server_session_id", "persistent" };

        #endregion

        #region Fields

        private readonly ICookieManager _cookieManager;

        private readonly TwitchLoginState _state;

        private string _accessToken = string.Empty;
        private Uri _redirectUri = new Uri(DEFAULT_REDIRECT_URI);
        private string _clientId = string.Empty;
        private string _currentUrl = string.Empty;

        #endregion

        #region Props

        public string AccessToken
        {
            get => _accessToken;
            private set {
                if (_accessToken != value) {

                    _accessToken = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        public Uri RedirectUri
        {
            get => _redirectUri;
            set {
                if (_redirectUri != value) {

                    _redirectUri = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AuthenticationUrl));
                }
            }
        }

        public string ClientId
        {
            get => _clientId;
            set
            {
                if (_clientId != value)
                {
                    _clientId = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AuthenticationUrl));
                }
            }
        }

        public string AuthenticationUrl
        {
            get
            {
                string source = $"{TWITCH_ID_BASE_URI}/oauth2/authorize?response_type=token&client_id={_clientId}&redirect_uri={RedirectUri.OriginalString}&state={_state}";

                return source;
            }
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(_accessToken);

        #endregion

        #region Events

        public event EventHandler<TwitchLoginSucceededEventArgs> LoginSucceeded;
        public event EventHandler<TwitchLoginFailedEventArgs> LoginFailed;
        public event EventHandler<TwitchLoginUnexpectedRedirectEventArgs> UnexpectedRedirect;
        public event EventHandler<TwitchLoginStateMismatchEventArgs> StateMismatchOccurred;
        public event EventHandler<TwitchLogoutEventArgs> LoggedOut;

        #endregion

        #region Ctors

        public TwitchLoginViewModel(ICookieManager cookieManager)
        {
            _cookieManager = cookieManager;
            _state = TwitchLoginState.GenerateNew();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the redirect URI including fragment parameters returned from the Twitch auth server.
        /// </summary>
        /// <param name="navigationUri">The URI that the Twitch server redirected to after the Login.</param>
        /// <returns>True if the uri could be processed</returns>
        public bool ProcessLoginResponse(Uri navigationUri)
        {
            TwitchLoginContext loginContext = GetLoginContext(navigationUri);

            switch (loginContext)
            {
                case TwitchLoginContext.TwitchAuthentication:
                    return true;
                case TwitchLoginContext.Twitch:
                    return true;
                case TwitchLoginContext.Redirect:
                    return HandleRedirection(navigationUri);
                default:
                    OnUnexpectedRedirect(new TwitchLoginUnexpectedRedirectEventArgs(navigationUri));
                    return false;
            }

        }

        private bool HandleRedirection(Uri navigationUri)
        {
            bool hasFragment = !string.IsNullOrEmpty(navigationUri.Fragment);
            var urlParams = HttpUtility.ParseQueryString(hasFragment ? navigationUri.Fragment.TrimStart('#') : navigationUri.Query);

            TwitchLoginState state = new TwitchLoginState(urlParams[FRAGMENT_PARAMETER_NAME_STATE] ?? string.Empty);

            if (!this._state.Equals(state))
            {
                // Possible XSS attack!
                OnStateMismatchOccurred(new TwitchLoginStateMismatchEventArgs(this._state, state));
                return false;
            }

            // The URI fragments indicate an error during authorization (e. g. User declined auth)
            if (IsErrorResponse(urlParams))
            {
                string error = urlParams[FRAGMENT_PARAMETER_NAME_ERROR] ?? string.Empty;
                string errorDescription = urlParams[FRAGMENT_PARAMETER_NAME_ERROR_DESCR] ?? string.Empty;

                TwitchLoginFailedEventArgs loginFailedArgs = new TwitchLoginFailedEventArgs(error, errorDescription)
                {
                    State = state
                };

                OnLoginFailed(loginFailedArgs);

                return false;
            }
            // No error: redirection to target redirect_uri succeeded
            else
            {
                string accessToken = urlParams[FRAGMENT_PARAMETER_NAME_ACCESS_TOKEN] ?? string.Empty;

                if (string.IsNullOrEmpty(accessToken))
                    return false;

                if (AccessToken == accessToken)
                    return false; // Same page has been reloaded, no new access token was received, so no login event should be raised

                AccessToken = accessToken;

                string scopeSequence = urlParams["scope"] ?? string.Empty;

                string[] scopes = scopeSequence.Split('%', StringSplitOptions.RemoveEmptyEntries).Select(s => HttpUtility.UrlDecode(s)).ToArray();

                TwitchLoginSucceededEventArgs loginSucceededArgs = new TwitchLoginSucceededEventArgs(accessToken, scopes)
                {
                    State = state
                };

                OnLoginSucceeded(loginSucceededArgs);

                return true;
            }
        }

        private TwitchLoginContext GetLoginContext(Uri navigationUri)
        {
            if (IsTwitchIdUri(navigationUri))
                return TwitchLoginContext.TwitchAuthentication;

            if (IsTwitchUri(navigationUri)) 
                return TwitchLoginContext.Twitch;

            if (IsRedirectionTarget(navigationUri))
                return TwitchLoginContext.Redirect;

            return TwitchLoginContext.Unknown;
        }

        private bool IsErrorResponse(NameValueCollection queryParamCollection)
        {
            return queryParamCollection.AllKeys.Any(k => k == FRAGMENT_PARAMETER_NAME_ERROR);
        }

        /// <summary>
        /// Checks whether the given URI is within the domain of the specified target redirect URI (see: <see cref="RedirectUri"/>).
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool IsRedirectionTarget(Uri uri) => RedirectUri.IsBaseOf(uri);

        public bool IsTwitchIdUri(Uri uri) => new Uri(TWITCH_ID_BASE_URI).IsBaseOf(uri);

        public bool IsTwitchUri(Uri uri) => uri.Host.EndsWith(TWITCH_HOST_NAME, StringComparison.OrdinalIgnoreCase);

        public bool IsExpectedUri(Uri uri)
        {
            return IsRedirectionTarget(uri) 
                || IsTwitchUri(uri)
                || IsTwitchIdUri(uri);
        }

        public void Logout()
        {
            DeleteLoginCookies();

            string oldAccessToken = AccessToken;

            AccessToken = string.Empty;

            OnLoggedOut(new TwitchLogoutEventArgs(oldAccessToken));
        }

        public async Task<IEnumerable<CookieManager.Cookie>> GetTwitchCookiesAsync()
        {
            return await _cookieManager.GetCookiesAsync(TWITCH_BASE_URI);
        }

        private void DeleteLoginCookies()
        {
            foreach (string cookieName in TWITCH_LOGIN_COOKIE_NAMES)
            {
                _cookieManager.DeleteCookie(TWITCH_BASE_URI, cookieName);
            }
        }

        protected virtual void OnLoginSucceeded(TwitchLoginSucceededEventArgs args)
        {
            var handler = LoginSucceeded;
            handler?.Invoke(this, args);
        }
        protected virtual void OnLoginFailed(TwitchLoginFailedEventArgs args)
        {
            var handler = LoginFailed;
            handler?.Invoke(this, args);
        }

        protected virtual void OnUnexpectedRedirect(TwitchLoginUnexpectedRedirectEventArgs args)
        {
            var handler = UnexpectedRedirect;
            handler?.Invoke(this, args);
        }

        protected virtual void OnStateMismatchOccurred(TwitchLoginStateMismatchEventArgs args)
        {
            var handler = StateMismatchOccurred;
            handler?.Invoke(this, args);
        }

        protected virtual void OnLoggedOut(TwitchLogoutEventArgs args)
        {
            var handler = LoggedOut;
            handler?.Invoke(this, args);
        }

        #endregion

    }
}
