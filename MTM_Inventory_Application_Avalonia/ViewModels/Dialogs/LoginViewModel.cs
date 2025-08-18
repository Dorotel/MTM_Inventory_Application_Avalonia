using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using System;
using System.Threading.Tasks;

namespace MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

public partial class LoginViewModel : ObservableObject
{
    private readonly IExceptionHandler _exceptionHandler;
    private readonly INavigationService _navigationService;
    private readonly ISettings _settings;
    private readonly ISessionContext _session;
    private readonly IAuthenticationService _auth;

    public LoginViewModel()
        : this(new ExceptionHandler(), new NavigationService(), new Settings(), new SessionContext(), new AuthenticationService(new Settings(), new SessionContext()))
    {
    }

    public LoginViewModel(IExceptionHandler exceptionHandler, INavigationService navigationService, ISettings settings, ISessionContext session, IAuthenticationService auth)
    {
        _exceptionHandler = exceptionHandler;
        _navigationService = navigationService;
        _settings = settings;
        _session = session;
        _auth = auth;
    }

    private string _username = string.Empty;
    public string LoginViewModel_TextBox_Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password = string.Empty;
    public string LoginViewModel_TextBox_Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string? _siteOrDomain;
    public string? LoginViewModel_TextBox_SiteOrDomain
    {
        get => _siteOrDomain;
        set => SetProperty(ref _siteOrDomain, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    [RelayCommand]
    private async Task LoginViewModel_Button_Login()
    {
        try
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(LoginViewModel_TextBox_Username))
                throw new InvalidOperationException("Username is required.");
            if (string.IsNullOrWhiteSpace(LoginViewModel_TextBox_Password))
                throw new InvalidOperationException("Password is required.");   

            var ok = await _auth.AuthenticateAsync(LoginViewModel_TextBox_Username, LoginViewModel_TextBox_Password, LoginViewModel_TextBox_SiteOrDomain);
            if (ok)
            {
                _navigationService.NavigateToMain();
            }
            else
            {
                throw new InvalidOperationException("Login failed. Check credentials.");
            }
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(LoginViewModel_Button_Login));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void LoginViewModel_Button_Cancel()
    {
        try
        {
            _navigationService.Exit();
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(LoginViewModel_Button_Cancel));
        }
    }
}
