using CharityApp.Models;
using CharityApp.Service;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Stripe settings from appsettings.json
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));


// Set Stripe API key
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// 🔹 Register database context
builder.Services.AddDbContext<CharityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

// 🔹 Register session services
builder.Services.AddSession();

builder.Services.AddSingleton<EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 Use session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
