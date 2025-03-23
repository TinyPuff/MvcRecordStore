using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcRecordStore.Data;
using MvcRecordStore.Extensions;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Storage.EntityFrameworkCore;
using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Storage.EntityFrameworkCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StoreDbContextConnection") ?? throw new InvalidOperationException("Connection string 'StoreDbContextConnection' not found.");

builder.Services.AddDbContext<StoreDbContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddDefaultIdentity<StoreUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StoreDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCustomServices();

// Configure Parbad
builder.Services.AddParbad()
    .ConfigureGateways(gateways =>
    {
        gateways
            .AddParbadVirtual()
            .WithOptions(options => options.GatewayPath = "/virtual-gateway");
    })
    .ConfigureHttpContext(builder => builder.UseDefaultAspNetCore())
    .ConfigureStorage(builder => builder.UseDistributedCache());
    /* .ConfigureStorage(builder =>
    {
        builder.UseEfCore(options => 
        {
            // Example 1: Using SQL Server
            var assemblyName = typeof(Program).Assembly.GetName().Name;
            options.ConfigureDbContext = db => db.UseSqlServer("StoreDbContextConnection", sql => sql.MigrationsAssembly(assemblyName));
        });
    }); */

var app = builder.Build();

// Seed the database.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreDbContext>();
    context.Database.EnsureCreated();
    try
    {
        await SeedData.Initialize(services, context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

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

// Registering Parbad's virtual gateway
app.UseParbadVirtualGateway();
app.Run();
