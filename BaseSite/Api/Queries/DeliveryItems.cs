#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;
using BaseSite.Models.Delivery;
namespace BaseSite.Api.Queries;
public static class DeliveryItems {
 public static List<DeliveryItem> ForOrder(Order_Order order, int? deliveryId = null) {
  var result=new List<DeliveryItem>();
  bool Include(int? assigned) => deliveryId.HasValue ? assigned == deliveryId : !assigned.HasValue;
  void Attachments(IEnumerable<Order_Panel_Attachment> items, byte type) {
   foreach(var a in items)
    if(Include(a.DeliveryId) && (a.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(a.AttachmentId)))
     result.Add(new() {Type=type,Id=a.Id,Name="ملحقات: "+a.Tb_Attachments.Name,Count=a.Count,Comment=a.DeliveryComment});
  }
  foreach(var c in order.Order_Cabin.Where(x=>x.CabinPanelId>0)) {
   if(Include(c.DeliveryId)) result.Add(new(){Type=1,Id=c.Id,Name="پنل داخل کابین: "+c.Tb_CabinPanels.Name,Count=c.Count,Comment=c.DeliveryComment});
   Attachments(c.Order_Panel_Attachment,11);
  }
  foreach(var h in order.Order_Hall.Where(x=>x.HallPanelId>0)) {
   if(Include(h.DeliveryId)) result.Add(new(){Type=2,Id=h.Id,Name="پنل طبقات: "+h.Tb_HallPanels.Name,Count=h.Count,Comment=h.DeliveryComment});
   Attachments(h.Order_Panel_Attachment,12);
  }
  foreach(var d in order.Order_DoorTop.Where(x=>x.DoorTopPanelId>0)) {
   if(Include(d.DeliveryId)) result.Add(new(){Type=3,Id=d.Id,Name="پنل سردرب: "+d.Tb_DoorTopPanels.Name,Count=d.Count,Comment=d.DeliveryComment});
   Attachments(d.Order_Panel_Attachment,13);
  }
  return result;
 }
 public static List<DeliveryItem> ForSale(Sale_Sale sale,int? deliveryId=null) => sale.Sale_Goods
  .Where(x=>deliveryId.HasValue ? x.DeliveryId==deliveryId : !x.DeliveryId.HasValue)
  .Select(x=>new DeliveryItem {Type=4,Id=x.Id,Name=x.Name,Count=x.Count,Comment=x.DeliveryComment ?? x.Comment}).ToList();
}
