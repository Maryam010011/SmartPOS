using Microsoft.EntityFrameworkCore;
using SmartPOS.Components;
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

// Register User Management Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.IUserService, SmartPOS.Services.MaryamJ.UserService>();

// Register Role Management Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.IRoleService, SmartPOS.Services.MaryamJ.RoleService>();

// Register Customer Management Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.ICustomerService, SmartPOS.Services.MaryamJ.CustomerService>();

// Register Promotion Management Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.IPromotionService, SmartPOS.Services.MaryamJ.PromotionService>();

// Register Product Management Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.IProductService, SmartPOS.Services.MaryamJ.ProductService>();

// Register Category Management Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.ICategoryService, SmartPOS.Services.MaryamJ.CategoryService>();

// Register Inventory Management Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.IInventoryService, SmartPOS.Services.MaryamY.InventoryService>();

// Register Supplier Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.ISupplierService, SmartPOS.Services.MaryamJ.SupplierService>();

// Register Purchase Order Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.IPurchaseOrderService, SmartPOS.Services.MaryamY.PurchaseOrderService>();

// Register Weather Service
builder.Services.AddScoped<SmartPOS.Shared.Interfaces.IWeatherService, SmartPOS.Services.MaryamJ.WeatherService>();

// Enable Web API Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Seed roles on startup (safe — skips if already seeded)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
