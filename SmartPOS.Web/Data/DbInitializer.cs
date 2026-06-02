using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Enums;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // ── Seed Roles ──
            if (!await context.Roles.AnyAsync())
            {
                var allModules = new[] { "UserManagement", "Inventory", "POS", "Reports", "Promotions", "Customers", "PurchaseOrders", "AuditLogs" };

                string BuildPermissionsJson(Dictionary<string, bool[]> moduleFlags)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var kvp in moduleFlags)
                    {
                        dict[kvp.Key] = new { CanCreate = kvp.Value[0], CanRead = kvp.Value[1], CanUpdate = kvp.Value[2], CanDelete = kvp.Value[3] };
                    }
                    return JsonSerializer.Serialize(new { Modules = dict });
                }

                var adminPerms = allModules.ToDictionary(m => m, _ => new[] { true, true, true, true });

                var managerPerms = new Dictionary<string, bool[]>
                {
                    ["UserManagement"]  = new[] { false, true,  true,  false },
                    ["Inventory"]       = new[] { true,  true,  true,  false },
                    ["POS"]             = new[] { true,  true,  true,  false },
                    ["Reports"]         = new[] { false, true,  false, false },
                    ["Promotions"]      = new[] { true,  true,  true,  false },
                    ["Customers"]       = new[] { true,  true,  true,  false },
                    ["PurchaseOrders"]  = new[] { true,  true,  true,  false },
                    ["AuditLogs"]       = new[] { false, true,  false, false },
                };

                var cashierPerms = new Dictionary<string, bool[]>
                {
                    ["UserManagement"]  = new[] { false, false, false, false },
                    ["Inventory"]       = new[] { false, true,  false, false },
                    ["POS"]             = new[] { true,  true,  true,  false },
                    ["Reports"]         = new[] { false, true,  false, false },
                    ["Promotions"]      = new[] { false, true,  false, false },
                    ["Customers"]       = new[] { true,  true,  true,  false },
                    ["PurchaseOrders"]  = new[] { false, false, false, false },
                    ["AuditLogs"]       = new[] { false, false, false, false },
                };

                var customerPerms = new Dictionary<string, bool[]>
                {
                    ["UserManagement"]  = new[] { false, false, false, false },
                    ["Inventory"]       = new[] { false, true,  false, false },
                    ["POS"]             = new[] { true,  true,  false, false },
                    ["Reports"]         = new[] { false, false, false, false },
                    ["Promotions"]      = new[] { false, true,  false, false },
                    ["Customers"]       = new[] { false, true,  true,  false },
                    ["PurchaseOrders"]  = new[] { false, false, false, false },
                    ["AuditLogs"]       = new[] { false, false, false, false },
                };

                context.Roles.AddRange(
                    new Role { Id = 1, RoleName = "Admin",    Permissions = BuildPermissionsJson(adminPerms) },
                    new Role { Id = 2, RoleName = "Manager",  Permissions = BuildPermissionsJson(managerPerms) },
                    new Role { Id = 3, RoleName = "Cashier",  Permissions = BuildPermissionsJson(cashierPerms) },
                    new Role { Id = 4, RoleName = "Customer", Permissions = BuildPermissionsJson(customerPerms) }
                );

                await context.SaveChangesAsync();
            }

            // ── Seed Users (individually — only if email doesn't exist) ──
            if (!await context.Users.AnyAsync(u => u.Email == "admin@smartpos.pk"))
            {
                context.Users.Add(new User
                {
                    Name = "Admin User",
                    Email = "admin@smartpos.pk",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "manager@smartpos.pk"))
            {
                context.Users.Add(new User
                {
                    Name = "Store Manager",
                    Email = "manager@smartpos.pk",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                    RoleId = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "cashier@smartpos.pk"))
            {
                context.Users.Add(new User
                {
                    Name = "Cashier User",
                    Email = "cashier@smartpos.pk",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                    RoleId = 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "customer@smartpos.pk"))
            {
                context.Users.Add(new User
                {
                    Name = "Test Customer",
                    Email = "customer@smartpos.pk",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                    RoleId = 4,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();

            // ── Seed Customers (only if none exist) ──
            if (!await context.Customers.AnyAsync())
            {
                var customers = new List<Customer>
                {
                    new() { Name = "Ayesha Tariq",     Email = "ayesha@email.com",    Phone = "0300-1234567", LoyaltyPoints = 240,  TotalSpent = 24000, IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-60) },
                    new() { Name = "Bilal Ahmed",      Email = "bilal@email.com",     Phone = "0301-2345678", LoyaltyPoints = 80,   TotalSpent = 8000,  IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-45) },
                    new() { Name = "Dania Khan",       Email = "dania@email.com",     Phone = "0302-3456789", LoyaltyPoints = 0,    TotalSpent = 0,     IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-30), Address = "House 12, Street 5, F-7/1, Islamabad" },
                    new() { Name = "Farhan Malik",     Email = "farhan@email.com",    Phone = "0303-4567890", LoyaltyPoints = 560,  TotalSpent = 56000, IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-20), DateOfBirth = new DateTime(1990, 5, 15) },
                    new() { Name = "Hina Shah",       Email = "hina@email.com",      Phone = "0304-5678901", LoyaltyPoints = 120,  TotalSpent = 12000, IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                    new() { Name = "Imran Ali",        Email = "imran@email.com",     Phone = "0305-6789012", LoyaltyPoints = 30,   TotalSpent = 3500,  IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-5) },
                };

                context.Customers.AddRange(customers);
                await context.SaveChangesAsync();
            }

            // ── Seed Promotions (only if none exist) ──
            if (!await context.Promotions.AnyAsync())
            {
                var now = DateTime.UtcNow;

                var promotions = new List<Promotion>
                {
                    // Active percentage discount
                    new()
                    {
                        Code = "WELCOME10",
                        DiscountType = SmartPOS.Shared.Enums.DiscountType.Percentage,
                        Value = 10,
                        MinOrderValue = 500,
                        MaxUsageLimit = 100,
                        UsageCount = 15,
                        ValidFrom = now.AddDays(-30),
                        ValidTo = now.AddDays(60),
                        IsActive = true,
                        Description = "10% off for all orders above Rs 500",
                        CreatedAt = now.AddDays(-30)
                    },
                    // Active flat discount
                    new()
                    {
                        Code = "FLAT50",
                        DiscountType = SmartPOS.Shared.Enums.DiscountType.Flat,
                        Value = 50,
                        MinOrderValue = 200,
                        MaxUsageLimit = 50,
                        UsageCount = 23,
                        ValidFrom = now.AddDays(-15),
                        ValidTo = now.AddDays(45),
                        IsActive = true,
                        Description = "Rs 50 flat discount on orders above Rs 200",
                        CreatedAt = now.AddDays(-15)
                    },
                    // Percentage for loyal customers
                    new()
                    {
                        Code = "LOYAL20",
                        DiscountType = SmartPOS.Shared.Enums.DiscountType.Percentage,
                        Value = 20,
                        MinOrderValue = 1000,
                        MaxUsageLimit = 25,
                        UsageCount = 8,
                        ValidFrom = now.AddDays(-10),
                        ValidTo = now.AddDays(20),
                        IsActive = true,
                        Description = "20% off for orders above Rs 1000",
                        CreatedAt = now.AddDays(-10)
                    },
                    // Inactive promotion
                    new()
                    {
                        Code = "SUMMER15",
                        DiscountType = SmartPOS.Shared.Enums.DiscountType.Percentage,
                        Value = 15,
                        MinOrderValue = 0,
                        MaxUsageLimit = 200,
                        UsageCount = 45,
                        ValidFrom = now.AddDays(-90),
                        ValidTo = now.AddDays(-30),
                        IsActive = false,
                        Description = "Summer sale - expired",
                        CreatedAt = now.AddDays(-90)
                    },
                    // Big discount for special events
                    new()
                    {
                        Code = "GRAND25",
                        DiscountType = SmartPOS.Shared.Enums.DiscountType.Percentage,
                        Value = 25,
                        MinOrderValue = 2000,
                        MaxUsageLimit = 50,
                        UsageCount = 0,
                        ValidFrom = now,
                        ValidTo = now.AddDays(7),
                        IsActive = true,
                        Description = "Grand opening special - 25% off on orders above Rs 2000",
                        CreatedAt = now
                    }
                };

                context.Promotions.AddRange(promotions);
                await context.SaveChangesAsync();
            }

            // ── Seed Categories (only if none exist) ──
            if (!context.Categories.Any())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Category ON");
                    context.Categories.AddRange(
                        new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
                        new Category { Id = 2, Name = "Food & Beverages", Description = "Food and drink products" },
                        new Category { Id = 3, Name = "Office Supplies", Description = "Office and stationery items" }
                    );
                    await context.SaveChangesAsync();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Category OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

            // ── Seed Suppliers (only if none exist) ──
            if (!context.Suppliers.Any())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Supplier ON");
                    context.Suppliers.AddRange(
                        new Supplier { Id = 1, Name = "TechWorld Distributors", ContactPerson = "Ali Raza", ContactNo = "0300-1234567", Email = "ali@techworld.com", Address = "123 Main Street, Lahore", IsActive = true },
                        new Supplier { Id = 2, Name = "Fresh Foods Supply Co.", ContactPerson = "Sara Khan", ContactNo = "0301-7654321", Email = "sara@freshfoods.com", Address = "456 Food Street, Karachi", IsActive = true },
                        new Supplier { Id = 3, Name = "Office Essentials Ltd.", ContactPerson = "Bilal Ahmed", ContactNo = "0302-9876543", Email = "bilal@officeessentials.com", Address = "789 Office Road, Islamabad", IsActive = true }
                    );
                    await context.SaveChangesAsync();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Supplier OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

            // ── Seed Products (only if none exist) ──
            if (!context.Products.Any())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Product ON");
                    context.Products.AddRange(
                        new Product { Id = 1, Name = "Wireless Mouse", SKU = "WM-001", Price = 250.00m, CostPrice = 150.00m, IsActive = true, CategoryId = 1, SupplierId = 1 },
                        new Product { Id = 2, Name = "Mechanical Keyboard", SKU = "MK-002", Price = 450.00m, CostPrice = 290.00m, IsActive = true, CategoryId = 1, SupplierId = 1 },
                        new Product { Id = 3, Name = "Organic Coffee Beans", SKU = "CB-001", Price = 120.00m, CostPrice = 55.00m, IsActive = true, CategoryId = 2, SupplierId = 2 },
                        new Product { Id = 4, Name = "Green Tea Bags", SKU = "GT-001", Price = 35.00m, CostPrice = 17.00m, IsActive = true, CategoryId = 2, SupplierId = 2 },
                        new Product { Id = 5, Name = "A4 Printer Paper", SKU = "PP-001", Price = 180.00m, CostPrice = 120.00m, IsActive = true, CategoryId = 3, SupplierId = 3 },
                        new Product { Id = 6, Name = "Stapler", SKU = "ST-001", Price = 120.00m, CostPrice = 80.00m, IsActive = true, CategoryId = 3, SupplierId = 3 },
                        new Product { Id = 7, Name = "USB-C Hub", SKU = "UC-001", Price = 300.00m, CostPrice = 185.00m, IsActive = true, CategoryId = 1, SupplierId = 1 },
                        new Product { Id = 8, Name = "Monitor Stand", SKU = "MS-001", Price = 150.00m, CostPrice = 95.00m, IsActive = true, CategoryId = 3, SupplierId = 3 }
                    );
                    await context.SaveChangesAsync();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Product OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

            // ── Seed Inventory (only if none exist) ──
            if (!context.Inventory.Any())
            {
                var inventoryRecords = new List<Inventory>
                {
                    new() { ProductId = 1, Quantity = 142, ReorderLevel = 20, LastUpdated = DateTime.UtcNow },
                    new() { ProductId = 2, Quantity = 4,   ReorderLevel = 15, LastUpdated = DateTime.UtcNow },
                    new() { ProductId = 3, Quantity = 0,   ReorderLevel = 30, LastUpdated = DateTime.UtcNow },
                    new() { ProductId = 4, Quantity = 88,  ReorderLevel = 25, LastUpdated = DateTime.UtcNow },
                    new() { ProductId = 5, Quantity = 6,   ReorderLevel = 20, LastUpdated = DateTime.UtcNow },
                    new() { ProductId = 6, Quantity = 54,  ReorderLevel = 15, LastUpdated = DateTime.UtcNow },
                    new() { ProductId = 7, Quantity = 12,  ReorderLevel = 25, LastUpdated = DateTime.UtcNow },
                    new() { ProductId = 8, Quantity = 63,  ReorderLevel = 20, LastUpdated = DateTime.UtcNow },
                };

                context.Inventory.AddRange(inventoryRecords);
                await context.SaveChangesAsync();
            }

            // ── Seed Purchase Orders (only if none exist) ──
            if (!context.PurchaseOrders.Any())
            {
                var now = DateTime.UtcNow;

                var po1 = new PurchaseOrder
                {
                    SupplierId = 1, UserId = 1, Status = POStatus.Draft,
                    Notes = "Monthly flour restock",
                    OrderDate = now.AddDays(-5),
                    TotalCost = 50 * 120,
                    LineItems = new List<POLineItem>
                    {
                        new() { ProductId = 3, OrderedQty = 50, UnitPrice = 120 }
                    }
                };

                var po2 = new PurchaseOrder
                {
                    SupplierId = 1, UserId = 1, Status = POStatus.Sent,
                    Notes = "Urgent sugar restock",
                    OrderDate = now.AddDays(-3),
                    TotalCost = (30 * 85) + (20 * 95),
                    LineItems = new List<POLineItem>
                    {
                        new() { ProductId = 2, OrderedQty = 30, UnitPrice = 85 },
                        new() { ProductId = 5, OrderedQty = 20, UnitPrice = 95 }
                    }
                };

                var po3 = new PurchaseOrder
                {
                    SupplierId = 1, UserId = 1, Status = POStatus.Received,
                    Notes = "Packaging materials",
                    OrderDate = now.AddDays(-10),
                    ReceivedAt = now.AddDays(-7),
                    TotalCost = 100 * 45,
                    LineItems = new List<POLineItem>
                    {
                        new() { ProductId = 4, OrderedQty = 100, UnitPrice = 45 }
                    }
                };

                var po4 = new PurchaseOrder
                {
                    SupplierId = 1, UserId = 1, Status = POStatus.Cancelled,
                    Notes = "Cancelled — supplier unavailable",
                    OrderDate = now.AddDays(-8),
                    TotalCost = 25 * 200,
                    LineItems = new List<POLineItem>
                    {
                        new() { ProductId = 7, OrderedQty = 25, UnitPrice = 200 }
                    }
                };

                context.PurchaseOrders.AddRange(po1, po2, po3, po4);
                await context.SaveChangesAsync();
            }
        }
    }
}
