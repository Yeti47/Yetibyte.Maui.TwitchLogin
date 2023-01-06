using System;

namespace Yetibyte.Maui.TwitchLogin.Services
{
    public static partial class ServiceProvider
    {
        private class TizenServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                throw new NotSupportedException("Cannot resolve dependencies in Tizen.");
            }
        }

        private static IServiceProvider _instance;
        
        public static IServiceProvider Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TizenServiceProvider();
                }

                return _instance;
            }
        }

    }


}
