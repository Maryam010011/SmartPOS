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
        }
    }
}
