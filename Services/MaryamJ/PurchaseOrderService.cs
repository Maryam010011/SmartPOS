using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.PurchaseOrders;
using SmartPOS.Shared.Enums;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private static readonly List<PurchaseOrderDto> MockOrders = new()
        {
            new PurchaseOrderDto
            {
                Id = 1, SupplierId = 1, SupplierName = "TechWorld Distributors", UserId = 2,
                Status = POStatus.Received, TotalCost = 15250.00m,
                OrderDate = DateTime.Now.AddDays(-5), ReceivedAt = DateTime.Now.AddDays(-3),
                Notes = "Regular monthly restock", LineItems = new()
                {
                    new POLineItemDto { ProductId = 1, ProductName = "Wireless Mouse", OrderedQty = 50, UnitPrice = 150.00m },
                    new POLineItemDto { ProductId = 2, ProductName = "Mechanical Keyboard", OrderedQty = 25, UnitPrice = 290.00m }
                }
            },
            new PurchaseOrderDto
            {
                Id = 2, SupplierId = 2, SupplierName = "Fresh Foods Supply Co.", UserId = 2,
                Status = POStatus.Sent, TotalCost = 8900.00m,
                OrderDate = DateTime.Now.AddDays(-2), LineItems = new()
                {
                    new POLineItemDto { ProductId = 3, ProductName = "Organic Coffee Beans", OrderedQty = 100, UnitPrice = 55.00m },
                    new POLineItemDto { ProductId = 4, ProductName = "Green Tea Bags", OrderedQty = 200, UnitPrice = 17.00m }
                }
            },
            new PurchaseOrderDto
            {
                Id = 3, SupplierId = 3, SupplierName = "Office Essentials Ltd.", UserId = 2,
                Status = POStatus.Draft, TotalCost = 3400.00m,
                OrderDate = DateTime.Now.AddDays(-1), LineItems = new()
                {
                    new POLineItemDto { ProductId = 5, ProductName = "A4 Printer Paper", OrderedQty = 20, UnitPrice = 120.00m },
                    new POLineItemDto { ProductId = 6, ProductName = "Stapler", OrderedQty = 10, UnitPrice = 80.00m }
                }
            },
            new PurchaseOrderDto
            {
                Id = 4, SupplierId = 1, SupplierName = "TechWorld Distributors", UserId = 2,
                Status = POStatus.Cancelled, TotalCost = 6100.00m,
                OrderDate = DateTime.Now.AddDays(-10), LineItems = new()
                {
                    new POLineItemDto { ProductId = 7, ProductName = "USB-C Hub", OrderedQty = 30, UnitPrice = 185.00m },
                    new POLineItemDto { ProductId = 8, ProductName = "Monitor Stand", OrderedQty = 5, UnitPrice = 95.00m }
                }
            }
        };
        private static int _nextId = 5;

        public Task<ApiResponse<List<PurchaseOrderDto>>> GetAll()
        {
            var result = MockOrders.OrderByDescending(o => o.OrderDate).ToList();
            return Task.FromResult(ApiResponse<List<PurchaseOrderDto>>.Ok(result));
        }

        public Task<ApiResponse<PurchaseOrderDto>> GetById(int id)
        {
            var order = MockOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return Task.FromResult(ApiResponse<PurchaseOrderDto>.Fail("Purchase order not found."));
            return Task.FromResult(ApiResponse<PurchaseOrderDto>.Ok(order));
        }

        public Task<ApiResponse<PurchaseOrderDto>> Create(CreatePODto dto)
        {
            var newOrder = new PurchaseOrderDto
            {
                Id = _nextId++,
                SupplierId = dto.SupplierId,
                SupplierName = $"Supplier #{dto.SupplierId}",
                UserId = dto.UserId,
                Status = POStatus.Draft,
                TotalCost = dto.LineItems.Sum(li => li.LineTotal),
                OrderDate = DateTime.Now,
                Notes = dto.Notes,
                LineItems = dto.LineItems.Select(li => new POLineItemDto
                {
                    ProductId = li.ProductId,
                    ProductName = li.ProductName,
                    OrderedQty = li.OrderedQty,
                    UnitPrice = li.UnitPrice
                }).ToList()
            };
            MockOrders.Add(newOrder);
            return Task.FromResult(ApiResponse<PurchaseOrderDto>.Ok(newOrder));
        }

        public Task<ApiResponse> MarkAsReceived(int poId, int userId)
        {
            var order = MockOrders.FirstOrDefault(o => o.Id == poId);
            if (order == null)
                return Task.FromResult(ApiResponse.Fail("Purchase order not found."));
            if (order.Status == POStatus.Received)
                return Task.FromResult(ApiResponse.Fail("Purchase order is already received."));
            order.Status = POStatus.Received;
            order.ReceivedAt = DateTime.Now;
            return Task.FromResult(ApiResponse.Ok("Purchase order marked as received."));
        }

        public Task<ApiResponse> Cancel(int poId)
        {
            var order = MockOrders.FirstOrDefault(o => o.Id == poId);
            if (order == null)
                return Task.FromResult(ApiResponse.Fail("Purchase order not found."));
            if (order.Status == POStatus.Received || order.Status == POStatus.Cancelled)
                return Task.FromResult(ApiResponse.Fail("Cannot cancel a received or already cancelled order."));
            order.Status = POStatus.Cancelled;
            return Task.FromResult(ApiResponse.Ok("Purchase order cancelled."));
        }
    }
}
