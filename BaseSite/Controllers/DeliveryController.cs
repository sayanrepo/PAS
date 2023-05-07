using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Delivery;
using BaseSite.Models.Log;
using BaseSite.Models.Order;
using BaseSite.Models.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseSite.Controllers
{
    public class DeliveryController : Controller
    {
        [CustomAuthorize(OPERATIONS.Delivery)]
        public ActionResult DeliveryList(int? orderId, int? saleId, int? docNumber, byte? deliveryStatusId, int? customerId, string deliveryDateFrom, string deliveryDateTo)
        {
            if (orderId.HasValue)
            {
                Order_Order order = OrderManager.Order_Order_Get(orderId.Value);
                docNumber = order.DocNumber;
                deliveryStatusId = null;
                customerId = null;
                deliveryDateFrom = "";
                deliveryDateTo = "";
            }
            else if (saleId.HasValue)
            {
                Sale_Sale sale = SaleManager.Sale_Sale_Get(saleId.Value);
                docNumber = sale.DocNumber;
                deliveryStatusId = null;
                customerId = null;
                deliveryDateFrom = "";
                deliveryDateTo = "";
            }
            else
            {
                customerId = (int?)Session["customerId"];
            }

            ViewBag.docNumber = docNumber;
            ViewBag.deliveryStatus = deliveryStatusId;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.deliveryDateFrom = deliveryDateFrom;
            ViewBag.deliveryDateTo = deliveryDateTo;

            List<Delivery_Delivery> deliveryList = DeliveryManager.Delivery_Delivery_Search(docNumber, deliveryStatusId, customerId,
                string.IsNullOrEmpty(deliveryDateFrom) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(deliveryDateTo) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateTo.Replace('-', '/')).ToDateTime().AddHours(24).AddSeconds(-1));

            ViewBag.RowCount = deliveryList.Count();
            return View(deliveryList);
        }

        [CustomAuthorize(OPERATIONS.Delivery_Add)]
        public ActionResult AddOrderDelivery(int orderId)
        {
            Delivery_Delivery obj = new Delivery_Delivery();
            obj.Order_Order = OrderManager.Order_Order_Get(orderId);
            obj.DocNumber = OrderManager.Order_GenerateDocNumber(obj.Order_Order.DocNumber, 5);
            obj.OrderId = orderId;
            obj.Date = DateTime.Now.Date;
            obj.PackTypeId = obj.Order_Order.PackTypeId;
            obj.SendResponsible = (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).FullName;
            obj.RecieveResponsible = obj.Order_Order.ClienteleName;
            obj.DeliveryLocationId = 1;
            obj.VehicleTypeId = 1;
            obj.RecieverName = obj.Order_Order.Account_Users.FullName;
            obj.RecieverPhone = obj.Order_Order.Account_Users.Phone1;
            obj.RecieverMobile = obj.Order_Order.Account_Users.ResponsiblePhone1;
            obj.DestinationType = 2;
            obj.DestinationAddress = obj.Order_Order.DeliveryAddress;
            obj.StatusId = (byte)DeliveryStatus.SaderShode;

            obj.Items = new List<CheckableItem>();
            foreach (Order_Cabin c in obj.Order_Order.Order_Cabin)
            {
                if (c.Tb_CabinPanels.Id > 0)
                {
                    if (!c.DeliveryId.HasValue)
                    {
                        obj.Items.Add(new CheckableItem() { Type = 1, Id = c.Id, Checked = false, Name = "پنل داخل کابین: " + c.Tb_CabinPanels.Name + " " + c.Tb_CabinPanels.Description, Count = c.Count, Comment = c.DeliveryComment });
                    }
                    foreach (var attach in c.Order_Panel_Attachment)
                    {
                        if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && !attach.DeliveryId.HasValue)
                        {
                            obj.Items.Add(new CheckableItem() { Type = 11, Id = attach.Id, Checked = false, Name = "ملحقات داخل کابین: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                        }
                    }
                }
            }
            foreach (Order_Hall h in obj.Order_Order.Order_Hall)
            {
                if (h.Tb_HallPanels.Id > 0)
                {
                    if (!h.DeliveryId.HasValue)
                    {
                        obj.Items.Add(new CheckableItem() { Type = 2, Id = h.Id, Checked = false, Name = "پنل طبقات: " + h.Tb_HallPanels.Name + " " + h.Tb_HallPanels.Description, Count = h.Count, Comment = h.DeliveryComment });
                    }
                    foreach (var attach in h.Order_Panel_Attachment)
                    {
                        if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && !attach.DeliveryId.HasValue)
                        {
                            obj.Items.Add(new CheckableItem() { Type = 12, Id = attach.Id, Checked = false, Name = "ملحقات طبقات: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                        }
                    }
                }
            }
            foreach (Order_DoorTop d in obj.Order_Order.Order_DoorTop)
            {
                if (d.Tb_DoorTopPanels.Id > 0)
                {
                    if (!d.DeliveryId.HasValue)
                    {
                        obj.Items.Add(new CheckableItem() { Type = 3, Id = d.Id, Checked = false, Name = "پنل سردرب: " + d.Tb_DoorTopPanels.Name + " " + d.Tb_DoorTopPanels.Description, Count = d.Count, Comment = d.DeliveryComment });
                    }
                    foreach (var attach in d.Order_Panel_Attachment)
                    {
                        if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && !attach.DeliveryId.HasValue)
                        {
                            obj.Items.Add(new CheckableItem() { Type = 13, Id = attach.Id, Checked = false, Name = "ملحقات سردرب: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                        }
                    }
                }
            }

            ViewBag.CustomerAddress = obj.Order_Order.Account_Users.Address1;
            ViewBag.ProjectAddress = obj.Order_Order.DeliveryAddress;

            Dictionary<byte, string> temp = new Dictionary<byte, string>();
            foreach (KeyValuePair<byte, string> kv in Models.Cache.Delivery_DeliveryStatus)
            {
                if (kv.Key == (byte)Models.DeliveryStatus.SaderShode)
                    temp.Add(kv.Key, kv.Value);
            }
            ViewBag.DeliveryStatus = temp;

            return View("DeliveryDetail", obj);
        }

        [CustomAuthorize(OPERATIONS.Delivery_Add)]
        public ActionResult AddSaleDelivery(int saleId)
        {
            Delivery_Delivery obj = new Delivery_Delivery();
            obj.Sale_Sale = SaleManager.Sale_Sale_Get(saleId);
            obj.DocNumber = OrderManager.Order_GenerateDocNumber(obj.Sale_Sale.DocNumber, 5);
            obj.SaleId = saleId;
            obj.Date = DateTime.Now.Date;
            obj.PackTypeId = 0;
            obj.SendResponsible = (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).FullName;
            obj.RecieveResponsible = obj.Sale_Sale.ClienteleName;
            obj.DeliveryLocationId = 1;
            obj.VehicleTypeId = 1;
            obj.RecieverName = obj.Sale_Sale.Account_Users.FullName;
            obj.RecieverPhone = obj.Sale_Sale.Account_Users.Phone1;
            obj.RecieverMobile = obj.Sale_Sale.Account_Users.ResponsiblePhone1;
            obj.DestinationType = 2;
            obj.DestinationAddress = obj.Sale_Sale.DeliveryAddress;
            obj.StatusId = (byte)DeliveryStatus.SaderShode;

            obj.Items = new List<CheckableItem>();
            foreach (Sale_Goods c in obj.Sale_Sale.Sale_Goods)
            {
                if (!c.DeliveryId.HasValue)
                {
                    obj.Items.Add(new CheckableItem() { Type = 4, Id = c.Id, Checked = false, Name = c.Name, Count = c.Count, Comment = c.Comment });
                }
            }

            ViewBag.CustomerAddress = obj.Sale_Sale.Account_Users.Address1;
            ViewBag.ProjectAddress = obj.Sale_Sale.DeliveryAddress;

            Dictionary<byte, string> temp = new Dictionary<byte, string>();
            foreach (KeyValuePair<byte, string> kv in Models.Cache.Delivery_DeliveryStatus)
            {
                if (kv.Key == (byte)Models.DeliveryStatus.SaderShode)
                    temp.Add(kv.Key, kv.Value);
            }
            ViewBag.DeliveryStatus = temp;

            return View("DeliveryDetail", obj);
        }

        [CustomAuthorize(OPERATIONS.Delivery_Detail)]
        public ActionResult DeliveryDetail(string deliveryId)
        {
            Delivery_Delivery delivery = DeliveryManager.Delivery_Delivery_Get(int.Parse(deliveryId));

            if (delivery.OrderId.HasValue)
            {
                ViewBag.CustomerAddress = delivery.Order_Order.Account_Users.Address1;
                ViewBag.ProjectAddress = delivery.Order_Order.DeliveryAddress;
            }
            else if (delivery.SaleId.HasValue)
            {
                ViewBag.CustomerAddress = delivery.Sale_Sale.Account_Users.Address1;
                ViewBag.ProjectAddress = delivery.Sale_Sale.DeliveryAddress;
            }

            Dictionary<byte, string> temp = new Dictionary<byte, string>();
            foreach (KeyValuePair<byte, string> kv in Models.Cache.Delivery_DeliveryStatus)
            {
                if (kv.Key == delivery.StatusId)
                {
                    temp.Add(kv.Key, kv.Value);
                }
                else if (kv.Key == delivery.StatusId + 1)
                {
                    if (kv.Key == (byte)Models.DeliveryStatus.TayidShode)
                    {
                        if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Delivery_Add))
                            temp.Add(kv.Key, kv.Value);
                    }
                    else if (kv.Key == (byte)Models.DeliveryStatus.ErsalShode)
                    {
                        if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Delivery_Confirm))
                            temp.Add(kv.Key, kv.Value);
                    }
                }
            }
            ViewBag.DeliveryStatus = temp;

            return View("DeliveryDetail", delivery);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Delivery_Add, OPERATIONS.Delivery_Confirm)] //Delivery_Add or Delivery_Confirm
        public ActionResult DeliveryDetail(Delivery_Delivery model, string submit)
        {
            bool isnew = true;
            if (model.Id > 0)
            {
                isnew = false;
                Delivery_Delivery entity = DeliveryManager.Delivery_Delivery_Get(model.Id);
                if (entity.StatusId == (byte)DeliveryStatus.SaderShode && (model.Items == null || model.Items.Any(m => m.Checked) == false))
                {
                    return View("Error", "", "هیچ آیتمی برای تحویل انتخاب نشده است.");
                }
                if (entity.StatusId > model.StatusId)
                    return RedirectToAction("AccessDenied", "Home");
            }
            else
            {
                if (model.Items == null || model.Items.Any(m => m.Checked) == false)
                {
                    return View("Error", "", "هیچ آیتمی برای تحویل انتخاب نشده است.");
                }
                if (model.OrderId.HasValue)
                {
                    Order_Order order = OrderManager.Order_Order_Get(model.OrderId.Value);
                    if (order.StatusId < (byte)OrderStatus.AmadeTahvil)
                        return RedirectToAction("AccessDenied", "Home");
                }
                if (model.SaleId.HasValue)
                {
                    Sale_Sale sale = SaleManager.Sale_Sale_Get(model.SaleId.Value);
                    if (sale.StatusId < (byte)OrderStatus.MojavezKhorooj)
                        return RedirectToAction("AccessDenied", "Home");
                }
            }

            Delivery_Delivery x = DeliveryManager.Delivery_Delivery_Edit(model, submit);

            if (isnew) LogManager.Log_Logs_Add((int)DB_Table.Delivery_Delivery, x.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, x.ToString());
            else LogManager.Log_Logs_Add((int)DB_Table.Delivery_Delivery, x.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, x.ToString());

            if (x.OrderId.HasValue)
            {
                Order_Order o = OrderManager.Order_Order_ChangeStatus(model.OrderId.Value, OrderStatus.MojavezKhorooj);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, o.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.ChangeStatus, o.Order_Status.Name);
            }
            else if (x.SaleId.HasValue)
            {
                Sale_Sale s = SaleManager.Sale_Sale_ChangeStatus(model.SaleId.Value, OrderStatus.MojavezKhorooj);
                LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, s.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.ChangeStatus, s.Order_Status.Name);
            }

            return RedirectToAction("DeliveryDetail", new { deliveryId = x.Id });
        }

        [CustomAuthorize(OPERATIONS.Delivery_Search)]
        public ActionResult SearchDelivery(int? docNumber, byte? deliveryStatusId, int? customerId, string Customer, string deliveryDateFrom, string deliveryDateTo)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (deliveryStatusId.HasValue) paramlist += ("deliveryStatusId=" + deliveryStatusId.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateFrom)) paramlist += ("deliveryDateFrom=" + deliveryDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateTo)) paramlist += ("deliveryDateTo=" + deliveryDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Delivery/DeliveryList" + paramlist));
        }

        [CustomAuthorize(OPERATIONS.Delivery_Print)]
        public ActionResult Print(string doc, int id)
        {
            if (doc == "delivery")
            {
                Delivery_Delivery model = DeliveryManager.Delivery_Delivery_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Delivery_Delivery, model.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ فرم تحویل کالا");
                return View("PrintDelivery", model);
            }
            else if (doc == "deliveryPack")
            {
                Delivery_Delivery model = DeliveryManager.Delivery_Delivery_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Delivery_Delivery, model.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ فرم محموله");
                return View("PrintDeliveryPack", model);
            }
            else
                return View("Error");
        }
    }
}
