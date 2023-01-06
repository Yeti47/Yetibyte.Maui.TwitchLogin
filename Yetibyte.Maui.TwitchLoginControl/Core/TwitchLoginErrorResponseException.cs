namespace Yetibyte.Maui.TwitchLoginControl.Core
{
    public class TwitchLoginErrorResponseException : Exception
    {
        public string Error { get; }
        public string ErrorDescription { get; }

        public TwitchLoginErrorResponseException(string error, string errorDescription)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

    }
}
