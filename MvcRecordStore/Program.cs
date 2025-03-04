using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcRecordStore.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StoreDbContextConnection") ?? throw new InvalidOperationException("Connection string 'StoreDbContextConnection' not found.");

builder.Services.AddDbContext<StoreDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<StoreUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<StoreDbContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
