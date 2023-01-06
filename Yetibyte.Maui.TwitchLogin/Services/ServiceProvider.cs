using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Services
{
    public static partial class ServiceProvider
    {
        public static T GetService<T>() => Current.GetService<T>();
    }
}
