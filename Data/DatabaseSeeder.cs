using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Web.Data;

public static class DatabaseSeeder
{
     public static async Task SeedAsync(IServiceProvider serviceProvider)
     {
          using var scope = serviceProvider.CreateScope();
          using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext();

          // Seed Roles
          if (!await db.Roles.AnyAsync())
          {
               db.Roles.AddRange(
                   new Role { Name = "Admin" },
                   new Role { Name = "Manager" },
                   new Role { Name = "Cashier" },
                   new Role { Name = "Customer" }
               );
               await db.SaveChangesAsync();
          }

          // Seed Admin User
          if (!await db.Users.AnyAsync())
          {
               var adminRole = await db.Roles.FirstAsync(r => r.Name == "Admin");
               db.Users.Add(new User
               {
                    Name = "Admin",
                    Email = "admin@smartpos.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    RoleId = adminRole.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
               });
               await db.SaveChangesAsync();
          }

          // Seed Category
          if (!await db.Categories.AnyAsync())
          {
               db.Categories.Add(new Category
               {
                    Name = "Bakery Items",
                    Description = "All bakery products"
               });
               await db.SaveChangesAsync();
          }

          // Seed Supplier
          if (!await db.Suppliers.AnyAsync())
          {
               db.Suppliers.Add(new Supplier
               {
                    Name = "Default Supplier",
                    Email = "supplier@smartpos.com",
                    IsActive = true
               });
               await db.SaveChangesAsync();
          }

          // Seed Product
          if (!await db.Products.AnyAsync())
          {
               var category = await db.Categories.FirstAsync();
               var supplier = await db.Suppliers.FirstAsync();
               var product = new Product
               {
                    Name = "Sample Croissant",
                    SKU = "BKRY-001",
                    Price = 150.00m,
                    CostPrice = 80.00m,
                    IsActive = true,
                    CategoryId = category.Id,
                    SupplierId = supplier.Id,
                    CreatedAt = DateTime.UtcNow
               };
               db.Products.Add(product);
               await db.SaveChangesAsync();

               // Seed Inventory for the product
               db.Inventories.Add(new Inventory
               {
                    ProductId = product.Id,
                    Quantity = 100,
                    ReorderLevel = 20,
                    LastUpdated = DateTime.UtcNow
               });
               await db.SaveChangesAsync();
          }
     }
}