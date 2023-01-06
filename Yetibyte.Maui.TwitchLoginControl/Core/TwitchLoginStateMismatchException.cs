namespace Yetibyte.Maui.TwitchLoginControl.Core
{
    public class TwitchLoginStateMismatchException : Exception
    {
        public TwitchLoginState ExpectedState { get; }
        public TwitchLoginState ActualState { get; }

        public TwitchLoginStateMismatchException(TwitchLoginState expectedState, TwitchLoginState actualState, string message = "") : base(message)
        {
            ExpectedState = expectedState;
            ActualState = actualState;
        }
    }
}
