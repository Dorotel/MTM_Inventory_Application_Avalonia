# MTM Inventory Application - Copilot Coding Agent Instructions

## Repository Overview

This is an **Avalonia desktop application** built with **.NET 8** for managing inventory operations and work order transactions. The application integrates with **Infor VISUAL ERP system** and follows strict **MVVM architecture** with comprehensive documentation requirements.

**Key Features:** Login authentication, Inventory Transfer, Work Order Transactions, Exception Handling, Settings management, and app-owned database (MAMP).

**Repository Size:** ~50 C# files, 20+ planning documents, extensive reference materials
**Target Runtime:** .NET 8 desktop application using Avalonia 11
**External Dependencies:** Infor VISUAL API toolkit, SQL Server integration

## Build and Validation Commands

### Environment Setup

- **Required:**
  - .NET SDK 8.0+
  - Avalonia Templates (`dotnet new install Avalonia.Templates`)
  - (If needed) Node.js & npm for web builds
- Always run `dotnet restore` after cloning or pulling
- Check for and run provided PowerShell/Batch scripts for setup or dependency management

### Build Steps

1. `dotnet restore`
2. `dotnet build --configuration Release`
3. `dotnet run --project MTM_Inventory_Application_Avalonia.Desktop`

**Build Time:** ~20 seconds for clean build
**Expected Warnings:** 2 MVVM toolkit warnings about direct field access in ExceptionDialogViewModel.cs (non-blocking)

### Test Steps

- If present: `dotnet test` (always build before testing)
- Currently **no automated test projects** - validation relies on:
  1. Successful build compilation
  2. Manual application testing
  3. Documentation consistency checks

### Lint/Style

- Use `.editorconfig` for consistent formatting (UTF-8 encoding, CRLF line endings)
- For HTML/CSS, check for linters in scripts or npm configs

### Common Problems/Workarounds

- Restore before build
- For XAML errors: try `dotnet clean` then rebuild
- On Windows, unblock scripts if needed (file properties)

### CI/CD & Validation

- Standard flow: restore → build → test
- Review `.github/workflows/` for CI details
- Always pass local build/tests before PR

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
├── Views/                                    # Avalonia XAML views (*.axaml)
├── ViewModels/                              # MVVM view models (*.cs)
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

### Config/Validation

- `*.csproj`: Project/build config
- `.editorconfig`: Lint/style config (UTF-8 encoding, CRLF line endings)
- `.github/workflows/`: Actions YAMLs
- `fix-markdown-utf8.ps1/.bat`: Utility scripts for UTF-8 encoding fixes

### Checks and Validation

- Restore → build → test (if present) for every change
- Address linter/style violations before PR
- Review/comply with CI checks in GitHub Actions

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

## Special Copilot Commands

### "Update Readme Files"

- For each `.cs` or `.axaml` in `Views/` or `ViewModels/`:
    - Ensure a matching `.md` exists in `Copilot Files/`
    - If missing, generate using the **standard Copilot markdown documentation format** (see below)
    - Each `.md` must stay up to date with its corresponding source file, documenting all field names, commands, validation, and business rules
    - If a `.md` exists, verify its content aligns with the associated code

### "Update HTML Files"

When updating or generating a help HTML file in `Copilot Files/`, always follow these rules to ensure consistency, accuracy, and usability:

- **Content Sources:**  
    - Use data from the View (`.axaml`), its ViewModel (`.cs`), and the connected `.md` documentation file in `Copilot Files/`
    - Do not invent or hallucinate facts—only include information that is present or implied in these sources
    - Reference and embed any images/screenshots or key artifacts from `References` as appropriate

- **Structure and Layout:**  
    - Use a consistent structure for all help files:
        1. **Title/Header:** Feature or view name
        2. **Purpose/Overview:** Summary of what the feature/view does
        3. **How to Use:** Step-by-step user workflow instructions
        4. **Field and Control Descriptions:** List and describe all important fields, buttons, and controls (match UI and ViewModel)
        5. **Validation and Error Handling:** Required fields, validation rules, common error messages
        6. **Roles and Permissions:** Describe actions restricted by user role (if applicable)
        7. **Keyboard/Barcode Shortcuts:** List available shortcuts and scanner behaviors
        8. **Dialog/Popup Windows:** Explain dialogs and their usage
        9. **References and Screenshots:** Link/embed relevant screenshots/reference files from `References`
        10. **Troubleshooting/Common Issues:** List known issues/tips (from code or documentation only)
        11. **Navigation:**  
            - Include navigation aids (table of contents, links to other help files, "Back to Main Help")
    - Use tables, lists, and headings for readability

