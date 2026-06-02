# Implementation Plan: SmartPOS+ Database Schema

## Overview

Implement the complete SmartPOS+ database schema by creating shared enums, replacing 16 model classes, removing 4 legacy models, fully rewriting `AppDbContext`, clearing old migrations, and generating a clean `FullDatabaseSetup` migration. All code is C# targeting .NET 10 / EF Core with SQL Server.

---

## Tasks

- [x] 1. Create shared enums file (`Contracts.cs`)
  - [x] 1.1 Create `Contracts.cs` at the project root (`SmartPOS/Contracts.cs`) in namespace `SmartPOS.Shared.Enums`
    - Define `DiscountType` enum: `Percentage = 0`, `Flat = 1`
    - Define `SaleType` enum: `Onsite = 0`, `Online = 1`
    - Define `SaleStatus` enum: `Completed = 0`, `Voided = 1`, `Refunded = 2`
    - Define `POStatus` enum: `Draft = 0`, `Sent = 1`, `Received = 2`, `Cancelled = 3`
    - Define `PaymentMethod` enum: `Cash = 0`, `Card = 1`, `Online = 2`
    - Define `PaymentStatus` enum: `Pending = 0`, `Completed = 1`, `Failed = 2`, `Refunded = 3`
    - _Requirements: 1.1, 1.2_

- [x] 2. Remove legacy audit log model files
  - [x] 2.1 Delete `SmartPOS/Models/AdminLog.cs`
    - This file is replaced by the unified `AuditLog` model
    - _Requirements: 6.7_
  - [x] 2.2 Delete `SmartPOS/Models/CashierLog.cs`
    - _Requirements: 6.7_
  - [x] 2.3 Delete `SmartPOS/Models/CustomerLog.cs`
    - _Requirements: 6.7_
  - [x] 2.4 Delete `SmartPOS/Models/ManagerAction.cs`
    - _Requirements: 6.7_

- [x] 3. Replace foundation model files (Role, Permission, User)
  - [x] 3.1 Rewrite `SmartPOS/Models/Role.cs`
    - `partial class Role` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `string Name`
    - Navigation: `virtual ICollection<User> Users`, `virtual ICollection<Permission> Permissions` (both initialized to `new List<T>()`)
    - _Requirements: 2.1, 2.2, 2.3, 2.4_
  - [x] 3.2 Rewrite `SmartPOS/Models/Permission.cs`
    - `partial class Permission` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `int RoleId`, `string Module`, `bool CanCreate`, `bool CanRead`, `bool CanUpdate`, `bool CanDelete`
    - Navigation: `virtual Role Role = null!`
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_
  - [x] 3.3 Rewrite `SmartPOS/Models/User.cs`
    - `partial class User` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `string Name`, `string Email`, `string PasswordHash`, `int RoleId`, `bool IsActive`, `DateTime CreatedAt`
    - Navigation: `virtual Role Role = null!`, `virtual Customer? Customer`, `virtual ICollection<Sale> Sales`, `virtual ICollection<PurchaseOrder> PurchaseOrders`, `virtual ICollection<AuditLog> AuditLogs`
    - Remove all legacy log collection navigations (`AdminLogs`, `CashierLogs`, `CustomerLogs`, `ManagerActions`)
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6_

- [x] 4. Create AuditLog model and replace Customer model
  - [x] 4.1 Create `SmartPOS/Models/AuditLog.cs`
    - `partial class AuditLog` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `int UserId`, `string Action`, `string Module`, `DateTime Timestamp`, `string? IPAddress`, `string? Details`
    - Navigation: `virtual User User = null!`
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_
  - [x] 4.2 Rewrite `SmartPOS/Models/Customer.cs`
    - `partial class Customer` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `int UserId`, `string Name`, `string Email`, `string? Phone`, `DateOnly? DateOfBirth`, `string? Address`, `int LoyaltyPoints`, `decimal TotalSpent`, `DateTime CreatedAt`
    - Navigation: `virtual User User = null!`, `virtual ICollection<Sale> Sales`, `virtual ICollection<Review> Reviews`
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7_

