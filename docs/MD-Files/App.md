# App - Shell, Startup, Navigation

- MainWindow hosts MainView. Login overlay appears on start; Window auto-sizes to the active child (login vs content) and restores previous size after login.
- Settings include a strict/assist toggle for incomplete part resolution; when enabled, the Parts-like dialog opens; when disabled, an error is shown.
- All VISUAL calls are performed by service adapters; UI remains UI-only and routes errors via IExceptionHandler.

# App.axaml - Planning and Integration Notes

Purpose
- Avalonia application bootstrap: resources, styles, and application lifetime.

Responsibilities
- Load XAML resources and configure global styles.
- OnFrameworkInitializationCompleted: create MainWindow, initialize NavigationService, and set up MainView.
- Remove default data validation plugin once (implemented) to avoid double-validation messages.

Initialization Flow
1) AvaloniaXamlLoader.Load(this)
2) If desktop lifetime -> create MainWindow, call NavigationService.Configure(MainWindow), then NavigationService.NavigateToLogin() to show Login overlay inside MainView.
3) If single-view lifetime -> create MainView with MainViewModel.

Error Handling
- Any failure during startup should be caught and shown via IExceptionHandler if possible; otherwise, log to debug output.

Testing Checklist
- App loads without XAML parse errors.
- MainWindow shows with MainView and Login overlay.

References
- ../../App.axaml
- ../../App.axaml.cs
- ../../Views/MainView.axaml
- ../../Views/MainWindow.axaml
- ../../Services/Services.cs (NavigationService, IExceptionHandler)
