using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MTM_Inventory_Application_Avalonia.ViewModels;
using MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;
using MTM_Inventory_Application_Avalonia.Views;
using MTM_Inventory_Application_Avalonia.Views.Dialogs;

namespace MTM_Inventory_Application_Avalonia.Services;

public interface IExceptionHandler
{
    void Handle(System.Exception ex, string context);
}

public class ExceptionHandler : IExceptionHandler
{
    public async void Handle(System.Exception ex, string context)
    {
        // Show a simple ExceptionDialog modal with details
        try
        {
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var owner = lifetime?.MainWindow;
            var dialogWindow = new Window
            {
                Title = "Error",
                SizeToContent = SizeToContent.WidthAndHeight,
                CanResize = false,
                Content = new ExceptionDialog
                {
                    DataContext = new ExceptionDialogViewModel
                    {
                        Title = $"{context}",
                        Message = ex.Message,
                        Details = ex.ToString(),
                        IsDetailsOpen = false
                    }
                }
            };

            if (owner is not null)
            {
                await dialogWindow.ShowDialog(owner);
            }
            else
            {
                dialogWindow.Show();
            }
        }
        catch
        {
            // Fallback to debug output if UI dialog fails
            System.Diagnostics.Debug.WriteLine($"[Error] {context}: {ex}");
        }
    }
}

public interface ISettings
{
    string Environment { get; }
}

public class Settings : ISettings
{
    public string Environment { get; set; } = "Development"; // TODO: read from config/env
}

public interface ISessionContext
{
    string? UserId { get; set; }
    string? SiteId { get; set; }
    string? WarehouseId { get; set; }
    string? Role { get; set; }
    void Logout();
}

public class SessionContext : ISessionContext
{
    public string? UserId { get; set; }
    public string? SiteId { get; set; }
    public string? WarehouseId { get; set; }
    public string? Role { get; set; }

    public void Logout()
    {
        UserId = null;
        SiteId = null;
        WarehouseId = null;
        Role = null;
    }
}

public interface IAuthenticationService
{
    Task<bool> AuthenticateAsync(string username, string password, string? siteOrDomain);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ISettings _settings;
    private readonly ISessionContext _session;

    public AuthenticationService(ISettings settings, ISessionContext session)
    {
        _settings = settings;
        _session = session;
    }

    public Task<bool> AuthenticateAsync(string username, string password, string? siteOrDomain)
    {
        // Placeholder: allow Admin/Admin when Environment == Development
        if (string.Equals(_settings.Environment, "Development", System.StringComparison.OrdinalIgnoreCase)
            && username == "Admin" && password == "Admin")
        {
            _session.UserId = username;
            _session.Role = "MaterialHandler";
            _session.SiteId = siteOrDomain ?? "SITE";
            _session.WarehouseId = "002";
            return Task.FromResult(true);
        }

        // TODO: implement VISUAL-backed auth via service adapter
        return Task.FromResult(false);
    }
}

public interface IPartDialogService
{
    Task<string?> PickPartAsync(string seed);
}

public class PartDialogService : IPartDialogService
{
    public async Task<string?> PickPartAsync(string seed)
    {
        var tcs = new TaskCompletionSource<string?>();

        var vm = new IncompletePartDialogViewModel(seed);
        vm.OnSelected += partId =>
        {
            tcs.TrySetResult(partId);
            CloseHostingWindow(vm);
        };
        vm.OnCanceled += () =>
        {
            tcs.TrySetResult(null);
            CloseHostingWindow(vm);
        };

        var dialog = new IncompletePartDialog { DataContext = vm };
        var window = new Window
        {
            Title = "Find Part",
            Width = 720,
            Height = 480,
            Content = dialog
        };

        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifetime?.MainWindow is Window owner)
        {
            await window.ShowDialog(owner);
        }
        else
        {
            window.Show();
        }

        return await tcs.Task;
    }

    private static void CloseHostingWindow(object vm)
    {
        // Attempt to find the window hosting the dialog via focused window
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d && d.MainWindow is Window owner)
        {
            // If a modal dialog is open, try to close the focused window
            owner?.OwnedWindows?[owner.OwnedWindows.Count - 1]?.Close();
        }
    }
}

public interface INavigationService
{
    void Configure(Window? mainWindow);
    void OpenInventoryTransfer();
    void OpenWorkOrderTransaction();
    void NavigateToLogin();
    void NavigateToMain();
    void Exit();
}

public class NavigationService : INavigationService
{
    private Window? _mainWindow;

    // Shared service instances for consistency during navigation
    private readonly IExceptionHandler _exceptionHandler = new ExceptionHandler();
    private readonly ISettings _settings = new Settings();
    private readonly ISessionContext _session = new SessionContext();
    private IAuthenticationService? _auth;
    private readonly IPartDialogService _partDialog = new PartDialogService();

    private MainView? _mainView;
    private MainViewModel? _mainVm;

    public void Configure(Window? mainWindow)
    {
        _mainWindow = mainWindow;
        _auth = new AuthenticationService(_settings, _session);
    }

    private void EnsureMainView()
    {
        if (_mainWindow is null) return;

        if (_mainWindow.Content is MainView existing)
        {
            _mainView = existing;
            _mainVm = existing.DataContext as MainViewModel;
            if (_mainVm is null)
            {
                _mainVm = new MainViewModel(_exceptionHandler, this, _session);
                existing.DataContext = _mainVm;
            }
            return;
        }

        _mainView = new MainView();
        _mainVm = new MainViewModel(_exceptionHandler, this, _session);
        _mainView.DataContext = _mainVm;
        _mainWindow.Content = _mainView;
    }

    public void OpenInventoryTransfer()
    {
        // Keep navigation inside the MainView content host when possible
        EnsureMainView();
        if (_mainVm is not null)
        {
            _mainVm.CurrentView = new InventoryTransferView
            {
                DataContext = new InventoryTransferViewModel(_exceptionHandler)
                {
                    PartDialog = _partDialog
                }
            };
        }
    }

    public void OpenWorkOrderTransaction()
    {
        EnsureMainView();
        if (_mainVm is not null)
        {
            _mainVm.CurrentView = new WorkOrderTransactionView
            {
                DataContext = new WorkOrderTransactionViewModel(_exceptionHandler)
                {
                    PartDialog = _partDialog
                }
            };
        }
    }

    public void NavigateToLogin()
    {
        EnsureMainView();
        if (_mainVm is null) return;
        if (_auth is null) _auth = new AuthenticationService(_settings, _session);

        // Show login overlay inside MainView and let MainView manage window sizing
        _mainVm.LoginVm = new LoginViewModel(_exceptionHandler, this, _settings, _session, _auth);
        _mainVm.IsLoginVisible = true;
    }

    public void NavigateToMain()
    {
        EnsureMainView();
        if (_mainVm is null) return;

        // Hide overlay and update session-bound UI; MainView will restore natural size
        _mainVm.OnAuthenticated();
    }

    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
        {
            d.Shutdown();
        }
    }
}