- [x] 5. Create Promotion and Category models
  - [x] 5.1 Rewrite `SmartPOS/Models/Promotion.cs`
    - `partial class Promotion` in `SmartPOS.Models` namespace
    - Add `using SmartPOS.Shared.Enums;`
    - Properties: `int Id`, `string Code`, `DiscountType DiscountType`, `decimal Value`, `decimal MinOrderValue`, `int? MaxUsageLimit`, `int UsageCount`, `DateOnly ValidFrom`, `DateOnly ValidTo`, `bool IsActive`
    - Navigation: `virtual ICollection<Sale> Sales`
    - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7_
  - [x] 5.2 Create `SmartPOS/Models/Category.cs`
    - `partial class Category` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `string Name`, `int? ParentCategoryId`, `string? Description`, `string? ImageURL`
    - Navigation: `virtual Category? ParentCategory`, `virtual ICollection<Category> SubCategories`, `virtual ICollection<Product> Products`
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 6. Create Supplier and Product models
  - [x] 6.1 Create `SmartPOS/Models/Supplier.cs`
    - `partial class Supplier` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `string Name`, `string? ContactPerson`, `string? ContactNo`, `string? Email`, `string? Address`, `bool IsActive`
    - Navigation: `virtual ICollection<Product> Products`, `virtual ICollection<PurchaseOrder> PurchaseOrders`
    - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_
  - [x] 6.2 Create `SmartPOS/Models/Product.cs`
    - `partial class Product` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `string Name`, `string SKU`, `string? Description`, `string? ImageURL`, `decimal Price`, `decimal CostPrice`, `bool IsActive`, `int CategoryId`, `int? SupplierId`, `DateTime CreatedAt`
    - Navigation: `virtual Category Category = null!`, `virtual Supplier? Supplier`, `virtual Inventory? Inventory`, `virtual ICollection<SaleItem> SaleItems`, `virtual ICollection<POLineItem> POLineItems`, `virtual ICollection<Review> Reviews`
    - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8_

- [x] 7. Create Sale and SaleItem models
  - [x] 7.1 Create `SmartPOS/Models/Sale.cs`
    - `partial class Sale` in `SmartPOS.Models` namespace
    - Add `using SmartPOS.Shared.Enums;`
    - Properties: `int Id`, `int? CustomerId`, `int UserId`, `int? PromoId`, `SaleType SaleType`, `decimal TotalAmount`, `decimal DiscountAmount`, `decimal TaxAmount`, `DateTime SaleDate`, `SaleStatus Status`
    - Navigation: `virtual Customer? Customer`, `virtual User User = null!`, `virtual Promotion? Promotion`, `virtual ICollection<SaleItem> SaleItems`, `virtual Payment? Payment`
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6, 11.7_
  - [x] 7.2 Create `SmartPOS/Models/SaleItem.cs`
    - `partial class SaleItem` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `int SaleId`, `int ProductId`, `int Quantity`, `decimal UnitPrice`, `decimal LineTotal`
    - Navigation: `virtual Sale Sale = null!`, `virtual Product Product = null!`
    - _Requirements: 12.1, 12.2, 12.3, 12.4, 12.5_
  - [ ]* 7.3 Write property test for SaleItem LineTotal invariant
    - **Property 1: SaleItem LineTotal Invariant**
    - **Validates: Requirements 12.4**
    - Use FsCheck or CsCheck; generate random positive `int` for `Quantity` and random non-negative `decimal` for `UnitPrice`
    - Assert `item.LineTotal == item.Quantity * item.UnitPrice` for all generated inputs
    - Minimum 100 iterations
    - Tag: `Feature: smartpos-database-schema, Property 1: SaleItem LineTotal Invariant`

- [ ] 8. Create Review, Inventory, PurchaseOrder, POLineItem, and Payment models
  - [ ] 8.1 Create `SmartPOS/Models/Review.cs`
    - `partial class Review` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `int CustomerId`, `int ProductId`, `int Rating`, `string? Comment`, `string? Sentiment`, `double? SentimentScore`, `DateTime CreatedAt`
    - Navigation: `virtual Customer Customer = null!`, `virtual Product Product = null!`
    - _Requirements: 13.1, 13.2, 13.3, 13.4, 13.5, 13.6_
  - [ ]* 8.2 Write property test for Review Rating bounds
    - **Property 2: Review Rating Bounds**
    - **Validates: Requirements 13.3, 18.7**
    - Generate random `int` outside [1, 5] for invalid cases; random `int` in [1, 5] for valid cases
    - Assert invalid ratings are rejected by the CHECK constraint (throws on DB insert)
    - Assert valid ratings round-trip correctly (insert succeeds, read-back preserves value)
    - Minimum 100 iterations
    - Tag: `Feature: smartpos-database-schema, Property 2: Review Rating Bounds`
  - [x] 8.3 Create `SmartPOS/Models/Inventory.cs`
    - `partial class Inventory` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `int ProductId`, `int Quantity`, `int ReorderLevel`, `DateTime LastUpdated`
    - Navigation: `virtual Product Product = null!`
    - _Requirements: 14.1, 14.2, 14.3, 14.4, 14.5_
  - [x] 8.4 Create `SmartPOS/Models/PurchaseOrder.cs`
    - `partial class PurchaseOrder` in `SmartPOS.Models` namespace
    - Add `using SmartPOS.Shared.Enums;`
    - Properties: `int Id`, `int SupplierId`, `int UserId`, `POStatus Status`, `decimal TotalCost`, `DateTime OrderDate`, `DateTime? ReceivedAt`, `string? Notes`
    - Navigation: `virtual Supplier Supplier = null!`, `virtual User User = null!`, `virtual ICollection<POLineItem> LineItems`
    - _Requirements: 15.1, 15.2, 15.3, 15.4, 15.5_
  - [x] 8.5 Create `SmartPOS/Models/POLineItem.cs`
    - `partial class POLineItem` in `SmartPOS.Models` namespace
    - Properties: `int Id`, `int POID`, `int ProductId`, `int OrderedQty`, `decimal UnitPrice`
    - Navigation: `virtual PurchaseOrder PurchaseOrder = null!`, `virtual Product Product = null!`
    - _Requirements: 16.1, 16.2, 16.3, 16.4, 16.5_
  - [x] 8.6 Create `SmartPOS/Models/Payment.cs`
    - `partial class Payment` in `SmartPOS.Models` namespace
    - Add `using SmartPOS.Shared.Enums;`
    - Properties: `int Id`, `int SaleId`, `PaymentMethod Method`, `decimal Amount`, `PaymentStatus Status`, `string? TransactionRef`, `DateTime? PaidAt`
    - Navigation: `virtual Sale Sale = null!`
    - _Requirements: 17.1, 17.2, 17.3, 17.4, 17.5, 17.6, 17.7_

