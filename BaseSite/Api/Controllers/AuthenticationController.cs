using BaseSite.Api.Authentication;
using BaseSite.Api.Contracts;
using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    public ActionResult<CurrentUserDto> Me() => Ok(GetCurrentUser());

    [Authorize]
    [HttpPost("change-password")]
    public IActionResult ChangePassword(ChangePasswordRequest request)
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        if (!AccountManager.Account_User_TryChangePassword(userId, request.UserName, request.CurrentPassword, request.NewPassword, out var message))
            return BadRequest(new ProblemDetails { Title = message });

        LogManager.Log_Logs_Add((int)DB_Table.Account_Users, userId, userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, "تغییر نام کاربری و رمز عبور حساب کاربری خود");
        return Ok(accessTokens.Create(GetCurrentUser()));
    }

    [Authorize]
    [HttpPost("change-image")]
    public IActionResult ChangeImage(ChangeImageRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // The web server stores uploaded images using an owner-specific, generated filename.
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.ImagePath ?? string.Empty,
                $@"\A{userId}_[a-f0-9]{{32}}\.(jpg|png|webp)\z"))
            return BadRequest(new ProblemDetails { Title = "نام فایل تصویر معتبر نیست." });

        AccountManager.Account_User_ChangeImage(userId, request.ImagePath);
        LogManager.Log_Logs_Add((int)DB_Table.Account_Users, userId, userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, "تغییر موفق تصویر پروفایل خود");
        return Ok(accessTokens.Create(GetCurrentUser()));
    }

    private CurrentUserDto GetCurrentUser()
    {
        var user = AccountManager.Account_User_Get(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
        return new CurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            ImagePath = user.ImagePath ?? "profile.png",
            Roles = user.Account_UserPost.Count > 0
                ? AccountManager.Account_Operation_Get((AccountRole)user.Account_UserPost.First().PostId)
                    .Distinct().Select(operation => operation.ToString()).ToArray()
                : []
        };
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
