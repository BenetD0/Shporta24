using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shporta24.Data;
using Shporta24.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity setup
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Sigurohu që databaza ekziston
    context.Database.EnsureCreated();

    // Shto kategori vetëm nëse nuk ekzistojnë
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
// Middleware
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

app.MapRazorPages(); // Identity Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
