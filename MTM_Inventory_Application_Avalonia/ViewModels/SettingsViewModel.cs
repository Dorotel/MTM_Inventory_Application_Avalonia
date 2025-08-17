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

    public SettingsViewModel() : this(new ExceptionHandler(), new Settings(), new SessionContext()) {}

    public SettingsViewModel(IExceptionHandler exceptionHandler, ISettings settings, ISessionContext session)
    {
        _exceptionHandler = exceptionHandler;
        _settings = settings;
        _session = session;

        Settings_Label_SessionUser = _session.UserId ?? string.Empty;
        Settings_Label_SiteId = _session.SiteId ?? string.Empty;
        Settings_Label_AppDbName = "AppDb (configured)";
        Settings_Text_WarehouseId = "002";
        Settings_Toggle_IsDevVisible = string.Equals(_settings.Environment, "Development", StringComparison.OrdinalIgnoreCase);
        Settings_Combo_EnvironmentIndex = Settings_Toggle_IsDevVisible ? 0 : 1;
    }

    [ObservableProperty] private int settings_Combo_EnvironmentIndex;
    [ObservableProperty] private string settings_Text_WarehouseId = string.Empty;
    [ObservableProperty] private string settings_Label_AppDbName = string.Empty;
    [ObservableProperty] private string settings_Label_SessionUser = string.Empty;
    [ObservableProperty] private string settings_Label_SiteId = string.Empty;
    [ObservableProperty] private bool settings_Toggle_IsDevVisible;
    [ObservableProperty] private bool settings_Toggle_DevLoginEnabled;

    [RelayCommand]
    private void Settings_Button_Save()
    {
        try
        {
            // TODO: persist to app DB / config
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(Settings_Button_Save)); }
    }

    [RelayCommand]
    private void Settings_Button_Cancel()
    {
        try { /* TODO: navigate back */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(Settings_Button_Cancel)); }
    }

    [RelayCommand]
    private void Settings_Button_TestAppDb()
    {
        try { /* TODO: ping app DB and report */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(Settings_Button_TestAppDb)); }
    }
}
