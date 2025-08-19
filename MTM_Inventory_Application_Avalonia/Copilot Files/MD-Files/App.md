# App - Shell, Startup, Navigation
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `App.axaml`
- **ViewModel:** None (Application bootstrap)
- **Primary Services:** NavigationService, IExceptionHandler
- **Related Classes:** MainWindow, MainView, MainViewModel
- **Last Updated:** 2024-12-19
- **Copilot Template Version:** 1.1

---

## Purpose

Avalonia application bootstrap responsible for application initialization, resources, styles, and navigation setup. Creates the main window structure and establishes service dependencies.

---

## Global Rules

- ALL methods implement try/catch and route errors through IExceptionHandler with the same normalization routine
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

---

## Scope

Application startup and lifetime management including:
- XAML resource loading and theme configuration
- Icon provider registration (MaterialDesign)
- Window creation and navigation service initialization
- Data validation plugin removal to avoid double-validation

---

## Platform and Shell Wiring

**Desktop Lifetime:**
- Creates MainWindow with title "MTM Inventory Application"
- WindowStartupLocation: CenterScreen, SizeToContent: WidthAndHeight
- Sets up NavigationService and navigates to Login overlay

**Single-View Lifetime:**
- Creates MainView with MainViewModel directly
- Used for mobile/web platforms

---

## Initialization Flow

1. `AvaloniaXamlLoader.Load(this)` - Load XAML resources
2. `BindingPlugins.DataValidators.RemoveAt(0)` - Remove default validation to avoid duplicates
3. `IconProvider.Current.Register<MaterialDesignIconProvider>()` - Register MaterialDesign icons
4. **If desktop lifetime:** create MainWindow, configure NavigationService, NavigateToLogin()
5. **If single-view lifetime:** create MainView with MainViewModel

---

## Current Implementation Details

**App.axaml:**
- FluentTheme with DataGrid styles
- Auto-width columns for all DataGrids

**App.axaml.cs:**
- NavigationService instance creation and configuration
- MainWindow properties: Title, WindowStartupLocation, SizeToContent
- Separate handling for desktop vs single-view application lifetimes

---

## Error Handling

- Startup failures should be caught and shown via IExceptionHandler if possible
- Otherwise log to debug output
- No direct exception handling in current implementation (relying on framework)

---

## Testing Checklist

- App loads without XAML parse errors
- MainWindow shows with correct sizing and centering
- Login overlay appears on startup
- MaterialDesign icons render properly
- DataGrid columns auto-size correctly

---

## References

- ../../App.axaml (XAML resources, styles, themes)
- ../../App.axaml.cs (Application bootstrap code)
- ../../Views/MainView.axaml (Main application view)
- ../../Services/Services.cs (NavigationService, IExceptionHandler)
