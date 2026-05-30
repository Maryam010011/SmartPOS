using System.Text.Json;
using Microsoft.EntityFrameworkCore;
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

            // ── Seed Users (only if none exist) ──
            if (!await context.Users.AnyAsync())
            {
                var adminRole  = await context.Roles.FirstAsync(r => r.RoleName == "Admin");
                var managerRole = await context.Roles.FirstAsync(r => r.RoleName == "Manager");
                var cashierRole = await context.Roles.FirstAsync(r => r.RoleName == "Cashier");
                var customerRole = await context.Roles.FirstAsync(r => r.RoleName == "Customer");

                var hash = BCrypt.Net.BCrypt.HashPassword("password123");

                var users = new List<User>
                {
                    new() { Name = "Admin User",    Email = "admin@smartpos.com",   PasswordHash = hash, RoleId = adminRole.Id,  IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-30) },
                    new() { Name = "Sara Khan",     Email = "sara@smartpos.com",    PasswordHash = hash, RoleId = managerRole.Id, IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-20) },
                    new() { Name = "Ali Raza",      Email = "ali@smartpos.com",     PasswordHash = hash, RoleId = cashierRole.Id, IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-15) },
                    new() { Name = "Fatima Ahmed",  Email = "fatima@smartpos.com",  PasswordHash = hash, RoleId = cashierRole.Id, IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-10) },
                    new() { Name = "Omar Hassan",   Email = "omar@smartpos.com",    PasswordHash = hash, RoleId = cashierRole.Id, IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                    new() { Name = "Zainab Malik",  Email = "zainab@smartpos.com",  PasswordHash = hash, RoleId = customerRole.Id, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-3) },
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

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
        }
    }
}
