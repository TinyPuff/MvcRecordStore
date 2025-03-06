using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRecordStore.Models;

public class RecordPrice
{
    public int ID { get; set; }

    [Required]
    public string Format { get; set; }

    [Required]
    public double Price { get; set; }

    public Record Record { get; set; }

    public int RecordID { get; set; }
}