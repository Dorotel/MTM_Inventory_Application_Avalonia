# MAMP Database — Application-owned schema for non-Visual data

Purpose
- Define the app-owned database used to store data that cannot or should not be written to the VISUAL (MTMFG) database: exception logs, app settings, and optional local history to support UX.

Scope
- Used by: Exception Handling Form (error logs/approvals), Work Order Transaction (exception reports, local history), app configuration (WarehouseId, etc.).
- Not part of the VISUAL database. Does not modify or overlap MTMFG tables.

Database names
- Production: mtm_visual_application
- Development/Test: mtm_visual_application_test
- Selection: see README Configuration for connection strings and environment overrides.

No overlap with VISUAL tables
- Verified against CSV dumps: none of the app tables below exist in the MTMFG schema. All names are application-specific to avoid collisions.

Tables (dialect-agnostic DDL illustrated)

1) closed_work_orders
- Columns: id (PK, bigint auto), wo_no, item_no, attempted_qty (decimal 18,6), reason (default 'WO Closed'), user_id, occurred_utc (datetime), extra_json (JSON/null)
- Indexes: ix_cwo_wo (wo_no), ix_cwo_time (occurred_utc)

2) over_receipts
- Columns: id (PK, bigint auto), wo_no, item_no, requested_qty (decimal 18,6), remaining_qty (decimal 18,6), approved_by (nullable), user_id, occurred_utc, extra_json (JSON/null)
- Indexes: ix_or_wo (wo_no), ix_or_time (occurred_utc)

3) not_enough_location
- Columns: id (PK, bigint auto), from_location, item_no, requested_qty (decimal 18,6), onhand_qty (decimal 18,6), user_id, occurred_utc, extra_json (JSON/null)
- Indexes: ix_nel_loc (from_location), ix_nel_time (occurred_utc)

4) local_tx_history
- Columns: id (PK, bigint auto), tx_type (Issue|Receipt|Transfer|…), wo_no (nullable), item_no, qty (decimal 18,6), site_id, warehouse_id, from_location (nullable), to_location (nullable), user_id, tx_utc (datetime), source_ref (nullable), extra_json (JSON/null)
- Indexes: ix_lth_time (tx_utc), ix_lth_wo (wo_no)

5) app_settings
- Columns: setting_key (PK), setting_value, updated_utc (datetime)
- Seed example: WarehouseId ? '002'

Seed recommendation
- Upsert WarehouseId on first run or provisioning:
  INSERT INTO app_settings(setting_key, setting_value, updated_utc)
  VALUES ('WarehouseId','002', UTC_TIMESTAMP())
  ON DUPLICATE KEY UPDATE setting_value=VALUES(setting_value), updated_utc=VALUES(updated_utc);

MySQL 5.7.24 compatible DDL
- Notes:
  - Uses ENGINE=InnoDB, UTF8MB4 encoding. JSON type is available in MySQL 5.7+.
  - Statements are idempotent via IF NOT EXISTS.

```sql
-- app_settings
CREATE TABLE IF NOT EXISTS app_settings (
  setting_key VARCHAR(128) NOT NULL,
  setting_value VARCHAR(512) NOT NULL,
  updated_utc DATETIME NOT NULL,
  PRIMARY KEY (setting_key)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- closed_work_orders
CREATE TABLE IF NOT EXISTS closed_work_orders (
  id BIGINT(20) NOT NULL AUTO_INCREMENT,
  wo_no VARCHAR(50) NOT NULL,
  item_no VARCHAR(50) NOT NULL,
  attempted_qty DECIMAL(18,6) NOT NULL,
  reason VARCHAR(64) NOT NULL DEFAULT 'WO Closed',
  user_id VARCHAR(64) NOT NULL,
  occurred_utc DATETIME NOT NULL,
  extra_json JSON DEFAULT NULL,
  PRIMARY KEY (id),
  KEY ix_cwo_wo (wo_no),
  KEY ix_cwo_time (occurred_utc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- local_tx_history
CREATE TABLE IF NOT EXISTS local_tx_history (
  id BIGINT(20) NOT NULL AUTO_INCREMENT,
  tx_type VARCHAR(16) NOT NULL,
  wo_no VARCHAR(50) DEFAULT NULL,
  item_no VARCHAR(50) NOT NULL,
  qty DECIMAL(18,6) NOT NULL,
  site_id VARCHAR(16) NOT NULL,
  warehouse_id VARCHAR(16) NOT NULL,
  from_location VARCHAR(64) DEFAULT NULL,
  to_location VARCHAR(64) DEFAULT NULL,
  user_id VARCHAR(64) NOT NULL,
  tx_utc DATETIME NOT NULL,
  source_ref VARCHAR(64) DEFAULT NULL,
  extra_json JSON DEFAULT NULL,
  PRIMARY KEY (id),
  KEY ix_lth_time (tx_utc),
  KEY ix_lth_wo (wo_no)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- not_enough_location
CREATE TABLE IF NOT EXISTS not_enough_location (
  id BIGINT(20) NOT NULL AUTO_INCREMENT,
  from_location VARCHAR(64) NOT NULL,
  item_no VARCHAR(50) NOT NULL,
  requested_qty DECIMAL(18,6) NOT NULL,
  onhand_qty DECIMAL(18,6) NOT NULL,
  user_id VARCHAR(64) NOT NULL,
  occurred_utc DATETIME NOT NULL,
  extra_json JSON DEFAULT NULL,
  PRIMARY KEY (id),
  KEY ix_nel_loc (from_location),
  KEY ix_nel_time (occurred_utc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- over_receipts
CREATE TABLE IF NOT EXISTS over_receipts (
  id BIGINT(20) NOT NULL AUTO_INCREMENT,
  wo_no VARCHAR(50) NOT NULL,
  item_no VARCHAR(50) NOT NULL,
  requested_qty DECIMAL(18,6) NOT NULL,
  remaining_qty DECIMAL(18,6) NOT NULL,
  approved_by VARCHAR(64) DEFAULT NULL,
  user_id VARCHAR(64) NOT NULL,
  occurred_utc DATETIME NOT NULL,
  extra_json JSON DEFAULT NULL,
  PRIMARY KEY (id),
  KEY ix_or_wo (wo_no),
  KEY ix_or_time (occurred_utc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

Usage mapping
- Work Order Transaction
  - Closed WO attempts ? closed_work_orders (block post, log attempt).
  - Over Receipts ? over_receipts (always log; may require Lead approval).
  - Not Enough @ From Location ? not_enough_location (log and block).
  - Optional UI/search ? local_tx_history.
- Exception Handling Form
  - Centralizes writes to the above tables and presents user prompts; see ExceptionHandling.md.
- App configuration
  - app_settings stores minimal app-scoped values (e.g., WarehouseId); environment and connection strings remain in app config/Database.config.

Environment and connections
- See README Configuration (Database.config keys and environment overrides) for:
  - ConnectionStrings:AppDb (prod) = mtm_visual_application
  - ConnectionStrings:AppDbTest (dev) = mtm_visual_application_test
  - INVENTORY__ENVIRONMENT, INVENTORY__CONNECTIONSTRINGS__APPDB, INVENTORY__CONNECTIONSTRINGS__APPDBTEST, INVENTORY__WAREHOUSEID

References
- ../../README.md (Configuration section)
- ./ExceptionHandling.md (logging model)
- ./WorkOrderTransaction.md (Reporting Hooks)
- ../../References/Visual CSV Database Dumps/MTMFG Tables.csv
- ../../References/Visual CSV Database Dumps/MTMFG Relationships.csv
- ../../References/Visual CSV Database Dumps/MTMFG Procedure List.csv