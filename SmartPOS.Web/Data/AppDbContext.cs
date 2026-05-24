// ============================================================
//  SmartPOS+ — Application DbContext
//
//  Note: User, Customer, Promotion DbSets will be added by
//  Maryam Jahangir. Inventory, PurchaseOrder DbSets will be
//  added by Maryam Yaqoob.
// ============================================================

using Microsoft.EntityFrameworkCore;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ── DbSets — Shahzain's Models ──

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<SaleItem> SaleItems { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        // ── DbSets — Maryam Jahangir's Models ──
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─────────────────────────────────────────────────
            // Category — self-referencing hierarchy
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.ParentCategory)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(c => c.ParentCategoryId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ─────────────────────────────────────────────────
            // Product → Category (required, restrict delete)
            // Product → Supplier (optional, set null on delete)
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Supplier)
                      .WithMany(s => s.Products)
                      .HasForeignKey(p => p.SupplierId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ─────────────────────────────────────────────────
            // Sale → SaleItems (required, cascade delete)
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasMany(s => s.SaleItems)
                      .WithOne(si => si.Sale)
                      .HasForeignKey(si => si.SaleId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ─────────────────────────────────────────────────
            // SaleItem → Product (required, restrict delete)
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasOne(si => si.Product)
                      .WithMany(p => p.SaleItems)
                      .HasForeignKey(si => si.ProductId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─────────────────────────────────────────────────
            // Review — Rating check constraint (1–5)
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));
            });

            // ─────────────────────────────────────────────────
            // User — Email unique constraint & index
            // User → Role FK (restrict delete so role can't be
            //         removed while users are assigned)
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─────────────────────────────────────────────────
            // Customer — Email unique index
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(c => c.Email)
                      .IsUnique();

                entity.HasMany(c => c.Sales)
                      .WithOne(s => s.Customer)
                      .HasForeignKey(s => s.CustomerId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.LoyaltyTransactions)
                      .WithOne(lt => lt.Customer)
                      .HasForeignKey(lt => lt.CustomerId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ─────────────────────────────────────────────────
            // Role — Unique RoleName index + Seed Data
            // ─────────────────────────────────────────────────
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => r.RoleName)
                      .IsUnique();
            });

            // Seed 4 default roles with full permissions JSON
            var allModules = new[] { "UserManagement", "Inventory", "POS", "Reports", "Promotions", "Customers", "PurchaseOrders", "AuditLogs" };

            string BuildPermissionsJson(Dictionary<string, bool[]> moduleFlags)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kvp in moduleFlags)
                {
                    dict[kvp.Key] = new { CanCreate = kvp.Value[0], CanRead = kvp.Value[1], CanUpdate = kvp.Value[2], CanDelete = kvp.Value[3] };
                }
                return System.Text.Json.JsonSerializer.Serialize(new { Modules = dict });
            }

            // Admin — all true
            var adminPerms = allModules.ToDictionary(m => m, _ => new[] { true, true, true, true });

            // Manager — all read/update, create on some, delete on none
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

            // Cashier — POS + read only on others
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

            // Customer — read only on products, own profile
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

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin",    Permissions = BuildPermissionsJson(adminPerms) },
                new Role { Id = 2, RoleName = "Manager",  Permissions = BuildPermissionsJson(managerPerms) },
                new Role { Id = 3, RoleName = "Cashier",  Permissions = BuildPermissionsJson(cashierPerms) },
                new Role { Id = 4, RoleName = "Customer", Permissions = BuildPermissionsJson(customerPerms) }
            );
        }
    }
}
