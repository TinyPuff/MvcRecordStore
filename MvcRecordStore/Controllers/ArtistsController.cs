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

namespace MvcRecordStore.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly IArtistService _artistService;
        private readonly IRecordService _recordService;
        private readonly IConfiguration Configuration;

        public ArtistsController(StoreDbContext context, IArtistService artistService, IRecordService recordService, IConfiguration configuration)
        {
            _context = context;
            _artistService = artistService;
            _recordService = recordService;
            Configuration = configuration;
        }

        // GET: Artists
        public async Task<IActionResult> Index(string currentFilter, int sortOrder, int genreFilter, int pageIndex)
        {
            pageIndex = pageIndex >= 1 ? pageIndex : 1;
            PopulateIndexViewData(currentFilter, sortOrder, genreFilter, pageIndex);
            ViewBag.Genres = _context.Genres.ToList();
            var pageSize = Configuration.GetValue("PageSize", 3);

            var data = await _artistService.ApplyFilters(currentFilter, sortOrder, genreFilter);
            ViewBag.TotalPages = Math.Ceiling(data.Count() / (double)pageSize);

            return View(_artistService.ApplyPagination(data, pageIndex, pageSize));
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id, string currentFilter, int sortOrder, int genreFilter, int pageIndex)
        {
            pageIndex = pageIndex >= 1 ? pageIndex : 1;
            PopulateIndexViewData(currentFilter, sortOrder, genreFilter, pageIndex);
            ViewBag.Genres = _context.Genres.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _artistService.GetArtistWithDependencies((int)id);
            var artistRecords = await _recordService.ApplyFilters(artist.Records.AsQueryable(), currentFilter, sortOrder, genreFilter);
            if (artist == null)
            {
                return NotFound();
            }
            var pageSize = Configuration.GetValue("PageSize", 3);

            var artistVM = new ArtistDetailsVM
            {
                Artist = artist,
                Records = _recordService.ApplyPagination(artistRecords, pageIndex, pageSize)
            };
            ViewBag.TotalPages = Math.Ceiling(artistRecords.Count() / (double)pageSize);

            return View(artistVM);
        }

        // GET: Artists/Create
        [Authorize(Roles = "Admin")]
        [Route("Admin/Artists/Create")]
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [Route("Admin/Artists/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Country,LabelID,SelectedGenres")] ArtistCreateVM artistVM)
        {
            if (ModelState.IsValid)
            {
                var artist = await _artistService.CreateNewArtist(artistVM);
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction("Artists", "Admin");
            }

            PopulateViewData();
            return View(artistVM);
        }

        // GET: Artists/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/Artists/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _artistService.GetArtistWithDependencies((int)id);
            if (artist == null)
            {
                return NotFound();
            }
            var artistVM = _artistService.GetArtistViewModelToEdit(artist);

            PopulateViewData();
            return View(artistVM);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [Route("Admin/Artists/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Country,LabelID,SelectedGenres")] ArtistCreateVM artistVM)
        {
            if (id != artistVM.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateViewData();
                return RedirectToAction("Edit", new { id = artistVM.ID });
            }

            var artist = await _artistService.GetArtistWithDependencies(id);
            _artistService.UpdateArtistProperties(artist, artistVM);
            try
            {
                _context.Update(artist);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_artistService.ArtistExists(artist.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Artists", "Admin");

        }

        // GET: Artists/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/Artists/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _artistService.GetArtistWithDependencies((int)id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/Artists/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _artistService.GetArtistWithoutDependencies(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Artists", "Admin");
        }

        public void PopulateIndexViewData(string? currentFilter, int? sortOrder, int? genreFilter, int? pageIndex)
        {
            ViewData["PageIndex"] = pageIndex;
            ViewData["CurrentFilter"] = currentFilter;
            ViewData["SortOrder"] = sortOrder;
            ViewData["GenreFilter"] = genreFilter;
        }

        public void PopulateViewData()
        {
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
        }
    }
}
