using BaseSite.Models;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseSite.Controllers;

public sealed class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private static readonly AsyncLocal<HttpContext> Current = new();
    private readonly OPERATIONS[] allowedOperations;
    public CustomAuthorizeAttribute(params OPERATIONS[] operations) => allowedOperations = operations;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        Current.Value = context.HttpContext;
        var session = new LegacySession(context.HttpContext);
        var operations = session["UserOperations"] as List<OPERATIONS>;
        if (operations is not null && allowedOperations.Any(operations.Contains)) return;
        if (session["PantaUser"] is null)
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.Result = new RedirectResult($"/Home/Index?returnurl={Uri.EscapeDataString(returnUrl)}");
            LogManager.Log_Logs_Add((int)DB_Table.Account_Users, 0, 0, context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty, (int)LogActivity.SessionTimeout, " ");
            return;
        }
        context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
    }

    public static bool isAuthorize(HttpContext context, OPERATIONS operation) => new LegacySession(context)["UserOperations"] is List<OPERATIONS> operations && operations.Contains(operation);
    public static bool isAuthorize(OPERATIONS operation) => Current.Value is not null && isAuthorize(Current.Value, operation);
    public static void SetCurrentContext(HttpContext context) => Current.Value = context;
    public static Account_Users getCurrentUser() => Current.Value is null ? null : getCurrentUser(Current.Value);
    public static Account_Users getCurrentUser(HttpContext context) => new LegacySession(context)["PantaUser"] as Account_Users;
}
