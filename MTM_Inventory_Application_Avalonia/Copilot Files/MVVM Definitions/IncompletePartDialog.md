# Incomplete Part Dialog — UI Planning Specification [Ref: ../../References/Visual Highlighted Screenshots/InventoryTransferIncompletePartID.png; ./InventoryTransfer.md]

Purpose: provide a dialog to resolve incomplete/invalid Item/Part IDs by searching and selecting a valid Part.

Global Rule — Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short?lived, per?request scope to acquire and dispose the license.

Scope
- Opens automatically when the entered Item/Part is invalid or incomplete; can also be opened explicitly from Inventory Transfer.

Platform and Shell wiring
- View: Views/Dialogs/IncompletePartDialog.axaml (Window/Dialog). DataContext: IncompletePartDialogViewModel.
- Opened by: InventoryTransferView via ResolveIncompletePartId command.
- Close behavior: Select closes with chosen Part; Cancel closes without changes.

## Code Naming and Error Handling Rules
- File naming: {Type}_{Parent}_{Name}
- Methods: {Class}_{ControlType}_{Name}
- Variables: {Method}_{Type(Int|string|exc)}_{Name}
- All methods must have error handling and must use the same centralized error handling (IExceptionHandler).

## Screenshot highlighting (reference only)
- The colors in the referenced screenshot are highlights only; use standard theming.

## Fields and Validation Rules
- SearchText (string): supports prefix/contains; debounce input.
- Results (read-only grid): Part ID, Description, Status (Active/Inactive).
- Filters (optional): Active only; Warehouse/Site scope.
- SelectedPart: required to enable Select.

## Visual API Commands (by scenario)
- Authenticate/Connect
  - Dbms.OpenLocal(instance, user, pass) or OpenLocalSSO(instance, userName, userSID, domain, domainSID); always Close/Dispose after use. [Intro - Development Guide.txt, p.13–14; Reference - Core.txt, p.33–37]
- Search Parts
  - Use GeneralQuery with parameterized LIKE to find parts by prefix/contains; include status as needed. [Reference - Shared Library.txt, p.5–24]

All methods must use centralized error handling (IExceptionHandler): wrap calls in try/catch and normalize via ExceptionHandler. Implement VISUAL calls in IPartService adapters; ViewModels remain UI-only per README rules.

## Business Rules and Exceptions
- Inactive parts are not selectable unless policy permits.
- Selecting a part populates the calling view's Item/Part field and refocuses Quantity.

## Workflows
- Type or scan prefix ? debounced search ? pick row ? Select ? dialog closes and returns Part ID to caller.
- Cancel ? close without changes.

## ViewModel, Commands, and Role Gating
- IncompletePartDialogViewModel properties: SearchText, Results, SelectedPart; IsBusy.
- Commands
  - IncompletePartDialog_TextBox_Search
  - IncompletePartDialog_DataGrid_Select
  - IncompletePartDialog_Button_Cancel
- Role gating: all roles can use the dialog; read-only selection only.

## Integration Approach
- InventoryTransferViewModel calls SearchAsync/Select on the dialog ViewModel.
- IPartService executes parameterized queries; injected via DI.

## Keyboard and Scanner UX
- Enter selects, Esc cancels. Scanner input supported in SearchText.

## UI Scaffolding (Avalonia 11)
- Views: Views/Dialogs/IncompletePartDialog.axaml with search box, scan input, results grid, Select/Cancel.
- ViewModels: ViewModels/Dialogs/IncompletePartDialogViewModel.cs with INotifyDataErrorInfo and async search.
- Services and DI: IInventoryService, IExceptionHandler.

## Testing and Acceptance Criteria
- Invalid/incomplete Item/Part triggers dialog automatically.
- Search shows results filtered by input; selection populates the main form and focuses Quantity.
- Error paths surface via Exception Handling Form.

## References (artifacts used by this document)
- ./InventoryTransfer.md
- ../../References/Visual Highlighted Screenshots/InventoryTransferIncompletePartID.png
- ../../References/Visual PDF Files/Text Conversion/Reference - Core.txt; Reference - Shared Library.txt; Intro - Development Guide.txt

## Implementation status (scaffold)
- View created: ../../Views/Dialogs/IncompletePartDialog.axaml
- ViewModel created: ../../ViewModels/Dialogs/IncompletePartDialogViewModel.cs
- Open from InventoryTransferView (planned) via ResolveIncompletePartId command; returns selected Part ID to caller.
- Placeholder commands and results; VISUAL search via GeneralQuery to be implemented in services.