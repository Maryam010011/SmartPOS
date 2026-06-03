using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Suppliers;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for managing suppliers in the SmartPOS system.
    /// Implements CRUD operations with robust error handling.
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupplierService"/> class.
        /// </summary>
        /// <param name="factory">The application database context factory.</param>
        public SupplierService(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Retrieves a supplier by its unique identifier.
        /// </summary>
        /// <param name="id">The supplier ID.</param>
        /// <returns>An ApiResponse containing the SupplierDto if found.</returns>
        public async Task<ApiResponse<SupplierDto>> GetById(int id)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var supplier = await context.Suppliers.FindAsync(id);

                if (supplier == null)
                    return ApiResponse<SupplierDto>.Fail("Supplier not found.");

                return ApiResponse<SupplierDto>.Ok(MapToDto(supplier));
            }
            catch (Exception ex)
            {
                return ApiResponse<SupplierDto>.Fail($"Error retrieving supplier: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all suppliers from the database.
        /// </summary>
        /// <returns>An ApiResponse containing a list of SupplierDto objects.</returns>
        public async Task<ApiResponse<List<SupplierDto>>> GetAll()
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var suppliers = await context.Suppliers.ToListAsync();

                return ApiResponse<List<SupplierDto>>.Ok(suppliers.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SupplierDto>>.Fail($"Error retrieving suppliers: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new supplier in the system.
        /// </summary>
        /// <param name="dto">The supplier creation data transfer object.</param>
        /// <returns>An ApiResponse containing the created SupplierDto.</returns>
        public async Task<ApiResponse<SupplierDto>> Create(CreateSupplierDto dto)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return ApiResponse<SupplierDto>.Fail("Supplier name is required.");

                // Check for duplicate email if provided
                if (!string.IsNullOrWhiteSpace(dto.Email) &&
                    await context.Suppliers.AnyAsync(s => s.Email == dto.Email))
                    return ApiResponse<SupplierDto>.Fail("A supplier with this email already exists.");

                var supplier = new Supplier
                {
                    Name = dto.Name,
                    ContactPerson = dto.ContactPerson,
                    ContactNo = dto.ContactNo,
                    Email = dto.Email,
                    Address = dto.Address,
                    IsActive = true
                };

                context.Suppliers.Add(supplier);
                await context.SaveChangesAsync();

                return ApiResponse<SupplierDto>.Ok(MapToDto(supplier), "Supplier created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<SupplierDto>.Fail($"Error creating supplier: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing supplier's information.
        /// </summary>
        /// <param name="dto">The supplier data transfer object with updated values.</param>
        /// <returns>An ApiResponse containing the updated SupplierDto.</returns>
        public async Task<ApiResponse<SupplierDto>> Update(SupplierDto dto)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var supplier = await context.Suppliers.FindAsync(dto.Id);
                if (supplier == null)
                    return ApiResponse<SupplierDto>.Fail("Supplier not found.");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return ApiResponse<SupplierDto>.Fail("Supplier name is required.");

                // Check for duplicate email if changed
                if (!string.IsNullOrWhiteSpace(dto.Email) &&
                    supplier.Email != dto.Email &&
                    await context.Suppliers.AnyAsync(s => s.Email == dto.Email))
                    return ApiResponse<SupplierDto>.Fail("Another supplier with this email already exists.");

                supplier.Name = dto.Name;
                supplier.ContactPerson = dto.ContactPerson;
                supplier.ContactNo = dto.ContactNo;
                supplier.Email = dto.Email;
                supplier.Address = dto.Address;
                supplier.IsActive = dto.IsActive;

                await context.SaveChangesAsync();

                return ApiResponse<SupplierDto>.Ok(MapToDto(supplier), "Supplier updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<SupplierDto>.Fail($"Error updating supplier: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a supplier from the database.
        /// </summary>
        /// <param name="id">The supplier ID.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var supplier = await context.Suppliers
                    .Include(s => s.Products)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (supplier == null)
                    return ApiResponse.Fail("Supplier not found.");

                // Prevent deletion if supplier has associated products
                if (supplier.Products.Any())
                    return ApiResponse.Fail("Cannot delete supplier with associated products. Remove or reassign products first.");

                context.Suppliers.Remove(supplier);
                await context.SaveChangesAsync();

                return ApiResponse.Ok("Supplier deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Error deleting supplier: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps a Supplier entity to a SupplierDto.
        /// </summary>
        /// <param name="supplier">The Supplier entity.</param>
        /// <returns>A SupplierDto populated from the entity.</returns>
        private static SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                ContactNo = supplier.ContactNo,
                Email = supplier.Email,
                Address = supplier.Address,
                IsActive = supplier.IsActive
            };
        }
    }
}
