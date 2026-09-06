#nullable enable

using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/documents")]
public sealed class DocumentsController : ControllerBase
{
    [HttpGet("{kind}")]
    public ActionResult<IReadOnlyList<DocumentSummaryDto>> Get(
        string kind,
        [FromQuery] int? documentNumber = null,
        [FromQuery] string? customer = null,
        [FromQuery] int take = 100)
    {
        take = Math.Clamp(take, 1, 200);
        if (!CanRead(kind))
            return Forbid();

        using var db = new PantaEntities();

        return kind.ToLowerInvariant() switch
        {
            "orders" => Ok(GetOrders(db, documentNumber, customer, take)),
            "sales" => Ok(GetSales(db, 1, documentNumber, customer, take, "sales")),
            "store" => Ok(GetSales(db, 2, documentNumber, customer, take, "store")),
            "payments" => Ok(GetPayments(db, documentNumber, customer, take)),
            "deliveries" => Ok(GetDeliveries(db, documentNumber, customer, take)),
            "services" => Ok(GetServices(db, documentNumber, customer, take)),
            "activities" => Ok(GetActivities(db, customer, take)),
            _ => NotFound(new ProblemDetails { Title = "نوع سند شناخته نشد." })
        };
    }

    private bool CanRead(string kind) => kind.ToLowerInvariant() switch
    {
        "orders" => User.IsInRole(nameof(OPERATIONS.Order)),
        "sales" => User.IsInRole(nameof(OPERATIONS.Sale)),
        "store" => User.IsInRole(nameof(OPERATIONS.Store)),
        "payments" => User.IsInRole(nameof(OPERATIONS.Payment)),
        "deliveries" => User.IsInRole(nameof(OPERATIONS.Delivery)),
        "services" => User.IsInRole(nameof(OPERATIONS.Service)),
        "activities" => User.IsInRole(nameof(OPERATIONS.CRM)),
        _ => true
    };

    private static List<DocumentSummaryDto> GetOrders(PantaEntities db, int? documentNumber, string? customer, int take)
    {
        var query = db.Order_Order.AsNoTracking().AsQueryable();
        if (documentNumber.HasValue) query = query.Where(x => x.DocNumber == documentNumber.Value);
        if (!string.IsNullOrWhiteSpace(customer)) query = query.Where(x => (x.Account_Users.Name + " " + x.Account_Users.LastName).Contains(customer));
        return query.OrderByDescending(x => x.Id).Take(take).Select(x => new DocumentSummaryDto
        {
            Kind = "orders", Id = x.Id, DocumentNumber = x.DocNumber, FactorNumber = x.FactorNumber,
            Customer = x.Account_Users.Name + " " + x.Account_Users.LastName, Status = x.Order_Status.Name,
            DocumentDate = x.DateOrder, DueDate = x.DateDelivery, Amount = x.Cost, Description = x.ProjectName
        }).ToList();
    }

    private static List<DocumentSummaryDto> GetSales(PantaEntities db, byte storeId, int? documentNumber, string? customer, int take, string kind)
    {
        var query = db.Sale_Sale.AsNoTracking().Where(x => x.StoreId == storeId);
        if (documentNumber.HasValue) query = query.Where(x => x.DocNumber == documentNumber.Value);
        if (!string.IsNullOrWhiteSpace(customer)) query = query.Where(x => (x.Account_Users.Name + " " + x.Account_Users.LastName).Contains(customer));
        return query.OrderByDescending(x => x.Id).Take(take).Select(x => new DocumentSummaryDto
        {
            Kind = kind, Id = x.Id, DocumentNumber = x.DocNumber, FactorNumber = x.FactorNumber,
            Customer = x.Account_Users.Name + " " + x.Account_Users.LastName, Status = x.Order_Status.Name,
            DocumentDate = x.DateOrder, DueDate = x.DateDelivery, Amount = x.Cost, Description = x.Comment
        }).ToList();
    }

