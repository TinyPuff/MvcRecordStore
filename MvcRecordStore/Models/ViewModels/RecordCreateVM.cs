using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Models;

namespace MvcRecordStore.Models.ViewModels;

public class RecordCreateVM
{
    public int ID { get; set; }

    [Required]
    public string Name { get; set; }

    public enum RecordType
    {
        Single,
        Demo,
        EP,
        Album,
        Compilation
    }

    [Required]
    public int Type { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime ReleaseDate { get; set; }

    public int ArtistID { get; set; }

    public int LabelID { get; set; }

    [Required]
    public List<int> SelectedGenres { get; set; }
}