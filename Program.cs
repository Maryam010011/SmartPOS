using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SmartPOS.Components;
using SmartPOS.Services;
using SmartPOS.Services.MaryamJ;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Auth;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Data;
using Blazored.LocalStorage;
using System.Text;
using SmartPOS.Providers;
using SmartPOS.Models;
using SmartPOS.Web.Services.Shahzain;
using SmartPOS.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

builder.Services.AddScoped<IDbContextFactory<AppDbContext>>(sp =>
{
    var options = sp.GetRequiredService<DbContextOptions<AppDbContext>>();
    return new SimpleDbContextFactory(options);
});

// ─── API Controllers ───────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ─── HttpClient (required by FBRService and AIChatbotService) ──
builder.Services.AddHttpClient();

// ─── Shahzain's Service Registrations ─────────────────────────
builder.Services.AddScoped<IProductService,     ProductService>();
builder.Services.AddScoped<ICategoryService,    CategoryService>();
builder.Services.AddScoped<ISupplierService,    SupplierService>();
builder.Services.AddScoped<ISaleService,        SaleService>();
builder.Services.AddScoped<IReviewService,      ReviewService>();
builder.Services.AddScoped<IAIChatbotService,   AIChatbotService>();
builder.Services.AddScoped<IFBRService,         FBRService>();
builder.Services.AddScoped<IBERTService,        BERTService>();
//builder.Services.AddScoped<IInventoryService,   InventoryServiceStub>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IWeatherService,     WeatherService>();

// ─── Shared Cart State (Scoped = per Blazor Server circuit / user session) ───
builder.Services.AddScoped<CartStateService>();

// ─── MaryamY's Service Registrations ──────────────────────────
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

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
builder.Services.AddScoped<AuthenticationStateProvider, SmartPOS.Providers.CustomAuthStateProvider>();
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

// Register Purchase Order Service
builder.Services.AddScoped<IPurchaseOrderService, SmartPOS.Services.MaryamJ.PurchaseOrderService>();

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "SmartPOS.Web", "wwwroot")),
    RequestPath = ""
});
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await SmartPOS.Web.Data.DatabaseSeeder.SeedAsync(app.Services);
await app.RunAsync();

class SimpleDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;

    public SimpleDbContextFactory(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }
}
