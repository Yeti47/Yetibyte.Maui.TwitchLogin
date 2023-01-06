using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Core
{
    public abstract class TwitchLoginEventArgs : EventArgs
    {

        public TwitchLoginState State { get; init; } = TwitchLoginState.Empty;

        protected TwitchLoginEventArgs() { }

    }

    public class TwitchLoginSucceededEventArgs : TwitchLoginEventArgs
    {
        private readonly string[] _scopes;

        public string AccessToken { get; }

        public IReadOnlyCollection<string> Scopes => _scopes.AsReadOnly();

        public TwitchLoginSucceededEventArgs(string accessToken)
        {
            AccessToken = accessToken;
            _scopes = Array.Empty<string>();
        }

        public TwitchLoginSucceededEventArgs(string accessToken, IEnumerable<string> scopes)
        {
            AccessToken = accessToken;
            _scopes = scopes.ToArray();
        }
    }

    public class TwitchLoginFailedEventArgs : TwitchLoginEventArgs
    {
        public string Error { get; }
        public string ErrorDescription { get; }

        public TwitchLoginFailedEventArgs(string error, string errorDescription)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }
    }

    public class TwitchLoginUnexpectedRedirectEventArgs : EventArgs
    {
        public Uri Uri { get; }

        public TwitchLoginUnexpectedRedirectEventArgs(Uri uri)
        {
            Uri = uri;
        }
    }

    public class TwitchLoginStateMismatchEventArgs : EventArgs
    {
        public TwitchLoginState ExpectedState { get; }
        public TwitchLoginState ActualState { get; }

        public TwitchLoginStateMismatchEventArgs(TwitchLoginState expectedState, TwitchLoginState actualState)
        {
            ExpectedState = expectedState;
            ActualState = actualState;
        }
    }

    public class TwitchLogoutEventArgs : EventArgs
    {
        public string OldAccessToken { get; }

        public bool WasLoggedIn => !string.IsNullOrEmpty(OldAccessToken);

        public TwitchLogoutEventArgs(string oldAccessToken)
        {
            OldAccessToken = oldAccessToken;
        }
    }
}
