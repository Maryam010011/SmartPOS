using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Data;

public static class DatabaseSeeder
{
     public static async Task SeedAsync(IServiceProvider serviceProvider)
     {
          var context = serviceProvider.GetRequiredService<AppDbContext>();

          // 1. Seed Roles
          if (!await context.Roles.AnyAsync())
          {
               var roles = new List<Role>
               {
                    new() { Name = "Admin" },
                    new() { Name = "Manager" },
                    new() { Name = "Cashier" },
                    new() { Name = "Customer" }
               };

               context.Roles.AddRange(roles);
               await context.SaveChangesAsync();
          }

          // 2. Seed Default Admin User
          if (!await context.Users.AnyAsync())
          {
               var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

               if (adminRole != null)
               {
                    var adminUser = new User
                    {
                         Name = "admin",
                         Email = "admin@smartpos.com",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                         RoleId = adminRole.Id,
                         IsActive = true,
                         CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(adminUser);
                    await context.SaveChangesAsync();
               }
          }

          // 3. Seed Default Category
          if (!await context.Categories.AnyAsync())
          {
               var category = new Category
               {
                    Name = "General",
                    Description = "Default category"
               };

               context.Categories.Add(category);
               await context.SaveChangesAsync();
          }

          // 4. Seed Default Supplier
          if (!await context.Suppliers.AnyAsync())
          {
               var supplier = new Supplier
               {
                    Name = "Default Supplier",
                    Email = "supplier@smartpos.com",
                    IsActive = true
               };

               context.Suppliers.Add(supplier);
               await context.SaveChangesAsync();
          }

          // 5. Seed Default Product
          if (!await context.Products.AnyAsync())
          {
               var category = await context.Categories
                    .FirstOrDefaultAsync(c => c.Name == "General");

               var supplier = await context.Suppliers
                    .FirstOrDefaultAsync(s => s.Name == "Default Supplier");

               if (category != null && supplier != null)
               {
                    var product = new Product
                    {
                         Name = "Sample Product",
                         SKU = "SKU-DEFAULT",
                         Description = "Default product",
                         Price = 100m,
                         CostPrice = 0m,
                         IsActive = true,
                         CategoryId = category.Id,
                         SupplierId = supplier.Id,
                         CreatedAt = DateTime.UtcNow
                    };

                    context.Products.Add(product);
                    await context.SaveChangesAsync();
               }
          }

          // 6. Seed Default Inventory Record
          if (!await context.Inventories.AnyAsync())
          {
               var product = await context.Products
                    .FirstOrDefaultAsync(p => p.Name == "Sample Product");

               if (product != null)
               {
                    var inventory = new Inventory
                    {
                         ProductId = product.Id,
                         Quantity = 100,
                         ReorderLevel = 10,
                         LastUpdated = DateTime.UtcNow
                    };

                    context.Inventories.Add(inventory);
                    await context.SaveChangesAsync();
               }
          }
     }
}