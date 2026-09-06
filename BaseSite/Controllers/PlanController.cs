using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using BaseSite.Models.Order;

using Microsoft.AspNetCore.Mvc;

namespace BaseSite.Controllers
{
    public class PlanController : BaseSiteController
    {
        [Authorize(Roles = nameof(OPERATIONS.Plan))]
        public ActionResult PlanList(int? docNumber, byte? orderStatusId, int? customerId, string orderDateFrom, string orderDateTo, string deliveryDateFrom, string deliveryDateTo)
        {
            customerId = (int?)Session["customerId"];

            ViewBag.docNumber = docNumber;
            ViewBag.orderStatus = orderStatusId;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.orderDateFrom = orderDateFrom;
            ViewBag.orderDateTo = orderDateTo;
            ViewBag.deliveryDateFrom = deliveryDateFrom;
            ViewBag.deliveryDateTo = deliveryDateTo;

            List<Models.Panel> planList = OrderManager.Order_Plan_Search(docNumber, orderStatusId, customerId,
                string.IsNullOrEmpty(orderDateFrom) ? null : (DateTime?)PersianDateTime.Parse(orderDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(orderDateTo) ? null : (DateTime?)PersianDateTime.Parse(orderDateTo.Replace('-', '/')).ToDateTime(),
                null, null,
                string.IsNullOrEmpty(deliveryDateFrom) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(deliveryDateTo) ? null : (DateTime?)PersianDateTime.Parse(deliveryDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = planList.Count();
            return View(planList);
        }

        [Authorize(Roles = nameof(OPERATIONS.Plan_Detail))]
        public ActionResult PlanDetail(int OrderId)
        {
            Order_Order order = OrderManager.Order_Order_Get(OrderId);
            return View("PlanDetail", order);
        }

        [HttpPost]
        [Authorize(Roles = nameof(OPERATIONS.Plan_Detail))]
        public ActionResult PlanDetail(int Id, string submit)
        {
            if (submit == "DarJaryaneTolid")
            {
                if (User.IsInRole(nameof(OPERATIONS.Plan_StartCommand)))
                {
                    Order_Order order = OrderManager.Order_Order_ChangeStatus(Id, Models.OrderStatus.DarJaryaneTolid);
                    LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, User.GetUserId(), HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.ChangeStatus, "تغییر وضعیت به: " + order.Order_Status.Name, order.Cost);
                }
                else
                    return RedirectToAction("AccessDenied", "Home");
            }
            //else if (submit == "AmadeTahvil")
            //{
            //    if (User.IsInRole(nameof(OPERATIONS.Plan_FinishCommand)))
            //        OrderManager.Order_Order_ChangeStatus(Id, Models.OrderStatus.AmadeTahvil);
            //    else
            //        return RedirectToAction("AccessDenied", "Home");
            //}
            //return RedirectToAction("PlanDetail", new { OrderId = Id });
            return RedirectToAction("CartableList", "Cartable");
        }

        [Authorize(Roles = nameof(OPERATIONS.Plan_Search))]
        public ActionResult SearchPlan(int? docNumber, byte? orderStatusId, int? customerId, string Customer, string orderDateFrom, string orderDateTo, string deliveryDateFrom, string deliveryDateTo)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (orderStatusId.HasValue) paramlist += ("orderStatusId=" + orderStatusId.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(orderDateFrom)) paramlist += ("orderDateFrom=" + orderDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(orderDateTo)) paramlist += ("orderDateTo=" + orderDateTo + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateFrom)) paramlist += ("deliveryDateFrom=" + deliveryDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(deliveryDateTo)) paramlist += ("deliveryDateTo=" + deliveryDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Plan/PlanList" + paramlist));
        }

        [Authorize(Roles = nameof(OPERATIONS.Plan_Print))]
        public ActionResult Print(string doc, int id)
        {
            List<int> printed = new List<int>();
            if (Request.Cookies["printed"] != null)
            {
                var list = Request.Cookies["Printed"].Split(',').ToList();
                int tmp;
                foreach (var item in list)
                {
                    if (int.TryParse(item, out tmp))
                        printed.Add(tmp);
                }
            }

            if (doc == "order")
            {
                Order_Order order = OrderManager.Order_Order_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, User.GetUserId(), HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print, "چاپ سند بزرگ");
                printed.Add(id);
                Response.Cookies.Append("printed", string.Join(",", printed));
                return View("PrintOrder", order);
            }
            else if (doc == "cabin")
            {
                Order_Order order = OrderManager.Order_Order_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, User.GetUserId(), HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print, "چاپ سند کوچک پنل داخل کابین");
                printed.Add(id);
                Response.Cookies.Append("printed", string.Join(",", printed));
                return View("PrintCabin", order);
            }
            else if (doc == "hall")
            {
                Order_Order order = OrderManager.Order_Order_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, User.GetUserId(), HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print, "چاپ سند کوچک پنل طبقات");
                printed.Add(id);
                Response.Cookies.Append("printed", string.Join(",", printed));
                return View("PrintHall", order);
            }
            else if (doc == "doortop")
            {
                Order_Order order = OrderManager.Order_Order_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, User.GetUserId(), HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print, "چاپ سند کوچک پنل سردرب");
                printed.Add(id);
                Response.Cookies.Append("printed", string.Join(",", printed));
                return View("PrintDoorTop", order);
            }
            else
                return View("Error");
        }
    }
}
