using System.ComponentModel.DataAnnotations;

namespace BaseSite.Web.Models;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "نام کاربری را وارد کنید.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور را وارد کنید.")]
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public CurrentUser User { get; set; } = new();
}

public sealed class CurrentUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = "profile.png";
    public string[] Roles { get; set; } = [];
}

public sealed class DashboardSummary
{
    public int Orders { get; set; }
    public int Sales { get; set; }
    public int Deliveries { get; set; }
    public int Payments { get; set; }
    public int Services { get; set; }
    public int Customers { get; set; }
    public int Activities { get; set; }
}

public sealed class DocumentSummary
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

public sealed class CustomerSummary
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
