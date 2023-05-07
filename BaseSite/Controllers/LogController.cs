using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseSite.Controllers
{
    public class LogController : Controller
    {
        [CustomAuthorize(OPERATIONS.Logs_Logs)]
        public ActionResult LogList(int? docNumber, int? tableId, int? customerId, string eventTimeFrom, string eventTimeTo)
        {
            //customerId = (int?)Session["customerId"];

            ViewBag.docNumber = docNumber;
            ViewBag.tableId = tableId;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.eventTimeFrom = eventTimeFrom;
            ViewBag.eventTimeTo = eventTimeTo;

            List<Log_Logs> logList = LogManager.Log_Logs_Search(docNumber, tableId, customerId,
                string.IsNullOrEmpty(eventTimeFrom) ? null : (DateTime?)PersianDateTime.Parse(eventTimeFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(eventTimeTo) ? null : (DateTime?)PersianDateTime.Parse(eventTimeTo.Replace('-', '/')).ToDateTime().AddHours(24).AddSeconds(-1));

            ViewBag.RowCount = logList.Count();
            return View(logList);
        }

        [CustomAuthorize(OPERATIONS.Logs_Detail)]
        public ActionResult LogDetail(string logId)
        {
            Log_Logs log = LogManager.Log_Logs_Get(int.Parse(logId));
            ViewBag.CustomerName = AccountManager.Account_User_Get(log.UserId).FullName;

            return View("LogDetail", log);
        }

        [CustomAuthorize(OPERATIONS.Logs_Detail)]
        public JsonResult GetLogDetail(int logId)
        {
            Log_Logs log = LogManager.Log_Logs_Get(logId);

            var res = new
                       {
                           shEventTime = log.ShEventTime,
                           userFullName = log.Account_Users.FullName,
                           log.IPAddress,
                           entityTable = log.BaseSystem_Tables.Label,
                           entityId = log.EntityId,
                           activity = log.Log_LogActivity.Name,
                           log.Description,
                           log.LogData1
                       };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(OPERATIONS.Logs_Logs)]
        public ActionResult SearchLog(int? docNumber, int? tableId, int? customerId, string Customer, string eventTimeFrom, string eventTimeTo)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (tableId.HasValue) paramlist += ("tableId=" + tableId.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(eventTimeFrom)) paramlist += ("eventTimeFrom=" + eventTimeFrom + "&");
            if (!string.IsNullOrWhiteSpace(eventTimeTo)) paramlist += ("eventTimeTo=" + eventTimeTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Log/LogList" + paramlist));
        }
    }
}
