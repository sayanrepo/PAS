#nullable enable
using BaseSite.Data;
using BaseSite.Models.DBModel;
using BaseSite.Models;
using System.Linq.Expressions;
namespace BaseSite.Api.Queries;
public sealed record BasicDefinition(string Key,string Page,string Title,Type EntityType,string ReadRole,string WriteRole,int TableId,string? ParentProperty,Func<PantaEntities,int?,List<object>> Read);
public static class BasicCatalog {
 public static readonly IReadOnlyList<BasicDefinition> All=[
 new("countries","cities","کشورها",typeof(Location_Countries),nameof(OPERATIONS.Setting_Cities),nameof(OPERATIONS.Setting_Cities),0,null,(db,parent)=>Read<Location_Countries>(db,parent,null)),
 new("provinces","cities","استان‌ها",typeof(Location_Provinces),nameof(OPERATIONS.Setting_Cities),nameof(OPERATIONS.Setting_Cities),0,"CountryId",(db,parent)=>Read<Location_Provinces>(db,parent,"CountryId")),
 new("cities","cities","شهرها",typeof(Location_Cities),nameof(OPERATIONS.Setting_Cities),nameof(OPERATIONS.Setting_Cities),0,"ProvinceId",(db,parent)=>Read<Location_Cities>(db,parent,"ProvinceId")),
 new("packs","orders","نوع بسته‌بندی",typeof(Tb_PackTypes),nameof(OPERATIONS.Setting_Order),nameof(OPERATIONS.Setting_Order_Packet),0,null,(db,parent)=>Read<Tb_PackTypes>(db,parent,null)),
 new("boards","orders","نوع تابلو",typeof(Tb_ElevatorBoards),nameof(OPERATIONS.Setting_Order),nameof(OPERATIONS.Setting_Order_ElevatorBoard),0,null,(db,parent)=>Read<Tb_ElevatorBoards>(db,parent,null)),
 new("deductions","orders","کسورات",typeof(Tb_Deductions),nameof(OPERATIONS.Setting_Order),nameof(OPERATIONS.Setting_Order_Deduction),0,null,(db,parent)=>Read<Tb_Deductions>(db,parent,null)),
 new("additions","orders","اضافات",typeof(Tb_Additions),nameof(OPERATIONS.Setting_Order),nameof(OPERATIONS.Setting_Order_Addition),0,null,(db,parent)=>Read<Tb_Additions>(db,parent,null)),
 new("attachments","attachments","ملحقات",typeof(Tb_Attachments),nameof(OPERATIONS.Setting_Attachment),nameof(OPERATIONS.Setting_Attachment),11,null,(db,parent)=>Read<Tb_Attachments>(db,parent,null)),
 new("buttons","buttons","پوش باتون",typeof(Tb_PushButtons),nameof(OPERATIONS.Setting_PushButton),nameof(OPERATIONS.Setting_PushButton),4,null,(db,parent)=>Read<Tb_PushButtons>(db,parent,null)),
 new("monitors","monitors","نمایشگر",typeof(Tb_Monitors),nameof(OPERATIONS.Setting_Monitor),nameof(OPERATIONS.Setting_Monitor),6,null,(db,parent)=>Read<Tb_Monitors>(db,parent,null)),
 new("metals","metals","فلز رویه",typeof(Tb_SurfaceMetals),nameof(OPERATIONS.Setting_CabinSurfaceMetal),nameof(OPERATIONS.Setting_CabinSurfaceMetal),4,null,(db,parent)=>Read<Tb_SurfaceMetals>(db,parent,null)),
 new("cabins","cabins","مدل پنل داخل کابین",typeof(Tb_CabinPanels),nameof(OPERATIONS.Setting_CabinPanel),nameof(OPERATIONS.Setting_CabinPanel),3,null,(db,parent)=>Read<Tb_CabinPanels>(db,parent,null)),
 new("cabin-metals","cabins","فلز رویه داخل کابین",typeof(Tb_CabinSurfaceMetals),nameof(OPERATIONS.Setting_CabinPanel),nameof(OPERATIONS.Setting_CabinSurfaceMetal),4,null,(db,parent)=>Read<Tb_CabinSurfaceMetals>(db,parent,null)),
 new("halls","halls","مدل پنل طبقات",typeof(Tb_HallPanels),nameof(OPERATIONS.Setting_HallPanel),nameof(OPERATIONS.Setting_HallPanel),7,null,(db,parent)=>Read<Tb_HallPanels>(db,parent,null)),
 new("hall-metals","halls","فلز رویه طبقات",typeof(Tb_HallSurfaceMetals),nameof(OPERATIONS.Setting_HallPanel),nameof(OPERATIONS.Setting_HallSurfaceMetal),4,null,(db,parent)=>Read<Tb_HallSurfaceMetals>(db,parent,null)),
 new("door-tops","door-tops","مدل پنل سردرب",typeof(Tb_DoorTopPanels),nameof(OPERATIONS.Setting_DoorTopPanel),nameof(OPERATIONS.Setting_DoorTopPanel),10,null,(db,parent)=>Read<Tb_DoorTopPanels>(db,parent,null)),
 ];
 public static List<object> Read<T>(PantaEntities db,int? parent=null,string? parentProperty=null) where T:class {
  var p=Expression.Parameter(typeof(T),"x");
  Expression filter=Expression.AndAlso(Expression.GreaterThan(Expression.Convert(Expression.Property(p,"Id"),typeof(int)),Expression.Constant(0)),Expression.Not(Expression.Property(p,"Deleted")));
  if(parentProperty is not null) filter=Expression.AndAlso(filter,Expression.Equal(Expression.Convert(Expression.Property(p,parentProperty),typeof(int)),Expression.Constant(parent??-1)));
  return db.Set<T>().Where(Expression.Lambda<Func<T,bool>>(filter,p)).ToList().Cast<object>().ToList();
 }
 public static object? Get(object entity,string property)=>entity.GetType().GetProperty(property)?.GetValue(entity);
 public static void Set(object entity,string property,object? value) {
  var p=entity.GetType().GetProperty(property);
  if(p is not null) p.SetValue(entity,value is null?null:Convert.ChangeType(value,Nullable.GetUnderlyingType(p.PropertyType)??p.PropertyType,System.Globalization.CultureInfo.InvariantCulture));
 }
 public static bool ValidPair(int primary,int secondary)=>new[]{3,7,10}.Contains(primary) && new[]{4,6}.Contains(secondary);
 public static bool ValidCompatibility(double value)=>new[]{0d,.5d,.75d,1d}.Contains(value);
}


