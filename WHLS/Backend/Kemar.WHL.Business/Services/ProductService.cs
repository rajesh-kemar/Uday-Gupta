using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ResultModel> AddOrUpdateAsync(ProductRequest request, int userId, string username)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return ResultModel.Invalid("Name is required");

        if (request.Weight <= 0)
            return ResultModel.Invalid("Weight must be greater than zero");

        // Add
        if (request.ProductId == null || request.ProductId == 0)
        {
            request.CreatedBy = username;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedBy = username;
            request.UpdatedAt = DateTime.UtcNow;

            var added = await _repo.AddAsync(request);
            return ResultModel.Created("Product added successfully", added);
        }

        // Update
        request.UpdatedBy = username;
        request.UpdatedAt = DateTime.UtcNow;

        var updated = await _repo.UpdateAsync(request.ProductId.Value, request);

        if (updated == null)
            return ResultModel.NotFound("Product not found");

        return ResultModel.Updated("Product updated successfully", updated);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        => await _repo.GetAllAsync();

    public async Task<ProductResponse?> GetByIdAsync(int id)
        => await _repo.GetByIdAsync(id);

    public async Task<IEnumerable<ProductResponse>> GetByFilterAsync(ProductFilterRequest filter)
        => await _repo.GetByFilterAsync(filter);

    public async Task<ResultModel> DeleteAsync(int id, string username)
    {
        var deleted = await _repo.SoftDeleteAsync(id, username);

        if (!deleted)
            return ResultModel.NotFound("Product not found");

        return ResultModel.Success("Product deleted successfully");
    }
}