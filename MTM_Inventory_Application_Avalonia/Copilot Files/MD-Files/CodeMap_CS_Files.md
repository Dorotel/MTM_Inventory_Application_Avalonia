# Code Map - C# Files Index
_Comprehensive reference for every .cs file in the MTM Inventory Application_

---

**Last Updated:** 2024-12-19  
**Total Source Files:** 22 C# files  
**Maintenance Rule:** Any .cs file change must update its section here in the same changeset

---

## Navigation Structure

- **Project Root:** MTM_Inventory_Application_Avalonia/
- **Desktop Project:** MTM_Inventory_Application_Avalonia.Desktop/
- **Source Files:** All implementation files excluding generated/obj directories

## File Organization

### Application Bootstrap
- App.axaml.cs
- Program.cs

### Services Layer
- Services.cs (primary services)
- Service_ExceptionHandler.cs (placeholder)

### ViewModels
- MainViewModel.cs
- ViewModelBase.cs
- Dialogs/LoginViewModel.cs
- Dialogs/ExceptionDialogViewModel.cs
- Dialogs/IncompletePartDialogViewModel.cs
- Dialogs/LocationPickerDialogViewModel.cs
- InventoryTransferViewModel.cs
- SettingsViewModel.cs
- WorkOrderTransactionViewModel.cs

### Views (Code-Behind)
- MainView.axaml.cs
- SettingsView.axaml.cs
- InventoryTransferView.axaml.cs
- WorkOrderTransactionView.axaml.cs
- Dialogs/LoginView.axaml.cs
- Dialogs/ExceptionDialog.axaml.cs
- Dialogs/IncompletePartDialog.axaml.cs
- Dialogs/LocationPickerDialog.axaml.cs

### Utilities
- Converters/InverseBooleanConverter.cs

---

## Detailed File Descriptions

### MTM_Inventory_Application_Avalonia.Desktop/Program.cs
- **Path:** MTM_Inventory_Application_Avalonia.Desktop/Program.cs
- **Types:** Program (static class)
- **Responsibilities:** Desktop application entry point
- **Key Members:**
  - `Main(string[] args)` - application entry point
  - `BuildAvaloniaApp()` - configures Avalonia AppBuilder
- **Dependencies:** Avalonia framework
- **Notes:** No business logic; pure application hosting

### MTM_Inventory_Application_Avalonia/App.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/App.axaml.cs
- **Types:** App : Application
- **Responsibilities:** Application lifetime management and initial navigation setup
- **Key Members:**
  - `Initialize()` - loads XAML resources
  - `OnFrameworkInitializationCompleted()` - configures services and navigation
    - Removes default data validation plugin
    - Registers MaterialDesign icon provider
    - Desktop: creates Window, NavigationService, navigates to Login
    - Single-view: creates MainView with MainViewModel
- **Dependencies:** NavigationService, MainViewModel, MainView
- **Notes:** All startup configuration happens here; error handling deferred to UI layer

### MTM_Inventory_Application_Avalonia/Services/Services.cs
- **Path:** MTM_Inventory_Application_Avalonia/Services/Services.cs
- **Types:** Multiple service interfaces and implementations:
  - IExceptionHandler, ExceptionHandler
  - ISettings, Settings
  - ISessionContext, SessionContext
  - IAuthenticationService, AuthenticationService
  - INavigationService, NavigationService
  - IPartDialogService, PartDialogService
- **Responsibilities:** Core application services
- **Key Members:**
  - `ExceptionHandler.Handle(ex, context)` - shows modal ExceptionDialog
  - `AuthenticationService.AuthenticateAsync()` - credential validation
  - `NavigationService.Configure(window)`, NavigateToLogin/Main() - overlay management
  - `PartDialogService.PickPartAsync(seed)` - modal part selection
- **Dependencies:** All Views and ViewModels
- **Notes:** Single-file service collection; development authentication bypass for Admin/Admin

### MTM_Inventory_Application_Avalonia/Services/Service_ExceptionHandler.cs
- **Path:** MTM_Inventory_Application_Avalonia/Services/Service_ExceptionHandler.cs
- **Types:** Service_ExceptionHandler (internal, placeholder)
- **Responsibilities:** Currently unused placeholder
- **Dependencies:** None
- **Notes:** Duplicate concept with ExceptionHandler in Services.cs; candidate for removal

