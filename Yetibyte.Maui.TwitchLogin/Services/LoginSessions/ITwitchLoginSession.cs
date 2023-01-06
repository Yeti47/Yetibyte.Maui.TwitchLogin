using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Services.LoginSessions
{
    public interface ITwitchLoginSession
    {
        string AccessToken { get; }

        DateTime StartDate { get; }
        DateTime EndDate { get; }

        bool IsActive { get; }

        void OnEnding();
    }
}
