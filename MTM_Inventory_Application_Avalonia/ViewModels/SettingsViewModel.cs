using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using System;

namespace MTM_Inventory_Application_Avalonia.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IExceptionHandler _exceptionHandler;
    private readonly ISettings _settings;
    private readonly ISessionContext _session;

    public SettingsViewModel() : this(new ExceptionHandler(), new Settings(), new SessionContext()) { }

    public SettingsViewModel(IExceptionHandler exceptionHandler, ISettings settings, ISessionContext session)
    {
        _exceptionHandler = exceptionHandler;
        _settings = settings;
        _session = session;

        // Initialize from current session (fallback default)
        Settings_Text_WarehouseId = string.IsNullOrWhiteSpace(_session.WarehouseId) ? "002" : _session.WarehouseId!;
    }

    // Only field used by SettingsView
    [ObservableProperty] private string settings_Text_WarehouseId = string.Empty;

    [RelayCommand]
    private void Settings_Button_Save()
    {
        try
        {
            // Persist to session (and optionally to ISettings/config if added later)
            _session.WarehouseId = Settings_Text_WarehouseId?.Trim();
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(Settings_Button_Save)); }
    }

    [RelayCommand]
    private void Settings_Button_Cancel()
    {
        try
        {
            // Revert UI to current session value
            Settings_Text_WarehouseId = _session.WarehouseId ?? Settings_Text_WarehouseId;
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(Settings_Button_Cancel)); }
    }
}