- [x] 9. Checkpoint — Verify project compiles
  - Ensure all model files compile without errors before touching `AppDbContext`
  - Run `dotnet build` from the project root and resolve any compilation errors
  - Ensure all tests pass, ask the user if questions arise.

- [x] 10. Replace `AppDbContext.cs` with full Fluent API configuration
  - [x] 10.1 Rewrite `SmartPOS/Data/AppDbContext.cs` — DbSets and constructor
    - Remove the parameterless constructor and the `OnConfiguring` override entirely
    - Keep only `AppDbContext(DbContextOptions<AppDbContext> options)` constructor
    - Add `virtual DbSet<T>` properties for all 16 tables: `Roles`, `Permissions`, `Users`, `Customers`, `AuditLogs`, `Promotions`, `Categories`, `Suppliers`, `Products`, `Sales`, `SaleItems`, `Reviews`, `Inventories`, `PurchaseOrders`, `POLineItems`, `Payments`
    - Remove `DbSet` properties for `AdminLogs`, `CashierLogs`, `CustomerLogs`, `ManagerActions`
    - _Requirements: 18.1, 18.2, 6.6_
  - [x] 10.2 Add `OnModelCreating` — Role, Permission, User, Customer, AuditLog configuration
    - Role: `Name` → `HasMaxLength(50)`, `IsRequired()`
    - Permission: index on `RoleId`; `Module` → `HasMaxLength(100)`, `IsRequired()`; all bool flags → `HasDefaultValue(false)`; FK to Role
    - User: unique index on `Email`; index on `RoleId`; `Name`/`Email`/`PasswordHash` max lengths; `IsActive` default `true`; `CreatedAt` default `getutcdate()`; FK to Role
    - Customer: unique index on `UserId` and `Email`; `Name`/`Email`/`Phone` max lengths; `Address` as `text`; `LoyaltyPoints` default `0`; `TotalSpent` precision `(10,2)` default `0.00m`; `CreatedAt` default `getutcdate()`; one-to-one with User via `UserId`
    - AuditLog: index on `UserId`; `Action`/`Module` max lengths; `Timestamp` default `getutcdate()`; `IPAddress` max length 45; `Details` as `text`; FK to User
    - _Requirements: 18.3, 18.10, 18.11, 18.12_
  - [x] 10.3 Add `OnModelCreating` — Promotion, Category, Supplier, Product configuration
    - Promotion: unique index on `Code`; `Code` max length 50; `Value`/`MinOrderValue` precision `(10,2)`; `MinOrderValue` default `0.00m`; `UsageCount` default `0`; `IsActive` default `true`
    - Category: `Name` max length 100; `Description` as `text`; `ImageURL` max length 255; self-ref FK `ParentCategoryId` → `OnDelete(DeleteBehavior.NoAction)`
    - Supplier: unique index on `Email`; `Name`/`ContactPerson`/`ContactNo`/`Email` max lengths; `Address` as `text`; `IsActive` default `true`
    - Product: unique index on `SKU`; `Name`/`SKU`/`ImageURL` max lengths; `Description` as `text`; `Price`/`CostPrice` precision `(10,2)`; `IsActive` default `true`; `CreatedAt` default `getutcdate()`; Category FK → `Restrict`; Supplier FK → `IsRequired(false)`, `SetNull`
    - _Requirements: 18.3, 18.4, 18.11, 18.12_
  - [x] 10.4 Add `OnModelCreating` — Sale, SaleItem, Review, Inventory, PurchaseOrder, POLineItem, Payment configuration
    - Sale: `TotalAmount`/`DiscountAmount`/`TaxAmount` precision `(10,2)`; `DiscountAmount` default `0.00m`; `SaleDate` default `getutcdate()`; `Status` default `SaleStatus.Completed`; `CustomerId`/`PromoId` → `IsRequired(false)`
    - SaleItem: index on `SaleId`; `UnitPrice`/`LineTotal` precision `(10,2)`; Sale FK → `Cascade`; Product FK → `Restrict`
    - Review: CHECK constraint `CK_Review_Rating` `[Rating] >= 1 AND [Rating] <= 5`; `Comment` as `text`; `Sentiment` max length 20; `CreatedAt` default `getutcdate()`
    - Inventory: unique index on `ProductId`; `Quantity` default `0`; `LastUpdated` default `getutcdate()`; one-to-one with Product via `ProductId`
    - PurchaseOrder: `TotalCost` precision `(10,2)`; `OrderDate` default `getutcdate()`; `Status` default `POStatus.Draft`; `Notes` as `text`
    - POLineItem: index on `POID`; `UnitPrice` precision `(10,2)`; PurchaseOrder FK → `Cascade`
    - Payment: unique index on `SaleId`; `Amount` precision `(10,2)`; `Status` default `PaymentStatus.Pending`; `TransactionRef` max length 255; one-to-one with Sale via `SaleId`
    - Keep `partial void OnModelCreatingPartial(ModelBuilder modelBuilder);` at the end
    - _Requirements: 18.5, 18.6, 18.7, 18.8, 18.9, 18.11, 18.12_

