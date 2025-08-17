using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

public partial class ExceptionDialogViewModel : ObservableObject
{
    [ObservableProperty] private string title = "Error";
    [ObservableProperty] private string message = string.Empty;
    [ObservableProperty] private string details = string.Empty;
    [ObservableProperty] private bool isDetailsOpen;

    [RelayCommand] private void ExceptionDialog_Button_Retry() { }
    [RelayCommand] private void ExceptionDialog_Button_Cancel() { }
    [RelayCommand] private void ExceptionDialog_Button_Help() { }
    [RelayCommand] private void ExceptionDialog_Button_CopyDetails() { }
}
