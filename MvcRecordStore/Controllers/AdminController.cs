using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MvcRecordStore.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly StoreDbContext _context;
    private readonly UserManager<StoreUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IArtistService _artistService;
    private readonly IGenreService _genreService;
    private readonly ILabelService _labelService;
    private readonly IRecordService _recordService;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public AdminController
    (
        StoreDbContext context,
        UserManager<StoreUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IRecordService recordService,
        IArtistService artistService,
        ILabelService labelService,
        IGenreService genreService,
        ICartService cartService,
        IOrderService orderService
    )
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _recordService = recordService;
        _artistService = artistService;
        _labelService = labelService;
        _genreService = genreService;
        _cartService = cartService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        return View(MakeIndexVM());
    }
    
    public async Task<IActionResult> Users(string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var users = await ApplyFiltersToUsers(searchString, sortOrder);
        var paginatedUsers = ApplyPaginationToUsers(users, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(users.Count() / (double)pageSize);
        ViewBag.TotalItems = users.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, users.Count);

        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            user.Roles = string.Join(", ", roles);
        }

        return View(paginatedUsers);
    }

    public async Task<IActionResult> Artists(string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var artists = await _artistService.ApplyFilters(searchString, sortOrder, null);
        var paginatedArtists = _artistService.ApplyPagination(artists, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(artists.Count() / (double)pageSize);
        ViewBag.TotalItems = artists.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, artists.Count);

        return View(paginatedArtists);
    }

    [Route("Admin/Artists/{id}")]
    public async Task<IActionResult> ArtistsRecords(int id, string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var artist = await _artistService.GetArtistWithDependencies(id);
        var data = await _recordService.ApplyFilters(artist.Records.AsQueryable(), searchString, sortOrder, null);
        var paginatedRecords = _recordService.ApplyPagination(data, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);
        ViewBag.TotalItems = data.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, data.Count);
        
        return View(paginatedRecords);
    }

    public async Task<IActionResult> Genres(string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var data = await _genreService.ApplyFilters(searchString, sortOrder);
        var paginatedGenres = _genreService.ApplyPagination(data, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);
        ViewBag.TotalItems = data.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, data.Count);

        return View(paginatedGenres);
    }

    public async Task<IActionResult> Labels(string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var data = await _labelService.ApplyFilters(searchString, sortOrder);
        var paginatedLabels = _labelService.ApplyPagination(data, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);
        ViewBag.TotalItems = data.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, data.Count);

        return View(paginatedLabels);
    }

    public async Task<IActionResult> Records(string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var data = await _recordService.ApplyFilters(_recordService.GetAllRecords(), searchString, sortOrder, null);
        var paginatedRecords = _recordService.ApplyPagination(data, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);
        ViewBag.TotalItems = data.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, data.Count);

        return View(paginatedRecords);
    }

    public async Task<IActionResult> Orders(string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var orders = _orderService.GetAllOrders().AsQueryable();
        var data = await _orderService.ApplyFilters(orders, searchString, sortOrder);
        var paginatedOrders = _orderService.ApplyPagination(data, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);
        ViewBag.TotalItems = data.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, data.Count);
        
        return View(paginatedOrders);
    }

    [Route("Admin/Orders/{id}")]
    public async Task<IActionResult> OrdersProducts(int id, string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        PopulateViewData(searchString, sortOrder, pageIndex, pageSize);
        var order = await _orderService.GetOrderWithDependencies(id);
        var data = await _cartService.ApplyFilters(order.Products.AsQueryable(), searchString, sortOrder);
        var paginatedProducts = _cartService.ApplyPagination(data, pageIndex, pageSize);

        ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);
        ViewBag.TotalItems = data.Count();
        ViewBag.StartItem = (pageIndex - 1) * pageSize + 1;
        ViewBag.EndItem = Math.Min(pageIndex * pageSize, data.Count);
        
        return View(paginatedProducts);
    }

    // GET: Admin/Users/Delete/5
    [Route("Admin/Users/Delete/{id}")]
    public async Task<IActionResult> UsersDelete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
    
    // POST: Admin/Users/Delete/5
    [Route("Admin/Users/Delete/{id}")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UsersDeleteConfirmed(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public AdminIndexVM MakeIndexVM()
    {
        return new AdminIndexVM
        {
            TotalArtists = _context.Artists.Count(),
            TotalGenres = _context.Genres.Count(),
            TotalLabels = _context.Labels.Count(),
            TotalRecords = _context.Records.Count()
        };
    }

    public void PopulateViewData(string searchString, int? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["SortOrder"] = sortOrder;
        ViewData["PageIndex"] = pageIndex;
        ViewData["PageSize"] = pageSize;
    }

    public async Task<List<StoreUser>> ApplyFiltersToUsers(string currentFilter, int? sortOrder)
    {
        var users = _context.Users.AsQueryable();
        if (!String.IsNullOrEmpty(currentFilter))
        {
            users = users.Where(r => r.Email.ToUpper().Contains(currentFilter.ToUpper())
            || r.Name.ToUpper().Contains(currentFilter.ToUpper())
            || r.Roles.ToUpper().Contains(currentFilter.ToUpper()));
        }

        switch (sortOrder)
        {
            case 1:
                users = users.OrderBy(i => i.Email);
                break;
            case 2:
                users = users.OrderByDescending(i => i.Email);
                break;
            default:
                break;
        }

        return await users.ToListAsync();
    }

    public List<StoreUser> ApplyPaginationToUsers(List<StoreUser> users, int pageIndex, int pageSize)
    {
        return users.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
    }
}