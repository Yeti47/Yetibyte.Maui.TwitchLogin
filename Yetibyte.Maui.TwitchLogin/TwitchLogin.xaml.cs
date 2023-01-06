using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Net;
using Yetibyte.Maui.TwitchLogin.Core;
using Yetibyte.Maui.TwitchLogin.Services;
using Yetibyte.Maui.TwitchLogin.Services.LoginSessions;
using Yetibyte.Maui.TwitchLogin.ViewModels;

namespace Yetibyte.Maui.TwitchLogin;

public partial class TwitchLogin : ContentView
{
    #region Bindable Props

    public static readonly BindableProperty ClientIdProperty =
        BindableProperty.Create(nameof(ClientId), typeof(string), typeof(TwitchLogin), string.Empty, propertyChanged: (o, oldVal, newVal) => ((TwitchLogin)o).ViewModel.ClientId = (string)newVal);

    public static readonly BindableProperty RedirectUriProperty =
        BindableProperty.Create(nameof(RedirectUri), typeof(string), typeof(TwitchLogin), TwitchLoginNavigator.DEFAULT_REDIRECT_URI, propertyChanged: (o, oldVal, newVal) => ((TwitchLogin)o).ViewModel.RedirectUri = new Uri(newVal.ToString()));

    public static readonly BindableProperty RedirectHtmlWebSourceProperty =
        BindableProperty.Create(nameof(RedirectHtmlWebSource), typeof(HtmlWebViewSource), typeof(TwitchLogin), propertyChanged: (o, oldVal, newVal) => ((TwitchLogin)o).ViewModel.UseCustomRedirectSource = newVal is not null);


    #endregion

    #region Fields

    private readonly ITwitchLoginSessionManager _sessionManager;

    #endregion

    #region Props

    public HtmlWebViewSource RedirectHtmlWebSource
    {
        get => (HtmlWebViewSource)GetValue(RedirectHtmlWebSourceProperty);
        set => SetValue(RedirectHtmlWebSourceProperty, value);
    }

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

    public event EventHandler<TwitchLogoutEventArgs> LoggedOut;

    #endregion

    #region Ctors

    public TwitchLogin()
	{
        InitializeComponent();
        _sessionManager = Yetibyte.Maui.TwitchLogin.Services.ServiceProvider.GetService<ITwitchLoginSessionManager>();

        ViewModel = new TwitchLoginViewModel(_sessionManager);
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        ViewModel.LoginSucceeded += ViewModel_LoginSucceeded;
        
		BindingContext = ViewModel;

        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
	}

    #endregion

    private void ViewModel_LoginSucceeded(object sender, TwitchLoginSucceededEventArgs e)
    {
        if (RedirectHtmlWebSource is not null)
        {
            webViewLogin.Source = RedirectHtmlWebSource;
        }
    }

    #region Methods

    public async Task LogoutAsync()
    {
        await _sessionManager.EndSessionAsync(); 
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        _sessionManager.SessionEnded += _sessionManager_SessionEnded;
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        _sessionManager.SessionEnded -= _sessionManager_SessionEnded;
    }

    private void _sessionManager_SessionEnded(object sender, TwitchLoginSessionEventArgs e)
    {
        string oldAccessToken = ViewModel.AccessToken;

        ViewModel.AccessToken = string.Empty;
        webViewLogin.Source = ViewModel.AuthenticationUrl;

        var loggedOutHandler = LoggedOut;
        loggedOutHandler?.Invoke(this, new TwitchLogoutEventArgs(oldAccessToken));
    }

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
        
        ViewModel.ProcessNavigationUri(targetUri);
    }

#endregion
}