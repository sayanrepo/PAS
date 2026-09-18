#nullable enable

using System.ComponentModel.DataAnnotations;

namespace BaseSite.Api.Contracts;

public sealed class PersonOrganizationDetailsDto
{
    public int Id { get; set; }
    public byte PersonTypeId { get; set; }
    public byte PartnerTypeId { get; set; }
    public byte StatusId { get; set; }
    public byte? DepartmentId { get; set; }
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
}

public sealed class PersonOrganizationSaveRequest
{
    [Required, MaxLength(255)] public string Name { get; set; } = string.Empty;
    [MaxLength(255)] public string LastName { get; set; } = string.Empty;
    public byte PersonTypeId { get; set; }
    public byte PartnerTypeId { get; set; }
    public byte? DepartmentId { get; set; }
    [MaxLength(15)] public string NationalNumber { get; set; } = string.Empty;
    [MaxLength(15)] public string EconomicalNumber { get; set; } = string.Empty;
    public string FindoutWay { get; set; } = string.Empty;
    [MaxLength(255)] public string Website { get; set; } = string.Empty;
    [MaxLength(255)] public string Email { get; set; } = string.Empty;
    [MaxLength(50)] public string Fax { get; set; } = string.Empty;
    [MaxLength(50)] public string Phone1 { get; set; } = string.Empty;
    [MaxLength(50)] public string Phone2 { get; set; } = string.Empty;
    [MaxLength(50)] public string Mobile1 { get; set; } = string.Empty;
    [MaxLength(50)] public string Mobile2 { get; set; } = string.Empty;
    public int? CityId1 { get; set; }
    public int? CityId2 { get; set; }
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    [MaxLength(15)] public string PostalCode1 { get; set; } = string.Empty;
    [MaxLength(15)] public string PostalCode2 { get; set; } = string.Empty;
    [MaxLength(50)] public string Responsible1 { get; set; } = string.Empty;
    [MaxLength(50)] public string ResponsiblePhone1 { get; set; } = string.Empty;
    [MaxLength(50)] public string Responsible2 { get; set; } = string.Empty;
    [MaxLength(50)] public string ResponsiblePhone2 { get; set; } = string.Empty;
    [MaxLength(50)] public string Responsible3 { get; set; } = string.Empty;
    [MaxLength(50)] public string ResponsiblePhone3 { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public sealed class NamedLookupDto
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class PersonOrganizationLookupsDto
{
    public List<NamedLookupDto> PersonTypes { get; set; } = [];
    public List<NamedLookupDto> PartnerTypes { get; set; } = [];
    public List<NamedLookupDto> FindoutWays { get; set; } = [];
    public List<NamedLookupDto> Countries { get; set; } = [];
    public List<NamedLookupDto> Provinces { get; set; } = [];
    public List<NamedLookupDto> Cities { get; set; } = [];
}

public sealed class PersonOrganizationSearch
{
    public string? Term { get; set; }
    public byte? PartnerTypeId { get; set; }
    public int? CountryId { get; set; }
    public int? ProvinceId { get; set; }
    public int? CityId { get; set; }
    [Range(0, 1000000)] public int Page { get; set; }
    [Range(1, 100)] public int PageSize { get; set; } = 20;
    public bool HasFilters => !string.IsNullOrWhiteSpace(Term) || PartnerTypeId.HasValue
        || CountryId.HasValue || ProvinceId.HasValue || CityId.HasValue;
}

public sealed class PersonOrganizationSummaryDto
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

public sealed class PersonOrganizationPageDto
{
    public List<PersonOrganizationSummaryDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public sealed class CrmPersonSummaryDto
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
    [Range(0, 1000000)] public int Page { get; set; }
    [Range(1, 100)] public int PageSize { get; set; } = 20;
}

public sealed class CrmPersonPageDto
{
    public List<CrmPersonSummaryDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public sealed class CrmPersonLookupsDto
{
    public List<NamedLookupDto> PartnerTypes { get; set; } = [];
    public List<NamedLookupDto> Countries { get; set; } = [];
    public List<NamedLookupDto> Provinces { get; set; } = [];
    public List<NamedLookupDto> Cities { get; set; } = [];
}

public sealed class CrmPersonProfileDto
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
    public List<PersonNoteDto> Notes { get; set; } = [];
    public List<PersonOrderHistoryDto> Orders { get; set; } = [];
}

public sealed class PersonNoteDto
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class PersonNoteRequest
{
    [Required, MaxLength(4000)] public string Comment { get; set; } = string.Empty;
}

public sealed class PersonOrderHistoryDto
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

public sealed class SystemUserSummaryDto
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
    [Range(0, 1000000)] public int Page { get; set; }
    [Range(1, 100)] public int PageSize { get; set; } = 20;
}

public sealed class SystemUserPageDto
{
    public List<SystemUserSummaryDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public sealed class SystemUserLookupsDto
{
    public List<NamedLookupDto> Roles { get; set; } = [];
    public List<NamedLookupDto> Statuses { get; set; } = [];
    public List<NamedLookupDto> Departments { get; set; } = [];
}

public sealed class SystemUserEditorDto
{
    public SystemUserSummaryDto User { get; set; } = new();
    public List<NamedLookupDto> Roles { get; set; } = [];
    public List<NamedLookupDto> Statuses { get; set; } = [];
}

public sealed class SystemUserSaveRequest
{
    public int PersonId { get; set; }
    [Required, MaxLength(255)] public string UserName { get; set; } = string.Empty;
    [MinLength(4)] public string? Password { get; set; }
    public byte StatusId { get; set; }
    public int RoleId { get; set; }
}
