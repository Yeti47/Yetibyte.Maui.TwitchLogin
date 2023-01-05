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
            Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
        }

        public Task<CookieManager.Cookie[]> GetCookiesAsync(string uri)
        {
            string cookieString = Android.Webkit.CookieManager.Instance.GetCookie(uri);

            CookieManager.Cookie[] cookies = 
                cookieString
                .Split(";")
                .Select(c => {
                    string[] comps = c.Split('=', StringSplitOptions.TrimEntries);
                    return new CookieManager.Cookie(comps[0], comps[1]);
                })
                .ToArray();

            return Task.FromResult(cookies);
        }

        public void DeleteCookie(string url, string cookieName)
        {
            Android.Webkit.CookieManager.Instance.SetCookie(url, $"{cookieName}=");
        }
    }
}
