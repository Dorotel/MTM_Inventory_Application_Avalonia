# MTM Inventory Application - Getting Started Guide
_Setup, Build, and Development Environment Configuration_

---

**Metadata**  
- **Target Audience:** Developers and Technical Team Members
- **Prerequisites:** .NET 8 SDK, Development Environment
- **Estimated Setup Time:** 30-45 minutes
- **Documentation Type:** Core
- **Last Updated:** 2024-12-22
- **Template Version:** 2.0

---

## Purpose

This guide provides step-by-step instructions for setting up the development environment, building the MTM Inventory Application, and beginning development work. It covers everything from initial repository setup to running your first successful build.

---

## Key References

- [Project Overview](README_Project_Overview.md) - Main project summary and rules
- [Architecture Guide](README_Architecture.md) - System design and patterns
- [Coding Conventions](../../HTML/Technical/coding-conventions.html) - Development standards
- [Code Map](../Development/README_Code_Map.md) - File organization reference

---

## Prerequisites and System Requirements

### Required Software
- **.NET 8 SDK** (version 8.0 or later)
- **Git** (for repository access and version control)
- **Visual Studio 2022** (recommended) or **VS Code** with C# extension
- **Windows 10/11** (primary development platform)

### Optional but Recommended
- **SQL Server Developer Edition** (for database integration testing)
- **GitHub Desktop** (for easier Git workflow)
- **Avalonia for Visual Studio Extension** (enhanced XAML editing)

### Hardware Requirements
- **Memory:** 8GB RAM minimum, 16GB recommended
- **Storage:** 5GB free space for SDK, tools, and source code
- **Display:** 1920x1080 resolution recommended for development

---

## Environment Setup

### Step 1: Install .NET 8 SDK

