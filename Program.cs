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

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
                .AddIdentityCookies();

builder.Services.AddScoped<ISchoolDataService, SchoolDataService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<ExceptionHandlerService>();


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
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});


builder.Services.AddIdentityCore<Person>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddEntityFrameworkStores<FcmsPortalUIContext>()
               .AddSignInManager()
               .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<Person>, IdentityNoOpEmailSender>();

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();
app.Run();
