using Prn222_Nghiahnc_BlazorApp1.Components;
using Prn222_Nghiahnc_BlazorApp1.Hubs;
using Prn222_Nghiahnc_BlazorApp1.Services;
using Prn222_Nghiahnc_BlazorApp1.Workers;
using Microsoft.EntityFrameworkCore;
using MVC.Data2;
using Repositories;
using Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ─── Razor / Blazor ────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();

// ─── Authentication (Cookie-based) ────────────────────────────────────────
// Technique: Cookies — auth cookie with 60-min expiry
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name        = "admin_auth_token";
        options.Cookie.HttpOnly    = true;         // Prevent JS access
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite    = SameSiteMode.Strict;
        options.LoginPath          = "/login";
        options.AccessDeniedPath   = "/access-denied";
        options.ExpireTimeSpan     = TimeSpan.FromMinutes(60);
        options.SlidingExpiration  = true;         // Extend on activity
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

// ─── SignalR ───────────────────────────────────────────────────────────────
// Technique: SignalR — real-time admin notifications + live badge counts
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval   = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// ─── Database ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<DemoMVC2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Repositories (DI) ────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository,               UserRepository>();
builder.Services.AddScoped<ITransactionRepository,        TransactionRepository>();
builder.Services.AddScoped<ISystemSettingRepository,      SystemSettingRepository>();
builder.Services.AddScoped<IPaymentGatewayConfigRepository, PaymentGatewayConfigRepository>();

// ─── Business Services (DI) ───────────────────────────────────────────────
builder.Services.AddScoped<IUserService,                  UserService>();
builder.Services.AddScoped<ITransactionService,           TransactionService>();
builder.Services.AddScoped<ISystemSettingService,         SystemSettingService>();
builder.Services.AddScoped<IPaymentGatewayConfigService,  PaymentGatewayConfigService>();

// ─── Admin Session Service ────────────────────────────────────────────────
// Technique: Session-equivalent — scoped per Blazor circuit (browser tab)
// Preserves filter/search state as admin navigates between pages
builder.Services.AddScoped<AdminSessionService>();

// ─── Background Worker ────────────────────────────────────────────────────
// Technique: Worker — polls DB every 30s, pushes SignalR on count change
builder.Services.AddHostedService<AdminNotificationWorker>();

// ─── Build App ────────────────────────────────────────────────────────────
var app = builder.Build();

// ─── Middleware Pipeline ───────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// ─── Endpoints ────────────────────────────────────────────────────────────
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// SignalR hub endpoint
app.MapHub<AdminHub>("/hubs/admin");

app.Run();
