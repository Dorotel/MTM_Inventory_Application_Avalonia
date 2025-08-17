using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

public class LocationBalanceDto
{
    public string Location { get; set; } = string.Empty;
    public decimal OnHand { get; set; }
    public decimal Allocated { get; set; }
    public decimal Available { get; set; }
    public string Warehouse { get; set; } = string.Empty;
    public string Site { get; set; } = string.Empty;
    public string BinType { get; set; } = string.Empty;
}

public partial class LocationPickerDialogViewModel : ObservableObject
{
    [ObservableProperty] private string headerText = string.Empty;

    // Filters
    [ObservableProperty] private string filters_Text = string.Empty;
    [ObservableProperty] private bool filters_NonZeroOnly;
    [ObservableProperty] private bool filters_CurrentWarehouseOnly = true;
    [ObservableProperty] private bool filters_CurrentSiteOnly = true;

    [ObservableProperty] private ObservableCollection<LocationBalanceDto> results = new();
    [ObservableProperty] private LocationBalanceDto? selectedRow;

    [RelayCommand] private void LocationPickerDialog_Button_Select() { }
    [RelayCommand] private void LocationPickerDialog_Button_Cancel() { }
}
