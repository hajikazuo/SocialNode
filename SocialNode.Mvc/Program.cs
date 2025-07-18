using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNode.Mvc.Context;
using SocialNode.Mvc.Models;
using SocialNode.Mvc.Services;
using SocialNode.Mvc.Settings;
using SocialNode.Mvc.TagHelpers;
using System;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var menuJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "navMenu.json");
var json = File.ReadAllText(menuJsonPath);

var menuConfig = JsonSerializer.Deserialize<JsonMenu>(json);
builder.Services.AddSingleton(menuConfig);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SocialNodeContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, Role>()
                  .AddEntityFrameworkStores<SocialNodeContext>()
                  .AddDefaultTokenProviders();

builder.Services.AddScoped<INeo4jService, Neo4jService>();
builder.Services.AddScoped<ISeedService, SeedService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.LogoutPath = "/Auth/Logout";
    options.SlidingExpiration = true;
});

builder.Services.Configure<Neo4jSettings>(builder.Configuration.GetSection("Neo4j"));

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(RT.Comb.Provider.Sql);
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
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SocialNodeContext>();
    db.Database.Migrate();

    var seedUserService = scope.ServiceProvider.GetRequiredService<ISeedService>();
    seedUserService.Seed();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
