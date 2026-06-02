# Requirements Document

## Introduction

This feature defines the complete database schema for SmartPOS+, a Blazor Server (.NET 10) point-of-sale application. The schema consists of 17 tables covering user management, product catalog, sales, inventory, purchasing, promotions, reviews, payments, and unified audit logging. All tables are implemented as EF Core model classes registered in `AppDbContext`. The four existing broken audit log tables (AdminLog, CashierLog, CustomerLog, ManagerAction) are replaced by a single unified AuditLogs table. Shared enums are defined in a `Contracts.cs` file under `SmartPOS.Shared.Enums`.

## Glossary

- **AppDbContext**: The EF Core `DbContext` class located at `SmartPOS/Data/AppDbContext.cs` that registers all DbSets and configures relationships via Fluent API.
- **Model**: A C# class in `SmartPOS/Models/` that maps to a database table.
- **Fluent API**: EF Core's `OnModelCreating` configuration method used to define constraints, indexes, defaults, and relationships.
- **EARS**: Easy Approach to Requirements Syntax — the pattern used for all acceptance criteria.
- **PK**: Primary key.
- **FK**: Foreign key.
- **Enum**: A C# enumeration type stored as an integer column in the database.
- **AuditLog**: The unified audit log model that replaces AdminLog, CashierLog, CustomerLog, and ManagerAction.
- **Cascade**: Delete behavior where dependent rows are automatically deleted when the principal row is deleted.
- **Restrict**: Delete behavior that prevents deletion of a principal row if dependent rows exist.
- **SetNull**: Delete behavior that sets the FK column to NULL when the principal row is deleted.
- **NoAction**: Delete behavior that takes no action at the database level (used for self-referencing relationships to avoid cycles).
- **One-to-one**: A relationship where each row in table A maps to exactly one row in table B.
- **Self-referencing**: A FK that points back to the same table (used for Category hierarchy).

---

## Requirements

### Requirement 1: Shared Enum Definitions

**User Story:** As a developer, I want all shared enum types defined in a single file, so that model classes and the application layer reference a consistent set of named constants.

#### Acceptance Criteria

1. THE `Contracts.cs` file SHALL define the following enums in the `SmartPOS.Shared.Enums` namespace: `DiscountType`, `SaleType`, `SaleStatus`, `POStatus`, `PaymentMethod`, and `PaymentStatus`.
2. WHEN a model property references an enum type, THE model SHALL use the fully qualified type from `SmartPOS.Shared.Enums`.

---

### Requirement 2: Role Model

**User Story:** As a developer, I want a `Role` model class, so that user roles can be stored and associated with users and permissions.

#### Acceptance Criteria

1. THE `Role` model SHALL have an `Id` property of type `int` configured as the primary key with auto-increment.
2. THE `Role` model SHALL have a `Name` property of type `string` mapped to `varchar(50)`, not null.
3. THE `Role` model SHALL have an `ICollection<User>` navigation property named `Users`.
4. THE `Role` model SHALL have an `ICollection<Permission>` navigation property named `Permissions`.

---

### Requirement 3: Permission Model

**User Story:** As a developer, I want a `Permission` model class, so that per-role, per-module CRUD access rights can be stored.

#### Acceptance Criteria

