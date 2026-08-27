using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
Environment.SetEnvironmentVariable("PantaEntitiesConnection", builder.Configuration.GetConnectionString("PantaEntities"));
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; });

var app = builder.Build();
app.UseExceptionHandler("/Home/Error");
app.UseHttpsRedirection();
foreach (var directory in new[] { "Contents", "Images", "GuideBook" })
    app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, directory)), RequestPath = $"/{directory}" });
app.UseRouting();
app.UseSession();
app.Use(async (context, next) =>
{
    BaseSite.Controllers.CustomAuthorizeAttribute.SetCurrentContext(context);
    await next();
});
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
