using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media;

namespace MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

public enum ErrorType
{
    Info,
    Warning,
    Error,
    Critical
}

public partial class ExceptionDialogViewModel : ObservableObject
{
    [ObservableProperty] private string title = "Error";
    [ObservableProperty] private string message = string.Empty;
    [ObservableProperty] private string details = string.Empty;
    [ObservableProperty] private bool isDetailsOpen;
    // Brief error title for expander header (placeholder default)
    [ObservableProperty] private string shortTitle = "I/O error";

    // Error type determining the icon visuals; notify dependent computed properties
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IconBackground))]
    [NotifyPropertyChangedFor(nameof(IconBrush))]
    [NotifyPropertyChangedFor(nameof(IconKindName))]
    private ErrorType errorType = ErrorType.Error;

    // Background color for legacy circular badge (kept for potential reuse)
    public IBrush IconBackground => errorType switch
    {
        ErrorType.Info => new SolidColorBrush(Color.Parse("#2563EB")),   // Blue
        ErrorType.Warning => new SolidColorBrush(Color.Parse("#F59E0B")),// Amber
        ErrorType.Error => new SolidColorBrush(Color.Parse("#DC2626")),  // Red
        ErrorType.Critical => new SolidColorBrush(Color.Parse("#7F1D1D")),// Dark Red
        _ => new SolidColorBrush(Color.Parse("#DC2626"))
    };

    // Foreground color for actual vector icon
    public IBrush IconBrush => IconBackground;

    // Projektanker MaterialDesign icon identifiers
    public string IconKindName => errorType switch
    {
        ErrorType.Info => "mdi:information-outline",
        ErrorType.Warning => "mdi:alert-outline",
        ErrorType.Error => "mdi:alert-circle",
        ErrorType.Critical => "mdi:alert-octagon",
        _ => "mdi:alert-circle"
    };

    [RelayCommand]
    private void ExceptionDialog_Button_Retry()
    {
        // Cycle through error types to test icons/colors
        var next = ErrorType switch
        {
            ErrorType.Info => ErrorType.Warning,
            ErrorType.Warning => ErrorType.Error,
            ErrorType.Error => ErrorType.Critical,
            ErrorType.Critical => ErrorType.Info,
            _ => ErrorType.Info
        };

        ErrorType = next;
        Title = next.ToString();
        ShortTitle = $"{next} details";
    }

    [RelayCommand] private void ExceptionDialog_Button_Cancel() { }
    [RelayCommand] private void ExceptionDialog_Button_Help() { }
    [RelayCommand] private void ExceptionDialog_Button_CopyDetails() { }
}
