using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<StoreUser> _userManager;

        public RecordsController(StoreDbContext context, UserManager<StoreUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .Include(r => r.Prices)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (record == null)
            {
                return NotFound();
            }

            var recordVM = new RecordDetailsVM()
            {
                ID = record.ID,
                Name = record.Name,
                Type = record.Type,
                ReleaseDate = record.ReleaseDate,
                Prices = record.Prices,
                Artist = record.Artist,
                ArtistID = record.ArtistID,
                Label = record.Label,
                LabelID = record.LabelID,
                Genres = record.Genres
            };

            return View(recordVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, [Bind("Input")] RecordDetailsVM recordVM)
        {
            var user = await _userManager.GetUserAsync(User);
            var record = await _context.Records.FirstOrDefaultAsync(m => m.ID == id);
            var selectedFormatPrice = JsonSerializer.Deserialize<Dictionary<string, double>>(recordVM.Input);
            var recordPrice = new RecordPrice(); // finding the RecordPrice object
            foreach (var item in selectedFormatPrice)
            {
                recordPrice = _context.RecordPrices
                .Include(r => r.Record)
                .FirstOrDefault(r => r.RecordID == id && r.Format == item.Key && r.Price == item.Value);
                Console.WriteLine($"FormatPrice Post = Key: {item.Key}, Value: {item.Value}");
            }

            if (record == null)
            {
                return NotFound();
            }

            if ((user == null) || (record == null))
            {
                return RedirectToAction(nameof(Index));
            }

            if (_context.CartItems.Any(c => c.ProductID == recordPrice.ID && c.Buyer == user))
            {
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Buyer == user); // Check for stock

                if ((cartItem.Quantity + 1) > recordPrice.Stock)
                {
                    return RedirectToAction("Details", new { id = record.ID });
                }
                else
                {
                    cartItem.Quantity++;
                }

                _context.Update(cartItem);
                await _context.SaveChangesAsync();
            }
            else if (recordPrice.Stock > 0)
            {
                var cart = new CartItem()
                {
                    Buyer = user,
                    BuyerID = user.Id,
                    Product = recordPrice,
                    ProductID = recordPrice.ID,
                    Quantity = 1
                };

                _context.Add(cart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = record.ID });
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
        public async Task<IActionResult> Create([Bind("ID,Name,Type,ReleaseDate,FormatPrices,ArtistID,LabelID,SelectedGenres")] RecordCreateVM recordVM)
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
                Console.WriteLine($"Formats: {recordVM.FormatPrices}");
                var record = new Record
                {
                    Name = recordVM.Name,
                    ReleaseDate = recordVM.ReleaseDate,
                    Type = (Models.RecordType)recordVM.Type,
                    Artist = artist,
                    ArtistID = artist.ID,
                    Label = label,
                    LabelID = label.ID,
                    Genres = genres
                };
                _context.Add(record);

                if ((recordVM != null) && (recordVM.FormatPrices != null))
                {
                    foreach (var formatPrice in recordVM.FormatPrices)
                    {
                        var format = new RecordPrice
                        {
                            Format = formatPrice.Format,
                            Price = (double)formatPrice.Price,
                            Stock = (int)formatPrice.Stock,
                            Record = record,
                            RecordID = record.ID
                        };
                        _context.Add(format);
                        record.Prices.Add(format);
                    }
                }
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
            foreach (var genre in record.Genres)
            {
                selectedGenres.Add(genre.ID);
            }

            var formatPrices = await _context.RecordPrices.Where(p => p.RecordID == id).ToListAsync();

            var recordVM = new RecordCreateVM
            {
                ID = record.ID,
                Name = record.Name,
                Type = (int)record.Type,
                ReleaseDate = record.ReleaseDate,
                ArtistID = record.ArtistID,
                LabelID = record.Label.ID,
                SelectedGenres = selectedGenres,
                FormatPrices = new List<FormatPriceVM>()
            };

            foreach (var item in formatPrices)
            {
                var formatPricesVM = new FormatPriceVM();
                formatPricesVM.Format = item.Format;
                formatPricesVM.Price = item.Price;
                formatPricesVM.Stock = item.Stock;
                recordVM.FormatPrices.Add(formatPricesVM);
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Type,ReleaseDate,FormatPrices,ArtistID,LabelID,SelectedGenres")] RecordCreateVM recordVM)
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
                .Include(r => r.Prices)
                .Include(r => r.Genres)
                .FirstOrDefaultAsync(r => r.ID == id);

                record.Name = recordVM.Name;
                record.Type = (Models.RecordType)recordVM.Type;
                record.ReleaseDate = recordVM.ReleaseDate;
                record.Artist = artist;
                record.ArtistID = artist.ID;
                record.Label = label;
                record.LabelID = label.ID;
                record.Genres = genres;

                if ((recordVM != null) && (recordVM.FormatPrices != null))
                {
                    if (record.Prices != null)
                    {
                        foreach (var price in record.Prices)
                        {
                            _context.Remove(price);
                        }
                    }

                    foreach (var formatPrice in recordVM.FormatPrices)
                    {
                        if ((formatPrice.Format != null) && (formatPrice.Price != null) && (formatPrice.Stock != null))
                        {
                            var format = new RecordPrice
                            {
                                Format = formatPrice.Format,
                                Price = (double)formatPrice.Price,
                                Stock = (int)formatPrice.Stock,
                                Record = record,
                                RecordID = record.ID
                            };
                            _context.Add(format);
                            record.Prices.Add(format);
                        }
                    }
                }

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
