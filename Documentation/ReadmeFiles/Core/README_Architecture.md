# MTM Inventory Application - Architecture Guide
_System Architecture and Design Patterns_

---

**Metadata**  
- **Application Type:** Desktop Avalonia Application
- **Platform:** .NET 8
- **Architecture Pattern:** MVVM with Service Layer
- **External Integration:** Infor VISUAL ERP
- **Documentation Type:** Core
- **Last Updated:** 2024-12-22
- **Template Version:** 2.0

---

## Purpose

This document describes the system architecture, design patterns, and structural organization of the MTM Inventory Application. It serves as a reference for developers, architects, and technical stakeholders to understand how the application is designed and how its components interact.

---

## Key References

- [Project Overview](README_Project_Overview.md) - Main project summary and rules
- [Code Map](../Development/README_Code_Map.md) - Detailed file organization
- [Planned Implementations](../Development/README_Planned_Implementations.md) - Development roadmap
- [Component Documentation](../Components/) - Individual component specifications

---

## Global Rules and Constraints

### Visual License Lifecycle
- All VISUAL server operations MUST follow proper license lifecycle
- Acquire license per request, perform operation, explicitly close/release immediately
- Use short-lived, per-request scope only to prevent connection leaks

### Error Handling Requirements
- All UI-invoked methods must have error handling via centralized IExceptionHandler
- Route all exceptions through standardized error handling patterns
- Provide user-friendly error messages with proper categorization

### Service Abstraction
- ViewModels depend only on service interfaces, never concrete implementations
- All VISUAL API integration occurs through service layer
- Maintain clean separation between UI, business logic, and data access

---

## Scope and Boundaries

### Included in Architecture
- Desktop application shell and navigation
- MVVM presentation layer with data binding
- Service layer for business logic and external integration
- Centralized error handling and exception management
- App-owned database (MAMP) for settings and audit trails

### External Dependencies
- Infor VISUAL ERP system (via Vmfg*.dll assemblies)
- SQL Server for both app database and VISUAL integration
- Avalonia UI framework for cross-platform desktop support

### Architecture Boundaries
- No web components or browser dependencies
- No mobile or embedded device support
- Integration limited to VISUAL ERP system
- Windows desktop primary platform (cross-platform capable)

---

## Implementation Guidelines

### Project Structure
```
MTM_Inventory_Application_Avalonia/           # Main application project
├── Views/                                    # Avalonia XAML views
├── ViewModels/                              # MVVM presentation logic
├── Services/                                # Business logic and interfaces
├── Models/                                  # Data models and DTOs
├── Converters/                              # Value converters for binding
└── Assets/                                  # Application resources

MTM_Inventory_Application_Avalonia.Desktop/  # Desktop launcher project
└── Program.cs                               # Application entry point
```

### Naming Conventions
- **Files:** `{Type}_{Parent}_{Name}` pattern
- **Classes:** PascalCase with descriptive names
- **Interfaces:** `I{Name}Service` pattern for service contracts
- **ViewModels:** `{Feature}ViewModel` pattern
- **Views:** `{Feature}View` pattern

---

## Technical Specifications

### Architecture Pattern: MVVM with Service Layer

#### Presentation Layer (Views + ViewModels)
```
Views (XAML)
├── MainWindow.axaml              # Application shell
├── MainView.axaml                # Primary content host
├── InventoryTransferView.axaml   # Inventory transfer interface
├── WorkOrderTransactionView.axaml # Work order processing
├── SettingsView.axaml            # Application settings
└── Dialogs/
    ├── LoginView.axaml           # Authentication dialog
    ├── ExceptionDialogView.axaml # Error display
    └── IncompletePartDialog.axaml # Part selection
```

ViewModels implement:
- `ObservableObject` base class from CommunityToolkit.Mvvm
- `ObservableProperty` attributes for data-bound properties
- `RelayCommand` attributes for UI command bindings
- Dependency injection for service interfaces

#### Service Layer
```
Service Interfaces:
├── IExceptionHandler          # Centralized error handling
├── IAuthenticationService     # Login and session management
├── INavigationService         # View navigation and hosting
├── IPartDialogService         # Enhanced dialog workflows
├── IInventoryService          # Inventory operations (planned)
├── IShopFloorService          # Work order processing (planned)
└── ISettingsStore            # Application settings (planned)

Service Implementations:
├── Service_ExceptionHandler   # Error categorization and display
└── Services.cs               # Centralized service registration
```

#### Data Layer
```
Models and DTOs:
├── InventoryResultRow        # Inventory transfer data grid
├── WorkOrderResultRow        # Work order transaction data grid
└── (Additional models as needed)

External Integration:
├── VISUAL API (Vmfg*.dll)    # ERP system integration
└── App Database (MAMP)       # Local settings and audit trails
```

