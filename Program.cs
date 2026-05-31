using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartPOS.Components;
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

// ── JWT + Cookie Authentication ──
var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "SmartPOSAuth";
    options.DefaultChallengeScheme = "SmartPOSAuth";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
})
.AddPolicyScheme("SmartPOSAuth", "SmartPOS Auth", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
            return "Bearer";
        return "Cookies";
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
    options.AddPolicy("CashierOrAbove", policy => policy.RequireRole("Cashier", "Manager", "Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

builder.Services.AddCascadingAuthenticationState();

// Register Auth Service
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

// ── Login API Endpoint ──
app.MapPost("/api/auth/login", async (HttpContext httpContext, LoginRequestDto request, IAuthService authService) =>
{
    var result = await authService.Login(request);
    if (!result.Success || result.Data == null)
        return Results.Json(new { success = false, message = result.Message }, statusCode: 401);

    var user = result.Data.User;

    var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(System.Security.Claims.ClaimTypes.Name, user.Name),
        new(System.Security.Claims.ClaimTypes.Email, user.Email),
        new(System.Security.Claims.ClaimTypes.Role, user.RoleName)
    };

    var identity = new System.Security.Claims.ClaimsIdentity(claims, "Cookies");
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

    await httpContext.SignInAsync("Cookies", principal, new Microsoft.AspNetCore.Authentication.AuthenticationProperties
    {
        IsPersistent = true,
        ExpiresUtc = DateTime.UtcNow.AddHours(2)
    });

    return Results.Json(new
    {
        success = true,
        message = "Login successful.",
        token = result.Data.Token,
        user = new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            role = user.RoleName
        }
    });
});

// ── Logout API Endpoint ──
app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync("Cookies");
    return Results.Json(new { success = true, message = "Logged out." });
});

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