1. THE `Permission` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Permission` model SHALL have a `RoleId` property of type `int`, not null, with a database index.
3. THE `Permission` model SHALL have a `Module` property of type `string` mapped to `varchar(100)`, not null.
4. THE `Permission` model SHALL have `CanCreate`, `CanRead`, `CanUpdate`, and `CanDelete` properties of type `bool`, each not null with a default value of `false`.
5. THE `Permission` model SHALL have a `Role` navigation property of type `Role` representing the many-to-one relationship to `Roles`.

---

### Requirement 4: User Model

**User Story:** As a developer, I want a `User` model class, so that application users with their credentials and role assignments can be persisted.

#### Acceptance Criteria

1. THE `User` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `User` model SHALL have `Name` mapped to `varchar(100)` not null, `Email` mapped to `varchar(150)` not null with a unique index, and `PasswordHash` mapped to `varchar(255)` not null.
3. THE `User` model SHALL have a `RoleId` property of type `int`, not null, with a database index, and a `Role` navigation property.
4. THE `User` model SHALL have an `IsActive` property of type `bool`, not null, with a default value of `true`.
5. THE `User` model SHALL have a `CreatedAt` property of type `DateTime`, not null, with a default value of UTC now.
6. THE `User` model SHALL have navigation properties: `Customer?`, `ICollection<Sale>`, `ICollection<PurchaseOrder>`, and `ICollection<AuditLog>`.

---

### Requirement 5: Customer Model

**User Story:** As a developer, I want a `Customer` model class, so that customer-specific profile data and loyalty information can be stored separately from the base user record.

#### Acceptance Criteria

1. THE `Customer` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Customer` model SHALL have a `UserId` property of type `int`, not null, with a unique index, establishing a one-to-one relationship with `Users`.
3. THE `Customer` model SHALL have `Name` mapped to `varchar(100)` not null, `Email` mapped to `varchar(150)` not null with a unique index.
4. THE `Customer` model SHALL have `Phone` mapped to `varchar(20)` nullable, `DateOfBirth` of type `DateOnly?` nullable, and `Address` of type `string?` mapped to `text` nullable.
5. THE `Customer` model SHALL have `LoyaltyPoints` of type `int` not null with default `0`, and `TotalSpent` of type `decimal(10,2)` not null with default `0.00`.
6. THE `Customer` model SHALL have a `CreatedAt` property of type `DateTime`, not null, with a default value of UTC now.
7. THE `Customer` model SHALL have navigation properties: `User`, `ICollection<Sale>`, and `ICollection<Review>`.

---

### Requirement 6: AuditLog Model (Replaces AdminLog, CashierLog, CustomerLog, ManagerAction)

**User Story:** As a developer, I want a single unified `AuditLog` model, so that all user actions across all roles are tracked in one table instead of four separate broken tables.

#### Acceptance Criteria

1. THE `AuditLog` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `AuditLog` model SHALL have a `UserId` property of type `int`, not null, with a database index, and a `User` navigation property.
3. THE `AuditLog` model SHALL have an `Action` property mapped to `varchar(255)` not null, and a `Module` property mapped to `varchar(100)` not null.
4. THE `AuditLog` model SHALL have a `Timestamp` property of type `DateTime`, not null, with a default value of UTC now.
5. THE `AuditLog` model SHALL have `IPAddress` mapped to `varchar(45)` nullable, and `Details` mapped to `text` nullable.
6. WHEN the `AuditLog` model is registered in `AppDbContext`, THE `AppDbContext` SHALL remove all `DbSet` registrations for `AdminLog`, `CashierLog`, `CustomerLog`, and `ManagerAction`.
7. THE model files `AdminLog.cs`, `CashierLog.cs`, `CustomerLog.cs`, and `ManagerAction.cs` SHALL be deleted from the `Models` folder.

---

### Requirement 7: Promotion Model

**User Story:** As a developer, I want a `Promotion` model class, so that discount codes with their rules and usage tracking can be stored.

#### Acceptance Criteria

