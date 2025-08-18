# SettingsView - Planning and Functional Notes

Purpose
- Display application/environment settings and allow safe modifications (non-Visual data only).

Scope
- Read-only display of WarehouseId (default "002"), Environment (Development/Production), and connection strings.
- Allow editing of app-managed settings when permitted (persist to app DB per policy).

UI/UX
- List of settings with descriptions; Save/Cancel buttons disabled when no changes.
- Validation with clear error display using ExceptionHandler dialog for critical failures.

Integration
- SettingsService (future) reads from ../../References/Visual DLL & Config Files/Database.config and environment overrides.
- Persist changes to mtm_visual_application(_test) per environment.

Testing
- Shows current settings; Save disabled if nothing changed.

References
- ../../Views/SettingsView.axaml
- ../../Views/SettingsView.axaml.cs
- ../../ViewModels/SettingsViewModel.cs
- ../../References/Visual DLL & Config Files/Database.config
