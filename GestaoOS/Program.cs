using GestaoOS.Data;
using GestaoOS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using GestaoOS.Services;


var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDbInitializer, DbInitializer>();



builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; 
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})


.AddEntityFrameworkStores<ApplicationDbContext>()

.AddDefaultTokenProviders(); 

builder.Services.AddRazorPages(); 

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});


builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitializer = services.GetRequiredService<IDbInitializer>();
    await dbInitializer.Initialize();
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