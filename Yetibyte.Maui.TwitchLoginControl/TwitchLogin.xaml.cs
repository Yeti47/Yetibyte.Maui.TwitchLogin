using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Net;
using Yetibyte.Maui.TwitchLoginControl.Core;
using Yetibyte.Maui.TwitchLoginControl.Services;
using Yetibyte.Maui.TwitchLoginControl.Services.LoginSessions;
using Yetibyte.Maui.TwitchLoginControl.ViewModels;

namespace Yetibyte.Maui.TwitchLoginControl;

public partial class TwitchLogin : ContentView
{
    #region Bindable Props

    public static readonly BindableProperty ClientIdProperty =
        BindableProperty.Create(nameof(ClientId), typeof(string), typeof(TwitchLogin), string.Empty, propertyChanged: (o, oldVal, newVal) => ((TwitchLogin)o).ViewModel.ClientId = (string)newVal);

    public static readonly BindableProperty RedirectUriProperty =
        BindableProperty.Create(nameof(RedirectUri), typeof(string), typeof(TwitchLogin), TwitchLoginNavigator.DEFAULT_REDIRECT_URI, propertyChanged: (o, oldVal, newVal) => ((TwitchLogin)o).ViewModel.RedirectUri = new Uri(newVal.ToString()));

    #endregion

    #region Fields

    private readonly ITwitchLoginSessionManager _sessionManager;

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

    public event EventHandler<TwitchLogoutEventArgs> LoggedOut;

    #endregion

    #region Ctors

    public TwitchLogin(ITwitchLoginSessionManager twitchLoginSessionManager)
	{
		InitializeComponent();

        _sessionManager = twitchLoginSessionManager;

        ViewModel = new TwitchLoginViewModel(_sessionManager);
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        
		BindingContext = ViewModel;

        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
	}

    public TwitchLogin() : this(new TwitchLoginSessionManager(new CookieManager(null)))
    {
    
    }

    #endregion

    #region Methods

    public void Logout()
    {
        _sessionManager.EndSession();
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