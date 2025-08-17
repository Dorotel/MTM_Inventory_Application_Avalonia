using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

public class PartSearchResult
{
    public string PartId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}

public partial class IncompletePartDialogViewModel : ObservableObject
{
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<PartSearchResult> results = new();
    [ObservableProperty] private PartSearchResult? selectedPart;

    [RelayCommand] private void IncompletePartDialog_Button_Select() { }
    [RelayCommand] private void IncompletePartDialog_Button_Cancel() { }
}
