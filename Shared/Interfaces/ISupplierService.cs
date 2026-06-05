using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Suppliers;

namespace SmartPOS.Shared.Interfaces;

public interface ISupplierService
{
    Task<ApiResponse<SupplierDto>> GetById(int id);
    Task<ApiResponse<List<SupplierDto>>> GetAll();
    Task<ApiResponse<SupplierDto>> Create(CreateSupplierDto dto);
    Task<ApiResponse<SupplierDto>> Update(SupplierDto dto);
    Task<ApiResponse> Delete(int id);
}
