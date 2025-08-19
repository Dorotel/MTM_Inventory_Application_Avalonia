# MTM Inventory Application - Copilot Coding Agent Instructions

## Repository Overview

This is an **Avalonia desktop application** built with **.NET 8** for managing inventory operations and work order transactions. The application integrates with **Infor VISUAL ERP system** and follows strict **MVVM architecture** with comprehensive documentation requirements.

**Key Features:** Login authentication, Inventory Transfer, Work Order Transactions, Exception Handling, Settings management, and app-owned database (MAMP).

**Repository Size:** ~50 C# files, 20+ planning documents, extensive reference materials
**Target Runtime:** .NET 8 desktop application using Avalonia 11
**External Dependencies:** Infor VISUAL API toolkit, SQL Server integration

## Build and Validation Commands

### Prerequisites
- .NET 8 SDK installed
- All commands run from repository root

### Essential Build Commands
```bash
# Build solution (required for all changes)
dotnet build

# Run desktop application 
dotnet run --project MTM_Inventory_Application_Avalonia.Desktop

# Clean build (when facing issues)
dotnet clean && dotnet build
```

**Build Time:** ~20 seconds for clean build
**Expected Warnings:** 2 MVVM toolkit warnings about direct field access in ExceptionDialogViewModel.cs (non-blocking)

### No Test Projects
This repository currently has **no automated test projects**. Validation relies on:
1. Successful build compilation
2. Manual application testing
3. Documentation consistency checks

### Known Build Issues and Workarounds

**UTF-8 Encoding Issues:** If markdown files cause Jekyll build failures:
```bash
# Fix all markdown files to proper UTF-8 encoding
.\fix-markdown-utf8.ps1
```

**MVVM Warnings:** Use generated properties instead of direct field access:
- Use `ErrorType` property instead of `errorType` field in ViewModels

## Project Architecture and Layout

### Solution Structure
```
MTM_Inventory_Application_Avalonia/           # Main application project
├── Views/                                    # Avalonia XAML views
├── ViewModels/                              # MVVM view models
├── Services/                                # Business logic interfaces/implementations
├── Copilot Files/                           # Planning and documentation
│   ├── MVVMDefinitions/                     # Feature specifications
│   ├── CodeMap_CS_Files.md                  # MANDATORY: Track all .cs files
│   └── Planned_Implementations.md           # Feature roadmap
├── References/                              # VISUAL API documentation
└── Assets/                                  # Application resources

MTM_Inventory_Application_Avalonia.Desktop/  # Desktop launcher project
docs/                                        # HTML documentation site
```

### Critical Documentation Maintenance Rules

**MANDATORY:** When modifying ANY .cs file, you MUST update `Copilot Files/CodeMap_CS_Files.md` in the same changeset with:
- File path, types, responsibilities, key members, dependencies, notes

**UI Planning Files:** Located in `Copilot Files/MVVMDefinitions/` - follow strict format requirements documented in README.md

### Architecture Patterns

**Service Layer:** All business logic goes through interfaces:
- `IExceptionHandler` - Centralized error handling (MANDATORY for all methods)
- `IAuthenticationService` - Login/session management  
- `IPartDialogService` - Part selection dialogs
- `INavigationService` - View navigation

**MVVM Implementation:**
- Use `CommunityToolkit.Mvvm` for ObservableProperty and RelayCommand
- ViewModels must implement try/catch and route errors through IExceptionHandler
- Views mapped via DataTemplates in App.axaml

### Critical Integration Rules

**VISUAL License Lifecycle (MANDATORY):**
```csharp
// Any VISUAL server operation MUST follow this pattern:
// 1. Acquire license per request
// 2. Perform operation
// 3. Explicitly close/release license immediately (success OR failure)
// Use short-lived, per-request scope only
```

**Development vs Production:**
- Development: Admin/Admin login allowed for testing
- Production: VISUAL-backed authentication required
- Environment configured via Database.config and environment variables

### Configuration

**Database Configuration:** `MTM_Inventory_Application_Avalonia/References/Visual DLL & Config Files/Database.config`

**Environment Variables (optional overrides):**
- `INVENTORY__ENVIRONMENT` (Development|Production)
- `INVENTORY__CONNECTIONSTRINGS__APPDB`
- `INVENTORY__CONNECTIONSTRINGS__APPDBTEST`
- `INVENTORY__WAREHOUSEID`

**Connection Behavior:** 
- Uses `Dbms.OpenLocal` for connections
- Development: `mtm_visual_application_test`
- Production: `mtm_visual_application`

### GitHub Workflows

**Current CI:** Jekyll documentation site build only (`.github/workflows/jekyll-docker.yml`)
**No automated testing** or code validation pipelines currently configured

### Validation Steps for Changes

1. **Build Verification:** `dotnet build` must succeed
2. **Documentation Sync:** Update CodeMap_CS_Files.md for any .cs changes  
3. **Manual Testing:** Run application and test affected features
4. **UTF-8 Validation:** Run fix-markdown-utf8.ps1 if touching .md files
5. **Error Handling:** Verify all new methods use IExceptionHandler

### Common File Locations

**Main Entry Point:** `MTM_Inventory_Application_Avalonia.Desktop/Program.cs`
**App Configuration:** `MTM_Inventory_Application_Avalonia/App.axaml.cs`
**Service Registrations:** `MTM_Inventory_Application_Avalonia/Services/Services.cs`
**Global Error Handler:** `MTM_Inventory_Application_Avalonia/Services/Service_ExceptionHandler.cs`

### Key Dependencies

**NuGet Packages:**
- Avalonia 11.3.4 (UI framework)
- CommunityToolkit.Mvvm 8.4.0 (MVVM helpers)
- Projektanker.Icons.Avalonia.MaterialDesign 9.6.2 (Icons)

**External Integrations:**
- Infor VISUAL API (Vmfg*.dll assemblies) - placeholder implementations present
- SQL Server (for app-owned database and VISUAL connections)

### Troubleshooting

**Build Failures:**
1. Clean and rebuild: `dotnet clean && dotnet build`
2. Check for encoding issues in .md files
3. Verify all project references are correct

**Runtime Issues:**
1. Check Database.config file exists and is accessible
2. Verify environment configuration matches deployment target
3. Ensure VISUAL API DLLs are available for production integrations

**Documentation Issues:**
1. Run UTF-8 normalization script
2. Verify all relative links in markdown files
3. Check Jekyll site generation in docs/ folder

### Agent Guidelines

**Trust These Instructions:** Only search repository if information here is incomplete or incorrect. This documentation is comprehensive and current.

**Minimal Changes:** Focus on surgical modifications. Existing placeholder implementations are intentional - only replace TODOs when explicitly requested.

**Error Handling Pattern:** ALL new methods must follow the established IExceptionHandler pattern - no exceptions.

**File Naming Convention:** `{Type}_{Parent}_{Name}` for files, `{Class}_{ControlType}_{Name}` for methods.