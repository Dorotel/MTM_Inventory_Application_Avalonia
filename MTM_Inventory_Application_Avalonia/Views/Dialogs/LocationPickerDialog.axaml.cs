using Avalonia;
using Avalonia.Controls;
using MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

namespace MTM_Inventory_Application_Avalonia.Views.Dialogs;

public partial class LocationPickerDialog : UserControl
{
    public LocationPickerDialog()
    {
        InitializeComponent();

        if (!Design.IsDesignMode && DataContext is null)
        {
            DataContext = new LocationPickerDialogViewModel();
        }
    }
}
