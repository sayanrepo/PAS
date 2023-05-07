using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Service;
using BaseSite.Models.Log;

namespace BaseSite.Controllers
{
    public class ServiceController : Controller
    {
        [CustomAuthorize(OPERATIONS.Service)]
        public ActionResult ServiceList(int? docNumber, byte? orderStatusId, int? customerId, string orderDateFrom, string orderDateTo, string factorDateFrom, string factorDateTo)
        {
            customerId = (int?)Session["customerId"];

            ViewBag.docNumber = docNumber;
            ViewBag.orderStatus = orderStatusId;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.orderDateFrom = orderDateFrom;
            ViewBag.orderDateTo = orderDateTo;
            ViewBag.factorDateFrom = factorDateFrom;
            ViewBag.factorDateTo = factorDateTo;

            List<Service_Service> serviceList = ServiceManager.Service_Service_Search(docNumber, orderStatusId, customerId,
                string.IsNullOrEmpty(orderDateFrom) ? null : (DateTime?)PersianDateTime.Parse(orderDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(orderDateTo) ? null : (DateTime?)PersianDateTime.Parse(orderDateTo.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(factorDateFrom) ? null : (DateTime?)PersianDateTime.Parse(factorDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(factorDateTo) ? null : (DateTime?)PersianDateTime.Parse(factorDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = serviceList.Count();
            return View(serviceList);
        }

        [CustomAuthorize(OPERATIONS.Service_Add)]
        public ActionResult AddService()
        {
            Service_Service obj = ServiceManager.Service_Service_Add();
            obj.DateOrder = DateTime.Now;
            //obj.DateDelivery = null;
            //obj.DateFactor = null; // DateTime.Now.AddDays(5);

            obj.DateDelivery = DateTime.Now;
            obj.DateFactor = DateTime.Now.AddDays(5);
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

            return View("ServiceDetail", obj);
        }

        [CustomAuthorize(OPERATIONS.Service_Detail)]
        public ActionResult ServiceDetail(string serviceId)
        {
            Service_Service service = ServiceManager.Service_Service_Get(int.Parse(serviceId));
            ViewBag.CustomerName = AccountManager.Account_User_Get(service.CustomerId).FullName;

            Dictionary<byte, string> temp = new Dictionary<byte, string>();
            if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Service_Edit) && (service.StatusId < (byte)Models.OrderStatus.TahvilShode))
            {
                foreach (KeyValuePair<byte, string> kv in Models.Cache.Order_OrderStatus)
                {
                    //if (kv.Key == service.StatusId || kv.Key == service.StatusId + 1 || kv.Key == (byte)Models.OrderStatus.Raked)
                    if (kv.Key == (byte)Models.OrderStatus.PishFactor || kv.Key == (byte)Models.OrderStatus.TahvilShode || kv.Key == (byte)Models.OrderStatus.Raked)
                        temp.Add(kv.Key, kv.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<byte, string> kv in Models.Cache.Order_OrderStatus)
                {
                    if (kv.Key == service.StatusId)
                        temp.Add(kv.Key, kv.Value);
                }
            }
            ViewBag.OrderStatus = temp;

            return View("ServiceDetail", service);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Service_Add)]
        public ActionResult ServiceDetail(Service_Service model, string DeliveryCost, string Discount, string submit)
        {
            Service_Service entity = ServiceManager.Service_Service_Get(model.Id);
            if (entity.StatusId > model.StatusId)
                return RedirectToAction("AccessDenied", "Home");
            if (model.StatusId > (byte)OrderStatus.PishFactor)
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Service_Edit))
                    return RedirectToAction("AccessDenied", "Home");
            }

            bool isNew = false;
            if (model.Id == 0)
            {
                isNew = true;
                model.AccepterId = Session["PantaUser"] == null ? 0 : (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id;
            }
            model.DeliveryCost = string.IsNullOrEmpty(DeliveryCost) ? 0 : double.Parse(DeliveryCost.Replace(",", ""));
            model.Discount = string.IsNullOrEmpty(Discount) ? 0 : double.Parse(Discount.Replace(",", ""));
            Service_Service x = ServiceManager.Service_Service_Edit(model, submit);
            LogManager.Log_Logs_Add((int)DB_Table.Service_Service, x.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, isNew ? (int)LogActivity.Add : (int)LogActivity.Edit, x.ToString(), x.Cost);
            return RedirectToAction("ServiceDetail", new { serviceId = x.Id });
        }

        [CustomAuthorize(OPERATIONS.Service_Search)]
        public ActionResult SearchService(int? docNumber, byte? orderStatusId, int? customerId, string Customer, string orderDateFrom, string orderDateTo, string factorDateFrom, string factorDateTo)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (orderStatusId.HasValue) paramlist += ("orderStatusId=" + orderStatusId.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(orderDateFrom)) paramlist += ("orderDateFrom=" + orderDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(orderDateTo)) paramlist += ("orderDateTo=" + orderDateTo + "&");
            if (!string.IsNullOrWhiteSpace(factorDateFrom)) paramlist += ("factorDateFrom=" + factorDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(factorDateTo)) paramlist += ("factorDateTo=" + factorDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Service/ServiceList" + paramlist));
        }

        [CustomAuthorize(OPERATIONS.Service_Print)]
        public ActionResult Print(string doc, int id)
        {
            if (doc == "service")
            {
                Service_Service service = ServiceManager.Service_Service_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Service_Service, service.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, service.ToString());
                return View("PrintService", service);
            }
            else
                return View("Error");
        }

        [CustomAuthorize(OPERATIONS.Service_Delete)]
        public ActionResult ServiceDelete(int serviceId)
        {
            /*try
            {
                List<Service_Service> ServiceList = ServiceManager.Service_Service_Search(null, (byte)OrderStatus.PishFactor, null,
                    null, (DateTime?)PersianDateTime.Parse("1399/07/01".Replace('-', '/')).ToDateTime(),
                    null, null);
                for (int i = 0; i < ServiceList.Count; i++)
                {
                    ServiceManager.Service_Service_Delete(ServiceList.ElementAt(i).Id);
                }
                return RedirectToAction("ServiceList", "Service");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }*/

            try
            {
                Service_Service service = ServiceManager.Service_Service_Get(serviceId);
                ServiceManager.Service_Service_Delete(serviceId);
                LogManager.Log_Logs_Add((int)DB_Table.Service_Service, service.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "");
                return RedirectToAction("ServiceList", "Service");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [CustomAuthorize(OPERATIONS.Service_ChangeStatus)]
        public ActionResult ServiceChangeStatus(int serviceId, byte newStatusId)
        {
            try
            {
                Service_Service service = ServiceManager.Service_Service_ChangeStatus(serviceId, (OrderStatus)newStatusId, true);
                LogManager.Log_Logs_Add((int)DB_Table.Service_Service, service.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.ChangeStatus, service.ToString(), service.Cost);
                return RedirectToAction("ServiceDetail", new { serviceId = serviceId });
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
    }
}