### MTM_Inventory_Application_Avalonia/Converters/InverseBooleanConverter.cs
- **Path:** MTM_Inventory_Application_Avalonia/Converters/InverseBooleanConverter.cs
- **Types:** InverseBooleanConverter : IValueConverter
- **Responsibilities:** Inverts boolean values for XAML bindings
- **Key Members:** Convert(), ConvertBack()
- **Dependencies:** Avalonia.Data.Converters
- **Notes:** Used for IsLoginVisible inverse binding in MainView

### MTM_Inventory_Application_Avalonia/ViewModels/ViewModelBase.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/ViewModelBase.cs
- **Types:** ViewModelBase : ObservableObject
- **Responsibilities:** Base class for all ViewModels
- **Key Members:** Inherits CommunityToolkit.Mvvm ObservableObject functionality
- **Dependencies:** CommunityToolkit.Mvvm
- **Notes:** Provides MVVM infrastructure for property change notifications

### MTM_Inventory_Application_Avalonia/ViewModels/MainViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/MainViewModel.cs
- **Types:** MainViewModel : ViewModelBase
- **Responsibilities:** Main application hub; session management; feature navigation
- **Key Members:**
  - Properties: SessionUser, SiteId, WarehouseId, CanOpenInventoryTransfer, CanOpenWorkOrderTransaction, IsMenuOpen, CurrentView, IsLoginVisible, LoginVm
  - Commands: MainView_Button_OpenInventoryTransfer/WorkOrderTransaction/Settings/Logout/TestExceptionDialog
  - `OnAuthenticated()` - hides login overlay, refreshes session data
- **Dependencies:** IExceptionHandler, INavigationService, ISessionContext
- **Notes:** Role-based access control; all commands use try/catch with IExceptionHandler

### MTM_Inventory_Application_Avalonia/ViewModels/InventoryTransferViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/InventoryTransferViewModel.cs
- **Types:** InventoryTransferViewModel : ObservableObject, InventoryTransferResultRow (record)
- **Responsibilities:** Location-to-location inventory transfer logic
- **Key Members:**
  - Properties: InventoryTransfer_TextBox_ItemId/Quantity/FromLocation/ToLocation, InventoryTransfer_Label_WarehouseId/SiteId
  - Commands: ValidateItem, CheckAvailabilityFrom, ShowLocationModal, PostTransfer, Reset, ResolveIncompletePartId
  - `IPartDialogService? PartDialog` - optional dialog service injection
- **Dependencies:** IExceptionHandler, IPartDialogService
- **Notes:** TODO placeholders for Visual API integration; DataGrid with ReadOnlyObservableCollection

### MTM_Inventory_Application_Avalonia/ViewModels/WorkOrderTransactionViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/WorkOrderTransactionViewModel.cs
- **Types:** WorkOrderTransactionViewModel : ObservableObject, WorkOrderResultRow (record)
- **Responsibilities:** Work order material issue/receipt transactions
- **Key Members:**
  - Properties: WorkOrder_TextBox_WorkOrderId, WorkOrder_ComboBox_TransactionTypeIndex, Quantity, FromLocation, ToLocation
  - Commands: ValidateWorkOrder, CheckAvailability, PostTransaction, Reset, ResolveIncompletePartId, ShowLocationModal
  - `IPartDialogService? PartDialog` - optional dialog service injection
- **Dependencies:** IExceptionHandler, IPartDialogService
- **Notes:** Issue (index 0) vs Receipt (index 1) transaction types; TODO Visual API integration

### MTM_Inventory_Application_Avalonia/ViewModels/SettingsViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/SettingsViewModel.cs
- **Types:** SettingsViewModel : ObservableObject
- **Responsibilities:** Application settings configuration
- **Key Members:**
  - Properties: Settings_Text_WarehouseId
  - Commands: Settings_Button_Save, Settings_Button_Cancel
  - Initialization from ISessionContext.WarehouseId or default "002"
- **Dependencies:** IExceptionHandler, ISettings, ISessionContext
- **Notes:** Currently only warehouse ID configuration; Save persists to session context

### MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LoginViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LoginViewModel.cs
- **Types:** LoginViewModel : ObservableObject
- **Responsibilities:** User authentication and login form logic
- **Key Members:**
  - Properties: LoginViewModel_TextBox_Username/Password/SiteOrDomain, IsBusy
  - Commands: LoginViewModel_Button_Login (async), LoginViewModel_Button_Cancel
  - Field validation and authentication flow
- **Dependencies:** IExceptionHandler, INavigationService, ISettings, ISessionContext, IAuthenticationService
- **Notes:** Development bypass accepts Admin/Admin; calls NavigateToMain() on success

### MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/ExceptionDialogViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/ExceptionDialogViewModel.cs
- **Types:** ExceptionDialogViewModel : ObservableObject, ErrorType (enum)
- **Responsibilities:** Error display dialog with severity indicators
- **Key Members:**
  - Properties: Title, Message, Details, ShortTitle, IsDetailsOpen, ErrorType
  - Computed: IconBackground, IconBrush, IconKindName (severity-based styling)
  - Commands: ExceptionDialog_Button_Retry/Cancel/Help/CopyDetails
  - ErrorType enum: Info, Warning, Error, Critical
- **Dependencies:** Avalonia.Media, CommunityToolkit.Mvvm
- **Notes:** Dynamic icon colors and MaterialDesign icons; Retry currently cycles error types for testing

### MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/IncompletePartDialogViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/IncompletePartDialogViewModel.cs
- **Types:** IncompletePartDialogViewModel : ObservableObject, PartSearchResultRow (record)
- **Responsibilities:** Part number search and selection dialog
- **Key Members:**
  - Properties: PartDialog_TextBox_PartFilter, PartDialog_DataGrid_Results, SelectedPart
  - Commands: PartDialog_Button_Search, PartDialog_Button_Select, PartDialog_Button_Cancel
  - Mock data generation for testing (SamplePartSearchResults)
- **Dependencies:** CommunityToolkit.Mvvm, System.Collections.ObjectModel
- **Notes:** TODO integration with Visual part master APIs; currently uses mock data

### MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LocationPickerDialogViewModel.cs
- **Path:** MTM_Inventory_Application_Avalonia/ViewModels/Dialogs/LocationPickerDialogViewModel.cs
- **Types:** LocationPickerDialogViewModel : ObservableObject, LocationRow (record)
- **Responsibilities:** Warehouse location selection dialog
- **Key Members:**
  - Properties: LocationPicker_TextBox_Filter, LocationPicker_DataGrid_Results, SelectedLocation
  - Commands: LocationPicker_Button_Search, LocationPicker_Button_Select, LocationPicker_Button_Cancel
  - Mock data generation for testing (SampleLocationResults)
- **Dependencies:** CommunityToolkit.Mvvm, System.Collections.ObjectModel
- **Notes:** TODO integration with Visual location master; currently uses mock data

### MTM_Inventory_Application_Avalonia/Views/MainView.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/MainView.axaml.cs
- **Types:** MainView : UserControl
- **Responsibilities:** Main application shell with login overlay and window sizing
- **Key Members:**
  - Event handlers: DataContextChanged, AttachedToVisualTree, DetachedFromVisualTree
  - `ApplyLoginSizing(isLoginVisible)` - window size management
  - Property change monitoring for MainViewModel.IsLoginVisible
- **Dependencies:** MainViewModel
- **Notes:** Manages Window.SizeToContent transitions between login overlay and main content

### MTM_Inventory_Application_Avalonia/Views/InventoryTransferView.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/InventoryTransferView.axaml.cs
- **Types:** InventoryTransferView : UserControl
- **Responsibilities:** Code-behind for inventory transfer XAML view
- **Key Members:** InitializeComponent()
- **Dependencies:** InventoryTransferViewModel (DataContext)
- **Notes:** Minimal code-behind; XAML handles UI structure and bindings

### MTM_Inventory_Application_Avalonia/Views/WorkOrderTransactionView.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/WorkOrderTransactionView.axaml.cs
- **Types:** WorkOrderTransactionView : UserControl
- **Responsibilities:** Code-behind for work order transaction XAML view
- **Key Members:** InitializeComponent()
- **Dependencies:** WorkOrderTransactionViewModel (DataContext)
- **Notes:** Minimal code-behind; XAML handles UI structure and bindings

