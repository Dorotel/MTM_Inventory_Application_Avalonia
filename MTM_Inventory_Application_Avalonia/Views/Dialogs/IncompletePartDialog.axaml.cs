using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

namespace MTM_Inventory_Application_Avalonia.Views.Dialogs;

public partial class IncompletePartDialog : UserControl
{
    public IncompletePartDialog()
    {
        InitializeComponent();

        // Provide placeholder data when in design mode or when no DataContext is supplied by the host.
        if (Design.IsDesignMode && DataContext is null)
        {
            DataContext = new IncompletePartDialogViewModel("21-28841-");
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnResultsDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is { } dc)
        {
            var type = dc.GetType();
            var cmdProp = type.GetProperty("IncompletePartDialog_Button_SelectCommand");
            var cmd = cmdProp?.GetValue(dc) as System.Windows.Input.ICommand;
            if (cmd?.CanExecute(null) == true)
            {
                cmd.Execute(null);
            }
        }
    }
}
