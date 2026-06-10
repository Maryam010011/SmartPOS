using Microsoft.EntityFrameworkCore;
using SmartPOS.Components;
using SmartPOS.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");

// Remove UseHttpsRedirection - Railway handles HTTPS externally
// app.UseHttpsRedirection();  ← COMMENT THIS OUT

app.UseStaticFiles(); // ← ADD THIS for CSS/JS to work
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();