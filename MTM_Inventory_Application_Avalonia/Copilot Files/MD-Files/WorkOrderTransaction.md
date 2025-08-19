# Work Order Transaction Form - Functional and Technical Specification
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `Views/WorkOrderTransactionView.axaml`
- **ViewModel:** `ViewModels/WorkOrderTransactionViewModel.cs`
- **Primary Commands:** ValidateWorkOrder, CheckAvailability, PostTransaction, Reset, ResolveIncompletePartId, ShowLocationModal
- **Related Dialogs:** LocationPickerDialog, IncompletePartDialog
- **Last Updated:** 2024-12-19
- **Copilot Template Version:** 1.1

---

## Purpose

Work order material transaction form for issuing materials to production work orders and receiving finished goods or byproducts back from production.

---

## Global Rules

- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.
- ALL methods implement try/catch and route errors through IExceptionHandler with the same normalization routine

---

## Scope

- Issue materials to work orders (raw materials, components)
- Receive finished goods and byproducts from production
- Validate work order status and material requirements
- Support barcode scanning for work orders and locations
- Post transactions to Visual system via work order APIs

---

## Platform and Shell Wiring

- **View:** `Views/WorkOrderTransactionView.axaml` (UserControl)
- **Host:** Displayed in MainView content area via CurrentView binding
- **Navigation:** Created by MainViewModel.MainView_Button_OpenWorkOrderTransactionCommand
- **Dialogs:** LocationPickerDialog, IncompletePartDialog (Ctrl+F)

---

## Current Implementation Details

**WorkOrderTransactionView.axaml Structure:**
- Header with clipboard-list-outline icon and "Work Order Transaction" title
- **Search Panel** (expandable, default expanded):
  - Work Order ID with magnify button (Ctrl+F)
  - Transaction Type ComboBox (Issue/Receipt)
  - Quantity text input
  - From Location with location picker
  - To Location with location picker
- **Results Panel** with DataGrid for transaction history/validation results
- **Actions Panel** with Validate, Check Availability, Post Transaction, Reset buttons

**Key UI Features:**
- Keyboard shortcut: Ctrl+F for work order lookup/part resolution
- Transaction type selection (Issue materials vs Receive products)
- Location picker integration for From/To locations
- Expandable panels for organized workflow

---

## Fields and Validation Rules

**Input Fields:**
- `WorkOrder_TextBox_WorkOrderId` (string) - Work order number
- `WorkOrder_ComboBox_TransactionTypeIndex` (int) - 0=Issue, 1=Receipt
- `WorkOrder_TextBox_Quantity` (string) - Transaction quantity
- `WorkOrder_TextBox_FromLocation` (string) - Source location
- `WorkOrder_TextBox_ToLocation` (string) - Destination location

**Read-Only Labels:**
- `WorkOrder_Label_WarehouseId` = "002" (default warehouse)
- `WorkOrder_Label_SiteId` = "SITE" (default site)

**Results DataGrid:**
- `WorkOrder_DataGrid_Results` - ReadOnlyObservableCollection of transaction records
- Record type: WorkOrderResultRow with PartNumber, Description, FromLocation, ToLocation, AvailableQuantity, Uom, WarehouseId, SiteId

---

## Visual API Commands (by scenario)

**Work Order Validation:**
- TODO: GetWorkOrderSummary to validate work order exists and is active
- Retrieve work order details and material requirements

**Material Issue (To Work Order):**
- TODO: InventoryTransaction with work order context
- Move materials from inventory to work order consumption

**Product Receipt (From Work Order):**
- TODO: InventoryTransaction for finished goods receipt
- Move completed products from work order to inventory

**Availability Check:**
- TODO: Query on-hand quantities for required materials
- Validate sufficient inventory before issuing

---

## Business Rules and Exceptions

- Work order must exist and be in active status
- Materials can only be issued if sufficient inventory available
- Receipts must match work order routing and expected outputs
- Transaction dates must be within work order date range
- All errors route through Exception Handling Form

---

## Workflows

**Material Issue Workflow:**
1. **Work Order Entry:** Enter/scan work order number → validate work order status
2. **Transaction Type:** Select "Issue" (index 0)
3. **Location Selection:** Specify From Location (inventory) and To Location (work order staging)
4. **Quantity Entry:** Specify quantity to issue
5. **Availability Check:** Verify sufficient material inventory
6. **Issue Posting:** Submit material issue transaction