1. THE `Promotion` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Promotion` model SHALL have a `Code` property mapped to `varchar(50)` not null with a unique index.
3. THE `Promotion` model SHALL have a `DiscountType` property of type `DiscountType` (enum from `SmartPOS.Shared.Enums`), not null.
4. THE `Promotion` model SHALL have `Value` of type `decimal(10,2)` not null, `MinOrderValue` of type `decimal(10,2)` not null with default `0.00`, `MaxUsageLimit` of type `int?` nullable, and `UsageCount` of type `int` not null with default `0`.
5. THE `Promotion` model SHALL have `ValidFrom` and `ValidTo` properties of type `DateOnly`, not null.
6. THE `Promotion` model SHALL have an `IsActive` property of type `bool`, not null, with a default value of `true`.
7. THE `Promotion` model SHALL have an `ICollection<Sale>` navigation property named `Sales`.

---

### Requirement 8: Category Model

**User Story:** As a developer, I want a `Category` model class with self-referencing hierarchy support, so that products can be organized into nested categories.

#### Acceptance Criteria

1. THE `Category` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Category` model SHALL have a `Name` property mapped to `varchar(100)` not null.
3. THE `Category` model SHALL have a `ParentCategoryId` property of type `int?` nullable, acting as a self-referencing FK to `Categories.Id`.
4. THE `Category` model SHALL have `Description` mapped to `text` nullable, and `ImageURL` mapped to `varchar(255)` nullable.
5. THE `Category` model SHALL have navigation properties: `Category? ParentCategory`, `ICollection<Category> SubCategories`, and `ICollection<Product> Products`.
6. WHEN configuring the self-referencing FK in `AppDbContext`, THE `AppDbContext` SHALL set the delete behavior to `NoAction` to prevent cascade cycles.

---

### Requirement 9: Supplier Model

**User Story:** As a developer, I want a `Supplier` model class, so that product supplier contact information can be stored and linked to products and purchase orders.

#### Acceptance Criteria

1. THE `Supplier` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Supplier` model SHALL have a `Name` property mapped to `varchar(100)` not null.
3. THE `Supplier` model SHALL have `ContactPerson` mapped to `varchar(100)` nullable, `ContactNo` mapped to `varchar(20)` nullable, `Email` mapped to `varchar(150)` nullable with a unique index, and `Address` mapped to `text` nullable.
4. THE `Supplier` model SHALL have an `IsActive` property of type `bool`, not null, with a default value of `true`.
5. THE `Supplier` model SHALL have navigation properties: `ICollection<Product> Products` and `ICollection<PurchaseOrder> PurchaseOrders`.

---

### Requirement 10: Product Model

**User Story:** As a developer, I want a `Product` model class, so that all product catalog data including pricing, categorization, and supplier linkage can be stored.

#### Acceptance Criteria

1. THE `Product` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Product` model SHALL have `Name` mapped to `varchar(150)` not null, `SKU` mapped to `varchar(50)` not null with a unique index, `Description` mapped to `text` nullable, and `ImageURL` mapped to `varchar(255)` nullable.
3. THE `Product` model SHALL have `Price` and `CostPrice` properties of type `decimal(10,2)` not null.
4. THE `Product` model SHALL have an `IsActive` property of type `bool`, not null, with a default value of `true`.
5. THE `Product` model SHALL have a `CategoryId` property of type `int`, not null, with delete behavior `Restrict` toward `Categories`.
6. THE `Product` model SHALL have a `SupplierId` property of type `int?` nullable, with delete behavior `SetNull` toward `Suppliers`.
7. THE `Product` model SHALL have a `CreatedAt` property of type `DateTime`, not null, with a default value of UTC now.
8. THE `Product` model SHALL have navigation properties: `Category`, `Supplier?`, `Inventory?`, `ICollection<SaleItem>`, `ICollection<POLineItem>`, and `ICollection<Review>`.

---

### Requirement 11: Sale Model

**User Story:** As a developer, I want a `Sale` model class, so that each sales transaction with its totals, type, status, and associations can be recorded.

#### Acceptance Criteria

