using FcmsPortal.Services;
using FcmsPortalUI.Components;
using FcmsPortalUI.Data;
using FcmsPortalUI.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISchoolDataService, SchoolDataService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<ExceptionHandlerService>();

builder.Services.AddDbContext<FcmsPortalUIContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("FcmsPortalUIContext"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("FcmsPortalUIContext")),
        mySqlOptions => mySqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)
    ));



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

app.Run();
