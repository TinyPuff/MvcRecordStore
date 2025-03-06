using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Data;

namespace MvcRecordStore.Models;

public class Order
{
    public int ID { get; set; }

    public int? TrackingNumber { get; set; }

    [ForeignKey(nameof(BuyerID))]
    public StoreUser Buyer { get; set; }

    public string BuyerID { get; set; }

    public double TotalPrice { get; set; } = 0.00;
}