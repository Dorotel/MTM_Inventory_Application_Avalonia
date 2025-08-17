using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MTM_Inventory_Application_Avalonia.Views.Dialogs;

public partial class IncompletePartDialog : UserControl
{
    public IncompletePartDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
