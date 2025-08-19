# Login Screen - Functional and Technical Specification
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `Views/Dialogs/LoginView.axaml`
- **ViewModel:** `ViewModels/Dialogs/LoginViewModel.cs`
- **Primary Commands:** Login, Cancel
- **Related Services:** IAuthenticationService, ISessionContext, INavigationService
- **Last Updated:** 2024-12-19
- **Copilot Template Version:** 1.1

---

## Purpose

Authentication overlay for validating Infor Visual credentials before accessing the main application functionality. Appears as a centered overlay within MainView on startup.

---

## Global Rules

- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.
- ALL methods implement try/catch and route errors through IExceptionHandler with the same normalization routine

---

## Scope

- Show Login overlay at application startup inside MainView
- Collect Visual Username, Password, and optional Site/Domain
- Verify credentials against Visual server using AuthenticationService
- On success: hide overlay, populate session context, remain in MainView
- On failure: show error via Exception Handling Form, remain on Login

---

## Platform and Shell Wiring

- **View:** `Views/Dialogs/LoginView.axaml` (UserControl overlay)
- **Host:** Centered inside MainView with white background Border
- **Navigation:** NavigationService.NavigateToLogin() shows overlay, NavigateToMain() hides it
- **Window Sizing:** MainView adjusts window size to fit login overlay, restores on hide

---

## Current Implementation Details

**LoginView.axaml Structure:**
- Header with login icon and "Sign in" title
- **Visual Log-In Information** panel (expanded by default):
  - Username TextBox
  - Password TextBox (masked)
- **Optional** panel (collapsed by default):
  - Site/Domain TextBox with "MTMFGPLAY" watermark
- Action buttons: Login and Cancel

**LoginViewModel Properties:**
- `LoginViewModel_TextBox_Username` (string)
- `LoginViewModel_TextBox_Password` (string) 
- `LoginViewModel_TextBox_SiteOrDomain` (string?)
- `IsBusy` (bool) - disables UI during authentication

**Commands:**
- `LoginViewModel_Button_LoginCommand` - async authentication
- `LoginViewModel_Button_CancelCommand` - exits application

---

## Fields and Validation Rules

**Required Fields:**
- Username: Cannot be empty/whitespace
- Password: Cannot be empty/whitespace

**Optional Fields:**
- Site/Domain: Used for specific environments, defaults to null

**UI States:**
- `IsBusy` = true during authentication (disables form)
- Invalid operations throw with appropriate error messages

---

## Visual API Commands (by scenario)

**Authentication Flow:**
1. `AuthenticationService.AuthenticateAsync(username, password, siteOrDomain)`
2. Implementation uses `Dbms.OpenLocal` or `OpenLocalSSO` for validation
3. Session context populated from `ApplGlobal` properties
4. License explicitly closed after verification

**Required DLL References:**
- LsaCore.dll, LsaShared.dll
- Appropriate Vmfg*.dll assemblies by domain
- Database.config co-located with application

---

## Business Rules and Exceptions

**Development Environment:**
- Username="Admin", Password="Admin" accepted when Environment=Development
- Bypasses Visual server authentication for testing

**Production Environment:**
- Full Visual server validation required
- Session context populated from server response

**Error Handling:**
- All authentication failures route through IExceptionHandler.Handle(ex, context)
- User sees Exception Dialog with retry/cancel options

---

## Workflows

1. **App Start:** MainView loads → Login overlay appears → window sizes to overlay
2. **Authentication:** Enter credentials → click Login → validate → populate session
3. **Success:** Hide overlay → MainView.OnAuthenticated() → window resizes to content
4. **Failure:** Show Exception Dialog → user choice → retry or remain on login
5. **Cancel:** Exit application via NavigationService.Exit()

---

## ViewModel/Command Conventions

**LoginViewModel Constructor:**
- Default: Creates service instances for testing
- Injected: Accepts IExceptionHandler, INavigationService, ISettings, ISessionContext, IAuthenticationService

**Authentication Process:**
1. Set `IsBusy = true`
2. Validate required fields (throw if invalid)
3. Call `_auth.AuthenticateAsync(username, password, siteOrDomain)`
4. If successful: `_navigationService.NavigateToMain()`
5. If failed: throw with login failure message
6. All exceptions caught and routed through IExceptionHandler
7. Set `IsBusy = false` in finally block

---

## Integration & Storage

**Services Used:**
- IAuthenticationService: credential validation
- INavigationService: overlay show/hide and application exit
- ISessionContext: user state persistence
- ISettings: environment configuration
- IExceptionHandler: error display

**Session Context Population:**
- UserId, SiteId, WarehouseId from authentication response
- ApplGlobal properties for environment defaults
- In-memory only, cleared on logout

---

## Keyboard/Scanner UX

**Current Implementation:**
- No explicit keyboard shortcuts defined
- Standard tab navigation between fields
- Enter/Return likely submits form (framework default)

**Planned Enhancements:**
- Enter triggers Login command when fields valid
- Esc triggers Cancel command

---

## UI Scaffold

**Views:**
- `Views/Dialogs/LoginView.axaml` - authentication form overlay
- Uses Expander controls for collapsible sections
- MaterialDesign icons for visual enhancement

**ViewModels:**
- `ViewModels/Dialogs/LoginViewModel.cs` - authentication logic
- Inherits from ObservableObject (CommunityToolkit.Mvvm)
- Implements async command pattern

**Services:**
- AuthenticationService for credential validation
- NavigationService for application flow control
- SessionContext for user state management

---

## Testing & Acceptance

- Login overlay appears on startup and sizes window appropriately
- Required field validation prevents submission with empty credentials
- Development bypass (Admin/Admin) works when Environment=Development
- Authentication success hides overlay and populates session context
- Authentication failure shows Exception Dialog with clear error message
- Cancel button exits application cleanly
- All errors route through centralized exception handling

---

## References

- ../../Views/Dialogs/LoginView.axaml (UI implementation)
- ../../ViewModels/Dialogs/LoginViewModel.cs (authentication logic)
- ../../Services/Service_Authentication.cs (credential validation)
- ../../References/Visual DLL & Config Files/Database.config
- ./ExceptionHandling.md (error handling flow)
- ./MAMPDatabase.md (local data storage)

---

## Implementation Status

- **Current:** Fully implemented with development bypass and production authentication
- **UI:** Complete with expandable sections and proper validation
- **Navigation:** Working overlay show/hide with window resize
- **Error Handling:** Integrated with centralized exception handling

---

## TODOs / Copilot Agent Notes

- [ ] Add keyboard shortcuts (Enter for login, Esc for cancel)
- [ ] Consider adding "Remember Site/Domain" functionality
- [ ] Verify Visual server authentication works in production environment
- [ ] Add loading indicator during authentication process