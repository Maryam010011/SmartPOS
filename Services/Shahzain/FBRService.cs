using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Sales;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for integrating with Pakistan's Federal Board of Revenue (FBR) API.
    /// Handles GST tax calculation, fiscal invoice submission, and invoice status tracking
    /// in compliance with FBR's POS integration requirements.
    /// </summary>
    public class FBRService : IFBRService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _posId;
        private readonly string _apiToken;

        /// <summary>
        /// Pakistan's standard General Sales Tax (GST) rate: 17%.
        /// </summary>
        private const decimal GST_RATE = 0.17m;

        /// <summary>
        /// Initializes a new instance of the <see cref="FBRService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making FBR API requests.</param>
        /// <param name="configuration">The application configuration for FBR API credentials.</param>
        public FBRService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["FBRService:BaseUrl"] ?? "https://gw.fbr.gov.pk/imsp/v1";
            _posId = configuration["FBRService:PosId"] ?? string.Empty;
            _apiToken = configuration["FBRService:ApiToken"] ?? string.Empty;
        }

        /// <summary>
        /// Calculates the 17% General Sales Tax (GST) on a given amount
        /// as per FBR regulations for Tier-1 POS retailers.
        /// </summary>
        /// <param name="amount">The pre-tax amount to calculate GST on.</param>
        /// <returns>An ApiResponse containing the calculated tax amount as a decimal.</returns>
        public async Task<ApiResponse<decimal>> CalculateTax(decimal amount)
        {
            try
            {
                if (amount < 0)
                    return ApiResponse<decimal>.Fail("Amount cannot be negative.");

                var taxAmount = Math.Round(amount * GST_RATE, 2);

                return await Task.FromResult(
                    ApiResponse<decimal>.Ok(taxAmount, $"GST at {GST_RATE:P0} on {amount:C} = {taxAmount:C}"));
            }
            catch (Exception ex)
            {
                return ApiResponse<decimal>.Fail($"Error calculating tax: {ex.Message}");
            }
        }

        /// <summary>
        /// Submits a completed sale as a fiscal invoice to the FBR POS Integration API.
        /// Sends the sale details (items, amounts, tax) and returns the FBR-assigned
        /// invoice reference number for record-keeping and receipt printing.
        /// </summary>
        /// <param name="sale">The sale result data to submit as an invoice.</param>
        /// <returns>An ApiResponse containing the FBR invoice reference number as a string.</returns>
        public async Task<ApiResponse<string>> SubmitInvoice(SaleResultDto sale)
        {
            try
            {
                // Validate sale data
                if (sale == null)
                    return ApiResponse<string>.Fail("Sale data is required.");

                if (sale.Items == null || !sale.Items.Any())
                    return ApiResponse<string>.Fail("Invoice must contain at least one item.");

                if (string.IsNullOrWhiteSpace(_apiToken) || string.IsNullOrWhiteSpace(_posId))
                    return ApiResponse<string>.Fail(
                        "FBR API credentials are not configured. Set 'FBRService:PosId' and 'FBRService:ApiToken' in appsettings.json.");

                // Build FBR invoice payload per FBR POS API spec
                var invoicePayload = new
                {
                    InvoiceNumber = sale.ReceiptNumber,
                    POSID = _posId,
                    USIN = GenerateUSIN(sale),
                    DateTime = sale.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    BuyerNTN = "",       // Optional: buyer's NTN if available
                    BuyerCNIC = "",      // Optional: buyer's CNIC if available
                    BuyerName = "",      // Optional: buyer name
                    BuyerPhoneNumber = "",
                    TotalBillAmount = sale.TotalAmount,
                    TotalQuantity = sale.Items.Sum(i => i.Quantity),
                    TotalSaleValue = sale.SubTotal,
                    TotalTaxCharged = sale.TaxAmount,
                    Discount = sale.DiscountAmount,
                    FurtherTax = 0.0m,
                    PaymentMode = 1,     // 1 = Cash, 2 = Card, 3 = Online
                    RefUSIN = "",        // For returns/refunds only
                    InvoiceType = 1,     // 1 = Normal, 2 = Return, 3 = Debit/Credit Note
                    Items = sale.Items.Select((item, index) => new
                    {
                        ItemCode = $"ITEM-{item.ProductId}",
                        ItemName = item.ProductName,
                        Quantity = item.Quantity,
                        PCTCode = "99.00.0000",  // Default HS code — should be configured per product
                        TaxRate = GST_RATE * 100,
                        SaleValue = item.LineTotal,
                        TotalAmount = item.LineTotal,
                        TaxCharged = Math.Round(item.LineTotal * GST_RATE, 2),
                        Discount = 0.0m,
                        FurtherTax = 0.0m,
                        InvoiceType = 1,
                        RefUSIN = ""
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(invoicePayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Set FBR API authorization headers
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiToken);

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Live/PostData", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return ApiResponse<string>.Fail(
                        $"FBR API returned {response.StatusCode}: {errorBody}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                // Extract the FBR invoice reference from response
                var invoiceRef = doc.RootElement.TryGetProperty("InvoiceNumber", out var refElement)
                    ? refElement.GetString() ?? string.Empty
                    : string.Empty;

                if (string.IsNullOrWhiteSpace(invoiceRef))
                {
                    // Try alternate response field
                    invoiceRef = doc.RootElement.TryGetProperty("Response", out var respElement)
                        ? respElement.GetString() ?? string.Empty
                        : $"FBR-{sale.ReceiptNumber}";
                }

                return ApiResponse<string>.Ok(invoiceRef, "Invoice submitted to FBR successfully.");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<string>.Fail($"FBR API connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Error submitting invoice to FBR: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks the status of a previously submitted fiscal invoice
        /// using its FBR-assigned reference number.
        /// </summary>
        /// <param name="invoiceRef">The FBR invoice reference number to query.</param>
        /// <returns>An ApiResponse containing the status description as a string.</returns>
        public async Task<ApiResponse<string>> GetTaxStatus(string invoiceRef)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(invoiceRef))
                    return ApiResponse<string>.Fail("Invoice reference number is required.");

                if (string.IsNullOrWhiteSpace(_apiToken) || string.IsNullOrWhiteSpace(_posId))
                    return ApiResponse<string>.Fail(
                        "FBR API credentials are not configured. Set 'FBRService:PosId' and 'FBRService:ApiToken' in appsettings.json.");

                // Set FBR API authorization headers
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiToken);

                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/Live/GetInvoiceStatus?POSID={_posId}&InvoiceNumber={invoiceRef}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return ApiResponse<string>.Fail(
                        $"FBR API returned {response.StatusCode}: {errorBody}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                // Parse status from FBR response
                var status = doc.RootElement.TryGetProperty("Status", out var statusElement)
                    ? statusElement.GetString() ?? "Unknown"
                    : "Unknown";

                var message = doc.RootElement.TryGetProperty("Message", out var msgElement)
                    ? msgElement.GetString() ?? string.Empty
                    : string.Empty;

                var statusDescription = string.IsNullOrWhiteSpace(message)
                    ? $"Invoice {invoiceRef}: Status = {status}"
                    : $"Invoice {invoiceRef}: Status = {status} — {message}";

                return ApiResponse<string>.Ok(statusDescription, "Tax status retrieved successfully.");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<string>.Fail($"FBR API connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Error retrieving tax status: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────
        //  Private Helper Methods
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Generates a Unique Sale Identification Number (USIN) for FBR.
        /// Format: {POSID}-{SaleId}-{Timestamp}
        /// </summary>
        /// <param name="sale">The sale result to generate a USIN for.</param>
        /// <returns>A unique string identifier for the fiscal invoice.</returns>
        private string GenerateUSIN(SaleResultDto sale)
        {
            var timestamp = sale.SaleDate.ToString("yyyyMMddHHmmss");
            return $"{_posId}-{sale.SaleId}-{timestamp}";
        }
    }
}
