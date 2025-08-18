# Settings View - UI Planning Specification [Ref: ../../README.md; ./MAMPDatabase.md; ../MVVM Definitions/MainView.md]

Purpose: define an in-app Settings UI for environment and app-owned configuration (warehouse, environment mode, non-Visual DB connection), with placeholders for any VISUAL-dependent reads.

Global Rule - Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Scope
- Manage app-owned settings and runtime toggles that are safe to configure without contacting VISUAL.
- Display read-only VISUAL context (e.g., SiteId) when available via session; do not open VISUAL connections from this View.
- Note: UOM is not used in production. No UOM-related settings or displays are present anywhere in the app.

Platform and Shell wiring
- View: Views/SettingsView.axaml (UserControl). DataContext: SettingsViewModel.
- Navigation: open from MainView (menu/toolbar) or dedicated button (planned via INavigationService).

## Code Naming and Error Handling Rules
- File naming: {Type}_{Parent}_{Name}
- Methods: {Class}_{ControlType}_{Name}
- Variables: {Method}_{Type(Int|string|exc)}_{Name}
- ALL methods have try/catch and route errors via IExceptionHandler.

## Screenshot highlighting (reference only)
- None. Use standard app styling.

## Fields and Validation Rules
- Environment (Development|Production): affects behaviors like Development login and connection selection.
  - Maps to ISettings.Environment and optionally to an app-owned setting for persistence.
  - Changing this value prompts for an application restart to take full effect.
- WarehouseId (string, required): app-owned default warehouse (e.g., "002").
  - Persist to app DB (see MAMPDatabase.md app_settings) or configuration.
  - Used by InventoryTransfer and WorkOrderTransaction screens as context.
- AppDb Connection (read-only text + Test button): shows effective app DB connection name.
  - Test triggers a non-VISUAL connectivity check to the app DB (placeholder service call; result shown via Exception Handling service/toast).
- Session (read-only): shows current UserId and SiteId from ISessionContext if authenticated.
- Development login enabled (toggle, Development only): when enabled, allows Admin/Admin dev login shortcut for testing.
  - Reads current environment to decide visibility.

Environment/Configuration keys (from README)
- Environment = Development | Production
- Optional environment overrides (examples):
  - INVENTORY__ENVIRONMENT
  - INVENTORY__CONNECTIONSTRINGS__APPDB
  - INVENTORY__CONNECTIONSTRINGS__APPDBTEST
  - INVENTORY__WAREHOUSEID

## Visual API Commands (by scenario)
- None initiated from this View. Do not call VISUAL directly here; any VISUAL context shown is read from existing ISessionContext populated by Login.

## Business Rules and Exceptions
- Changing Environment requires restart; ask user to restart now or later.
- WarehouseId updates take effect on next start or when services reload settings.
- Dev login toggle is visible only when Environment = Development; ignored in Production.
- All errors route via Exception Handling Form.

## Workflows
- Open Settings ? edit WarehouseId (and Dev toggle if in Development) ? Save ? success toast; prompt for restart if Environment changed.
- Test AppDb connection ? show pass/fail via Exception Handling service/toast.
- Cancel ? close without saving (no changes persisted).

## ViewModel, Commands, and Role Gating
- SettingsViewModel properties
  - Settings_Combo_EnvironmentIndex (int)
  - Settings_Text_WarehouseId (string)
  - Settings_Label_AppDbName (string)
  - Settings_Label_SessionUser (string)
  - Settings_Label_SiteId (string)
  - Settings_Toggle_IsDevVisible (bool)
  - Settings_Toggle_DevLoginEnabled (bool)
- Commands
  - Settings_Button_Save - persists changes (placeholder); prompts restart if Environment changed.
  - Settings_Button_Cancel - discards changes.
  - Settings_Button_TestAppDb - tests app DB connectivity (placeholder) and reports result.
- Role gating
  - Only users with appropriate role (e.g., Lead/Admin) can change settings; others read-only.

## Integration Approach
- ISettings exposes current Environment and can read from env vars/config; WarehouseId persisted to app DB via IAppDb (planned) or config fallback.
- Use IExceptionHandler for all error paths; IClock for timestamps if logging tests.
- INavigationService to provide entry point from MainView (planned method: OpenSettings()).

## Local Storage and Reporting
- app_settings table (see MAMPDatabase.md) stores WarehouseId and optional DevLoginEnabled flag.
- Optionally log Settings changes to a local audit table with user/time/context.

## Keyboard and Scanner UX
- Enter = Save; Esc = Cancel.

## UI Scaffolding (Avalonia 11)
- Views
  - Views/SettingsView.axaml
- ViewModels
  - ViewModels/SettingsViewModel.cs with properties/commands above.
- Services and DI
  - ISettings, IExceptionHandler, IAppDb (planned), IClock, ISessionContext.

## Testing and Acceptance Criteria
- WarehouseId saves to app DB/config and is reflected when reopening Settings.
- Environment shows current value and prompts on change; restart prompt works.
- Dev login toggle is only visible in Development; hidden in Production.
- Test AppDb shows pass/fail with friendly message via Exception Handling.
- No UOM references anywhere in the app.

## References
- ../../README.md (Configuration; Development login; app start; placeholder rules; environment overrides)
- ./MAMPDatabase.md (app_settings storage)
- ../MVVM Definitions/MainView.md (navigation entry point)

## Implementation status (scaffold)
- View created: ../../Views/SettingsView.axaml
- ViewModel created: ../../ViewModels/SettingsViewModel.cs
- Navigation entry from MainView via INavigationService is planned (OpenSettings()).
- All persistence and connectivity actions are placeholders pending service implementation.
