namespace Yetibyte.Maui.TwitchLoginControl.Services.LoginSessions
{
    internal class InitialTwitchLoginSession : ITwitchLoginSession
    {
        public static InitialTwitchLoginSession Instance { get; } = new InitialTwitchLoginSession();

        public string AccessToken => string.Empty;

        public DateTime StartDate => default;

        public DateTime EndDate => default;

        public bool IsActive => false;

        void ITwitchLoginSession.OnEnding() { }
    }
}
