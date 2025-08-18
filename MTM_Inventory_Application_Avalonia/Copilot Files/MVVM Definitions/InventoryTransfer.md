# Inventory Transfer (Location-to-Location) Form - Functional and Technical Specification [Ref: ../../References/Visual Highlighted Screenshots/InventoryTransferHighights.png; ../../References/Visual Highlighted Screenshots/InventoryTransferLocationScreen.png; ../../References/Visual Highlighted Screenshots/InventoryTransferIncompletePartID.png]

Purpose: define UI, validations, workflow, role gating, integration, storage, and configuration to implement Location-to-Location inventory transfers within the same warehouse in this Avalonia/.NET 8 application.

Global Rule - Visual license lifecycle
- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.

Scope: transfers on-hand quantity for an Item/Part from a From Location to a To Location in the same warehouse; optimized for keyboard/barcode workflows; centralized exception handling and local reporting/logging.

Platform: Avalonia 11 MVVM on .NET 8 with desktop lifetime; view binds to InventoryTransferViewModel.

Shell wiring: open from MainView via NavigationService.OpenInventoryTransfer(); MainView hosts the feature view in its content region. [Ref: ../../App.axaml.cs]

## Code Naming and Error Handling Rules
- File naming: {Type}_{Parent}_{Name}
- Methods: {Class}_{ControlType}_{Name}
- Variables: {Method}_{Type(Int|string|exc)}_{Name}
- All methods must have error handling and must use the same centralized error handling (IExceptionHandler).

## Screenshot highlighting (reference only)
- The colored highlights in the screenshots are illustrative to indicate field roles and dialog sections. They do not imply application theming. Implement standard app styling; follow the functional groupings shown by the highlights.
- Highlights map to: fixed context (e.g., Warehouse/Site/Date), user inputs (Item/Part, Locations, Quantity), and derived/display values (description, balances, flags, warnings).

## Top/Bottom Description text
- Mapping: Old Location = From Location; New Location = To Location.
- Top text: {UserName} + " Transfered from " + {FromLocation} + " using MTM-App."
- Bottom text: {UserName} + " Transfered to " + {ToLocation} + " using MTM-App."
- Render within the Location dialog header/footer; update live as user edits From/To and when user context changes.

## Fields and Validation Rules
- Item/Part ID (required): must exist and be active; if not found or incomplete, open the Incomplete Part ID dialog (planned).
- From Location (required): must exist, be active, and have sufficient on-hand for transfer.
- To Location (required): must exist and be active; cannot equal From Location.
- Quantity (required): > 0; precision per item; if lot/serial controlled, enforce capture before post.
- Warehouse ID (read-only): fixed from settings (e.g., "002"); From/To must belong to this warehouse.
- Site ID (read-only): derived from environment configuration. [Ref: ../../References/Visual PDF Files/Text Conversion/Reference - VMFG Shared Library.txt]
- Transaction Date (read-only): auto-set to current date. [Ref: ../../References/Visual PDF Files/Text Conversion/Reference - Inventory.txt]

## Derived/Display
- Item description, lot/serial flags.
- On-hand/Allocated/Available at From Location.
- Any validation/exception messages (rendered by Exception Handling service/modal).

## Visual API Commands (by scenario)
- Authenticate/Connect
  - Dbms.OpenLocal(instance, user, pass) or OpenLocalSSO(instance, userName, userSID, domain, domainSID); always Close/Dispose after use. [Intro - Development Guide.txt, p.13-14; Reference - Core.txt, p.33-37]
  - Confirm identity: Dbms.UserID(instance). [Reference - Core.txt, p.48]
- Validate Item/Part
  - Use GeneralQuery to verify Part exists/active (parameterized query). [Reference - Shared Library.txt, p.5-24]
  - Optional defaulting: VmfgShared.GetPartDefaultWhseLoc(part) to suggest warehouse/location. [Reference - VMFG Shared Library.txt, p.5-6]
- Check Availability at From Location
  - Use GeneralQuery to read On-hand/Allocated/Available for Part at From Warehouse/Location (parameterized). [Reference - Shared Library.txt, p.5-24]
- Post Location-to-Location Transfer
  - InventoryTransaction: populate required fields (TRANSACTION_DATE, QTY, ITEM_NO, FROM_WAREHOUSE/LOCATION, TO_WAREHOUSE/LOCATION; include TRACE rows for lot/serial when required) and execute/save. [Reference - Inventory.txt, p.110-113]
