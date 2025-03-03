
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HM_SkincareApp.Data;
using HM_SkincareApp.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// PASUL 2 - useri si roluri

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// PASUL 5 - useri si roluri

// CreateScope ofera acces la instanta curenta a aplicatiei
// variabila scope are un Service Provider folosit pentru a injecta dependentele 
// in aplicatie -> bd, cookie, sesiune, autentif, pachete, etc

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}



if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    pattern: "{controller=Bookmarks}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "profile",
    pattern: "{controller=Users}/{action=Profile}/{userId?}");

app.MapControllerRoute(
    name: "editProfile",
    pattern: "Users/EditProfile/{id?}",
    defaults: new { controller = "Users", action = "EditProfile" });


app.MapRazorPages();

app.Run();