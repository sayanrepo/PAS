#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;
using BaseSite.Models.Delivery;
namespace BaseSite.Api.Queries;
public static class DeliveryItems {
 public static string TypeName(byte type) => type switch { 1=>"پنل داخل کابین",11=>"ملحقات داخل کابین",2=>"پنل طبقات",12=>"ملحقات طبقات",3=>"پنل سردرب",13=>"ملحقات سردرب",4=>"فروش قطعه",_=>"" };
 public static List<DeliveryItem> ForOrder(Order_Order order, int? deliveryId = null, bool includeUnassigned = false) {
  var result=new List<DeliveryItem>();
  bool Include(int? assigned) => deliveryId.HasValue ? assigned == deliveryId || includeUnassigned && !assigned.HasValue : !assigned.HasValue;
  void Attachments(IEnumerable<Order_Panel_Attachment> items, byte type) {
   foreach(var a in items)
    if(Include(a.DeliveryId) && (a.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(a.AttachmentId)))
     result.Add(new() {Type=type,Id=a.Id,Name=TypeName(type)+": "+a.Tb_Attachments.Name+" "+a.Tb_Attachments.Description,
      Model=a.Tb_Attachments.Name,TypeName=TypeName(type),Count=a.Count,Selected=deliveryId.HasValue && a.DeliveryId==deliveryId,Comment=a.DeliveryComment});
  }
  foreach(var c in order.Order_Cabin.Where(x=>x.CabinPanelId>0)) {
   if(Include(c.DeliveryId)) result.Add(new(){Type=1,Id=c.Id,Name="پنل داخل کابین: "+c.Tb_CabinPanels.Name+" "+c.Tb_CabinPanels.Description,
    Model=c.Tb_CabinPanels.Name,TypeName=TypeName(1),Count=c.Count,Selected=deliveryId.HasValue && c.DeliveryId==deliveryId,Comment=c.DeliveryComment});
   Attachments(c.Order_Panel_Attachment,11);
  }
  foreach(var h in order.Order_Hall.Where(x=>x.HallPanelId>0)) {
   if(Include(h.DeliveryId)) result.Add(new(){Type=2,Id=h.Id,Name="پنل طبقات: "+h.Tb_HallPanels.Name+" "+h.Tb_HallPanels.Description,
    Model=h.Tb_HallPanels.Name,TypeName=TypeName(2),Count=h.Count,Selected=deliveryId.HasValue && h.DeliveryId==deliveryId,Comment=h.DeliveryComment});
   Attachments(h.Order_Panel_Attachment,12);
  }
  foreach(var d in order.Order_DoorTop.Where(x=>x.DoorTopPanelId>0)) {
   if(Include(d.DeliveryId)) result.Add(new(){Type=3,Id=d.Id,Name="پنل سردرب: "+d.Tb_DoorTopPanels.Name+" "+d.Tb_DoorTopPanels.Description,
    Model=d.Tb_DoorTopPanels.Name,TypeName=TypeName(3),Count=d.Count,Selected=deliveryId.HasValue && d.DeliveryId==deliveryId,Comment=d.DeliveryComment});
   Attachments(d.Order_Panel_Attachment,13);
  }
  return result;
 }
 public static List<DeliveryItem> ForSale(Sale_Sale sale,int? deliveryId=null,bool includeUnassigned=false) => sale.Sale_Goods
  .Where(x=>deliveryId.HasValue ? x.DeliveryId==deliveryId || includeUnassigned && !x.DeliveryId.HasValue : !x.DeliveryId.HasValue)
  .Select(x=>new DeliveryItem {Type=4,Id=x.Id,Name=x.Name,Model=x.Name,TypeName=TypeName(4),Count=x.Count,
   Selected=deliveryId.HasValue && x.DeliveryId==deliveryId,Comment=x.DeliveryComment ?? x.Comment}).ToList();
}
