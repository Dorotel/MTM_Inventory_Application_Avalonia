# Main View — Shell and Navigation [Ref: ../UIX Presentation/mainview.html]

Purpose: central hub hosting feature views; shows login overlay first, then content.

Global Rule — Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Shell wiring
- MainWindow hosts MainView. On startup, LoginView overlays MainView; after login, overlay hides and MainView shows feature content in its content region.
- Auto sizing: MainView monitors IsLoginVisible and adjusts the Window SizeToContent (WidthAndHeight) to fit the active child (login or main content). When hiding login, it restores the previous window size if it was concrete; otherwise it keeps WidthAndHeight until the user resizes. See Views/MainView.axaml.cs ApplyLoginSizing.

Navigation
- Feature views are hosted in the main content region: InventoryTransferView, WorkOrderTransactionView, SettingsView.
- Current implementation assigns CurrentView directly from MainViewModel commands. NavigationService also exposes OpenInventoryTransfer/OpenWorkOrderTransaction helpers that create the views and inject dialog services.

UX rules
- Keyboard/Scanner flows bubble to active feature view.
- Button content must remain inside buttons (no overflow into panel areas); keep horizontal stacks for icon+text.

Error handling
- All UI methods use try/catch and route through IExceptionHandler.

Testing
- Login overlay sizes the window to content; on login success window restores prior size or remains size-to-content if prior was NaN.
- Replacing child views keeps the window sized to its active content region.




# Main View (Home) — UI Planning Specification [Ref: ../MVVM Definitions/InventoryTransfer.md; ../MVVM Definitions/WorkOrderTransaction.md; ../MVVM Definitions/LoginScreen.md]

Purpose: define the home screen that users see after a successful login, providing clear entry points to Inventory Transfer and Work Order Transaction features.

Global Rule — Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Scope
- Acts as the hub after Login: opens Inventory Transfer and Work Order Transaction UI flows.
- Shows basic session context (user, site, warehouse) and provides Logout.
- Hosts a Login overlay until authentication succeeds, then resizes the window to the base MainView size when overlay hides.
- Does not call VISUAL APIs directly; delegates to feature-specific views/services which implement their own API flows per spec.

Platform and Shell wiring
- View: Views/MainView.axaml (UserControl). DataContext: MainViewModel.
- Host: MainWindow displays MainView as its content at startup. A Login overlay (Views/Dialogs/LoginView) is centered inside MainView while unauthenticated.
- Navigation: MainViewModel exposes commands to open features in the MainView content host (CurrentView) and may call INavigationService helpers to construct views and inject dialog services.
- Window sizing: MainView code-behind toggles the parent Window.SizeToContent to match Login overlay when visible; on hide, restores previous dimensions if saved, otherwise sizes the window to the MainView base content.

## Code Naming and Error Handling Rules
- File naming: {Type}_{Parent}_{Name}
- Methods: {Class}_{ControlType}_{Name}
- Variables: {Method}_{Type(Int|string|exc)}_{Name}
- All methods must have error handling and must use the same centralized error handling (IExceptionHandler).

## Screenshot highlighting (reference only)
- No specific screenshot is required for MainView. Any color highlights used in other artifacts are illustrative only and do not define application theming.

## Fields and Validation Rules
- Session summary (read-only): UserId/DisplayName, SiteId, WarehouseId; ensure session context is present or redirect to Login overlay.
- Navigation controls: buttons/menu items/tiles for Inventory Transfer and Work Order Transaction.
- Logout: always enabled; clears session and returns to Login overlay.
- Role gating: navigation to features may be hidden/disabled based on roles described in each feature spec.

## Visual API Commands (by scenario)
- MainView does not call VISUAL APIs directly. Feature navigation routes to:
  - Inventory Transfer — see InventoryTransfer.md (Dbms.OpenLocal/OpenLocalSSO, GeneralQuery, InventoryTransaction, TRACE, UOM conv).
  - Work Order Transaction — see WorkOrderTransaction.md (Dbms.OpenLocal/OpenLocalSSO, GetWorkOrderSummary, GeneralQuery, InventoryTransaction, TRACE, UOM conv).

