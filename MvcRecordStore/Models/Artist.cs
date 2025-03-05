using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models;

public class Artist
{
    public int ID { get; set; }

    [Required]
    [StringLength(150, ErrorMessage = "This field does not accept more than 150 characters.")]
    public string Name { get; set; }

    [Required]
    [StringLength(60, ErrorMessage = "This field does not accept more than 150 characters.")]
    public string Country { get; set; }

    public Label Label { get; set; }

    public int LabelID { get; set; }

    [Required]
    public ICollection<Genre> Genres { get; set; }

    public ICollection<Record>? Records { get; set; }
}