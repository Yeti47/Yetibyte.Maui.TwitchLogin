namespace Yetibyte.Maui.TwitchLoginControl.Core
{
    public class TwitchLoginResult
    {
        public string AccessToken { get; }
        public string[] Scopes { get; }

        public bool Success => !string.IsNullOrEmpty(AccessToken);

        public TwitchLoginResult(string accessToken, string[] scopes)
        {
            AccessToken = accessToken;
            Scopes = scopes;
        }

        public static TwitchLoginResult CreateEmpty() => new TwitchLoginResult(string.Empty, Array.Empty<string>());

    }
}