### MTM_Inventory_Application_Avalonia/Views/SettingsView.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/SettingsView.axaml.cs
- **Types:** SettingsView : UserControl
- **Responsibilities:** Code-behind for settings configuration XAML view
- **Key Members:** InitializeComponent()
- **Dependencies:** SettingsViewModel (DataContext)
- **Notes:** Minimal code-behind; XAML handles UI structure and bindings

### MTM_Inventory_Application_Avalonia/Views/Dialogs/LoginView.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/Dialogs/LoginView.axaml.cs
- **Types:** LoginView : UserControl
- **Responsibilities:** Code-behind for login dialog XAML
- **Key Members:** InitializeComponent()
- **Dependencies:** LoginViewModel (DataContext)
- **Notes:** Hosted as overlay in MainView; minimal code-behind

### MTM_Inventory_Application_Avalonia/Views/Dialogs/ExceptionDialog.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/Dialogs/ExceptionDialog.axaml.cs
- **Types:** ExceptionDialog : UserControl
- **Responsibilities:** Code-behind for exception display dialog XAML
- **Key Members:** InitializeComponent()
- **Dependencies:** ExceptionDialogViewModel (DataContext)
- **Notes:** Hosted in modal Window by IExceptionHandler; minimal code-behind

### MTM_Inventory_Application_Avalonia/Views/Dialogs/IncompletePartDialog.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/Dialogs/IncompletePartDialog.axaml.cs
- **Types:** IncompletePartDialog : UserControl
- **Responsibilities:** Code-behind for part search dialog XAML
- **Key Members:** InitializeComponent()
- **Dependencies:** IncompletePartDialogViewModel (DataContext)
- **Notes:** Modal dialog for part number lookup; minimal code-behind

### MTM_Inventory_Application_Avalonia/Views/Dialogs/LocationPickerDialog.axaml.cs
- **Path:** MTM_Inventory_Application_Avalonia/Views/Dialogs/LocationPickerDialog.axaml.cs
- **Types:** LocationPickerDialog : UserControl
- **Responsibilities:** Code-behind for location selection dialog XAML
- **Key Members:** InitializeComponent()
- **Dependencies:** LocationPickerDialogViewModel (DataContext)
- **Notes:** Modal dialog for warehouse location selection; minimal code-behind

---

## Architecture Summary

### MVVM Pattern Implementation
- **Views:** Minimal code-behind, XAML-driven UI with data binding
- **ViewModels:** CommunityToolkit.Mvvm with ObservableProperty and RelayCommand
- **Models:** Record types for DataGrid display (ResultRow types)

### Service Layer
- **Centralized Services:** All services in Services.cs for simplicity
- **Interface-Based:** All services implement interfaces for testability
- **Error Handling:** IExceptionHandler used throughout application

### Navigation Architecture
- **Single Window:** MainWindow hosts MainView throughout application lifetime
- **Overlay Pattern:** Login shown as overlay, not separate window
- **Content Hosting:** Features displayed in MainView.CurrentView

### Dialog Management
- **Modal Windows:** Dialogs created as Windows with UserControl content
- **Service Integration:** IPartDialogService for enhanced dialog workflows
- **Consistent Patterns:** All dialogs follow similar ViewModel/Command structure

---

## Development Notes

### Visual API Integration Status
- **Current:** Placeholder implementations with TODO comments
- **Planned:** Integration with Vmfg*.dll assemblies for production
- **Development:** Mock data and Admin/Admin bypass for testing

### Error Handling Strategy
- **Centralized:** All exceptions route through IExceptionHandler.Handle()
- **Consistent:** Every command method has try/catch wrapper
- **User-Friendly:** ExceptionDialog provides clear error presentation

### Future Enhancements
- **Visual API:** Replace TODO placeholders with actual API calls
- **Persistence:** App-owned database for settings and audit trails
- **Testing:** Comprehensive unit and integration test coverage

---

## Maintenance Guidelines

1. **File Updates:** Any changes to .cs files must update this CodeMap in same changeset
2. **New Files:** Add complete section following established format
3. **Removed Files:** Move to "Removed Files" section with date
4. **Refactoring:** Update all affected file descriptions

**Last Comprehensive Review:** 2024-12-19