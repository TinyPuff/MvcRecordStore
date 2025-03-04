using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models;

public class Artist
{
    public int ID { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Country { get; set; }

    public Label Label { get; set; }

    public int LabelID { get; set; }

    [Required]
    public ICollection<Genre> Genres { get; set; }

    [Required]
    public ICollection<Record> Records { get; set; }
}