// ============================================================
//  SmartPOS+ — Shared Contracts
//  Interfaces, DTOs, Service Signatures
//  To be placed in: SmartPOS.Shared project / folder
//  ALL THREE MEMBERS MUST AGREE ON THIS BEFORE CODING
// ============================================================

// ─────────────────────────────────────────────────────────────
// SHARED RESPONSE WRAPPER
// Used by every service method as the return type
// ─────────────────────────────────────────────────────────────

namespace SmartPOS.Shared.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    // For operations that return no data (delete, update)
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Ok(string message = "Success")
            => new() { Success = true, Message = message };

        public static ApiResponse Fail(string message)
            => new() { Success = false, Message = message };
    }
}


// ─────────────────────────────────────────────────────────────
// ENUMS
// ─────────────────────────────────────────────────────────────

namespace SmartPOS.Shared.Enums
{
    public enum SaleType
    {
        Onsite,
        Online
    }

    public enum SaleStatus
    {
        Completed,
        Voided,
        Refunded
    }

    public enum POStatus
    {
        Draft,
        Sent,
        Received,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        Online
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum DiscountType
    {
        Percentage,
        Flat
    }

    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        Cashier = 3,
        Customer = 4
    }
}


// ============================================================
//  OWNER: MARYAM JAHANGIR
//  Modules: Auth, Users, Roles, Customers, Promotions
// ============================================================

namespace SmartPOS.Shared.DTOs.Auth
{
    using SmartPOS.Shared.DTOs.Users;
    // Login request
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Register request
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // Login response — JWT token returned
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = new();
    }
}

