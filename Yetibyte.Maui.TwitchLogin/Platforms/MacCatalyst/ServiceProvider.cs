namespace Yetibyte.Maui.TwitchLogin.Services
{
    public static partial class ServiceProvider
    {
        public static IServiceProvider Current => MauiUIApplicationDelegate.Current.Services;

    }

}
