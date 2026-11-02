using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

var isTesting = builder.Environment.IsEnvironment("Testing");
var useInMemoryDatabase = isTesting && builder.Configuration.GetValue<bool>("Testing:UseInMemoryDatabase");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? (isTesting ? "Server=localhost;Database=GiftOfTheGiversTests;User Id=test;Password=test;" : null);

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' must be configured outside the Testing environment.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useInMemoryDatabase)
    {
        var databaseName = builder.Configuration["Testing:DatabaseName"] ?? "GiftOfTheGiversTests";
        options.UseInMemoryDatabase(databaseName);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

builder.Services.Configure<AzureFunctionsOptions>(builder.Configuration.GetSection("AzureFunctions"));
builder.Services.AddHttpClient<IAzureFunctionClient, AzureFunctionClient>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (useInMemoryDatabase)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(services);
}
else if (!isTesting)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await SeedData.InitializeAsync(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

public partial class Program
{
}
