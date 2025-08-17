using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using MTM_Inventory_Application_Avalonia.Views;
using MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;
using System;

namespace MTM_Inventory_Application_Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IExceptionHandler _exceptionHandler;
    private readonly INavigationService _navigationService;
    private readonly ISessionContext _sessionContext;

    public MainViewModel()
        : this(new ExceptionHandler(), new NavigationService(), new SessionContext())
    {
    }

    public MainViewModel(IExceptionHandler exceptionHandler, INavigationService navigationService, ISessionContext sessionContext)
    {
        _exceptionHandler = exceptionHandler;
        _navigationService = navigationService;
        _sessionContext = sessionContext;

        // Initialize derived properties from session
        SessionUser = _sessionContext.UserId ?? "Unknown";
        SiteId = _sessionContext.SiteId ?? "";
        WarehouseId = _sessionContext.WarehouseId ?? "";

        // Simple role-based capability (expand as needed)
        var role = _sessionContext.Role ?? "MaterialHandler";
        CanOpenInventoryTransfer = role is "MaterialHandler" or "InventorySpecialist" or "Lead" or "ReadOnly";
        CanOpenWorkOrderTransaction = role is "MaterialHandler" or "InventorySpecialist" or "Lead" or "ReadOnly";

        // Default UI state: show login overlay until authenticated
        IsMenuOpen = true;
        IsLoginVisible = string.IsNullOrEmpty(_sessionContext.UserId);
    }

    public string Greeting => "Welcome";

    [ObservableProperty]
    private string sessionUser = string.Empty;

    [ObservableProperty]
    private string siteId = string.Empty;

    [ObservableProperty]
    private string warehouseId = string.Empty;

    [ObservableProperty]
    private bool canOpenInventoryTransfer;

    [ObservableProperty]
    private bool canOpenWorkOrderTransaction;

    // Side panel state and hosted content
    [ObservableProperty]
    private bool isMenuOpen;

    [ObservableProperty]
    private object? currentView;

    // Login overlay flag and VM used as DataContext for LoginView
    [ObservableProperty]
    private bool isLoginVisible;

    private LoginViewModel? _loginVm;
    public LoginViewModel? LoginVm
    {
        get => _loginVm;
        set => SetProperty(ref _loginVm, value);
    }

    [RelayCommand]
    private void MainView_Button_OpenInventoryTransfer()
    {
        try
        {
            CurrentView = new InventoryTransferView { DataContext = new InventoryTransferViewModel(_exceptionHandler) };
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(MainView_Button_OpenInventoryTransfer));
        }
    }

    [RelayCommand]
    private void MainView_Button_OpenWorkOrderTransaction()
    {
        try
        {
            CurrentView = new WorkOrderTransactionView { DataContext = new WorkOrderTransactionViewModel(_exceptionHandler) };
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(MainView_Button_OpenWorkOrderTransaction));
        }
    }

    [RelayCommand]
    private void MainView_Button_OpenSettings()
    {
        try
        {
            CurrentView = new SettingsView { DataContext = new SettingsViewModel(_exceptionHandler, new Settings(), _sessionContext) };
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(MainView_Button_OpenSettings));
        }
    }

    [RelayCommand]
    private void MainView_Button_Logout()
    {
        try
        {
            _sessionContext.Logout();
            // Show login overlay and request window shrink via nav service
            IsLoginVisible = true;
            _navigationService.NavigateToLogin();
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(MainView_Button_Logout));
        }
    }

    // Called by NavigationService after successful login to hide overlay
    public void OnAuthenticated()
    {
        IsLoginVisible = false;
        SessionUser = _sessionContext.UserId ?? "Unknown";
        SiteId = _sessionContext.SiteId ?? string.Empty;
        WarehouseId = _sessionContext.WarehouseId ?? string.Empty;

        // Clear the login VM reference
        LoginVm = null;
    }
}
