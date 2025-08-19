# Exception Dialog - UI Planning Specification [Ref: ./ExceptionHandling.md]

Purpose: define the modal dialog UI that surfaces blocking errors normalized by IExceptionHandler, including details and user actions.

Global Rule - Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Scope
- Modal dialog shown for blocking errors (connection/auth, closed WO, insufficient inventory, etc.).
- Non-modal toast handles low-severity validations (planned in ExceptionHandling.md).

Platform and Shell wiring
- View: Views/Dialogs/ExceptionDialog.axaml (UserControl hosted in a modal Window). DataContext: ExceptionDialogViewModel.
- Opened by: IExceptionHandler.Handle(exception, context) (see ExceptionHandling.md and ../../Services/Services.cs implementation).

## Code Naming and Error Handling Rules
- File naming: {Type}_{Parent}_{Name}
- Methods: {Class}_{ControlType}_{Name}
- Variables: {Method}_{Type(Int|string|exc)}_{Name}
- All methods must have error handling and must use the same centralized error handling (IExceptionHandler).

## Screenshot highlighting (reference only)
- No screenshot required. Use standard app styling.

## Fields and Validation Rules
- Model (read-only): Title, Message, Details, IsDetailsOpen (current implementation). Future: Category, Code, Severity, Context (WO, Part, Qty, Site, Warehouse, FromLocation, ToLocation, User, Timestamp).
- Buttons: Retry, Cancel, Help, Copy Details; enable/disable per model.

## Visual API Commands (by scenario)
- None. This dialog does not call VISUAL APIs; it only presents error information and returns user choice.

## Business Rules and Exceptions
- Always display available server/procedure message text when present.
- Provide a "View details" expander and a "Copy details" action.

## Workflows
- Exception raised -> IExceptionHandler.Handle constructs ExceptionDialogViewModel -> modal Window hosting ExceptionDialog is shown -> user selects action -> window closes.

## ViewModel, Commands, and Role Gating
- ExceptionDialogViewModel properties: Title, Message, Details, IsDetailsOpen.
- Commands
  - ExceptionDialog_Button_Retry
  - ExceptionDialog_Button_Cancel
  - ExceptionDialog_Button_Help
  - ExceptionDialog_Button_CopyDetails
- Role gating: not applicable.

## Integration Approach
- IExceptionHandler (Services.ExceptionHandler) creates a Window with ExceptionDialog as Content, sets DataContext to a new ExceptionDialogViewModel populated from exception/context, and calls ShowDialog(owner).

## Keyboard and Scanner UX
- Enter = default action (Retry if available, otherwise Close);
- Esc = Cancel; Ctrl+C = Copy details; F1 = Help.

## UI Scaffolding (Avalonia 11)
- Views: Views/Dialogs/ExceptionDialog.axaml with header, message, details expander, context grid placeholder, buttons.
- ViewModels: ViewModels/Dialogs/ExceptionDialogViewModel.cs with commands above.

## Testing and Acceptance Criteria
- Model fields render correctly.
- Retry/Cancel/Help/CopyDetails commands are wired (placeholders now).
- Details can be expanded/collapsed; Copy copies all fields to clipboard (planned).

## References (artifacts used by this document)
- ./ExceptionHandling.md
- ../../README.md

## Implementation status (current)
- View created: ../../Views/Dialogs/ExceptionDialog.axaml
- ViewModel created: ../../ViewModels/Dialogs/ExceptionDialogViewModel.cs
- IExceptionHandler implemented in ../../Services/Services.cs shows this dialog via Handle(ex, context).
