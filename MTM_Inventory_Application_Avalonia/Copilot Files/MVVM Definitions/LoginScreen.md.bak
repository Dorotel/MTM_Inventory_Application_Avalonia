# Login Screen — Functional and Technical Specification

Reevaluation (latest)
- Changes from previous version:
  - Clarified authentication using Dbms.OpenLocal and OpenLocalSSO with exact source citations.
  - Added session context hydration from ApplGlobal (SiteID, MultiSite, currency) with citations.
  - Documented required DLL references and Database.config placement with citations.
  - Linked centralized Exception Handling Form for all error flows.
  - Linked centralized MAMPDatabase.md for app-owned storage policy.
  - Updated navigation to reflect Login overlay hosted inside MainView (no window swap after login).
  - Updated error handling note to reflect current IExceptionHandler.Handle(ex, context) dialog behavior.

Purpose
- Define the startup login experience for authenticating a user with Infor Visual credentials before opening the main application UI.

Global Rule — Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Screenshot highlighting (reference only)
- Any colors shown in screenshots are illustrative callouts to indicate field roles/sections; they do not define application theming. Implement standard app styling.

Scope
- Show the Login screen at application startup as an overlay inside MainView.
- Collect Visual Username and Password (and optional Domain/Site if required by environment).
- Verify credentials against the Visual server using approved APIs or stored procedures.
- On success, persist a signed session token (not the raw password) in memory for the runtime session, then hide the Login overlay and remain in MainView.
- On failure, display an error and remain on the Login screen (all errors are routed to the Exception Handling Form). See ../MVVM Definitions/ExceptionHandling.md.

Platform
- Avalonia 11 on .NET 8; MVVM pattern (View, ViewModel, Services).
- Hosting: MainWindow hosts MainView at startup; MainView displays a centered Login overlay (Views/Dialogs/LoginView) until authentication succeeds.

Security and Storage
- Do not store plain passwords on disk.
- Keep only an in?memory session context for the running process; clear it on logout/exit.
- Use secure string handling and zeroing where practical.
- For any app-owned data storage, see ../MVVM Definitions/MAMPDatabase.md.

User Interface (implemented as overlay)
- Fields: Username, Password (masked), optional Site/Domain selector, Login button, Cancel/Exit.
- Validation: required fields, debounced login attempt; disable Login button while authenticating.
- Errors: show Visual server messages when available, otherwise a generic message (via the Exception Handling Form).

Authentication Flow
1) App starts ? MainWindow hosts MainView ? Login overlay shown.
2) User enters credentials ? clicks Login.
3) Client calls AuthenticationService.AuthenticateAsync(username, password, site?)
   - Acquire Visual license for the authentication call.
   - Perform server verification (assemblies or stored procedures per environment policy).
   - Always release/close the license in a finally block.
   - Implementation note (assemblies): establish a short?lived VISUAL connection using Dbms.OpenLocal(instance, user, pass) or OpenLocalSSO(instance, userName, userSID, domain, domainSID), then immediately close after verification. [Intro - Development Guide.txt, p.13 (OpenLocal/Database.config); p.14 (OpenLocalSSO); Reference - Core.txt, p.33–37 (OpenLocal overloads)]
4) If success ? store session context ? hide Login overlay and remain in MainView.
5) If failure ? route the error and context to the Exception Handling Form; after user choice, either retry or stay on Login.

Implementation Notes
- Required references: LsaCore.dll, LsaShared.dll and appropriate Vmfg*.dll by domain. [Intro - Development Guide.txt, p.11–12]
- Database.config must be co?located with the app and API DLLs at runtime. [Intro - Development Guide.txt, p.15]
- Dbms.UserID(instance) can be used to confirm the connected VISUAL user. [Reference - Core.txt, p.48]

Integration Options
- Assemblies: wrap VmfgShared/VmfgInventory/VmfgShopFloor (if licensed) behind an adapter that exposes AuthenticateAsync and manages license acquire/close per call.
- Database: if using a stored procedure for validation, call via a minimal DAL; still acquire/close license if procedure requires one.

