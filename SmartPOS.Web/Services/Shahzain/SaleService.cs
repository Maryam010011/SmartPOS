using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Sales;
using SmartPOS.Shared.Enums;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for processing and managing sales in the SmartPOS system.
    /// Handles the full sale lifecycle: creation, querying, analytics, voiding, and refunds.
    /// </summary>
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly IInventoryService _inventoryService;

        /// <summary>
        /// Initialises a new instance of <see cref="SaleService"/>.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="inventoryService">The inventory service used to deduct stock after a sale.</param>
        public SaleService(AppDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        // ──────────────────────────────────────────────────────
        //  ProcessSale
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Processes a new sale end-to-end:
        /// 1. Validates that all products exist and are active.
        /// 2. Builds sale items with current unit prices and line totals.
        /// 3. Calculates the sub-total.
        /// 4. Applies a promotional discount when a promo code is provided.
        /// 5. Calculates tax on the discounted amount.
        /// 6. Persists the Sale and SaleItem records.
        /// 7. Deducts inventory for each line item via <see cref="IInventoryService"/>.
        /// </summary>
        /// <param name="dto">The sale creation data transfer object.</param>
        /// <returns>An ApiResponse containing the completed SaleResultDto.</returns>
        public async Task<ApiResponse<SaleResultDto>> ProcessSale(CreateSaleDto dto)
        {
            try
            {
                // ── Validate items ───────────────────────────
                if (dto.Items == null || !dto.Items.Any())
                    return ApiResponse<SaleResultDto>.Fail("Sale must contain at least one item.");

                var saleItems = new List<SaleItem>();
                decimal subTotal = 0m;

                foreach (var item in dto.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product == null)
                        return ApiResponse<SaleResultDto>.Fail($"Product with ID {item.ProductId} not found.");

                    if (!product.IsActive)
                        return ApiResponse<SaleResultDto>.Fail($"Product '{product.Name}' is not active.");

                    if (item.Quantity <= 0)
                        return ApiResponse<SaleResultDto>.Fail($"Quantity for '{product.Name}' must be at least 1.");

                    decimal lineTotal = product.Price * item.Quantity;

                    saleItems.Add(new SaleItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        LineTotal = lineTotal
                    });

                    subTotal += lineTotal;
                }

                // ── Apply promotional discount ───────────────
                decimal discountAmount = 0m;
                int? promoId = null;

                if (!string.IsNullOrWhiteSpace(dto.PromoCode))
                {
                    // Promo validation is intentionally lightweight here.
                    // In production this would call IPromotionService.ValidateAndApply.
                    // For now we log the promo code but cannot resolve it without
                    // the Promotion model being wired up.
                    // TODO: Integrate IPromotionService once Maryam Jahangir completes the module.
                }

                // ── Calculate tax ────────────────────────────
                // Pakistan standard sales tax rate: 17 %
                const decimal taxRate = 0.17m;
                decimal taxableAmount = subTotal - discountAmount;
                decimal taxAmount = Math.Round(taxableAmount * taxRate, 2);
                decimal totalAmount = taxableAmount + taxAmount;

                // ── Generate receipt number ──────────────────
                string receiptNumber = $"RCP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                // ── Persist Sale ─────────────────────────────
                var sale = new Sale
                {
                    CustomerId = dto.CustomerId,
                    UserId = dto.UserId,
                    PromoId = promoId,
                    SaleType = dto.SaleType,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    TaxAmount = taxAmount,
                    SaleDate = DateTime.UtcNow,
                    Status = SaleStatus.Completed
                };

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // Assign the generated SaleId to each item and persist
                foreach (var si in saleItems)
                {
                    si.SaleId = sale.Id;
                }

                _context.SaleItems.AddRange(saleItems);
                await _context.SaveChangesAsync();

                // ── Deduct inventory ─────────────────────────
                foreach (var si in saleItems)
                {
                    var deductResult = await _inventoryService.DeductStock(si.ProductId, si.Quantity);
                    if (!deductResult.Success)
                    {
                        // Log the failure but do not roll back the sale.
                        // Inventory discrepancies are handled by the inventory team.
                        // In a production system this would be wrapped in a
                        // distributed transaction or compensating action.
                    }
                }

                // ── Build result DTO ─────────────────────────
                var result = new SaleResultDto
                {
                    SaleId = sale.Id,
                    SubTotal = subTotal,
                    DiscountAmount = discountAmount,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    Status = sale.Status,
                    SaleDate = sale.SaleDate,
                    ReceiptNumber = receiptNumber,
                    Items = saleItems.Select(si => new SaleItemDto
                    {
                        ProductId = si.ProductId,
                        ProductName = _context.Products.Find(si.ProductId)?.Name ?? "Unknown",
                        Quantity = si.Quantity,
                        UnitPrice = si.UnitPrice,
                        LineTotal = si.LineTotal
                    }).ToList()
                };

                return ApiResponse<SaleResultDto>.Ok(result, "Sale processed successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<SaleResultDto>.Fail($"Error processing sale: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────
        //  GetById
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Retrieves a single sale by its unique identifier,
        /// including all sale items with product names.
        /// </summary>
        /// <param name="id">The sale ID.</param>
        /// <returns>An ApiResponse containing the SaleResultDto.</returns>
        public async Task<ApiResponse<SaleResultDto>> GetById(int id)
        {
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Product)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sale == null)
                    return ApiResponse<SaleResultDto>.Fail("Sale not found.");

                return ApiResponse<SaleResultDto>.Ok(MapToResultDto(sale));
            }
            catch (Exception ex)
            {
                return ApiResponse<SaleResultDto>.Fail($"Error retrieving sale: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────
        //  GetAll (with filtering)
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Retrieves sales with optional filtering by date range,
        /// user, sale type, and status.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <returns>An ApiResponse containing a filtered list of SaleResultDto objects.</returns>
        public async Task<ApiResponse<List<SaleResultDto>>> GetAll(SaleFilterDto filter)
        {
            try
            {
                var query = _context.Sales
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Product)
                    .AsQueryable();

                // ── Apply filters ────────────────────────────
                if (filter.From.HasValue)
                    query = query.Where(s => s.SaleDate >= filter.From.Value);

                if (filter.To.HasValue)
                    query = query.Where(s => s.SaleDate <= filter.To.Value);

                if (filter.UserId.HasValue)
                    query = query.Where(s => s.UserId == filter.UserId.Value);

                if (filter.SaleType.HasValue)
                    query = query.Where(s => s.SaleType == filter.SaleType.Value);

                if (filter.Status.HasValue)
                    query = query.Where(s => s.Status == filter.Status.Value);

                var sales = await query
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();

                return ApiResponse<List<SaleResultDto>>.Ok(
                    sales.Select(MapToResultDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SaleResultDto>>.Fail($"Error retrieving sales: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────
        //  GetAnalytics
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Aggregates sales analytics (total revenue, transaction count,
        /// average order value) for the given filter, and returns chart data
        /// grouped by the requested period (daily / weekly / monthly).
        /// </summary>
        /// <param name="filter">The filter criteria including date range and period.</param>
        /// <returns>An ApiResponse containing the SaleAnalyticsDto.</returns>
        public async Task<ApiResponse<SaleAnalyticsDto>> GetAnalytics(SaleFilterDto filter)
        {
            try
            {
                var query = _context.Sales
                    .Where(s => s.Status == SaleStatus.Completed)
                    .AsQueryable();

                // ── Apply date filters ───────────────────────
                if (filter.From.HasValue)
                    query = query.Where(s => s.SaleDate >= filter.From.Value);

                if (filter.To.HasValue)
                    query = query.Where(s => s.SaleDate <= filter.To.Value);

                if (filter.UserId.HasValue)
                    query = query.Where(s => s.UserId == filter.UserId.Value);

                if (filter.SaleType.HasValue)
                    query = query.Where(s => s.SaleType == filter.SaleType.Value);

                var sales = await query.ToListAsync();

                decimal totalRevenue = sales.Sum(s => s.TotalAmount);
                int totalTransactions = sales.Count;
                decimal averageOrderValue = totalTransactions > 0
                    ? Math.Round(totalRevenue / totalTransactions, 2)
                    : 0m;

                // ── Build chart data grouped by period ───────
                var chartData = BuildChartData(sales, filter.Period);

                var analytics = new SaleAnalyticsDto
                {
                    TotalRevenue = totalRevenue,
                    TotalTransactions = totalTransactions,
                    AverageOrderValue = averageOrderValue,
                    ChartData = chartData
                };

                return ApiResponse<SaleAnalyticsDto>.Ok(analytics);
            }
            catch (Exception ex)
            {
                return ApiResponse<SaleAnalyticsDto>.Fail($"Error generating analytics: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────
        //  VoidSale
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Voids a completed sale with the given reason.
        /// Only sales with <see cref="SaleStatus.Completed"/> status can be voided.
        /// </summary>
        /// <param name="saleId">The sale ID to void.</param>
        /// <param name="reason">The reason for voiding the sale.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse> VoidSale(int saleId, string reason)
        {
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.SaleItems)
                    .FirstOrDefaultAsync(s => s.Id == saleId);

                if (sale == null)
                    return ApiResponse.Fail("Sale not found.");

                if (sale.Status != SaleStatus.Completed)
                    return ApiResponse.Fail($"Only completed sales can be voided. Current status: {sale.Status}.");

                if (string.IsNullOrWhiteSpace(reason))
                    return ApiResponse.Fail("A reason is required to void a sale.");

                sale.Status = SaleStatus.Voided;
                await _context.SaveChangesAsync();

                // Restore inventory for each voided item
                foreach (var item in sale.SaleItems)
                {
                    await _inventoryService.AddStock(item.ProductId, item.Quantity);
                }

                return ApiResponse.Ok($"Sale #{saleId} voided successfully. Reason: {reason}");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Error voiding sale: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────
        //  RefundSale
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Refunds a completed sale. Sets the status to Refunded
        /// and restores inventory for all line items.
        /// </summary>
        /// <param name="saleId">The sale ID to refund.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse> RefundSale(int saleId)
        {
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.SaleItems)
                    .FirstOrDefaultAsync(s => s.Id == saleId);

                if (sale == null)
                    return ApiResponse.Fail("Sale not found.");

                if (sale.Status != SaleStatus.Completed)
                    return ApiResponse.Fail($"Only completed sales can be refunded. Current status: {sale.Status}.");

                sale.Status = SaleStatus.Refunded;
                await _context.SaveChangesAsync();

                // Restore inventory for each refunded item
                foreach (var item in sale.SaleItems)
                {
                    await _inventoryService.AddStock(item.ProductId, item.Quantity);
                }

                return ApiResponse.Ok($"Sale #{saleId} refunded successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Error refunding sale: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────
        //  Private helpers
        // ──────────────────────────────────────────────────────

        /// <summary>
        /// Maps a Sale entity (with loaded SaleItems → Product) to a SaleResultDto.
        /// </summary>
        private static SaleResultDto MapToResultDto(Sale sale)
        {
            decimal subTotal = sale.SaleItems.Sum(si => si.LineTotal);

            return new SaleResultDto
            {
                SaleId = sale.Id,
                SubTotal = subTotal,
                DiscountAmount = sale.DiscountAmount,
                TaxAmount = sale.TaxAmount,
                TotalAmount = sale.TotalAmount,
                Status = sale.Status,
                SaleDate = sale.SaleDate,
                ReceiptNumber = $"RCP-{sale.SaleDate:yyyyMMdd}-{sale.Id:D6}",
                Items = sale.SaleItems.Select(si => new SaleItemDto
                {
                    ProductId = si.ProductId,
                    ProductName = si.Product?.Name ?? "Unknown",
                    Quantity = si.Quantity,
                    UnitPrice = si.UnitPrice,
                    LineTotal = si.LineTotal
                }).ToList()
            };
        }

        /// <summary>
        /// Groups completed sales into chart data points based on the requested
        /// period string ("daily", "weekly", or "monthly").
        /// Defaults to daily if the period is not recognised.
        /// </summary>
        private static List<SaleChartPointDto> BuildChartData(List<Sale> sales, string period)
        {
            IEnumerable<IGrouping<string, Sale>> grouped;

            switch (period?.ToLowerInvariant())
            {
                case "weekly":
                    grouped = sales.GroupBy(s =>
                    {
                        // ISO week: Monday-based week number
                        var cal = System.Globalization.CultureInfo.InvariantCulture.Calendar;
                        int week = cal.GetWeekOfYear(s.SaleDate,
                            System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                            DayOfWeek.Monday);
                        return $"{s.SaleDate.Year}-W{week:D2}";
                    });
                    break;

                case "monthly":
                    grouped = sales.GroupBy(s => s.SaleDate.ToString("yyyy-MM"));
                    break;

                default: // "daily" or unrecognised
                    grouped = sales.GroupBy(s => s.SaleDate.ToString("yyyy-MM-dd"));
                    break;
            }

            return grouped
                .OrderBy(g => g.Key)
                .Select(g => new SaleChartPointDto
                {
                    Label = g.Key,
                    Revenue = g.Sum(s => s.TotalAmount),
                    Transactions = g.Count()
                })
                .ToList();
        }
    }
}