### Data Flow Architecture

1. **User Interaction → ViewModel Command**
   - User clicks button or enters data
   - XAML binding triggers RelayCommand method
   - ViewModel validates input and calls service

2. **ViewModel → Service Interface**
   - ViewModel depends only on interface contracts
   - Service implementation handles business logic
   - Error handling occurs at service boundary

3. **Service → External System (VISUAL)**
   - Service acquires VISUAL license per operation
   - Executes business operation via VISUAL API
   - Releases license immediately after completion

4. **Data Return → ViewModel → View**
   - Service returns data to ViewModel
   - ViewModel updates observable properties
   - XAML binding automatically updates UI

### Integration Points

#### VISUAL ERP Integration
- **Connection Management:** Short-lived, per-request license lifecycle
- **Authentication:** OpenLocal/OpenLocalSSO for production, Admin/Admin for development
- **Data Operations:** Inventory queries, work order transactions, part lookups
- **Error Handling:** VISUAL exceptions routed through IExceptionHandler

#### Application Database (MAMP)
- **Purpose:** Settings storage, audit trails, local configuration
- **Technology:** SQL Server with simple schema
- **Access Pattern:** Direct SQL queries for settings, audit logging for operations
- **Development:** Separate test database for development environment

---

## User Experience Guidelines

### Navigation Pattern
- **Single Window Application:** MainWindow hosts all content
- **Content Hosting:** MainView displays current feature via DataTemplates
- **Modal Dialogs:** Login overlay, exception handling, part selection dialogs
- **State Preservation:** Navigation maintains user context and form state

### Validation and Error Handling
- **Client-Side Validation:** Input validation at ViewModel level
- **Server-Side Validation:** VISUAL API validation with user-friendly error translation
- **Error Display:** Centralized ExceptionDialog with details expansion
- **Recovery Guidance:** Clear error messages with suggested resolution steps

### Accessibility and Usability
- **Keyboard Navigation:** Full keyboard support with logical tab order
- **Screen Reader Support:** Proper ARIA labels and semantic markup
- **High Contrast:** Support for system accessibility themes
- **Responsive Layout:** Adaptive layout for different screen sizes

---

## Business Rules

### Core Business Logic
- **Inventory Transfers:** Location-to-location movement with quantity validation
- **Work Order Transactions:** Material issue and receipt with job tracking
- **Part Resolution:** Incomplete part dialog for part selection workflows
- **Authentication:** Role-based access with VISUAL credentials

### Data Validation Rules
- **Required Fields:** Enforced at ViewModel level with visual indicators
- **Business Rules:** VISUAL system validation with user-friendly translation
- **Quantity Validation:** Positive numbers, available inventory checks
- **Location Validation:** Valid location codes from VISUAL system

### Audit and Compliance
- **Transaction Logging:** All operations logged to app database
- **Error Tracking:** Exception details captured for debugging and analysis
- **User Actions:** User context included in all audit trails
- **Data Integrity:** Transaction rollback on validation failures

---

## Workflow Documentation

### Application Startup Workflow
1. **Application Launch:** Desktop launcher starts Avalonia application
2. **Shell Initialization:** MainWindow created with MainView content
3. **Service Registration:** Dependency injection container configured
4. **Login Display:** Login overlay shown over MainView
5. **Authentication:** User credentials validated against VISUAL
6. **Main Interface:** Login overlay hidden, application features available

### Feature Navigation Workflow
1. **Feature Selection:** User clicks navigation button or menu
2. **View Creation:** Appropriate ViewModel created via DI container
3. **Service Injection:** Required services injected into ViewModel
4. **Content Display:** View bound to ViewModel via DataTemplate
5. **State Management:** Previous view state preserved for return navigation

### Error Handling Workflow
1. **Exception Occurrence:** Any error in UI-invoked methods
2. **Exception Capture:** Try/catch block captures exception
3. **Service Routing:** Exception passed to IExceptionHandler.Handle()
4. **Error Categorization:** Exception analyzed and categorized
5. **User Display:** ExceptionDialog shown with appropriate message and details
6. **Recovery Options:** User can retry, cancel, or get additional help

---

## API and Service Integration

### Service Interface Contracts

#### IExceptionHandler
```csharp
public interface IExceptionHandler
{
    void Handle(Exception exception, string context);
    Task HandleAsync(Exception exception, string context);
}
```

#### IAuthenticationService
```csharp
public interface IAuthenticationService
{
    Task<bool> AuthenticateAsync(string username, string password, string site);
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    string CurrentUser { get; }
}
```

#### INavigationService
```csharp
public interface INavigationService
{
    void NavigateTo<T>() where T : class;
    void NavigateBack();
    bool CanNavigateBack { get; }
}
```

