# Work Order Transaction Form — Functional and Technical Specification [Ref: WorkOrderScreenshotHighlighted.png; Reference - Inventory - 259 Pages !.pdf]

Purpose: define UI, validations, workflow, role gating, integration, storage, and configuration to implement Work Order (WO) transactions for Infor Visual (VMFG) in this Avalonia/.NET 8 application. [Ref: Reference - Shop Floor - 194 Pages !.pdf]

Scope: supports Receipt by WO (completion to stock) and Issue to WO; detects Over Receipts; blocks Closed WOs while logging them; enforces availability checks; and writes non-Visual data to the application database. [Ref: Reference - Inventory - 259 Pages !.pdf]

Platform: Avalonia 11 MVVM on .NET 8 with desktop lifetime; MainWindow/MainView bind to a ViewModel orchestrating commands and services. [Ref: MTM_Inventory_Application_Avalonia.Desktop/MTM_Inventory_Application_Avalonia.Desktop.csproj]

Shell wiring: DataContext initialization occurs in App.OnFrameworkInitializationCompleted where MainWindow/MainView receive MainViewModel (extend to a WorkOrderTransactionViewModel). [Ref: MTM_Inventory_Application_Avalonia/App.axaml.cs]


## UI color legend (per WorkOrderScreenshotHighlighted.png)
- Yellow = user-entered fields
- Green = static fields (pre-populated to the values shown in the screenshot; read-only)
- Orange/Red = fields that auto-populate after a valid Work Order is entered; read-only
- Blue = current date (auto-set to today; read-only)


## User Roles and Permissions (effective on this form) [Ref: Reference - Core - 48 Pages !.pdf]
- Material Handler: can validate WOs, check availability, and post Issue/Receipt transactions; can add entries to exception reports (Closed WO, Over Receipt, Not Enough @ Location). [Ref: Reference - Inventory - 259 Pages !.pdf]
- Inventory Specialist: can perform corrective transactions, adjust reasons, and resolve discrepancies; can add exception entries. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Material Handler Lead (Supervisor/Approver): all above plus approval authority for policy-gated overrides (e.g., over-receipt approval if allowed). [Ref: Reference - Core - 48 Pages !.pdf]
- Read-Only: can search and view WO status, item, locations, and history; cannot post or create/modify exception entries. [Ref: Reference - VMFG Shared Library - 877 Pages.pdf]

Role-to-command mapping: ValidateWorkOrder/CheckAvailability for all roles; PostTransaction for Material Handler/Inventory Specialist/Lead; ApproveOverReceipt only for Lead; all write operations denied for Read-Only (client and server enforced). [Ref: Reference - Core - 48 Pages !.pdf]


## Color-Coded Field Behavior [Ref: WorkOrderScreenshotHighlighted.png; Reference - Inventory - 259 Pages !.pdf]
- Yellow (user-entered): Work Order ID; Quantity; Location (From for Issue; To for Receipt).
- Green (static/read-only, set to what is in the screenshot): Site ID; Base/Split/Sub IDs; Warehouse ID (e.g., "002"); any other fixed context shown in the image.
- Orange/Red (auto after WO input, read-only): Item/Part; Description; UOM; WO Quantities (Desired/Received/Due); On Hand/Available; Unit Costs; User ID; other calculated/informational values in the image.
- Blue (auto current date, read-only): Transaction Date is set to today.

Warehouse ID policy: green and read-only; initialized to the value shown in the screenshot (e.g., "002"); editable later via configuration without code changes. [Ref: Database.config]


## Fields and Validation Rules [Ref: Reference - Inventory - 259 Pages !.pdf]
- Work Order ID (yellow, required): must exist and be open/eligible; if Closed/Canceled, posting is blocked and an entry is added to the Closed Work Orders report. [Ref: Reference - Shop Floor - 194 Pages !.pdf]
- Transaction Type (required): Issue to WO or Receipt by WO (completion to stock); first release supports these two types. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Quantity (yellow, required): > 0; for Receipt compare to Remaining (over-receipt detection); for Issue compare to Available at From Location (insufficient detection). [Ref: Reference - Inventory - 259 Pages !.pdf]
- UOM (orange/red, derived/validated): must be valid for Item; convert to base UOM for checks and posting. [Ref: Reference - Inventory - 259 Pages !.pdf]
- From Location (yellow for Issue): must exist and be active; show On-hand/Allocated/Available. [Ref: Reference - Inventory - 259 Pages !.pdf]
- To Location (yellow for Receipt): must exist and be active; bin-type compatibility as applicable. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Warehouse ID (green): fixed from settings/screenshot (e.g., "002"), not editable; included in all posts. [Ref: Database.config]
- Site ID (green): derived from environment/site configuration; not editable. [Ref: Reference - VMFG Shared Library - 877 Pages.pdf]
- Transaction Date (blue): auto-set to current date; read-only. [Ref: Reference - Core - 48 Pages !.pdf]
- Operation/Sequence (optional): for routing context if required by policy. [Ref: Reference - Shop Floor - 194 Pages !.pdf]
- Lot/Serial (conditional): required for lot/serial-controlled items; serial count must match quantity for serial-controlled. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Reason Code (conditional): required for over-receipt or variance if configured. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Comments (optional): allow audit context up to policy-defined length. [Ref: Reference - Core - 48 Pages !.pdf]


