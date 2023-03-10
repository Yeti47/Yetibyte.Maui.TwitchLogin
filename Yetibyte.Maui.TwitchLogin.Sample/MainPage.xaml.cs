using Microsoft.Maui.Controls;
using Yetibyte.Maui.TwitchLogin.Services;

namespace Yetibyte.Maui.TwitchLogin.Sample
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
            await this.twitchLogin.LogoutAsync();
        }

    }
}