#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.CRM;
using BaseSite.Models.DBModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;

[ApiController, Authorize(Roles = nameof(OPERATIONS.Payment_Detail))]
[Route("api/payments/{id:int}/activity")]
public sealed class PaymentActivityController : ControllerBase
{
    [HttpGet]
    public ActionResult<OrderActivity> Get(int id)
    {
        using var db = new PantaEntities();
        var documentNumber = db.Payment_Payment.AsNoTracking().Where(x => x.Id == id)
            .Select(x => (int?)x.DocNumber).SingleOrDefault();
        if (!documentNumber.HasValue || id <= 0) return NotFound();
        var comments = db.CRM_Comments.AsNoTracking()
            .Where(x => x.TrunkTableId == (short)DB_Table.Payment_Payment && x.TrunkId == id)
            .OrderBy(x => x.CreateDate).Select(x => new OrderCommentItem
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Owner = x.Account_Users != null ? (x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? "") : x.OwnerName,
                Comment = x.Comment,
                CreatedAt = x.CreateDate
            }).ToList();
        var history = db.Log_Logs.AsNoTracking()
            .Where(x => x.EntityTableId == (int)DB_Table.Payment_Payment && x.EntityId == documentNumber.Value)
            .OrderByDescending(x => x.EventTime).Select(x => new OrderHistoryItem
            {
                Id = x.Id,
                EventTime = x.EventTime,
                User = (x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? ""),
                Category = x.BaseSystem_Tables.Label,
                DocumentNumber = x.EntityId,
                Activity = x.Log_LogActivity.Name,
                Description = x.Description,
                IpAddress = x.IPAddress,
                Amount = x.LogData1
            }).ToList();
        return Ok(new OrderActivity { Comments = comments, History = history });
    }

    [HttpPost("comments")]
    public ActionResult<OrderCommentItem> AddComment(int id, OrderCommentRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using (var db = new PantaEntities())
            if (!db.Payment_Payment.AsNoTracking().Any(x => x.Id == id && id > 0)) return NotFound();
        var currentUser = AccountManager.Account_User_Get(userId);
        var saved = CommentManager.CRM_Comments_Edit(new CRM_Comments
        {
            TrunkTableId = (short)DB_Table.Payment_Payment,
            TrunkId = id,
            OwnerId = userId,
            OwnerName = currentUser.FullName,
            OwnerEmail = currentUser.Email,
            Comment = request.Comment.Trim(),
            CreateDate = DateTime.Now
        });
        return Ok(new OrderCommentItem
        {
            Id = saved.Id,
            ParentId = saved.ParentId,
            Owner = saved.Account_Users?.FullName ?? saved.OwnerName,
            Comment = saved.Comment,
            CreatedAt = saved.CreateDate
        });
    }
}
