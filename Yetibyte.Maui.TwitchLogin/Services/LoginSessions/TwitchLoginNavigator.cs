using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yetibyte.Maui.TwitchLogin.Core;

namespace Yetibyte.Maui.TwitchLogin.Services.LoginSessions
{
    public class TwitchLoginNavigator
    {
        #region Constants

        public const string DEFAULT_REDIRECT_URI = "https://localhost";

        public const string TWITCH_HOST_NAME = "twitch.tv";
        public const string TWITCH_BASE_URI = "https://twitch.tv";
        public const string TWITCH_ID_BASE_URI = "https://id.twitch.tv";

        private const string FRAGMENT_PARAMETER_NAME_ACCESS_TOKEN = "access_token";
        private const string FRAGMENT_PARAMETER_NAME_ERROR = "error";
        private const string FRAGMENT_PARAMETER_NAME_ERROR_DESCR = "error_description";
        private const string FRAGMENT_PARAMETER_NAME_STATE = "state";

        #endregion

        #region Fields

        private string _latestAccessToken = string.Empty;

        #endregion

        #region Props

        public Uri RedirectUri { get; set; }

        #endregion

        #region Ctors

        public TwitchLoginNavigator() : this(new Uri(DEFAULT_REDIRECT_URI))
        {
        }

        public TwitchLoginNavigator(Uri redirectUri)
        {
            RedirectUri = redirectUri;
        }

        #endregion

        #region Methods

        public TwitchLoginContext GetLoginContext(Uri navigationUri)
        {
            if (IsTwitchIdUri(navigationUri))
                return TwitchLoginContext.TwitchAuthentication;

            if (IsTwitchUri(navigationUri))
                return TwitchLoginContext.Twitch;

            if (IsRedirectionTarget(navigationUri))
                return TwitchLoginContext.Redirect;

            return TwitchLoginContext.Unknown;
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

        public TwitchLoginResult ProcessLoginRedirect(Uri navigationUri, TwitchLoginState expectedState)
        {
            if (GetLoginContext(navigationUri) != TwitchLoginContext.Redirect)
                throw new ArgumentException("The given navigation URL is not within the domain of the redirection target.");

            bool hasFragment = !string.IsNullOrEmpty(navigationUri.Fragment);
            var urlParams = HttpUtility.ParseQueryString(hasFragment ? navigationUri.Fragment.TrimStart('#') : navigationUri.Query);

            TwitchLoginState state = new TwitchLoginState(urlParams[FRAGMENT_PARAMETER_NAME_STATE] ?? string.Empty);

            if (!expectedState.Equals(state))
            {
                // Possible XSS attack: Throw
                throw new TwitchLoginStateMismatchException(expectedState, state);
            }

            // The URI fragments indicate an error during authorization (e. g. User declined auth)
            if (IsErrorResponse(urlParams))
            {
                string error = urlParams[FRAGMENT_PARAMETER_NAME_ERROR] ?? string.Empty;
                string errorDescription = urlParams[FRAGMENT_PARAMETER_NAME_ERROR_DESCR] ?? string.Empty;

                throw new TwitchLoginErrorResponseException(error, errorDescription);
            }
            // No error: redirection to target redirect_uri succeeded
            else
            {
                string accessToken = urlParams[FRAGMENT_PARAMETER_NAME_ACCESS_TOKEN] ?? string.Empty;

                if (string.IsNullOrEmpty(accessToken))
                    return TwitchLoginResult.CreateEmpty();

                if (_latestAccessToken == accessToken)
                    return TwitchLoginResult.CreateEmpty(); // Same page has been reloaded, no new access token was received, so no login has occurred

                _latestAccessToken = accessToken;

                string scopeSequence = urlParams["scope"] ?? string.Empty;

                scopeSequence = HttpUtility.UrlDecode(scopeSequence);

                string[] scopes = scopeSequence.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => HttpUtility.UrlDecode(s)).ToArray();

                return new TwitchLoginResult(accessToken, scopes);
            }
        }

        private bool IsErrorResponse(NameValueCollection queryParamCollection)
        {
            return queryParamCollection.AllKeys.Any(k => k == FRAGMENT_PARAMETER_NAME_ERROR);
        }

        #endregion

    }
}
