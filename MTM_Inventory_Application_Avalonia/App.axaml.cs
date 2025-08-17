using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using MTM_Inventory_Application_Avalonia.ViewModels;
using MTM_Inventory_Application_Avalonia.Views;
using MTM_Inventory_Application_Avalonia.Services;

namespace MTM_Inventory_Application_Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var nav = new NavigationService();
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;
            nav.Configure(mainWindow);

            // Start with login overlay hosted inside MainView in a small frame
            nav.NavigateToLogin();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
