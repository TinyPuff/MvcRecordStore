using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models.ViewModels;

public class RecordDetailsVM : Record
{
    public string Input { get; set; }
    
    public int Quantity { get; set; }
}

public enum RecordType
{
    Single,
    Demo,
    EP,
    Album,
    Compilation
}