- [x] 11. Checkpoint — Verify full build after AppDbContext replacement
  - Run `dotnet build` from the project root and resolve any remaining compilation errors
  - Ensure all tests pass, ask the user if questions arise.

- [x] 12. Clear existing migrations and generate new migration
  - [x] 12.1 Delete all files in `SmartPOS/Migrations/`
    - Delete `20260517195004_InitialCreate.cs`
    - Delete `20260517195004_InitialCreate.Designer.cs`
    - Delete `20260522155004_SyncModel.cs`
    - Delete `20260522155004_SyncModel.Designer.cs`
    - Delete `AppDbContextModelSnapshot.cs`
    - _Requirements: 19.1_
  - [x] 12.2 Run `dotnet ef migrations add FullDatabaseSetup` from the project root
    - If the command fails due to compilation errors, resolve all build errors first and retry
    - Verify the generated migration file references all 16 tables
    - _Requirements: 19.1, 19.3_
  - [x] 12.3 Run `dotnet ef database update` from the project root
    - Verify the command completes without errors
    - _Requirements: 19.2_

---

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation before proceeding to the next phase
- Property tests validate universal correctness properties (SaleItem arithmetic, Review rating bounds)
- Unit tests validate specific examples and edge cases
- The design document contains the exact C# code for every model and the full `AppDbContext` — use it as the authoritative reference during implementation
- `Contracts.cs` must be created before any model that references an enum (`Promotion`, `Sale`, `PurchaseOrder`, `Payment`)
- Legacy model deletions (Task 2) must happen before rewriting `User.cs` (Task 3.3) to avoid dangling navigation references

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1.1"] },
    { "id": 1, "tasks": ["2.1", "2.2", "2.3", "2.4"] },
    { "id": 2, "tasks": ["3.1", "3.2"] },
    { "id": 3, "tasks": ["3.3"] },
    { "id": 4, "tasks": ["4.1", "4.2"] },
    { "id": 5, "tasks": ["5.1", "5.2"] },
    { "id": 6, "tasks": ["6.1"] },
    { "id": 7, "tasks": ["6.2"] },
    { "id": 8, "tasks": ["7.1"] },
    { "id": 9, "tasks": ["7.2"] },
    { "id": 10, "tasks": ["7.3", "8.1"] },
    { "id": 11, "tasks": ["8.2", "8.3", "8.4"] },
    { "id": 12, "tasks": ["8.5", "8.6"] },
    { "id": 13, "tasks": ["10.1"] },
    { "id": 14, "tasks": ["10.2"] },
    { "id": 15, "tasks": ["10.3"] },
    { "id": 16, "tasks": ["10.4"] },
    { "id": 17, "tasks": ["12.1"] },
    { "id": 18, "tasks": ["12.2"] },
    { "id": 19, "tasks": ["12.3"] }
  ]
}
```
