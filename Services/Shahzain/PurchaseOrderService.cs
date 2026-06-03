using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.PurchaseOrders;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Web.Services.Shahzain;

/// <summary>
/// Service for managing purchase orders in the SmartPOS system.
/// Implements CRUD operations and purchase order status management with robust error handling.
/// </summary>
public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PurchaseOrderService"/> class.
    /// </summary>
    /// <param name="factory">The application database context factory.</param>
    public PurchaseOrderService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Retrieves a purchase order by its unique identifier.
    /// </summary>
    /// <param name="id">The purchase order ID.</param>
    /// <returns>An ApiResponse containing the PurchaseOrderDto if found.</returns>
    public async Task<ApiResponse<PurchaseOrderDto>> GetById(int id)
    {
        try
        {
            using var context = _factory.CreateDbContext();
            var purchaseOrder = await context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                .ThenInclude(li => li.Product)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (purchaseOrder == null)
                return ApiResponse<PurchaseOrderDto>.Fail("Purchase order not found.");

            return ApiResponse<PurchaseOrderDto>.Ok(MapToDto(purchaseOrder));
        }
        catch (Exception ex)
        {
            return ApiResponse<PurchaseOrderDto>.Fail($"Error retrieving purchase order: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves all purchase orders from the database.
    /// </summary>
    /// <returns>An ApiResponse containing a list of PurchaseOrderDto objects.</returns>
    public async Task<ApiResponse<List<PurchaseOrderDto>>> GetAll()
    {
        try
        {
            using var context = _factory.CreateDbContext();
            var purchaseOrders = await context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                .ThenInclude(li => li.Product)
                .ToListAsync();

            return ApiResponse<List<PurchaseOrderDto>>.Ok(purchaseOrders.Select(MapToDto).ToList());
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PurchaseOrderDto>>.Fail($"Error retrieving purchase orders: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new purchase order in the system.
    /// </summary>
    /// <param name="dto">The purchase order creation data transfer object.</param>
    /// <returns>An ApiResponse containing the created PurchaseOrderDto.</returns>
    public async Task<ApiResponse<PurchaseOrderDto>> Create(CreatePODto dto)
    {
        try
        {
            using var context = _factory.CreateDbContext();
            // Validate required fields
            if (dto.SupplierId <= 0)
                return ApiResponse<PurchaseOrderDto>.Fail("Supplier ID is required.");

            if (dto.LineItems == null || dto.LineItems.Count == 0)
                return ApiResponse<PurchaseOrderDto>.Fail("At least one line item is required.");

            // Verify supplier exists
            var supplier = await context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null)
                return ApiResponse<PurchaseOrderDto>.Fail("Supplier not found.");

            // Verify all products exist
            var productIds = dto.LineItems.Select(li => li.ProductId).ToList();
            var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            if (products.Count != productIds.Distinct().Count())
                return ApiResponse<PurchaseOrderDto>.Fail("One or more products not found.");

            // Create purchase order
            var purchaseOrder = new PurchaseOrder
            {
                SupplierId = dto.SupplierId,
                UserId = dto.UserId,
                Status = POStatus.Draft,
                OrderDate = DateTime.UtcNow,
                Notes = dto.Notes ?? string.Empty,
                TotalCost = 0 // Will be calculated from line items
            };

            context.PurchaseOrders.Add(purchaseOrder);
            await context.SaveChangesAsync();

            // Add line items
            decimal totalCost = 0;
            foreach (var lineItem in dto.LineItems)
            {
                var item = new POLineItem
                {
                    POID = purchaseOrder.Id,
                    ProductId = lineItem.ProductId,
                    OrderedQty = lineItem.OrderedQty,
                    UnitPrice = lineItem.UnitPrice
                };
                context.POLineItems.Add(item);
                totalCost += lineItem.LineTotal;
            }

            purchaseOrder.TotalCost = totalCost;
            await context.SaveChangesAsync();

            // Fetch and return the created order with related data
            return await GetById(purchaseOrder.Id);
        }
        catch (Exception ex)
        {
            return ApiResponse<PurchaseOrderDto>.Fail($"Error creating purchase order: {ex.Message}");
        }
    }

    /// <summary>
    /// Marks a purchase order as received.
    /// </summary>
    /// <param name="poId">The purchase order ID.</param>
    /// <param name="userId">The user ID performing the action.</param>
    /// <returns>An ApiResponse indicating success or failure.</returns>
    public async Task<ApiResponse> MarkAsReceived(int poId, int userId)
    {
        try
        {
            using var context = _factory.CreateDbContext();
            var purchaseOrder = await context.PurchaseOrders.FindAsync(poId);
            if (purchaseOrder == null)
                return ApiResponse.Fail("Purchase order not found.");

            if (purchaseOrder.Status == POStatus.Received)
                return ApiResponse.Fail("Purchase order is already marked as received.");

            if (purchaseOrder.Status == POStatus.Cancelled)
                return ApiResponse.Fail("Cannot mark a cancelled purchase order as received.");

            purchaseOrder.Status = POStatus.Received;
            purchaseOrder.ReceivedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse.Ok("Purchase order marked as received successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Error marking purchase order as received: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancels a purchase order.
    /// </summary>
    /// <param name="poId">The purchase order ID.</param>
    /// <returns>An ApiResponse indicating success or failure.</returns>
    public async Task<ApiResponse> Cancel(int poId)
    {
        try
        {
            using var context = _factory.CreateDbContext();
            var purchaseOrder = await context.PurchaseOrders.FindAsync(poId);
            if (purchaseOrder == null)
                return ApiResponse.Fail("Purchase order not found.");

            if (purchaseOrder.Status == POStatus.Cancelled)
                return ApiResponse.Fail("Purchase order is already cancelled.");

            if (purchaseOrder.Status == POStatus.Received)
                return ApiResponse.Fail("Cannot cancel a purchase order that has been received.");

            purchaseOrder.Status = POStatus.Cancelled;

            await context.SaveChangesAsync();

            return ApiResponse.Ok("Purchase order cancelled successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Error cancelling purchase order: {ex.Message}");
        }
    }

    /// <summary>
    /// Maps a PurchaseOrder entity to a PurchaseOrderDto.
    /// </summary>
    /// <param name="purchaseOrder">The PurchaseOrder entity.</param>
    /// <returns>A PurchaseOrderDto populated from the entity.</returns>
    private static PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            SupplierId = purchaseOrder.SupplierId,
            SupplierName = purchaseOrder.Supplier?.Name ?? string.Empty,
            UserId = purchaseOrder.UserId,
            Status = purchaseOrder.Status,
            TotalCost = purchaseOrder.TotalCost,
            OrderDate = purchaseOrder.OrderDate,
            ReceivedAt = purchaseOrder.ReceivedAt,
            Notes = purchaseOrder.Notes ?? string.Empty,
            LineItems = purchaseOrder.LineItems?.Select(li => new POLineItemDto
            {
                ProductId = li.ProductId,
                ProductName = li.Product?.Name ?? string.Empty,
                OrderedQty = li.OrderedQty,
                UnitPrice = li.UnitPrice
            }).ToList() ?? new()
        };
    }
}