    private static List<DocumentSummaryDto> GetPayments(PantaEntities db, int? documentNumber, string? customer, int take)
    {
        var query = db.Payment_Payment.AsNoTracking().AsQueryable();
        if (documentNumber.HasValue) query = query.Where(x => x.DocNumber == documentNumber.Value);
        if (!string.IsNullOrWhiteSpace(customer)) query = query.Where(x => (x.Account_Users.Name + " " + x.Account_Users.LastName).Contains(customer));
        return query.OrderByDescending(x => x.Id).Take(take).Select(x => new DocumentSummaryDto
        {
            Kind = "payments", Id = x.Id, DocumentNumber = x.DocNumber,
            Customer = x.Account_Users.Name + " " + x.Account_Users.LastName, Status = x.Payment_Status.Name,
            DocumentDate = x.DateSanad, DueDate = x.DateSarresid, Amount = x.Amount, Description = x.Comment
        }).ToList();
    }

    private static List<DocumentSummaryDto> GetDeliveries(PantaEntities db, int? documentNumber, string? customer, int take)
    {
        var query = db.Delivery_Delivery.AsNoTracking().AsQueryable();
        if (documentNumber.HasValue) query = query.Where(x => x.DocNumber == documentNumber.Value);
        if (!string.IsNullOrWhiteSpace(customer))
            query = query.Where(x => ((x.Order_Order.Account_Users.Name + " " + x.Order_Order.Account_Users.LastName) ??
                                      (x.Sale_Sale.Account_Users.Name + " " + x.Sale_Sale.Account_Users.LastName)).Contains(customer));
        return query.OrderByDescending(x => x.Id).Take(take).Select(x => new DocumentSummaryDto
        {
            Kind = "deliveries", Id = x.Id, DocumentNumber = x.DocNumber,
            Customer = x.Order_Order != null
                ? x.Order_Order.Account_Users.Name + " " + x.Order_Order.Account_Users.LastName
                : x.Sale_Sale.Account_Users.Name + " " + x.Sale_Sale.Account_Users.LastName,
            Status = x.Delivery_Status.Name, DocumentDate = x.Date, DueDate = null, Amount = null,
            Description = x.DestinationAddress
        }).ToList();
    }

    private static List<DocumentSummaryDto> GetServices(PantaEntities db, int? documentNumber, string? customer, int take)
    {
        var query = db.Service_Service.AsNoTracking().AsQueryable();
        if (documentNumber.HasValue) query = query.Where(x => x.DocNumber == documentNumber.Value);
        if (!string.IsNullOrWhiteSpace(customer)) query = query.Where(x => (x.Account_Users.Name + " " + x.Account_Users.LastName).Contains(customer));
        return query.OrderByDescending(x => x.Id).Take(take).Select(x => new DocumentSummaryDto
        {
            Kind = "services", Id = x.Id, DocumentNumber = x.DocNumber, FactorNumber = x.FactorNumber,
            Customer = x.Account_Users.Name + " " + x.Account_Users.LastName, Status = x.Order_Status.Name,
            DocumentDate = x.DateOrder, DueDate = x.DateDelivery, Amount = x.Cost, Description = x.Comment
        }).ToList();
    }

    private static List<DocumentSummaryDto> GetActivities(PantaEntities db, string? customer, int take)
    {
        var query = db.CRM_Activity.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(customer)) query = query.Where(x => (x.Account_Users2.Name + " " + x.Account_Users2.LastName).Contains(customer));
        return query.OrderByDescending(x => x.StartTime).Take(take).Select(x => new DocumentSummaryDto
        {
            Kind = "activities", Id = x.Id, DocumentNumber = x.Id,
            Customer = x.Account_Users2.Name + " " + x.Account_Users2.LastName, Status = x.CRM_ActivityState.Name,
            DocumentDate = x.StartTime, DueDate = x.EndTime, Description = x.Subject
        }).ToList();
    }
}
