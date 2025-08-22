# MainView - Shell and Navigation Hub
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `Views/MainView.axaml`
- **ViewModel:** `ViewModels/MainViewModel.cs`
- **Primary Commands:** OpenInventoryTransfer, OpenWorkOrderTransaction, OpenSettings, Logout, TestExceptionDialog
- **Related Dialogs:** LoginView overlay
- **Last Updated:** 2024-12-19
- **Copilot Template Version:** 1.1

---

## Purpose

Central hub hosting feature views after successful authentication. Shows login overlay first, then provides navigation to Inventory Transfer, Work Order Transaction, and Settings features.

---

## Global Rules

- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.
- ALL methods implement try/catch and route errors through IExceptionHandler with the same normalization routine

---

## Scope

- Acts as the hub after Login: opens Inventory Transfer, Work Order Transaction, and Settings UI flows
- Shows session context (user, site, warehouse) and provides Logout
- Hosts a Login overlay until authentication succeeds, then resizes the window
- Does not call VISUAL APIs directly; delegates to feature-specific views/services

---

## Platform and Shell Wiring

- **View:** `Views/MainView.axaml` (UserControl) with DataContext: MainViewModel
- **Host:** MainWindow displays MainView as content at startup
- **Login Overlay:** `Views/Dialogs/LoginView` centered inside MainView while unauthenticated
- **Window Sizing:** MainView code-behind toggles parent Window.SizeToContent to match login overlay

---

## Current Implementation Details

**MainView.axaml Structure:**
- Header with dashboard icon, greeting, and session info (User, Site, Warehouse)
- Left collapsible side panel with navigation buttons (240px width when open)
- Right content host (ContentControl) for feature views
- Footer with Settings button and menu toggle
- Login overlay (Border with white background) covering entire view when not authenticated

**Buttons and Commands:**
- Test Exception Dialog (`MainView_Button_TestExceptionDialogCommand`)
- Work Order Transaction (`MainView_Button_OpenWorkOrderTransactionCommand`)
- Inventory Transfer (`MainView_Button_OpenInventoryTransferCommand`)
- Settings (`MainView_Button_OpenSettingsCommand`)
- Logout (`MainView_Button_LogoutCommand`)

**Role-Based Access:**
- `CanOpenInventoryTransfer` and `CanOpenWorkOrderTransaction` based on user role
- Supported roles: MaterialHandler, InventorySpecialist, Lead, ReadOnly

---

## Fields and Validation Rules

**Session Properties (Read-Only):**
- `SessionUser` - from ISessionContext.UserId or "Unknown"
- `SiteId` - from ISessionContext.SiteId
- `WarehouseId` - from ISessionContext.WarehouseId

**Navigation Properties:**
- `CanOpenInventoryTransfer` - role-based permission
- `CanOpenWorkOrderTransaction` - role-based permission
- `IsMenuOpen` - controls side panel visibility
- `CurrentView` - hosts feature views (object?)
- `IsLoginVisible` - controls login overlay visibility
- `LoginVm` - LoginViewModel instance for overlay

---

## Visual API Commands (by scenario)

MainView does not call VISUAL APIs directly. Feature navigation routes to:
- Inventory Transfer - see InventoryTransfer.md
- Work Order Transaction - see WorkOrderTransaction.md
- Settings - local configuration only

---

## Business Rules and Exceptions

- If session context is missing/expired, show the Login overlay
- Role-based visibility/enabling of feature entries
- All errors surfaced via centralized Exception Handling Form
- Test Exception Dialog available for debugging purposes

---

## Workflows

1. **App Start:** MainView shows Login overlay → authentication → overlay hides → select feature
2. **Feature Navigation:** Click button → CurrentView set to new feature view with appropriate ViewModel
3. **Logout:** Clear session → show Login overlay → reset UI state

---

## ViewModel/Command Conventions

**MainViewModel Properties:**
- `Greeting` = "Welcome"
- `SessionUser`, `SiteId`, `WarehouseId` - from ISessionContext
- `CanOpenInventoryTransfer`, `CanOpenWorkOrderTransaction` - role-derived
- `IsMenuOpen` - side panel toggle (default: true)
- `CurrentView` - feature view host
- `IsLoginVisible` - login overlay control
- `LoginVm` - login view model

**Commands:**
- `MainView_Button_OpenInventoryTransferCommand` - creates InventoryTransferView with ViewModel
- `MainView_Button_OpenWorkOrderTransactionCommand` - creates WorkOrderTransactionView with ViewModel  
- `MainView_Button_OpenSettingsCommand` - creates SettingsView with ViewModel
- `MainView_Button_LogoutCommand` - clears session, shows login overlay
- `MainView_Button_TestExceptionDialogCommand` - opens test exception dialog

---

## Integration & Storage

- **Services:** IExceptionHandler, INavigationService, ISessionContext
- **View Creation:** Direct view instantiation with dependency injection
- **No local storage** specific to MainView

---

## Keyboard/Scanner UX

**Tooltips and Shortcuts:**
- Work Order Transaction: "Alt+W"
- Inventory Transfer: "Alt+I" 
- Logout: "Alt+L"
- Focus management handled by Avalonia framework

---

## UI Scaffold

**Views:**
- `Views/MainView.axaml` - main layout with side panel and content host
- `Views/Dialogs/LoginView.axaml` - overlay for authentication

**ViewModels:**
- `ViewModels/MainViewModel.cs` - navigation hub logic
- `ViewModels/Dialogs/LoginViewModel.cs` - authentication logic

**Services:**
- NavigationService for window management
- SessionContext for user state
- ExceptionHandler for error routing

---

## Testing & Acceptance

- Login overlay sizes window to content; after authentication window resizes
- Navigation buttons create appropriate feature views in CurrentView
- Role gating properly enables/disables features
- Exception dialog test functionality works
- All errors route through Exception Handling Form

---

## References

- ../../Views/MainView.axaml (UI layout and bindings)
- ../../Views/MainView.axaml.cs (window sizing logic)
- ../../ViewModels/MainViewModel.cs (navigation and state management)
- ./LoginScreen.md (authentication overlay)
- ./ExceptionHandling.md (error handling)

---

## Implementation Status

- **Current:** Fully implemented with all navigation commands and role-based access
- **Window sizing:** Auto-resize logic in MainView.axaml.cs is working
- **Login overlay:** Functional with proper authentication flow
- **Test features:** Exception dialog test button available for debugging

---

## TODOs / Copilot Agent Notes

- [ ] Add keyboard shortcuts (Alt+I, Alt+W, Alt+L) as KeyBindings in XAML
- [ ] Consider adding tooltips for all navigation buttons
- [ ] Verify role gating works properly for all user types