using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using BaseSite.Models.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseSite.Controllers
{
    public class SaleController : Controller
    {
        private static byte StoreId = 1; //Centeral office

        [CustomAuthorize(OPERATIONS.Sale)]
        public ActionResult SaleList(int? docNumber, byte? orderStatusId, byte? orderTradeTypeId, int? customerId, string orderDateFrom, string orderDateTo, string factorDateFrom, string factorDateTo)
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

            List<Sale_Sale> saleList = SaleManager.Sale_Sale_Search(StoreId, docNumber, orderStatusId, orderTradeTypeId, customerId,
                string.IsNullOrEmpty(orderDateFrom) ? null : (DateTime?)PersianDateTime.Parse(orderDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(orderDateTo) ? null : (DateTime?)PersianDateTime.Parse(orderDateTo.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(factorDateFrom) ? null : (DateTime?)PersianDateTime.Parse(factorDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(factorDateTo) ? null : (DateTime?)PersianDateTime.Parse(factorDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = saleList.Count();
            return View(saleList);
        }

        [CustomAuthorize(OPERATIONS.Sale_Add)]
        public ActionResult AddSale()
        {
            Sale_Sale obj = SaleManager.Sale_Sale_Add();
            obj.StoreId = StoreId;
            obj.DateOrder = DateTime.Now;
            obj.DateDelivery = null;
            obj.DateFactor = null; // DateTime.Now.AddDays(5);
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

            return View("SaleDetail", obj);
        }

        [CustomAuthorize(OPERATIONS.Sale_Detail)]
        public ActionResult SaleDetail(string saleId)
        {
            Sale_Sale sale = SaleManager.Sale_Sale_Get(int.Parse(saleId));
            ViewBag.CustomerName = AccountManager.Account_User_Get(sale.CustomerId).FullName;

            Dictionary<byte, string> temp = new Dictionary<byte, string>();
            if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Sale_Edit) && (sale.StatusId < (byte)Models.OrderStatus.MojavezKhorooj))
            {
                foreach (KeyValuePair<byte, string> kv in Models.Cache.Order_OrderStatus)
                {
                    if (kv.Key == (byte)Models.OrderStatus.PishFactor || kv.Key == (byte)Models.OrderStatus.DarDasteEghdam || kv.Key == (byte)Models.OrderStatus.MojavezKhorooj || kv.Key == (byte)Models.OrderStatus.Raked)
                        if (kv.Key >= (byte)sale.StatusId)
                            temp.Add(kv.Key, kv.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<byte, string> kv in Models.Cache.Order_OrderStatus)
                {
                    if (kv.Key == sale.StatusId)
                        temp.Add(kv.Key, kv.Value);
                }
            }
            ViewBag.OrderStatus = temp;

            return View("SaleDetail", sale);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Sale_Add)]
        public ActionResult SaleDetail(Sale_Sale model, string DeliveryCost, string Discount, string submit)
        {
            Sale_Sale entity = SaleManager.Sale_Sale_Get(model.Id);
            if (entity.StatusId > model.StatusId)
                return RedirectToAction("AccessDenied", "Home");
            if (model.StatusId > (byte)OrderStatus.PishFactor)
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Sale_Edit))
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
            model.StoreId = StoreId;
            Sale_Sale x = SaleManager.Sale_Sale_Edit(model, submit);
            LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, x.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, isNew ? (int)LogActivity.Add : (int)LogActivity.Edit, x.ToString(), x.Cost);
            return RedirectToAction("SaleDetail", new { saleId = x.Id });
        }

        [CustomAuthorize(OPERATIONS.Sale_Search)]
        public ActionResult SearchSale(int? docNumber, byte? orderStatusId, byte? orderTradeTypeId, int? customerId, string Customer, string orderDateFrom, string orderDateTo, string factorDateFrom, string factorDateTo)
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

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Sale/SaleList" + paramlist));
        }

        [CustomAuthorize(OPERATIONS.Sale_Print)]
        public ActionResult Print(string doc, int id)
        {
            if (doc == "sale")
            {
                Sale_Sale sale = SaleManager.Sale_Sale_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, sale.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ فاکتور");
                return View("PrintSale", sale);
            }
            else if (doc == "bill")
            {
                Sale_Sale sale = SaleManager.Sale_Sale_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, sale.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ صورتحساب فروش");
                return View("PrintBill", sale);
            }
            else
                return View("Error");
        }

        [CustomAuthorize(OPERATIONS.Sale_Delete)]
        public ActionResult SaleDelete(int saleId)
        {
            /*try
            {
                List<Sale_Sale> SaleList = SaleManager.Sale_Sale_Search(null, (byte)OrderStatus.PishFactor, null,
                    null, (DateTime?)PersianDateTime.Parse("1399/07/01".Replace('-', '/')).ToDateTime(),
                    null, null);
                for (int i = 0; i < SaleList.Count; i++)
                {
                    SaleManager.Sale_Sale_Delete(SaleList.ElementAt(i).Id);
                }
                return RedirectToAction("SaleList", "Sale");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }*/

            try
            {
                Sale_Sale sale = SaleManager.Sale_Sale_Get(saleId);
                SaleManager.Sale_Sale_Delete(saleId);
                LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, sale.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "");
                return RedirectToAction("SaleList", "Sale");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [CustomAuthorize(OPERATIONS.Sale_ChangeStatus)]
        public ActionResult SaleChangeStatus(int saleId, byte newStatusId)
        {
            try
            {
                Sale_Sale sale = SaleManager.Sale_Sale_ChangeStatus(saleId, (OrderStatus)newStatusId, true);
                LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, sale.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.ChangeStatus, sale.ToString(), sale.Cost);
                return RedirectToAction("SaleDetail", new { saleId = saleId });
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
    }
}
