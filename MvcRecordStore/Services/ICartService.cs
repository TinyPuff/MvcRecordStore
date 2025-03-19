using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parbad;

namespace MvcRecordStore.Services;

public interface ICartService
{
    Task<List<CartItem>> GetUserCart(StoreUser user);
    Task<CartItem> GetItemWithDependencies(int id);
    Task<List<CartItem>> ApplyFilters(IQueryable<CartItem> items, string? currentFilter, int? sortOrder);
    List<CartItem> ApplyPagination(List<CartItem> items, int pageIndex, int pageSize);
    Task<bool> IsInvoiceAlreadyProcessed(IPaymentFetchResult invoice, StoreUser user);
    Invoice CreateNewInvoice(IPaymentVerifyResult invoice, StoreUser user);
    Task<Order> CreateNewOrder(Invoice invoice, StoreUser user);
    Task<long> GetTotalPrice(StoreUser user);
}