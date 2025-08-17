using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MTM_Inventory_Application_Avalonia.Views.Dialogs;

public partial class LocationPickerDialog : UserControl
{
    public LocationPickerDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
