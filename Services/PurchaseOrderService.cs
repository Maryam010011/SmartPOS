using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.PurchaseOrders;
using SmartPOS.Shared.Enums;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Services.MaryamJ
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _context;
        private readonly IInventoryService _inventoryService;

        public PurchaseOrderService(AppDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        public async Task<ApiResponse<List<PurchaseOrderDto>>> GetAll()
        {
            var orders = await _context.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                    .ThenInclude(li => li.Product)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();

            var dtos = orders.Select(MapToDto).ToList();
            return ApiResponse<List<PurchaseOrderDto>>.Ok(dtos);
        }

        public async Task<ApiResponse<PurchaseOrderDto>> GetById(int id)
        {
            var order = await _context.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                    .ThenInclude(li => li.Product)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (order == null)
                return ApiResponse<PurchaseOrderDto>.Fail("Purchase order not found.");

            return ApiResponse<PurchaseOrderDto>.Ok(MapToDto(order));
        }

        public async Task<ApiResponse<PurchaseOrderDto>> Create(CreatePODto dto)
        {
            var supExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId);
            if (!supExists)
                return ApiResponse<PurchaseOrderDto>.Fail("Supplier not found.");

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
                return ApiResponse<PurchaseOrderDto>.Fail("User not found.");

            var order = new PurchaseOrder
            {
                SupplierId = dto.SupplierId,
                UserId = dto.UserId,
                Status = POStatus.Draft,
                OrderDate = DateTime.UtcNow,
                Notes = dto.Notes,
                TotalCost = dto.LineItems.Sum(li => li.OrderedQty * li.UnitPrice)
            };

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var li in dto.LineItems)
            {
                var product = await _context.Products.FindAsync(li.ProductId);
                if (product == null)
                {
                    _context.PurchaseOrders.Remove(order);
                    await _context.SaveChangesAsync();
                    return ApiResponse<PurchaseOrderDto>.Fail($"Product with ID {li.ProductId} not found.");
                }

                order.LineItems.Add(new POLineItem
                {
                    POID = order.Id,
                    ProductId = li.ProductId,
                    OrderedQty = li.OrderedQty,
                    UnitPrice = li.UnitPrice
                });
            }

            await _context.SaveChangesAsync();

            var created = await _context.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                    .ThenInclude(li => li.Product)
                .FirstAsync(po => po.Id == order.Id);

            return ApiResponse<PurchaseOrderDto>.Ok(MapToDto(created));
        }

        public async Task<ApiResponse> MarkAsReceived(int poId, int userId)
        {
            var order = await _context.PurchaseOrders
                .Include(po => po.LineItems)
                .FirstOrDefaultAsync(po => po.Id == poId);

            if (order == null)
                return ApiResponse.Fail("Purchase order not found.");

            if (order.Status != POStatus.Sent)
                return ApiResponse.Fail("Only orders with status 'Sent' can be marked as received.");

            order.Status = POStatus.Received;
            order.ReceivedAt = DateTime.UtcNow;

            foreach (var li in order.LineItems)
            {
                var result = await _inventoryService.AddStock(li.ProductId, li.OrderedQty);
                if (!result.Success)
                {
                    return ApiResponse.Fail($"Failed to update inventory for Product ID {li.ProductId}: {result.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return ApiResponse.Ok("Purchase order marked as received. Inventory updated.");
        }

        public async Task<ApiResponse> Cancel(int poId)
        {
            var order = await _context.PurchaseOrders.FindAsync(poId);

            if (order == null)
                return ApiResponse.Fail("Purchase order not found.");

            if (order.Status == POStatus.Received)
                return ApiResponse.Fail("Cannot cancel a received purchase order.");

            if (order.Status == POStatus.Cancelled)
                return ApiResponse.Fail("Purchase order is already cancelled.");

            order.Status = POStatus.Cancelled;
            await _context.SaveChangesAsync();

            return ApiResponse.Ok("Purchase order cancelled.");
        }

        private static PurchaseOrderDto MapToDto(PurchaseOrder order)
        {
            return new PurchaseOrderDto
            {
                Id = order.Id,
                SupplierId = order.SupplierId,
                SupplierName = order.Supplier?.Name ?? $"Supplier #{order.SupplierId}",
                UserId = order.UserId,
                Status = order.Status,
                TotalCost = order.TotalCost,
                OrderDate = order.OrderDate,
                ReceivedAt = order.ReceivedAt,
                Notes = order.Notes,
                LineItems = order.LineItems.Select(li => new POLineItemDto
                {
                    ProductId = li.ProductId,
                    ProductName = li.Product?.Name ?? $"Product #{li.ProductId}",
                    OrderedQty = li.OrderedQty,
                    UnitPrice = li.UnitPrice
                }).ToList()
            };
        }
    }
}
