using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MTM_Inventory_Application_Avalonia.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
