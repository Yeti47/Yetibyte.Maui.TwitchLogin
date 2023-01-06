using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Services
{
    public partial class CookieManager
    {
        public Task DeleteAllCookiesAsync()
        {
            throw new NotSupportedException("Currently, MacCatalyst does not support deleting cookies.");
        }

        public Task<CookieManager.Cookie[]> GetCookiesAsync(string uri)
        {
            throw new NotImplementedException("Currently, reading out cookies is not supported on MacCatalyst.");
        }

        public Task DeleteCookieAsync(string url, string cookieName)
        {
            throw new NotImplementedException("Currently, deleting cookies is not supported on MacCatalyst.");

        }
    }
}
