#nullable enable

namespace BaseSite.Api.Contracts;

public sealed class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class ChangePasswordRequest
{
    public string UserName { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public CurrentUserDto User { get; set; } = new();
}

public sealed class CurrentUserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = "profile.png";
    public string[] Roles { get; set; } = [];
}

public sealed class DashboardSummaryDto
{
    public int Orders { get; set; }
    public int Sales { get; set; }
    public int Deliveries { get; set; }
    public int Payments { get; set; }
    public int Services { get; set; }
    public int Customers { get; set; }
    public int Activities { get; set; }
}

public sealed class DocumentSummaryDto
{
    public string Kind { get; set; } = string.Empty;
    public int Id { get; set; }
    public int DocumentNumber { get; set; }
    public int? FactorNumber { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DocumentDate { get; set; }
    public DateTime? DueDate { get; set; }
    public double? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class CustomerSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