namespace SmartPOS.Shared.DTOs.Users
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserDetailDto : UserDto
    {
        public List<string> LoginHistory { get; set; } = new();
        public string AuditSummary { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserFilterDto
    {
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

namespace SmartPOS.Shared.DTOs.Customers
{
    using SmartPOS.Shared.Enums;

    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }
        public decimal TotalSpent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalOrders { get; set; }
    }

    public class CustomerDetailDto : CustomerDto
    {
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public List<SaleSummaryDto> RecentOrders { get; set; } = new();
        public List<LoyaltyHistoryEntryDto> LoyaltyHistory { get; set; } = new();
    }

    public class SaleSummaryDto
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; }
        public int ItemCount { get; set; }
    }

    public class LoyaltyHistoryEntryDto
    {
        public int Points { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateCustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class CustomerFilterDto
    {
        public bool? IsActive { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public DateTime? RegistrationDateFrom { get; set; }
        public DateTime? RegistrationDateTo { get; set; }
        public decimal? MinSpend { get; set; }
        public decimal? MaxSpend { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class LoyaltyAdjustDto
    {
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

namespace SmartPOS.Shared.DTOs.Promotions
{
    using SmartPOS.Shared.Enums;

    public class PromotionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public int MaxUsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePromotionDto
    {
        public string? Code { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public int MaxUsageLimit { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
    }

    public class UpdatePromotionDto
    {
        public string? Code { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? Value { get; set; }
        public decimal? MinOrderValue { get; set; }
        public int? MaxUsageLimit { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool? IsActive { get; set; }
        public string? Description { get; set; }
    }

    public class PromoValidationResult
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public DiscountType? DiscountType { get; set; }
        public int? PromotionId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class DailyUsageStat
    {
        public DateTime Date { get; set; }
        public int UsageCount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class PromoAnalyticsDto
    {
        public int TotalTimesUsed { get; set; }
        public decimal TotalDiscountGiven { get; set; }
        public decimal RevenueFromPromoOrders { get; set; }
        public List<DailyUsageStat> DailyUsageList { get; set; } = new();
    }

    public class ApplyPromoResultDto
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

// ─────────────────────────────────────────────────────────────
// SERVICE INTERFACES — MARYAM JAHANGIR
// ─────────────────────────────────────────────────────────────

namespace SmartPOS.Shared.Interfaces
{
    using SmartPOS.Shared.Common;
    using SmartPOS.Shared.DTOs.Auth;
    using SmartPOS.Shared.DTOs.Users;
    using SmartPOS.Shared.DTOs.Customers;
    using SmartPOS.Shared.DTOs.Promotions;

    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> Login(LoginRequestDto request);
        Task<ApiResponse> Logout(string token);
        Task<ApiResponse<LoginResponseDto>> RefreshToken(string refreshToken);
        Task<ApiResponse<LoginResponseDto>> Register(RegisterDto dto);
        Task<ApiResponse> ForgotPassword(string email);
        Task<ApiResponse> ResetPassword(string email, string code, string newPassword);
    }

    public interface IUserService
    {
        Task<ApiResponse<List<UserDto>>> GetAllUsers(UserFilterDto filter);
        Task<ApiResponse<UserDetailDto>> GetUserById(int id);
        Task<ApiResponse<UserDto>> CreateUser(CreateUserDto dto);
        Task<ApiResponse<UserDto>> UpdateUser(int id, UpdateUserDto dto);
        Task<ApiResponse> DeleteUser(int id);
        Task<ApiResponse> ActivateUser(int id);
        Task<ApiResponse> DeactivateUser(int id);
        Task<ApiResponse> BulkActivate(List<int> ids);
        Task<ApiResponse> BulkDeactivate(List<int> ids);
    }

    public interface ICustomerService
    {
        // Shahzain's Sales module calls these
        Task<ApiResponse<CustomerDto>> GetById(int id);
        Task<ApiResponse<CustomerDto>> GetByEmail(string email);
        Task<ApiResponse<CustomerDto>> CreateCustomer(CreateCustomerDto dto);
        Task<ApiResponse<CustomerDto>> UpdateCustomer(int id, UpdateCustomerDto dto);
        Task<ApiResponse> AddLoyaltyPoints(int customerId, int points, string reason);
        Task<ApiResponse> DeductLoyaltyPoints(int customerId, int points, string reason);

        // New methods
        Task<ApiResponse<List<CustomerDto>>> GetAllCustomers(CustomerFilterDto filter);
        Task<ApiResponse<CustomerDetailDto>> GetCustomerById(int id);
        Task<ApiResponse> DeleteCustomer(int id);
        Task<ApiResponse> ActivateCustomer(int id);
        Task<ApiResponse> AdjustLoyaltyPoints(int customerId, int adjustment, string reason);
        Task<ApiResponse<List<SaleSummaryDto>>> GetCustomerPurchaseHistory(int customerId);
        Task<ApiResponse> SendPromotionalEmail(int customerId);
    }

    public interface IPromotionService
    {
        // Shahzain's Sales module calls these (backward compatible)
        Task<ApiResponse<PromotionDto>> GetByCode(string code);
        Task<ApiResponse<ApplyPromoResultDto>> ValidateAndApply(string code, decimal orderTotal);
        Task<ApiResponse> IncrementUsage(int promoId);
        Task<ApiResponse<List<PromotionDto>>> GetAll();
        Task<ApiResponse<PromotionDto>> Create(PromotionDto dto);
        Task<ApiResponse<PromotionDto>> Update(PromotionDto dto);
        Task<ApiResponse> Delete(int id);

        // New methods for MaryamJ's Promotion module
        Task<ApiResponse<List<PromotionDto>>> GetAllPromotions();
        Task<ApiResponse<PromotionDto>> GetById(int id);
        Task<ApiResponse<PromotionDto>> CreatePromotion(CreatePromotionDto dto);
        Task<ApiResponse<PromotionDto>> UpdatePromotion(int id, UpdatePromotionDto dto);
        Task<ApiResponse> DeletePromotion(int id);
        Task<ApiResponse> ToggleActive(int id);
        Task<ApiResponse<PromoValidationResult>> ValidatePromoCode(string code, decimal orderTotal);
         Task<ApiResponse> ApplyPromoCode(int promoId);
         Task<ApiResponse<PromoAnalyticsDto>> GetPromoAnalytics(int id);
         Task<ApiResponse<List<PromotionDto>>> GetActivePromotions();
     }
 }


// ============================================================
//  OWNER: SHAHZAIN NADEEM
//  Modules: Products, Categories, Suppliers, Sales, AI, FBR
// ============================================================

namespace SmartPOS.Shared.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public string ImageURL { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public string ImageURL { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }
    }

    public class UpdateProductDto : CreateProductDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}

namespace SmartPOS.Shared.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; } = string.Empty;
        public List<CategoryDto> SubCategories { get; set; } = new();
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryDto : CreateCategoryDto
    {
        public int Id { get; set; }
    }
}

namespace SmartPOS.Shared.DTOs.Suppliers
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}

namespace SmartPOS.Shared.DTOs.Sales
{
    using SmartPOS.Shared.Enums;

    public class SaleItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class CreateSaleDto
    {
        public int? CustomerId { get; set; }
        public int UserId { get; set; }
        public string? PromoCode { get; set; }
        public SaleType SaleType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
    }

    public class SaleResultDto
    {
        public int SaleId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; }
        public DateTime SaleDate { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public List<SaleItemDto> Items { get; set; } = new();
    }

    public class SaleFilterDto
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Period { get; set; } = string.Empty; // "daily" "weekly" "monthly"
        public int? UserId { get; set; }
        public SaleType? SaleType { get; set; }
        public SaleStatus? Status { get; set; }
    }

    public class SaleAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<SaleChartPointDto> ChartData { get; set; } = new();
    }

    public class SaleChartPointDto
    {
        public string Label { get; set; } = string.Empty; // date or week or month
        public decimal Revenue { get; set; }
        public int Transactions { get; set; }
    }
}

namespace SmartPOS.Shared.DTOs.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; } = string.Empty;
        public string Sentiment { get; set; } = string.Empty; // Positive Negative Neutral
        public float SentimentScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewDto
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}

// ─────────────────────────────────────────────────────────────
// SERVICE INTERFACES — SHAHZAIN
// ─────────────────────────────────────────────────────────────

namespace SmartPOS.Shared.Interfaces
{
    using SmartPOS.Shared.Common;
    using SmartPOS.Shared.DTOs.Products;
    using SmartPOS.Shared.DTOs.Categories;
    using SmartPOS.Shared.DTOs.Suppliers;
    using SmartPOS.Shared.DTOs.Sales;
    using SmartPOS.Shared.DTOs.Reviews;

    public interface IProductService
    {
        // Maryam Yaqoob's Inventory module calls these
        Task<ApiResponse<ProductDto>> GetById(int id);
        Task<ApiResponse<List<ProductDto>>> GetAll();
        Task<ApiResponse<List<ProductDto>>> GetByCategory(int categoryId);
        Task<ApiResponse<List<ProductDto>>> GetBySupplier(int supplierId);
        Task<ApiResponse<ProductDto>> Create(CreateProductDto dto);
        Task<ApiResponse<ProductDto>> Update(UpdateProductDto dto);
        Task<ApiResponse> Delete(int id);
        Task<ApiResponse> ToggleActive(int id);
        Task<ApiResponse<List<ProductDto>>> Search(string keyword);
    }

    public interface ICategoryService
    {
        Task<ApiResponse<CategoryDto>> GetById(int id);
        Task<ApiResponse<List<CategoryDto>>> GetAll();
        Task<ApiResponse<List<CategoryDto>>> GetTree(); // returns nested tree structure
        Task<ApiResponse<CategoryDto>> Create(CreateCategoryDto dto);
        Task<ApiResponse<CategoryDto>> Update(UpdateCategoryDto dto);
        Task<ApiResponse> Delete(int id);
    }

    public interface ISupplierService
    {
        // Maryam Yaqoob's PO module calls these
        Task<ApiResponse<SupplierDto>> GetById(int id);
        Task<ApiResponse<List<SupplierDto>>> GetAll();
        Task<ApiResponse<SupplierDto>> Create(CreateSupplierDto dto);
        Task<ApiResponse<SupplierDto>> Update(SupplierDto dto);
        Task<ApiResponse> Delete(int id);
    }

    public interface ISaleService
    {
        Task<ApiResponse<SaleResultDto>> ProcessSale(CreateSaleDto dto);
        Task<ApiResponse<SaleResultDto>> GetById(int id);
        Task<ApiResponse<List<SaleResultDto>>> GetAll(SaleFilterDto filter);
        Task<ApiResponse<SaleAnalyticsDto>> GetAnalytics(SaleFilterDto filter);
        Task<ApiResponse> VoidSale(int saleId, string reason);
        Task<ApiResponse> RefundSale(int saleId);
    }

    public interface IReviewService
    {
        Task<ApiResponse<ReviewDto>> Create(CreateReviewDto dto);
        Task<ApiResponse<List<ReviewDto>>> GetByProduct(int productId);
        Task<ApiResponse<List<ReviewDto>>> GetAll();
        Task<ApiResponse> Delete(int id);
    }

    public interface IAIChatbotService
    {
        Task<ApiResponse<string>> GetInsight(string query);
        Task<ApiResponse<SaleAnalyticsDto>> AnalyzeSales(SaleFilterDto filter);
        Task<ApiResponse<string>> ForecastDemand(int productId);
    }

    public interface IFBRService
    {
        Task<ApiResponse<decimal>> CalculateTax(decimal amount);
        Task<ApiResponse<string>> SubmitInvoice(SaleResultDto sale);
        Task<ApiResponse<string>> GetTaxStatus(string invoiceRef);
    }

    public interface IBERTService
    {
        // Called after a review is created
        Task<ApiResponse<string>> AnalyzeSentiment(string reviewText);
        // Returns aggregated sentiment stats for admin charts
        Task<ApiResponse<List<SentimentStatDto>>> GetSentimentStats(int? productId = null);
    }
}

namespace SmartPOS.Shared.DTOs.Reviews
{
    public class SentimentStatDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalReviews { get; set; }
        public int Positive { get; set; }
        public int Neutral { get; set; }
        public int Negative { get; set; }
        public double AverageRating { get; set; }
    }
}


// ============================================================
//  OWNER: MARYAM YAQOOB
//  Modules: Inventory, Purchase Orders, Weather API
// ============================================================

namespace SmartPOS.Shared.DTOs.Inventory
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLowStock => Quantity <= ReorderLevel;
    }

    public class AdjustStockDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class AddStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class DeductStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

namespace SmartPOS.Shared.DTOs.PurchaseOrders
{
    using SmartPOS.Shared.Enums;

    public class POLineItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OrderedQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => OrderedQty * UnitPrice;
    }

    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public POStatus Status { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<POLineItemDto> LineItems { get; set; } = new();
    }

    public class CreatePODto
    {
        public int SupplierId { get; set; }
        public int UserId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<POLineItemDto> LineItems { get; set; } = new();
    }

    public class ReceiveRequest
    {
        public int UserId { get; set; }
    }
}

