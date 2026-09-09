#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
namespace BaseSite.Api.Controllers;
[ApiController]
[Authorize(Roles=nameof(OPERATIONS.Report))]
[Route("api/reports")]
public sealed class ReportsController(IConfiguration configuration) : ControllerBase {
 private static readonly Dictionary<string,string> Labels=LoadLabels();
 private static Dictionary<string,string> LoadLabels() {
  using var stream=typeof(ReportsController).Assembly.GetManifestResourceStream("BaseSite.Api.Queries.ReportColumns.json");
  return stream is null ? [] : JsonSerializer.Deserialize<Dictionary<string,string>>(stream)??[];
 }
 [HttpGet]
 public ActionResult<List<ReportInfo>> List() => Ok(ReportCatalog.All.Where(x=>ReportCatalog.CanRead(User,x)).Select(x=>x.Info).ToList());
 [HttpGet("customers")]
 [Authorize(Roles=nameof(OPERATIONS.Report_CustomerBill))]
 public async Task<ActionResult<List<OrderLookup>>> Customers(string? term,CancellationToken token) {
  using var db=new PantaEntities();
  var q=db.Account_Users.AsNoTracking().Where(x=>x.Id>0);
  if(!string.IsNullOrWhiteSpace(term)) {var text=term.Trim();q=q.Where(x=>((x.Name??"")+" "+(x.LastName??"")).Contains(text));}
  return Ok(await q.OrderBy(x=>x.Name).ThenBy(x=>x.Id).Take(30).Select(x=>new OrderLookup{Id=x.Id,Name=(x.Name??"")+" "+(x.LastName??"")}).ToListAsync(token));
 }
 [HttpGet("{key}")]
 public async Task<ActionResult<ReportResult>> Run(string key,[FromQuery] ReportFilter filter,CancellationToken token) {
  var definition=ReportCatalog.All.SingleOrDefault(x=>x.Info.Key==key);
  if(definition is null) return NotFound();
  if(!ReportCatalog.CanRead(User,definition)) return Forbid();
  if(definition.Info.Mode=="powerbi") {
   var root=configuration["Reports:GatewayUrl"];
   if(!Uri.TryCreate(root,UriKind.Absolute,out var uri) || uri.Scheme!="https")
    return StatusCode(503,new ProblemDetails{Title="نشانی امن سرور گزارش تنظیم نشده است."});
   var path=string.Join("/",definition.Path!.Split('/').Select(Uri.EscapeDataString));
   return Ok(new ReportResult{EmbedUrl=root!.TrimEnd('/')+"/reports/powerbi/"+path+"?rs:embed=true"});
  }
  using var db=new PantaEntities();
  var start=filter.DateFrom!.Value.Date;
  var end=filter.DateTo!.Value.Date.AddDays(1);
  if(definition.Info.Mode=="logs") {
   var rows=await db.Log_Logs.AsNoTracking().Where(x=>x.EventTime>=start && x.EventTime<end)
    .OrderByDescending(x=>x.EventTime).ThenByDescending(x=>x.Id)
    .Select(x=>new{x.EventTime,x.EntityId,User=(x.Account_Users.Name??"")+" "+(x.Account_Users.LastName??""),Table=x.BaseSystem_Tables.Label,Activity=x.Log_LogActivity.Name}).ToListAsync(token);
   return Ok(new ReportResult{Tables=[new(){Title=definition.Info.Title,Columns=["تاریخ و زمان","شماره سند","کاربر","بخش","فعالیت"],
    Rows=rows.Select(x=>new List<string>{Format(x.EventTime),Format(x.EntityId),x.User,x.Table,x.Activity}).ToList()}]});
  }
  if(definition.Info.Mode=="production") {
   if(!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var userId)) return Unauthorized();
   var q=db.Order_Process.AsNoTracking().Where(x=>x.PTime>=start && x.PTime<end);
   if(!User.IsInRole(nameof(OPERATIONS.Report_productFactor_AllOperators))) q=q.Where(x=>x.UserId==userId);
   else q=q.Where(x=>x.Account_Users.DepartmentId==(byte)Department.Tolid);
   var rows=await q.OrderBy(x=>x.UserId).ThenBy(x=>x.PTime).Select(x=>new {
    Name=(x.Account_Users.Name??"")+" "+(x.Account_Users.LastName??""),x.UserId,x.PTime,x.ProductDocNumber,
    x.Description,x.Count,x.Percent,x.CalculatedFactor,Status=x.Order_ProductStatus.Name}).ToListAsync(token);
   return Ok(new ReportResult{Tables=[
    new(){Title="مجموع کارکرد",Columns=["نام","مجموع کارکرد"],Rows=rows.GroupBy(x=>new{x.UserId,x.Name}).Select(g=>new List<string>{g.Key.Name,Format(g.Sum(x=>x.CalculatedFactor*x.Percent/100))}).ToList()},
    new(){Title="ریز کارکرد",Columns=["نام","تاریخ","شماره سند محصول","شرح","مرحله","تعداد","درصد","کارکرد"],Rows=rows.Select(x=>new List<string>{x.Name,Format(x.PTime),Format(x.ProductDocNumber),x.Description??"",x.Status,Format(x.Count),Format(x.Percent),Format(x.CalculatedFactor*x.Percent/100)}).ToList()}
   ]});
  }
  if(definition.Info.CustomerFilter && filter.CustomerId<=0) return BadRequest(new ProblemDetails{Title="مشتری را انتخاب کنید."});
  // Procedure names come exclusively from the server catalog; all user values are parameters.
  var connection=db.Database.Connection;
  await connection.OpenAsync(token);
  using var command=connection.CreateCommand();
  command.CommandText="EXEC ["+definition.Info.Key+"] @customerid, @shdatefrom, @shdateto";
  command.CommandTimeout=90;
  void Add(string name,object value,DbType type) {var p=command.CreateParameter();p.ParameterName=name;p.Value=value;p.DbType=type;command.Parameters.Add(p);}
  Add("@customerid",definition.Info.PartFilter ? filter.Part : definition.Info.CustomerFilter ? filter.CustomerId : 0,DbType.Int32);
  Add("@shdatefrom",ShDate(start),DbType.String);
  Add("@shdateto",ShDate(filter.DateTo.Value),DbType.String);
  using var reader=await command.ExecuteReaderAsync(token);
  var result=new ReportResult();
  do {
   if(reader.FieldCount==0) continue;
   var table=new ReportTable{Title=definition.Info.Key=="RCustomerBill" ? (result.Tables.Count==0 ? "صورت حساب مشتری" : "صورت حساب سفارشات تحویل نشده مشتری") : definition.Info.Title+" — بخش "+(result.Tables.Count+1)};
   var visible=Enumerable.Range(0,reader.FieldCount).Where(i=>!reader.GetName(i).EndsWith("Id",StringComparison.OrdinalIgnoreCase)
    && reader.GetName(i)!="Id" && !reader.GetName(i).EndsWith("Id2",StringComparison.OrdinalIgnoreCase)).ToList();
   foreach(var i in visible) {var field=reader.GetName(i);table.Columns.Add(Labels.GetValueOrDefault(field,field));}
   while(await reader.ReadAsync(token)) {
    var row=new List<string>();foreach(var i in visible) row.Add(Format(reader.GetValue(i)));table.Rows.Add(row);
    if(key=="RSales_Payments_Monthly") {
     result.ChartLabels.Add(Format(reader["ShYear"])+" / "+Format(reader["ShMonthName"]));
     result.OrderAmounts.Add(reader["OrderCost"] is DBNull ? 0 : Convert.ToDouble(reader["OrderCost"]));
     result.PaymentAmounts.Add(reader["PaymentCost"] is DBNull ? 0 : Convert.ToDouble(reader["PaymentCost"]));
    }
   }
   result.Tables.Add(table);
  } while(await reader.NextResultAsync(token));
  return Ok(result);
 }
 public static string ShDate(DateTime value) {var p=new PersianCalendar();return string.Format(CultureInfo.InvariantCulture,"{0:0000}/{1:00}/{2:00}",p.GetYear(value),p.GetMonth(value),p.GetDayOfMonth(value));}
 public static string Format(object? value) => value switch {
  null or DBNull=>"",DateTime date=>ShDate(date)+(date.TimeOfDay==TimeSpan.Zero?"":" "+date.ToString("HH:mm",CultureInfo.InvariantCulture)),
  double number=>number.ToString("N2",CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.'),
  decimal number=>number.ToString("N2",CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.'),
  _=>Convert.ToString(value,CultureInfo.InvariantCulture)??""
 };
}
