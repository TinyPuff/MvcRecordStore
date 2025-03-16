namespace MvcRecordStore.Models.ViewModels;

public class HomeVM
{
    public int ID { get; set; }

    public string Name { get; set; }

    public string ArtistName { get; set; }

    public string LabelName { get; set; }

    public ICollection<RecordPrice>? Prices { get; set; }
}