using FcmsPortal.Models;
using FcmsPortal.Services;
using FcmsPortalUI.Components;
using FcmsPortalUI.Components.Account;
using FcmsPortalUI.Data;
using FcmsPortalUI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("FcmsPortalUIContext");
builder.Services.AddDbContext<FcmsPortalUIContext>(options =>
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
});

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

builder.Services.AddSingleton<IEmailSender<Person>, IdentityNoOpEmailSender>();
builder.Services.AddAuthorization();

// Blazor Auth 
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Application services
builder.Services.AddScoped<ISchoolDataService, SchoolDataService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<ExceptionHandlerService>();

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
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    // Seed roles once
    await RoleSeeder.EnsureRolesAsync(services);

    // Seed Developer & Principal backup accounts (reads secrets from IConfiguration)
    await AccountSeeder.EnsureSpecialAccountsAsync(services, config, app.Environment.IsDevelopment());
}



app.Run();
