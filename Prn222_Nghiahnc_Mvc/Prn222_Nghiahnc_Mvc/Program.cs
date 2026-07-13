using Services;
using Microsoft.EntityFrameworkCore;
using MVC.Data2;
using Prn222_Nghiahnc_Mvc.BackgroundServices;
using Repositories;


var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext: dùng DbContext nằm trong Data/DAL project
builder.Services.AddDbContext<DemoMVC2Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Email config
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Repositories - DAL
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Services - BLL
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IEventReminderService, EventReminderService>();

// Background service
builder.Services.AddHostedService<EventReminderBackgroundService>();

//SERRVICES
builder.Services.AddScoped<IOrganizerReportRepository, OrganizerReportRepository>();
builder.Services.AddScoped<IOrganizerReportService, OrganizerReportService>();
// auditlogs
builder.Services.AddScoped<IOrganizerAuditLogRepository, OrganizerAuditLogRepository>();
builder.Services.AddScoped<IOrganizerAuditLogService, OrganizerAuditLogService>();
// QRTicket
builder.Services.AddScoped<ITicketIssueRepository, TicketIssueRepository>();
builder.Services.AddScoped<ITicketIssueService, TicketIssueService>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();



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

app.MapGet("/test-login-organizer", context =>
{
    context.Session.SetInt32("UserId", 1002);
    context.Session.SetInt32("Role", 2);

    context.Response.Redirect("/OrganizerReports/Dashboard");

    return Task.CompletedTask;
});

app.Run();