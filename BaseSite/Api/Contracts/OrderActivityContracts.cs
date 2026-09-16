#nullable enable
namespace BaseSite.Api.Contracts;

public sealed class OrderActivity
{
    public List<OrderCommentItem> Comments { get; set; } = [];
    public List<OrderHistoryItem> History { get; set; } = [];
}
public sealed class OrderCommentItem
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Owner { get; set; } = "";
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
public sealed class OrderCommentRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MaxLength(4000)]
    public string Comment { get; set; } = "";
}
public sealed class OrderHistoryItem
{
    public long Id { get; set; }
    public DateTime EventTime { get; set; }
    public string User { get; set; } = "";
    public string Category { get; set; } = "";
    public int DocumentNumber { get; set; }
    public string Activity { get; set; } = "";
    public string Description { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public double? Amount { get; set; }
}

