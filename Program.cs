using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartPOS.Components;
using SmartPOS.Services.MaryamJ;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Auth;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;

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

// Register Auth State & Services
builder.Services.AddScoped<SmartPOS.Services.MaryamJ.AuthStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, SmartPOS.Services.MaryamJ.AuthService>();

// Register User Management Service
builder.Services.AddScoped<IUserService, SmartPOS.Services.MaryamJ.UserService>();

// Register Role Management Service
builder.Services.AddScoped<IRoleService, SmartPOS.Services.MaryamJ.RoleService>();

// Register Customer Management Service
builder.Services.AddScoped<ICustomerService, SmartPOS.Services.MaryamJ.CustomerService>();

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

// Seed data on startup (safe — skips if already seeded)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context);
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
