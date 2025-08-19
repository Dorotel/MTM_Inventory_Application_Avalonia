# Exception Dialog - Error Display and User Actions
_Functional and Technical Specification_

---

**Metadata**  
- **View:** `Views/Dialogs/ExceptionDialog.axaml`
- **ViewModel:** `ViewModels/Dialogs/ExceptionDialogViewModel.cs`
- **Primary Commands:** Retry, Cancel, Help, CopyDetails
- **Related Services:** IExceptionHandler integration
- **Last Updated:** 2024-12-19
- **Copilot Template Version:** 1.1

---

## Purpose

Centralized error display dialog for all application exceptions. Provides consistent error presentation with severity indicators, expandable details, and user action options.

---

## Global Rules

- Any time the app performs an operation against the Visual server that requires a license, the license MUST be explicitly closed/released immediately after the request completes (success or failure). Always use a short-lived, per-request scope to acquire and dispose the license.
- ALL methods implement try/catch and route errors through IExceptionHandler with the same normalization routine

---

## Scope

- Display error messages with appropriate severity indicators
- Provide expandable technical details for troubleshooting
- Offer user actions: Retry, Help, Copy Details, Cancel
- Support different error types: Info, Warning, Error, Critical
- Integrate with centralized IExceptionHandler service

---

## Platform and Shell Wiring

- **View:** `Views/Dialogs/ExceptionDialog.axaml` (UserControl)
- **Host:** Modal Window created by IExceptionHandler.Handle()
- **Navigation:** Triggered by exception handling throughout application
- **Window Properties:** SizeToContent, CenterScreen positioning

---

## Current Implementation Details

**ExceptionDialog.axaml Structure:**
- Header with severity icon and error title
- Message text with wrapping support
- Expandable details panel with read-only TextBox
- Action buttons: Retry, Help, Copy Details, Cancel

**Severity Indicators:**
- Info: Blue information-outline icon
- Warning: Amber alert-outline icon  
- Error: Red alert-circle icon
- Critical: Dark red alert-octagon icon

**Visual Features:**
- Dynamic icon colors based on error type
- Expandable details with collapsible header
- Action buttons with proper spacing and alignment

---

## Fields and Validation Rules

**ExceptionDialogViewModel Properties:**
- `Title` (string) - Main error title
- `Message` (string) - User-friendly error description
- `Details` (string) - Technical error details (stack trace, etc.)
- `ShortTitle` (string) - Brief title for details expander
- `IsDetailsOpen` (bool) - Controls details panel expansion
- `ErrorType` (enum) - Info, Warning, Error, Critical

**Computed Properties:**
- `IconBackground` - Color brush for error type
- `IconBrush` - Foreground color for icon
- `IconKindName` - MaterialDesign icon identifier

---

## Visual API Commands (by scenario)

**No Direct Visual API Integration:**
- ExceptionDialog displays errors from Visual API calls
- Does not initiate Visual server connections
- Purely UI component for error presentation

---

## Business Rules and Exceptions

- Error type determines icon color and style
- Details panel provides technical information for support
- Retry action available for recoverable errors
- Cancel always available to dismiss dialog
- Copy Details for technical support scenarios

---

## Workflows

1. **Error Occurs:** Application code throws exception
2. **Exception Routing:** IExceptionHandler.Handle(ex, context) called
3. **Dialog Creation:** ExceptionDialogViewModel populated with error info
4. **Dialog Display:** Modal window shown with ExceptionDialog content
5. **User Action:** User selects Retry, Help, Copy Details, or Cancel
6. **Dialog Close:** Window closed, control returned to calling code

---

## ViewModel/Command Conventions

**ExceptionDialogViewModel Commands:**
- `ExceptionDialog_Button_RetryCommand` - Currently cycles error types for testing
- `ExceptionDialog_Button_CancelCommand` - Dismiss dialog (placeholder)
- `ExceptionDialog_Button_HelpCommand` - Show help information (placeholder)
- `ExceptionDialog_Button_CopyDetailsCommand` - Copy technical details (placeholder)

**Property Notifications:**
- ErrorType changes notify dependent computed properties
- Proper ObservableProperty attributes for MVVM binding

---

## Integration & Storage

**Services Used:**
- IExceptionHandler creates and displays the dialog
- No persistent storage - transient error display only

**Integration Points:**
- Called from all ViewModels and services
- Consistent error presentation across application
- Modal dialog pattern for user attention

---

## Keyboard/Scanner UX

**Current Implementation:**
- Standard dialog keyboard navigation
- Tab between action buttons
- Esc likely dismisses dialog (framework default)

**Planned Enhancements:**
- Enter key for default action (Retry or Cancel)
- Ctrl+C for copy details shortcut
- F1 for help action

---

## UI Scaffold

**Views:**
- `Views/Dialogs/ExceptionDialog.axaml` - error display dialog
- MaterialDesign icons for severity indicators
- Expandable details panel for technical information

**ViewModels:**
- `ViewModels/Dialogs/ExceptionDialogViewModel.cs` - error data and actions
- ErrorType enum for severity classification
- Computed properties for dynamic styling

**Hosting:**
- Created by IExceptionHandler as modal Window
- CenterScreen positioning and SizeToContent sizing

---

## Testing & Acceptance

- Dialog displays with appropriate severity icon and colors
- Error message shows clearly with text wrapping
- Details panel expands/collapses properly
- All action buttons are functional
- Modal behavior prevents interaction with parent window
- Dialog closes properly on user action
- Integration with IExceptionHandler service works

---

## References

- ../../Views/Dialogs/ExceptionDialog.axaml (UI implementation)
- ../../ViewModels/Dialogs/ExceptionDialogViewModel.cs (dialog logic)
- ../../Services/Service_ExceptionHandler.cs (service integration)
- ./ExceptionHandling.md (overall error handling strategy)

---

## Implementation Status

- **Current:** Fully implemented with all error types and UI elements
- **Commands:** Basic structure implemented, actions need full implementation
- **Styling:** Complete with dynamic icons and colors
- **Integration:** Working with IExceptionHandler service

---

## TODOs / Copilot Agent Notes

- [ ] Implement actual Copy Details functionality (clipboard)
- [ ] Implement Help action (context-sensitive help)
- [ ] Enhance Retry logic beyond error type cycling
- [ ] Add keyboard shortcuts for common actions
- [ ] Consider adding error logging/reporting capabilities
- [ ] Add proper Cancel dialog close behavior