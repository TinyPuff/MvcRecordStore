using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Services;

namespace MvcRecordStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GenresController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly IGenreService _genreService;

        public GenresController(StoreDbContext context, IGenreService genreService)
        {
            _context = context;
            _genreService = genreService;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(_genreService.GetAllGenres());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _genreService.GetGenreWithDependencies((int)id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Genres/Create
        [Route("Admin/Genres/Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Admin/Genres/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] GenreCreateVM genreVM)
        {
            if (!ModelState.IsValid)
            {
                return View(genreVM);
            }

            var genre = _genreService.CreateNewGenre(genreVM);
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction("Genres", "Admin");
        }

        // GET: Genres/Edit/5
        [Route("Admin/Genres/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _genreService.GetGenreWithDependencies((int)id);
            if (genre == null)
            {
                return NotFound();
            }

            var genreVM = _genreService.GetGenreViewModelToEdit(genre);
            return View(genreVM);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Admin/Genres/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] GenreCreateVM genreVM)
        {
            if (id != genreVM.ID)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = genreVM.ID });;
            }

            var genre = await _genreService.GetGenreWithoutDependencies(id);
            _genreService.UpdateGenreProperties(genre, genreVM);

            try
            {
                _context.Update(genre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_genreService.GenreExists(genre.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Genres", "Admin");
        }

        // GET: Genres/Delete/5
        [Route("Admin/Genres/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _genreService.GetGenreWithoutDependencies((int)id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [Route("Admin/Genres/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _genreService.GetGenreWithoutDependencies(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Genres", "Admin");
        }
    }
}