namespace SmartPOS.Shared.DTOs.Weather
{
    public class WeatherDto
    {
        public string Condition { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string City { get; set; } = string.Empty;
        public DateTime FetchedAt { get; set; }
    }
}

// ─────────────────────────────────────────────────────────────
// SERVICE INTERFACES — MARYAM YAQOOB
// ─────────────────────────────────────────────────────────────

namespace SmartPOS.Shared.Interfaces
{
    using SmartPOS.Shared.Common;
    using SmartPOS.Shared.DTOs.Inventory;
    using SmartPOS.Shared.DTOs.PurchaseOrders;
    using SmartPOS.Shared.DTOs.Weather;

    public interface IInventoryService
    {
        // Shahzain's Sales module calls DeductStock after every sale
        Task<ApiResponse> DeductStock(int productId, int quantity);
        Task<ApiResponse> AddStock(int productId, int quantity);
        Task<ApiResponse> AdjustStock(AdjustStockDto dto);
        Task<ApiResponse<InventoryDto>> GetByProduct(int productId);
        Task<ApiResponse<List<InventoryDto>>> GetAll();
        Task<ApiResponse<List<InventoryDto>>> GetLowStock();
    }

    public interface IPurchaseOrderService
    {
        Task<ApiResponse<PurchaseOrderDto>> GetById(int id);
        Task<ApiResponse<List<PurchaseOrderDto>>> GetAll();
        Task<ApiResponse<PurchaseOrderDto>> Create(CreatePODto dto);
        Task<ApiResponse> MarkAsReceived(int poId, int userId);
        Task<ApiResponse> Cancel(int poId);
    }

