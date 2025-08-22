# Inventory Transfer (Location-to-Location) Form - Functional and Technical Specification
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `Views/InventoryTransferView.axaml`
- **ViewModel:** `ViewModels/InventoryTransferViewModel.cs`
- **Primary Commands:** ValidateItem, CheckAvailabilityFrom, ShowLocationModal, PostTransfer, Reset, ResolveIncompletePartId
- **Related Dialogs:** LocationPickerDialog, IncompletePartDialog
- **Last Updated:** 2024-12-19
- **Copilot Template Version:** 1.1

---

## Purpose

Location-to-location inventory transfer form for moving inventory between warehouse locations within the same site. Provides validation, availability checking, and posting capabilities.

---

## Global Rules

- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.
- ALL methods implement try/catch and route errors through IExceptionHandler with the same normalization routine

---

## Scope

- Transfer inventory between locations within the same warehouse/site
- Validate part numbers and check availability
- Support barcode scanning for part numbers and locations
- Post transfers to Visual system via InventoryTransaction API
- Provide audit trail and error handling

---

## Platform and Shell Wiring

- **View:** `Views/InventoryTransferView.axaml` (UserControl)
- **Host:** Displayed in MainView content area via CurrentView binding
- **Navigation:** Created by MainViewModel.MainView_Button_OpenInventoryTransferCommand
- **Dialogs:** LocationPickerDialog (F2/F3), IncompletePartDialog (Ctrl+F)

---

## Current Implementation Details

**InventoryTransferView.axaml Structure:**
- Header with swap-horizontal icon and "Inventory Transfer" title
- **Search Panel** (expandable, default expanded):
  - Part Number with magnify button (Ctrl+F)
  - Quantity text input
  - From Location with map-marker button (F2)
  - To Location with map-marker button (F3)
- **Results Panel** with DataGrid for transfer history/validation results
- **Actions Panel** with Validate, Check Availability, Post Transfer, Reset buttons

**Key UI Features:**
- Keyboard shortcuts: Ctrl+F (find part), F2 (from location), F3 (to location)
- Icon buttons for location picker and part resolution
- Expandable panels for organized workflow
- DataGrid with auto-width columns for results

---

## Fields and Validation Rules

**Input Fields:**
- `InventoryTransfer_TextBox_ItemId` (string) - Part number, supports partial entry
- `InventoryTransfer_TextBox_Quantity` (string) - Transfer quantity 
- `InventoryTransfer_TextBox_FromLocation` (string) - Source location
- `InventoryTransfer_TextBox_ToLocation` (string) - Destination location

**Read-Only Labels:**
- `InventoryTransfer_Label_WarehouseId` = "002" (default warehouse)
- `InventoryTransfer_Label_SiteId` = "SITE" (default site)

**Results DataGrid:**
- `InventoryTransfer_DataGrid_Results` - ReadOnlyObservableCollection of transfer records
- Record type: InventoryTransferResultRow with PartNumber, Description, FromLocation, ToLocation, AvailableQuantity, Uom, WarehouseId, SiteId

---

## Visual API Commands (by scenario)

**Item Validation:**
- TODO: VISUAL validate item via service (currently placeholder)
- Use GeneralQuery or item master lookup

**Availability Check:**
- TODO: Query on-hand quantities at From Location
- Use inventory availability APIs

**Transfer Posting:**
- TODO: VISUAL InventoryTransaction for transfer
- Post location-to-location transfer transaction

**Location Services:**
- LocationPickerDialog provides location selection UI
- Integration with warehouse location master data

---

## Business Rules and Exceptions

- Source and destination locations must be different
- Sufficient quantity must be available at From Location
- Part number must exist in item master
- Transfer must be within same warehouse/site
- All errors route through Exception Handling Form

---

## Workflows

