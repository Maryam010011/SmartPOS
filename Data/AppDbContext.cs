using Microsoft.EntityFrameworkCore;
using SmartPOS.Models;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<Promotion> Promotions { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Sale> Sales { get; set; }
    public virtual DbSet<SaleItem> SaleItems { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Inventory> Inventories { get; set; }
    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public virtual DbSet<POLineItem> POLineItems { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Role ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name)
                  .HasMaxLength(50)
                  .IsRequired();
        });

        // ── Permission ────────────────────────────────────────────────────────
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_Permissions_RoleId");

            entity.Property(e => e.Module)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.CanCreate).HasDefaultValue(false);
            entity.Property(e => e.CanRead).HasDefaultValue(false);
            entity.Property(e => e.CanUpdate).HasDefaultValue(false);
            entity.Property(e => e.CanDelete).HasDefaultValue(false);

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Permissions)
                  .HasForeignKey(e => e.RoleId);
        });

        // ── User ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(e => e.RoleId);
        });

        // ── Customer ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Customers_UserId").IsUnique();
            entity.HasIndex(e => e.Email, "IX_Customers_Email").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
            entity.Property(e => e.TotalSpent).HasPrecision(10, 2).HasDefaultValue(0.00m);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(e => e.User)
                  .WithOne(u => u.Customer)
                  .HasForeignKey<Customer>(e => e.UserId);
        });

        // ── AuditLog ──────────────────────────────────────────────────────────
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AuditLogs_UserId");

            entity.Property(e => e.Action).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Module).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IPAddress).HasMaxLength(45);
            entity.Property(e => e.Details).HasColumnType("text");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(e => e.UserId);
        });

        // ── Promotion ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_Promotions_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Value).HasPrecision(10, 2);
            entity.Property(e => e.MinOrderValue).HasPrecision(10, 2).HasDefaultValue(0.00m);
            entity.Property(e => e.UsageCount).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // ── Category ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageURL).HasMaxLength(255);

            entity.HasOne(e => e.ParentCategory)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Supplier ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Suppliers_Email").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.ContactNo).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // ── Product ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.SKU, "IX_Products_SKU").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.SKU).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageURL).HasMaxLength(255);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.CostPrice).HasPrecision(10, 2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.Products)
                  .HasForeignKey(e => e.SupplierId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ── Sale ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(10, 2).HasDefaultValue(0.00m);
            entity.Property(e => e.TaxAmount).HasPrecision(10, 2);
            entity.Property(e => e.SaleDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue(SaleStatus.Completed);

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Sales)
                  .HasForeignKey(e => e.CustomerId)
                  .IsRequired(false);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Sales)
                  .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Promotion)
                  .WithMany(p => p.Sales)
                  .HasForeignKey(e => e.PromoId)
                  .IsRequired(false);
        });

        // ── SaleItem ──────────────────────────────────────────────────────────
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasIndex(e => e.SaleId, "IX_SaleItems_SaleId");

            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.Property(e => e.LineTotal).HasPrecision(10, 2);

            entity.HasOne(e => e.Sale)
                  .WithMany(s => s.SaleItems)
                  .HasForeignKey(e => e.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.SaleItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Review ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.Sentiment).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Reviews)
                  .HasForeignKey(e => e.CustomerId);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Reviews)
                  .HasForeignKey(e => e.ProductId);
        });

        // ── Inventory ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_Inventories_ProductId").IsUnique();

            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(e => e.Product)
                  .WithOne(p => p.Inventory)
                  .HasForeignKey<Inventory>(e => e.ProductId);
        });

        // ── PurchaseOrder ─────────────────────────────────────────────────────
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.Property(e => e.TotalCost).HasPrecision(10, 2);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue(POStatus.Draft);
            entity.Property(e => e.Notes).HasColumnType("text");

            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.PurchaseOrders)
                  .HasForeignKey(e => e.SupplierId);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.PurchaseOrders)
                  .HasForeignKey(e => e.UserId);
        });

        // ── POLineItem ────────────────────────────────────────────────────────
        modelBuilder.Entity<POLineItem>(entity =>
        {
            entity.HasIndex(e => e.POID, "IX_POLineItems_POID");

            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);

            entity.HasOne(e => e.PurchaseOrder)
                  .WithMany(po => po.LineItems)
                  .HasForeignKey(e => e.POID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.POLineItems)
                  .HasForeignKey(e => e.ProductId);
        });

        // ── Payment ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.SaleId, "IX_Payments_SaleId").IsUnique();

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasDefaultValue(PaymentStatus.Pending);
            entity.Property(e => e.TransactionRef).HasMaxLength(255);

            entity.HasOne(e => e.Sale)
                  .WithOne(s => s.Payment)
                  .HasForeignKey<Payment>(e => e.SaleId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
