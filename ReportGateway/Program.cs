using ReportGateway;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var reportOptions = builder.Configuration.GetSection(ReportServerOptions.SectionName).Get<ReportServerOptions>();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        handler.Credentials =
            new NetworkCredential(
                reportOptions?.Username,
                reportOptions?.Password,
                reportOptions?.Domain);

        handler.PreAuthenticate = true;
    });

//--------------------------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapReverseProxy();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();