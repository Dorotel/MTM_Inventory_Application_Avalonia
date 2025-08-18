using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTM_Inventory_Application_Avalonia.ViewModels.Dialogs;

public class PartSearchResult
{
    public string PartId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";

    // Extra columns to resemble the Visual Parts window
    public string StockUm { get; set; } = "EA";
    public bool Fab { get; set; } // Fabricated
    public bool Pur { get; set; } // Purchased
}

public partial class IncompletePartDialogViewModel : ObservableObject
{
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<PartSearchResult> results = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(IncompletePartDialog_Button_SelectCommand))]
    private PartSearchResult? selectedPart;

    // Filters
    [ObservableProperty] private bool filters_ActiveOnly;
    [ObservableProperty] private bool filters_SearchInDescription;
    // 0 = Contains (%seed%), 1 = Starts With (seed%)
    [ObservableProperty] private int filters_ModeIndex;

    public event Action<string>? OnSelected;
    public event Action? OnCanceled;

    public bool CanSelect => SelectedPart != null && string.Equals(SelectedPart.Status, "Active", StringComparison.OrdinalIgnoreCase);

    public IncompletePartDialogViewModel() : this(string.Empty) { }

    public IncompletePartDialogViewModel(string seed)
    {
        searchText = seed ?? string.Empty;
        filters_ModeIndex = 0; // Contains by default
        filters_ActiveOnly = false; // show all by default; inactive cannot be selected
        LoadSeedResults();
    }

    private List<PartSearchResult> GenerateCandidates(string seed)
    {
        seed = seed ?? string.Empty;
        var baseId = string.IsNullOrWhiteSpace(seed) ? "21-28841" : seed; // mimic screenshot format
        var list = new List<PartSearchResult>();

        string[] suffixes = ["-006", "-007", "-008", "-009", "-Blank", "-Patch-1.25", "-Patch-2.00"];        
        for (int i = 0; i < suffixes.Length; i++)
        {
            list.Add(new PartSearchResult
            {
                PartId = baseId + suffixes[i],
                Description = i switch
                {
                    0 => "Bracket-Mntg, Clad Bmpr, LH",
                    1 => "Bracket-Mntg, Clad Bmpr, RH",
                    2 => "Bracket-Mntg, Clad Bmpr, LH",
                    3 => "Bracket-Mntg, Clad Bmpr, RH",
                    4 => "Bracket-Mntg, Clad Bmpr Blank",
                    5 => "Clad Bmpr 1.25x1.25 Weld Patch",
                    6 => "Clad Bmpr 2x2 Weld Patch",
                    _ => $"Item for {baseId}"
                },
                Status = i % 5 == 0 ? "Inactive" : "Active",
                StockUm = "EA",
                Fab = true,
                Pur = i % 2 == 0 ? false : true
            });
        }

        // Noise entries for contains/starts-with simulation
        if (!string.IsNullOrWhiteSpace(baseId))
        {
            list.Add(new PartSearchResult { PartId = $"{baseId}-KIT", Description = "Assembly Kit", Status = "Active", StockUm = "EA", Fab = false, Pur = true });
            list.Add(new PartSearchResult { PartId = $"ALT-{baseId}", Description = "Alternate Part", Status = "Inactive", StockUm = "EA", Fab = false, Pur = true });
        }

        return list;
    }

    private void LoadSeedResults()
    {
        var seed = (SearchText ?? string.Empty).Trim();
        var candidates = GenerateCandidates(seed);

        // Apply filter mode
        IEnumerable<PartSearchResult> filtered = candidates;
        if (!string.IsNullOrWhiteSpace(seed))
        {
            if (Filters_ModeIndex == 1)
            {
                filtered = filtered.Where(r => r.PartId.StartsWith(seed, StringComparison.OrdinalIgnoreCase)
                    || (Filters_SearchInDescription && r.Description.StartsWith(seed, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                filtered = filtered.Where(r => r.PartId.Contains(seed, StringComparison.OrdinalIgnoreCase)
                    || (Filters_SearchInDescription && r.Description.Contains(seed, StringComparison.OrdinalIgnoreCase)));
            }
        }

        if (Filters_ActiveOnly)
        {
            filtered = filtered.Where(r => string.Equals(r.Status, "Active", StringComparison.OrdinalIgnoreCase));
        }

        var finalList = filtered
            .OrderBy(r => r.PartId, StringComparer.OrdinalIgnoreCase)
            .Take(200)
            .ToList();

        Results.Clear();
        foreach (var row in finalList)
        {
            Results.Add(row);
        }

        SelectedPart = Results.FirstOrDefault();
    }

    partial void OnSearchTextChanged(string value) => LoadSeedResults();
    partial void OnFilters_ModeIndexChanged(int value) => LoadSeedResults();
    partial void OnFilters_ActiveOnlyChanged(bool value) => LoadSeedResults();
    partial void OnFilters_SearchInDescriptionChanged(bool value) => LoadSeedResults();

    [RelayCommand(CanExecute = nameof(CanSelect))]
    private void IncompletePartDialog_Button_Select()
    {
        var id = SelectedPart?.PartId;
        if (!string.IsNullOrWhiteSpace(id))
        {
            OnSelected?.Invoke(id!);
        }
    }

    [RelayCommand]
    private void IncompletePartDialog_Button_Cancel()
    {
        OnCanceled?.Invoke();
    }
}
