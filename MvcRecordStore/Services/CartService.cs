using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Parbad;
using System;

namespace MvcRecordStore.Services;

public class CartService : ICartService
{
    private readonly StoreDbContext _context;
    
    public CartService(StoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all of the user's cart items.
    /// </summary>
    /// <param name="user">Current user</param>
    public async Task<List<CartItem>> GetUserCart(StoreUser user)
    {
        return await _context.CartItems
            .Include(c => c.Buyer)
            .Include(c => c.Product)
                .ThenInclude(p => p.Record)
                    .ThenInclude(r => r.Artist)
                    .ThenInclude(r => r.Label)
            .Where(c => c.Buyer == user).ToListAsync();
    }

    public async Task<CartItem> GetItemWithDependencies(int id)
    {
        return await _context.CartItems
            .Include(c => c.Product)
                .ThenInclude(p => p.Record)
                    .ThenInclude(r => r.Artist)
            .FirstOrDefaultAsync(c => c.ID == id);
    }

    public async Task<List<CartItem>> ApplyFilters(IQueryable<CartItem> items, string? currentFilter, int? sortOrder)
    {
        if (!String.IsNullOrEmpty(currentFilter))
        {
            items = items.Where(r => r.Product.Record.Name.ToUpper().Contains(currentFilter.ToUpper())
                || r.Product.Record.Artist.Name.ToUpper().Contains(currentFilter));
        }

        switch (sortOrder)
        {
            case 1:
                items = items.OrderBy(i => i.Product.Record.Artist.Name);
                break;
            case 2:
                items = items.OrderByDescending(i => i.Product.Record.Artist.Name);
                break;
            case 3:
                items = items.OrderBy(i => i.Product.Price);
                break;
            case 4:
                items = items.OrderByDescending(i => i.Product.Price);
                break;
            default:
                break;
        }

        return items.ToList();
    }

    public List<CartItem> ApplyPagination(List<CartItem> items, int pageIndex, int pageSize)
    {
        return items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
    }

    /// <summary>
    /// Checks whether an Invoice was already processed.
    /// </summary>
    /// <param name="invoice">Invoice returned from the gateway</param>
    /// <param name="user">Current logged in user</param>
    /// <returns>True if it was already processed, false if not.</returns>
    public async Task<bool> IsInvoiceAlreadyProcessed(IPaymentFetchResult invoice, StoreUser user)
    {
        var oldInvoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Buyer == user && i.TrackingNumber == invoice.TrackingNumber);
        if (oldInvoice == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Invoice CreateNewInvoice(IPaymentVerifyResult invoice, StoreUser user)
    {
        return new Invoice
            {
                Buyer = user,
                BuyerID = user.Id,
                GatewayAccountName = invoice.GatewayAccountName,
                GatewayName = invoice.GatewayName,
                GatewayResponseCode = invoice.GatewayResponseCode,
                IsSucceed = invoice.IsSucceed,
                Message = invoice.Message,
                PaymentDateTime = DateTime.Now,
                Status = invoice.Status.ToString(),
                TotalPrice = invoice.Amount,
                TrackingNumber = invoice.TrackingNumber,
                TransactionCode = invoice.TransactionCode
            };
    }

    /// <summary>
    /// Create a new order based on the invoice.
    /// </summary>
    /// <param name="invoice">Verified Invoice</param>
    /// <param name="user">The current logged in user</param>
    /// <returns>An Order object.</returns>
    public async Task<Order> CreateNewOrder(Invoice invoice, StoreUser user)
    {
        return new Order
            {
                Invoice = invoice,
                Status = $"Payment: {invoice.Status}",
                Products = await GetUserCart(user)
            };
    }

    public async Task<long> GetTotalPrice(StoreUser user)
    {
        long price = 0;
        foreach (var item in await GetUserCart(user))
        {
            price += (long)item.Product.Price * item.Quantity;
        }

        return price;
    }
}