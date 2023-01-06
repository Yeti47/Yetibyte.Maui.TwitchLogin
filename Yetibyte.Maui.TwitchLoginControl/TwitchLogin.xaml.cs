using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Net;
using Yetibyte.Maui.TwitchLoginControl.Core;
using Yetibyte.Maui.TwitchLoginControl.Services;
using Yetibyte.Maui.TwitchLoginControl.ViewModels;

namespace Yetibyte.Maui.TwitchLoginControl;

public partial class TwitchLogin : ContentView
{
    #region Bindable Props

    public static readonly BindableProperty ClientIdProperty =
        BindableProperty.Create(nameof(ClientId), typeof(string), typeof(TwitchLogin), string.Empty, propertyChanged: (o, oldVal, newVal) => ((TwitchLogin)o).ViewModel.ClientId = (string)newVal);

    public static readonly BindableProperty RedirectUriProperty =
        BindableProperty.Create(nameof(RedirectUri), typeof(string), typeof(TwitchLogin), TwitchLoginViewModel.DEFAULT_REDIRECT_URI, propertyChanged: (o, oldVal, newVal) => ((TwitchLogin)o).ViewModel.RedirectUri = new Uri(newVal.ToString()));

    #endregion

    #region Fields

    private readonly ICookieManager _cookieManager;

    #endregion

    #region Props

    public string ClientId
    {
        get => (string)GetValue(ClientIdProperty);
        set => SetValue(ClientIdProperty, value);
    }

    public string RedirectUri
    {
        get => (string)GetValue(RedirectUriProperty);
        set => SetValue(RedirectUriProperty, value);
    }

    public bool IsLoggedIn => ViewModel.IsLoggedIn;

    public string AccessToken => ViewModel.AccessToken;

    internal TwitchLoginViewModel ViewModel { get; private set; }

    #endregion

    #region Events

    public event EventHandler<TwitchLoginSucceededEventArgs> LoginSucceeded
    {
        add => this.ViewModel.LoginSucceeded += value;
        remove => this.ViewModel.LoginSucceeded -= value;
    }

    public event EventHandler<TwitchLoginFailedEventArgs> LoginFailed
    {
        add => this.ViewModel.LoginFailed += value;
        remove => this.ViewModel.LoginFailed -= value;
    }

    public event EventHandler<TwitchLoginUnexpectedRedirectEventArgs> UnexpectedRedirect
    {
        add => this.ViewModel.UnexpectedRedirect += value;
        remove => this.ViewModel.UnexpectedRedirect -= value;
    }
    public event EventHandler<TwitchLoginStateMismatchEventArgs> StateMismatchOccurred
    {
        add => this.ViewModel.StateMismatchOccurred += value;
        remove => this.ViewModel.StateMismatchOccurred -= value;
    }

    public event EventHandler<TwitchLogoutEventArgs> LoggedOut
    {
        add => this.ViewModel.LoggedOut += value;
        remove => this.ViewModel.LoggedOut -= value;
    }

    #endregion

    #region Ctors

    public TwitchLogin()
	{
		InitializeComponent();

        _cookieManager = new CookieManager(webViewLogin);

        ViewModel = new TwitchLoginViewModel(_cookieManager);
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        
		BindingContext = ViewModel;

	}

    #endregion

    #region Methods

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsLoggedIn))
        {
            OnPropertyChanged(nameof(IsLoggedIn));
        }
        else if (e.PropertyName == nameof(ViewModel.AccessToken))
        {
            OnPropertyChanged(nameof(AccessToken));
        }
    }

    private void webViewLogin_Navigating(object sender, WebNavigatingEventArgs e)
    {
        Uri targetUri = new Uri(e.Url);
        
        ViewModel.ProcessLoginResponse(targetUri);
    }

    public void Logout()
    {
        ViewModel.Logout();
        webViewLogin.Source = ViewModel.AuthenticationUrl;
    }

    public async Task<IEnumerable<CookieManager.Cookie>> GetTwitchCookiesAsync() => await ViewModel.GetTwitchCookiesAsync();

    #endregion
}