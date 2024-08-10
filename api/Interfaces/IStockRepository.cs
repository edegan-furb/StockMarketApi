namespace api.Interfaces;

using api.Dtos.Stock;
using api.Helpers;
using api.Models;

public interface IStockRepository
{
    Task<List<Stock>> GetAllAsync(StockQueryObject query);
    Task<Stock?> GetByIdAsync(int id);
    Task<Stock?> GetBySymbolAsync(string symbol);
    Task<Stock> CreateAsync(Stock stockModel);
    Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto);
    Task<Stock?> DeleteAsync(int id);
    Task<bool> StockExists(int id);
}

