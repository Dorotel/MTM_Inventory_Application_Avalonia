using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

namespace MTM_Inventory_Application_Avalonia.Views.Dialogs;

public partial class IncompletePartDialog : UserControl
{
    public IncompletePartDialog()
    {
        InitializeComponent();

        if (!Design.IsDesignMode && DataContext is null)
        {
            // Seed like the design-time context
            DataContext = new IncompletePartDialogViewModel("21-28841-");
        }
    }

    private void OnResultsDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is IncompletePartDialogViewModel vm &&
            vm.IncompletePartDialog_Button_SelectCommand.CanExecute(null))
        {
            vm.IncompletePartDialog_Button_SelectCommand.Execute(null);
        }
    }
}
