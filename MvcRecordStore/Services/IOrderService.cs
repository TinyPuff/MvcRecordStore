using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public interface IOrderService
{
    List<Order> GetAllOrders();
    List<Order> GetUserOrders(StoreUser user);
    Task<Order> GetOrderWithDependencies(int id);
    Task<List<Order>> ApplyFilters(IQueryable<Order> items, string? currentFilter, int? sortOrder);
    List<Order> ApplyPagination(List<Order> items, int pageIndex, int pageSize);
}