1. THE `Sale` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Sale` model SHALL have `CustomerId` of type `int?` nullable (FK → Customers), `UserId` of type `int` not null (FK → Users), and `PromoId` of type `int?` nullable (FK → Promotions).
3. THE `Sale` model SHALL have a `SaleType` property of type `SaleType` (enum), not null.
4. THE `Sale` model SHALL have `TotalAmount` of type `decimal(10,2)` not null, `DiscountAmount` of type `decimal(10,2)` not null with default `0.00`, and `TaxAmount` of type `decimal(10,2)` not null.
5. THE `Sale` model SHALL have a `SaleDate` property of type `DateTime`, not null, with a default value of UTC now.
6. THE `Sale` model SHALL have a `Status` property of type `SaleStatus` (enum), not null, with a default value of `Completed`.
7. THE `Sale` model SHALL have navigation properties: `Customer?`, `User`, `Promotion?`, `ICollection<SaleItem>`, and `Payment?`.

---

### Requirement 12: SaleItem Model

**User Story:** As a developer, I want a `SaleItem` model class, so that individual line items within a sale, including quantity and pricing, can be stored.

#### Acceptance Criteria

1. THE `SaleItem` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `SaleItem` model SHALL have a `SaleId` property of type `int`, not null, with a database index and delete behavior `Cascade` toward `Sales`.
3. THE `SaleItem` model SHALL have a `ProductId` property of type `int`, not null, with delete behavior `Restrict` toward `Products`.
4. THE `SaleItem` model SHALL have `Quantity` of type `int` not null, `UnitPrice` of type `decimal(10,2)` not null, and `LineTotal` of type `decimal(10,2)` not null.
5. THE `SaleItem` model SHALL have navigation properties: `Sale` and `Product`.

---

### Requirement 13: Review Model

**User Story:** As a developer, I want a `Review` model class, so that customer product ratings and optional sentiment analysis results can be stored.

#### Acceptance Criteria

1. THE `Review` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Review` model SHALL have `CustomerId` of type `int` not null (FK → Customers) and `ProductId` of type `int` not null (FK → Products).
3. THE `Review` model SHALL have a `Rating` property of type `int`, not null, with a CHECK constraint enforcing `Rating >= 1 AND Rating <= 5`.
4. THE `Review` model SHALL have `Comment` mapped to `text` nullable, `Sentiment` mapped to `varchar(20)` nullable, and `SentimentScore` of type `float?` nullable.
5. THE `Review` model SHALL have a `CreatedAt` property of type `DateTime`, not null, with a default value of UTC now.
6. THE `Review` model SHALL have navigation properties: `Customer` and `Product`.

---

### Requirement 14: Inventory Model

**User Story:** As a developer, I want an `Inventory` model class, so that stock levels and reorder thresholds for each product can be tracked in a one-to-one relationship.

#### Acceptance Criteria

