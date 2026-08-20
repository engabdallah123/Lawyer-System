using System.Globalization;
using App.Application;
using App.Infrastructure;
using App.Infrastructure.Database;
using App.Infrastructure.Identity.Seeder;
using App.Web.Components;
using Microsoft.EntityFrameworkCore;
using App.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using QuestPDF.Infrastructure;
using Shared.Application;
using Shared.Infrastructure;

// Configure QuestPDF Community License
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Components & Interactive Server Mode
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register MudBlazor Services with Snackbar configuration
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
});

// Authentication & Authorization Services
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>(sp =>
    (CustomAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

// Register Clean Architecture Layers
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddSharedApplication();
builder.Services.AddAppInfrastructure(builder.Configuration);
builder.Services.AddAppApplication();

// Configure Arabic Culture & Localization
var supportedCultures = new[]
{
    new CultureInfo("ar-EG"),
    new CultureInfo("ar-SA"),
    new CultureInfo("ar")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar-EG");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Seed Database and Identity Data safely
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<LegalPracticeDbContext>();
    await dbContext.Database.MigrateAsync();
    await IdentityDataSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetService<ILogger<Program>>();
    logger?.LogWarning(ex, "Note: Identity seeding will take effect once the SQL Server database is reachable.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-EG"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/api/invoices/{id:guid}/pdf", async (Guid id, MediatR.IMediator mediator, App.Application.Finance.Services.IInvoicePdfService pdfService) =>
{
    var result = await mediator.Send(new App.Application.Finance.Queries.GetInvoiceById.GetInvoiceByIdQuery(id));
    if (result.IsFailure || result.Value is null)
    {
        return Results.NotFound("لم يتم العثور على الفاتورة المطلوبة");
    }

    var pdfBytes = pdfService.GenerateInvoicePdf(result.Value);
    return Results.File(pdfBytes, "application/pdf", $"Invoice-{result.Value.InvoiceNumber}.pdf");
});

app.MapStaticAssets();
app.MapRazorComponents<global::App.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
