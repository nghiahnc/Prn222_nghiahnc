using Microsoft.EntityFrameworkCore;
using MVC.Data2;
using Prn222_MembershipWorker;
using Repositories;
using Services;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<DemoMVC2Context>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IMembershipMaintenanceService, MembershipMaintenanceService>();
builder.Services.Configure<MembershipWorkerOptions>(
    builder.Configuration.GetSection(MembershipWorkerOptions.SectionName));
builder.Services.AddHostedService<MembershipMaintenanceWorker>();

var host = builder.Build();
host.Run();
