using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Models;

namespace MvcRecordStore.Models.ViewModels;

public class GenreCreateVM
{
    public int ID { get; set; }

    [Required]
    [StringLength(40, ErrorMessage = "This field does not accept more than 40 characters.")]
    public string Name { get; set; }
}