#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
namespace BaseSite.Api.Controllers;
[ApiController]
[Authorize(Roles=nameof(OPERATIONS.Delivery))]
[Route("api/deliveries")]
public sealed class DeliveriesController(ILogger<DeliveriesController> logger) : ControllerBase {
 [HttpGet]
 public async Task<ActionResult<DeliveryPage>> Get([FromQuery] DeliverySearch filter,CancellationToken token) {
  if(filter.HasFilters && !User.IsInRole(nameof(OPERATIONS.Delivery_Search))) return Forbid();
  using var db=new PantaEntities();
  var query=DeliveryQueries.Apply(db.Delivery_Delivery.AsNoTracking(),filter);
  var count=await query.CountAsync(token);
  var offset=filter.Page*filter.PageSize;
  var items=await DeliveryQueries.Project(query.OrderByDescending(x=>x.Date).ThenByDescending(x=>x.Order_Order.FactorNumber)
   .ThenByDescending(x=>x.Sale_Sale.FactorNumber).ThenByDescending(x=>x.Id).Skip(offset).Take(filter.PageSize)).ToListAsync(token);
  for(var i=0;i<items.Count;i++) items[i].RowNumber=offset+i+1;
  return Ok(new DeliveryPage {Items=items,TotalCount=count});
 }
 [HttpGet("lookups")]
 [Authorize(Roles=nameof(OPERATIONS.Delivery_Search)+","+nameof(OPERATIONS.Delivery_Add))]
 public ActionResult<DeliveryLookups> Lookups() {
  using var db=new PantaEntities();
  return Ok(new DeliveryLookups {
   Statuses=db.Delivery_Status.OrderBy(x=>x.Id).Select(x=>new OrderLookup{Id=x.Id,Name=x.Name}).ToList(),
   Packs=db.Tb_PackTypes.OrderBy(x=>x.Id).Select(x=>new OrderLookup{Id=x.Id,Name=x.Name}).ToList(),
   Locations=db.Delivery_DeliveryLocations.OrderBy(x=>x.Id).Select(x=>new OrderLookup{Id=x.Id,Name=x.Name}).ToList(),
   Vehicles=db.Delivery_VehicleTypes.OrderBy(x=>x.Id).Select(x=>new OrderLookup{Id=x.Id,Name=x.Name}).ToList()
  });
 }
 [HttpGet("customers")]
 [Authorize(Roles=nameof(OPERATIONS.Delivery_Search))]
 public async Task<ActionResult<List<OrderLookup>>> Customers(string? term,CancellationToken token) {
  using var db=new PantaEntities();
  var query=db.Account_Users.Where(x=>x.Id>0 && db.Delivery_Delivery.Any(d=>d.OrderId.HasValue ? d.Order_Order.CustomerId==x.Id : d.Sale_Sale.CustomerId==x.Id));
  if(!string.IsNullOrWhiteSpace(term)) { var text=term.Trim(); query=query.Where(x=>((x.Name??"")+" "+(x.LastName??"")).Contains(text)); }
  return Ok(await query.OrderBy(x=>x.Name).ThenBy(x=>x.Id).Take(30).Select(x=>new OrderLookup{Id=x.Id,Name=(x.Name??"")+" "+(x.LastName??"")}).ToListAsync(token));
 }
 private static IQueryable<Order_Order> Orders(PantaEntities db) => db.Order_Order.Include("Account_Users")
  .Include("Order_Cabin.Tb_CabinPanels").Include("Order_Cabin.Order_Panel_Attachment.Tb_Attachments")
  .Include("Order_Hall.Tb_HallPanels").Include("Order_Hall.Order_Panel_Attachment.Tb_Attachments")
  .Include("Order_DoorTop.Tb_DoorTopPanels").Include("Order_DoorTop.Order_Panel_Attachment.Tb_Attachments");
 private static IQueryable<Sale_Sale> Sales(PantaEntities db) => db.Sale_Sale.Include("Account_Users").Include("Sale_Goods");
 [HttpGet("{id:int}")]
 [Authorize(Roles=nameof(OPERATIONS.Delivery_Detail))]
 public async Task<ActionResult<DeliveryDetail>> Detail(int id,CancellationToken token) {
  using var db=new PantaEntities();
  var query=db.Delivery_Delivery.Where(x=>x.Id==id && x.Id>0);
  var summary=await DeliveryQueries.Project(query).SingleOrDefaultAsync(token);
  if(summary is null) return NotFound(new ProblemDetails{Title="تحویل یافت نشد."});
  var d=await query.Include("Tb_PackTypes").Include("Delivery_DeliveryLocations").Include("Delivery_VehicleTypes").SingleAsync(token);
  var items=d.OrderId.HasValue ? DeliveryItems.ForOrder(await Orders(db).SingleAsync(x=>x.Id==d.OrderId.Value,token),id)
   : d.SaleId.HasValue ? DeliveryItems.ForSale(await Sales(db).SingleAsync(x=>x.Id==d.SaleId.Value,token),id) : [];
  return Ok(new DeliveryDetail {Summary=summary,Items=items,Fields=new(){
   ["نوع بسته‌بندی"]=d.Tb_PackTypes?.Name??"",["محل تحویل"]=d.Delivery_DeliveryLocations?.Name??"",["وسیله حمل"]=d.Delivery_VehicleTypes?.Name??"",
   ["مسئول ارسال"]=d.SendResponsible??"",["مسئول دریافت"]=d.RecieveResponsible??"",["تحویل گیرنده"]=d.RecieverName??"",
   ["تلفن"]=d.RecieverPhone??"",["موبایل"]=d.RecieverMobile??"",["آدرس تحویل"]=d.DestinationAddress??"",
   ["باربری"]=d.CarierAgencyName??"",["شماره بارنامه"]=d.CarierAgencyBill??"",["پلاک خودرو"]=d.VehiclePlaque??"",
   ["راننده"]=d.DriverName??"",["تلفن راننده"]=d.DriverPhone??""
  }});
 }
 [HttpGet("source/{kind}/{number:int}")]
 [Authorize(Roles=nameof(OPERATIONS.Delivery_Add))]
 public ActionResult<DeliveryDraft> Source(string kind,int number) {
  using var db=new PantaEntities();
  var order=kind=="orders" ? Orders(db).SingleOrDefault(x=>x.DocNumber==number && x.Id>0) : null;
  var sale=kind=="sales" ? Sales(db).SingleOrDefault(x=>x.DocNumber==number && x.Id>0 && x.StoreId==1) : null;
  if(order is null && sale is null) return NotFound(new ProblemDetails{Title="سند مبدأ یافت نشد."});
  if(order is not null && order.StatusId<(byte)OrderStatus.AmadeTahvil || sale is not null && sale.StatusId<(byte)OrderStatus.MojavezKhorooj)
   return BadRequest(new ProblemDetails{Title="سند مبدأ هنوز مجوز تحویل ندارد."});
  var customer=order?.Account_Users ?? sale!.Account_Users;
  return Ok(new DeliveryDraft {Customer=(customer.Name??"")+" "+(customer.LastName??""),Form=new(){
   Kind=kind,SourceId=order?.Id??sale!.Id,PackTypeId=order?.PackTypeId??0,RecieveResponsible=order?.ClienteleName??sale?.ClienteleName,
   RecieverName=(customer.Name??"")+" "+(customer.LastName??""),RecieverPhone=customer.Phone1,RecieverMobile=customer.ResponsiblePhone1,
   DestinationAddress=order?.DeliveryAddress??sale?.DeliveryAddress,
   Items=order is not null ? DeliveryItems.ForOrder(order) : DeliveryItems.ForSale(sale!)
  }});
 }
 [HttpPost]
 [Authorize(Roles=nameof(OPERATIONS.Delivery_Add))]
 public async Task<ActionResult<CreatedDocument>> Create(DeliveryForm form) {
  if(!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var userId)) return Unauthorized();
  if(form.Items is null || !form.Items.Any(x=>x.Selected) || form.Items.Any(x=>!Validator.TryValidateObject(x,new ValidationContext(x),new List<ValidationResult>(),true)))
   return BadRequest(new ProblemDetails{Title="حداقل یک قلم معتبر برای تحویل انتخاب کنید."});
  using var db=new PantaEntities();
  // Lock source and item reads until the delivery is committed, preventing duplicate assignment.
  using var transaction=db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
  var order=form.Kind=="orders" ? Orders(db).SingleOrDefault(x=>x.Id==form.SourceId && x.Id>0) : null;
  var sale=form.Kind=="sales" ? Sales(db).SingleOrDefault(x=>x.Id==form.SourceId && x.Id>0 && x.StoreId==1) : null;
  if(order is null && sale is null) return NotFound();
  if(order is not null && order.StatusId<(byte)OrderStatus.AmadeTahvil || sale is not null && sale.StatusId<(byte)OrderStatus.MojavezKhorooj) return Forbid();
  var available=order is not null ? DeliveryItems.ForOrder(order) : DeliveryItems.ForSale(sale!);
  var selected=form.Items.Where(x=>x.Selected).ToList();
  if(selected.Select(x=>(x.Type,x.Id)).Distinct().Count()!=selected.Count || selected.Any(x=>!available.Any(a=>a.Type==x.Type && a.Id==x.Id)))
   return BadRequest(new ProblemDetails{Title="برخی اقلام دیگر قابل تحویل نیستند. سند مبدأ را دوباره دریافت کنید."});
  if(!db.Tb_PackTypes.Any(x=>x.Id==form.PackTypeId) || !db.Delivery_DeliveryLocations.Any(x=>x.Id==form.DeliveryLocationId) || !db.Delivery_VehicleTypes.Any(x=>x.Id==form.VehicleTypeId))
   return BadRequest(new ProblemDetails{Title="بسته‌بندی، محل تحویل و وسیله حمل را انتخاب کنید."});
  var number=(order?.DocNumber??sale!.DocNumber)+5000000;
  while(db.Delivery_Delivery.Any(x=>x.DocNumber==number)) number=checked(number+1000000);
  var d=new Delivery_Delivery {TableId=20,DocNumber=number,OrderId=order?.Id,SaleId=sale?.Id,Date=DateTime.Today,StatusId=(byte)DeliveryStatus.SaderShode,
   PackTypeId=form.PackTypeId,DeliveryLocationId=form.DeliveryLocationId,VehicleTypeId=form.VehicleTypeId,
   SendResponsible=form.SendResponsible,RecieveResponsible=form.RecieveResponsible,RecieverName=form.RecieverName,
   RecieverPhone=form.RecieverPhone,RecieverMobile=form.RecieverMobile,DestinationType=2,DestinationAddress=form.DestinationAddress,
   CarierAgencyName=form.CarierAgencyName,CarierAgencyBill=form.CarierAgencyBill,VehiclePlaque=form.VehiclePlaque,
   DriverName=form.DriverName,DriverPhone=form.DriverPhone};
  foreach(var item in selected) {
   switch(item.Type) {
    case 1: var c=order!.Order_Cabin.Single(x=>x.Id==item.Id); c.DeliveryComment=item.Comment; d.Order_Cabin.Add(c); break;
    case 2: var h=order!.Order_Hall.Single(x=>x.Id==item.Id); h.DeliveryComment=item.Comment; d.Order_Hall.Add(h); break;
    case 3: var t=order!.Order_DoorTop.Single(x=>x.Id==item.Id); t.DeliveryComment=item.Comment; d.Order_DoorTop.Add(t); break;
    case 4: var g=sale!.Sale_Goods.Single(x=>x.Id==item.Id); g.DeliveryComment=item.Comment; d.Sale_Goods.Add(g); break;
    default: var a=db.Order_Panel_Attachment.Single(x=>x.Id==item.Id); a.DeliveryComment=item.Comment; d.Order_Panel_Attachment.Add(a); break;
   }
  }
  if(order is not null && order.StatusId<(byte)OrderStatus.MojavezKhorooj) { order.StatusId=(byte)OrderStatus.MojavezKhorooj; order.DateFactor=DateTime.Now; }
  db.Delivery_Delivery.Add(d);
  await db.SaveChangesAsync();
  transaction.Commit();
  try { LogManager.Log_Logs_Add((int)DB_Table.Delivery_Delivery,d.DocNumber,userId,HttpContext.Connection.RemoteIpAddress?.ToString(),(int)LogActivity.Add,"ثبت تحویل"); }
  catch(Exception ex) { logger.LogError(ex,"Unable to record delivery audit {Id}",d.Id); }
  return Ok(new CreatedDocument {Id=d.Id,DocumentNumber=d.DocNumber});
 }
}
