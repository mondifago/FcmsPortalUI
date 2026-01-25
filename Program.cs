using FcmsPortal.Models;
using FcmsPortalUI.Components;
using FcmsPortalUI.Components.Account;
using FcmsPortalUI.Data;
using FcmsPortalUI.Infrastructure.Startup;
using FcmsPortalUI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("FcmsPortalUIContext");
void ConfigureDbContext(DbContextOptionsBuilder options)
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)
    );

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
}
builder.Services.AddPooledDbContextFactory<FcmsPortalUIContext>(ConfigureDbContext);
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IDbContextFactory<FcmsPortalUIContext>>().CreateDbContext());

// Authentication schemes
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

// Identity configuration
builder.Services.AddIdentityCore<Person>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddRoles<IdentityRole<int>>()
               .AddEntityFrameworkStores<FcmsPortalUIContext>()
               .AddSignInManager()
               .AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailSender<Person>, IdentityEmailSender>();
builder.Services.AddAuthorization();

// Blazor Auth 
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
// Application services
builder.Services.AddScoped<ISchoolDataService, SchoolDataService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<ExceptionHandlerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddScoped<IAccountInvitationService, AccountInvitationService>();

// QuickGrid with EF Core
builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    await RoleSeeder.EnsureRolesAsync(services);
    await AccountSeeder.EnsureSpecialAccountsAsync(services, config, app.Environment.IsDevelopment());
}
app.MapAccountServices();
app.Run();
