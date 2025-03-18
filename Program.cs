using FcmsPortal.Services;
using FcmsPortalUI.Components;
using FcmsPortalUI.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AddressService>();
builder.Services.AddScoped<IFileAttachmentService, FileAttachmentService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<SchoolFeesService>();
builder.Services.AddScoped<GuardianService>();
builder.Services.AddScoped<StaffService>();

builder.Services.AddSingleton<ISchoolDataService, SchoolDataService>();

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

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
