# Location Picker Dialog - UI Planning Specification [Ref: ../../References/Visual Highlighted Screenshots/InventoryTransferLocationScreen.png; ./InventoryTransfer.md]

Purpose: provide a modal dialog to browse/filter/select a location for the current Item/Part during Inventory Transfer, with live header/footer descriptions.

Global Rule - Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Scope
- Opens from Inventory Transfer when the user presses the Location button (F2) to pick either From or To location.
- Displays filters and a read-only grid of location balances; selection applies the location to the active target and closes.

Platform and Shell wiring
- View: Views/Dialogs/LocationPickerDialog.axaml (Window/Dialog). DataContext: LocationPickerDialogViewModel.
- Opened by: InventoryTransferView via ShowLocationModal command.
- Close behavior: double-click row, Enter to select; Esc to cancel.

## Code Naming and Error Handling Rules
- File naming: {Type}_{Parent}_{Name}
- Methods: {Class}_{ControlType}_{Name}
- Variables: {Method}_{Type(Int|string|exc)}_{Name}
- All methods must have error handling and must use the same centralized error handling (IExceptionHandler).

## Screenshot highlighting (reference only)
- The colors in the referenced screenshot are highlights only; use standard theming. Follow functional grouping for header/footer, filters, and results grid.

## Fields and Validation Rules
- Inputs (filters): Text contains, Toggles (NonZeroOnly, CurrentWarehouseOnly, CurrentSiteOnly), Optional filters (BinType, Lot/Serial).
- Results grid (read-only): Location, OnHand, Allocated, Available, Warehouse, Site, BinType, LotControlled, SerialControlled, LastTxUtc.
- Header/Footer descriptions (read-only):
  - Top: "{UserName} Transfered from {FromLocation} using MTM-App." (live updates)
  - Bottom: "{UserName} Transfered to {ToLocation} using MTM-App." (live updates)
- ApplyTo target: From or To; default uses current focus in the calling view.

## Visual API Commands (by scenario)
- Authenticate/Connect
  - Dbms.OpenLocal(instance, user, pass) or OpenLocalSSO(instance, userName, userSID, domain, domainSID); always Close/Dispose after use. [Intro - Development Guide.txt, p.13-14; Reference - Core.txt, p.33-37]
- Load locations for Item/Part
  - Use GeneralQuery to retrieve location balances scoped by Item, Warehouse/Site (parameterized). Apply filters client-side. [Reference - Shared Library.txt, p.5-24]

All methods must use centralized error handling (IExceptionHandler): wrap calls in try/catch and normalize via ExceptionHandler. Implement VISUAL calls in service adapters (IInventoryService/ILocationService), not in ViewModels/Views.

## Business Rules and Exceptions
- Only locations belonging to the current Warehouse/Site are selectable when the corresponding toggles are set.
- If Item/Part is missing/invalid, dialog should not load; prompt user in calling view.

## Workflows
- Open ? Load results for current Item ? Filter as needed ? Select row (Enter/double-click) ? Apply to From/To ? Close.
- Cancel (Esc) ? Close without changes.

## ViewModel, Commands, and Role Gating
- LocationPickerDialogViewModel properties (current implementation)
  - HeaderText (string)
  - Filters_Text (string)
  - Filters_NonZeroOnly (bool)
  - Filters_CurrentWarehouseOnly (bool)
  - Filters_CurrentSiteOnly (bool)
  - Results (ObservableCollection<LocationBalanceDto>)
  - SelectedRow (LocationBalanceDto?)
- Commands (placeholders)
  - LocationPickerDialog_Button_Select
  - LocationPickerDialog_Button_Cancel
- Role gating
  - Read-Only: dialog is read-only; selection allowed.

## Integration Approach
- InventoryTransferViewModel opens the dialog and receives the selected location; updates FromLocation or ToLocation accordingly (planned wiring).
- Use DI to provide IInventoryService/IExceptionHandler; run GeneralQuery in a service layer.

## Keyboard and Scanner UX
- Enter - select selected row; Esc - cancel; F2 pressed in main screen opens this dialog.

## UI Scaffolding (Avalonia 11)
- Views
  - Views/Dialogs/LocationPickerDialog.axaml: header/footer labels; filters; DataGrid; ApplyTo toggle; Select/Cancel buttons.
- ViewModels
  - ViewModels/Dialogs/LocationPickerDialogViewModel.cs as above; INotifyDataErrorInfo for input validation (planned).
- Services and DI
  - ILocationService (read model), IExceptionHandler; IClock for LastTxUtc display.

## Testing and Acceptance Criteria
- Opening from FromLocation context applies selection to FromLocation.
- Opening from ToLocation context applies selection to ToLocation.
- Filters apply instantly; results limited to current Warehouse/Site when toggles set.
- Header/Footer descriptions update live to reflect user and current From/To input.

## References (artifacts used by this document)
- ./InventoryTransfer.md
- ../../References/Visual Highlighted Screenshots/InventoryTransferLocationScreen.png
- ../../References/Visual PDF Files/Text Conversion/Reference - Core.txt; Reference - Shared Library.txt; Intro - Development Guide.txt

## Implementation status (scaffold)
- View created: ../../Views/Dialogs/LocationPickerDialog.axaml
- ViewModel created: ../../ViewModels/Dialogs/LocationPickerDialogViewModel.cs
- Open from InventoryTransferView (planned) via InventoryTransfer_Button_ShowLocationModal command; returns selected location to caller.
- Contains placeholder commands and DTOs; VISUAL read models to be implemented in services.