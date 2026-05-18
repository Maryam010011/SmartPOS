using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Models;

namespace SmartPOS.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<CashierLog> CashierLogs { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerLog> CustomerLogs { get; set; }

    public virtual DbSet<ManagerAction> ManagerActions { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=SmartPOS_DB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AdminLog__3214EC07DF8E601A");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminLogs)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__AdminLogs__Admin__6B24EA82");
        });

        modelBuilder.Entity<CashierLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CashierL__3214EC0705F884A8");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Cashier).WithMany(p => p.CashierLogs)
                .HasForeignKey(d => d.CashierId)
                .HasConstraintName("FK__CashierLo__Cashi__72C60C4A");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Customers_UserId").IsUnique();

            entity.Property(e => e.TotalSpent).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithOne(p => p.Customer).HasForeignKey<Customer>(d => d.UserId);
        });

        modelBuilder.Entity<CustomerLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07C8C74AF6");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerLogs)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__CustomerL__Custo__76969D2E");
        });

        modelBuilder.Entity<ManagerAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ManagerA__3214EC07E6364934");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Manager).WithMany(p => p.ManagerActions)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__ManagerAc__Manag__6EF57B66");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_Permissions_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.Property(e => e.MinOrderValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