1. **Part Entry:** Enter/scan part number → click Validate or magnify button for part lookup
2. **Location Selection:** Enter/scan locations OR use F2/F3 for location picker dialog
3. **Quantity Entry:** Specify transfer quantity
4. **Availability Check:** Verify sufficient inventory at From Location
5. **Transfer Posting:** Submit transfer to Visual system
6. **Reset:** Clear all fields for next transaction

---

## ViewModel/Command Conventions

**InventoryTransferViewModel Properties:**
- Input fields with ObservableProperty attributes
- ReadOnlyObservableCollection for DataGrid binding
- IPartDialogService for enhanced part selection (when injected)

**Commands:**
- `InventoryTransfer_Button_ValidateItemCommand` - validate part number
- `InventoryTransfer_Button_CheckAvailabilityFromCommand` - check inventory levels
- `InventoryTransfer_Button_ShowLocationModalCommand` - open location picker
- `InventoryTransfer_Button_PostTransferCommand` - submit transfer
- `InventoryTransfer_Button_ResetCommand` - clear all fields
- `InventoryTransfer_Button_ResolveIncompletePartIdCommand` - part lookup dialog

**Service Integration:**
- IExceptionHandler for error handling
- IPartDialogService for enhanced part selection (optional)
- Direct window creation for dialogs when service not available

---

## Integration & Storage

**Services Used:**
- IExceptionHandler for centralized error handling
- IPartDialogService for part selection (optional enhancement)
- Visual API services for item validation and transfer posting

**Data Storage:**
- Results displayed in DataGrid for session visibility
- No local persistence beyond current session
- Audit trail through Visual system transaction logs

---

## Keyboard/Scanner UX

**Keyboard Shortcuts:**
- Ctrl+F: Resolve incomplete part ID (opens part search dialog)
- F2: Pick From Location (opens location picker)
- F3: Pick To Location (opens location picker)

**Scanner Support:**
- Part number field accepts barcode input
- Location fields accept location barcode input
- Tab navigation between fields for efficient data entry

---

## UI Scaffold

**Views:**
- `Views/InventoryTransferView.axaml` - main transfer form
- `Views/Dialogs/LocationPickerDialog.axaml` - location selection
- `Views/Dialogs/IncompletePartDialog.axaml` - part search/selection

**ViewModels:**
- `ViewModels/InventoryTransferViewModel.cs` - transfer logic
- `ViewModels/Dialogs/LocationPickerDialogViewModel.cs` - location picker
- `ViewModels/Dialogs/IncompletePartDialogViewModel.cs` - part search

**Data Types:**
- `InventoryTransferResultRow` record for DataGrid display
- Service interfaces for Visual API integration

---

## Testing & Acceptance

- Form loads with proper layout and expandable panels
- Keyboard shortcuts trigger appropriate dialogs
- Part number validation connects to Visual item master
- Location picker provides valid warehouse locations
- Availability check shows accurate inventory levels
- Transfer posting creates proper Visual transactions
- Reset command clears all input fields
- All errors display through Exception Handling Form

---

## References

- ../../Views/InventoryTransferView.axaml (UI implementation)
- ../../ViewModels/InventoryTransferViewModel.cs (business logic)
- ../../References/Visual Highlighted Screenshots/InventoryTransferHighights.png
- ./ExceptionHandling.md (error handling flow)
- ./LocationPickerDialog.md (location selection)
- ./IncompletePartDialog.md (part search)

---

## Implementation Status

- **Current:** Basic UI and ViewModel structure implemented
- **TODO:** Visual API integration for validation and posting
- **Dialogs:** Location picker and part search dialogs functional
- **Navigation:** Integrated with MainView navigation system

---

## TODOs / Copilot Agent Notes

- [ ] Implement Visual API calls for item validation
- [ ] Implement inventory availability checking
- [ ] Implement transfer posting via InventoryTransaction
- [ ] Add proper data validation and business rule enforcement
- [ ] Consider adding transfer history/audit display
- [ ] Enhance barcode scanning integration