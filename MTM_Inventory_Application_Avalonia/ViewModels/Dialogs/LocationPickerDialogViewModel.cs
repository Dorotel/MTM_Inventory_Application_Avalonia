using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

public class LocationBalanceDto
{
    public string Location { get; set; } = string.Empty;
    public decimal OnHand { get; set; }
    public decimal Allocated { get; set; }
    public decimal Available { get; set; }
    public string Warehouse { get; set; } = string.Empty;
    public string Site { get; set; } = string.Empty;
    public string BinType { get; set; } = string.Empty;
}

public partial class LocationPickerDialogViewModel : ObservableObject
{
    [ObservableProperty] private string headerText = string.Empty;

    // Existing filters
    [ObservableProperty] private string filters_Text = string.Empty;
    [ObservableProperty] private bool filters_NonZeroOnly;
    [ObservableProperty] private bool filters_CurrentWarehouseOnly = true;
    [ObservableProperty] private bool filters_CurrentSiteOnly = true;

    // Added for XAML bindings
    [ObservableProperty] private bool filters_ActiveOnly;
    [ObservableProperty] private bool filters_SearchInDescription;
    [ObservableProperty] private int filters_ModeIndex;

    // Search text kept in sync with Filters_Text
    [ObservableProperty] private string searchText = string.Empty;

    partial void OnSearchTextChanged(string value)
    {
        if (Filters_Text != value)
            Filters_Text = value;

        LoadSeedResults();
    }

    partial void OnFilters_TextChanged(string value)
    {
        if (SearchText != value)
            SearchText = value;

        LoadSeedResults();
    }

    partial void OnFilters_ModeIndexChanged(int value) => LoadSeedResults();
    partial void OnFilters_ActiveOnlyChanged(bool value) => LoadSeedResults();
    partial void OnFilters_SearchInDescriptionChanged(bool value) => LoadSeedResults();

    [ObservableProperty] private ObservableCollection<LocationBalanceDto> results = new();
    [ObservableProperty] private LocationBalanceDto? selectedRow;

    [RelayCommand] private void LocationPickerDialog_Button_Select() { }
    [RelayCommand] private void LocationPickerDialog_Button_Cancel() { }

    public LocationPickerDialogViewModel()
    {
        LoadSeedResults();
    }

    private List<LocationBalanceDto> GenerateCandidates(string seed)
    {
        var list = new List<LocationBalanceDto>();
        string[] warehouses = new[] { "MAIN", "A1", "B2" };
        string[] sites = new[] { "HQ", "EAST", "WEST" };
        string[] binTypes = new[] { "STD", "BULK", "RACK" };

        for (int i = 1; i <= 30; i++)
        {
            var onHand = (decimal)(i * 3 % 25);
            var allocated = (decimal)(i * 2 % 10);
            var available = onHand - allocated;

            list.Add(new LocationBalanceDto
            {
                Location = $"LOC-{i:000}-{(char)('A' + (i % 6))}",
                OnHand = onHand,
                Allocated = allocated,
                Available = available,
                Warehouse = warehouses[i % warehouses.Length],
                Site = sites[i % sites.Length],
                BinType = binTypes[i % binTypes.Length]
            });
        }

        // Extra entries influenced by seed to make design-time/search interesting
        if (!string.IsNullOrWhiteSpace(seed))
        {
            list.Add(new LocationBalanceDto { Location = $"{seed.ToUpperInvariant()}-X01", OnHand = 12, Allocated = 2, Available = 10, Warehouse = "MAIN", Site = "HQ", BinType = "STD" });
            list.Add(new LocationBalanceDto { Location = $"A-{seed.ToUpperInvariant()}-Z99", OnHand = 5, Allocated = 1, Available = 4, Warehouse = "A1", Site = "EAST", BinType = "RACK" });
        }

        return list;
    }

    private void LoadSeedResults()
    {
        var seed = (SearchText ?? string.Empty).Trim();
        var candidates = GenerateCandidates(seed);

        IEnumerable<LocationBalanceDto> filtered = candidates;

        if (!string.IsNullOrWhiteSpace(seed))
        {
            if (Filters_ModeIndex == 1)
            {
                filtered = filtered.Where(r => r.Location.StartsWith(seed, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                filtered = filtered.Where(r => r.Location.Contains(seed, StringComparison.OrdinalIgnoreCase));
            }
        }

        // Basic interpretation for demo: "Active" => Available > 0 or NonZeroOnly
        if (Filters_ActiveOnly || Filters_NonZeroOnly)
        {
            filtered = filtered.Where(r => r.Available > 0);
        }

        if (Filters_CurrentWarehouseOnly)
        {
            filtered = filtered.Where(r => string.Equals(r.Warehouse, "MAIN", StringComparison.OrdinalIgnoreCase));
        }

        if (Filters_CurrentSiteOnly)
        {
            filtered = filtered.Where(r => string.Equals(r.Site, "HQ", StringComparison.OrdinalIgnoreCase));
        }

        var finalList = filtered
            .OrderBy(r => r.Location, StringComparer.OrdinalIgnoreCase)
            .Take(200)
            .ToList();

        Results.Clear();
        foreach (var row in finalList)
        {
            Results.Add(row);
        }

        SelectedRow = Results.FirstOrDefault();
    }
}
