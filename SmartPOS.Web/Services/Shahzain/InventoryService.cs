using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Inventory;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Services.Shahzain;

public class InventoryService : IInventoryService
{
     private readonly AppDbContext _db;

     public InventoryService(AppDbContext db)
     {
          _db = db;
     }

     public async Task<ApiResponse> DeductStock(int productId, int quantity)
     {
          try
          {
               var inventory = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
               if (inventory == null)
                    return ApiResponse.Fail($"No inventory record found for product ID {productId}");
               if (inventory.Quantity < quantity)
                    return ApiResponse.Fail($"Insufficient stock. Available: {inventory.Quantity}, Requested: {quantity}");
               inventory.Quantity -= quantity;
               inventory.LastUpdated = DateTime.UtcNow;
               await _db.SaveChangesAsync();
               return ApiResponse.Ok("Stock deducted successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse.Fail($"Failed to deduct stock: {ex.Message}");
          }
     }

     public async Task<ApiResponse> AddStock(int productId, int quantity)
     {
          try
          {
               var inventory = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
               if (inventory == null)
                    return ApiResponse.Fail($"No inventory record found for product ID {productId}");
               inventory.Quantity += quantity;
               inventory.LastUpdated = DateTime.UtcNow;
               await _db.SaveChangesAsync();
               return ApiResponse.Ok("Stock added successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse.Fail($"Failed to add stock: {ex.Message}");
          }
     }

     public async Task<ApiResponse> AdjustStock(AdjustStockDto dto)
     {
          try
          {
               var inventory = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == dto.ProductId);
               if (inventory == null)
                    return ApiResponse.Fail($"No inventory record found for product ID {dto.ProductId}");
               inventory.Quantity = dto.Quantity;
               inventory.LastUpdated = DateTime.UtcNow;
               await _db.SaveChangesAsync();
               return ApiResponse.Ok("Stock adjusted successfully");
          }
          catch (Exception ex)
          {
               return ApiResponse.Fail($"Failed to adjust stock: {ex.Message}");
          }
     }

     public async Task<ApiResponse<InventoryDto>> GetByProduct(int productId)
     {
          try
          {
               var inventory = await _db.Inventories
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

     public async Task<ApiResponse<List<InventoryDto>>> GetAll()
     {
          try
          {
               var inventories = await _db.Inventories
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

     public async Task<ApiResponse<List<InventoryDto>>> GetLowStock()
     {
          try
          {
               var inventories = await _db.Inventories
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
          }
     }
}