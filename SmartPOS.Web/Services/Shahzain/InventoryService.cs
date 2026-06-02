using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Inventory;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for handling all inventory and stock operations in the SmartPOS system.
    /// </summary>
public class InventoryService : IInventoryService
{
     private readonly IDbContextFactory<AppDbContext> _factory;

     public InventoryService(IDbContextFactory<AppDbContext> factory)
     {
          _factory = factory;
     }

        /// <summary>
        /// Deducts stock for a product when a sale is processed.
        /// </summary>
     public async Task<ApiResponse> DeductStock(int productId, int quantity)
     {
          try
          {
                if (quantity <= 0)
                    return ApiResponse.Fail("Quantity must be greater than zero.");

                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == productId);

               using var db = _factory.CreateDbContext();
               var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
               if (inventory == null)
                    return ApiResponse.Fail($"No inventory record found for product ID {productId}");
               if (inventory.Quantity < quantity)
                    return ApiResponse.Fail($"Insufficient stock. Available: {inventory.Quantity}, Requested: {quantity}");
               inventory.Quantity -= quantity;
               inventory.LastUpdated = DateTime.UtcNow;
               await db.SaveChangesAsync();
               return ApiResponse.Ok("Stock deducted successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse.Fail($"Failed to deduct stock: {ex.Message}");
          }
     }

        /// <summary>
        /// Adds stock back for a product when a sale is voided or refunded.
        /// </summary>
     public async Task<ApiResponse> AddStock(int productId, int quantity)
     {
          try
          {
                if (quantity <= 0)
                    return ApiResponse.Fail("Quantity must be greater than zero.");

                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == productId);

               using var db = _factory.CreateDbContext();
               var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
               if (inventory == null)
                    return ApiResponse.Fail($"No inventory record found for product ID {productId}");
               inventory.Quantity += quantity;
               inventory.LastUpdated = DateTime.UtcNow;
               await db.SaveChangesAsync();
               return ApiResponse.Ok("Stock added successfully");
          }

                await _context.SaveChangesAsync();

                return ApiResponse.Ok("Stock added successfully.");
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
          try
          {
                if (dto.Quantity < 0)
                    return ApiResponse.Fail("Quantity cannot be negative.");

                if (string.IsNullOrWhiteSpace(dto.Reason))
                    return ApiResponse.Fail("A reason is required for stock adjustment.");

                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == dto.ProductId);

               using var db = _factory.CreateDbContext();
               var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.ProductId == dto.ProductId);
               if (inventory == null)
                    return ApiResponse.Fail($"No inventory record found for product ID {dto.ProductId}");
               inventory.Quantity = dto.Quantity;
               inventory.LastUpdated = DateTime.UtcNow;
               await db.SaveChangesAsync();
               return ApiResponse.Ok("Stock adjusted successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse.Fail($"Failed to adjust stock: {ex.Message}");
          }
     }

        /// <summary>
        /// Retrieves current stock level for a single product.
        /// </summary>
     public async Task<ApiResponse<InventoryDto>> GetByProduct(int productId)
     {
          try
          {
               using var db = _factory.CreateDbContext();
               var inventory = await db.Inventories
                   .FirstOrDefaultAsync(i => i.ProductId == productId);

               if (inventory == null)
                    return ApiResponse<InventoryDto>.Fail($"No inventory record found for product ID {productId}");
               var dto = new InventoryDto
               {
                    Id = inventory.Id,
                    ProductId = inventory.ProductId,
                    Quantity = inventory.Quantity,
                    ReorderLevel = inventory.ReorderLevel,
                    LastUpdated = inventory.LastUpdated
               };
               return ApiResponse<InventoryDto>.Ok(dto, "Inventory retrieved successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse<InventoryDto>.Fail($"Failed to retrieve inventory: {ex.Message}");
          }
     }

        /// <summary>
        /// Retrieves all inventory records for the manager's inventory page.
        /// </summary>
     public async Task<ApiResponse<List<InventoryDto>>> GetAll()
     {
          try
          {
               using var db = _factory.CreateDbContext();
               var inventories = await db.Inventories
                   .Include(i => i.Product)
                   .ToListAsync();
               var dtos = inventories.Select(i => new InventoryDto
               {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    ReorderLevel = i.ReorderLevel,
                    LastUpdated = i.LastUpdated
               }).ToList();
               return ApiResponse<List<InventoryDto>>.Ok(dtos, "All inventory retrieved successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse<List<InventoryDto>>.Fail($"Failed to retrieve inventory: {ex.Message}");
          }
     }

        /// <summary>
        /// Retrieves all low stock inventory records.
        /// </summary>
     public async Task<ApiResponse<List<InventoryDto>>> GetLowStock()
     {
          try
          {
               using var db = _factory.CreateDbContext();
               var inventories = await db.Inventories
                   .Include(i => i.Product)
                   .Where(i => i.Quantity <= i.ReorderLevel)
                   .ToListAsync();
               var dtos = inventories.Select(i => new InventoryDto
               {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    ReorderLevel = i.ReorderLevel,
                    LastUpdated = i.LastUpdated
               }).ToList();
               return ApiResponse<List<InventoryDto>>.Ok(dtos, "Low stock items retrieved successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse<List<InventoryDto>>.Fail($"Failed to retrieve low stock: {ex.Message}");
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                ProductName = inventory.Product?.Name ?? "N/A",
                Quantity = inventory.Quantity,
                ReorderLevel = inventory.ReorderLevel,
                LastUpdated = inventory.LastUpdated
            };
          }
     }
}