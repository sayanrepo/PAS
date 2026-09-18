using System.ComponentModel.DataAnnotations;

namespace BaseSite.Web.Models;

public sealed class PersonOrganizationDetails
{
    public int Id { get; set; }
    public byte PersonTypeId { get; set; } = 1;
    public byte PartnerTypeId { get; set; } = 2;
    public byte StatusId { get; set; } = 2;
    public byte? DepartmentId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalNumber { get; set; } = string.Empty;
    public string EconomicalNumber { get; set; } = string.Empty;
    public string FindoutWay { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Phone1 { get; set; } = string.Empty;
    public string Phone2 { get; set; } = string.Empty;
    public string Mobile1 { get; set; } = string.Empty;
    public string Mobile2 { get; set; } = string.Empty;
    public int? CityId1 { get; set; }
    public int? CityId2 { get; set; }
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string PostalCode1 { get; set; } = string.Empty;
    public string PostalCode2 { get; set; } = string.Empty;
    public string Responsible1 { get; set; } = string.Empty;
    public string ResponsiblePhone1 { get; set; } = string.Empty;
    public string Responsible2 { get; set; } = string.Empty;
    public string ResponsiblePhone2 { get; set; } = string.Empty;
    public string Responsible3 { get; set; } = string.Empty;
    public string ResponsiblePhone3 { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;

    public PersonOrganizationSave ToSaveRequest() => new()
    {
        Name = Name, LastName = LastName, PersonTypeId = PersonTypeId, PartnerTypeId = PartnerTypeId,
        DepartmentId = DepartmentId, NationalNumber = NationalNumber, EconomicalNumber = EconomicalNumber,
        FindoutWay = FindoutWay, Website = Website, Email = Email, Fax = Fax,
        Phone1 = Phone1, Phone2 = Phone2, Mobile1 = Mobile1, Mobile2 = Mobile2,
        CityId1 = CityId1, CityId2 = CityId2, Address1 = Address1, Address2 = Address2,
        PostalCode1 = PostalCode1, PostalCode2 = PostalCode2, Responsible1 = Responsible1,
        ResponsiblePhone1 = ResponsiblePhone1, Responsible2 = Responsible2,
        ResponsiblePhone2 = ResponsiblePhone2, Responsible3 = Responsible3,
        ResponsiblePhone3 = ResponsiblePhone3, Comment = Comment
    };
}

public sealed class PersonOrganizationSave
{
    [Required(ErrorMessage = "نام را وارد کنید."), MaxLength(255)] public string Name { get; set; } = string.Empty;
    [MaxLength(255)] public string LastName { get; set; } = string.Empty;
    public byte PersonTypeId { get; set; }
    public byte PartnerTypeId { get; set; }
    public byte? DepartmentId { get; set; }
    public string NationalNumber { get; set; } = string.Empty;
    public string EconomicalNumber { get; set; } = string.Empty;
    public string FindoutWay { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Phone1 { get; set; } = string.Empty;
    public string Phone2 { get; set; } = string.Empty;
    public string Mobile1 { get; set; } = string.Empty;
    public string Mobile2 { get; set; } = string.Empty;
    public int? CityId1 { get; set; }
    public int? CityId2 { get; set; }
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string PostalCode1 { get; set; } = string.Empty;
    public string PostalCode2 { get; set; } = string.Empty;
    public string Responsible1 { get; set; } = string.Empty;
    public string ResponsiblePhone1 { get; set; } = string.Empty;
    public string Responsible2 { get; set; } = string.Empty;
    public string ResponsiblePhone2 { get; set; } = string.Empty;
    public string Responsible3 { get; set; } = string.Empty;
    public string ResponsiblePhone3 { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public sealed class NamedLookup { public int Id { get; set; } public int? ParentId { get; set; } public string Name { get; set; } = string.Empty; }
public sealed class PersonOrganizationLookups
{
    public List<NamedLookup> PersonTypes { get; set; } = [];
    public List<NamedLookup> PartnerTypes { get; set; } = [];
    public List<NamedLookup> FindoutWays { get; set; } = [];
    public List<NamedLookup> Countries { get; set; } = [];
    public List<NamedLookup> Provinces { get; set; } = [];
    public List<NamedLookup> Cities { get; set; } = [];
}

public sealed class PersonOrganizationSearch
{
    public string? Term { get; set; }
    public byte? PartnerTypeId { get; set; }
    public int? CountryId { get; set; }
    public int? ProvinceId { get; set; }
    public int? CityId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; } = 20;
}

public sealed class PersonOrganizationSummary
{
    public int Id { get; set; }
    public int RowNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PersonType { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public string PartnerType { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Registrar { get; set; } = string.Empty;
    public DateTime? RegistrationDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class PersonOrganizationPage
{
    public List<PersonOrganizationSummary> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public sealed class CrmPersonSummary
{
    public int Id { get; set; }
    public int RowNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PersonType { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public string PartnerType { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Registrar { get; set; } = string.Empty;
    public DateTime? RegistrationDate { get; set; }
    public int NotesCount { get; set; }
    public int OrdersCount { get; set; }
}

public sealed class CrmPersonSearch
{
    public string? Term { get; set; }
    public byte? PartnerTypeId { get; set; }
    public int? CountryId { get; set; }
    public int? ProvinceId { get; set; }
    public int? CityId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; } = 20;
}

public sealed class CrmPersonPage
{
    public List<CrmPersonSummary> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public sealed class CrmPersonLookups
{
    public List<NamedLookup> PartnerTypes { get; set; } = [];
    public List<NamedLookup> Countries { get; set; } = [];
    public List<NamedLookup> Provinces { get; set; } = [];
    public List<NamedLookup> Cities { get; set; } = [];
}

public sealed class CrmPersonProfile
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PersonType { get; set; } = string.Empty;
    public string PartnerType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string NationalNumber { get; set; } = string.Empty;
    public string EconomicalNumber { get; set; } = string.Empty;
    public string FindoutWay { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Mobile2 { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Phone2 { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Responsible1 { get; set; } = string.Empty;
    public string ResponsiblePhone1 { get; set; } = string.Empty;
    public string Responsible2 { get; set; } = string.Empty;
    public string ResponsiblePhone2 { get; set; } = string.Empty;
    public string Responsible3 { get; set; } = string.Empty;
    public string ResponsiblePhone3 { get; set; } = string.Empty;
    public string City1 { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string PostalCode1 { get; set; } = string.Empty;
    public string City2 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string PostalCode2 { get; set; } = string.Empty;
    public string Registrar { get; set; } = string.Empty;
    public DateTime? RegistrationDate { get; set; }
    public string Comment { get; set; } = string.Empty;
    public List<PersonNote> Notes { get; set; } = [];
    public List<PersonOrderHistory> Orders { get; set; } = [];
}

public sealed class PersonNote
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class PersonOrderHistory
{
    public string Kind { get; set; } = string.Empty;
    public int Id { get; set; }
    public int DocumentNumber { get; set; }
    public int FactorNumber { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public double Amount { get; set; }
    public bool Returned { get; set; }
}

public sealed class SystemUserSummary
{
    public int PersonId { get; set; }
    public int RowNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PersonType { get; set; } = string.Empty;
    public string PartnerType { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool HasPassword { get; set; }
    public byte StatusId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public sealed class SystemUserSearch
{
    public string? Term { get; set; }
    public int? RoleId { get; set; }
    public byte? StatusId { get; set; }
    public byte? DepartmentId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; } = 20;
}

public sealed class SystemUserPage
{
    public List<SystemUserSummary> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public sealed class SystemUserLookups
{
    public List<NamedLookup> Roles { get; set; } = [];
    public List<NamedLookup> Statuses { get; set; } = [];
    public List<NamedLookup> Departments { get; set; } = [];
}

public sealed class SystemUserEditor
{
    public SystemUserSummary User { get; set; } = new();
    public List<NamedLookup> Roles { get; set; } = [];
    public List<NamedLookup> Statuses { get; set; } = [];
}

public sealed class SystemUserSave
{
    public int PersonId { get; set; }
    [Required(ErrorMessage = "نام کاربری را وارد کنید."), MaxLength(255)] public string UserName { get; set; } = string.Empty;
    [MinLength(4, ErrorMessage = "رمز عبور باید حداقل چهار کاراکتر باشد.")] public string? Password { get; set; }
    public byte StatusId { get; set; }
    public int RoleId { get; set; }
}
