# Services Architecture and Implementation
_Service Layer Design and Interface Contracts_

---

**Metadata**  
- **Architecture Layer:** Service Layer / Business Logic
- **Primary Pattern:** Interface Segregation and Dependency Injection
- **Integration Points:** Infor VISUAL ERP, App Database (MAMP)
- **Documentation Type:** Component
- **Last Updated:** 2024-12-22
- **Template Version:** 2.0

---

## Purpose

This document describes the service layer architecture, interface contracts, and implementation patterns for the MTM Inventory Application. The service layer provides a clean abstraction between the presentation layer (ViewModels) and external systems (VISUAL ERP, databases).

---

## Key References

- [Architecture Guide](../Core/README_Architecture.md) - Overall system design
- [Code Map](../Development/README_Code_Map.md) - File locations and responsibilities
- [Project Overview](../Core/README_Project_Overview.md) - Global rules and constraints
- [Coding Conventions](../../HTML/Technical/coding-conventions.html) - Development standards

---

## Global Rules and Constraints

### Visual License Lifecycle (Critical)
- All VISUAL operations MUST follow proper license lifecycle
- Acquire license per request, perform operation, explicitly close/release immediately
- Use short-lived, per-request scope only to prevent connection leaks and system instability

### Interface-First Design
- All services MUST be defined as interfaces first
- ViewModels depend only on service interfaces, never concrete implementations
- Service implementations handle all external system integration complexity

### Error Handling Requirements
- All service methods must implement comprehensive error handling
- VISUAL exceptions must be mapped to user-friendly application exceptions
- All exceptions must be categorized for proper user presentation

---

## Scope and Boundaries

### Service Layer Responsibilities
- Business logic execution and validation
- External system integration (VISUAL ERP)
- Data transformation between external and application models
- Error handling and exception translation
- Audit logging and transaction management

### What Services Do NOT Handle
- UI logic or presentation concerns (handled by ViewModels)
- Direct XAML binding or user interface updates
- User input validation (handled at ViewModel level)
- Navigation or dialog management (handled by specialized services)

---

## Service Architecture Overview

### Current Service Interfaces

#### Core Infrastructure Services
```csharp
// Exception handling and error management
public interface IExceptionHandler
{
    void Handle(Exception exception, string context);
    Task HandleAsync(Exception exception, string context);
    void HandleWithUserMessage(Exception exception, string userMessage, string context);
}

// Application navigation and view management  
public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateBack();
    bool CanNavigateBack { get; }
    TViewModel GetCurrentViewModel<TViewModel>() where TViewModel : class;
}

// Enhanced dialog workflows
public interface IPartDialogService  
{
    Task<string?> PickPartAsync(string seedValue);
    Task<LocationInfo?> PickLocationAsync(string currentLocation);
    Task<bool> ConfirmActionAsync(string message, string title);
}
```

#### Business Domain Services (Planned)
```csharp
// Authentication and session management
public interface IAuthenticationService
{
    Task<bool> AuthenticateAsync(string username, string password, string site);
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    string CurrentUser { get; }
    string CurrentSite { get; }
}

// Inventory operations and queries
public interface IInventoryService
{
    Task<InventoryInfo> GetInventoryAsync(string partId, string location);
    Task<bool> TransferAsync(string partId, string fromLocation, string toLocation, decimal quantity);
    Task<List<InventoryLocation>> GetAvailableLocationsAsync(string partId);
    Task<List<PartInfo>> SearchPartsAsync(string searchTerm);
}

// Work order processing and shop floor operations
public interface IShopFloorService
{
    Task<WorkOrderInfo> GetWorkOrderAsync(string workOrderId);
    Task<bool> IssueToWorkOrderAsync(string workOrderId, string partId, decimal quantity);
    Task<bool> ReceiveFromWorkOrderAsync(string workOrderId, string partId, decimal quantity);
    Task<List<WorkOrderOperation>> GetOperationsAsync(string workOrderId);
}

// Application settings and configuration
public interface ISettingsStore
{
    Task<T> GetSettingAsync<T>(string key, T defaultValue);
    Task SetSettingAsync<T>(string key, T value);
    Task<Dictionary<string, object>> GetAllSettingsAsync();
    Task SaveSettingsAsync();
}
```

