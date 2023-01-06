using Microsoft.Maui.Platform;
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
            MauiWebView? mauiWebView = this._webView.Handler.PlatformView as MauiWebView;
            
            var cookieManager = mauiWebView?.CoreWebView2?.CookieManager;

            cookieManager?.DeleteAllCookies();
        }

        public async Task<CookieManager.Cookie[]> GetCookiesAsync(string uri)
        {
            MauiWebView? mauiWebView = this._webView.Handler.PlatformView as MauiWebView;

            var cookieManager = mauiWebView?.CoreWebView2?.CookieManager;

            var cookies = await cookieManager?.GetCookiesAsync(uri);

            return cookies.Select(c => new CookieManager.Cookie(c.Name, c.Value)).ToArray();
        }

        public void DeleteCookie(string url, string cookieName)
        {
            MauiWebView? mauiWebView = this._webView.Handler.PlatformView as MauiWebView;
            
            var cookieManager = mauiWebView?.CoreWebView2?.CookieManager;

            Uri uri = new Uri(url);
            string domain = "." + uri.Host;
            string path = uri.AbsolutePath;

            //cookieManager.DeleteCookies(cookieName, url);
            cookieManager.DeleteCookiesWithDomainAndPath(cookieName, domain, path);

        }
    }
}
