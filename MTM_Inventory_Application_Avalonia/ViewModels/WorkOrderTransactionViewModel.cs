using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using MTM_Inventory_Application_Avalonia.Views.Dialogs;
using MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

namespace MTM_Inventory_Application_Avalonia.ViewModels;

public partial class WorkOrderTransactionViewModel : ObservableObject
{
    private readonly IExceptionHandler _exceptionHandler;

    public IPartDialogService? PartDialog { get; set; }

    public WorkOrderTransactionViewModel() : this(new ExceptionHandler()) { }

    public WorkOrderTransactionViewModel(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;

        WorkOrder_DataGrid_Results = new ReadOnlyObservableCollection<WorkOrderResultRow>(_workOrder_DataGrid_Results);
    }

    // Fields and labels
    [ObservableProperty] private string workOrder_TextBox_WorkOrderId = string.Empty;
    [ObservableProperty] private int workOrder_ComboBox_TransactionTypeIndex = 0; // 0 Issue, 1 Receipt
    [ObservableProperty] private string workOrder_TextBox_Quantity = string.Empty;
    [ObservableProperty] private string workOrder_TextBox_FromLocation = string.Empty;
    [ObservableProperty] private string workOrder_TextBox_ToLocation = string.Empty;
    [ObservableProperty] private string workOrder_Label_WarehouseId = "002";
    [ObservableProperty] private string workOrder_Label_SiteId = "SITE";

    // Results DataGrid
    private readonly ObservableCollection<WorkOrderResultRow> _workOrder_DataGrid_Results = new();
    public ReadOnlyObservableCollection<WorkOrderResultRow> WorkOrder_DataGrid_Results { get; }

    private void SetWorkOrderResults(System.Collections.Generic.IEnumerable<WorkOrderResultRow> rows)
    {
        _workOrder_DataGrid_Results.Clear();
        foreach (var r in rows)
            _workOrder_DataGrid_Results.Add(r);
    }

    // Commands
    [RelayCommand]
    private void WorkOrder_Button_ValidateWorkOrder()
    {
        try { /* TODO */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_ValidateWorkOrder)); }
    }

    [RelayCommand]
    private void WorkOrder_Button_CheckAvailability()
    {
        try { _workOrder_DataGrid_Results.Clear(); }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_CheckAvailability)); }
    }

    [RelayCommand]
    private void WorkOrder_Button_PostTransaction()
    {
        try { /* TODO */ }
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

            _workOrder_DataGrid_Results.Clear();
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_Reset)); }
    }

    [RelayCommand]
    private async Task WorkOrder_Button_ResolveIncompletePartId()
    {
        try
        {
            var win = new Window
            {
                Title = "Incomplete Part",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new IncompletePartDialog { DataContext = new IncompletePartDialogViewModel() }
            };
            win.Show();
            await Task.CompletedTask;
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_ResolveIncompletePartId)); }
    }

    [RelayCommand]
    private void WorkOrder_Button_ShowLocationModal(string? target)
    {
        try
        {
            var win = new Window
            {
                Title = "Location Picker",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new LocationPickerDialog { DataContext = new LocationPickerDialogViewModel() }
            };
            win.Show();
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(WorkOrder_Button_ShowLocationModal)); }
    }

    // Row type displayed by the DataGrid
    public sealed record WorkOrderResultRow(
        string PartNumber,
        string Description,
        string FromLocation,
        string ToLocation,
        decimal AvailableQuantity,
        string Uom,
        string WarehouseId,
        string SiteId);
}
