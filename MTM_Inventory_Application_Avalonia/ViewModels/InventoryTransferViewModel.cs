using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTM_Inventory_Application_Avalonia.ViewModels;

public partial class InventoryTransferViewModel : ObservableObject
{
    private readonly IExceptionHandler _exceptionHandler;

    public IPartDialogService? PartDialog { get; set; }

    public InventoryTransferViewModel() : this(new ExceptionHandler()) {}

    public InventoryTransferViewModel(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;

        // Keep ItemsSource stable; mutate the inner collection
        InventoryTransfer_DataGrid_Results = new ReadOnlyObservableCollection<InventoryTransferResultRow>(_inventoryTransfer_DataGrid_Results);
    }

    // Fields
    [ObservableProperty] private string inventoryTransfer_TextBox_ItemId = string.Empty;
    [ObservableProperty] private string inventoryTransfer_TextBox_Quantity = string.Empty;
    [ObservableProperty] private string inventoryTransfer_TextBox_FromLocation = string.Empty;
    [ObservableProperty] private string inventoryTransfer_TextBox_ToLocation = string.Empty;

    // Read-only labels
    [ObservableProperty] private string inventoryTransfer_Label_WarehouseId = "002";
    [ObservableProperty] private string inventoryTransfer_Label_SiteId = "SITE";

    // Results DataGrid backing store (mutable) + read-only projection for binding
    private readonly ObservableCollection<InventoryTransferResultRow> _inventoryTransfer_DataGrid_Results = new();
    public ReadOnlyObservableCollection<InventoryTransferResultRow> InventoryTransfer_DataGrid_Results { get; }

    // Helper to replace current results
    private void SetInventoryTransferResults(System.Collections.Generic.IEnumerable<InventoryTransferResultRow> rows)
    {
        _inventoryTransfer_DataGrid_Results.Clear();
        foreach (var r in rows)
            _inventoryTransfer_DataGrid_Results.Add(r);
    }

    // Commands (placeholders with centralized error handling)
    [RelayCommand]
    private void InventoryTransfer_Button_ValidateItem()
    {
        try { /* TODO: VISUAL validate item via service */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(InventoryTransfer_Button_ValidateItem)); }
    }

    [RelayCommand]
    private void InventoryTransfer_Button_CheckAvailabilityFrom()
    {
        try
        {
            // TODO: Replace with service call; example usage:
            // var rows = await _inventoryService.GetAvailabilityAsync(...);
            // SetInventoryTransferResults(rows);

            // For now, ensure previous results are cleared
            _inventoryTransfer_DataGrid_Results.Clear();
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(InventoryTransfer_Button_CheckAvailabilityFrom)); }
    }

    [RelayCommand]
    private void InventoryTransfer_Button_ShowLocationModal()
    {
        try { /* TODO: open LocationPickerDialog */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(InventoryTransfer_Button_ShowLocationModal)); }
    }

    [RelayCommand]
    private void InventoryTransfer_Button_PostTransfer()
    {
        try { /* TODO: VISUAL InventoryTransaction for transfer */ }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(InventoryTransfer_Button_PostTransfer)); }
    }

    [RelayCommand]
    private void InventoryTransfer_Button_Reset()
    {
        try
        {
            InventoryTransfer_TextBox_ItemId = string.Empty;
            InventoryTransfer_TextBox_Quantity = string.Empty;
            InventoryTransfer_TextBox_FromLocation = string.Empty;
            InventoryTransfer_TextBox_ToLocation = string.Empty;

            _inventoryTransfer_DataGrid_Results.Clear();
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(InventoryTransfer_Button_Reset)); }
    }

    [RelayCommand]
    private async Task InventoryTransfer_Button_ResolveIncompletePartId()
    {
        try
        {
            var seed = InventoryTransfer_TextBox_ItemId;
            if (PartDialog is null) return;
            var picked = await PartDialog.PickPartAsync(seed);
            if (!string.IsNullOrWhiteSpace(picked))
            {
                InventoryTransfer_TextBox_ItemId = picked!;
            }
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(InventoryTransfer_Button_ResolveIncompletePartId)); }
    }

    // Row type displayed by the DataGrid
    public sealed record InventoryTransferResultRow(
        string PartNumber,
        string Description,
        string FromLocation,
        string ToLocation,
        decimal AvailableQuantity,
        string Uom,
        string WarehouseId,
        string SiteId);
}
