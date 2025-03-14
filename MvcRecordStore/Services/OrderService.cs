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
            .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                    .ThenInclude(p => p.Record)
                        .ThenInclude(p => p.Artist)
            .FirstOrDefaultAsync(o => o.ID == id);
    }
}