1. THE `Inventory` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Inventory` model SHALL have a `ProductId` property of type `int`, not null, with a unique index, establishing a one-to-one relationship with `Products`.
3. THE `Inventory` model SHALL have `Quantity` of type `int` not null with default `0`, and `ReorderLevel` of type `int` not null.
4. THE `Inventory` model SHALL have a `LastUpdated` property of type `DateTime`, not null, with a default value of UTC now.
5. THE `Inventory` model SHALL have a `Product` navigation property.

---

### Requirement 15: PurchaseOrder Model

**User Story:** As a developer, I want a `PurchaseOrder` model class, so that supplier purchase orders with their status, cost, and associated line items can be tracked.

#### Acceptance Criteria

1. THE `PurchaseOrder` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `PurchaseOrder` model SHALL have `SupplierId` of type `int` not null (FK → Suppliers) and `UserId` of type `int` not null (FK → Users).
3. THE `PurchaseOrder` model SHALL have a `Status` property of type `POStatus` (enum), not null, with a default value of `Draft`.
4. THE `PurchaseOrder` model SHALL have `TotalCost` of type `decimal(10,2)` not null, `OrderDate` of type `DateTime` not null with default UTC now, `ReceivedAt` of type `DateTime?` nullable, and `Notes` mapped to `text` nullable.
5. THE `PurchaseOrder` model SHALL have navigation properties: `Supplier`, `User`, and `ICollection<POLineItem> LineItems`.

---

### Requirement 16: POLineItem Model

**User Story:** As a developer, I want a `POLineItem` model class, so that individual product line items within a purchase order can be stored.

#### Acceptance Criteria

1. THE `POLineItem` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `POLineItem` model SHALL have a `POID` property of type `int`, not null, with a database index and delete behavior `Cascade` toward `PurchaseOrders`.
3. THE `POLineItem` model SHALL have a `ProductId` property of type `int`, not null (FK → Products).
4. THE `POLineItem` model SHALL have `OrderedQty` of type `int` not null and `UnitPrice` of type `decimal(10,2)` not null.
5. THE `POLineItem` model SHALL have navigation properties: `PurchaseOrder` and `Product`.

---

### Requirement 17: Payment Model

**User Story:** As a developer, I want a `Payment` model class, so that payment details for each sale can be stored in a one-to-one relationship.

#### Acceptance Criteria

1. THE `Payment` model SHALL have an `Id` property of type `int` configured as the primary key.
2. THE `Payment` model SHALL have a `SaleId` property of type `int`, not null, with a unique index, establishing a one-to-one relationship with `Sales`.
3. THE `Payment` model SHALL have a `Method` property of type `PaymentMethod` (enum), not null.
4. THE `Payment` model SHALL have an `Amount` property of type `decimal(10,2)` not null.
5. THE `Payment` model SHALL have a `Status` property of type `PaymentStatus` (enum), not null, with a default value of `Pending`.
6. THE `Payment` model SHALL have `TransactionRef` mapped to `varchar(255)` nullable, and `PaidAt` of type `DateTime?` nullable.
7. THE `Payment` model SHALL have a `Sale` navigation property.

---

### Requirement 18: AppDbContext Configuration

**User Story:** As a developer, I want `AppDbContext` fully configured with all 16 DbSets and Fluent API relationships, so that EF Core can generate a correct migration and database schema.

#### Acceptance Criteria

1. THE `AppDbContext` SHALL expose `DbSet` properties for all 16 tables: `Roles`, `Permissions`, `Users`, `Customers`, `AuditLogs`, `Promotions`, `Categories`, `Suppliers`, `Products`, `Sales`, `SaleItems`, `Reviews`, `Inventories`, `PurchaseOrders`, `POLineItems`, and `Payments`.
2. THE `AppDbContext` SHALL NOT contain an `OnConfiguring` override (the connection string is registered in `Program.cs` via `appsettings.json`).
3. WHEN configuring `Category`, THE `AppDbContext` SHALL set the self-referencing FK delete behavior to `NoAction`.
4. WHEN configuring `Product`, THE `AppDbContext` SHALL set the `Category` FK delete behavior to `Restrict` and the `Supplier` FK delete behavior to `SetNull`.
5. WHEN configuring `SaleItem`, THE `AppDbContext` SHALL set the `Sale` FK delete behavior to `Cascade` and the `Product` FK delete behavior to `Restrict`.
6. WHEN configuring `POLineItem`, THE `AppDbContext` SHALL set the `PurchaseOrder` FK delete behavior to `Cascade`.
7. WHEN configuring `Review`, THE `AppDbContext` SHALL add a CHECK constraint enforcing `Rating >= 1 AND Rating <= 5`.
8. WHEN configuring `Inventory`, THE `AppDbContext` SHALL configure the one-to-one relationship with `Product` using `ProductId` as the unique FK.
9. WHEN configuring `Payment`, THE `AppDbContext` SHALL configure the one-to-one relationship with `Sale` using `SaleId` as the unique FK.
10. WHEN configuring `Customer`, THE `AppDbContext` SHALL configure the one-to-one relationship with `User` using `UserId` as the unique FK.
11. THE `AppDbContext` SHALL configure all `decimal` columns with precision `(10, 2)` using Fluent API.
12. THE `AppDbContext` SHALL configure all `varchar` length constraints using `HasMaxLength` in Fluent API.

---

### Requirement 19: EF Core Migration

**User Story:** As a developer, I want a clean EF Core migration generated from the final models, so that the database schema can be created or updated without errors.

#### Acceptance Criteria

1. WHEN all model files and `AppDbContext` are in their final state, THE developer SHALL run `dotnet ef migrations add FullDatabaseSetup` from the project root to generate the migration.
2. WHEN the migration is generated without errors, THE developer SHALL run `dotnet ef database update` to apply the schema to the database.
3. IF the migration command fails due to compilation errors, THEN THE developer SHALL resolve all build errors in model files and `AppDbContext` before retrying.
