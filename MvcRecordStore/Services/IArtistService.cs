using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public interface IArtistService
{
    IQueryable<Artist> GetAllArtists();
    Task<Artist> GetArtistWithDependencies(int id);
    Task<Artist> GetArtistWithoutDependencies(int id);
    List<Genre> GetArtistSelectedGenres(ArtistCreateVM artistVM);
    Task<Label> GetArtistSelectedLabel(ArtistCreateVM artistVM);
    List<int> GetArtistGenreIDs(Artist artist);
    void UpdateArtistProperties(Artist artist, ArtistCreateVM artistVM);
    Task<Artist> CreateNewArtist(ArtistCreateVM artistVM);
    ArtistCreateVM GetArtistViewModelToEdit(Artist artist);
    Task<List<Artist>> ApplyFilters(string? currentFilter, int? sortOrder, int? genreFilter);
    List<Artist> ApplyPagination(List<Artist> artists, int pageIndex, int pageSize);
    bool ArtistExists(int id);
}