---

## Implementation Guidelines

### Service Implementation Pattern

#### Standard Service Structure
```csharp
public class InventoryService : IInventoryService
{
    private readonly IVisualConnectionService visualConnection;
    private readonly IExceptionHandler exceptionHandler;
    private readonly ILogger<InventoryService> logger;

    public InventoryService(
        IVisualConnectionService visualConnection,
        IExceptionHandler exceptionHandler,
        ILogger<InventoryService> logger)
    {
        this.visualConnection = visualConnection;
        this.exceptionHandler = exceptionHandler;
        this.logger = logger;
    }

    public async Task<InventoryInfo> GetInventoryAsync(string partId, string location)
    {
        try
        {
            // CRITICAL: Acquire license per request (short-lived scope)
            using var connection = await visualConnection.AcquireLicenseAsync();
            
            // Perform VISUAL operation
            var visualData = await connection.QueryInventoryAsync(partId, location);
            
            // Transform to application model
            var result = MapToInventoryInfo(visualData);
            
            logger.LogInformation("Retrieved inventory for {PartId} at {Location}", partId, location);
            return result;
        }
        catch (VisualException vex)
        {
            // Map VISUAL exceptions to user-friendly messages
            var message = MapVisualException(vex);
            throw new InventoryException(message, vex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve inventory for {PartId} at {Location}", partId, location);
            throw;
        }
        // License automatically released by using statement
    }
}
```

### Error Handling and Exception Mapping

#### VISUAL Exception Translation
```csharp
private string MapVisualException(VisualException vex)
{
    return vex.ErrorCode switch
    {
        "INV001" => "Part not found in inventory system",
        "INV002" => "Insufficient inventory available", 
        "LOC001" => "Invalid location specified",
        "AUTH001" => "Not authorized for this inventory operation",
        _ => $"Inventory system error: {vex.Message}"
    };
}
```

#### Application Exception Types
```csharp
public class InventoryException : ApplicationException
{
    public string ErrorCode { get; }
    public InventoryException(string message, string errorCode = null) : base(message)
    {
        ErrorCode = errorCode ?? "INV_GENERAL";
    }
}

public class WorkOrderException : ApplicationException  
{
    public string WorkOrderId { get; }
    public WorkOrderException(string message, string workOrderId) : base(message)
    {
        WorkOrderId = workOrderId;
    }
}
```

---

## Service Registration and Dependency Injection

### Current Registration (Services.cs)
```csharp
public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core infrastructure services
        services.AddSingleton<IExceptionHandler, Service_ExceptionHandler>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddTransient<IPartDialogService, PartDialogService>();
        
        // Business domain services (planned implementations)
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IShopFloorService, ShopFloorService>();
        services.AddSingleton<ISettingsStore, SettingsStore>();
        
        // VISUAL integration services
        services.AddScoped<IVisualConnectionService, VisualConnectionService>();
        
        return services;
    }
}
```

### Service Lifetime Guidelines
- **Singleton:** Services with no state that can be shared across the application
- **Scoped:** Services that maintain state during a user session or operation
- **Transient:** Services that are created fresh for each use (dialogs, short operations)

---

## Integration Patterns

### VISUAL API Integration Pattern