1. **Download .NET 8 SDK**
   - Visit [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
   - Download .NET 8 SDK for your operating system
   - Choose the SDK (not just the runtime)

2. **Verify Installation**
   ```bash
   dotnet --version
   # Should show version 8.0.x or later
   ```

3. **Install Avalonia Templates**
   ```bash
   dotnet new install Avalonia.Templates
   # Installs Avalonia project templates for development
   ```

### Step 2: Development Environment Setup

#### Visual Studio 2022 (Recommended)
1. **Install Visual Studio 2022**
   - Download Community, Professional, or Enterprise edition
   - Include ".NET desktop development" workload
   - Include "ASP.NET and web development" workload (for future web components)

2. **Install Extensions**
   - **Avalonia for Visual Studio** - Enhanced XAML support
   - **GitHub Extension for Visual Studio** - Git integration
   - **EditorConfig Language Service** - Code formatting support

#### VS Code Alternative
1. **Install VS Code**
   - Download from [code.visualstudio.com](https://code.visualstudio.com/)

2. **Install Extensions**
   - **C# Dev Kit** - C# language support
   - **Avalonia for VSCode** - XAML and Avalonia support
   - **GitLens** - Enhanced Git capabilities
   - **EditorConfig for VS Code** - Formatting support

### Step 3: Repository Access

1. **Clone Repository**
   ```bash
   git clone https://github.com/Dorotel/MTM_Inventory_Application_Avalonia.git
   cd MTM_Inventory_Application_Avalonia
   ```

2. **Verify Repository Structure**
   ```
   MTM_Inventory_Application_Avalonia/
   ├── MTM_Inventory_Application_Avalonia/     # Main application
   ├── MTM_Inventory_Application_Avalonia.Desktop/  # Desktop launcher
   ├── Documentation/                          # Modern documentation
   ├── docs/                                  # Legacy documentation
   ├── README.md                              # Repository overview
   └── MTM_Inventory_Application_Avalonia.sln # Solution file
   ```

---

## Build and Run Instructions

### Initial Build Process

1. **Restore Dependencies**
   ```bash
   dotnet restore
   # Downloads and installs all NuGet packages
   ```

2. **Build Solution**
   ```bash
   dotnet build --configuration Release
   # Compiles the entire solution
   ```

3. **Expected Build Output**
   - Build should complete successfully
   - You may see StyleCop warnings (these are non-blocking)
   - Look for "Build succeeded" message

### Running the Application

1. **Run from Command Line**
   ```bash
   dotnet run --project MTM_Inventory_Application_Avalonia.Desktop
   ```

2. **Run from Visual Studio**
   - Set `MTM_Inventory_Application_Avalonia.Desktop` as startup project
   - Press F5 or click "Start Debugging"

3. **Expected Behavior**
   - Application window opens
   - Login dialog appears (use Admin/Admin for development)
   - Main application interface becomes available after login

### Development vs Production Mode

#### Development Mode (Default)
- **Login:** Admin/Admin credentials accepted
- **Database:** Uses test database (`mtm_visual_application_test`)
- **VISUAL Integration:** Stubbed/placeholder implementations
- **Error Handling:** Detailed error information shown

#### Production Mode
- **Environment Variable:** Set `INVENTORY__ENVIRONMENT=Production`
- **Login:** Requires actual VISUAL credentials
- **Database:** Uses production database (`mtm_visual_application`)
- **VISUAL Integration:** Full API integration (requires VISUAL DLLs)

---

## Development Workflow

### Daily Development Process

1. **Start Development Session**
   ```bash
   git pull origin main                    # Get latest changes
   dotnet restore                          # Update dependencies
   dotnet build                           # Verify build works
   ```

2. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make Changes**
   - Follow [Coding Conventions](../../HTML/Technical/coding-conventions.html)
   - Update [Code Map](../Development/README_Code_Map.md) for .cs file changes
   - Write tests for new functionality

4. **Build and Test**
   ```bash
   dotnet build                           # Verify compilation
   dotnet run --project MTM_Inventory_Application_Avalonia.Desktop  # Test functionality
   ```

5. **Commit Changes**
   ```bash
   git add .
   git commit -m "Brief description of changes"
   git push origin feature/your-feature-name
   ```

### Code Quality Checklist

Before committing any code, verify:
- [ ] Code builds without errors
- [ ] All methods have proper error handling with IExceptionHandler
- [ ] File naming follows established conventions
- [ ] CodeMap_CS_Files.md updated for any .cs file changes
- [ ] VISUAL license lifecycle properly implemented (if applicable)
- [ ] StyleCop warnings addressed or documented as acceptable

---

## Common Development Tasks

### Adding a New View and ViewModel

1. **Create ViewModel**
   ```csharp
   // MTM_Inventory_Application_Avalonia/ViewModels/YourFeatureViewModel.cs
   using CommunityToolkit.Mvvm.ComponentModel;
   using CommunityToolkit.Mvvm.Input;

   namespace MTM_Inventory_Application_Avalonia.ViewModels;

   public partial class YourFeatureViewModel : ObservableObject
   {
       private readonly IExceptionHandler exceptionHandler;

       public YourFeatureViewModel(IExceptionHandler exceptionHandler)
       {
           this.exceptionHandler = exceptionHandler;
       }

       [RelayCommand]
       private async Task YourFeature_Button_Action()
       {
           try
           {
               // Implementation here
           }
           catch (Exception ex)
           {
               exceptionHandler.Handle(ex, "Your Feature Action");
           }
       }
   }
   ```

2. **Create View**
   ```xml
   <!-- MTM_Inventory_Application_Avalonia/Views/YourFeatureView.axaml -->
   <UserControl xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                x:Class="MTM_Inventory_Application_Avalonia.Views.YourFeatureView">
     <Grid>
       <Button Content="Action" Command="{Binding YourFeature_Button_ActionCommand}" />
     </Grid>
   </UserControl>
   ```

3. **Register DataTemplate**
   ```xml
   <!-- In App.axaml -->
   <DataTemplate DataType="{x:Type vm:YourFeatureViewModel}">
     <views:YourFeatureView />
   </DataTemplate>
   ```

4. **Update Code Map**
   - Add entries for both ViewModel and View files in `CodeMap_CS_Files.md`

### Adding a New Service

1. **Define Interface**
   ```csharp
   public interface IYourService
   {
       Task<ResultType> PerformOperationAsync(string parameter);
   }
   ```

2. **Implement Service**
   ```csharp
   public class YourService : IYourService
   {
       public async Task<ResultType> PerformOperationAsync(string parameter)
       {
           // Implementation with proper VISUAL license lifecycle
           // if interacting with VISUAL API
       }
   }
   ```

3. **Register Service**
   ```csharp
   // In Services.cs
   services.AddSingleton<IYourService, YourService>();
   ```

### Working with VISUAL API Integration

1. **Service Pattern for VISUAL Operations**
   ```csharp
   public async Task<InventoryData> GetInventoryAsync(string partId)
   {
       // Acquire license per request (short-lived scope)
       using var connection = await visualConnectionService.AcquireLicenseAsync();
       
       try
       {
           // Perform VISUAL operation
           var result = await connection.QueryInventoryAsync(partId);
           return MapToApplicationModel(result);
       }
       catch (VisualException vex)
       {
           // Map VISUAL exceptions to user-friendly messages
           throw new ApplicationException($"Unable to retrieve inventory for {partId}: {vex.Message}", vex);
       }
       // License automatically released by using statement
   }
   ```

---

## Troubleshooting Common Issues

### Build Issues

#### "Project file does not exist" Error
```bash
# Verify you're in the correct directory
pwd
ls -la MTM_Inventory_Application_Avalonia.sln
```

#### "Package restore failed" Error
```bash
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore --force
```

#### StyleCop Warnings
- Review `.editorconfig` settings in repository root
- Most StyleCop warnings are non-blocking and can be addressed gradually
- Critical warnings should be fixed before committing

### Runtime Issues

#### "Unable to find VISUAL database" Error
- Verify `Database.config` file exists in `References/Visual DLL & Config Files/`
- Check database connection strings in configuration
- For development, ensure test database is accessible

#### Login Issues
- **Development:** Use Admin/Admin credentials
- **Production:** Verify VISUAL user credentials and permissions
- Check network connectivity to VISUAL server

#### Performance Issues
- Monitor VISUAL license usage - ensure licenses are properly released
- Check for memory leaks in long-running operations
- Review database query performance

### Development Environment Issues

#### Avalonia Designer Not Working
1. Restart Visual Studio
2. Rebuild solution
3. Check Avalonia extension is installed and updated

#### Git Issues
```bash
# Reset to clean state if needed
git stash                          # Save current changes
git pull origin main              # Get latest changes
git stash pop                     # Restore your changes
```

---

## Testing and Validation

### Manual Testing Process

1. **Application Startup**
   - Application launches without errors
   - Login dialog appears and functions
   - Main interface loads after authentication

2. **Core Functionality**
   - Navigation between features works
   - Error dialogs display properly
   - Data entry and validation functions

3. **Integration Testing**
   - VISUAL connection established (production mode)
   - Database operations succeed
   - Exception handling works correctly

### Automated Testing (Future)

Current status: Manual testing only
Planned additions:
- Unit tests for ViewModels and Services
- Integration tests for VISUAL API
- UI automation tests for critical workflows

---

## Resources and Documentation

### Essential Reading
- [Architecture Guide](README_Architecture.md) - System design patterns
- [Coding Conventions](../../HTML/Technical/coding-conventions.html) - Development standards
- [Code Map](../Development/README_Code_Map.md) - File organization

### External Documentation
- [Avalonia Framework](https://docs.avaloniaui.net/) - UI framework documentation
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/) - Platform documentation
- [CommunityToolkit.Mvvm](https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) - MVVM framework

### Development Tools
- [GitHub Repository](https://github.com/Dorotel/MTM_Inventory_Application_Avalonia)
- [Visual Studio Download](https://visualstudio.microsoft.com/)
- [.NET SDK Download](https://dotnet.microsoft.com/download)

---

## Getting Help

### Internal Resources
- **Code Questions:** Review [Code Map](../Development/README_Code_Map.md) for file responsibilities
- **Architecture Questions:** See [Architecture Guide](README_Architecture.md)
- **Process Questions:** Check [Planned Implementations](../Development/README_Planned_Implementations.md)

### External Support
- **Avalonia Issues:** [GitHub Issues](https://github.com/AvaloniaUI/Avalonia/issues)
- **.NET Issues:** [Microsoft Documentation](https://docs.microsoft.com/en-us/dotnet/)
- **VISUAL Integration:** Infor support documentation (as available)

### Team Communication
- Review existing documentation before asking questions
- Include error messages and steps to reproduce issues
- Reference specific files and line numbers when possible

---

## Implementation Checklist

### Initial Setup
- [ ] .NET 8 SDK installed and verified
- [ ] Development environment configured (VS 2022 or VS Code)
- [ ] Repository cloned and accessible
- [ ] Initial build completed successfully
- [ ] Application runs and displays login dialog

### Development Readiness
- [ ] Coding conventions documentation reviewed
- [ ] Architecture guide understanding confirmed
- [ ] Git workflow established
- [ ] Code Map location bookmarked
- [ ] Error handling patterns understood

### First Development Task
- [ ] Feature branch created
- [ ] Code changes follow established patterns
- [ ] Build succeeds without new errors
- [ ] Code Map updated for any .cs file changes
- [ ] Changes committed with descriptive message

---

## Maintenance and Updates

### Regular Maintenance Tasks
- **Weekly:** Update dependencies and check for security updates
- **Monthly:** Review and update documentation for accuracy
- **Quarterly:** Evaluate development tooling and process improvements

### Update Triggers
- **.NET Framework Updates:** Test compatibility and update SDK
- **Avalonia Updates:** Review breaking changes and update accordingly
- **VISUAL System Changes:** Update integration patterns as needed
- **Team Growth:** Expand getting started guide for new team members

### Version History
- **v2.0** (2024-12-22): Comprehensive getting started guide with modern tooling
- **v1.0** (2024-12-19): Initial setup instructions

---

_Documentation Standards:_
- _Follow repository .editorconfig for UTF-8 encoding_
- _Use ASCII punctuation only for platform compatibility_
- _Keep relative links for portability_
- _Update when setup processes change_