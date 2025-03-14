using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Data;

namespace MvcRecordStore.Models;

public class Order
{
    public int ID { get; set; }

    public Invoice Invoice { get; set; }

    public int InvoiceID { get; set; }

    public string? Status { get; set; }

    public ICollection<CartItem> Products { get; set; }
}