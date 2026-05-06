using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Respositories;
using onlineShop.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configuration de la base de données
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration AppSettings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Configuration des sessions pour le panier
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "OnlineShop.Session";
});

// Configuration de l'authentification par cookies
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "OnlineShop.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Configuration de l'autorisation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("ClientOnly", policy => policy.RequireRole("client"));
});

builder.Services.AddHttpContextAccessor();

// Enregistrement des repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProduitRepository, ProduitRepository>();
builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();
builder.Services.AddScoped<IFactureRepository, FactureRepository>();
builder.Services.AddScoped<IReclamationRepository, ReclamationRepository>();

// Enregistrement des services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<ICommandeService, CommandeService>();
builder.Services.AddScoped<IFactureService, FactureService>();
builder.Services.AddScoped<IPanierService, PanierService>();

// Ajoutez ces services
builder.Services.AddHttpClient<IProductAssistantService, ProductAssistantService>();
builder.Services.AddScoped<IProductAssistantService, ProductAssistantService>();

// Si vous utilisez la version avec règles
builder.Services.AddScoped<IProductAssistantService, ProductAssistantService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();