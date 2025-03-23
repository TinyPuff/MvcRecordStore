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

namespace MvcRecordStore.Controllers;

[Authorize(Roles = "Admin")]
public class OrdersController : Controller
{
    private readonly StoreDbContext _context;
    private readonly UserManager<StoreUser> _userManager;
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;

    public OrdersController(StoreDbContext context, UserManager<StoreUser> userManager, IOrderService orderService, ICartService cartService)
    {
        _context = context;
        _userManager = userManager;
        _orderService = orderService;
        _cartService = cartService;
    }

    // GET: Orders
    public async Task<IActionResult> Index(string currentFilter, int sortOrder, int pageIndex)
    {
        pageIndex = pageIndex >= 1 ? pageIndex : 1;
        PopulateIndexViewData(currentFilter, sortOrder, pageIndex);
        
        var user = await _userManager.GetUserAsync(User);
        var orders = _orderService.GetUserOrders(user);
        var pageSize = 10;
        var data = await _orderService.ApplyFilters(orders.AsQueryable(), currentFilter, sortOrder);
        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);

        return View(_orderService.ApplyPagination(data, pageIndex, pageSize));
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _orderService.GetOrderWithDependencies((int)id);
        var user = await _userManager.GetUserAsync(User);
        if (order == null || order.Invoice.Buyer != user)
        {
            return NotFound();
        }

        return View(order);
    }

    public void PopulateIndexViewData(string? currentFilter, int? sortOrder, int? pageIndex)
    {
        ViewData["PageIndex"] = pageIndex;
        ViewData["CurrentFilter"] = currentFilter;
        ViewData["SortOrder"] = sortOrder;
    }
}