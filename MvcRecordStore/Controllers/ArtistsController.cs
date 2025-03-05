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

namespace MvcRecordStore.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly StoreDbContext _context;

        public ArtistsController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Artists
        public async Task<IActionResult> Index()
        {
            var storeDbContext = _context.Artists.Include(a => a.Label);
            return View(await storeDbContext.ToListAsync());
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .Include(a => a.Label)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: Artists/Create
        public IActionResult Create()
        {
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();

            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Country,LabelID,SelectedGenres")] ArtistCreateVM artistVM)
        {
            var genres = _context.Genres
            .Where(g => artistVM.SelectedGenres.Contains(g.ID))
            .ToList();

            var label = await _context.Labels.FirstOrDefaultAsync(l => l.ID == artistVM.LabelID);

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var artist = new Artist
                {
                    Name = artistVM.Name,
                    Country = artistVM.Country,
                    Label = label,
                    LabelID = label.ID,
                    Genres = genres
                };
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View(artistVM);
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
            .Include(a => a.Genres)
            .Include(a => a.Label)
            .FirstOrDefaultAsync(a => a.ID == id);
            if (artist == null)
            {
                return NotFound();
            }

            var selectedGenres = new List<int>();
            foreach(var genre in artist.Genres)
            {
                selectedGenres.Add(genre.ID);
            }

            var artistVM = new ArtistCreateVM
            {
                ID = artist.ID,
                Name = artist.Name,
                Country = artist.Country,
                LabelID = artist.LabelID,
                SelectedGenres = selectedGenres
            };

            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View(artistVM);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Country,LabelID,SelectedGenres")] ArtistCreateVM artistVM)
        {
            var genres = _context.Genres
            .Where(g => artistVM.SelectedGenres.Contains(g.ID))
            .ToList();

            var label = await _context.Labels.FirstOrDefaultAsync(l => l.ID == artistVM.LabelID);

            if (id != artistVM.ID)
            {
                return NotFound();
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var artist = await _context.Artists
                .Include(a => a.Genres)
                .Include(a => a.Label)
                .FirstOrDefaultAsync(a => a.ID == id);

                artist.Name = artistVM.Name;
                artist.Country = artistVM.Country;
                artist.Label = label;
                artist.LabelID = label.ID;
                artist.Genres = genres;

                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.ID))
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
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return RedirectToAction("Edit", new { id = artistVM.ID });
        }

        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .Include(a => a.Label)
                .FirstOrDefaultAsync(m => m.ID == id);
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
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.ID == id);
        }
    }
}
