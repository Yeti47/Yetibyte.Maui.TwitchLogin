namespace Yetibyte.Maui.TwitchLoginControl.Services
{
    public partial class CookieManager : ICookieManager
    {
        public record Cookie(string Name, string Value);

        protected readonly WebView _webView;

        public CookieManager(WebView webview)
        {
            _webView = webview;
        }
    }

}
