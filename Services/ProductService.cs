using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Products;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class ProductService : IProductService
    {
        private static readonly List<ProductDto> MockProducts = new()
        {
            new ProductDto { Id = 1, Name = "Wireless Mouse", SKU = "WM-001", Price = 250.00m, CostPrice = 150.00m, IsActive = true, CategoryId = 1, CategoryName = "Electronics", SupplierId = 1, SupplierName = "TechWorld Distributors", CurrentStock = 45 },
            new ProductDto { Id = 2, Name = "Mechanical Keyboard", SKU = "MK-002", Price = 450.00m, CostPrice = 290.00m, IsActive = true, CategoryId = 1, CategoryName = "Electronics", SupplierId = 1, SupplierName = "TechWorld Distributors", CurrentStock = 12 },
            new ProductDto { Id = 3, Name = "Organic Coffee Beans", SKU = "CB-001", Price = 120.00m, CostPrice = 55.00m, IsActive = true, CategoryId = 2, CategoryName = "Food & Beverages", SupplierId = 2, SupplierName = "Fresh Foods Supply Co.", CurrentStock = 80 },
            new ProductDto { Id = 4, Name = "Green Tea Bags", SKU = "GT-001", Price = 35.00m, CostPrice = 17.00m, IsActive = true, CategoryId = 2, CategoryName = "Food & Beverages", SupplierId = 2, SupplierName = "Fresh Foods Supply Co.", CurrentStock = 150 },
            new ProductDto { Id = 5, Name = "A4 Printer Paper", SKU = "PP-001", Price = 180.00m, CostPrice = 120.00m, IsActive = true, CategoryId = 3, CategoryName = "Office Supplies", SupplierId = 3, SupplierName = "Office Essentials Ltd.", CurrentStock = 30 },
            new ProductDto { Id = 6, Name = "Stapler", SKU = "ST-001", Price = 120.00m, CostPrice = 80.00m, IsActive = true, CategoryId = 3, CategoryName = "Office Supplies", SupplierId = 3, SupplierName = "Office Essentials Ltd.", CurrentStock = 25 },
            new ProductDto { Id = 7, Name = "USB-C Hub", SKU = "UC-001", Price = 300.00m, CostPrice = 185.00m, IsActive = true, CategoryId = 1, CategoryName = "Electronics", SupplierId = 1, SupplierName = "TechWorld Distributors", CurrentStock = 8 },
            new ProductDto { Id = 8, Name = "Monitor Stand", SKU = "MS-001", Price = 150.00m, CostPrice = 95.00m, IsActive = true, CategoryId = 3, CategoryName = "Office Supplies", SupplierId = 3, SupplierName = "Office Essentials Ltd.", CurrentStock = 5 }
        };
        private static int _nextId = 9;

        public Task<ApiResponse<List<ProductDto>>> GetAll()
        {
            return Task.FromResult(ApiResponse<List<ProductDto>>.Ok(MockProducts.ToList()));
        }

        public Task<ApiResponse<ProductDto>> GetById(int id)
        {
            var product = MockProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return Task.FromResult(ApiResponse<ProductDto>.Fail("Product not found."));
            return Task.FromResult(ApiResponse<ProductDto>.Ok(product));
        }

        public Task<ApiResponse<List<ProductDto>>> GetByCategory(int categoryId)
        {
            var result = MockProducts.Where(p => p.CategoryId == categoryId).ToList();
            return Task.FromResult(ApiResponse<List<ProductDto>>.Ok(result));
        }

        public Task<ApiResponse<List<ProductDto>>> GetBySupplier(int supplierId)
        {
            var result = MockProducts.Where(p => p.SupplierId == supplierId).ToList();
            return Task.FromResult(ApiResponse<List<ProductDto>>.Ok(result));
        }

        public Task<ApiResponse<List<ProductDto>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Task.FromResult(ApiResponse<List<ProductDto>>.Ok(MockProducts.ToList()));
            var kw = keyword.Trim().ToLower();
            var result = MockProducts.Where(p =>
                p.Name.ToLower().Contains(kw) ||
                p.SKU.ToLower().Contains(kw)).ToList();
            return Task.FromResult(ApiResponse<List<ProductDto>>.Ok(result));
        }

        public Task<ApiResponse<ProductDto>> Create(CreateProductDto dto)
        {
            var product = new ProductDto
            {
                Id = _nextId++,
                Name = dto.Name,
                SKU = dto.SKU,
                Description = dto.Description,
                Price = dto.Price,
                CostPrice = dto.CostPrice,
                ImageURL = dto.ImageURL,
                IsActive = true,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                CurrentStock = 0
            };
            MockProducts.Add(product);
            return Task.FromResult(ApiResponse<ProductDto>.Ok(product));
        }

        public Task<ApiResponse<ProductDto>> Update(UpdateProductDto dto)
        {
            var existing = MockProducts.FirstOrDefault(p => p.Id == dto.Id);
            if (existing == null)
                return Task.FromResult(ApiResponse<ProductDto>.Fail("Product not found."));
            existing.Name = dto.Name;
            existing.SKU = dto.SKU;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.CostPrice = dto.CostPrice;
            existing.ImageURL = dto.ImageURL;
            existing.CategoryId = dto.CategoryId;
            existing.SupplierId = dto.SupplierId;
            return Task.FromResult(ApiResponse<ProductDto>.Ok(existing));
        }

        public Task<ApiResponse> Delete(int id)
        {
            var product = MockProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return Task.FromResult(ApiResponse.Fail("Product not found."));
            product.IsActive = false;
            return Task.FromResult(ApiResponse.Ok("Product deactivated."));
        }

        public Task<ApiResponse> ToggleActive(int id)
        {
            var product = MockProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return Task.FromResult(ApiResponse.Fail("Product not found."));
            product.IsActive = !product.IsActive;
            return Task.FromResult(ApiResponse.Ok($"Product {(product.IsActive ? "activated" : "deactivated")}."));
        }
    }
}
