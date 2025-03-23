using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public class LabelService : ILabelService
{
    private readonly StoreDbContext _context;

    public LabelService(StoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all labels.
    /// </summary>
    /// <returns>A list of Label objects.</returns>
    public List<Label> GetAllLabels()
    {
        return _context.Labels
        .Include(l => l.Artists)
        .Include(l => l.Records)
        .ToList();
    }

    /// <summary>
    /// Retrieves the label with its dependencies.
    /// </summary>
    /// <returns>A Label object.</returns>
    public async Task<Label> GetLabelWithDependencies(int id)
    {
        return await _context.Labels
            .Include(l => l.Artists)
            .Include(l => l.Records)
            .FirstOrDefaultAsync(l => l.ID == id);
    }


    /// <summary>
    /// Retrieves the label without its dependencies.
    /// </summary>
    /// <returns>A Label object.</returns>
    public async Task<Label> GetLabelWithoutDependencies(int id)
    {
        return await _context.Labels.FirstOrDefaultAsync(l => l.ID == id);
    }

    public async Task<List<Label>> ApplyFilters(string? currentFilter, int? sortOrder)
    {
        var labels = GetAllLabels().AsQueryable();
        if (!String.IsNullOrEmpty(currentFilter))
        {
            labels = labels.Where(r => r.Name.ToUpper().Contains(currentFilter.ToUpper()));
        }

        switch (sortOrder)
        {
            case 1:
                labels = labels.OrderBy(i => i.Name);
                break;
            case 2:
                labels = labels.OrderByDescending(i => i.Name);
                break;
            default:
                break;
        }

        return labels.ToList();
    }

    public List<Label> ApplyPagination(List<Label> labels, int pageIndex, int pageSize)
    {
        return labels.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
    }

    /// <summary>
    /// Creates a new label.
    /// </summary>
    /// <returns>A new Label object.</returns>
    public Label CreateNewLabel(LabelCreateVM labelVM)
    {
        return new Label
            {
                Name = labelVM.Name,
                Country = labelVM.Country
            };
    }

    /// <summary>
    /// Retrieves the edit view model for a label.
    /// </summary>
    /// <returns>An LabelCreateVM object.</returns>
    public LabelCreateVM GetLabelViewModelToEdit(Label label)
    {
        return new LabelCreateVM
            {
                ID = label.ID,
                Name = label.Name,
                Country = label.Country
            };
    }

    /// <summary>
    /// Updates the label's properties.
    /// </summary>
    public void UpdateLabelProperties(Label label, LabelCreateVM labelVM)
    {
        label.Name = labelVM.Name;
        label.Country = labelVM.Country;
    }

    /// <summary>
    /// Checks whether a label exists or not.
    /// </summary>
    /// <returns>True if it does, false if not.</returns>
    public bool LabelExists(int id)
    {
        return _context.Labels.Any(e => e.ID == id);
    }
}