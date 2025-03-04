using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models;

public class Label
{
    public int ID { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    [StringLength(60, ErrorMessage = "This field does not accept more than 60 characters.")]
    public string Country { get; set; }

    public ICollection<Artist>? Artists { get; set; }

    public ICollection<Record>? Records { get; set; }
}