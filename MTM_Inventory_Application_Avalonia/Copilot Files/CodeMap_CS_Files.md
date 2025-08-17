# Code Map — C# Files Index

Purpose: Centralized, detailed reference for every .cs file in the application. Keep this document in sync with the code. See README rule: any .cs file change must update its section here.

Navigation:
- Project root: MTM_Inventory_Application_Avalonia
- Desktop project: MTM_Inventory_Application_Avalonia.Desktop

Conventions used below
- Path: relative to solution root
- Types: primary classes/records/interfaces declared in the file
- Responsibilities: what the file does in the app
- Key members: notable properties/commands/methods
- Dependencies: other app services/types it relies on
- Notes: error handling, placeholders, navigation, and other behaviors

---

## MTM_Inventory_Application_Avalonia/App.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/App.axaml.cs
- Types: App : Avalonia.Application
- Responsibilities: Configure Avalonia application lifetime and initial navigation.
- Key members:
  - Initialize(): loads App.axaml resources
  - OnFrameworkInitializationCompleted():
    - Removes default data validation plugin (BindingPlugins.DataValidators.RemoveAt(0))
    - Desktop: creates MainWindow, configures NavigationService with it, and navigates to Login overlay
    - SingleView: creates MainView with MainViewModel
- Dependencies: Views.MainWindow, Services.NavigationService, ViewModels.{MainViewModel}
- Notes: All startup errors should be routed via centralized exception handling in UI flows; this file wires the shell only.

## MTM_Inventory_Application_Avalonia.Desktop/Program.cs
- Path: MTM_Inventory_Application_Avalonia.Desktop/Program.cs
- Types: Program
- Responsibilities: App entry point for classic desktop lifetime; configures AppBuilder and starts.
- Key members: Main(args), BuildAvaloniaApp()
- Dependencies: Avalonia
- Notes: No business logic here.

## MTM_Inventory_Application_Avalonia/Services/Services.cs
- Path: MTM_Inventory_Application_Avalonia/Services/Services.cs
- Types:
  - IExceptionHandler, ExceptionHandler
  - ISettings, Settings
  - ISessionContext, SessionContext
  - IAuthenticationService, AuthenticationService
  - INavigationService, NavigationService
- Responsibilities:
  - Cross-cutting services and navigation host
  - Dev placeholder authentication (Admin/Admin when Environment is Development)
  - Maintain and expose session info (user, site, warehouse, role)
  - NavigationService ensures a single MainView is hosted in MainWindow and shows LoginView as an overlay via MainViewModel
  - Feature navigation loads views into MainViewModel.CurrentView
- Key members:
  - ExceptionHandler.Handle(ex, context): shows a modal ExceptionDialog (Window hosting Views.Dialogs.ExceptionDialog) with Message and Details; falls back to Debug.WriteLine if dialog fails
  - Settings.Environment: string flag for Dev/Prod
  - SessionContext.Logout(): clears session
  - AuthenticationService.AuthenticateAsync(...): dev stub to set session
  - NavigationService.Configure(window), NavigateToLogin(), NavigateToMain(), OpenInventoryTransfer(), OpenWorkOrderTransaction(), Exit()
- Dependencies: Views (MainView, InventoryTransferView, WorkOrderTransactionView, Dialogs.ExceptionDialog), ViewModels, Avalonia lifetime
- Notes:
  - Uses EnsureMainView() to always host a single MainView instance
  - After successful login, calls MainViewModel.OnAuthenticated()
  - All UI-invoked operations wrap errors using IExceptionHandler per app rules

## MTM_Inventory_Application_Avalonia/Services/Service_ExceptionHandler.cs
- Path: MTM_Inventory_Application_Avalonia/Services/Service_ExceptionHandler.cs
- Types: Service_ExceptionHandler (internal)
- Responsibilities: Currently a placeholder with no implementation.
- Dependencies: None
- Notes: Duplicate concept with ExceptionHandler in Services.cs; plan to remove or merge into the central IExceptionHandler implementation.

## MTM_Inventory_Application_Avalonia/Views/MainWindow.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/MainWindow.axaml.cs
- Types: MainWindow : Avalonia.Controls.Window
- Responsibilities: Code-behind bootstrap for the main window XAML.
- Key members: ctor calls InitializeComponent()
- Dependencies: None
- Notes: All content is set by NavigationService/App startup.

## MTM_Inventory_Application_Avalonia/Views/MainView.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/MainView.axaml.cs
- Types: MainView : Avalonia.Controls.UserControl
- Responsibilities:
  - Hosts main application layout and a login overlay (see XAML)
  - Adjusts parent Window size based on login overlay visibility
