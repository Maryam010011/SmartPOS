using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Customers;

namespace SmartPOS.Shared.Interfaces;

public interface ICustomerService
{
    Task<ApiResponse<List<CustomerDto>>> GetAllCustomers(CustomerFilterDto filter);
    Task<ApiResponse<CustomerDto>> GetById(int id);
    Task<ApiResponse<CustomerDetailDto>> GetCustomerById(int id);
    Task<ApiResponse<CustomerDto>> GetByEmail(string email);
    Task<ApiResponse<CustomerDto>> CreateCustomer(CreateCustomerDto dto);
    Task<ApiResponse<CustomerDto>> UpdateCustomer(int id, UpdateCustomerDto dto);
    Task<ApiResponse> DeleteCustomer(int id);
    Task<ApiResponse> ActivateCustomer(int id);
    Task<ApiResponse> AddLoyaltyPoints(int id, int points, string reason);
    Task<ApiResponse> DeductLoyaltyPoints(int id, int points, string reason);
    Task<ApiResponse> AdjustLoyaltyPoints(int id, int points, string reason);
    Task<ApiResponse<List<SaleSummaryDto>>> GetCustomerPurchaseHistory(int id);
    Task<ApiResponse> SendPromotionalEmail(int id);
}
