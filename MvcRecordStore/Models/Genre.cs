using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models;

public class Genre
{
    public int ID { get; set; }

    [Required]
    public string Name { get; set; }

    public ICollection<Artist>? Artists { get; set; }

    public ICollection<Record>? Records { get; set; }
}