- Key members:
  - Event hooks: DataContextChanged, AttachedToVisualTree
  - ApplyLoginSizing(isLoginVisible):
    - Saves prior SizeToContent/Width/Height when login shows
    - Sets Window.SizeToContent = WidthAndHeight while login visible to match LoginView
    - On hide: restores previous sizing and dimensions if saved; if not available (NaN), sets SizeToContent = WidthAndHeight so the window sizes to the MainView base content
- Dependencies: ViewModels.MainViewModel.IsLoginVisible
- Notes: Ensures a smooth size transition for the window when switching between login and main UI. Handles initial NaN width/height correctly.

## MTM_Inventory_Application_Avalonia/ViewModels/MainViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/MainViewModel.cs
- Types: MainViewModel : ViewModelBase
- Responsibilities:
  - Session summary fields (User, Site, Warehouse)
  - Feature navigation (InventoryTransfer, WorkOrderTransaction, Settings)
  - Manage login overlay (IsLoginVisible, LoginVm) and menu state
  - Handle Logout
- Key members:
  - Properties: SessionUser, SiteId, WarehouseId, CanOpenInventoryTransfer, CanOpenWorkOrderTransaction, IsMenuOpen, CurrentView, IsLoginVisible, LoginVm
  - Commands: MainView_Button_OpenInventoryTransfer, MainView_Button_OpenWorkOrderTransaction, MainView_Button_OpenSettings, MainView_Button_Logout
  - OnAuthenticated(): hides login overlay; refreshes session labels
- Dependencies: Services.{IExceptionHandler, INavigationService, ISessionContext}, Views (feature views)
- Notes: All commands wrap try/catch and route to IExceptionHandler per app rules.

## MTM_Inventory_Application_Avalonia/Views/Dialogs/LoginView.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/Dialogs/LoginView.axaml.cs
- Types: LoginView : UserControl
- Responsibilities: Code-behind for login dialog view XAML
- Dependencies: DataContext expected to be ViewModels.Dialogs.LoginViewModel
- Notes: Hosted as a centered overlay inside MainView.

## MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LoginViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LoginViewModel.cs
- Types: LoginViewModel : ObservableObject
- Responsibilities: Handle login and cancel actions; bind to username/password/site fields
- Key members:
  - Bindables: LoginViewModel_TextBox_Username, LoginViewModel_TextBox_Password, LoginViewModel_TextBox_SiteOrDomain, IsBusy
  - Commands: LoginViewModel_Button_Login (async), LoginViewModel_Button_Cancel
- Dependencies: Services.{IExceptionHandler, INavigationService, ISettings, ISessionContext, IAuthenticationService}
- Notes: On successful auth, calls NavigationService.NavigateToMain(); Dev auth accepts Admin/Admin.

## MTM_Inventory_Application_Avalonia/Views/InventoryTransferView.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/InventoryTransferView.axaml.cs
- Types: InventoryTransferView : UserControl
- Responsibilities: Code-behind bootstrap for the Inventory Transfer view XAML.
- Notes: DataContext expected to be InventoryTransferViewModel.

## MTM_Inventory_Application_Avalonia/ViewModels/InventoryTransferViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/InventoryTransferViewModel.cs
- Types: InventoryTransferViewModel : ObservableObject
- Responsibilities: State and commands for inventory transfer flow (placeholders)
- Key members:
  - Fields: ItemId, Quantity, FromLocation, ToLocation; labels WarehouseId, SiteId
  - Commands: ValidateItem, CheckAvailabilityFrom, ShowLocationModal, PostTransfer, Reset
- Dependencies: IExceptionHandler
- Notes: All business actions are placeholders; future service adapters will call VISUAL.

## MTM_Inventory_Application_Avalonia/Views/WorkOrderTransactionView.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/WorkOrderTransactionView.axaml.cs
- Types: WorkOrderTransactionView : UserControl
- Responsibilities: Code-behind bootstrap for the Work Order transaction view XAML.
- Notes: DataContext expected to be WorkOrderTransactionViewModel.

## MTM_Inventory_Application_Avalonia/ViewModels/WorkOrderTransactionViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/WorkOrderTransactionViewModel.cs
- Types: WorkOrderTransactionViewModel : ObservableObject
- Responsibilities: State and commands for Issue/Receipt against Work Orders (placeholders)
- Key members:
  - Fields: WorkOrderId, TransactionTypeIndex, Quantity, FromLocation, ToLocation; labels WarehouseId, SiteId
  - Commands: ValidateWorkOrder, CheckAvailability, PostTransaction, Reset