### VISUAL API Integration Patterns
- **License Management:** Using statements for automatic disposal
- **Error Translation:** VISUAL exceptions mapped to user-friendly messages
- **Data Transformation:** VISUAL data structures converted to application DTOs
- **Performance Optimization:** Batch operations where possible, minimal round trips

---

## Development Notes

### Current Implementation Status
- ✅ **MVVM Infrastructure:** Complete with CommunityToolkit.Mvvm
- ✅ **Service Registration:** Centralized DI container setup
- ✅ **Exception Handling:** IExceptionHandler service implemented
- ✅ **Navigation:** Basic navigation service for view management
- 🔄 **VISUAL Integration:** Placeholder implementations with TODO markers
- 🔄 **Authentication:** Development bypass, production VISUAL auth planned
- 🔄 **Business Services:** Interface definitions with stubbed implementations

### Testing Approach
- **Unit Testing:** Service layer testing with mocked dependencies
- **Integration Testing:** VISUAL API integration testing in development environment
- **UI Testing:** Manual testing of MVVM binding and user workflows
- **Error Testing:** Exception handling validation across all error scenarios

### Known Technical Debt
- **Service Implementation:** Replace TODO placeholders with actual VISUAL API calls
- **Error Mapping:** Complete mapping of VISUAL exceptions to user messages
- **Performance:** Optimize VISUAL license usage and data retrieval patterns
- **Configuration:** Environment-specific configuration management

---

## Configuration and Setup

### Development Environment
- **.NET 8 SDK:** Required for compilation and debugging
- **Avalonia Templates:** For view generation and development tools
- **Visual Studio 2022:** Recommended IDE with Avalonia extensions
- **VISUAL Development License:** For integration testing (optional for UI development)

### Production Deployment
- **Desktop Installation:** Windows Installer package with dependencies
- **VISUAL Integration:** Production VISUAL system with appropriate licenses
- **Database Setup:** SQL Server with app database schema creation
- **User Permissions:** VISUAL user accounts with appropriate role assignments

### Configuration Management
- **Database.config:** VISUAL connection string configuration
- **Environment Variables:** Optional overrides for connection strings and settings
- **App Settings:** Local configuration stored in app database
- **License Management:** VISUAL license pool configuration

---

## References and Citations

### Architecture Documentation
- [MVVM Pattern Documentation](https://docs.microsoft.com/en-us/dotnet/architecture/modern-desktop-apps/mvvm)
- [Avalonia Framework Documentation](https://docs.avaloniaui.net/)
- [CommunityToolkit.Mvvm Documentation](https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

### Related Project Documentation
- [Code Map](../Development/README_Code_Map.md) - Complete file organization
- [Component Specifications](../Components/) - Individual feature documentation
- [Database Design](../Database/README_MAMP_Database.md) - App database schema

### External Integration
- VISUAL API Documentation (References/Visual DLL & Config Files/)
- VISUAL Database Schema (References/Visual CSV Database Dumps/)

---

## Implementation Checklist

### Architecture Compliance
- [ ] All ViewModels extend ObservableObject
- [ ] All service dependencies use interface injection
- [ ] All UI methods implement try/catch with IExceptionHandler
- [ ] All VISUAL operations follow license lifecycle pattern
- [ ] All models implement proper data binding patterns

### Quality Assurance
- [ ] Service interfaces defined for all external dependencies
- [ ] Error handling covers all exception scenarios
- [ ] Navigation preserves user context and state
- [ ] Data validation occurs at appropriate layers
- [ ] Performance optimization for VISUAL integration

### Documentation Maintenance
- [ ] Architecture changes reflected in this document
- [ ] Code Map updated with new components
- [ ] Component documentation created for new features
- [ ] Integration patterns documented for new services

---

## Maintenance and Updates

### Architecture Evolution
- **Service Expansion:** Add new service interfaces as features are implemented
- **Performance Optimization:** Monitor and improve VISUAL integration performance
- **Cross-Platform Support:** Evaluate and test on non-Windows platforms
- **Security Enhancement:** Implement additional security measures as needed

### Update Triggers
- **Major Feature Addition:** Review architecture impact and document changes
- **External System Changes:** Update integration patterns for VISUAL system changes
- **Performance Issues:** Analyze and document architectural improvements
- **Technology Updates:** Evaluate and plan for framework and dependency updates

### Version History
- **v2.0** (2024-12-22): Comprehensive architecture documentation with modernized structure
- **v1.0** (2024-12-19): Initial architecture documentation

---

_Documentation Standards:_
- _Follow repository .editorconfig for UTF-8 encoding_
- _Use ASCII punctuation only for platform compatibility_
- _Keep relative links for portability_
- _Update when architectural changes are made_