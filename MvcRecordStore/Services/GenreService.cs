using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public class GenreService : IGenreService
{
    private readonly StoreDbContext _context;
    public GenreService(StoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all genres.
    /// </summary>
    /// <returns>A list of Genre objects.</returns>
    public List<Genre> GetAllGenres()
    {
        return _context.Genres
        .Include(g => g.Artists)
        .Include(g => g.Records)
        .ToList();
    }

    /// <summary>
    /// Retrieves a genre with its dependencies.
    /// </summary>
    /// <returns>A Genre object.</returns>
    public async Task<Genre> GetGenreWithDependencies(int id)
    {
        return await _context.Genres
            .Include(g => g.Artists)
            .Include(g => g.Records)
            .FirstOrDefaultAsync(g => g.ID == id);
    }

    /// <summary>
    /// Retrieves a genre without its dependencies.
    /// </summary>
    /// <returns>A Genre object.</returns>
    public async Task<Genre> GetGenreWithoutDependencies(int id)
    {
        return await _context.Genres.FirstOrDefaultAsync(g => g.ID == id);
    }

    public async Task<List<Genre>> ApplyFilters(string? currentFilter, int? sortOrder)
    {
        var genres = GetAllGenres().AsQueryable();
        if (!String.IsNullOrEmpty(currentFilter))
        {
            genres = genres.Where(r => r.Name.ToUpper().Contains(currentFilter.ToUpper()));
        }

        switch (sortOrder)
        {
            case 1:
                genres = genres.OrderBy(i => i.Name);
                break;
            case 2:
                genres = genres.OrderByDescending(i => i.Name);
                break;
            default:
                break;
        }

        return genres.ToList();
    }

    public List<Genre> ApplyPagination(List<Genre> genres, int pageIndex, int pageSize)
    {
        return genres.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
    }

    /// <summary>
    /// Creates a new genre.
    /// </summary>
    /// <returns>A new Genre object.</returns>
    public Genre CreateNewGenre(GenreCreateVM genreVM)
    {
        return new Genre
            {
                Name = genreVM.Name
            };
    }

    /// <summary>
    /// Makes genre view model for the edit action.
    /// </summary>
    /// <returns>A GenreCreateVM object.</returns>
    public GenreCreateVM GetGenreViewModelToEdit(Genre genre)
    {
        return new GenreCreateVM
            {
                ID = genre.ID,
                Name = genre.Name
            };
    }

    /// <summary>
    /// Updates a genre's properties.
    /// </summary>
    public void UpdateGenreProperties(Genre genre, GenreCreateVM genreVM)
    {
        genre.Name = genreVM.Name;
    }
    
    /// <summary>
    /// Checks whether a genre exists or not.
    /// </summary>
    /// <returns>True if it does, false if not.</returns>
    public bool GenreExists(int id)
    {
        return _context.Genres.Any(e => e.ID == id);
    }
}