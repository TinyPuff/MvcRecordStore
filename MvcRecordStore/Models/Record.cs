using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models;

public class Record
{
    public int ID { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public RecordType Type { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime ReleaseDate { get; set; }

    public int Stock { get; set; }

    public ICollection<RecordPrice> Prices { get; set; }

    public Artist Artist { get; set; }

    public int ArtistID { get; set; }

    public Label? Label { get; set; }

    public int? LabelID { get; set; }

    [Required]
    public ICollection<Genre> Genres { get; set; }
}

public enum RecordType
{
    Single,
    Demo,
    EP,
    Album,
    Compilation
}