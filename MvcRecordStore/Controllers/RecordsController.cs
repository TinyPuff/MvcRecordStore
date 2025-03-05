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
    public class RecordsController : Controller
    {
        private readonly StoreDbContext _context;

        public RecordsController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Records
        public async Task<IActionResult> Index()
        {
            var storeDbContext = _context.Records.Include(r => r.Artist).Include(r => r.Label);
            return View(await storeDbContext.ToListAsync());
        }

        // GET: Records/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Label)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        // GET: Records/Create
        public IActionResult Create()
        {
            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View();
        }

        // POST: Records/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Type,ReleaseDate,ArtistID,LabelID,SelectedGenres")] RecordCreateVM recordVM)
        {
            var genres = _context.Genres
            .Where(g => recordVM.SelectedGenres.Contains(g.ID))
            .ToList();

            var label = await _context.Labels.FirstOrDefaultAsync(l => l.ID == recordVM.LabelID);

            var artist = await _context.Artists.FirstOrDefaultAsync(l => l.ID == recordVM.ArtistID);

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var record = new Record
                {
                    Name = recordVM.Name,
                    ReleaseDate = recordVM.ReleaseDate,
                    Type = (RecordType)recordVM.Type,
                    Artist = artist,
                    ArtistID = artist.ID,
                    Label = label,
                    LabelID = label.ID,
                    Genres = genres
                };
                _context.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View(recordVM);
        }

        // GET: Records/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.Records
            .Include(r => r.Artist)
            .Include(r => r.Label)
            .Include(r => r.Genres)
            .FirstOrDefaultAsync(r => r.ID == id);
            if (record == null)
            {
                return NotFound();
            }

            var selectedGenres = new List<int>();
            foreach(var genre in record.Genres)
            {
                selectedGenres.Add(genre.ID);
            }

            var recordVM = new RecordCreateVM
            {
                ID = record.ID,
                Name = record.Name,
                Type = (int)record.Type,
                ReleaseDate = record.ReleaseDate,
                ArtistID = record.ArtistID,
                LabelID = record.Label.ID,
                SelectedGenres = selectedGenres
            };

            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View(recordVM);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Type,ReleaseDate,ArtistID,LabelID,SelectedGenres")] RecordCreateVM recordVM)
        {
            if (id != recordVM.ID)
            {
                return NotFound();
            }

            var genres = _context.Genres
            .Where(g => recordVM.SelectedGenres.Contains(g.ID))
            .ToList();

            var label = await _context.Labels.FirstOrDefaultAsync(l => l.ID == recordVM.LabelID);

            var artist = await _context.Artists.FirstOrDefaultAsync(l => l.ID == recordVM.ArtistID);

            if (ModelState.IsValid)
            {
                var record = await _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Label)
                .Include(r => r.Genres)
                .FirstOrDefaultAsync(r => r.ID == id);

                record.Name = recordVM.Name;
                record.Type = (RecordType)recordVM.Type;
                record.ReleaseDate = recordVM.ReleaseDate;
                record.Artist = artist;
                record.ArtistID = artist.ID;
                record.Label = label;
                record.LabelID = label.ID;
                record.Genres = genres;

                try
                {
                    _context.Update(record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordExists(record.ID))
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
            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Labels = _context.Labels.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return RedirectToAction("Edit", new { id = recordVM.ID });
        }

        // GET: Records/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Label)
                .FirstOrDefaultAsync(m => m.ID == id);
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
            var record = await _context.Records.FindAsync(id);
            if (record != null)
            {
                _context.Records.Remove(record);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordExists(int id)
        {
            return _context.Records.Any(e => e.ID == id);
        }
    }
}
