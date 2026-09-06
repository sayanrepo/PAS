using System.Security.Claims;
using BaseSite.Api.Authentication;
using BaseSite.Api.Contracts;
using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;

namespace BaseSite.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController(AccessTokenService accessTokens) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var user = AccountManager.Login(request.UserName, request.Password, HttpContext.Connection.RemoteIpAddress?.ToString());
        if (user.Id == 0)
        {
            LogManager.Log_Logs_Add((int)DB_Table.Account_Users, 0, 0,
                HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.LoginFailed,
                $"نام کاربری وارد شده: {request.UserName}");
            return Unauthorized(new ProblemDetails { Title = "نام کاربری یا رمز عبور صحیح نیست." });
        }

        var roles = user.Account_UserPost.Count > 0
            ? AccountManager.Account_Operation_Get((AccountRole)user.Account_UserPost.First().PostId)
                .Distinct()
                .Select(operation => operation.ToString())
                .ToArray()
            : [];

        var currentUser = new CurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            ImagePath = user.ImagePath ?? "profile.png",
            Roles = roles
        };

        LogManager.Log_Logs_Add((int)DB_Table.Account_Users, user.Id, user.Id,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Login, string.Empty);
        return Ok(accessTokens.Create(currentUser));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserDto> Me() => Ok(new CurrentUserDto
    {
        Id = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        UserName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
        FullName = User.FindFirstValue("panta:full_name") ?? string.Empty,
        ImagePath = User.FindFirstValue("panta:image_path") ?? "profile.png",
        Roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray()
    });

    [Authorize]
    [HttpPost("change-password")]
    public IActionResult ChangePassword(ChangePasswordRequest request)
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        var message = AccountManager.Account_User_ChangePassword(userId, request.UserName, request.CurrentPassword, request.NewPassword);
        return Ok(new { message });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        LogManager.Log_Logs_Add((int)DB_Table.Account_Users, userId, userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.LogOut, " ");
        return NoContent();
    }
}
