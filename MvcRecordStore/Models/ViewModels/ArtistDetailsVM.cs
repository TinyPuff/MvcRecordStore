namespace MvcRecordStore.Models.ViewModels;

public class ArtistDetailsVM
{
    public Artist Artist { get; set; }

    public ICollection<Record> Records { get; set; }
}