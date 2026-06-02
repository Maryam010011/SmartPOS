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
        private readonly AppDbContext _context;

     {
            _context = context;
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

               if (inventory == null)
                    return ApiResponse.Fail("Inventory record not found for this product.");

               if (inventory.Quantity < quantity)
                    return ApiResponse.Fail("Insufficient stock to complete the sale.");

               inventory.Quantity -= quantity;
               inventory.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse.Ok("Stock deducted successfully.");
          }
          catch (Exception ex)
          {
                return ApiResponse.Fail($"Error deducting stock: {ex.Message}");
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

               if (inventory == null)
                {
                    if (!await _context.Products.AnyAsync(p => p.Id == productId))
                        return ApiResponse.Fail("Product not found.");

                    inventory = new Inventory
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        ReorderLevel = 0,
                        LastUpdated = DateTime.UtcNow
                    };

                    _context.Inventories.Add(inventory);
                }
                else
                {
               inventory.Quantity += quantity;
               inventory.LastUpdated = DateTime.UtcNow;
               await _db.SaveChangesAsync();
               return ApiResponse.Ok("Stock added successfully");
          }

                await _context.SaveChangesAsync();

                return ApiResponse.Ok("Stock added successfully.");
            }
          catch (Exception ex)
          {
                return ApiResponse.Fail($"Error adding stock: {ex.Message}");
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

               if (inventory == null)
                {
                    if (!await _context.Products.AnyAsync(p => p.Id == dto.ProductId))
                        return ApiResponse.Fail("Product not found.");

                    inventory = new Inventory
                    {
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        ReorderLevel = 0,
                        LastUpdated = DateTime.UtcNow
                    };

                    _context.Inventories.Add(inventory);
                }
                else
                {
               inventory.Quantity = dto.Quantity;
               inventory.LastUpdated = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return ApiResponse.Ok("Stock adjusted successfully.");
          }
          catch (Exception ex)
          {
                return ApiResponse.Fail($"Error adjusting stock: {ex.Message}");
          }
     }

        /// <summary>
        /// Retrieves current stock level for a single product.
        /// </summary>
     public async Task<ApiResponse<InventoryDto>> GetByProduct(int productId)
     {
          try
          {
                var inventory = await _context.Inventories
                    .Include(i => i.Product)
                   .FirstOrDefaultAsync(i => i.ProductId == productId);

               if (inventory == null)
                    return ApiResponse<InventoryDto>.Fail("Inventory record not found for this product.");

                return ApiResponse<InventoryDto>.Ok(MapToDto(inventory));
          }
          catch (Exception ex)
          {
                return ApiResponse<InventoryDto>.Fail($"Error retrieving inventory: {ex.Message}");
          }
     }

        /// <summary>
        /// Retrieves all inventory records for the manager's inventory page.
        /// </summary>
     public async Task<ApiResponse<List<InventoryDto>>> GetAll()
     {
          try
          {
                var inventories = await _context.Inventories
                   .Include(i => i.Product)
                   .ToListAsync();

                return ApiResponse<List<InventoryDto>>.Ok(inventories.Select(MapToDto).ToList());
          }
          catch (Exception ex)
          {
                return ApiResponse<List<InventoryDto>>.Fail($"Error retrieving inventory: {ex.Message}");
          }
     }

        /// <summary>
        /// Retrieves all low stock inventory records.
        /// </summary>
     public async Task<ApiResponse<List<InventoryDto>>> GetLowStock()
     {
          try
          {
                var inventories = await _context.Inventories
                   .Include(i => i.Product)
                   .Where(i => i.Quantity <= i.ReorderLevel)
                   .ToListAsync();

                return ApiResponse<List<InventoryDto>>.Ok(inventories.Select(MapToDto).ToList());
            }
            catch (Exception ex)
               {
                return ApiResponse<List<InventoryDto>>.Fail($"Error retrieving low stock items: {ex.Message}");
            }
          }

        /// <summary>
        /// Maps an Inventory entity to InventoryDto.
        /// </summary>
        private static InventoryDto MapToDto(Inventory inventory)
        {
            return new InventoryDto
          {
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