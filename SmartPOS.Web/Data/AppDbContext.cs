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
        }
    }
}
