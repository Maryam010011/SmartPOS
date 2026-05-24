using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Customers;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Services.MaryamJ;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<CustomerDto>>> GetAllCustomers(CustomerFilterDto filter)
    {
        try
        {
            var query = _context.Customers.AsQueryable();

            if (filter.IsActive.HasValue)
                query = query.Where(c => c.IsActive == filter.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    c.Email.ToLower().Contains(term) ||
                    (c.Phone != null && c.Phone.Contains(term)));
            }

            if (filter.RegistrationDateFrom.HasValue)
                query = query.Where(c => c.CreatedAt >= filter.RegistrationDateFrom.Value);

            if (filter.RegistrationDateTo.HasValue)
                query = query.Where(c => c.CreatedAt <= filter.RegistrationDateTo.Value);

            if (filter.MinSpend.HasValue)
                query = query.Where(c => c.TotalSpent >= filter.MinSpend.Value);

            if (filter.MaxSpend.HasValue)
                query = query.Where(c => c.TotalSpent <= filter.MaxSpend.Value);

            query = query.OrderByDescending(c => c.CreatedAt);

            int page = filter.Page > 0 ? filter.Page : 1;
            int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;

            var customers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone ?? string.Empty,
                    LoyaltyPoints = c.LoyaltyPoints,
                    TotalSpent = c.TotalSpent,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<CustomerDto>>.Ok(customers);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CustomerDto>>.Fail($"Failed to retrieve customers: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDto>> GetById(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return ApiResponse<CustomerDto>.Fail("Customer not found.");

            return ApiResponse<CustomerDto>.Ok(MapToDto(customer));
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDto>.Fail($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDetailDto>> GetCustomerById(int id)
    {
        try
        {
            var customer = await _context.Customers
                .Include(c => c.Sales)
                    .ThenInclude(s => s.SaleItems)
                .Include(c => c.LoyaltyTransactions.OrderByDescending(lt => lt.CreatedAt).Take(50))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return ApiResponse<CustomerDetailDto>.Fail("Customer not found.");

            var detail = new CustomerDetailDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone ?? string.Empty,
                DateOfBirth = customer.DateOfBirth,
                Address = customer.Address,
                LoyaltyPoints = customer.LoyaltyPoints,
                TotalSpent = customer.TotalSpent,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                RecentOrders = (customer.Sales ?? new List<Sale>())
                    .OrderByDescending(s => s.SaleDate)
                    .Take(20)
                    .Select(s => new SaleSummaryDto
                    {
                        Id = s.Id,
                        SaleDate = s.SaleDate,
                        TotalAmount = s.TotalAmount,
                        Status = s.Status,
                        ItemCount = s.SaleItems?.Count ?? 0
                    })
                    .ToList(),
                LoyaltyHistory = (customer.LoyaltyTransactions ?? new List<LoyaltyTransaction>())
                    .OrderByDescending(lt => lt.CreatedAt)
                    .Select(lt => new LoyaltyHistoryEntryDto
                    {
                        Points = lt.Points,
                        Type = lt.Type,
                        Reason = lt.Reason,
                        CreatedAt = lt.CreatedAt
                    })
                    .ToList()
            };

            return ApiResponse<CustomerDetailDto>.Ok(detail);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDetailDto>.Fail($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDto>> GetByEmail(string email)
    {
        try
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.Trim().ToLower());

            if (customer == null)
                return ApiResponse<CustomerDto>.Fail("Customer not found.");

            return ApiResponse<CustomerDto>.Ok(MapToDto(customer));
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDto>.Fail($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDto>> Create(CreateCustomerDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email))
                return ApiResponse<CustomerDto>.Fail("Name and Email are required.");

            var emailExists = await _context.Customers
                .AnyAsync(c => c.Email.ToLower() == dto.Email.Trim().ToLower());

            if (emailExists)
                return ApiResponse<CustomerDto>.Fail("A customer with this email already exists.");

            var customer = new Customer
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone?.Trim(),
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address?.Trim(),
                LoyaltyPoints = 0,
                TotalSpent = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return ApiResponse<CustomerDto>.Ok(MapToDto(customer), "Customer created successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDto>.Fail($"Failed to create customer: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDto>> Update(int id, UpdateCustomerDto dto)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return ApiResponse<CustomerDto>.Fail("Customer not found.");

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email))
                return ApiResponse<CustomerDto>.Fail("Name and Email are required.");

            var cleanedEmail = dto.Email.Trim().ToLower();
            if (customer.Email.ToLower() != cleanedEmail)
            {
                var emailExists = await _context.Customers
                    .AnyAsync(c => c.Email.ToLower() == cleanedEmail && c.Id != id);

                if (emailExists)
                    return ApiResponse<CustomerDto>.Fail("Email is already in use by another customer.");
            }

            customer.Name = dto.Name.Trim();
            customer.Email = dto.Email.Trim();
            customer.Phone = dto.Phone?.Trim();
            customer.DateOfBirth = dto.DateOfBirth;
            customer.Address = dto.Address?.Trim();
            customer.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return ApiResponse<CustomerDto>.Ok(MapToDto(customer), "Customer updated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDto>.Fail($"Failed to update customer: {ex.Message}");
        }
    }

    public async Task<ApiResponse> DeleteCustomer(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return ApiResponse.Fail("Customer not found.");

            customer.IsActive = false;
            await _context.SaveChangesAsync();

            return ApiResponse.Ok("Customer deactivated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Failed to delete customer: {ex.Message}");
        }
    }

    public async Task<ApiResponse> AddLoyaltyPoints(int customerId, int points)
    {
        return await AddLoyaltyPointsWithReason(customerId, points, "Purchase earned points");
    }

    public async Task<ApiResponse> DeductLoyaltyPoints(int customerId, int points)
    {
        return await AddLoyaltyPointsWithReason(customerId, -points, "Points redeemed at checkout");
    }

    public async Task<ApiResponse> AdjustLoyaltyPoints(int customerId, int adjustment, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return ApiResponse.Fail("Reason is required for manual adjustment.");

        return await AddLoyaltyPointsWithReason(customerId, adjustment, reason);
    }

    private async Task<ApiResponse> AddLoyaltyPointsWithReason(int customerId, int points, string reason)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return ApiResponse.Fail("Customer not found.");

            if (!customer.IsActive)
                return ApiResponse.Fail("Cannot modify loyalty points for an inactive customer.");

            if (customer.LoyaltyPoints + points < 0)
                return ApiResponse.Fail("Insufficient loyalty points.");

            customer.LoyaltyPoints += points;

            var type = points >= 0 ? "Earned" : "Redeemed";

            var transaction = new LoyaltyTransaction
            {
                CustomerId = customerId,
                Points = points,
                Type = type,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };

            _context.LoyaltyTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            var msg = points >= 0
                ? $"{points} points added. New balance: {customer.LoyaltyPoints}"
                : $"{Math.Abs(points)} points deducted. New balance: {customer.LoyaltyPoints}";

            return ApiResponse.Ok(msg);
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Failed to update loyalty points: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SaleSummaryDto>>> GetCustomerPurchaseHistory(int customerId)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return ApiResponse<List<SaleSummaryDto>>.Fail("Customer not found.");

            var sales = await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.CustomerId == customerId)
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new SaleSummaryDto
                {
                    Id = s.Id,
                    SaleDate = s.SaleDate,
                    TotalAmount = s.TotalAmount,
                    Status = s.Status,
                    ItemCount = s.SaleItems.Count
                })
                .ToListAsync();

            return ApiResponse<List<SaleSummaryDto>>.Ok(sales);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SaleSummaryDto>>.Fail($"Error: {ex.Message}");
        }
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone ?? string.Empty,
            LoyaltyPoints = customer.LoyaltyPoints,
            TotalSpent = customer.TotalSpent,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt
        };
    }
}