**Product Receipt Workflow:**
1. **Work Order Entry:** Enter/scan work order number → validate work order status
2. **Transaction Type:** Select "Receipt" (index 1)
3. **Location Selection:** Specify From Location (production) and To Location (finished goods)
4. **Quantity Entry:** Specify quantity received
5. **Receipt Posting:** Submit finished goods receipt transaction

---

## ViewModel/Command Conventions

**WorkOrderTransactionViewModel Properties:**
- Input fields with ObservableProperty attributes
- ReadOnlyObservableCollection for DataGrid binding
- IPartDialogService for enhanced work order/part selection (when injected)

**Commands:**
- `WorkOrder_Button_ValidateWorkOrderCommand` - validate work order
- `WorkOrder_Button_CheckAvailabilityCommand` - check material availability
- `WorkOrder_Button_PostTransactionCommand` - submit transaction
- `WorkOrder_Button_ResetCommand` - clear all fields
- `WorkOrder_Button_ResolveIncompletePartIdCommand` - part/WO lookup dialog
- `WorkOrder_Button_ShowLocationModalCommand` - location picker

**Service Integration:**
- IExceptionHandler for error handling
- IPartDialogService for enhanced selection (optional)
- Direct window creation for dialogs when service not available

---

## Integration & Storage

**Services Used:**
- IExceptionHandler for centralized error handling
- IPartDialogService for work order/part selection (optional enhancement)
- Visual API services for work order validation and transaction posting

**Data Storage:**
- Results displayed in DataGrid for session visibility
- No local persistence beyond current session
- Audit trail through Visual system transaction logs

---

## Keyboard/Scanner UX

**Keyboard Shortcuts:**
- Ctrl+F: Resolve incomplete part/work order ID (opens search dialog)

**Scanner Support:**
- Work order ID field accepts barcode input
- Location fields accept location barcode input
- Tab navigation between fields for efficient data entry

**Planned Enhancements:**
- F2/F3 shortcuts for location pickers
- Enter key for form submission
- Esc for reset/cancel

---

## UI Scaffold

**Views:**
- `Views/WorkOrderTransactionView.axaml` - main transaction form
- `Views/Dialogs/LocationPickerDialog.axaml` - location selection
- `Views/Dialogs/IncompletePartDialog.axaml` - work order/part search

**ViewModels:**
- `ViewModels/WorkOrderTransactionViewModel.cs` - transaction logic
- `ViewModels/Dialogs/LocationPickerDialogViewModel.cs` - location picker
- `ViewModels/Dialogs/IncompletePartDialogViewModel.cs` - search functionality

**Data Types:**
- `WorkOrderResultRow` record for DataGrid display
- Service interfaces for Visual API integration

---

## Testing & Acceptance

- Form loads with proper layout and expandable panels
- Transaction type ComboBox switches between Issue/Receipt modes
- Work order validation connects to Visual work order master
- Location picker provides valid warehouse locations
- Availability check shows accurate inventory/work order status
- Transaction posting creates proper Visual work order transactions
- Reset command clears all input fields
- All errors display through Exception Handling Form

---

## References

- ../../Views/WorkOrderTransactionView.axaml (UI implementation)
- ../../ViewModels/WorkOrderTransactionViewModel.cs (business logic)
- ../../References/Visual Highlighted Screenshots/WorkOrderScreenshotHighlighted.png
- ./ExceptionHandling.md (error handling flow)
- ./LocationPickerDialog.md (location selection)
- ./IncompletePartDialog.md (search functionality)

---

## Implementation Status

- **Current:** Basic UI and ViewModel structure implemented
- **TODO:** Visual API integration for work order validation and posting
- **Transaction Types:** Issue/Receipt selection implemented
- **Dialogs:** Location picker and search dialogs functional
- **Navigation:** Integrated with MainView navigation system

---

## TODOs / Copilot Agent Notes

- [ ] Implement GetWorkOrderSummary API for work order validation
- [ ] Implement material issue transactions via InventoryTransaction
- [ ] Implement finished goods receipt transactions
- [ ] Add work order status and material requirements display
- [ ] Consider adding work order routing step selection
- [ ] Enhance barcode scanning integration for work orders