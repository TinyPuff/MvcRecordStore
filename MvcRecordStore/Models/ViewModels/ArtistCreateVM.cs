using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Models;

namespace MvcRecordStore.Models.ViewModels;

public class ArtistCreateVM
{
    public int ID { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Country { get; set; }

    public int LabelID { get; set; }

    [Required]
    public List<int> SelectedGenres { get; set; }
}