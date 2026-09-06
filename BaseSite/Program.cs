using BaseSite.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.AddServiceDefaults();
Environment.SetEnvironmentVariable("PantaEntitiesConnection", builder.Configuration.GetConnectionString("PantaEntities"));

builder.Services.AddProblemDetails();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".keys")))
    .SetApplicationName("BaseSite.Api");
builder.Services
    .AddAuthentication(AccessTokenAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, AccessTokenAuthenticationHandler>(
        AccessTokenAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<AccessTokenService>();
builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy("Web", policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseCors("Web");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
