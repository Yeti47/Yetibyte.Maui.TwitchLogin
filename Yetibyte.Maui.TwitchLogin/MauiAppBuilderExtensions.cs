using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Maui.TwitchLogin.Services;
using Yetibyte.Maui.TwitchLogin.Services.LoginSessions;

namespace Yetibyte.Maui.TwitchLogin
{
    public static class MauiAppBuilderExtensions
    {
        public static MauiAppBuilder AddTwitchLogin(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ICookieManager, CookieManager>();
            builder.Services.AddSingleton<ITwitchLoginSessionManager, TwitchLoginSessionManager>();

            return builder;
        }

        public static MauiAppBuilder AddTwitchLogin(this MauiAppBuilder builder, Func<IServiceProvider, ICookieManager> cookieManagerFactory = null, Func<IServiceProvider, ITwitchLoginSessionManager> sessionManagerFactory = null)
        {
            if (cookieManagerFactory == null)
                cookieManagerFactory = sp => new CookieManager();

            if (sessionManagerFactory == null)
                sessionManagerFactory = sp => new TwitchLoginSessionManager(sp.GetService<ICookieManager>());

            builder.Services.AddSingleton<ICookieManager>(cookieManagerFactory);
            builder.Services.AddSingleton<ITwitchLoginSessionManager>(sessionManagerFactory);

            return builder;
        }
    }
}
