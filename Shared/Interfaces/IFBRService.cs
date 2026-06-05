using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Sales;

namespace SmartPOS.Shared.Interfaces;

public interface IFBRService
{
    Task<ApiResponse<decimal>> CalculateTax(decimal amount);
    Task<ApiResponse<string>> SubmitInvoice(SaleResultDto sale);
    Task<ApiResponse<string>> GetTaxStatus(string invoiceRef);
}
