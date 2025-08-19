using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using MTM_Inventory_Application_Avalonia.Views;
using MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;
using MTM_Inventory_Application_Avalonia.Views.Dialogs;
using System;
using Avalonia.Controls;

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

        SessionUser = _sessionContext.UserId ?? "Unknown";
        SiteId = _sessionContext.SiteId ?? "";
        WarehouseId = _sessionContext.WarehouseId ?? "";

        var role = _sessionContext.Role ?? "MaterialHandler";
        CanOpenInventoryTransfer = role is "MaterialHandler" or "InventorySpecialist" or "Lead" or "ReadOnly";
        CanOpenWorkOrderTransaction = role is "MaterialHandler" or "InventorySpecialist" or "Lead" or "ReadOnly";

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

    [ObservableProperty]
    private bool isMenuOpen;

    [ObservableProperty]
    private object? currentView;

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
            IsLoginVisible = true;
            _navigationService.NavigateToLogin();
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(MainView_Button_Logout));
        }
    }

    [RelayCommand]
    private void MainView_Button_TestExceptionDialog()
    {
        try
        {
            var win = new Window
            {
                Title = "Exception Dialog (Test)",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new ExceptionDialog { DataContext = new ExceptionDialogViewModel() }
            };
            win.Show();
        }
        catch (Exception ex)
        {
            _exceptionHandler.Handle(ex, nameof(MainView_Button_TestExceptionDialog));
        }
    }

    public void OnAuthenticated()
    {
        IsLoginVisible = false;
        SessionUser = _sessionContext.UserId ?? "Unknown";
        SiteId = _sessionContext.SiteId ?? string.Empty;
        WarehouseId = _sessionContext.WarehouseId ?? string.Empty;

        LoginVm = null;
    }
}
