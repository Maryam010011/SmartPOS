using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartPOS.Components;
using SmartPOS.Services.MaryamJ;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Auth;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Data;
using Blazored.LocalStorage;
using System.Text;
using SmartPOS.Providers;
using SmartPOS.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── JWT Authentication ──
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAbove", p => p.RequireRole("Admin", "Manager"));
    options.AddPolicy("CashierOrAbove", p => p.RequireRole("Admin", "Manager", "Cashier"));
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Register Auth State & Services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthService, ServerAuthService>();

// HttpClient for client-side services
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

// Register User Management Service
builder.Services.AddScoped<IUserService, SmartPOS.Services.MaryamJ.UserService>();

// Register Role Management Service
builder.Services.AddScoped<IRoleService, SmartPOS.Services.MaryamJ.RoleService>();

// Register Customer Management Service
builder.Services.AddScoped<ICustomerService, SmartPOS.Services.MaryamJ.CustomerService>();

// Register Audit Log Service
builder.Services.AddScoped<IAuditLogService, SmartPOS.Services.MaryamJ.AuditLogService>();

// Register Promotion Management Service
builder.Services.AddScoped<IPromotionService, SmartPOS.Services.MaryamJ.PromotionService>();

// Register Product Management Service
builder.Services.AddScoped<IProductService, SmartPOS.Services.MaryamJ.ProductService>();

// Register Category Management Service
builder.Services.AddScoped<ICategoryService, SmartPOS.Services.MaryamJ.CategoryService>();

// Register Inventory Management Service
builder.Services.AddScoped<IInventoryService, SmartPOS.Services.MaryamJ.InventoryService>();

// Register Supplier Service
builder.Services.AddScoped<ISupplierService, SmartPOS.Services.MaryamJ.SupplierService>();

// Register Purchase Order Service
builder.Services.AddScoped<IPurchaseOrderService, SmartPOS.Services.MaryamJ.PurchaseOrderService>();

// Register Weather Service
builder.Services.AddScoped<IWeatherService, SmartPOS.Services.MaryamJ.WeatherService>();

// Enable Web API Controllers
builder.Services.AddControllers();

var app = builder.Build();

// 5. Auto-create DB + tables + seed default data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // Since we are using a Database-First approach, database migrations on startup are bypassed.
        // db.Database.Migrate();

        // Seed Roles if empty
        if (!db.Roles.Any())
        {
            db.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "Manager" },
                new Role { Name = "Cashier" },
                new Role { Name = "Customer" }
            );
            db.SaveChanges();
        }

        // Seed default Admin user if not present
        if (!db.Users.Any(u => u.Email == "admin@smartpos.com"))
        {
            var adminRole = db.Roles.First(r => r.Name == "Admin");
            db.Users.Add(new User
            {
                Name = "Admin",
                Email = "admin@smartpos.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                RoleId = adminRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        }

        logger.LogInformation("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed. Check your SQL Server connection string in appsettings.json.");
        throw; // Re-throw so the app fails fast — prevents runtime errors later
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
