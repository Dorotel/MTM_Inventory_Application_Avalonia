using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Inventory_Application_Avalonia.Services;
using System;

namespace MTM_Inventory_Application_Avalonia.ViewModels;

public partial class InventoryTransferViewModel : ObservableObject
{
    private readonly IExceptionHandler _exceptionHandler;

    public InventoryTransferViewModel() : this(new ExceptionHandler()) {}

    public InventoryTransferViewModel(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
    }

    // Fields
    [ObservableProperty] private string inventoryTransfer_TextBox_ItemId = string.Empty;
    [ObservableProperty] private string inventoryTransfer_TextBox_Quantity = string.Empty;
    [ObservableProperty] private string inventoryTransfer_TextBox_FromLocation = string.Empty;
    [ObservableProperty] private string inventoryTransfer_TextBox_ToLocation = string.Empty;

    // Read-only labels
    [ObservableProperty] private string inventoryTransfer_Label_WarehouseId = "002";
    [ObservableProperty] private string inventoryTransfer_Label_SiteId = "SITE";

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
        try { /* TODO: VISUAL availability check via service */ }
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
        }
        catch (Exception ex) { _exceptionHandler.Handle(ex, nameof(InventoryTransfer_Button_Reset)); }
    }
}