Session Context
- Expose ISessionContext with properties: UserId, DisplayName, Roles, SiteId, WarehouseId, AuthTimeUtc, and a method LogoutAsync() that clears credentials and returns to Login.
- Populate environment/site defaults after auth using ApplGlobal where available:
  - SiteID (current): ApplGlobal.SiteID. [Reference - VMFG Shared Library.txt, p.84]
  - MultiSite flag: ApplGlobal.MultiSite. [Reference - VMFG Shared Library.txt, p.78]
  - System currency: ApplGlobal.SystemCurrencyID. [Reference - VMFG Shared Library.txt, p.86]

Navigation
- On successful authentication, INavigationService.NavigateToMain() invokes MainViewModel.OnAuthenticated() to hide the overlay; MainView remains loaded in MainWindow. See ../../App.axaml.cs and ../../Services/Services.cs.

Error Handling
- All errors (configuration, connection/auth, environment, and API/business) are routed to the Exception Handling Form. Current implementation calls IExceptionHandler.Handle(ex, context) which opens a modal ExceptionDialog with details. See ../MVVM Definitions/ExceptionHandling.md.

UI Scaffolding (Avalonia 11)
- Views
  - Views/Dialogs/LoginView.axaml: username, password, optional site/domain selector, Login/Cancel.
- ViewModels
  - ViewModels/Dialogs/LoginViewModel.cs: Username, Password, SiteOrDomain, IsBusy; AuthenticateCommand, CancelCommand; INotifyDataErrorInfo.
- Commands and Shortcuts
  - Enter triggers AuthenticateCommand when fields valid; Esc cancels.
- Services and DI
  - IAuthenticationService, IExceptionHandler, ISettings, ISessionContext.
- DataTemplates and Navigation
  - NavigationService.NavigateToLogin() shows the overlay inside MainView; NavigateToMain() calls MainViewModel.OnAuthenticated().

Citations (page and line)
- Visual shared settings and site context: Reference - VMFG Shared Library.txt, p.55 — ApplGlobal properties list (see also p.78 MultiSite; p.84 SiteID; p.86 SystemCurrencyID).
- Authentication connection behaviors: Intro - Development Guide.txt, p.13 — “Connection information is obtained from the Database.Config file.” (OpenLocal); p.14 — OpenLocalSSO with user/domain SIDs; Reference - Core.txt, p.33–37 — Dbms.OpenLocal overloads; p.48 — Dbms.UserID.
- Work order/shop-floor login relevance: Reference - Shop Floor.txt, p.98–100 — GetWorkOrderSummary service (post-login usage scenario).

References
- ../../References/Visual DLL & Config Files/Database.config
- ../../References/Visual PDF Files/Text Conversion/Reference - Core.txt
- ../../References/Visual PDF Files/Text Conversion/Intro - Development Guide.txt
- ../../References/Visual PDF Files/Text Conversion/Reference - VMFG Shared Library.txt
- ../../References/Visual PDF Files/Text Conversion/Reference - Shop Floor.txt
- ./ExceptionHandling.md
- ./MAMPDatabase.md

## Implementation status (scaffold)
- View created: ../../Views/Dialogs/LoginView.axaml (UserControl)
- ViewModel created: ../../ViewModels/Dialogs/LoginViewModel.cs
- Navigation: App starts at Login overlay via NavigationService.NavigateToLogin(); on success, NavigateToMain() hides the overlay and keeps MainView active.
- Development login: Username=Admin, Password=Admin accepted when Environment=Development (see README.md).

# Login Screen — Planning Specification [Ref: ../UIX Presentation/login.html]

Purpose: authenticate then hide overlay and continue in MainView.

Global Rule — Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes.

Shell wiring
- LoginView is shown as an overlay inside MainView. MainView sizes the Window to fit the overlay while visible and restores previous Manual size when hidden.

UX rules
- Minimal form: Username, Password, Site/Domain; Enter to submit; Esc cancels/clears in dev.
- Buttons keep text within their bounds; use icon+text stacks when applicable.

Error handling
- All UI methods use try/catch and route through IExceptionHandler.