- Lot/Serial Capture
  - Populate TRACE sub-table entries per lot/serial control before executing the InventoryTransaction. [Reference - Inventory.txt, p.112-113]

All methods must use centralized error handling (IExceptionHandler): wrap each of the calls above in try/catch and normalize via ExceptionHandler. Production VISUAL calls are implemented in service adapters, not in ViewModels/Views.

## Location dialog (triggered by Location button)
- Purpose: show all current locations for the entered Item/Part ID with ability to filter and select a location.
- Trigger: a button on the Location section (per InventoryTransferLocationScreen.png) bound to InventoryTransfer_Button_ShowLocationModalCommand. If focus is in From Location, selection applies to From; if focus is in To Location, selection applies to To.
- Contents:
  - Header label (top description): "{UserName} Transfered from {FromLocation} using MTM-App." (updates live)
  - Filters row: 
    - Text filter: Location/Contains
    - Toggles: Only non-zero OnHand; Only current Warehouse; Only current Site
    - Optional: Bin type filter; Lot/Serial filter
  - Results grid (read-only): columns Location, OnHand, Allocated, Available, Warehouse, Site, BinType, LotControlled, SerialControlled, LastTxUtc
  - Footer label (bottom description): "{UserName} Transfered to {ToLocation} using MTM-App." (updates live)
- Behavior:
  - Opens as a modal dialog; loads asynchronously once valid Item/Part is present.
  - Selecting a row (double-click or Enter) applies the selected Location to the active target (From/To) and closes the dialog.
  - Filters apply client-side immediately; server-side query can be re-run on demand if implemented.
  - Esc closes dialog without selection.

## Incomplete Part ID window (secondary flow)
- Trigger:
  - Automatically when Item/Part ID is missing/invalid or incomplete (e.g., prefix only), or
  - Via explicit command from Inventory Transfer.
- Behavior:
  - Dialog with a search box (supports prefix/contains), scan input, and results list (Part ID, Description, status).
  - Allows selecting a part to populate Item/Part ID on the main form.
  - Optional filters: active only, warehouse/site scope.
  - Close on selection; refocus Quantity.

## Business Rules and Exceptions
- Insufficient at From Location: if On-hand < Quantity, block posting; log to app DB not_enough_location (from_location, item_no, requested_qty, onhand_qty, user, timestamp; optional extra_json).
- Invalid/Inactive Item or Locations: block and show message; do not post.
- From and To must be different and belong to the same warehouse; else block and message.
- Lot/Serial compliance (if item-controlled): enforce capture before posting. [Ref: ../../References/Visual PDF Files/Text Conversion/Reference - Inventory.txt]
- All exception paths surface through centralized Exception Handling service/modal (see ../MVVM Definitions/ExceptionHandling.md).

## Workflows
- Happy path: enter Item/Part ? load item details ? enter From Location, To Location, Quantity ? current date auto ? optionally open Location dialog ? validate availability ? Post Transfer ? write Visual transaction + local audit ? clear.
- Incomplete Part ID: enter partial ? open Incomplete Part window ? select part ? continue as above.
- Not Enough at From: show message; log to not_enough_location; stop.

## ViewModel, Commands, and Role Gating (Avalonia MVVM)
- ViewModel: InventoryTransferViewModel exposes fields with current code names:
  - InventoryTransfer_TextBox_ItemId, InventoryTransfer_TextBox_Quantity,
    InventoryTransfer_TextBox_FromLocation, InventoryTransfer_TextBox_ToLocation,
    InventoryTransfer_Label_WarehouseId (ro), InventoryTransfer_Label_SiteId (ro)
- Commands implemented:
  - InventoryTransfer_Button_ValidateItemCommand
  - InventoryTransfer_Button_CheckAvailabilityFromCommand
  - InventoryTransfer_Button_ShowLocationModalCommand
  - InventoryTransfer_Button_PostTransferCommand
  - InventoryTransfer_Button_ResetCommand
  - InventoryTransfer_Button_ResolveIncompletePartIdCommand (implemented, uses IPartDialogService to open Incomplete Part dialog and write back selection)
- Role gating: enable/disable inputs and commands per role; Read-Only role cannot post or create/modify exception entries (client and server enforced).

## Integration Approach
- Use official Visual APIs/procedures for inventory transfer posting and balance lookup when available; do not write directly to Visual transactional tables. [Ref: ../../References/Visual PDF Files/Text Conversion/Reference - Inventory.txt]
- License lifecycle: acquire Visual license per call and release immediately after completion (success or failure).
- Keep VISUAL calls in service adapters (IInventoryService/ILocationService) per README rules; ViewModel remains UI-only.

