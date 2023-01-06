using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Services.LoginSessions
{
    public interface ITwitchLoginSessionManager
    {
        ITwitchLoginSession CurrentSession { get; }

        event EventHandler<TwitchLoginSessionEventArgs> SessionStarted;
        event EventHandler<TwitchLoginSessionEventArgs> SessionEnding;
        event EventHandler<TwitchLoginSessionEventArgs> SessionEnded;

        ITwitchLoginSession StartSession(string accessToken);
        bool EndSession();

        Task<IEnumerable<CookieManager.Cookie>> GetTwitchCookiesAsync();
        Task<IEnumerable<CookieManager.Cookie>> GetSessionCookiesAsync();
        Task ClearTwitchCookiesAsync();
    }
}
