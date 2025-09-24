using ElectronicsStoreMVC.Services;
using Microsoft.EntityFrameworkCore;
using ElectronicsStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using sib_api_v3_sdk.Client;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);
Env.Load();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySQL(connectionString);
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    }
    ).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// Bind secrets from configuration
builder.Services.Configure<BrevoSettings>(builder.Configuration.GetSection("BrevoSettings"));
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPalSettings"));

// Apply Brevo API key to SDK
var brevoApiKey = builder.Configuration["BrevoSettings:ApiKey"];
if (string.IsNullOrEmpty(brevoApiKey))
{
    throw new InvalidOperationException("Brevo API key is missing! Check your .env file or environment variables.");
}
Configuration.Default.ApiKey.Add("api-key", brevoApiKey);

//Configuration.Default.ApiKey.Add("api-key", builder.Configuration["BrevoSettings:ApiKey"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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


using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
    var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
    await DatabaseInitializer.SeedDataAsync(userManager, roleManager);


}

app.Run();
