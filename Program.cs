using Microsoft.EntityFrameworkCore;
using IT15_LabExam_Tero.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container (MVC).
builder.Services.AddControllersWithViews();

// 2. EXACT CODE: Register AppDbContext using Pomelo MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// 3. EXACT CODE: Automatically create the database without typing migration commands!
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // This command checks XAMPP. If techcoredb doesn't exist, it builds it and the tables.
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred creating the DB: " + ex.Message);
    }
}

// 4. Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();