using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CookBook.Data;
using CookBook.Services; 
using CookBook.Models;
using CookBook.Areas.Identity.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 1. Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Configuração do Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// --- CORREÇÃO DE INJEÇÃO DE DEPENDÊNCIA (DI) ---
// Estas linhas são essenciais para que o sistema saiba como resolver 
// a interface IReceitaService (e outras) para os seus Controllers.

// Registra o GamificationService
builder.Services.AddScoped<GamificationService>(); 

// ESSENCIAL: Diz ao DI: Se alguém pedir IReceitaService, entregue uma instância de ReceitaService
builder.Services.AddScoped<IReceitaService, ReceitaService>(); 
// ESSENCIAL: Adicionei também o IUserService, caso ele seja usado em algum lugar
builder.Services.AddScoped<IUserService, UserService>(); 

// ----------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

// UseAuthentication deve vir antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); 

app.Run();