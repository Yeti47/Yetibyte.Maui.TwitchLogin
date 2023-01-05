using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLoginControl.Services
{
    public partial class CookieManager
    {
        public void DeleteAllCookies()
        {
            throw new NotSupportedException("Currently, Tizen does not support deleting cookies.");
        }

        public Task<CookieManager.Cookie[]> GetCookiesAsync(string uri)
        {
            throw new NotImplementedException("Currently, reading out cookies is not supported on Tizen.");
        }

        public void DeleteCookie(string url, string cookieName)
        {
            throw new NotImplementedException("Currently, deleting cookies is not supported on Tizen.");

        }
    }
}
