using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Services
{
    public interface ICookieManager
    {
        void DeleteAllCookies();

        void DeleteCookie(string uri, string cookieName);

        Task<CookieManager.Cookie[]> GetCookiesAsync(string uri);
    }

}
