# Exception Handling Form - Functional and Technical Specification

Purpose: centralize all error handling UX and logic across the app. All forms route errors to this Form via the IExceptionHandler service.

Global Rule - Visual license lifecycle
- Anytime an operation against the Visual server requires a license, acquire it in a short-lived scope and release/close it in a finally block regardless of success/failure. [Intro - Development Guide.txt, p.13-14; Reference - Core.txt, p.33-37]

Screenshot note (reference only)
- Any colors shown in screenshots are highlights to indicate field roles/sections and do not define application theming. Implement standard app styling; follow functional groupings only.

Scope: applies to LoginScreen, WorkOrderTransaction, Inventory Transfer, and future screens. Covers configuration/connection/auth errors, environment/runtime preconditions, API/business validation failures, and service execution errors. Displays actionable messages, logs details to the app database when applicable, and preserves the global license lifecycle rule.

Platform/Shell wiring
- Service: IExceptionHandler (current implementation: Services.ExceptionHandler in ../../Services/Services.cs).
- Callers invoke: IExceptionHandler.Handle(exception, contextString).
- Current behavior: constructs an ExceptionDialogViewModel and shows a modal Window hosting Views/Dialogs/ExceptionDialog with MainWindow as owner; falls back to Debug.WriteLine on failure.
- Future enhancements: normalization/logging/toast host are planned; DataTemplates and a WindowService can replace direct Window creation.

## Error Taxonomy (handled categories)
1) Configuration/Registration
   - Missing Database.config, wrong location, or instance not registered. [Intro - Development Guide.txt, p.13; p.15]
2) Connection/Authentication
   - Dbms.OpenLocal failure due to invalid credentials or unreachable database. [Reference - Core.txt, p.33-37]
   - Single sign-on errors (OpenLocalSSO) such as SSO not enabled, missing domain/user SIDs, or user not mapped. [Intro - Development Guide.txt, p.14]
3) Environment/Runtime Preconditions
   - Architecture mismatch or DLL reference issues (toolkit is 32-bit x86; applications must also be built as x86). [Intro - Development Guide.txt, p.8]
   - Missing required DLL references (LsaCore.dll, LsaShared.dll, Vmfg*.dll). [Intro - Development Guide.txt, p.11-12]
4) API/Business Validation
   - InventoryTransaction schema/required fields (WORKORDER_*, QTY, TRANSACTION_DATE default), lot/serial policy violations. [Reference - Inventory.txt, p.110-113]
   - Closed/invalid Work Order status when attempting status-dependent actions. [Reference - Shop Floor.txt, p.15]
5) Service Execution
   - GeneralQuery misuse (unprepared/parameters missing) or multiple active queries without disposing previous instance. [Intro - Development Guide.txt, p.17; Reference - Shared Library.txt, p.5-24]

## Exception Handling Model
- UI Surface (this Form):
  - Non-blocking toast for low-severity issues (planned) and modal dialog for blocking errors (connection/auth, closed WO, insufficient inventory, etc.).
  - Always show the Visual/Procedure message text when available; otherwise show a friendly message with a reference code.
- Central Service (IExceptionHandler):
  - Current: Handle(ex, context) immediately shows ExceptionDialog and writes debug output as fallback (no normalization yet).
  - Planned: Map raw exceptions/results to a normalized error model (Category, Code, Severity, Title, Message, Details, SuggestedAction), decide UI modality/logging/retry.
  - Provides helpers to attach context (WO, Part, Qty, Site, Warehouse, FromLocation, ToLocation, User, Timestamp).
- Logging (planned):
  - Writes exception entries to the application database when appropriate (e.g., Closed WO, Over Receipts, Not Enough) using the tables defined in WorkOrderTransaction.md and MAMPDatabase.md.
  - For login errors, record non-sensitive diagnostics locally (no passwords).

## Form Responsibilities and Flow
- Inputs: Exception/Result object, optional context (screen name, operation, parameters), and a severity hint from the caller.
- Steps (current):
  1) Caller invokes IExceptionHandler.Handle(ex, context).
  2) Service creates ExceptionDialogViewModel and shows modal ExceptionDialog.
  3) Fallback to Debug.WriteLine if UI cannot be presented.
