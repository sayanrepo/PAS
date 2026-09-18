#nullable enable

using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.CRM;
using BaseSite.Models.DBModel;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.CRM))]
[Route("api/crm/people")]
public sealed class CrmPeopleController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CrmPersonPageDto>> Get([FromQuery] CrmPersonSearch filter, CancellationToken cancellationToken)
    {
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Term))
        {
            var search = filter.Term.Replace(" ", "").Trim();
            query = query.Where(x => ((x.Name ?? "") + (x.LastName ?? "") + (x.Responsible1 ?? "")
                + (x.Responsible2 ?? "") + (x.Responsible3 ?? "")).Replace(" ", "").Contains(search));
        }
        if (filter.PartnerTypeId.HasValue) query = query.Where(x => x.PartnerTypeId == filter.PartnerTypeId.Value);
        if (filter.CountryId.HasValue) query = query.Where(x => x.Location_Cities.Location_Provinces.CountryId == filter.CountryId.Value);
        if (filter.ProvinceId.HasValue) query = query.Where(x => x.Location_Cities.ProvinceId == filter.ProvinceId.Value);
        if (filter.CityId.HasValue) query = query.Where(x => x.CityId1 == filter.CityId.Value);

        var tableId = (short)DB_Table.Account_Users;
        var count = await query.CountAsync(cancellationToken);
        var offset = filter.Page * filter.PageSize;
        var items = await query.OrderByDescending(x => x.Id).Skip(offset).Take(filter.PageSize).Select(x => new CrmPersonSummaryDto
        {
            Id = x.Id,
            FullName = ((x.Name ?? "") + " " + (x.LastName ?? "")).Trim(),
            PersonType = x.Account_PersonTypes.Name,
            Responsible = x.Responsible1 ?? "",
            PartnerType = x.Account_PartnerTypes.Name,
            Phone = x.Phone1 ?? "",
            City = x.Location_Cities != null ? x.Location_Cities.Name : "",
            Registrar = x.Account_Users2 != null ? ((x.Account_Users2.Name ?? "") + " " + (x.Account_Users2.LastName ?? "")).Trim() : "",
            RegistrationDate = x.RegistrationDate,
            NotesCount = db.CRM_Comments.Count(c => c.TrunkTableId == tableId && c.TrunkId == x.Id),
            OrdersCount = db.Order_Order.Count(o => o.CustomerId == x.Id) + db.Sale_Sale.Count(s => s.CustomerId == x.Id)
        }).ToListAsync(cancellationToken);
        for (var i = 0; i < items.Count; i++) items[i].RowNumber = offset + i + 1;

        return Ok(new CrmPersonPageDto { Items = items, TotalCount = count });
    }

    [HttpGet("lookups")]
    public ActionResult<CrmPersonLookupsDto> GetLookups()
    {
        using var db = new PantaEntities();
        return Ok(new CrmPersonLookupsDto
        {
            PartnerTypes = db.Account_PartnerTypes.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            Countries = db.Location_Countries.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            Provinces = db.Location_Provinces.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, ParentId = x.CountryId, Name = x.Name }).ToList(),
            Cities = db.Location_Cities.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, ParentId = x.ProvinceId, Name = x.Name }).ToList()
        });
    }

    [HttpGet("{id:int}")]
    public ActionResult<CrmPersonProfileDto> GetById(int id)
    {
        using var db = new PantaEntities();
        var person = db.Account_Users.AsNoTracking().Where(x => x.Id == id).Select(x => new CrmPersonProfileDto
        {
            Id = x.Id,
            FullName = ((x.Name ?? "") + " " + (x.LastName ?? "")).Trim(),
            PersonType = x.Account_PersonTypes.Name,
            PartnerType = x.Account_PartnerTypes.Name,
            Status = x.Account_UserStatus != null ? x.Account_UserStatus.Name : "",
            Department = x.DepartmentId == 1 ? "فروش" : x.DepartmentId == 2 ? "تولید" : "نامعلوم",
            NationalNumber = x.NationalNumber ?? "",
            EconomicalNumber = x.EconomicalNumber ?? "",
            FindoutWay = x.FindoutWay ?? "",
            Mobile = x.Mobile1 ?? "",
            Mobile2 = x.Mobile2 ?? "",
            Phone = x.Phone1 ?? "",
            Phone2 = x.Phone2 ?? "",
            Fax = x.Fax ?? "",
            Email = x.Email ?? "",
            Website = x.Website ?? "",
            Responsible1 = x.Responsible1 ?? "",
            ResponsiblePhone1 = x.ResponsiblePhone1 ?? "",
            Responsible2 = x.Responsible2 ?? "",
            ResponsiblePhone2 = x.ResponsiblePhone2 ?? "",
            Responsible3 = x.Responsible3 ?? "",
            ResponsiblePhone3 = x.ResponsiblePhone3 ?? "",
            City1 = x.Location_Cities != null ? x.Location_Cities.Name : "",
            Address1 = x.Address1 ?? "",
            PostalCode1 = x.PostalCode1 ?? "",
            City2 = x.Location_Cities1 != null ? x.Location_Cities1.Name : "",
            Address2 = x.Address2 ?? "",
            PostalCode2 = x.PostalCode2 ?? "",
            Registrar = x.Account_Users2 != null ? ((x.Account_Users2.Name ?? "") + " " + (x.Account_Users2.LastName ?? "")).Trim() : "",
            RegistrationDate = x.RegistrationDate,
            Comment = x.Comment ?? ""
        }).SingleOrDefault();
        if (person is null) return NotFound();

        var tableId = (short)DB_Table.Account_Users;
        person.Notes = db.CRM_Comments.AsNoTracking()
            .Where(x => x.TrunkTableId == tableId && x.TrunkId == id)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new PersonNoteDto
            {
                Id = x.Id, ParentId = x.ParentId,
                Owner = x.Account_Users != null ? (x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? "") : x.OwnerName,
                Comment = x.Comment, CreatedAt = x.CreateDate
            }).ToList();

        var orders = db.Order_Order.AsNoTracking().Where(x => x.CustomerId == id)
            .Select(x => new PersonOrderHistoryDto
            {
                Kind = "orders", Id = x.Id, DocumentNumber = x.DocNumber, FactorNumber = x.FactorNumber,
                OrderDate = x.DateOrder, DeliveryDate = x.DateFactor, ProjectName = x.ProjectName,
                Receiver = (x.Account_Users1.Name ?? "") + " " + (x.Account_Users1.LastName ?? ""),
                Status = x.Order_Status.Name, OrderType = x.Tb_OrderTypes.Name, Amount = x.Cost
            }).ToList();
        var sales = db.Sale_Sale.AsNoTracking().Where(x => x.CustomerId == id)
            .Select(x => new PersonOrderHistoryDto
            {
                Kind = "sales", Id = x.Id, DocumentNumber = x.DocNumber, FactorNumber = x.FactorNumber,
                OrderDate = x.DateOrder, DeliveryDate = x.DateFactor, ProjectName = "",
                Receiver = (x.Account_Users1.Name ?? "") + " " + (x.Account_Users1.LastName ?? ""),
                Status = x.Order_Status.Name, OrderType = x.Tb_OrderTypes.Name, Amount = x.Cost, Returned = x.GiveBack
            }).ToList();
        person.Orders = orders.Concat(sales).OrderByDescending(x => x.OrderDate).ThenByDescending(x => x.Id).ToList();
        return Ok(person);
    }

    [HttpPost("{id:int}/notes")]
    public ActionResult<PersonNoteDto> AddNote(int id, PersonNoteRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using (var db = new PantaEntities())
            if (!db.Account_Users.AsNoTracking().Any(x => x.Id == id)) return NotFound();

        var currentUser = AccountManager.Account_User_Get(userId);
        var saved = CommentManager.CRM_Comments_Edit(new CRM_Comments
        {
            TrunkTableId = (short)DB_Table.Account_Users,
            TrunkId = id,
            OwnerId = userId,
            OwnerName = currentUser.FullName,
            OwnerEmail = currentUser.Email,
            Comment = request.Comment.Trim(),
            CreateDate = DateTime.Now
        });
        return Ok(new PersonNoteDto
        {
            Id = saved.Id, ParentId = saved.ParentId, Owner = saved.Account_Users?.FullName ?? saved.OwnerName,
            Comment = saved.Comment, CreatedAt = saved.CreateDate
        });
    }
}
