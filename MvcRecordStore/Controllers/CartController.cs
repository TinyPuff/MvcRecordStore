using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Services;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Gateway.IdPay;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.ZarinPal;

namespace MvcRecordStore.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly StoreDbContext _context;
    private readonly ICartService _cartService;
    private readonly UserManager<StoreUser> _userManager;
    private readonly IOnlinePayment _onlinePayment;

    public CartController(StoreDbContext context, ICartService cartService, UserManager<StoreUser> userManager, IOnlinePayment onlinePayment)
    {
        _context = context;
        _cartService = cartService;
        _userManager = userManager;
        _onlinePayment = onlinePayment;
    }

    // GET: Cart
    public async Task<IActionResult> Index(string currentFilter, int sortOrder, int pageIndex)
    {
        pageIndex = pageIndex >= 1 ? pageIndex : 1;
        PopulateIndexViewData(currentFilter, sortOrder, pageIndex);

        var user = await _userManager.GetUserAsync(User);
        var cart = (await _cartService.GetUserCart(user)).Where(c => c.PaidFor == false);
        var pageSize = 10;
        var data = await _cartService.ApplyFilters(cart.AsQueryable(), currentFilter, sortOrder);
        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);

        return View(_cartService.ApplyPagination(data, pageIndex, pageSize));
    }

    // GET: Cart/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cartItem = await _cartService.GetItemWithDependencies((int)id);
        if (cartItem == null)
        {
            return NotFound();
        }

        return View(cartItem);
    }
    
    // POST: Cart/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Cart/Payment
    [HttpGet, HttpPost]
    public async Task<IActionResult> Payment()
    {
        var user = await _userManager.GetUserAsync(User);
        var price = await _cartService.GetTotalPrice(user);
        var result = await _onlinePayment.RequestAsync(invoice =>
        {
            invoice
                .UseAutoRandomTrackingNumber() // Need to make it so that this will start from the last Invoice's ID
                .SetAmount(price)
                .SetCallbackUrl("http://localhost:5182/Cart/Verify")
                .SetGateway("ParbadVirtual");
        });

        if (result.IsSucceed)
        {
            // Save the TrackingNumber inside your database.

            // It will redirect the client to the gateway.
            return result.GatewayTransporter.TransportToGateway();
        }
        else
        {
            // The request was not successful. You can see the Message property for more information.
            Console.WriteLine($"Gateway error: {result.Message}");
            return NotFound();
        }
    }

    // GET, POST: Cart/Verify
    [HttpGet, HttpPost] // Gateways contact you with different HTTP methods, so make sure that you support both GET and Post.
    public async Task<IActionResult> Verify()
    {
        var invoice = await _onlinePayment.FetchAsync();
        var user = await _userManager.GetUserAsync(User);
        var verifyResult = await _onlinePayment.VerifyAsync(invoice);
        
        if (!await _cartService.IsInvoiceAlreadyProcessed(invoice, user))
        {
            var newInvoice = _cartService.CreateNewInvoice(verifyResult, user);
            _context.Add(newInvoice);
            if (verifyResult.IsSucceed == true)
            {
                var newOrder = await _cartService.CreateNewOrder(newInvoice, user);
                _context.Add(newOrder);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Orders", new { id = newOrder.ID });
            }
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index", "Orders");
    }

    public void PopulateIndexViewData(string? currentFilter, int? sortOrder, int? pageIndex)
    {
        ViewData["PageIndex"] = pageIndex;
        ViewData["CurrentFilter"] = currentFilter;
        ViewData["SortOrder"] = sortOrder;
    }
}