## Business Rules and Exceptions [Ref: Reference - Inventory - 259 Pages !.pdf]
- Closed Work Orders: block posting and create/append entry in Closed Work Orders report with WO, Item, Attempted Qty, User, Timestamp, Reason "WO Closed". [Ref: Reference - Shop Floor - 194 Pages !.pdf]
- Over Receipts (Receipt by WO): if Quantity > Remaining, prompt user; always log to Over Receipts report; allow override only if policy allows and Lead approves. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Not Enough at From Location (Issue): if On-hand < Quantity, prompt to verify or log "Not enough @ {From Location}"; posting is blocked. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Lot/Serial compliance: enforce capture before posting per item control flags. [Ref: Reference - Inventory - 259 Pages !.pdf]
- UOM conversions: convert input to base UOM; apply precision rules; validate against thresholds. [Ref: Reference - Inventory - 259 Pages !.pdf]


## Workflows [Ref: Reference - Shop Floor - 194 Pages !.pdf]
- Receipt by WO (happy path): enter WO (yellow) → load header/status/remaining → enter Quantity and To Location (yellow) → Warehouse = green (screenshot value), Date = today (blue) → validate (closed/over-receipt/lot-serial) → post → write Visual transaction + local audit → clear. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Issue to WO (happy path): enter WO (yellow) → enter Quantity and From Location (yellow) → Warehouse = green (screenshot value) → validate availability → post → write Visual transaction + local audit → clear. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Exception paths: Closed WO (block + report), Over Receipt (prompt + report + optional approval), Not Enough (prompt + report + block). [Ref: Reference - Inventory - 259 Pages !.pdf]


## Reporting Hooks (exception logs) [Ref: Reference - Core - 48 Pages !.pdf]
- Closed Work Orders: WO, Item, Attempted Qty, User, Timestamp, "WO Closed" to app DB. [Ref: Database.config]
- Over Receipts: WO, Item, Requested Qty, Remaining Qty, User, Timestamp, approval info to app DB. [Ref: Database.config]
- Not Enough at Location: From Location, Item, Requested Qty, On-hand, User, Timestamp to app DB. [Ref: Database.config]


## Data Storage Policy and Environment Configuration [Ref: Database.config]
- Any data that cannot be stored on the Visual server is written to mtm_visual_application (production) or mtm_visual_application_test (development). [Ref: Database.config]
- Selection logic: Release/Production → mtm_visual_application; Debug/Development → mtm_visual_application_test; supports environment variable override. [Ref: Intro - Development Guide - 32 Pages !.pdf]
- Resolution order: environment variable → Database.config → hard default (dev → test DB; prod → prod DB). [Ref: Database.config]
- Typical non-Visual data: exception reports, application settings (e.g., WarehouseId), optional local transaction history for UI/search/export. [Ref: Reference - Core - 48 Pages !.pdf]

Suggested schema (illustrative; MySQL shown) used by this app when Visual cannot store the data: [Ref: Database.config]

