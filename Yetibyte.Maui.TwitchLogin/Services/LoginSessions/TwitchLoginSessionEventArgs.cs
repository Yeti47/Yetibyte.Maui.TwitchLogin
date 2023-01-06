namespace Yetibyte.Maui.TwitchLogin.Services.LoginSessions
{
    public class TwitchLoginSessionEventArgs : EventArgs
    {
        public ITwitchLoginSession Session { get; }

        public TwitchLoginSessionEventArgs(ITwitchLoginSession session)
        {
            Session = session;
        }
    }
}