- Steps (planned normalization path):
  1) Normalize via service.
  2) Render UX (toast/dialog) with action choices (Retry, Cancel, Help).
  3) If configured, persist to app DB and/or append to in-memory session diagnostics.
  4) Return a result object indicating user choice and next step.

## Integration Contracts (callers)
- LoginScreen:
  - Route all connection/auth failures (OpenLocal/OpenLocalSSO), missing config, or x86/DLL issues to IExceptionHandler.Handle.
  - Use Dbms.UserID to confirm identity post-auth; if absent or mismatch, raise a handled error. [Reference - Core.txt, p.48]
- WorkOrderTransaction:
  - Route business validation failures and procedure errors (Closed WO, Over Receipt prompt/approval, Not Enough @ From Location, lot/serial) to IExceptionHandler.Handle.
- Inventory Transfer:
  - Route insufficient at From, invalid/inactive Item/Locations, and lot/serial compliance failures to IExceptionHandler.Handle.

## User Experience Guidelines
- Keep messages concise; include the operation and key context (e.g., WO 12345, Part ABC, Qty 10, From LOC1).
- Provide a "View details" expander for raw server/procedure text and a "Copy details" action.
- Ensure keyboard-first access and default focus on the safest action.

Non-Visual Data Persistence (planned)
- Use the app database mtm_visual_application (prod) or mtm_visual_application_test (dev) per environment to store exception records and diagnostics per WorkOrderTransaction.md specification. [README.md Configuration; Database.config]

Citations (file and page)
- Intro - Development Guide.txt
  - p.13 - OpenLocal reads Database.config.
  - p.14 - OpenLocalSSO and GetSingleSignOnData (domain/user SIDs).
  - p.15 - Co-locate code, API DLLs, and Database.config.
  - p.8 - Toolkit DLLs are 32-bit (x86); apps must be built as x86.
- Reference - Core.txt
  - p.33-37 - Dbms.OpenLocal overloads.
  - p.48 - Dbms.UserID returns the connected user ID.
- Reference - Shared Library.txt
  - p.5-24 - GeneralQuery Prepare/Parameters/Execute result sets.
- Reference - Inventory.txt
  - p.110-113 - InventoryTransaction fields and defaults (TRANSACTION_DATE default, required fields, TRACE rules).
- Reference - Shop Floor.txt
  - p.15 - ChangeWorkOrderStatus NEW_STATUS values (Closed/Released relevance).

## UI Scaffolding (Avalonia 11)
- Views
  - Views/Dialogs/ExceptionDialog.axaml: modal dialog (UserControl) with Title, Message, optional Details expander, context grid (WO/Part/Qty/Locations/Site/Warehouse), buttons (Retry, Cancel, Help, Copy Details).
  - Toast surface (non-modal): lightweight notification control for low-severity validations (planned).
- ViewModels
  - ViewModels/Dialogs/ExceptionDialogViewModel.cs: Title, Message, Details, IsDetailsOpen; future: Category, Code, Severity, Context model (WO, Part, Qty, Site, Warehouse, FromLocation, ToLocation, User, Timestamp); Commands: RetryCommand, CancelCommand, HelpCommand, CopyDetailsCommand.
- Services and DI
  - IExceptionHandler implemented by Services.ExceptionHandler shows the dialog via Handle(ex, context).
- Keyboard/Mouse
  - Enter = default action (Retry when available, otherwise Close); Esc = Cancel; Ctrl+C = Copy details; F1 = Help.

## References
- ../MVVMDefinitions/LoginScreen.md; ../MVVMDefinitions/WorkOrderTransaction.md; ../MVVMDefinitions/InventoryTransfer.md
- ../../References/Visual PDF Files/Text Conversion/Intro - Development Guide.txt; ../../References/Visual PDF Files/Text Conversion/Reference - Core.txt; ../../References/Visual PDF Files/Text Conversion/Reference - Shared Library.txt; ../../References/Visual PDF Files/Text Conversion/Reference - Inventory.txt; ../../References/Visual PDF Files/Text Conversion/Reference - Shop Floor.txt
- ../../README.md

## Implementation status (current)
- IExceptionHandler implemented in ../../Services/Services.cs (class ExceptionHandler) opens a modal Window hosting ExceptionDialog with details bound; commands are placeholders.
- Callers (e.g., LoginViewModel.LoginViewModel_Button_Login catch) invoke _exceptionHandler.Handle(ex, context) which presents the dialog.