```
CREATE TABLE IF NOT EXISTS closed_work_orders (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  wo_no VARCHAR(50) NOT NULL,
  item_no VARCHAR(50) NOT NULL,
  attempted_qty DECIMAL(18,6) NOT NULL,
  reason VARCHAR(64) NOT NULL DEFAULT 'WO Closed',
  user_id VARCHAR(64) NOT NULL,
  occurred_utc DATETIME NOT NULL,
  extra_json JSON NULL,
  INDEX ix_cwo_wo (wo_no),
  INDEX ix_cwo_time (occurred_utc)
);

CREATE TABLE IF NOT EXISTS over_receipts (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  wo_no VARCHAR(50) NOT NULL,
  item_no VARCHAR(50) NOT NULL,
  requested_qty DECIMAL(18,6) NOT NULL,
  remaining_qty DECIMAL(18,6) NOT NULL,
  approved_by VARCHAR(64) NULL,
  user_id VARCHAR(64) NOT NULL,
  occurred_utc DATETIME NOT NULL,
  extra_json JSON NULL,
  INDEX ix_or_wo (wo_no),
  INDEX ix_or_time (occurred_utc)
);

CREATE TABLE IF NOT EXISTS not_enough_location (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  from_location VARCHAR(64) NOT NULL,
  item_no VARCHAR(50) NOT NULL,
  requested_qty DECIMAL(18,6) NOT NULL,
  onhand_qty DECIMAL(18,6) NOT NULL,
  user_id VARCHAR(64) NOT NULL,
  occurred_utc DATETIME NOT NULL,
  extra_json JSON NULL,
  INDEX ix_nel_loc (from_location),
  INDEX ix_nel_time (occurred_utc)
);

CREATE TABLE IF NOT EXISTS local_tx_history (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  tx_type VARCHAR(16) NOT NULL,
  wo_no VARCHAR(50) NULL,
  item_no VARCHAR(50) NOT NULL,
  qty DECIMAL(18,6) NOT NULL,
  uom VARCHAR(16) NOT NULL,
  site_id VARCHAR(16) NOT NULL,
  warehouse_id VARCHAR(16) NOT NULL,
  from_location VARCHAR(64) NULL,
  to_location VARCHAR(64) NULL,
  user_id VARCHAR(64) NOT NULL,
  tx_utc DATETIME NOT NULL,
  source_ref VARCHAR(64) NULL,
  extra_json JSON NULL,
  INDEX ix_lth_time (tx_utc),
  INDEX ix_lth_wo (wo_no)
);

CREATE TABLE IF NOT EXISTS app_settings (
  setting_key VARCHAR(128) PRIMARY KEY,
  setting_value VARCHAR(512) NOT NULL,
  updated_utc DATETIME NOT NULL
);
```

Seed WarehouseId default: [Ref: Database.config]

```
INSERT INTO app_settings(setting_key, setting_value, updated_utc)
VALUES ('WarehouseId','002', UTC_TIMESTAMP())
ON DUPLICATE KEY UPDATE setting_value=VALUES(setting_value), updated_utc=VALUES(updated_utc);
```


## Configuration Keys (minimum) [Ref: Database.config]
- Environment = Development | Production (determines DB selection and logging defaults). [Ref: Database.config]
- ConnectionStrings:AppDb = server=…;database=mtm_visual_application;… (production). [Ref: Database.config]
- ConnectionStrings:AppDbTest = server=…;database=mtm_visual_application_test;… (development). [Ref: Database.config]
- WarehouseId = 002 (read-only in UI; may be changed in settings later without code changes). [Ref: Database.config]
- Optional env overrides: INVENTORY__ENVIRONMENT, INVENTORY__WAREHOUSEID, INVENTORY__CONNECTIONSTRINGS__APPDB, INVENTORY__CONNECTIONSTRINGS__APPDBTEST. [Ref: Intro - Development Guide - 32 Pages !.pdf]


## ViewModel, Commands, and Role Gating (Avalonia MVVM) [Ref: MTM_Inventory_Application_Avalonia/App.axaml.cs]
- ViewModel: WorkOrderTransactionViewModel exposes properties for fields above with INotifyDataErrorInfo validation and computed availability. [Ref: Reference - Core - 48 Pages !.pdf]
- Commands: ValidateWorkOrder, CheckAvailability, PostTransaction, Reset, OpenHistory, OpenReports, ApproveOverReceipt (Lead only). [Ref: Reference - Core - 48 Pages !.pdf]
- Role gating: enable/disable inputs and commands per role; always enforce server-side authorization to prevent elevation. [Ref: Reference - VMFG Shared Library - 877 Pages.pdf]


## Integration Approach [Ref: MTMFG Procedure List.csv]
- Prefer official stored procedures for posting/validation where documented in the procedure list; avoid direct writes to Visual transactional tables. [Ref: MTMFG Procedure List.csv]
- If assemblies are licensed/available, encapsulate calls to VmfgInventory/VmfgShopFloor/VmfgShared in an application service adapter; never bind UI directly to assemblies. [Ref: VmfgInventory.dll; VmfgShopFloor.dll; VmfgShared.dll; VmfgTrace.dll; LsaCore.dll; LsaShared.dll]
- Use MTMFG Tables/Relationships CSVs to confirm entity names and joins for read models and availability lookups. [Ref: MTMFG Tables.csv; MTMFG Relationships.csv]


## Error Handling and UX [Ref: Reference - Core - 48 Pages !.pdf]
- Show actionable messages with WO/Item/Qty/Location context and procedure error text when available. [Ref: MTMFG Procedure List.csv]
- Debounce lookups and cache last-used locations/transaction type per user; optimize for barcode scanning. [Ref: Reference - Shared Library - 24 Pages !.pdf]
- Maintain keyboard-first flow; set sensible default focus (WO → Qty → Location). [Ref: Reference - Inventory - 259 Pages !.pdf]


