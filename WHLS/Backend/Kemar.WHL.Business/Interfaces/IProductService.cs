using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

public interface IProductService
{
    Task<ResultModel> AddOrUpdateAsync(ProductRequest request, int userId, string username);
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<IEnumerable<ProductResponse>> GetByFilterAsync(ProductFilterRequest filter);
    Task<ResultModel> DeleteAsync(int id, string username);
}