- Dependencies: IExceptionHandler
- Notes: Placeholder implementations; VISUAL interactions to be added via service layer.

## MTM_Inventory_Application_Avalonia/Views/SettingsView.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/SettingsView.axaml.cs
- Types: SettingsView : UserControl
- Responsibilities: Code-behind bootstrap for settings view XAML.
- Notes: DataContext expected to be SettingsViewModel.

## MTM_Inventory_Application_Avalonia/ViewModels/SettingsViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/SettingsViewModel.cs
- Types: SettingsViewModel : ObservableObject
- Responsibilities: Manage settings form state (environment, warehouse, app DB testing)
- Key members:
  - Fields: Settings_Combo_EnvironmentIndex, Settings_Text_WarehouseId, Settings_Label_AppDbName, Settings_Label_SessionUser, Settings_Label_SiteId, Settings_Toggle_IsDevVisible, Settings_Toggle_DevLoginEnabled
  - Commands: Settings_Button_Save, Settings_Button_Cancel, Settings_Button_TestAppDb
- Dependencies: IExceptionHandler, ISettings, ISessionContext
- Notes: Commands are placeholders; implement persistence and tests via service adapters.

## MTM_Inventory_Application_Avalonia/ViewModels/ViewModelBase.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/ViewModelBase.cs
- Types: ViewModelBase : ObservableObject
- Responsibilities: Common base for app view models; currently no extra behavior.

## MTM_Inventory_Application_Avalonia/Views/Dialogs/ExceptionDialog.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/Dialogs/ExceptionDialog.axaml.cs
- Types: ExceptionDialog : UserControl
- Responsibilities: Code-behind for exception dialog XAML.
- Notes: Loaded via AvaloniaXamlLoader.

## MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/ExceptionDialogViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/ExceptionDialogViewModel.cs
- Types: ExceptionDialogViewModel : ObservableObject
- Responsibilities: ViewModel for an exception dialog with Title, Message, Details, and simple commands.

## MTM_Inventory_Application_Avalonia/Views/Dialogs/LocationPickerDialog.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/Dialogs/LocationPickerDialog.axaml.cs
- Types: LocationPickerDialog : UserControl
- Responsibilities: Code-behind for a modal locations picker dialog.

## MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LocationPickerDialogViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LocationPickerDialogViewModel.cs
- Types:
  - LocationPickerDialogViewModel : ObservableObject
  - LocationBalanceDto (DTO)
- Responsibilities: Provide filtering and selection of locations with inventory balances (placeholders)
- Key members:
  - Filters: text, NonZeroOnly, CurrentWarehouseOnly, CurrentSiteOnly
  - Results: ObservableCollection<LocationBalanceDto>, SelectedRow
  - Commands: Select, Cancel (placeholders)

## MTM_Inventory_Application_Avalonia/Views/Dialogs/IncompletePartDialog.axaml.cs
- Path: MTM_Inventory_Application_Avalonia/Views/Dialogs/IncompletePartDialog.axaml.cs
- Types: IncompletePartDialog : UserControl
- Responsibilities: Code-behind for a part search/selection dialog.

## MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/IncompletePartDialogViewModel.cs
- Path: MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/IncompletePartDialogViewModel.cs
- Types:
  - IncompletePartDialogViewModel : ObservableObject
  - PartSearchResult (DTO)
- Responsibilities: Provide search and selection of parts (placeholders)
- Key members:
  - Fields: SearchText, Results, SelectedPart
  - Commands: Select, Cancel (placeholders)

## MTM_Inventory_Application_Avalonia/Views/Dialogs/LoginView.axaml.cs (duplicate listed above)
- See the LoginView section above.

## MTM_Inventory_Application_Avalonia/Converters/InverseBooleanConverter.cs
- Path: MTM_Inventory_Application_Avalonia/Converters/InverseBooleanConverter.cs
- Types: InverseBooleanConverter : IValueConverter
- Responsibilities: Convert bool <-> inverted bool for bindings
- Key members: Convert, ConvertBack (both logical negation when input is bool)
- Notes: Used in MainView.axaml to hide the main layout when Login overlay is visible.

---

Maintenance Rule
- Whenever any .cs file is added, removed, renamed, or edited, update the corresponding section here in the same pull request/commit.
- If a file is removed, keep its section under a “Removed” appendix with the date and replacement file, if any.
- Validate that paths and types match the codebase after each change.
