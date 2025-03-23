using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public class RecordService : IRecordService
{
    private readonly StoreDbContext _context;

    public RecordService(StoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all records.
    /// </summary>
    /// <returns></returns>
    public IQueryable<Record> GetAllRecords()
    {
        return _context.Records
            .Include(r => r.Artist)
            .Include(r => r.Label)
            .Include(r => r.Genres)
            .Include(r => r.Prices);
    }

    /// <summary>
    /// Retrieves a specified number of records and maps them to the HomeVM view model.
    /// </summary>
    /// <param name="amount">The number of records to retrieve.</param>
    /// <returns>An IQueryable of HomeVM objects.</returns>
    public IQueryable<HomeVM> GetHomePageViewModel(int amount)
    {
        var recordsVM = new List<HomeVM>();
        foreach (var record in GetAllRecords().Take(amount))
        {
            var recordVM = new HomeVM()
            {
                ID = record.ID,
                Name = record.Name,
                ArtistName = record.Artist.Name,
                LabelName = record.Label.Name,
                Prices = new List<RecordPrice>()
            };
            
            foreach (var price in record.Prices)
            {
                if (price.Stock > 0)
                {
                    recordVM.Prices.Add(price);
                }
            }
            recordsVM.Add(recordVM);
        }

        return recordsVM.AsQueryable();
    }

    /// <summary>
    /// Retrieves a record with its dependencies.
    /// </summary>
    /// <param name="id">Record ID</param>
    /// <returns>Record object.</returns>
    public async Task<Record> GetRecordWithDependencies(int id)
    {
        return await _context.Records
            .Include(r => r.Artist)
            .Include(r => r.Label)
            .Include(r => r.Prices)
            .Include(r => r.Genres)
            .FirstOrDefaultAsync(r => r.ID == id);
    }

    /// <summary>
    /// Retrieves a record without its dependencies.
    /// </summary>
    /// <param name="id">Record ID</param>
    /// <returns>Record object.</returns>
    public async Task<Record> GetRecordWithoutDependencies(int id)
    {
        return await _context.Records.FirstOrDefaultAsync(r => r.ID == id);
    }

    /// <summary>
    /// Retrieves the artist without its dependencies.
    /// </summary>
    /// <returns>Artist object.</returns>
    public async Task<Artist> GetSelectedArtist(RecordCreateVM recordVM)
    {
        return await _context.Artists.FirstOrDefaultAsync(a => a.ID == recordVM.ArtistID);
    }

    /// <summary>
    /// Retrieves a list of the record's genres.
    /// </summary>
    /// <returns>A list of Genres.</returns>
    public List<Genre> GetRecordSelectedGenres(RecordCreateVM recordVM)
    {
        return _context.Genres
            .Where(g => recordVM.SelectedGenres.Contains(g.ID))
            .ToList();
    }

    /// <summary>
    /// Retrieves the record's label.
    /// </summary>
    /// <returns>A Label object.</returns>
    public async Task<Label> GetRecordSelectedLabel(RecordCreateVM recordVM)
    {
        return await _context.Labels.FirstOrDefaultAsync(l => l.ID == recordVM.LabelID);
    }

    /// <summary>
    /// Get a list of the record's GenreIDs.
    /// </summary>
    /// <returns>A list of GenreIDs.</returns>
    public List<int> GetRecordGenreIDs(Record record)
    {
        var selectedGenres = new List<int>();
        foreach (var genre in record.Genres)
        {
            selectedGenres.Add(genre.ID);
        }
        return selectedGenres;
    }

    /// <summary>
    /// Retrieves the prices of a record.
    /// </summary>
    /// <returns>A list of RecordPrice objects.</returns>
    public async Task<List<RecordPrice>> GetRecordPrices(int recordID)
    {
        return await _context.RecordPrices.Where(p => p.RecordID == recordID).ToListAsync();
    }

    /// <summary>
    /// Retrieves the details view model.
    /// </summary>
    /// <param name="record">Record object</param>
    /// <param name="id">Record ID</param>
    /// <returns>RecordDetailsVM object.</returns>
    public async Task<RecordDetailsVM> GetRecordViewModelDetails(Record record, int id)
    {
        return new RecordDetailsVM()
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
    }

    public async Task<List<Record>> ApplyFilters(IQueryable<Record> records, string? currentFilter, int? sortOrder, int? genreFilter)
    {
        if (!String.IsNullOrEmpty(currentFilter))
        {
            records = records.Where(r => r.Name.ToUpper().Contains(currentFilter.ToUpper())
                || r.Artist.Name.ToUpper().Contains(currentFilter)
                || r.Label.Name.ToUpper().Contains(currentFilter.ToUpper()));
        }

        switch (sortOrder)
        {
            case 1:
                records = records.OrderBy(i => i.Name);
                break;
            case 2:
                records = records.OrderByDescending(i => i.Name);
                break;
            case 3:
                records = records.OrderBy(i => i.ReleaseDate);
                break;
            case 4:
                records = records.OrderByDescending(i => i.ReleaseDate);
                break;
            default:
                break;
        }

        if (genreFilter != null)
        {   
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.ID == genreFilter);
            if (genre != null)
            {
                records = records.Where(i => i.Genres.Contains(genre));
            }
        }
        return records.ToList();
    }

    public List<Record> ApplyPagination(List<Record> records, int pageIndex, int pageSize)
    {
        return records.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
    }

    /// <summary>
    /// Creates a new record from the view model.
    /// </summary>
    /// <param name="recordVM">RecordCreateVM object</param>
    /// <returns>Returns the created record.</returns>
    public async Task<Record> CreateNewRecord(RecordCreateVM recordVM)
    {
        return new Record
        {
            Name = recordVM.Name,
            ReleaseDate = recordVM.ReleaseDate,
            Type = (Models.RecordType)recordVM.Type,
            ArtistID = (await GetSelectedArtist(recordVM)).ID,
            LabelID = (await GetRecordSelectedLabel(recordVM)).ID,
            Genres = GetRecordSelectedGenres(recordVM)
        };
    }

    /// <summary>
    /// Retrieves the edit view model for a record.
    /// </summary>
    /// <param name="record">Record object</param>
    /// <param name="id">Record ID</param>
    /// <returns>Returns a RecordCreateVM object.</returns>
    public async Task<RecordCreateVM> GetRecordViewModelToEdit(Record record, int id)
    {
        var recordVM = new RecordCreateVM
        {
            ID = record.ID,
            Name = record.Name,
            Type = (int)record.Type,
            ReleaseDate = record.ReleaseDate,
            ArtistID = record.ArtistID,
            LabelID = record.Label.ID,
            SelectedGenres = GetRecordGenreIDs(record),
            FormatPrices = new List<FormatPriceVM>()
        };

        var formatPrices = await GetRecordPrices(id);
        foreach (var item in formatPrices)
        {
            var formatPricesVM = new FormatPriceVM();
            formatPricesVM.Format = item.Format;
            formatPricesVM.Price = item.Price;
            formatPricesVM.Stock = item.Stock;
            recordVM.FormatPrices.Add(formatPricesVM);
        }

        return recordVM;
    }

    /// <summary>
    /// Updates the properties of a record.
    /// </summary>
    /// <param name="record">Record object</param>
    /// <param name="recordVM">RecordCreateVM object</param>
    public void UpdateRecordProperties(Record record, RecordCreateVM recordVM)
    {
        record.Name = recordVM.Name;
        record.Type = (Models.RecordType)recordVM.Type;
        record.ArtistID = recordVM.ArtistID;
        record.LabelID = recordVM.LabelID;
        record.Genres = _context.Genres
            .Where(g => recordVM.SelectedGenres.Contains(g.ID))
            .ToList();
    }

    /// <summary>
    /// Updates the prices of a record.
    /// </summary>
    /// <param name="record">Record object</param>
    /// <param name="recordVM">RecordCreateVM object</param>
    public void UpdateRecordPrices(Record record, RecordCreateVM recordVM)
    {
        if (recordVM != null)
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
    }

    /// <summary>
    /// Retrieves the selected format price for a record.
    /// </summary>
    /// <param name="recordVM">RecordDetailsVM object</param>
    /// <param name="id">Record ID</param>
    /// <returns>RecordPrice object.</returns>
    public async Task<RecordPrice> GetSelectedFormat(RecordDetailsVM recordVM, int id)
    {
        var selectedFormatPrice = JsonSerializer.Deserialize<Dictionary<string, double>>(recordVM.Input);
        var recordPrice = new RecordPrice(); // finding the RecordPrice object
        foreach (var item in selectedFormatPrice)
        {
            recordPrice = await _context.RecordPrices
            .Include(r => r.Record)
            .FirstOrDefaultAsync(r => r.RecordID == id && r.Format == item.Key && r.Price == item.Value);
        }

        return recordPrice;
    }

    /// <summary>
    /// Retrieves the specified product from a user's shopping cart.
    /// </summary>
    /// <returns>CartItem object.</returns>
    public async Task<CartItem> GetCartItem(int productID, StoreUser user)
    {
        return await _context.CartItems.FirstOrDefaultAsync(c => c.Buyer == user && c.ProductID == productID && c.PaidFor == false);
    }

    /// <summary>
    /// Checks whether a product is in stock or not.
    /// </summary>
    /// <returns>True if it is, false if not.</returns>
    public bool IsProductInStock(CartItem? cartItem, RecordPrice recordPrice, int quantity)
    {
        if (cartItem != null)
        {
            if ((cartItem.Quantity + quantity) > recordPrice.Stock)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return recordPrice.Stock >= quantity;
        }
    }

    /// <summary>
    /// Adds the selected record format to the shopping cart.
    /// </summary>
    /// <param name="recordPrice">RecordPrice object</param>
    /// <param name="user">Current User</param>
    /// <returns>True if the task is successful, false if not.</returns>
    public async Task<bool> AddRecordToCart(RecordPrice recordPrice, StoreUser user, int quantity)
    {
        var cartItem = await GetCartItem(recordPrice.ID, user);

        if (cartItem != null)
        {
            if (IsProductInStock(cartItem, recordPrice, quantity))
            {
                cartItem.Quantity += quantity;
                _context.Update(cartItem);
            }
            else
            {
                return false; // Not enough stock
            }
        }
        else if (IsProductInStock(null, recordPrice, quantity))
        {
            var cart = new CartItem()
            {
                Buyer = user,
                BuyerID = user.Id,
                Product = recordPrice,
                ProductID = recordPrice.ID,
                Quantity = quantity
            };

            _context.Add(cart);
        }
        else
        {
            return false; // Not enough stock
        }
        
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Checks if a record exists.
    /// </summary>
    /// <param name="id">Record ID</param>
    /// <returns>True if it does, false if not.</returns>
    public bool RecordExists(int id)
    {
        return _context.Records.Any(e => e.ID == id);
    }
}