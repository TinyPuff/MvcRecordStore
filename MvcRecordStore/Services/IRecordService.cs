using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcRecordStore.Services;

public interface IRecordService
{
    IQueryable<Record> GetAllRecords();
    Task<Record> GetRecordWithDependencies(int id);
    Task<Record> GetRecordWithoutDependencies(int id);
    Task<Artist> GetSelectedArtist(RecordCreateVM recordVM);
    List<Genre> GetRecordSelectedGenres(RecordCreateVM recordVM);
    Task<Label> GetRecordSelectedLabel(RecordCreateVM recordVM);
    List<int> GetRecordGenreIDs(Record record);
    Task<List<RecordPrice>> GetRecordPrices(int recordID);
    Task<RecordDetailsVM> GetRecordViewModelDetails(Record record, int id);
    Task<Record> CreateNewRecord(RecordCreateVM recordVM);
    Task<RecordCreateVM> GetRecordViewModelToEdit(Record record, int id);
    void UpdateRecordProperties(Record record, RecordCreateVM recordVM);
    void UpdateRecordPrices(Record record, RecordCreateVM recordVM);
    Task<RecordPrice> GetSelectedFormat(RecordDetailsVM recordVM, int id);
    Task<CartItem> GetCartItem(int productID, StoreUser user);
    bool IsProductInStock(CartItem? cartItem, RecordPrice recordPrice);
    Task<bool> AddRecordToCart(RecordPrice recordPrice, StoreUser user);
    bool RecordExists(int id);
}