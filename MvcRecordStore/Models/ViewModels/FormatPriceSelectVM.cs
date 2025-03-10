using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models.ViewModels;

public class FormatPriceSelectVM
{
    public List<FormatPriceVM> FormatPrices { get; set; }
}