    public interface IWeatherService
    {
        Task<ApiResponse<WeatherDto>> GetCurrentWeather(string city);
    }
}


// ============================================================
//  ROLE MANAGEMENT — MARYAM JAHANGIR
//  DTOs and Interface for Role CRUD & Permissions
// ============================================================

namespace SmartPOS.Shared.DTOs.Roles
{
    public class PermissionsDto
    {
        public Dictionary<string, ModulePermission> Modules { get; set; } = new();
    }

    public class ModulePermission
    {
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }

    public class RoleDetailDto : RoleDto
    {
        public PermissionsDto Permissions { get; set; } = new();
    }

    public class CreateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public PermissionsDto Permissions { get; set; } = new();
    }

    public class UpdateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public PermissionsDto Permissions { get; set; } = new();
    }
}

// ─────────────────────────────────────────────────────────────
// SERVICE INTERFACE — ROLE MANAGEMENT (MARYAM JAHANGIR)
// ─────────────────────────────────────────────────────────────

namespace SmartPOS.Shared.Interfaces
{
    using SmartPOS.Shared.Common;
    using SmartPOS.Shared.DTOs.Roles;

    public interface IRoleService
    {
        Task<ApiResponse<List<RoleDto>>> GetAllRoles();
        Task<ApiResponse<RoleDetailDto>> GetRoleById(int id);
        Task<ApiResponse<RoleDto>> CreateRole(CreateRoleDto dto);
        Task<ApiResponse<RoleDto>> UpdateRole(int id, UpdateRoleDto dto);
        Task<ApiResponse> DeleteRole(int id);
        Task<ApiResponse<PermissionsDto>> GetRolePermissions(int id);
        Task<ApiResponse> UpdateRolePermissions(int id, PermissionsDto permissions);
    }
}