#### License Management Service
```csharp
public interface IVisualConnectionService
{
    Task<IVisualConnection> AcquireLicenseAsync();
    Task<bool> TestConnectionAsync();
    bool IsConnected { get; }
}

public class VisualConnectionService : IVisualConnectionService
{
    public async Task<IVisualConnection> AcquireLicenseAsync()
    {
        // Acquire VISUAL license - MUST be short-lived
        var connection = new VisualConnection();
        await connection.OpenAsync();
        return connection; // Implements IDisposable for automatic cleanup
    }
}

// Usage pattern in services
public async Task<T> PerformVisualOperation<T>(Func<IVisualConnection, Task<T>> operation)
{
    using var connection = await visualConnectionService.AcquireLicenseAsync();
    return await operation(connection);
}
```

### Data Transformation Patterns

#### VISUAL to Application Model Mapping
```csharp
private InventoryInfo MapToInventoryInfo(VisualInventoryRecord visual)
{
    return new InventoryInfo
    {
        PartId = visual.PartNumber?.Trim(),
        Description = visual.Description?.Trim(),
        Location = visual.LocationCode?.Trim(),
        QuantityOnHand = visual.QtyOnHand ?? 0,
        QuantityAvailable = visual.QtyAvailable ?? 0,
        UnitOfMeasure = visual.UOM?.Trim(),
        LastUpdated = visual.LastUpdateDate ?? DateTime.Now
    };
}
```

---

## Service Implementation Status

### Implemented Services ✅
- **IExceptionHandler:** Complete with error categorization and user-friendly display
- **INavigationService:** Basic implementation for view management
- **IPartDialogService:** Stub implementation for part selection workflows

### Planned Services 🔄
- **IAuthenticationService:** Interface defined, implementation needed for VISUAL integration
- **IInventoryService:** Interface defined, requires VISUAL API integration
- **IShopFloorService:** Interface defined, requires work order processing logic
- **ISettingsStore:** Interface defined, requires app database implementation
- **IVisualConnectionService:** Critical for all VISUAL operations

### Service Dependencies
```
IInventoryService → IVisualConnectionService → VISUAL API
IShopFloorService → IVisualConnectionService → VISUAL API  
IAuthenticationService → IVisualConnectionService → VISUAL API
ISettingsStore → App Database (MAMP)
IExceptionHandler → ExceptionDialog (UI)
```

---

## Testing and Validation Patterns

### Service Testing Strategy

#### Unit Testing with Mocks
```csharp
[Test]
public async Task GetInventoryAsync_ValidPart_ReturnsInventoryInfo()
{
    // Arrange
    var mockVisualConnection = new Mock<IVisualConnectionService>();
    var mockConnection = new Mock<IVisualConnection>();
    
    mockVisualConnection.Setup(x => x.AcquireLicenseAsync())
                       .ReturnsAsync(mockConnection.Object);
    
    mockConnection.Setup(x => x.QueryInventoryAsync("PART001", "LOC001"))
                  .ReturnsAsync(new VisualInventoryRecord { /* test data */ });
    
    var service = new InventoryService(mockVisualConnection.Object, /* other deps */);
    
    // Act
    var result = await service.GetInventoryAsync("PART001", "LOC001");
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual("PART001", result.PartId);
    mockConnection.Verify(x => x.Dispose(), Times.Once); // Verify license released
}
```

#### Integration Testing
```csharp
[Test]
[Category("Integration")]
public async Task GetInventoryAsync_RealVisualConnection_ReturnsData()
{
    // Test against actual VISUAL system (development environment)
    var service = TestServiceProvider.GetService<IInventoryService>();
    
    var result = await service.GetInventoryAsync("TEST_PART", "MAIN");
    
    Assert.IsNotNull(result);
    // Additional assertions for real data
}
```

---

## Performance and Optimization

### VISUAL License Optimization
- **Connection Pooling:** Consider connection pooling for high-frequency operations
- **Batch Operations:** Group related operations to minimize license acquisitions
- **Caching Strategy:** Cache frequently accessed data with appropriate expiration
- **Async Patterns:** Use async/await throughout to prevent UI blocking

### Memory Management
- **Dispose Pattern:** All services implementing IDisposable properly
- **Weak References:** For event subscriptions to prevent memory leaks
- **Resource Cleanup:** Explicit cleanup in finally blocks or using statements

