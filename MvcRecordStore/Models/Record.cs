using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models;

public class Record
{
    public int ID { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime ReleaseDate { get; set; }

    public Artist Artist { get; set; }

    public int ArtistID { get; set; }

    public Label? Label { get; set; }

    public int? LabelID { get; set; }

    [Required]
    public ICollection<Genre> Genres { get; set; }
}