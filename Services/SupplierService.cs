using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Suppliers;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class SupplierService : ISupplierService
    {
        private static readonly List<SupplierDto> MockSuppliers = new()
        {
            new SupplierDto { Id = 1, Name = "TechWorld Distributors", ContactPerson = "Ali Raza", ContactNo = "0300-1234567", Email = "ali@techworld.com", Address = "123 Main Street, Lahore", IsActive = true },
            new SupplierDto { Id = 2, Name = "Fresh Foods Supply Co.", ContactPerson = "Sara Khan", ContactNo = "0301-7654321", Email = "sara@freshfoods.com", Address = "456 Food Street, Karachi", IsActive = true },
            new SupplierDto { Id = 3, Name = "Office Essentials Ltd.", ContactPerson = "Bilal Ahmed", ContactNo = "0302-9876543", Email = "bilal@officeessentials.com", Address = "789 Office Road, Islamabad", IsActive = true },
            new SupplierDto { Id = 4, Name = "Global Traders Inc.", ContactPerson = "Fatima Noor", ContactNo = "0303-1112223", Email = "fatima@globaltraders.com", Address = "321 Trade Avenue, Lahore", IsActive = false }
        };
        private static int _nextId = 5;

        public Task<ApiResponse<List<SupplierDto>>> GetAll()
        {
            return Task.FromResult(ApiResponse<List<SupplierDto>>.Ok(MockSuppliers.ToList()));
        }

        public Task<ApiResponse<SupplierDto>> GetById(int id)
        {
            var supplier = MockSuppliers.FirstOrDefault(s => s.Id == id);
            if (supplier == null)
                return Task.FromResult(ApiResponse<SupplierDto>.Fail("Supplier not found."));
            return Task.FromResult(ApiResponse<SupplierDto>.Ok(supplier));
        }

        public Task<ApiResponse<SupplierDto>> Create(CreateSupplierDto dto)
        {
            var supplier = new SupplierDto
            {
                Id = _nextId++,
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                ContactNo = dto.ContactNo,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = true
            };
            MockSuppliers.Add(supplier);
            return Task.FromResult(ApiResponse<SupplierDto>.Ok(supplier));
        }

        public Task<ApiResponse<SupplierDto>> Update(SupplierDto dto)
        {
            var existing = MockSuppliers.FirstOrDefault(s => s.Id == dto.Id);
            if (existing == null)
                return Task.FromResult(ApiResponse<SupplierDto>.Fail("Supplier not found."));
            existing.Name = dto.Name;
            existing.ContactPerson = dto.ContactPerson;
            existing.ContactNo = dto.ContactNo;
            existing.Email = dto.Email;
            existing.Address = dto.Address;
            existing.IsActive = dto.IsActive;
            return Task.FromResult(ApiResponse<SupplierDto>.Ok(existing));
        }

        public Task<ApiResponse> Delete(int id)
        {
            var supplier = MockSuppliers.FirstOrDefault(s => s.Id == id);
            if (supplier == null)
                return Task.FromResult(ApiResponse.Fail("Supplier not found."));
            supplier.IsActive = false;
            return Task.FromResult(ApiResponse.Ok("Supplier deactivated."));
        }
    }
}
