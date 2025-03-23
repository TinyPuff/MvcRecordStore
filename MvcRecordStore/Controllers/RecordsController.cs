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
using Microsoft.Extensions.Configuration;

namespace MvcRecordStore.Controllers
{
    public class RecordsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly IRecordService _recordService;
        private readonly UserManager<StoreUser> _userManager;
        private readonly IConfiguration Configuration;

        public RecordsController(StoreDbContext context, IRecordService recordService, UserManager<StoreUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _recordService = recordService;
            _userManager = userManager;
            Configuration = configuration;
        }

        // GET: Records
        public async Task<IActionResult> Index(string currentFilter, int sortOrder, int genreFilter, int pageIndex)
        {
            pageIndex = pageIndex >= 1 ? pageIndex : 1;
            PopulateIndexViewData(currentFilter, sortOrder, genreFilter, pageIndex);
            var pageSize = Configuration.GetValue("PageSize", 3);

            var data = await _recordService.ApplyFilters(_recordService.GetAllRecords(), currentFilter, sortOrder, genreFilter);
            ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);

            return View(_recordService.ApplyPagination(data, pageIndex, pageSize));
        }

        // GET: Records/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _recordService.GetRecordWithDependencies((int)id);
            if (record == null)
            {
                return NotFound();
            }

            var recordVM = await _recordService.GetRecordViewModelDetails(record, (int)id);
            return View(recordVM);
        }

        // POST: Records/Details/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, [Bind("Input,Quantity")] RecordDetailsVM recordVM)
        {
            var user = await _userManager.GetUserAsync(User);
            var record = await _recordService.GetRecordWithoutDependencies(id);
            if (record == null)
            {
                return NotFound();
            }

            var recordPrice = await _recordService.GetSelectedFormat(recordVM, id);
            if (await _recordService.AddRecordToCart(recordPrice, user, recordVM.Quantity)) // Redirects to shopping cart in case of success
            {
                return RedirectToRoute(new
                {
                    controller = "Cart",
                    action = "Index"
                });
            }
            else
            {
                return RedirectToAction("Details", new { id = record.ID });
            }
        }

        // GET: Records/Create
        [Authorize(Roles = "Admin")]
        [Route("Admin/Records/Create")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Records/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [Route("Admin/Records/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Type,ReleaseDate,FormatPrices,ArtistID,LabelID,SelectedGenres")] RecordCreateVM recordVM)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(recordVM);
            }

            var record = await _recordService.CreateNewRecord(recordVM);
            _context.Add(record);

            _recordService.UpdateRecordPrices(record, recordVM);
            await _context.SaveChangesAsync();

            return RedirectToAction("Records", "Admin");
        }
        
        // GET: Records/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/Records/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _recordService.GetRecordWithDependencies((int)id);
            if (record == null)
            {
                return NotFound();
            }

            var recordVM = await _recordService.GetRecordViewModelToEdit(record, (int)id);

            PopulateDropdowns();
            return View(recordVM);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [Route("Admin/Records/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Type,ReleaseDate,FormatPrices,ArtistID,LabelID,SelectedGenres")] RecordCreateVM recordVM)
        {
            if (id != recordVM.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = recordVM.ID });
            }

            var record = await _recordService.GetRecordWithDependencies(id);
            if (record == null)
            {
                return NotFound();
            }

            _recordService.UpdateRecordProperties(record, recordVM);

            if (recordVM.FormatPrices != null)
            {
                _recordService.UpdateRecordPrices(record, recordVM);
            }

            try
            {
                _context.Update(record);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_recordService.RecordExists(record.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Records", "Admin");
        }

        // GET: Records/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/Records/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _recordService.GetRecordWithDependencies((int)id);
            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        // POST: Records/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/Records/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _recordService.GetRecordWithoutDependencies(id);
            if (record != null)
            {
                _context.Records.Remove(record);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Records", "Admin");
        }

        public void PopulateIndexViewData(string? currentFilter, int? sortOrder, int? genreFilter, int? pageIndex)
        {
            ViewData["PageIndex"] = pageIndex;
            ViewData["CurrentFilter"] = currentFilter;
            ViewData["SortOrder"] = sortOrder;
            ViewData["GenreFilter"] = genreFilter;
            ViewBag.Genres = _context.Genres.ToList();
        }

        /// <summary>
        /// Populates the view data for dropdown lists.
        /// </summary>
        public void PopulateDropdowns()
        {
            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
        }
    }
}
