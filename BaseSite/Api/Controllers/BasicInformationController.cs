#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
namespace BaseSite.Api.Controllers;
[ApiController]
[Authorize]
[Route("api/basic")]
public sealed class BasicInformationController(ILogger<BasicInformationController> logger):ControllerBase {
 private static readonly (string Key,string Label,string Kind,string Role)[] Fields=[
  ("Name","نام","text","Setting_Name"),("Description","توضیحات","text","Setting_Description"),
  ("Cost","قیمت (ریال)","number","Setting_Cost"),("ProductFactor","ضریب کارکرد","number","Setting_ProductFactor"),
  ("Available","موجود","bool","Setting_Available"),("IsDeliveryItem","قابل تحویل","bool",""),
  ("SurfaceArea","مساحت رویه","number","Setting_SurfaceArea"),("StartFrom","شروع تولید از","choice","Setting_ProductFactor"),
  ("Height","ارتفاع","number","Setting_Size"),("Width","عرض","number","Setting_Size"),("Depth","عمق","number","Setting_Size")
 ];
 private List<BasicField> Schema(BasicDefinition d) {
  var rich=d.TableId!=0;
  return Fields.Where(f=>d.EntityType.GetProperty(f.Key)!=null && (rich || f.Key=="Name"))
   .Where(f=>f.Key is "Name" or "Description" || f.Role=="" || User.IsInRole(f.Role))
   .Select(f=>new BasicField{Key=f.Key,Label=f.Label,Kind=f.Kind,
    MaxLength=d.EntityType.GetProperty(f.Key)!.GetCustomAttributes(typeof(MaxLengthAttribute),true).Cast<MaxLengthAttribute>().FirstOrDefault()?.Length??(f.Key=="Description"?255:100),
    CanEdit=!rich || f.Role=="" || User.IsInRole(f.Role),CanAdd=!rich || f.Key is "Name" or "Description" || f.Role=="" || User.IsInRole(f.Role)}).ToList();
 }
 [HttpGet("{key}")]
 public ActionResult<BasicTable> Get(string key,int? parentId) {
  var d=BasicCatalog.All.SingleOrDefault(x=>x.Key==key);if(d is null)return NotFound();
  if(!User.IsInRole(d.ReadRole))return Forbid();
  using var db=new PantaEntities();
  var schema=Schema(d);
  var stage=schema.SingleOrDefault(x=>x.Key=="StartFrom");
  if(stage is not null)stage.Choices=db.Order_ProductStatus.Select(x=>new OrderLookup{Id=x.Id,Name=x.Name}).ToList();
  var parents=new List<OrderLookup>();
  string? parentKey=d.ParentProperty=="CountryId"?"countries":d.ParentProperty=="ProvinceId"?"provinces":null;
  if(parentKey is not null) {
   parents=(parentKey=="countries"?BasicCatalog.Read<Location_Countries>(db):BasicCatalog.Read<Location_Provinces>(db))
    .Select(x=>new OrderLookup{Id=Convert.ToInt32(BasicCatalog.Get(x,"Id")),Name=BasicCatalog.Get(x,"Name")?.ToString()??""}).OrderBy(x=>x.Name).ToList();
   if(!parentId.HasValue)parentId=parents.FirstOrDefault()?.Id;
   if(parentId.HasValue && !parents.Any(x=>x.Id==parentId))return BadRequest(new ProblemDetails{Title="کشور یا استان معتبر نیست."});
  }
  var rows=d.Read(db,parentId).OrderBy(x=>BasicCatalog.Get(x,"Name")?.ToString()).Select(x=>new BasicRecord{
   Id=Convert.ToInt32(BasicCatalog.Get(x,"Id")),Values=schema.ToDictionary(f=>f.Key,f=>Convert.ToString(BasicCatalog.Get(x,f.Key),CultureInfo.InvariantCulture)??"")
  }).ToList();
  return Ok(new BasicTable{Key=key,Title=d.Title,ParentKey=parentKey,ParentId=parentId,Parents=parents,Fields=schema,Rows=rows,
   CanAdd=User.IsInRole(d.WriteRole+"_Add"),CanEdit=User.IsInRole(d.WriteRole+"_Edit"),CanDelete=User.IsInRole(d.WriteRole+"_Delete")});
 }
 [HttpPost("{key}")]
 public Task<ActionResult> Add(string key,BasicSave request)=>Save(key,0,request);
 [HttpPut("{key}/{id:int}")]
 public Task<ActionResult> Edit(string key,int id,BasicSave request)=>id<=0?Task.FromResult<ActionResult>(BadRequest()):Save(key,id,request);
 private async Task<ActionResult> Save(string key,int id,BasicSave request) {
  var d=BasicCatalog.All.SingleOrDefault(x=>x.Key==key);if(d is null)return NotFound();
  if(!User.IsInRole(d.ReadRole)||!User.IsInRole(d.WriteRole+(id==0?"_Add":"_Edit")))return Forbid();
  using var db=new PantaEntities();
  var set=db.Set(d.EntityType);
  var idType=d.EntityType.GetProperty("Id")!.PropertyType;
  var entity=id==0?Activator.CreateInstance(d.EntityType):set.Find(Convert.ChangeType(id,idType));
  if(entity is null || id>0 && BasicCatalog.Get(entity,"Deleted") is true)return NotFound();
  var schema=Schema(d);
  if(request.Values is null || request.Values.Keys.Any(k=>!schema.Any(f=>f.Key==k)))return BadRequest();
  if(id==0) {
   BasicCatalog.Set(entity,"TableId",d.TableId);BasicCatalog.Set(entity,"Available",true);
   BasicCatalog.Set(entity,"SurfaceArea",1);BasicCatalog.Set(entity,"StartFrom",1);
   if(d.ParentProperty is not null) {
    var parentType=d.ParentProperty=="CountryId"?typeof(Location_Countries):typeof(Location_Provinces);
    var parent=db.Set(parentType).Find(request.ParentId??0);
    if(parent is null || BasicCatalog.Get(parent,"Deleted") is true || request.ParentId<=0)return BadRequest(new ProblemDetails{Title="ابتدا کشور یا استان را انتخاب کنید."});
    BasicCatalog.Set(entity,d.ParentProperty,request.ParentId);
   }
  }
  foreach(var field in schema) {
   if(!request.Values.TryGetValue(field.Key,out var value))continue;
   if(!(id==0?field.CanAdd:field.CanEdit))continue;
   if(field.Kind=="text") {
    value=value.Trim();
    if(value.Length>field.MaxLength || field.Key=="Name" && value.Length==0)return BadRequest(new ProblemDetails{Title="نام و طول متن فیلدها را بررسی کنید."});
    BasicCatalog.Set(entity,field.Key,value);
   } else if(field.Kind=="bool") {
    if(!bool.TryParse(value,out var boolean))return BadRequest();
    BasicCatalog.Set(entity,field.Key,boolean);
   } else {
    if(!double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var number)||!double.IsFinite(number)||number<0)return BadRequest(new ProblemDetails{Title="مقادیر عددی باید معتبر و نامنفی باشند."});
    if(field.Key=="StartFrom" && (number!=Math.Truncate(number)||number>255||!db.Order_ProductStatus.Any(x=>x.Id==(byte)number)))return BadRequest();
    BasicCatalog.Set(entity,field.Key,number);
   }
  }
  if(string.IsNullOrWhiteSpace(BasicCatalog.Get(entity,"Name")?.ToString()))return BadRequest(new ProblemDetails{Title="نام را وارد کنید."});
  if(id==0)set.Add(entity);
  await db.SaveChangesAsync();
  var savedId=Convert.ToInt32(BasicCatalog.Get(entity,"Id"));
  AfterSave(d,savedId,id==0?LogActivity.Add:LogActivity.Edit);
  return Ok(new {Id=savedId});
 }
 [HttpDelete("{key}/{id:int}")]
 public async Task<ActionResult> Delete(string key,int id) {
  var d=BasicCatalog.All.SingleOrDefault(x=>x.Key==key);if(d is null || id<=0)return NotFound();
  if(!User.IsInRole(d.ReadRole)||!User.IsInRole(d.WriteRole+"_Delete"))return Forbid();
  using var db=new PantaEntities();
  var entity=db.Set(d.EntityType).Find(Convert.ChangeType(id,d.EntityType.GetProperty("Id")!.PropertyType));
  if(entity is null || BasicCatalog.Get(entity,"Deleted") is true)return NotFound();
  if(key=="countries") {
   var provinces=db.Location_Provinces.Where(x=>x.CountryId==id).ToList();
   var ids=provinces.Select(x=>x.Id).ToList();
   foreach(var c in db.Location_Cities.Where(x=>ids.Contains(x.ProvinceId)))c.Deleted=true;
   foreach(var p in provinces)p.Deleted=true;
  } else if(key=="provinces") foreach(var c in db.Location_Cities.Where(x=>x.ProvinceId==id))c.Deleted=true;
  BasicCatalog.Set(entity,"Deleted",true);
  await db.SaveChangesAsync();AfterSave(d,id,LogActivity.Delete);return NoContent();
 }
 [HttpPost("{key}/prices")]
 public async Task<ActionResult> Prices(string key,BasicPriceChange change) {
  var d=BasicCatalog.All.SingleOrDefault(x=>x.Key==key);if(d is null||d.EntityType.GetProperty("Cost") is null)return NotFound();
  if(!User.IsInRole(d.ReadRole)||!User.IsInRole(d.WriteRole+"_Edit")||!User.IsInRole(nameof(OPERATIONS.Setting_Cost)))return Forbid();
  if(!double.IsFinite(change.Percent)||!double.IsFinite(change.Amount)||change.Rows is null||change.Rows.Count==0||change.Rows.Any(x=>x.Id<=0)||change.Rows.Select(x=>x.Id).Distinct().Count()!=change.Rows.Count)return BadRequest();
  using var db=new PantaEntities();
  using var transaction=db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
  foreach(var row in change.Rows) {
   var entity=db.Set(d.EntityType).Find(Convert.ChangeType(row.Id,d.EntityType.GetProperty("Id")!.PropertyType));
   if(entity is null||BasicCatalog.Get(entity,"Deleted") is true)return Conflict(new ProblemDetails{Title="فهرست تغییر کرده است. ابتدا تازه‌سازی کنید."});
   var cost=Convert.ToDouble(BasicCatalog.Get(entity,"Cost"));
   if(cost!=row.ExpectedCost)return Conflict(new ProblemDetails{Title="قیمت بعضی اقلام تغییر کرده است. ابتدا تازه‌سازی کنید."});
   var updated=cost*(1+change.Percent/100)+change.Amount;
   if(!double.IsFinite(updated)||updated<0)return BadRequest(new ProblemDetails{Title="تغییر واردشده باعث قیمت نامعتبر می‌شود."});
   BasicCatalog.Set(entity,"Cost",updated);
  }
  await db.SaveChangesAsync();transaction.Commit();
  for(var i=0;i<change.Rows.Count;i++)AfterSave(d,change.Rows[i].Id,LogActivity.Edit,i==0);
  return NoContent();
 }
 private void AfterSave(BasicDefinition d,int id,LogActivity activity,bool refresh=true) {
  try {if(refresh)Cache.Update();}catch(Exception ex){logger.LogError(ex,"Unable to refresh base information cache");}
  try {if(int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var userId))LogManager.Log_Logs_Add(d.TableId,id,userId,HttpContext.Connection.RemoteIpAddress?.ToString(),(int)activity,d.Title);}
  catch(Exception ex){logger.LogError(ex,"Unable to record base information audit");}
 }
 [HttpGet("compatibility/{primary:int}/{secondary:int}")]
 public ActionResult<CompatibilityGrid> Compatibility(int primary,int secondary) {
  if(!User.IsInRole(nameof(OPERATIONS.Setting_TruthTable)))return Forbid();
  if(!BasicCatalog.ValidPair(primary,secondary))return BadRequest();
  using var db=new PantaEntities();
  var a=BasicCatalog.All.Single(x=>x.TableId==primary && new[]{"cabins","halls","door-tops"}.Contains(x.Key));
  var b=BasicCatalog.All.Single(x=>x.Key==(secondary==4?"buttons":"monitors"));
  List<OrderLookup> Rows(BasicDefinition d)=>d.Read(db,null).OrderBy(x=>BasicCatalog.Get(x,"Name")?.ToString()).Select(x=>new OrderLookup{Id=Convert.ToInt32(BasicCatalog.Get(x,"Id")),Name=BasicCatalog.Get(x,"Name")?.ToString()??""}).ToList();
  return Ok(new CompatibilityGrid{Panels=Rows(a),Parts=Rows(b),CanEdit=User.IsInRole(nameof(OPERATIONS.Setting_TruthTable_Edit)),
   Cells=db.Tb_Truth.Where(x=>x.PrimaryTableId==primary&&x.SecondaryTableId==secondary).Select(x=>new CompatibilityCell{PanelId=x.PrimaryId,PartId=x.SecondaryId,Value=x.TValue}).ToList()});
 }
 [HttpPut("compatibility/{primary:int}/{secondary:int}")]
 public async Task<ActionResult> SaveCompatibility(int primary,int secondary,CompatibilityCell cell) {
  if(!User.IsInRole(nameof(OPERATIONS.Setting_TruthTable))||!User.IsInRole(nameof(OPERATIONS.Setting_TruthTable_Edit)))return Forbid();
  if(!BasicCatalog.ValidPair(primary,secondary)||!BasicCatalog.ValidCompatibility(cell.Value))return BadRequest();
  using var db=new PantaEntities();
  var a=BasicCatalog.All.Single(x=>x.TableId==primary && new[]{"cabins","halls","door-tops"}.Contains(x.Key));
  var b=BasicCatalog.All.Single(x=>x.Key==(secondary==4?"buttons":"monitors"));
  if(!a.Read(db,null).Any(x=>Convert.ToInt32(BasicCatalog.Get(x,"Id"))==cell.PanelId)||!b.Read(db,null).Any(x=>Convert.ToInt32(BasicCatalog.Get(x,"Id"))==cell.PartId))return BadRequest();
  using var transaction=db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
  var entity=db.Tb_Truth.SingleOrDefault(x=>x.PrimaryTableId==primary&&x.SecondaryTableId==secondary&&x.PrimaryId==cell.PanelId&&x.SecondaryId==cell.PartId);
  if(entity is null){entity=new(){PrimaryTableId=primary,SecondaryTableId=secondary,PrimaryId=cell.PanelId,SecondaryId=cell.PartId};db.Tb_Truth.Add(entity);}
  entity.TValue=cell.Value;await db.SaveChangesAsync();transaction.Commit();
  AfterSave(a,entity.Id,LogActivity.Edit);return NoContent();
 }
}
