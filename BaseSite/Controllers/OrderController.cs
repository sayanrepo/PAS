using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseSite.Models.Information;
using BaseSite.Models.DBModel;
using BaseSite.Models.Order;
using BaseSite.Models.Account;
using BaseSite.Models;
using BaseSite.Models.Log;

namespace BaseSite.Controllers
{
    public class OrderController : Controller
    {
        private static byte StoreId = 1; //Centeral office

        //***************************************** AutoComplete ******************************
        [HttpPost]
        public JsonResult AutoCompleteUsers(string Prefix)
        {
            var res = (from u in AccountManager.Account_User_Get(Prefix)
                       select new { Name = u.FullName, u.Id }).Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteEmployees(string Prefix)
        {
            var res = (from u in AccountManager.Account_User_Get(Prefix)
                       where u.PartnerTypeId == 1
                       select new { Name = u.FullName, u.Id }).Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteCabinPanels(string Prefix)
        {
            List<Tb_CabinPanels> ObjList = InformationManager.Cabin_Panel_Get(true);

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompletePushButtons(string Prefix, int containerTableId = 0, int containerId = 0)
        {
            List<Tb_PushButtons> ObjList = InformationManager.Cabin_PushButton_Get().Where(u => u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())).ToList();
            List<Tb_Truth> truthList = InformationManager.TruthTable_Get2(containerTableId, containerId);

            var res = (from u in ObjList
                       join t in truthList on new { TId = u.TableId, Id = u.Id } equals new { TId = t.SecondaryTableId, Id = t.SecondaryId } into t2
                       //where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       from t3 in t2.DefaultIfEmpty()
                       select new { u.Name, u.Id, u.Cost, Color = (t3 == null || t3.TValue == 1) ? "#07c21fbf" : (t3.TValue == 0.75 ? "#12beb3bf" : (t3.TValue == 0.5 ? "#d8b90cbf" : "#d90c0cbf")) }).ToList();//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);


            //List<Tb_PushButtons> ObjList = InformationManager.Cabin_PushButton_Get();

            //var res = (from u in ObjList
            //           where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
            //           select new { u.Name, u.Id, u.Cost, Color = "rgba(7, 194, 31, 0.75)" });//.Take(15);
            //return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteMonitors(string Prefix, int containerTableId = 0, int containerId = 0)
        {
            List<Tb_Monitors> ObjList = InformationManager.Cabin_Monitor_Get();
            List<Tb_Truth> truthList = InformationManager.TruthTable_Get2(containerTableId, containerId);

            var res = (from u in ObjList
                       join t in truthList on new { TId = u.TableId, Id = u.Id } equals new { TId = t.SecondaryTableId, Id = t.SecondaryId } into t2
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       from t3 in t2.DefaultIfEmpty()
                       select new { u.Name, u.Id, u.Cost, Color = (t3 == null || t3.TValue == 1) ? "#07c21fbf" : (t3.TValue == 0.75 ? "#12beb3bf" : (t3.TValue == 0.5 ? "#d8b90cbf" : "#d90c0cbf")) }).ToList();//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);


            //List<Tb_Monitors> ObjList = InformationManager.Cabin_Monitor_Get();

            //var res = (from u in ObjList
            //           where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
            //           select new { u.Name, u.Id, u.Cost, Color = "rgba(7, 194, 31, 0.75)" });//.Take(15);
            //return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteProducts(string Prefix)
        {
            List<Tb_Products> ObjList = InformationManager.Products_Get();

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost, u.Description, Color = "rgba(7, 194, 31, 0.75)" });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteAttachments(string Prefix)
        {
            List<Tb_Attachments> ObjList = InformationManager.Order_Attachment_Get();

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteCabinSurfaceMetals(string Prefix)
        {
            List<Tb_CabinSurfaceMetals> ObjList = InformationManager.Cabin_SurfaceMetal_Get();

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteHallPanels(string Prefix)
        {
            List<Tb_HallPanels> ObjList = InformationManager.Hall_Panel_Get(true);

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteHallSurfaceMetals(string Prefix)
        {
            List<Tb_HallSurfaceMetals> ObjList = InformationManager.Hall_SurfaceMetal_Get();

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteDoorTopPanels(string Prefix)
        {
            List<Tb_DoorTopPanels> ObjList = InformationManager.DoorTop_Panel_Get(true);

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteSurfaceMetals(string Prefix)
        {
            List<Tb_SurfaceMetals> ObjList = InformationManager.SurfaceMetal_Get();

            var res = (from u in ObjList
                       where u.Name.Replace(" ", "").ToLower().Contains(Prefix.Replace(" ", "").ToLower())
                       select new { u.Name, u.Id, u.Cost });//.Take(15);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteSaleGoods(string Prefix, byte type)
        {
            if (type == 1)
                return AutoCompletePushButtons(Prefix);
            else if (type == 2)
                return AutoCompleteMonitors(Prefix);
            else if (type == 3)
                return AutoCompleteAttachments(Prefix);
            else if (type == 4)
                return AutoCompleteProducts(Prefix);
            else return Json("");
        }

        public JsonResult GetPersonInfo(int userId)
        {
            Account_Users u = AccountManager.Account_User_Get(userId);
            var res = new
            {
                u.Id,
                Name = u.Name == null ? "" : u.Name,
                LastName = u.LastName == null ? "" : u.LastName,
                u.FullName,
                Address1 = u.Address1 == null ? "-" : u.Address1,
                Address2 = u.Address2 == null ? "-" : u.Address2,
                Fax = u.Fax == null ? "-" : u.Fax,
                Phone1 = u.Phone1 == null ? "-" : u.Phone1,
                Responsible1 = u.Responsible1 == null ? "-" : u.Responsible1,
                ResponsiblePhone1 = u.ResponsiblePhone1 == null ? "-" : u.ResponsiblePhone1
            };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProvinceGet(string countryid)
        {
            var pro = InformationManager.Location_Province_Get(int.Parse(countryid));
            var list = (from p in pro
                        select new
                        {
                            p.Id,
                            p.Name
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CityGet(string provinceid)
        {
            var pro = InformationManager.Location_City_Get(int.Parse(provinceid));
            var list = (from p in pro
                        select new
                        {
                            p.Id,
                            p.Name
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //*************************************************************************
        [CustomAuthorize(OPERATIONS.Order)]
        public ActionResult OrderList(int? docNumber, byte? orderStatusId, byte? orderTradeTypeId, int? customerId, string orderDateFrom, string orderDateTo, string factorDateFrom, string factorDateTo, string projectName)
        {
            customerId = (int?)Session["customerId"];

            ViewBag.docNumber = docNumber;
            ViewBag.orderStatus = orderStatusId;
            ViewBag.orderTradeType = orderTradeTypeId;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.orderDateFrom = orderDateFrom;
            ViewBag.orderDateTo = orderDateTo;
            ViewBag.factorDateFrom = factorDateFrom;
            ViewBag.factorDateTo = factorDateTo;
            ViewBag.projectName = projectName;

            List<Order_Order> OrderList = OrderManager.Order_Order_Search(docNumber, orderStatusId, orderTradeTypeId, customerId,
                string.IsNullOrEmpty(orderDateFrom) ? null : (DateTime?)PersianDateTime.Parse(orderDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(orderDateTo) ? null : (DateTime?)PersianDateTime.Parse(orderDateTo.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(factorDateFrom) ? null : (DateTime?)PersianDateTime.Parse(factorDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(factorDateTo) ? null : (DateTime?)PersianDateTime.Parse(factorDateTo.Replace('-', '/')).ToDateTime(),
                null, null, projectName);

            ViewBag.RowCount = OrderList.Count();
            return View(OrderList);
        }

        [CustomAuthorize(OPERATIONS.Order_Add)]
        public ActionResult AddOrder()
        {
            Order_Order obj = OrderManager.Order_Order_Get(0);
            obj.StoreId = StoreId;
            obj.Order_Cabin.ElementAt(0).Count = 1;
            obj.Order_Hall.ElementAt(0).Count = 1;
            obj.Order_DoorTop.ElementAt(0).Count = 1;

            obj.DateOrder = DateTime.Now;
            obj.DateDelivery = null;
            obj.DateFactor = null; // DateTime.Now.AddDays(10);
            if (Session["customerId"] != null)
            {
                try
                {
                    Account_Users user = AccountManager.Account_User_Get((int)(Session["customerId"]));
                    obj.CustomerId = user.Id;
                    obj.DeliveryAddress = user.Address1;
                    ViewBag.CustomerName = user.FullName;
                }
                catch { }
            }

            Dictionary<byte, string> temp = new Dictionary<byte, string>();
            foreach (KeyValuePair<byte, string> kv in Models.Cache.Order_OrderStatus)
            {
                if (kv.Key == (byte)Models.OrderStatus.PishFactor)
                    temp.Add(kv.Key, kv.Value);
            }
            ViewBag.OrderStatus = temp;

            return View("OrderDetail", obj);
        }

        [CustomAuthorize(OPERATIONS.Order_Detail)]
        public ActionResult OrderDetail(string OrderId)
        {
            Order_Order order = OrderManager.Order_Order_Get(int.Parse(OrderId));

            ViewBag.CustomerName = AccountManager.Account_User_Get(order.CustomerId).FullName;
            ViewBag.CabinPanelName = order.Order_Cabin.Count > 0 ? InformationManager.Cabin_Panel_Get(order.Order_Cabin.ElementAt(0).CabinPanelId).Name : "";
            ViewBag.CabinPushButtonName = order.Order_Cabin.Count > 0 ? InformationManager.Cabin_PushButton_Get(order.Order_Cabin.ElementAt(0).PushButtonId).Name : "";
            ViewBag.CabinMonitorName = order.Order_Cabin.Count > 0 ? InformationManager.Cabin_Monitor_Get(order.Order_Cabin.ElementAt(0).MonitorId).Name : "";
            ViewBag.CabinSurfaceMetalName = order.Order_Cabin.Count > 0 ? InformationManager.Cabin_SurfaceMetal_Get(order.Order_Cabin.ElementAt(0).SurfaceMetalId).Name : "";
            ViewBag.CabinSurfaceMetalName2 = order.Order_Cabin.Count > 0 && order.Order_Cabin.ElementAt(0).SurfaceMetalId2.HasValue ? InformationManager.Cabin_SurfaceMetal_Get(order.Order_Cabin.ElementAt(0).SurfaceMetalId2.Value).Name : "";
            ViewBag.HallPanelName = order.Order_Hall.Count > 0 ? InformationManager.Hall_Panel_Get(order.Order_Hall.ElementAt(0).HallPanelId).Name : "";
            ViewBag.HallPushButtonName = order.Order_Hall.Count > 0 ? InformationManager.Cabin_PushButton_Get(order.Order_Hall.ElementAt(0).PushButtonId).Name : "";
            ViewBag.HallMonitorName = order.Order_Hall.Count > 0 ? InformationManager.Cabin_Monitor_Get(order.Order_Hall.ElementAt(0).MonitorId).Name : "";
            ViewBag.HallSurfaceMetalName = order.Order_Hall.Count > 0 ? InformationManager.Hall_SurfaceMetal_Get(order.Order_Hall.ElementAt(0).SurfaceMetalId).Name : "";
            ViewBag.DoorTopPanelName = order.Order_DoorTop.Count > 0 ? InformationManager.DoorTop_Panel_Get(order.Order_DoorTop.ElementAt(0).DoorTopPanelId).Name : "";
            ViewBag.DoorTopMonitorName = order.Order_DoorTop.Count > 0 ? InformationManager.Cabin_Monitor_Get(order.Order_DoorTop.ElementAt(0).MonitorId).Name : "";
            ViewBag.DoorTopSurfaceMetalName = order.Order_DoorTop.Count > 0 ? InformationManager.SurfaceMetal_Get(order.Order_DoorTop.ElementAt(0).SurfaceMetalId).Name : "";

            Dictionary<byte, string> temp = new Dictionary<byte, string>();
            if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Order_Edit_Factor) && (order.StatusId < (byte)Models.OrderStatus.DarkhasteTolid))// || order.StatusId >= (byte)Models.OrderStatus.AmadeTahvil))
            {
                foreach (KeyValuePair<byte, string> kv in Models.Cache.Order_OrderStatus)
                {
                    if (kv.Key == order.StatusId || kv.Key == order.StatusId + 1 || kv.Key == (byte)Models.OrderStatus.Raked)
                        temp.Add(kv.Key, kv.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<byte, string> kv in Models.Cache.Order_OrderStatus)
                {
                    if (kv.Key == order.StatusId)
                        temp.Add(kv.Key, kv.Value);
                }
            }
            ViewBag.OrderStatus = temp;

            return View("OrderDetail", order);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Order_Add)]
        public ActionResult OrderDetail(Order_Order model, string DeliveryCost, string submit)
        {
            bool isNew = false;
            Order_Order entity = OrderManager.Order_Order_Get(model.Id);
            if (entity.StatusId > model.StatusId)
                return RedirectToAction("AccessDenied", "Home");
            if (model.StatusId > (byte)OrderStatus.PishFactor)
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Order_Edit_Factor))
                    return RedirectToAction("AccessDenied", "Home");
            }

            if (model.Id == 0)
            {
                model.AccepterId = Session["PantaUser"] == null ? 0 : (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id;
                isNew = true;
            }
            model.DeliveryCost = string.IsNullOrEmpty(DeliveryCost) ? 0 : double.Parse(DeliveryCost.Replace(",", ""));
            model.StoreId = StoreId;
            Order_Order x = OrderManager.Order_Order_Edit(model, submit);
            LogManager.Log_Logs_Add((int)DB_Table.Order_Order, x.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, isNew ? (int)LogActivity.Add : (int)LogActivity.Edit, x.ToString(), x.Cost);
            return RedirectToAction("OrderDetail", new { OrderId = x.Id });
        }

        [CustomAuthorize(OPERATIONS.Order_Search)]
        public ActionResult SearchOrder(int? docNumber, byte? orderStatusId, byte? orderTradeTypeId, int? customerId, string Customer, string orderDateFrom, string orderDateTo, string factorDateFrom, string factorDateTo, string projectName)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (orderStatusId.HasValue) paramlist += ("orderStatusId=" + orderStatusId.Value.ToString() + "&");
            if (orderTradeTypeId.HasValue) paramlist += ("orderTradeTypeId=" + orderTradeTypeId.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(orderDateFrom)) paramlist += ("orderDateFrom=" + orderDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(orderDateTo)) paramlist += ("orderDateTo=" + orderDateTo + "&");
            if (!string.IsNullOrWhiteSpace(factorDateFrom)) paramlist += ("factorDateFrom=" + factorDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(factorDateTo)) paramlist += ("factorDateTo=" + factorDateTo + "&");
            if (!string.IsNullOrWhiteSpace(projectName)) paramlist += ("projectName=" + projectName + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Order/OrderList" + paramlist));
        }

        [CustomAuthorize(OPERATIONS.Order_Print)]
        public ActionResult Print(string doc, int id)
        {
            if (doc == "order")
            {
                Order_Order order = OrderManager.Order_Order_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ فاکتور");
                return View("PrintOrder", order);
            }
            else if (doc == "bill")
            {
                Order_Order order = OrderManager.Order_Order_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ صورتحساب فروش");
                return View("PrintBill", order);
            }
            else
                return View("Error");
        }

        [CustomAuthorize(OPERATIONS.Order_Delete)]
        public ActionResult OrderDelete(int orderId)
        {
            /*try
            {
                List<Order_Order> OrderList = OrderManager.Order_Order_Search(null, (byte)OrderStatus.PishFactor, null,
                    null, (DateTime?)PersianDateTime.Parse("1399/07/01".Replace('-', '/')).ToDateTime(),
                    null, null,
                    null, null);
                for (int i = 0; i < OrderList.Count; i++)
                {
                    OrderManager.Order_Order_Delete(OrderList.ElementAt(i).Id);
                }
                return RedirectToAction("OrderList", "Order");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }*/

            try
            {
                Order_Order order = OrderManager.Order_Order_Get(orderId);
                OrderManager.Order_Order_Delete(orderId);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "");
                return RedirectToAction("OrderList", "Order");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [CustomAuthorize(OPERATIONS.Order_ChangeStatus)]
        public ActionResult OrderChangeStatus(int orderId, byte newStatusId)
        {
            try
            {
                Order_Order order = OrderManager.Order_Order_ChangeStatus(orderId, (OrderStatus)newStatusId, true);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.ChangeStatus, order.ToString(), order.Cost);
                return RedirectToAction("OrderDetail", new { OrderId = orderId });
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
    }
}
