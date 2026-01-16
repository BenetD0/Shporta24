using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shporta24.Data;
using Shporta24.Models;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // ?? SHUMË E RËNDËSISHME
.AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// ================= SEED DATA + ROLES =================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    context.Database.EnsureCreated();

    // ====== ROLES ======
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ====== ADMIN DEFAULT ======
    string adminEmail = "admin@shporta24.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // ====== CATEGORIES ======
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "Elektronikë" },
            new Category { Name = "Auto Pjesë" },
            new Category { Name = "Shtëpi" }
        );
        context.SaveChanges();
    }
}

// ================= MIDDLEWARE =================
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

app.MapRazorPages(); // Identity
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
