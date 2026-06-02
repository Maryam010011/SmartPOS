using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Products;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for managing products in the SmartPOS system.
    /// Implements CRUD operations with robust error handling.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public ProductService(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>An ApiResponse containing the ProductDto.</returns>
        public async Task<ApiResponse<ProductDto>> GetById(int id)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var product = await context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return ApiResponse<ProductDto>.Fail("Product not found.");

                return ApiResponse<ProductDto>.Ok(MapToDto(product));
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.Fail($"Error retrieving product: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>An ApiResponse containing a list of ProductDto objects.</returns>
        public async Task<ApiResponse<List<ProductDto>>> GetAll()
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var products = await context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .ToListAsync();

                return ApiResponse<List<ProductDto>>.Ok(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDto>>.Fail($"Error retrieving products: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves products belonging to a specific category.
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        /// <returns>An ApiResponse containing a list of ProductDto objects.</returns>
        public async Task<ApiResponse<List<ProductDto>>> GetByCategory(int categoryId)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var products = await context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Where(p => p.CategoryId == categoryId)
                    .ToListAsync();

                return ApiResponse<List<ProductDto>>.Ok(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDto>>.Fail($"Error retrieving products by category: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves products supplied by a specific supplier.
        /// </summary>
        /// <param name="supplierId">The supplier ID.</param>
        /// <returns>An ApiResponse containing a list of ProductDto objects.</returns>
        public async Task<ApiResponse<List<ProductDto>>> GetBySupplier(int supplierId)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var products = await context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Where(p => p.SupplierId == supplierId)
                    .ToListAsync();

                return ApiResponse<List<ProductDto>>.Ok(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDto>>.Fail($"Error retrieving products by supplier: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new product in the system.
        /// </summary>
        /// <param name="dto">The product data transfer object.</param>
        /// <returns>An ApiResponse containing the created ProductDto.</returns>
        public async Task<ApiResponse<ProductDto>> Create(CreateProductDto dto)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                // Basic validation
                if (await context.Products.AnyAsync(p => p.SKU == dto.SKU))
                    return ApiResponse<ProductDto>.Fail("A product with this SKU already exists.");

                var product = new Product
                {
                    Name = dto.Name,
                    SKU = dto.SKU,
                    Description = dto.Description,
                    Price = dto.Price,
                    CostPrice = dto.CostPrice,
                    ImageURL = dto.ImageURL,
                    CategoryId = dto.CategoryId,
                    SupplierId = dto.SupplierId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.Products.Add(product);
                await context.SaveChangesAsync();

                // Fetch again to get related entities for mapping
                return await GetById(product.Id);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.Fail($"Error creating product: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing product's information.
        /// </summary>
        /// <param name="dto">The product update data transfer object.</param>
        /// <returns>An ApiResponse containing the updated ProductDto.</returns>
        public async Task<ApiResponse<ProductDto>> Update(UpdateProductDto dto)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var product = await context.Products.FindAsync(dto.Id);
                if (product == null)
                    return ApiResponse<ProductDto>.Fail("Product not found.");

                // Check SKU uniqueness if changed
                if (product.SKU != dto.SKU && await context.Products.AnyAsync(p => p.SKU == dto.SKU))
                    return ApiResponse<ProductDto>.Fail("Another product with this SKU already exists.");

                product.Name = dto.Name;
                product.SKU = dto.SKU;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.CostPrice = dto.CostPrice;
                product.ImageURL = dto.ImageURL;
                product.CategoryId = dto.CategoryId;
                product.SupplierId = dto.SupplierId;
                product.IsActive = dto.IsActive;

                await context.SaveChangesAsync();

                return await GetById(product.Id);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.Fail($"Error updating product: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a product from the database.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var product = await context.Products.FindAsync(id);
                if (product == null)
                    return ApiResponse.Fail("Product not found.");

                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return ApiResponse.Ok("Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Error deleting product: {ex.Message}");
            }
        }

        /// <summary>
        /// Toggles the active status of a product.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse> ToggleActive(int id)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var product = await context.Products.FindAsync(id);
                if (product == null)
                    return ApiResponse.Fail("Product not found.");

                product.IsActive = !product.IsActive;
                await context.SaveChangesAsync();

                return ApiResponse.Ok($"Product is now {(product.IsActive ? "Active" : "Inactive")}.");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Error toggling product status: {ex.Message}");
            }
        }

        /// <summary>
        /// Searches for products by name or SKU.
        /// </summary>
        /// <param name="keyword">The search keyword.</param>
        /// <returns>An ApiResponse containing a list of matching ProductDto objects.</returns>
        public async Task<ApiResponse<List<ProductDto>>> Search(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return await GetAll();

                using var context = _factory.CreateDbContext();
                var products = await context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Where(p => p.Name.Contains(keyword) || p.SKU.Contains(keyword))
                    .ToListAsync();

                return ApiResponse<List<ProductDto>>.Ok(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDto>>.Fail($"Error searching products: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps a Product entity to a ProductDto.
        /// </summary>
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description ?? string.Empty,
                Price = product.Price,
                CostPrice = product.CostPrice,
                ImageURL = product.ImageURL ?? string.Empty,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "N/A",
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier?.Name ?? "N/A",
                CurrentStock = 0 // Placeholder: To be implemented with Inventory module
            };
        }
    }
}
