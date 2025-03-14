using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Data;

namespace MvcRecordStore.Models;

public class Invoice
{
    public int ID { get; set; }

    public string? AdditionalData { get; set; }

    public StoreUser Buyer { get; set; }

    public string BuyerID { get; set; }

    public string? GatewayAccountName { get; set; }

    public string? GatewayName { get; set; }

    public string? GatewayResponseCode { get; set; }

    public bool IsSucceed { get; set; }

    public string Message { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime PaymentDateTime { get; set; }

    public string Status { get; set; }

    public double TotalPrice { get; set; }

    public long TrackingNumber { get; set; }

    public string? TransactionCode { get; set; }
}