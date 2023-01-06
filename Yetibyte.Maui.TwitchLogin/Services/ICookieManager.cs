using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Services
{
    public interface ICookieManager
    {
        Task DeleteAllCookiesAsync();

        Task DeleteCookieAsync(string uri, string cookieName);

        Task<CookieManager.Cookie[]> GetCookiesAsync(string uri);
    }

}