## Business Rules and Exceptions
- If session context is missing/expired, show the Login overlay.
- Role-based visibility/enabling of feature entries (Read-Only can view, but posting actions are disabled at feature level).
- All errors surfaced via centralized Exception Handling Form.

## Workflows
- App start ? MainView shows Login overlay ? (success) overlay hides ? select feature (Inventory Transfer or Work Order Transaction) ? feature flow per spec.
- Logout from MainView shows Login overlay again.

## ViewModel, Commands, and Role Gating
- MainViewModel properties
  - Greeting (string) — optional welcome text.
  - SessionUser (string), SiteId (string), WarehouseId (string) — read-only, from ISessionContext.
  - CanOpenInventoryTransfer (bool), CanOpenWorkOrderTransaction (bool) — derived from roles.
  - IsLoginVisible (bool) — whether the Login overlay is shown.
  - CurrentView (object?) — host for feature views.
- Commands
  - MainView_Button_OpenInventoryTransfer
  - MainView_Button_OpenWorkOrderTransaction
  - MainView_Button_OpenSettings
  - MainView_Button_Logout
- Role gating
  - Material Handler/Inventory Specialist/Lead: full access to open both features.
  - Read-Only: can open features in read-only mode; posting disabled within features.

## Integration Approach
- INavigationService provides OpenInventoryTransfer and OpenWorkOrderTransaction which can set MainViewModel.CurrentView to the appropriate View with dialog services injected.
- Settings entry can be opened directly from MainViewModel by assigning SettingsView to CurrentView.
- Propagate ISessionContext to features for environment/role decisions.

## Local Storage and Reporting
- None specific to MainView. Feature flows write to local history/exception tables per their specs.

## Keyboard and Scanner UX
- Shortcuts
  - Alt+I: open Inventory Transfer
  - Alt+W: open Work Order Transaction
  - Alt+L: logout
- Focus defaults to the primary navigation control.

## UI Scaffolding (Avalonia 11)
- Views
  - Views/MainView.axaml: home UI with session summary, Login overlay, and navigation buttons/tiles; a content host (ContentControl) for feature views.
- ViewModels
  - ViewModels/MainViewModel.cs: Greeting, Session props, derived permissions; IsLoginVisible; CurrentView; commands above; INotifyPropertyChanged.
- Commands and Shortcuts
  - Bind buttons/menu items to commands; add KeyBindings in XAML for Alt+I/Alt+W/Alt+L if desired.
- Services and DI
  - ISessionContext, INavigationService, IExceptionHandler, ISettings.
- DataTemplates and Navigation
  - Map child ViewModels to Views (InventoryTransferViewModel ? InventoryTransferView, WorkOrderTransactionViewModel ? WorkOrderTransactionView) or assign Views directly to CurrentView.

## Testing and Acceptance Criteria
- On startup, MainWindow hosts MainView and the Login overlay is visible; window sizes to the overlay.
- After successful Login, overlay hides and window resizes to the base MainView size (or restores previous dimensions when available).
- Clicking Inventory Transfer opens InventoryTransferView; clicking Work Order Transaction opens WorkOrderTransactionView in the content host.
- Shortcuts (Alt+I/Alt+W) open respective features; Alt+L logs out and shows Login overlay.
- Role gating: when user lacks permissions, feature entries are disabled/hidden; features still enforce server-side permissions.
- Errors anywhere route through the Exception Handling Form.

## References (artifacts used by this document)
- ../MVVM Definitions/LoginScreen.md
- ../MVVM Definitions/InventoryTransfer.md
- ../MVVM Definitions/WorkOrderTransaction.md
- ../MVVM Definitions/ExceptionHandling.md
- ../../README.md