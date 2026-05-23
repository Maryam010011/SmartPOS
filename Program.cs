using Microsoft.EntityFrameworkCore;
using SmartPOS.Components;
using SmartPOS.Web.Data;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Services.Shahzain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<IInventoryService,   InventoryServiceStub>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

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
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
