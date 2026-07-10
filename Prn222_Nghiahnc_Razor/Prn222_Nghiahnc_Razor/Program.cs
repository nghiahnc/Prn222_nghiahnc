using Microsoft.EntityFrameworkCore;
using MVC.Data2;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = ResolveContentRoot()
});

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Customer/Events/Index", "/Events");
    options.Conventions.AddPageRoute("/Customer/Events/Details", "/Events/Details/{id:int}");
    options.Conventions.AddPageRoute("/Customer/Categories/Index", "/EventCategories");
    options.Conventions.AddPageRoute("/Customer/Categories/Details", "/EventCategories/Details/{id:int}");
    options.Conventions.AddPageRoute("/Customer/Bookings/Index", "/Bookings");
    options.Conventions.AddPageRoute("/Customer/Bookings/Details", "/Bookings/Details/{id:int}");
    options.Conventions.AddPageRoute("/Customer/Tickets/Index", "/Tickets");
    options.Conventions.AddPageRoute("/Customer/Tickets/Qr", "/Tickets/Qr/{id:int}");
    options.Conventions.AddPageRoute("/Customer/Tickets/Scan", "/Tickets/Scan");
    options.Conventions.AddPageRoute("/Customer/Rewards/Index", "/Memberships/Rewards");
    options.Conventions.AddPageRoute("/Customer/Memberships/Index", "/Memberships");
    options.Conventions.AddPageRoute("/Customer/Login", "/Logins/Login");
    options.Conventions.AddPageRoute("/Customer/Logout", "/Logins/Logout");
});
builder.Services.AddDbContext<DemoMVC2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerWorkflowService, CustomerWorkflowService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

static string ResolveContentRoot()
{
    var currentDirectory = Directory.GetCurrentDirectory();
    if (Directory.Exists(Path.Combine(currentDirectory, "wwwroot"))
        && Directory.Exists(Path.Combine(currentDirectory, "Pages")))
    {
        return currentDirectory;
    }

    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory != null)
    {
        if (Directory.Exists(Path.Combine(directory.FullName, "wwwroot"))
            && Directory.Exists(Path.Combine(directory.FullName, "Pages")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    return currentDirectory;
}
