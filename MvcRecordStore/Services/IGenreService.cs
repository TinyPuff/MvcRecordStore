using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public interface IGenreService
{
    List<Genre> GetAllGenres();
    Task<Genre> GetGenreWithDependencies(int id);
    Task<Genre> GetGenreWithoutDependencies(int id);
    Task<List<Genre>> ApplyFilters(string? currentFilter, int? sortOrder);
    List<Genre> ApplyPagination(List<Genre> genres, int pageIndex, int pageSize);
    Genre CreateNewGenre(GenreCreateVM genreVM);
    GenreCreateVM GetGenreViewModelToEdit(Genre genre);
    void UpdateGenreProperties(Genre genre, GenreCreateVM genreVM);
    bool GenreExists(int id);
}