using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yetibyte.Maui.TwitchLogin.Core;
using Yetibyte.Maui.TwitchLogin.Services;
using Yetibyte.Maui.TwitchLogin.Services.LoginSessions;

namespace Yetibyte.Maui.TwitchLogin.ViewModels
{
    internal class TwitchLoginViewModel : ViewModel
    {
        #region Fields

        private readonly ITwitchLoginSessionManager _sessionManager;

        private readonly TwitchLoginNavigator _loginNavigator;

        private readonly TwitchLoginState _state;

        private string _accessToken = string.Empty;
        private Uri _redirectUri = new Uri(TwitchLoginNavigator.DEFAULT_REDIRECT_URI);
        private string _clientId = string.Empty;
        private string _scope = string.Empty;
        private bool _useCustomRedirectSource = false;

        #endregion

        #region Props

        public string AccessToken
        {
            get => _accessToken;
            set
            {
                if (_accessToken != value)
                {

                    _accessToken = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        public Uri RedirectUri
        {
            get => _redirectUri;
            set
            {
                if (_redirectUri != value)
                {

                    _redirectUri = value;
                    _loginNavigator.RedirectUri = value;

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

        public string Scope
        {
            get => _scope;
            set
            {
                if (_scope != value)
                {
                    _scope = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AuthenticationUrl));
                }
            }
        }

        public string AuthenticationUrl
        {
            get
            {
                string source = $"{TwitchLoginNavigator.TWITCH_ID_BASE_URI}/oauth2/authorize?response_type=token&client_id={_clientId}&redirect_uri={RedirectUri.OriginalString}&scope={Scope}&state={_state}";
                return source;
            }
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(_accessToken);

        public bool UseCustomRedirectSource { 
            get => _useCustomRedirectSource; 
            set => _useCustomRedirectSource = value; 
        }

        #endregion

        #region Events

        public event EventHandler<TwitchLoginSucceededEventArgs> LoginSucceeded;
        public event EventHandler<TwitchLoginFailedEventArgs> LoginFailed;
        public event EventHandler<TwitchLoginUnexpectedRedirectEventArgs> UnexpectedRedirect;
        public event EventHandler<TwitchLoginStateMismatchEventArgs> StateMismatchOccurred;

        #endregion

        #region Ctors

        public TwitchLoginViewModel(ITwitchLoginSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _loginNavigator = new TwitchLoginNavigator();

            _state = TwitchLoginState.GenerateNew();
        }

        #endregion

        #region Methods

        public bool ProcessNavigationUri(Uri navigationUri)
        {
            TwitchLoginContext loginContext = _loginNavigator.GetLoginContext(navigationUri);

            switch (loginContext)
            {
                case TwitchLoginContext.TwitchAuthentication:
                    return true;
                case TwitchLoginContext.Twitch:
                    return true;
                case TwitchLoginContext.Redirect:
                    return TryLogin(navigationUri);
                default:
                    if (UseCustomRedirectSource && navigationUri.ToString().StartsWith("data:text/html"))
                        return true;
                    OnUnexpectedRedirect(new TwitchLoginUnexpectedRedirectEventArgs(navigationUri));
                    return false;
            }
        }

        private bool TryLogin(Uri navigationUri)
        {
            try
            {
                var loginResult = _loginNavigator.ProcessLoginRedirect(navigationUri, _state);

                if (loginResult.Success)
                {
                    AccessToken = loginResult.AccessToken;

                    OnLoginSucceeded(new TwitchLoginSucceededEventArgs(loginResult.AccessToken, loginResult.Scopes));

                    return true;
                }
                return false;
            }
            catch (TwitchLoginStateMismatchException stateMismatchException)
            {
                OnStateMismatchOccurred(new TwitchLoginStateMismatchEventArgs(stateMismatchException.ExpectedState, stateMismatchException.ActualState));
                return false;
            }

            catch (TwitchLoginErrorResponseException errorResponseException)
            {
                OnLoginFailed(new TwitchLoginFailedEventArgs(errorResponseException.Error, errorResponseException.ErrorDescription));
                return false;
            }
        }

        protected virtual void OnLoginSucceeded(TwitchLoginSucceededEventArgs args)
        {
            _sessionManager.StartSession(args.AccessToken);

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

        #endregion

    }
}
