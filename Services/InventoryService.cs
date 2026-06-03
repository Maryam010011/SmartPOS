using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Inventory;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Services
{
    /// <summary>
    /// Service for handling all inventory and stock operations in the SmartPOS system.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<InventoryDto>>> GetAll()
        {
            var records = await _context.Inventory
                .AsNoTracking()
                .Include(i => i.Product)
                .OrderByDescending(i => i.LastUpdated)
                .ToListAsync();

            var dtos = records.Select(MapToDto).ToList();
            return ApiResponse<List<InventoryDto>>.Ok(dtos);
            }

        public async Task<ApiResponse<InventoryDto>> GetByProduct(int productId)
            {
            var record = await _context.Inventory
                .AsNoTracking()
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (record == null)
                return ApiResponse<InventoryDto>.Fail("Inventory record not found for this product.");

            return ApiResponse<InventoryDto>.Ok(MapToDto(record));
            }
        }

        public async Task<ApiResponse<List<InventoryDto>>> GetLowStock()
        {
            var records = await _context.Inventory
                .AsNoTracking()
                .Include(i => i.Product)
                .Where(i => i.Quantity <= i.ReorderLevel)
                .OrderByDescending(i => i.ReorderLevel - i.Quantity)
                .ToListAsync();

            var dtos = records.Select(MapToDto).ToList();
            return ApiResponse<List<InventoryDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Failed to add stock: {ex.Message}");
            }
        }

        /// <summary>
        /// Manually adjusts stock level for a product.
        /// </summary>
        public async Task<ApiResponse> AdjustStock(AdjustStockDto dto)
        {
            var record = await _context.Inventory
                .FirstOrDefaultAsync(i => i.ProductId == dto.ProductId);

            if (record == null)
                return ApiResponse.Fail("Inventory record not found for this product.");

            var newQty = record.Quantity + dto.Quantity;
            if (newQty < 0)
                return ApiResponse.Fail($"Insufficient stock. Current quantity is {record.Quantity}, cannot deduct {Math.Abs(dto.Quantity)}.");

            record.Quantity = newQty;
            record.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ApiResponse.Ok($"Stock adjusted successfully. New quantity: {newQty}");
            }

        public async Task<ApiResponse> AddStock(int productId, int quantity)
            {
            if (quantity <= 0)
                return ApiResponse.Fail("Quantity must be positive.");

            var record = await _context.Inventory
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (record == null)
            {
                var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
                if (!productExists)
                    return ApiResponse.Fail("Product not found.");

                record = new Inventory
                {
                    ProductId = productId,
                    Quantity = quantity,
                    ReorderLevel = 10,
                    LastUpdated = DateTime.UtcNow
                };
                _context.Inventory.Add(record);
            }
            else
            {
                record.Quantity += quantity;
                record.LastUpdated = DateTime.UtcNow;
        }

            await _context.SaveChangesAsync();
            return ApiResponse.Ok($"Added {quantity} units. New quantity: {record.Quantity}");
            }

        public async Task<ApiResponse> DeductStock(int productId, int quantity)
            {
            if (quantity <= 0)
                return ApiResponse.Fail("Quantity must be positive.");

            var record = await _context.Inventory
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (record == null)
                return ApiResponse.Fail("Inventory record not found for this product.");

            if (quantity > record.Quantity)
                return ApiResponse.Fail($"Insufficient stock. Available: {record.Quantity}, requested: {quantity}.");

            record.Quantity -= quantity;
            record.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ApiResponse.Ok($"Deducted {quantity} units. New quantity: {record.Quantity}");
        }

        private static InventoryDto MapToDto(Inventory record)
                {
            return new InventoryDto
            {
                Id = record.Id,
                ProductId = record.ProductId,
                ProductName = record.Product?.Name ?? $"Product #{record.ProductId}",
                Quantity = record.Quantity,
                ReorderLevel = record.ReorderLevel,
                LastUpdated = record.LastUpdated
            };
        }
    }
}