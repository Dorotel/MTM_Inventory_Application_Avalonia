using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using System;

namespace MTM_Inventory_Application_Avalonia.ViewModels;

public partial class WorkOrderTransactionViewModel : ObservableObject
{
    private readonly IExceptionHandler _exceptionHandler;

    public WorkOrderTransactionViewModel() : this(new ExceptionHandler()) {}

    public WorkOrderTransactionViewModel(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
    }

    // Fields and labels
    [ObservableProperty] private string workOrder_TextBox_WorkOrderId = string.Empty;
    [ObservableProperty] private int workOrder_ComboBox_TransactionTypeIndex = 0; // 0 Issue, 1 Receipt
    [ObservableProperty] private string workOrder_TextBox_Quantity = string.Empty;
    [ObservableProperty] private string workOrder_TextBox_FromLocation = string.Empty;
    [ObservableProperty] private string workOrder_TextBox_ToLocation = string.Empty;
    [ObservableProperty] private string workOrder_Label_WarehouseId = "002";
    [ObservableProperty] private string workOrder_Label_SiteId = "SITE";

    // Commands (placeholders)
    [RelayCommand]
    private void WorkOrder_Button_ValidateWorkOrder()
    {
        try { /* TODO: VISUAL validate work order */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_ValidateWorkOrder)); }
    }

    [RelayCommand]
    private void WorkOrder_Button_CheckAvailability()
    {
        try { /* TODO: VISUAL availability for Issue */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_CheckAvailability)); }
    }

    [RelayCommand]
    private void WorkOrder_Button_PostTransaction()
    {
        try { /* TODO: VISUAL InventoryTransaction for Issue/Receipt */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_PostTransaction)); }
    }

    [RelayCommand]
    private void WorkOrder_Button_Reset()
    {
        try
        {
            WorkOrder_TextBox_WorkOrderId = string.Empty;
            WorkOrder_ComboBox_TransactionTypeIndex = 0;
            WorkOrder_TextBox_Quantity = string.Empty;
            WorkOrder_TextBox_FromLocation = string.Empty;
            WorkOrder_TextBox_ToLocation = string.Empty;
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_Reset)); }
    }
}
