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
    bool ArtistExists(int id);
}