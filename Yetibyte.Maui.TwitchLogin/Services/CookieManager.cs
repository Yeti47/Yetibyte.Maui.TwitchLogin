namespace Yetibyte.Maui.TwitchLogin.Services
{
    public partial class CookieManager : ICookieManager
    {
        public record Cookie(string Name, string Value);

        public CookieManager() { }

    }


}
