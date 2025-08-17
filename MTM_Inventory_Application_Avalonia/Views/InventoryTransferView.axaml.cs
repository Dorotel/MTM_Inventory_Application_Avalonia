using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MTM_Inventory_Application_Avalonia.Views;

public partial class InventoryTransferView : UserControl
{
    public InventoryTransferView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
