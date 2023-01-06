namespace Yetibyte.Maui.TwitchLoginControl.Services.LoginSessions
{
    public class TwitchLoginSessionManager : ITwitchLoginSessionManager
    {
        private class TwitchLoginSession : ITwitchLoginSession
        {
            public string AccessToken { get; private set; }

            public DateTime StartDate { get; private set; }
            public DateTime EndDate { get; private set; }

            public bool IsActive => StartDate != default && EndDate == default;

            public TwitchLoginSession(string accessToken)
            {
                ArgumentException.ThrowIfNullOrEmpty(accessToken);

                AccessToken = accessToken;

                StartDate = DateTime.Now;
            }

            public void OnEnding()
            {
                EndDate= DateTime.Now;
            }
        }

        private const string COOKIE_NAME_USER_NAME = "name";

        private static readonly string[] TWITCH_LOGIN_COOKIE_NAMES = new[] { 
            "last_login",
            COOKIE_NAME_USER_NAME, 
            "login", 
            "twitch.lohp.countryCode", 
            "api_token", 
            "auth-token", 
            "server_session_id", 
            "persistent" 
        };

        private readonly ICookieManager _cookieManager;

        private ITwitchLoginSession _currentSession = InitialTwitchLoginSession.Instance;

        public event EventHandler<TwitchLoginSessionEventArgs> SessionStarted;
        public event EventHandler<TwitchLoginSessionEventArgs> SessionEnding;
        public event EventHandler<TwitchLoginSessionEventArgs> SessionEnded;

        public ITwitchLoginSession CurrentSession => _currentSession;

        public TwitchLoginSessionManager(ICookieManager cookieManager)
        {
            _cookieManager = cookieManager;
        }

        public ITwitchLoginSession StartSession(string accessToken)
        {
            TwitchLoginSession session = new TwitchLoginSession(accessToken);

            _currentSession = session;

            OnSessionStarted(_currentSession);

            return session;
        }

        public bool EndSession()
        {
            ClearSessionCookies();

            OnSessionEnding(_currentSession);

            if (!_currentSession.IsActive)
                return false;

            _currentSession.OnEnding();

            OnSessionEnded(_currentSession);

            return true;
        }

        public async Task<IEnumerable<CookieManager.Cookie>> GetTwitchCookiesAsync()
        {
            IEnumerable<CookieManager.Cookie> cookies = await _cookieManager.GetCookiesAsync(TwitchLoginNavigator.TWITCH_BASE_URI);

            return cookies;
        }

        public async Task<IEnumerable<CookieManager.Cookie>> GetSessionCookiesAsync()
        {
            IEnumerable<CookieManager.Cookie> cookies = await GetTwitchCookiesAsync();

            return cookies.Where(c => TWITCH_LOGIN_COOKIE_NAMES.Contains(c.Name));
        }

        public async Task ClearTwitchCookiesAsync()
        {
            IEnumerable<CookieManager.Cookie> cookies = await GetTwitchCookiesAsync();

            foreach(var cookie in cookies)
            {
                _cookieManager.DeleteCookie(TwitchLoginNavigator.TWITCH_BASE_URI, cookie.Name);
            }

            _currentSession.OnEnding();
        }

        private void ClearSessionCookies()
        {
            foreach (string cookieName in TWITCH_LOGIN_COOKIE_NAMES)
            {
                _cookieManager.DeleteCookie(TwitchLoginNavigator.TWITCH_BASE_URI, cookieName);
            }
        }

        protected virtual void OnSessionStarted(ITwitchLoginSession session)
        {
            TwitchLoginSessionEventArgs args = new TwitchLoginSessionEventArgs(session);

            var handler = SessionStarted;
            handler?.Invoke(this, args);
        }

        protected virtual void OnSessionEnding(ITwitchLoginSession session)
        {
            TwitchLoginSessionEventArgs args = new TwitchLoginSessionEventArgs(session);

            var handler = SessionEnding;
            handler?.Invoke(this, args);
        }

        protected virtual void OnSessionEnded(ITwitchLoginSession session)
        {
            TwitchLoginSessionEventArgs args = new TwitchLoginSessionEventArgs(session);

            var handler = SessionEnded;
            handler?.Invoke(this, args);
        }
    }
}
