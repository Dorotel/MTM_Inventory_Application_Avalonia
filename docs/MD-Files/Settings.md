# Settings View - Configuration Management
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `Views/SettingsView.axaml`
- **ViewModel:** `ViewModels/SettingsViewModel.cs`
- **Primary Commands:** Save, Cancel
- **Related Services:** ISettings, ISessionContext, IExceptionHandler
- **Last Updated:** 2024-12-19
- **Copilot Template Version:** 1.1

---

## Purpose

In-app settings configuration for warehouse ID and other application-specific settings. Provides interface for managing runtime configuration without requiring Visual server connections.

---

## Global Rules

- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.
- ALL methods implement try/catch and route errors through IExceptionHandler with the same normalization routine

---

## Scope

- Manage app-owned settings and runtime toggles safe to configure without contacting VISUAL
- Display warehouse ID configuration for inventory transactions
- Provide Save/Cancel operations for settings persistence
- Does not initiate Visual server connections from this view

---

## Platform and Shell Wiring

- **View:** `Views/SettingsView.axaml` (UserControl)
- **Host:** Displayed in MainView content area via CurrentView binding
- **Navigation:** Created by MainViewModel.MainView_Button_OpenSettingsCommand
- **Data Context:** SettingsViewModel with ISettings, ISessionContext dependencies

---

## Current Implementation Details

**SettingsView.axaml Structure:**
- Header with cog-outline icon and "Settings" title
- **Transaction Settings** panel (expandable, default expanded):
  - "Set Warehouse ID" field with TextBox input
- Action buttons: Save and Cancel with appropriate icons

**Key UI Features:**
- Expandable panel for organized settings groups
- Consistent styling with other application forms
- Icon-based action buttons for clear user intent

---

## Fields and Validation Rules

**Configurable Fields:**
- `Settings_Text_WarehouseId` (string) - Default warehouse for inventory transactions
  - Initialized from ISessionContext.WarehouseId or defaults to "002"
  - Used by InventoryTransfer and WorkOrderTransaction screens

**Planned Fields (not currently implemented):**
- Environment (Development|Production) - affects login behaviors
- Development login toggle - enables Admin/Admin bypass
- AppDb connection display and test functionality
- Session context display (UserId, SiteId)

---

## Visual API Commands (by scenario)

**Current Implementation:**
- No Visual API calls initiated from Settings view
- All Visual context read from existing ISessionContext populated by Login

**Planned Integrations:**
- AppDb connectivity testing (non-Visual)
- Configuration persistence to app-owned database

---

## Business Rules and Exceptions

- Warehouse ID changes take effect immediately upon Save
- Cancel operation reverts UI to current session values
- All errors route through centralized Exception Handling Form
- No Visual server connections required for current settings

---

## Workflows

1. **Open Settings:** Click Settings button from MainView → SettingsView loads in content area
2. **Edit Warehouse:** Modify warehouse ID in text field
3. **Save Changes:** Click Save → persist to session context → success feedback
4. **Cancel Changes:** Click Cancel → revert UI to original values
5. **Navigation:** Settings view remains open until user navigates to another feature

---

## ViewModel/Command Conventions

**SettingsViewModel Constructor:**
- Default: Creates service instances for testing
- Injected: Accepts IExceptionHandler, ISettings, ISessionContext

**Properties:**
- `Settings_Text_WarehouseId` - warehouse ID field with ObservableProperty

**Commands:**
- `Settings_Button_SaveCommand` - persists warehouse ID to session context
- `Settings_Button_CancelCommand` - reverts UI to current session values

**Service Integration:**
- ISessionContext for warehouse ID persistence
- ISettings for future configuration expansion
- IExceptionHandler for error routing

---

## Integration & Storage

**Services Used:**
- ISessionContext: warehouse ID storage and retrieval
- ISettings: configuration interface (future expansion)
- IExceptionHandler: centralized error handling

**Data Persistence:**
- Current: In-memory session context only
- Planned: App-owned database settings table
- Configuration: Environment variable overrides supported

---

## Keyboard/Scanner UX

**Current Implementation:**
- Standard tab navigation between fields
- Enter key likely submits form (framework default)

**Planned Enhancements:**
- Keyboard shortcuts for Save (Ctrl+S) and Cancel (Esc)
- Direct warehouse ID barcode scanning

---

## UI Scaffold

**Views:**
- `Views/SettingsView.axaml` - settings configuration form
- Expandable Transaction Settings panel
- Save/Cancel action buttons with icons

**ViewModels:**
- `ViewModels/SettingsViewModel.cs` - settings management logic
- Inherits from ObservableObject (CommunityToolkit.Mvvm)
- Simple property binding and command pattern

**Services:**
- Settings service for configuration persistence
- SessionContext for runtime state management
- ExceptionHandler for error display

---

## Testing & Acceptance

- Settings view loads with current warehouse ID from session
- Warehouse ID changes are reflected in text field
- Save button persists changes to session context
- Cancel button reverts changes to original values
- Navigation back to main features uses updated warehouse ID
- All errors display through Exception Handling Form

---

## References

- ../../Views/SettingsView.axaml (UI implementation)
- ../../ViewModels/SettingsViewModel.cs (settings logic)
- ../../Services/Service_Settings.cs (configuration service)
- ./MAMPDatabase.md (future database persistence)
- ./ExceptionHandling.md (error handling flow)

---

## Implementation Status

- **Current:** Basic warehouse ID configuration implemented
- **Save/Cancel:** Working with session context persistence
- **UI:** Complete with expandable panels and action buttons
- **Navigation:** Integrated with MainView navigation system

---

## TODOs / Copilot Agent Notes

- [ ] Add environment selection (Development/Production)
- [ ] Implement app database connectivity testing
- [ ] Add development login toggle (Development mode only)
- [ ] Add session context display (UserId, SiteId)
- [ ] Implement persistent storage to app-owned database
- [ ] Add keyboard shortcuts (Ctrl+S for Save, Esc for Cancel)
- [ ] Consider adding configuration import/export functionality