using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public interface ILabelService
{
    List<Label> GetAllLabels();
    Task<Label> GetLabelWithDependencies(int id);
    Task<Label> GetLabelWithoutDependencies(int id);
    Task<List<Label>> ApplyFilters(string? currentFilter, int? sortOrder);
    List<Label> ApplyPagination(List<Label> labels, int pageIndex, int pageSize);
    Label CreateNewLabel(LabelCreateVM labelVM);
    LabelCreateVM GetLabelViewModelToEdit(Label label);
    void UpdateLabelProperties(Label label, LabelCreateVM labelVM);
    bool LabelExists(int id);
}