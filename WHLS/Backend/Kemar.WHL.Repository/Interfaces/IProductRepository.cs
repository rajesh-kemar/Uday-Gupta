using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

public interface IProductRepository
{
    Task<ProductResponse> AddAsync(ProductRequest request);
    Task<ProductResponse?> UpdateAsync(int id, ProductRequest request);
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<IEnumerable<ProductResponse>> GetByFilterAsync(ProductFilterRequest filter);
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<bool> SoftDeleteAsync(int id, string username); // Added SoftDelete for audit
}