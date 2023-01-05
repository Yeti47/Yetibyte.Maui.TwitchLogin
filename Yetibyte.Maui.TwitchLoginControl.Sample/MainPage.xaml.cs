using Microsoft.Maui.Controls;
using Yetibyte.Maui.TwitchLoginControl.Services;

namespace Yetibyte.Maui.TwitchLoginControl.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void twitchLogin_LoginFailed(object sender, Core.TwitchLoginFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - Login Failed event raised. Error: {e.Error} | ErrorDescription: {e.ErrorDescription}");
        }

        private void twitchLogin_LoginSucceeded(object sender, Core.TwitchLoginSucceededEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - Login Succeeded event raised. Token: {e.AccessToken.Substring(0, 4)}");
            entryAuthToken.Text = e.AccessToken;
        }

        private void twitchLogin_UnexpectedRedirect(object sender, Core.TwitchLoginUnexpectedRedirectEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - Unexpected Redirect event raised. URI: {e.Uri}");
        }

        private void twitchLogin_StateMismatchOccurred(object sender, Core.TwitchLoginStateMismatchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - State Mismatch event raised. ExpectedState: {e.ExpectedState} | ActualState: {e.ActualState}");
        }

        private void twitchLogin_LoggedOut(object sender, Core.TwitchLogoutEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - Logged Out event raised. OldToken: {e.OldAccessToken.Substring(0, 4)}");
            entryAuthToken.Text = string.Empty;
        }

        private async void btnClear_Clicked(object sender, EventArgs e)
        {
            var cookiesBeforeLogout = await twitchLogin.GetTwitchCookiesAsync();

            this.twitchLogin.Logout();

            var cookiesAfterLogout = await twitchLogin.GetTwitchCookiesAsync();

            string t = ";";
        }


        private async void btnInspectCookies_Clicked(object sender, EventArgs e)
        {
            var cookies = await twitchLogin.GetTwitchCookiesAsync();
        }
    }
}