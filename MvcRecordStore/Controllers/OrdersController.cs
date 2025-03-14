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

[Authorize]
public class OrdersController : Controller
{
    private readonly StoreDbContext _context;
    private readonly UserManager<StoreUser> _userManager;
    private readonly IOrderService _orderService;

    public OrdersController(StoreDbContext context, UserManager<StoreUser> userManager, IOrderService orderService)
    {
        _context = context;
        _userManager = userManager;
        _orderService = orderService;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(_orderService.GetUserOrders(user));
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _orderService.GetOrderWithDependencies((int)id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}