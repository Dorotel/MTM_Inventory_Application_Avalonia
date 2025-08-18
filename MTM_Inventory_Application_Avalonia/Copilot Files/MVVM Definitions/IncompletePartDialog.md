# Incomplete Part Dialog - UI Planning Specification [Ref: ../../References/Visual Highlighted Screenshots/InventoryTransferIncompletePartID.png; ./InventoryTransfer.md]

Purpose: provide a dialog to resolve incomplete/invalid Item/Part IDs by searching and selecting a valid Part. The view mimics the Visual "Parts" window layout for familiarity.

Global Rule - Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Scope
- Opens automatically when the entered Item/Part is invalid or incomplete (configurable); can also be opened explicitly from Inventory Transfer and Work Order flows.
- Seeds the search box with the caller's text (e.g., "21-28841-") and shows suggested/like matches.

Behavior toggle (setting)
- Settings.General.ResolveIncompletePartEnabled (bool, default: true)
  - true: show this dialog when part ID is incomplete/not found (assist mode).
  - false: do not open dialog; surface an error instead (strict mode).

Platform and Shell wiring
- View: Views/Dialogs/IncompletePartDialog.axaml (UserControl hosted in a modal Window). DataContext: IncompletePartDialogViewModel.
- Opened by: InventoryTransferViewModel.InventoryTransfer_Button_ResolveIncompletePartId and WorkOrderTransactionViewModel.WorkOrder_Button_ResolveIncompletePartId via IPartDialogService.
- Close behavior: Select closes with chosen Part and returns PartId to the caller; Cancel closes with null.

## Code Naming and Error Handling Rules
- File naming: {Type}_{Parent}_{Name}
- Methods: {Class}_{ControlType}_{Name}
- Variables: {Method}_{Type(Int|string|exc)}_{Name}
- All methods have try/catch and route errors via IExceptionHandler.

## Screenshot highlighting (reference only)
- The colors in the referenced screenshot are highlights only; use standard theming.

## UI and Interaction
- Toolbar/header (visual chrome only): compact icon row (accept, cancel, refresh, search, filter, clear) plus search box and filter toggles right-aligned.
- SearchText (string): initialized from caller seed; supports Contains (%seed%) and StartsWith (seed%).
- Filters: Active only; Search in description; Match mode (Contains/StartsWith).
- Results grid (read-only): columns Part ID, Description, Stock UM, Fab, Pur, Status (Active/Inactive). SelectedPart bound to the current row.
- Selection rules: only Active parts are selectable (Select disabled otherwise). Double?click row or press Enter to select; Esc cancels.

## Visual API Commands (by scenario)
- Authenticate/Connect
  - Dbms.OpenLocal(instance, user, pass) or OpenLocalSSO(instance, userName, userSID, domain, domainSID); always Close/Dispose after use. [Intro - Development Guide.txt, p.13-14; Reference - Core.txt, p.33-37]
- Search Parts
  - Use GeneralQuery with parameterized LIKE to find parts by prefix/contains; include status/UOM flags as needed. [Reference - Shared Library.txt, p.5-24]

All methods must use centralized error handling (IExceptionHandler). Implement VISUAL calls in IPartService adapters; ViewModels remain UI?only per README rules.

## Business Rules and Exceptions
- Inactive parts are not selectable unless policy permits (disable Select if Status != Active).
- Selecting a part populates the calling view's Item/Part field and refocuses Quantity.

## Workflows
- Caller sets seed ? dialog opens with seed ? type to refine ? pick row ? Select ? dialog closes and returns Part ID to caller which updates its textbox.
- Cancel ? close without changes.

## ViewModel, Commands, and Role Gating
- IncompletePartDialogViewModel properties: SearchText, Results, SelectedPart, CanSelect.
- Commands
  - IncompletePartDialog_TextBox_Search (implicit via SearchText change refresh)
  - IncompletePartDialog_Button_Select (Enter/Double?click)
  - IncompletePartDialog_Button_Cancel (Esc)
- Role gating: read?only selection for all roles.

## Integration Approach
- IPartDialogService.PickPartAsync(seed) shows the dialog and returns the chosen PartId.
- InventoryTransferViewModel.InventoryTransfer_Button_ResolveIncompletePartId calls PickPartAsync(InventoryTransfer_TextBox_ItemId) and writes the result back if not null.
- WorkOrderTransactionViewModel.WorkOrder_Button_ResolveIncompletePartId behaves similarly (placeholder for Part field; currently seeds from WorkOrderId and writes back to it per code comments).

## Keyboard and Scanner UX
- Enter selects, Esc cancels. Scanner input supported in SearchText.

## UI Scaffolding (Avalonia 11)
- Views: Views/Dialogs/IncompletePartDialog.axaml with header toolbar, seeded SearchText, results DataGrid, Select/Cancel.
- ViewModels: ViewModels/Dialogs/IncompletePartDialogViewModel.cs with seed?based suggestions and selection/cancel.
- Services and DI: IPartDialogService to open modal dialog and return result; integrated into NavigationService to supply dialogs to feature VMs.

## Testing and Acceptance Criteria
- Opening dialog from Inventory Transfer seeds search and returns selection to textbox.
- Strict/assist behavior follows Settings.General.ResolveIncompletePartEnabled.
- Error paths surface via IExceptionHandler.

## References
- ../../References/Visual Highlighted Screenshots/InventoryTransferIncompletePartID.png
- ../../References/Visual PDF Files/Text Conversion/Reference - Core.txt; Reference - Shared Library.txt; Intro - Development Guide.txt

## Implementation status (scaffold)
- View created and polished.
- ViewModel implemented with seed-based suggestions and selection/cancel events.
- Service (IPartDialogService) added; integrated into NavigationService to supply dialogs to feature VMs.