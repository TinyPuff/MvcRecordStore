using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Data;

namespace MvcRecordStore.Models;

public class CartItem
{
    public int ID { get; set; }

    [ForeignKey(nameof(BuyerID))]
    public StoreUser Buyer { get; set; }

    public string BuyerID { get; set; }

    public int Quantity { get; set; } = 0;

    public RecordPrice Product { get; set; }

    public int ProductID { get; set; }

    public bool PaidFor { get; set; } = false;
}