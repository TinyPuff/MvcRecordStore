using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public class ArtistService : IArtistService
{
    private readonly StoreDbContext _context;

    public ArtistService(StoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all artists.
    /// </summary>
    public IQueryable<Artist> GetAllArtists()
    {
        return _context.Artists.Include(a => a.Label);
    }

    /// <summary>
    /// Retrieves the artist with its dependencies.
    /// </summary>
    /// <returns>Artist object.</returns>
    public async Task<Artist> GetArtistWithDependencies(int id)
    {
        return await _context.Artists
            .Include(a => a.Label)
            .Include(a => a.Genres)
            .Include(a => a.Records)
            .FirstOrDefaultAsync(a => a.ID == id);
    }

    /// <summary>
    /// Retrieves the artist without its dependencies.
    /// </summary>
    /// <returns>Artist object.</returns>
    public async Task<Artist> GetArtistWithoutDependencies(int id)
    {
        return await _context.Artists.FirstOrDefaultAsync(a => a.ID == id);
    }

    /// <summary>
    /// Retrieves a list of the artist's genres.
    /// </summary>
    /// <returns>A list of Genres.</returns>
    public List<Genre> GetArtistSelectedGenres(ArtistCreateVM artistVM)
    {
        return _context.Genres
            .Where(g => artistVM.SelectedGenres.Contains(g.ID))
            .ToList();
    }

    /// <summary>
    /// Retrieves the artist's label.
    /// </summary>
    /// <returns>A Label object.</returns>
    public async Task<Label> GetArtistSelectedLabel(ArtistCreateVM artistVM)
    {
        return await _context.Labels.FirstOrDefaultAsync(l => l.ID == artistVM.LabelID);
    }

    /// <summary>
    /// Get a list of the artist's GenreIDs.
    /// </summary>
    /// <returns>A list of GenreIDs.</returns>
    public List<int> GetArtistGenreIDs(Artist artist)
    {
        var selectedGenres = new List<int>();
        foreach (var genre in artist.Genres)
        {
            selectedGenres.Add(genre.ID);
        }
        return selectedGenres;
    }

    public async void UpdateArtistProperties(Artist artist, ArtistCreateVM artistVM)
    {
        artist.Name = artistVM.Name;
        artist.Country = artistVM.Country;
        artist.LabelID = (await GetArtistSelectedLabel(artistVM)).ID;
        artist.Genres = GetArtistSelectedGenres(artistVM);
    }

    /// <summary>
    /// Creates a new artist.
    /// </summary>
    /// <returns>Artist object.</returns>
    public async Task<Artist> CreateNewArtist(ArtistCreateVM artistVM)
    {
        return new Artist
        {
            Name = artistVM.Name,
            Country = artistVM.Country,
            LabelID = (await GetArtistSelectedLabel(artistVM)).ID,
            Genres = GetArtistSelectedGenres(artistVM)
        };
    }

    /// <summary>
    /// Retrieves the edit view model for a record.
    /// </summary>
    /// <returns>An ArtistCreateVM object.</returns>
    public ArtistCreateVM GetArtistViewModelToEdit(Artist artist)
    {
        return new ArtistCreateVM
        {
            ID = artist.ID,
            Name = artist.Name,
            Country = artist.Country,
            LabelID = artist.LabelID,
            SelectedGenres = GetArtistGenreIDs(artist)
        };
    }
    
    /// <summary>
    /// Checks whether the artist exists or not.
    /// </summary>
    /// <returns>True if it does, false if not.</returns>
    public bool ArtistExists(int id)
    {
        return _context.Artists.Any(e => e.ID == id);
    }
}