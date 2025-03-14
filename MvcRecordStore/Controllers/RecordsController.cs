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

namespace MvcRecordStore.Controllers
{
    public class RecordsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly IRecordService _recordService;
        private readonly UserManager<StoreUser> _userManager;

        public RecordsController(StoreDbContext context, IRecordService recordService, UserManager<StoreUser> userManager)
        {
            _context = context;
            _recordService = recordService;
            _userManager = userManager;
        }

        // GET: Records
        public async Task<IActionResult> Index()
        {
            var storeDbContext = _recordService.GetAllRecords();
            return View(await storeDbContext.ToListAsync());
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
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, [Bind("Input")] RecordDetailsVM recordVM)
        {
            var user = await _userManager.GetUserAsync(User);
            var record = await _recordService.GetRecordWithoutDependencies(id);
            if (record == null)
            {
                return NotFound();
            }

            var recordPrice = await _recordService.GetSelectedFormat(recordVM, id);
            if (await _recordService.AddRecordToCart(recordPrice, user)) // Redirects to shopping cart in case of success
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
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Records/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Type,ReleaseDate,FormatPrices,ArtistID,LabelID,SelectedGenres")] RecordCreateVM recordVM)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewData();
                return View(recordVM);
            }

            var record = await _recordService.CreateNewRecord(recordVM);
            _context.Add(record);

            _recordService.UpdateRecordPrices(record, recordVM);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Records/Edit/5
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

            PopulateViewData();
            return View(recordVM);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            return RedirectToAction(nameof(Index));
        }

        // GET: Records/Delete/5
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
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Populates the view data for dropdown lists.
        /// </summary>
        public void PopulateViewData()
        {
            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
        }
    }
}
