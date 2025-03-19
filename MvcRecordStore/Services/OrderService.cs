using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Parbad;
using Microsoft.Identity.Client;

namespace MvcRecordStore.Services;

public class OrderService : IOrderService
{
    private readonly StoreDbContext _context;

    public OrderService(StoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a list of the user's orders.
    /// </summary>
    /// <param name="user">The current logged in user.</param>
    /// <returns>A list of Order objects.</returns>
    public List<Order> GetUserOrders(StoreUser user)
    {
        return _context.Orders
            .Where(o => o.Invoice.Buyer == user)
            .Include(o => o.Invoice)
            .ToList();
    }

    /// <summary>
    /// Retrieves an order's info with its dependencies.
    /// </summary>
    /// <param name="id">Order's ID</param>
    /// <returns>An Order object.</returns>
    public async Task<Order> GetOrderWithDependencies(int id)
    {
        return await _context.Orders
            .Include(o => o.Invoice)
                .ThenInclude(o => o.Buyer)
            .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                    .ThenInclude(p => p.Record)
                        .ThenInclude(p => p.Artist)
            .FirstOrDefaultAsync(o => o.ID == id);
    }

    public async Task<List<Order>> ApplyFilters(IQueryable<Order> orders, string? currentFilter, int? sortOrder)
    {
        if (!String.IsNullOrEmpty(currentFilter))
        {
            orders = orders.Where(r => r.Invoice.TrackingNumber.ToString() == currentFilter);
        }

        switch (sortOrder)
        {
            case 1:
                orders = orders.OrderBy(i => i.Invoice.PaymentDateTime);
                break;
            case 2:
                orders = orders.OrderByDescending(i => i.Invoice.PaymentDateTime);
                break;
            case 3:
                orders = orders.OrderBy(i => i.Invoice.TotalPrice);
                break;
            case 4:
                orders = orders.OrderByDescending(i => i.Invoice.TotalPrice);
                break;
            default:
                break;
        }

        return orders.ToList();
    }

    public List<Order> ApplyPagination(List<Order> items, int pageIndex, int pageSize)
    {
        return items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
    }
}