using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public interface IRecordService
{
    IQueryable<Record> GetAllRecords();
    IQueryable<HomeVM> GetHomePageViewModel(int amount);
    Task<Record> GetRecordWithDependencies(int id);
    Task<Record> GetRecordWithoutDependencies(int id);
    Task<Artist> GetSelectedArtist(RecordCreateVM recordVM);
    List<Genre> GetRecordSelectedGenres(RecordCreateVM recordVM);
    Task<Label> GetRecordSelectedLabel(RecordCreateVM recordVM);
    List<int> GetRecordGenreIDs(Record record);
    Task<List<RecordPrice>> GetRecordPrices(int recordID);
    Task<RecordDetailsVM> GetRecordViewModelDetails(Record record, int id);
    Task<List<Record>> ApplyFilters(IQueryable<Record> records, string? currentFilter, int? sortOrder, int? genreFilter);
    List<Record> ApplyPagination(List<Record> records, int pageIndex, int pageSize);
    Task<Record> CreateNewRecord(RecordCreateVM recordVM);
    Task<RecordCreateVM> GetRecordViewModelToEdit(Record record, int id);
    void UpdateRecordProperties(Record record, RecordCreateVM recordVM);
    void UpdateRecordPrices(Record record, RecordCreateVM recordVM);
    Task<RecordPrice> GetSelectedFormat(RecordDetailsVM recordVM, int id);
    Task<CartItem> GetCartItem(int productID, StoreUser user);
    bool IsProductInStock(CartItem? cartItem, RecordPrice recordPrice, int quantity);
    Task<bool> AddRecordToCart(RecordPrice recordPrice, StoreUser user, int quantity);
    bool RecordExists(int id);

}