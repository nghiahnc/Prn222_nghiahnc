using Microsoft.EntityFrameworkCore;
using MVC.Data2;
using Repositories;
using Services;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = ResolveContentRoot()
});

// MVC
builder.Services.AddControllersWithViews();

// DbContext hay do chổ này tại vì mvc cũng có data.DemoMVC2Context mà repo cx có v 
builder.Services.AddDbContext<DemoMVC2Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Session (chuẩn hơn)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// DI
builder.Services.AddScoped<IUserRepository, UserRepository>(); 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerWorkflowService, CustomerWorkflowService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static string ResolveContentRoot()
{
    var currentDirectory = Directory.GetCurrentDirectory();
    if (Directory.Exists(Path.Combine(currentDirectory, "wwwroot"))
        && Directory.Exists(Path.Combine(currentDirectory, "Views")))
    {
        return currentDirectory;
    }

    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory != null)
    {
        if (Directory.Exists(Path.Combine(directory.FullName, "wwwroot"))
            && Directory.Exists(Path.Combine(directory.FullName, "Views")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    return currentDirectory;
}
