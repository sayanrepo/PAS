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
    public class ProductController : Controller
    {
        [CustomAuthorize(OPERATIONS.Product)]
        public ActionResult ProductList(int? docNumber, byte? productStatusId, int? customerId, string orderDateFrom, string orderDateTo, string deliveryDateFrom, string deliveryDateTo)
        {
            customerId = (int?)Session["customerId"];

            ViewBag.docNumber = docNumber;
            ViewBag.productStatus = productStatusId;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.orderDateFrom = orderDateFrom;
            ViewBag.orderDateTo = orderDateTo;
            ViewBag.deliveryDateFrom = deliveryDateFrom;
            ViewBag.deliveryDateTo = deliveryDateTo;

            List<Models.Panel> productList = OrderManager.Order_Product_Search(docNumber, productStatusId, customerId,
                string.IsNullOrEmpty(orderDateFrom) ? null : (DateTime?)PersianDateTime.Parse(orderDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(orderDateTo) ? null : (DateTime?)PersianDateTime.Parse(orderDateTo.Replace('-', '/')).ToDateTime(),
                null, null,
                string.IsNullOrEmpty(deliveryDateFrom) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(deliveryDateTo) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = productList.Count();
            return View(productList);
        }

        [CustomAuthorize(OPERATIONS.Product_Detail)]
        public ActionResult ProductDetail(int OrderId)
        {
            Order_Order order = OrderManager.Order_Order_Get(OrderId);
            return View("~/Views/Plan/PlanDetail.cshtml", order);
        }

        [CustomAuthorize(OPERATIONS.Product_Search)]
        public ActionResult SearchProduct(int? docNumber, byte? productStatusId, int? customerId, string Customer, string orderDateFrom, string orderDateTo, string deliveryDateFrom, string deliveryDateTo)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (productStatusId.HasValue) paramlist += ("productStatusId=" + productStatusId.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(orderDateFrom)) paramlist += ("orderDateFrom=" + orderDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(orderDateTo)) paramlist += ("orderDateTo=" + orderDateTo + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateFrom)) paramlist += ("deliveryDateFrom=" + deliveryDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateTo)) paramlist += ("deliveryDateTo=" + deliveryDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Product/ProductList" + paramlist));
        }
    }
}
