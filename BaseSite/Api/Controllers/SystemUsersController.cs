#nullable enable

using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.Setting_Persons_AssignAccess) + "," + nameof(OPERATIONS.Setting_Persons_AssignUserName))]
[Route("api/system-users")]
public sealed class SystemUsersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SystemUserPageDto>> Get([FromQuery] SystemUserSearch filter, CancellationToken cancellationToken)
    {
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking()
            .Where(x => !string.IsNullOrEmpty(x.UserName) && !string.IsNullOrEmpty(x.Password) && x.Account_UserPost.Any());
        if (!string.IsNullOrWhiteSpace(filter.Term))
        {
            var search = filter.Term.Replace(" ", "").Trim();
            query = query.Where(x => ((x.Name ?? "") + (x.LastName ?? "")).Replace(" ", "").Contains(search));
        }
        if (filter.RoleId.HasValue) query = query.Where(x => x.Account_UserPost.Any(up => up.PostId == filter.RoleId.Value));
        if (filter.StatusId.HasValue) query = query.Where(x => x.Status == filter.StatusId.Value);
        if (filter.DepartmentId.HasValue) query = query.Where(x => x.DepartmentId == filter.DepartmentId.Value);

        var count = await query.CountAsync(cancellationToken);
        var offset = filter.Page * filter.PageSize;
        var items = await query.OrderByDescending(x => x.Id).Skip(offset).Take(filter.PageSize).Select(x => new SystemUserSummaryDto
        {
            PersonId = x.Id,
            FullName = ((x.Name ?? "") + " " + (x.LastName ?? "")).Trim(),
            PersonType = x.Account_PersonTypes.Name,
            PartnerType = x.Account_PartnerTypes.Name,
            UserName = x.UserName ?? "",
            HasPassword = !string.IsNullOrEmpty(x.Password),
            StatusId = x.Status,
            Status = x.Account_UserStatus != null ? x.Account_UserStatus.Name : "",
            RoleId = x.Account_UserPost.Select(up => (int?)up.PostId).FirstOrDefault(),
            Role = x.Account_UserPost.Select(up => up.Account_Posts.Name).FirstOrDefault() ?? ""
        }).ToListAsync(cancellationToken);
        for (var i = 0; i < items.Count; i++) items[i].RowNumber = offset + i + 1;

        return Ok(new SystemUserPageDto { Items = items, TotalCount = count });
    }

    [HttpGet("lookups")]
    public ActionResult<SystemUserLookupsDto> GetLookups()
    {
        using var db = new PantaEntities();
        return Ok(new SystemUserLookupsDto
        {
            Roles = db.Account_Posts.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            Statuses = db.Account_UserStatus.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            Departments = new List<NamedLookupDto>
            {
                new() { Id = 0, Name = "نامعلوم" },
                new() { Id = 1, Name = "فروش" },
                new() { Id = 2, Name = "تولید" }
            }
        });
    }

    [HttpGet("eligible-people")]
    public ActionResult<IReadOnlyList<NamedLookupDto>> GetEligiblePeople([FromQuery] string? term = null, [FromQuery] int take = 50)
    {
        take = Math.Clamp(take, 1, 100);
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking()
            .Where(x => x.PersonTypeId == 1 && (string.IsNullOrEmpty(x.UserName) || string.IsNullOrEmpty(x.Password) || !x.Account_UserPost.Any()));
        if (!string.IsNullOrWhiteSpace(term))
        {
            var search = term.Trim();
            query = query.Where(x => (x.Name + " " + x.LastName).Contains(search) || x.Mobile1.Contains(search));
        }
        return Ok(query.OrderBy(x => x.Name).ThenBy(x => x.LastName).Take(take)
            .Select(x => new NamedLookupDto { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToList());
    }

    [HttpGet("{personId:int}")]
    public ActionResult<SystemUserEditorDto> GetById(int personId)
    {
        using var db = new PantaEntities();
        var person = db.Account_Users.AsNoTracking().SingleOrDefault(x => x.Id == personId);
        if (person is null) return NotFound();
        return Ok(Editor(db, person));
    }

    [HttpPut("{personId:int}")]
    public ActionResult<SystemUserEditorDto> Save(int personId, SystemUserSaveRequest request)
    {
        if (!User.IsInRole(nameof(OPERATIONS.Setting_Persons_AssignAccess))
            || !User.IsInRole(nameof(OPERATIONS.Setting_Persons_AssignUserName))) return Forbid();
        if (personId != request.PersonId) return BadRequest(new ProblemDetails { Title = "شناسه شخص معتبر نیست." });

        using var db = new PantaEntities();
        var person = db.Account_Users.Include(x => x.Account_UserPost).SingleOrDefault(x => x.Id == personId);
        if (person is null) return NotFound();
        if (person.PersonTypeId != 1) return BadRequest(new ProblemDetails { Title = "حساب کاربری فقط برای شخص حقیقی قابل ایجاد است." });
        if (!db.Account_Posts.Any(x => x.Id == request.RoleId)) return BadRequest(new ProblemDetails { Title = "نقش انتخاب‌شده معتبر نیست." });
        if (!db.Account_UserStatus.Any(x => x.Id == request.StatusId)) return BadRequest(new ProblemDetails { Title = "وضعیت حساب معتبر نیست." });

        var userName = request.UserName.Trim();
        if (db.Account_Users.Any(x => x.Id != personId && x.UserName != null && x.UserName.ToLower() == userName.ToLower()))
            return Conflict(new ProblemDetails { Title = "این نام کاربری قبلاً استفاده شده است." });
        var isNewAccount = string.IsNullOrWhiteSpace(person.UserName)
            || string.IsNullOrWhiteSpace(person.Password)
            || !person.Account_UserPost.Any();
        if (isNewAccount && string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ProblemDetails { Title = "برای حساب جدید رمز اولیه را وارد کنید." });

        var categoryId = db.Account_Categories.AsNoTracking().OrderBy(x => x.Id).Select(x => (int?)x.Id).FirstOrDefault();
        if (!categoryId.HasValue) return BadRequest(new ProblemDetails { Title = "دسته‌بندی دسترسی در سیستم تعریف نشده است." });

        using var transaction = db.Database.BeginTransaction();
        person.UserName = userName;
        if (!string.IsNullOrWhiteSpace(request.Password)) person.Password = AccountManager.GetMD5(request.Password);
        person.Status = request.StatusId;
        db.Account_UserPost.RemoveRange(person.Account_UserPost.ToList());
        db.Account_UserPost.Add(new Account_UserPost { UserId = personId, PostId = request.RoleId, CategoryId = categoryId.Value });
        db.SaveChanges();
        transaction.Commit();

        LogManager.Log_Logs_Add((int)DB_Table.Account_Users, personId, CurrentUserId(),
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit,
            isNewAccount ? $"ایجاد حساب کاربری برای {person.FullName}" : $"ویرایش حساب کاربری {person.FullName}");

        db.Entry(person).Reload();
        return Ok(Editor(db, person));
    }

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private static SystemUserEditorDto Editor(PantaEntities db, Account_Users person)
    {
        var assignment = db.Account_UserPost.AsNoTracking().Where(x => x.UserId == person.Id)
            .Select(x => new { x.PostId, x.Account_Posts.Name }).FirstOrDefault();
        return new SystemUserEditorDto
        {
            User = new SystemUserSummaryDto
            {
                PersonId = person.Id, FullName = person.FullName, UserName = person.UserName ?? "",
                HasPassword = !string.IsNullOrEmpty(person.Password),
                StatusId = person.Status, Status = db.Account_UserStatus.Where(x => x.Id == person.Status).Select(x => x.Name).FirstOrDefault() ?? "",
                RoleId = assignment?.PostId, Role = assignment?.Name ?? ""
            },
            Roles = db.Account_Posts.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            Statuses = db.Account_UserStatus.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList()
        };
    }
}
