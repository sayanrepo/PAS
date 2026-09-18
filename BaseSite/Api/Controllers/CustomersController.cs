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
[Authorize(Roles = nameof(OPERATIONS.Setting_Persons))]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PersonOrganizationPageDto>> Get([FromQuery] PersonOrganizationSearch filter, CancellationToken cancellationToken)
    {
        if (filter.HasFilters && !User.IsInRole(nameof(OPERATIONS.Setting_Persons_Search))) return Forbid();
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

        var count = await query.CountAsync(cancellationToken);
        var offset = filter.Page * filter.PageSize;
        var items = await query.OrderByDescending(x => x.Id).Skip(offset).Take(filter.PageSize)
            .Select(x => new PersonOrganizationSummaryDto
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
                Status = x.Account_UserStatus != null ? x.Account_UserStatus.Name : ""
            }).ToListAsync(cancellationToken);
        for (var i = 0; i < items.Count; i++) items[i].RowNumber = offset + i + 1;

        return Ok(new PersonOrganizationPageDto { Items = items, TotalCount = count });
    }

    [HttpGet("lookups")]
    public ActionResult<PersonOrganizationLookupsDto> GetLookups()
    {
        using var db = new PantaEntities();
        return Ok(new PersonOrganizationLookupsDto
        {
            PersonTypes = db.Account_PersonTypes.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            PartnerTypes = db.Account_PartnerTypes.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            FindoutWays = db.Account_FindoutWays.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            Countries = db.Location_Countries.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, Name = x.Name }).ToList(),
            Provinces = db.Location_Provinces.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name)
                .Select(x => new NamedLookupDto { Id = x.Id, ParentId = x.CountryId, Name = x.Name }).ToList(),
            Cities = db.Location_Cities.AsNoTracking().OrderBy(x => x.Name)
                .Where(x => !x.Deleted).Select(x => new NamedLookupDto { Id = x.Id, ParentId = x.ProvinceId, Name = x.Name }).ToList()
        });
    }

    [HttpGet("{id:int}")]
    public ActionResult<PersonOrganizationDetailsDto> GetById(int id)
    {
        using var db = new PantaEntities();
        var person = db.Account_Users.AsNoTracking().SingleOrDefault(x => x.Id == id);
        return person is null ? NotFound() : Ok(Map(person));
    }

    [HttpPost]
    public ActionResult<PersonOrganizationDetailsDto> Create(PersonOrganizationSaveRequest request)
    {
        if (!User.IsInRole(nameof(OPERATIONS.Setting_Persons_Add))) return Forbid();
        using var db = new PantaEntities();
        var validation = ValidateReferences(db, request);
        if (validation is not null) return BadRequest(new ProblemDetails { Title = validation });

        var person = new Account_Users
        {
            TableId = (int)DB_Table.Account_Users,
            Status = (byte)UserStatus.DeActive,
            RegistrarId = CurrentUserId(),
            RegistrationDate = DateTime.Now,
            ImagePath = "profile.png"
        };
        Apply(person, request);
        db.Account_Users.Add(person);
        db.SaveChanges();
        Cache.Update();
        LogManager.Log_Logs_Add((int)DB_Table.Account_Users, person.Id, CurrentUserId(),
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Add, $"ثبت شخص یا سازمان: {person.FullName}");
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, Map(person));
    }

    [HttpPut("{id:int}")]
    public ActionResult<PersonOrganizationDetailsDto> Update(int id, PersonOrganizationSaveRequest request)
    {
        if (!User.IsInRole(nameof(OPERATIONS.Setting_Persons_Edit))) return Forbid();
        using var db = new PantaEntities();
        var person = db.Account_Users.SingleOrDefault(x => x.Id == id);
        if (person is null) return NotFound();
        var validation = ValidateReferences(db, request);
        if (validation is not null) return BadRequest(new ProblemDetails { Title = validation });

        Apply(person, request);
        db.SaveChanges();
        Cache.Update();
        LogManager.Log_Logs_Add((int)DB_Table.Account_Users, person.Id, CurrentUserId(),
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, $"ویرایش شخص یا سازمان: {person.FullName}");
        return Ok(Map(person));
    }

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private static string? ValidateReferences(PantaEntities db, PersonOrganizationSaveRequest request)
    {
        if (request.PersonTypeId == 0 || !db.Account_PersonTypes.Any(x => x.Id == request.PersonTypeId)) return "نوع شخص معتبر نیست.";
        if (request.PartnerTypeId == 0 || !db.Account_PartnerTypes.Any(x => x.Id == request.PartnerTypeId)) return "نوع همکاری معتبر نیست.";
        if (request.CityId1.HasValue && !db.Location_Cities.Any(x => x.Id == request.CityId1.Value)) return "شهر نشانی اول معتبر نیست.";
        if (request.CityId2.HasValue && !db.Location_Cities.Any(x => x.Id == request.CityId2.Value)) return "شهر نشانی دوم معتبر نیست.";
        return null;
    }

    private static void Apply(Account_Users person, PersonOrganizationSaveRequest request)
    {
        person.Name = request.Name.Trim();
        person.LastName = request.LastName?.Trim() ?? "";
        person.PersonTypeId = request.PersonTypeId;
        person.PartnerTypeId = request.PartnerTypeId;
        person.DepartmentId = request.DepartmentId;
        person.NationalNumber = request.NationalNumber?.Trim() ?? "";
        person.EconomicalNumber = request.EconomicalNumber?.Trim() ?? "";
        person.FindoutWay = request.FindoutWay?.Trim() ?? "";
        person.Website = request.Website?.Trim() ?? "";
        person.Email = request.Email?.Trim() ?? "";
        person.Fax = request.Fax?.Trim() ?? "";
        person.Phone1 = request.Phone1?.Trim() ?? "";
        person.Phone2 = request.Phone2?.Trim() ?? "";
        person.Mobile1 = request.Mobile1?.Trim() ?? "";
        person.Mobile2 = request.Mobile2?.Trim() ?? "";
        person.CityId1 = request.CityId1;
        person.CityId2 = request.CityId2;
        person.Address1 = request.Address1?.Trim() ?? "";
        person.Address2 = request.Address2?.Trim() ?? "";
        person.PostalCode1 = request.PostalCode1?.Trim() ?? "";
        person.PostalCode2 = request.PostalCode2?.Trim() ?? "";
        person.Responsible1 = request.Responsible1?.Trim() ?? "";
        person.ResponsiblePhone1 = request.ResponsiblePhone1?.Trim() ?? "";
        person.Responsible2 = request.Responsible2?.Trim() ?? "";
        person.ResponsiblePhone2 = request.ResponsiblePhone2?.Trim() ?? "";
        person.Responsible3 = request.Responsible3?.Trim() ?? "";
        person.ResponsiblePhone3 = request.ResponsiblePhone3?.Trim() ?? "";
        person.Comment = request.Comment?.Trim() ?? "";
    }

    private static PersonOrganizationDetailsDto Map(Account_Users x) => new()
    {
        Id = x.Id, PersonTypeId = x.PersonTypeId, PartnerTypeId = x.PartnerTypeId, StatusId = x.Status,
        DepartmentId = x.DepartmentId, Name = x.Name ?? "", LastName = x.LastName ?? "",
        NationalNumber = x.NationalNumber ?? "", EconomicalNumber = x.EconomicalNumber ?? "",
        FindoutWay = x.FindoutWay ?? "", Website = x.Website ?? "", Email = x.Email ?? "", Fax = x.Fax ?? "",
        Phone1 = x.Phone1 ?? "", Phone2 = x.Phone2 ?? "", Mobile1 = x.Mobile1 ?? "", Mobile2 = x.Mobile2 ?? "",
        CityId1 = x.CityId1, CityId2 = x.CityId2, Address1 = x.Address1 ?? "", Address2 = x.Address2 ?? "",
        PostalCode1 = x.PostalCode1 ?? "", PostalCode2 = x.PostalCode2 ?? "",
        Responsible1 = x.Responsible1 ?? "", ResponsiblePhone1 = x.ResponsiblePhone1 ?? "",
        Responsible2 = x.Responsible2 ?? "", ResponsiblePhone2 = x.ResponsiblePhone2 ?? "",
        Responsible3 = x.Responsible3 ?? "", ResponsiblePhone3 = x.ResponsiblePhone3 ?? "", Comment = x.Comment ?? ""
    };
}
