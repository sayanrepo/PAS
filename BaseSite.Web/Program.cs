using BaseSite.Web.Components;
using BaseSite.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.AddServiceDefaults();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".keys")))
    .SetApplicationName("BaseSite.Web");
builder.Services.AddMudServices();
builder.Services.AddRazorPages();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddSingleton<PrintSessionCookie>();
builder.Services.AddScoped<ApiSession>();
builder.Services.AddSingleton<ProfileImageStore>();
void ConfigureApiClient(HttpClient httpClient)
{
    var discoveredAddress = builder.Configuration["services:basesite-api:http:0"];
    var configuredAddress = builder.Configuration["BaseSiteApi:BaseUrl"] ?? "http://localhost:5184";
    httpClient.BaseAddress = new Uri(discoveredAddress ?? configuredAddress);
    httpClient.Timeout = TimeSpan.FromSeconds(30);
}
builder.Services.AddHttpClient<BaseSiteApiClient>(ConfigureApiClient);
builder.Services.AddHttpClient<OrderPrintClient>(ConfigureApiClient);
builder.Services.AddHttpClient<SalePrintClient>(ConfigureApiClient);
builder.Services.AddHttpClient<ServicePrintClient>(ConfigureApiClient);
builder.Services.AddHttpClient<PaymentPrintClient>(ConfigureApiClient);
builder.Services.AddHttpClient<DeliveryPrintClient>(ConfigureApiClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

// Serve profile images created after the application was built or published.
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorPages();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapDefaultEndpoints();

app.Run();