## Local Storage and Reporting (app-owned DB)
- Write a local audit to local_tx_history with tx_type = "Transfer" including: item_no, qty, site_id, warehouse_id, from_location, to_location, user_id, tx_utc (UTC), source_ref (e.g., "InventoryTransferForm"), and extra_json containing the two description texts.
- On insufficient at From, write to not_enough_location.
- See ../MVVM Definitions/MAMPDatabase.md for MySQL 5.7.24 DDL and index definitions.

## Keyboard and Scanner UX
- Default focus: Item/Part ? Quantity ? From Location ? To Location.
- Enter commits field; Esc closes dialogs; F2 opens Location dialog; F3 opens Incomplete Part window.
- Support barcode scans into Item/Part and Locations; debounce lookups; in Location dialog, hitting Enter applies the highlighted row.

## UI Scaffolding (Avalonia 11)
- Views
  - Views/InventoryTransferView.axaml: main form (grid layout; sections for Item, Quantity, Locations, and read-only labels for fixed context; standard app styling).
  - Views/Dialogs/LocationPickerDialog.axaml: dialog with header/footer description labels; filters; results DataGrid; ApplyTo (From/To) toggle; Select/Cancel.
  - Views/Dialogs/IncompletePartDialog.axaml: search box, scan input, results DataGrid, Select/Cancel.
- ViewModels
  - ViewModels/InventoryTransferViewModel.cs (fields/commands as listed above).
  - ViewModels/Dialogs/LocationPickerDialogViewModel.cs: HeaderText; Filters (Filters_Text, Filters_NonZeroOnly, Filters_CurrentWarehouseOnly, Filters_CurrentSiteOnly); Results; SelectedRow; Commands: LocationPickerDialog_Button_Select, LocationPickerDialog_Button_Cancel.
  - ViewModels/Dialogs/IncompletePartDialogViewModel.cs: SearchText, Results, SelectedPart; Commands: IncompletePartDialog_Button_Select, IncompletePartDialog_Button_Cancel.
- Services and DI (registration example)
  - IInventoryService, IExceptionHandler, IClock, ISettings, IAppDb.
  - Register in App bootstrapper or DI container; resolve in ViewModels via constructor injection.
- DataTemplates
  - App.axaml: map ViewModel to View using DataTemplates for dialogs; or open dialogs via Interaction/WindowService pattern.
- Navigation
  - Open from MainView via NavigationService.OpenInventoryTransfer(); MainView hosts the view in its content area.

## Testing and Acceptance Criteria
- Implement field behaviors per the screenshot highlights (reference only); no special color theming is required in the application.
- Location button opens the Location dialog showing current locations for the entered Item/Part; selecting a row sets the active From/To location.
- The two description strings render exactly as: "{UserName} Transfered from {FromLocation} using MTM-App." and "{UserName} Transfered to {ToLocation} using MTM-App." (updates live).
- Incomplete Part ID window appears on invalid/incomplete Item/Part ID and allows selecting a valid Part ID.
- From Location ? To Location and both in WarehouseId; posting with equal or mismatched warehouse is blocked.
- Posting blocked on insufficient on-hand; a not_enough_location row is created.
- A local_tx_history row is created on successful transfer with tx_type = "Transfer" and extra_json as above.

## References (artifacts used by this document)
- ../../References/Visual Highlighted Screenshots/InventoryTransferHighights.png; ../../References/Visual Highlighted Screenshots/InventoryTransferLocationScreen.png; ../../References/Visual Highlighted Screenshots/InventoryTransferIncompletePartID.png
- ../../References/Visual PDF Files/Text Conversion/Reference - Inventory.txt; ../../References/Visual PDF Files/Text Conversion/Reference - Shared Library.txt; ../../References/Visual PDF Files/Text Conversion/Reference - VMFG Shared Library.txt; ../../References/Visual PDF Files/Text Conversion/Reference - Core.txt
- ./ExceptionHandling.md; ./MAMPDatabase.md; ../../App.axaml.cs

## Implementation status (scaffold)
- View created: ../../Views/InventoryTransferView.axaml
- ViewModel created: ../../ViewModels/InventoryTransferViewModel.cs
- Open from MainView via NavigationService.OpenInventoryTransfer().
- Commands implemented as listed; ResolveIncompletePartId wired to IPartDialogService. Errors route through IExceptionHandler.