- **Style and Accessibility:**  
    - Use clear, user-focused language
    - Avoid jargon unless defined
    - Use semantic HTML (headings, lists, tables, links, strong/em) for accessibility
    - Ensure images/screenshots have meaningful alt text/captions
    - Apply consistent formatting across all pages

- **Data Accuracy and Maintenance:**  
    - Ensure all descriptions, workflows, and field names are derived from the actual code and `.md` documentation
    - Update the help file if any code, validation, or business rule changes
    - Do not copy outdated or deprecated information from other sources
    - If information is not found in code or documentation, omit it or clearly mark as "Not Documented"

- **Navigation and Integration:**  
    - Cross-link related help pages for easy navigation
    - Always provide an easy way back to the main help index/dashboard

- **Other Best Practices:**  
    - Include a "Last Updated" date and version/documentation status
    - Reference the origin of business rules or workflows (design doc, `.md` file)
    - Use examples/images only if sourced from repo assets
    - Proofread all help files for clarity, grammar, and spelling
    - Test navigation and links after each update

## Standard Copilot Markdown Documentation Format

Every `.md` file in `Copilot Files/` should strictly follow this structure:

````markdown
# {Feature or View Name}
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `{ViewFile.axaml}`
- **ViewModel:** `{ViewModelFile.cs}`
- **Primary Commands:** (list all)
- **Related Dialogs:** (if any)
- **Last Updated:** {YYYY-MM-DD}
- **Copilot Template Version:** 1.1

---

## Purpose

_Describe the functionality, scope, and intent of this View or feature._

---

## Key Visual References

- ![Screenshot1](References/relative/path/to/screenshot1.png)
- ...

---

## Global Rules

_List global technical, business, or architectural constraints._

---

## Scope

_What is covered by this View/feature? Workflows, boundaries, and limitations._

---

## Naming, Error Handling, and Coding Conventions

- File, method, variable naming rules
- Error handling: always use `IExceptionHandler`

---

## UX/Validation Rules

- Field definitions, validations, required/optional/read-only
- Description text mapping
- Exception handling surface conventions

---

## Visual API Command Patterns

- Backend/service patterns: authentication, validation, posting, etc

---

## Dialogs

- Outline all dialogs, triggers, and core logic/UI

---

## Business Rules

- Enumerate posting/validation rules and exception paths

---

## Workflow Summary

- Happy and alternate path steps

---

## ViewModel/Command Conventions

- List field and command names, role gating, etc

---

## Integration & Storage

- Service/API use, local audits/logging, DB refs

---

## Keyboard/Scanner UX

- Focus order, shortcuts, barcode workflow

---

## UI Scaffold

- Views, ViewModels, services, navigation patterns

---

## Testing & Acceptance

- Behaviors, dialog flows, error, and audit requirements

---

## References

- Screenshots, PDFs, code docs, related markdown files

---

## Implementation Status

- Current implementation state, what's wired/stubbed

---

## TODOs / Copilot Agent Notes

- [ ] Checklist for future enhancement/validation

---

_Copilot Note:  
When generating or updating this file:_
- _Always use the metadata header_
- _Reference related Views, ViewModels, commands, dialogs_
- _Highlight changes to requirements, validation, or commands since last update_
- _If missing for a View, generate from .axaml and .cs using this format_
````

### Agent Guidelines

**Trust These Instructions:** Only search repository if information here is incomplete or incorrect. This documentation is comprehensive and current.

**Minimal Changes:** Focus on surgical modifications. Existing placeholder implementations are intentional - only replace TODOs when explicitly requested.

**Error Handling Pattern:** ALL new methods must follow the established IExceptionHandler pattern - no exceptions.

**File Naming Convention:** `{Type}_{Parent}_{Name}` for files, `{Class}_{ControlType}_{Name}` for methods.