using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public interface IOrderService
{
    List<Order> GetUserOrders(StoreUser user);
    Task<Order> GetOrderWithDependencies(int id);
}