using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseSite.Controllers
{
    public class CartableController : Controller
    {
        [CustomAuthorize(OPERATIONS.Cartable)]
        public ActionResult CartableList(int? docNumber, int? customerId, string orderDateFrom, string orderDateTo, string deliveryDateFrom, string deliveryDateTo)
        {
            customerId = (int?)Session["customerId"];

            ViewBag.docNumber = docNumber;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.orderDateFrom = orderDateFrom;
            ViewBag.orderDateTo = orderDateTo;
            ViewBag.deliveryDateFrom = deliveryDateFrom;
            ViewBag.deliveryDateTo = deliveryDateTo;

            List<Order_Order> orderList = OrderManager.Order_Order_Search(docNumber, (byte)OrderStatus.DarkhasteTolid, null, customerId,
                string.IsNullOrEmpty(orderDateFrom) ? null : (DateTime?)PersianDateTime.Parse(orderDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(orderDateTo) ? null : (DateTime?)PersianDateTime.Parse(orderDateTo.Replace('-', '/')).ToDateTime(),
                null, null,
                string.IsNullOrEmpty(deliveryDateFrom) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(deliveryDateTo) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = orderList.Count();
            return View(orderList);
        }

        [CustomAuthorize(OPERATIONS.Cartable_Detail)]
        public ActionResult CartableDetail(int OrderId)
        {
            Order_Order order = OrderManager.Order_Order_Get(OrderId);
            return View("~/Views/Plan/PlanDetail.cshtml", order);
        }

        [CustomAuthorize(OPERATIONS.Cartable_Search)]
        public ActionResult SearchCartable(int? docNumber, int? customerId, string Customer, string orderDateFrom, string orderDateTo, string deliveryDateFrom, string deliveryDateTo)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(orderDateFrom)) paramlist += ("orderDateFrom=" + orderDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(orderDateTo)) paramlist += ("orderDateTo=" + orderDateTo + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateFrom)) paramlist += ("deliveryDateFrom=" + deliveryDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateTo)) paramlist += ("deliveryDateTo=" + deliveryDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Cartable/CartableList" + paramlist));
        }

    }
}
