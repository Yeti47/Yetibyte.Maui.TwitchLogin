using Foundation;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Services
{
    public partial class CookieManager
    {
        public void DeleteAllCookies()
        {
            NSHttpCookieStorage.SharedStorage.RemoveCookiesSinceDate(NSDate.DistantPast);
        }

        public Task<CookieManager.Cookie[]> GetCookiesAsync(string uri)
        {
            throw new NotImplementedException("Currently, reading out cookies is not supported on iOS.");
        }

        public void DeleteCookie(string url, string cookieName)
        {
            var cookiesToDelete = NSHttpCookieStorage.SharedStorage.CookiesForUrl(new NSUrl(url)).Where(c => c.Name == cookieName).ToArray();

            foreach ( var cookie in cookiesToDelete)
            {
                NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);
            }

        }
    }
}