---

## Security Considerations

### Authentication and Authorization
- **Credential Management:** Secure storage and transmission of VISUAL credentials
- **Session Management:** Proper session timeout and cleanup
- **Role-Based Access:** Service-level enforcement of user permissions
- **Audit Logging:** Comprehensive logging of all service operations

### Data Protection
- **Sensitive Data:** Avoid logging sensitive information (passwords, personal data)
- **Encryption:** Encrypt sensitive configuration data
- **Secure Communication:** Ensure secure communication with VISUAL system

---

## Monitoring and Diagnostics

### Logging Strategy
```csharp
public async Task<InventoryInfo> GetInventoryAsync(string partId, string location)
{
    using var activity = Activity.StartActivity("GetInventory");
    activity?.SetTag("partId", partId);
    activity?.SetTag("location", location);
    
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        logger.LogInformation("Getting inventory for {PartId} at {Location}", partId, location);
        
        var result = await PerformInventoryQuery(partId, location);
        
        logger.LogInformation("Retrieved inventory for {PartId} in {ElapsedMs}ms", 
                            partId, stopwatch.ElapsedMilliseconds);
        
        return result;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to get inventory for {PartId} at {Location} after {ElapsedMs}ms",
                       partId, location, stopwatch.ElapsedMilliseconds);
        throw;
    }
}
```

### Performance Metrics
- **Response Times:** Track service operation performance
- **Error Rates:** Monitor exception frequencies and patterns
- **License Usage:** Track VISUAL license acquisition and release patterns
- **Resource Utilization:** Monitor memory and CPU usage

---

## Future Enhancements

### Planned Service Additions
- **ICacheService:** For performance optimization and offline scenarios
- **IAuditService:** For comprehensive operation tracking and compliance
- **INotificationService:** For user notifications and alerts
- **IReportingService:** For data export and reporting capabilities

### Integration Expansions
- **Other ERP Systems:** Abstract integration patterns for multiple systems
- **Cloud Services:** Integration with cloud-based inventory management
- **Mobile Support:** Service layer ready for mobile client development
- **Web API:** Expose services through web API for external integration

---

## Implementation Checklist

### Service Development
- [ ] Interface defined with clear contracts and documentation
- [ ] Implementation follows established patterns and error handling
- [ ] VISUAL license lifecycle properly managed (if applicable)
- [ ] Dependency injection registration added to Services.cs
- [ ] Unit tests created with appropriate mocking

### Integration Testing
- [ ] Service tested against actual VISUAL system (development)
- [ ] Error scenarios tested and handled appropriately
- [ ] Performance characteristics validated
- [ ] Memory usage and resource cleanup verified
- [ ] Logging and monitoring implemented

### Documentation Maintenance
- [ ] Service interface and responsibilities documented
- [ ] Code Map updated with new service files
- [ ] Architecture documentation updated for new patterns
- [ ] Usage examples provided for ViewModels

---

## Maintenance and Updates

### Regular Maintenance
- **Dependency Updates:** Keep service dependencies current and secure
- **Performance Review:** Regular analysis of service performance metrics
- **Error Pattern Analysis:** Review exception logs for recurring issues
- **Documentation Sync:** Keep service documentation current with implementation

### Update Triggers
- **VISUAL System Changes:** Update integration patterns for ERP system changes
- **Business Rule Changes:** Update service logic for new business requirements
- **Performance Issues:** Optimize service implementations for better performance
- **Security Updates:** Apply security patches and improve authentication

### Version History
- **v2.0** (2024-12-22): Comprehensive service architecture documentation
- **v1.0** (2024-12-19): Initial service interface definitions

---

_Documentation Standards:_
- _Follow repository .editorconfig for UTF-8 encoding_
- _Use ASCII punctuation only for platform compatibility_
- _Keep relative links for portability_
- _Update when service implementations change_