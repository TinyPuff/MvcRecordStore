using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Models;

namespace MvcRecordStore.Models.ViewModels;

public class LabelCreateVM
{
    public int ID { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "This field does not accept more than 100 characters.")]
    public string Name { get; set; }

    [Required]
    [StringLength(60, ErrorMessage = "This field does not accept more than 60 characters.")]
    public string Country { get; set; }
}