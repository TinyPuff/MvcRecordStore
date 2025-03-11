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

namespace MvcRecordStore.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly IArtistService _artistService;

        public ArtistsController(StoreDbContext context, IArtistService artistService)
        {
            _context = context;
            _artistService = artistService;
        }

        // GET: Artists
        public async Task<IActionResult> Index()
        {
            var storeDbContext = _artistService.GetAllArtists();
            return View(await storeDbContext.ToListAsync());
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Artists/Create
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Country,LabelID,SelectedGenres")] ArtistCreateVM artistVM)
        {
            if (ModelState.IsValid)
            {
                var artist = _artistService.CreateNewArtist(artistVM);
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateViewData();
            return View(artistVM);
        }

        // GET: Artists/Edit/5
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
            return RedirectToAction(nameof(Index));
        }

        // GET: Artists/Delete/5
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
            return RedirectToAction(nameof(Index));
        }

        public void PopulateViewData()
        {
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
        }
    }
}