## Testing and Acceptance Criteria [Ref: Reference - Inventory - 259 Pages !.pdf]
- Yellow fields editable per role; green fields read-only and pre-filled to the screenshot values; orange/red fields auto-populate after WO; blue date auto = current date.
- Transaction Date auto = current date; cannot be changed by user (blue). [Ref: Reference - Core - 48 Pages !.pdf]
- Warehouse ID read-only and equals settings/screenshot value (e.g., "002"); changing settings updates value on next start. [Ref: Database.config]
- Closed WO attempts block posting and append to Closed Work Orders report (app DB). [Ref: Reference - Shop Floor - 194 Pages !.pdf]
- Over Receipt detection prompts, logs, and requires Lead approval if policy allows override. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Not Enough @ From Location prompts, logs, and blocks posting. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Non-Visual data writes to mtm_visual_application (prod) or mtm_visual_application_test (dev) per environment. [Ref: Database.config]


## Citation Map — insert exact page and section numbers
- Closed Work Orders rule [Reference - Shop Floor - 194 Pages !.pdf, p. TBD, sec. TBD]
- Over Receipt detection and handling [Reference - Inventory - 259 Pages !.pdf, p. TBD, sec. TBD]
- Not Enough @ From Location handling [Reference - Inventory - 259 Pages !.pdf, p. TBD, sec. TBD]
- Lot/Serial enforcement rules [Reference - Inventory - 259 Pages !.pdf, p. TBD, sec. TBD]
- UOM conversion and precision [Reference - Inventory - 259 Pages !.pdf, p. TBD, sec. TBD]
- Transaction Date set to current date (read-only)	 [Reference - Core - 48 Pages !.pdf, p. TBD, sec. TBD
- Role-to-command/permissions model [Reference - Core - 48 Pages !.pdf, p. TBD, sec. TBD]
- Read-Only role restrictions [Reference - VMFG Shared Library - 877 Pages.pdf, p. TBD, sec. TBD]
- Workflows (Receipt/Issue happy paths) [Reference - Shop Floor - 194 Pages !.pdf, p. TBD, sec. TBD; Reference - Inventory - 259 Pages !.pdf, p. TBD, sec. TBD]
- Exception reports data points (Closed WO, Over Receipts, Not Enough) [Reference - Inventory - 259 Pages !.pdf, p. TBD, sec. TBD]
- Environment/config selection guidance [Intro - Development Guide - 32 Pages !.pdf, p. TBD, sec. TBD]


## Open Questions for Confirmation [Ref: Reference - Inventory - 259 Pages !.pdf]
- Final list of reason codes and which transactions require them. [Ref: Reference - Inventory - 259 Pages !.pdf]
- Whether operation/sequence is mandatory in any receipt scenarios. [Ref: Reference - Shop Floor - 194 Pages !.pdf]
- Exact stored procedure names/parameters for Issue/Receipt posting and history retrieval. [Ref: MTMFG Procedure List.csv]


## References (artifacts used by this document) [Ref: Reference - Core - 48 Pages !.pdf]
- WorkOrderScreenshotHighlighted.png (UI color legend and field groupings). [Ref: WorkOrderScreenshotHighlighted.png]
- Intro - Development Guide - 32 Pages !.pdf (environment/config guidance). [Ref: Intro - Development Guide - 32 Pages !.pdf]
- Reference - Core - 48 Pages !.pdf (roles, audit, validation patterns). [Ref: Reference - Core - 48 Pages !.pdf]
- Reference - Inventory - 259 Pages !.pdf (transactions, UOM, lot/serial, rules). [Ref: Reference - Inventory - 259 Pages !.pdf]
- Reference - Shop Floor - 194 Pages !.pdf (WO status, completion). [Ref: Reference - Shop Floor - 194 Pages !.pdf]
- Reference - VMFG Shared Library - 877 Pages.pdf (shared types/permissions). [Ref: Reference - VMFG Shared Library - 877 Pages.pdf]
- MTMFG Tables.csv; MTMFG Relationships.csv; MTMFG Procedure List.csv (entities/joins/procedures). [Ref: MTMFG Tables.csv]
- VmfgInventory.dll; VmfgShopFloor.dll; VmfgShared.dll; VmfgTrace.dll; LsaCore.dll; LsaShared.dll (integration assemblies). [Ref: VmfgInventory.dll]
- MTM_Inventory_Application_Avalonia.Desktop/MTM_Inventory_Application_Avalonia.Desktop.csproj; MTM_Inventory_Application_Avalonia/App.axaml.cs (application wiring). [Ref: MTM_Inventory_Application_Avalonia/App.axaml.cs]
