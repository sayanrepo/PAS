using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.CRM;
using BaseSite.Models.DBModel;
using BaseSite.Models.Information;
using BaseSite.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseSite.Controllers
{
    public class CRMController : Controller
    {
        //######################################################### Person #################################################################
        [CustomAuthorize(OPERATIONS.Setting_Persons)]
        public ActionResult Persons(string Customer, byte? departmentId, byte? partnerTypeId, byte? statusId, int? postId, int? hcountryId, int? hprovinceId, int? hcityId)
        {
            if (TempData["Result"] != null && !string.IsNullOrEmpty(TempData["Result"].ToString()))
            {
                ViewBag.Result = TempData["Result"].ToString();
            }
            if (TempData["ResultMessge"] != null && !string.IsNullOrEmpty(TempData["ResultMessge"].ToString()))
            {
                ViewBag.ResultMessge = TempData["ResultMessge"].ToString();
            }

            ViewBag.CustomerName = Customer;
            ViewBag.departmentId = departmentId;
            ViewBag.partnerTypeId = partnerTypeId;
            ViewBag.statusId = statusId;
            ViewBag.postId = postId;
            ViewBag.hcountryId = hcountryId;
            ViewBag.hprovinceId = hprovinceId;
            ViewBag.hcityId = hcityId;

            List<Account_Users> UserList = AccountManager.Account_User_Search(Customer, departmentId, partnerTypeId, statusId, postId, hcountryId, hprovinceId, hcityId);
            ViewBag.RowCount = UserList.Count();
            return View(UserList);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Persons_Search)]
        public ActionResult SearchAccount(string Customer, byte? departmentId, byte? partnerTypeId, byte? statusId, int? postId, int? hcountryId, int? hprovinceId, int? hcityId)
        {
            string paramlist = "";
            if (!string.IsNullOrWhiteSpace(Customer)) paramlist += ("Customer=" + Customer + "&");
            if (departmentId.HasValue) paramlist += ("departmentId=" + departmentId.Value.ToString() + "&");
            if (partnerTypeId.HasValue) paramlist += ("partnerTypeId=" + partnerTypeId.Value.ToString() + "&");
            if (statusId.HasValue) paramlist += ("statusId=" + statusId.Value.ToString() + "&");
            if (postId.HasValue) paramlist += ("postId=" + postId.Value.ToString() + "&");
            if (hcountryId.HasValue) paramlist += ("hcountryId=" + hcountryId.Value.ToString() + "&");
            if (hprovinceId.HasValue) paramlist += ("hprovinceId=" + hprovinceId.Value.ToString() + "&");
            if (hcityId.HasValue) paramlist += ("hcityId=" + hcityId.Value.ToString() + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Crm/Persons" + paramlist));
        }

        [HttpGet]
        [CustomAuthorize(OPERATIONS.Setting_Persons_Add)]
        public ActionResult PersonAdd()
        {
            return RedirectToAction("PersonDetail", new { PersonId = 0 });
        }

        [HttpGet]
        [CustomAuthorize(OPERATIONS.Setting_Persons_Detail)]
        public ActionResult PersonDetail(int PersonId)
        {
            if (TempData["Result"] != null && !string.IsNullOrEmpty(TempData["Result"].ToString()))
            {
                ViewBag.Result = TempData["Result"].ToString();
            }
            if (TempData["ResultMessge"] != null && !string.IsNullOrEmpty(TempData["ResultMessge"].ToString()))
            {
                ViewBag.ResultMessge = TempData["ResultMessge"].ToString();
            }
            if (PersonId > 0)
            {
                TempData["BackUrl"] = Request.UrlReferrer.ToString();
            }
            Account_Users list = AccountManager.Account_User_Get(PersonId);

            //LogManager.Log_Logs_Add((int)DB_Table.Account_Users, list.Id, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.View, string.Format("مشاهده مشخصات شخص - {0}", list.FullName));
            return View(list);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Persons_Detail)]
        public ActionResult PersonDetail(Account_Users user, string HCity, string ComCity)
        {
            bool newUser = false;

            if (user.Id == 0)
            {
                newUser = true;
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Setting_Persons_Add))
                    return RedirectToAction("AccessDenied", "Home");

                user.RegistrarId = Session["PantaUser"] == null ? 0 : (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id;
                user.RegistrationDate = DateTime.Now;
            }
            if (user.Id > 0)
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Setting_Persons_Edit))
                    return RedirectToAction("AccessDenied", "Home");
            }

            try
            {
                user.CityId1 = !string.IsNullOrWhiteSpace(HCity) ? int.Parse(HCity) : (int?)null;
                user.CityId2 = !string.IsNullOrWhiteSpace(ComCity) ? int.Parse(ComCity) : (int?)null;
                user.LastName = string.IsNullOrWhiteSpace(user.LastName) ? " " : user.LastName;

                int usrId = AccountManager.Account_User_Edit(user);
                TempData["Result"] = "ok";
                TempData["ResultMessge"] = newUser ? "افزودن کاربر با موفقیت انجام شد." : "ویرایش اطلاعات با موفقیت انجام شد.";

                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, usrId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, newUser ? (int)LogActivity.Add : (int)LogActivity.Edit, user.ToString());

                if (TempData["BackUrl"] != null && !string.IsNullOrEmpty(TempData["BackUrl"].ToString()))
                    return Redirect(TempData["BackUrl"].ToString());
                else
                    return RedirectToAction("Persons");
            }
            catch
            {
                TempData["Result"] = "error";
                TempData["ResultMessge"] = "ثبت اطلاعات با مشکل مواجه شد";
                return RedirectToAction("PersonDetail", new { PersonId = user.Id });
            }
        }


        //######################################################### Comments #################################################################
        public ActionResult InsertComment(short ttid, int tid, int parentid = 0)
        {
            if (parentid != 0)
            {
                return PartialView("CommentsInsert", new CRM_Comments()
                {
                    ParentId = parentid,
                    TrunkTableId = ttid,
                    TrunkId = tid
                });
            }
            return PartialView("CommentsInsert", new CRM_Comments()
            {
                ParentId = parentid,
                TrunkTableId = ttid,
                TrunkId = tid
            });
        }

        [HttpPost]
        public ActionResult InsertComment(short ttid, int tid, string Comment, int? ParentId)
        {
            CRM_Comments obj = new CRM_Comments();
            obj.OwnerId = BaseSite.Controllers.CustomAuthorizeAttribute.getCurrentUser().Id;
            obj.OwnerName = BaseSite.Controllers.CustomAuthorizeAttribute.getCurrentUser().FullName;
            obj.OwnerEmail = BaseSite.Controllers.CustomAuthorizeAttribute.getCurrentUser().Email;
            obj.Comment = Comment;
            obj.CreateDate = DateTime.Now;
            obj.ParentId = ParentId;
            obj.TrunkTableId = ttid;
            obj.TrunkId = tid;
            CRM_Comments res = CommentManager.CRM_Comments_Edit(obj);
            //return PartialView("CommentsShow", CommentManager.CRM_Comments_Get(ttid, tid));
            return PartialView("Comment", res);
        }


        //######################################################### Activities #################################################################
        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult Activities(byte? status, int? customerId, int? ownerId, int? assignedToId, byte? typeId, byte? priorityId, string term, string startDateFrom, string startDateTo, string endDateFrom, string endDateTo)
        {
            customerId = (int?)Session["customerId"];

            ViewBag.activityStatus = status;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.ownerId = ownerId;
            ViewBag.OwnerName = (ViewBag.ownerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.ownerId)).FullName;
            ViewBag.assignedToId = assignedToId;
            ViewBag.AssignedToName = (ViewBag.assignedToId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.assignedToId)).FullName;
            ViewBag.typeId = typeId;
            ViewBag.priorityId = priorityId;
            ViewBag.term = term;
            ViewBag.startDateFrom = startDateFrom;
            ViewBag.startDateTo = startDateTo;
            ViewBag.endDateFrom = endDateFrom;
            ViewBag.endDateTo = endDateTo;

            List<CRM_Activity> activityList = ActivityManager.CRM_Activity_Search(status, customerId, ownerId, assignedToId, typeId, priorityId, term,
                string.IsNullOrEmpty(startDateFrom) ? null : (DateTime?)PersianDateTime.Parse(startDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(startDateTo) ? null : (DateTime?)PersianDateTime.Parse(startDateTo.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(endDateFrom) ? null : (DateTime?)PersianDateTime.Parse(endDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(endDateTo) ? null : (DateTime?)PersianDateTime.Parse(endDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = activityList.Count();
            return View(activityList);
        }

        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult AddActivity(byte type = 0)
        {
            CRM_Activity obj = new CRM_Activity();

            obj.StartTime = DateTime.Now;
            obj.EndTime = DateTime.Now;
            obj.StateId = (byte)CrmActivityState.Open;
            obj.TypeId = type;
            obj.OwnerId = obj.AssignedToId = CustomAuthorizeAttribute.getCurrentUser().Id;
            obj.Account_Users = obj.Account_Users1 = CustomAuthorizeAttribute.getCurrentUser();
            if (Session["customerId"] != null)
            {
                obj.CustomerId = (int)Session["customerId"];
                ViewBag.CustomerName = AccountManager.Account_User_Get(obj.CustomerId).FullName;
            }

            return View("ActivityDetail", obj);
        }

        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult ActivityDetail(string activityId)
        {
            CRM_Activity activity = ActivityManager.CRM_Activity_Get(int.Parse(activityId));
            ViewBag.CustomerName = AccountManager.Account_User_Get(activity.CustomerId).FullName;

            return View("ActivityDetail", activity);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult ActivityDetail(CRM_Activity model, string submit)
        {
            CRM_Activity x = ActivityManager.CRM_Activity_Edit(model, submit);
            //LogManager.Log_Logs_Add((int)DB_Table.Order_Order, x.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, isNew ? (int)LogActivity.Add : (int)LogActivity.Edit, x.ToString(), x.Cost);
            return RedirectToAction("Activities");
        }

        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult SearchActivity(byte? status, int? customerId, int? ownerId, int? assignedToId, byte? typeId, byte? priorityId, string term, string startDateFrom, string startDateTo, string endDateFrom, string endDateTo)
        {
            string paramlist = "";
            if (status.HasValue) paramlist += ("status=" + status.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (ownerId.HasValue) paramlist += ("ownerId=" + ownerId.Value.ToString() + "&");
            if (assignedToId.HasValue) paramlist += ("assignedToId=" + assignedToId.Value.ToString() + "&");
            if (typeId.HasValue) paramlist += ("typeId=" + typeId.Value.ToString() + "&");
            if (priorityId.HasValue) paramlist += ("priorityId=" + priorityId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(term)) paramlist += ("term=" + term + "&");
            if (!string.IsNullOrWhiteSpace(startDateFrom)) paramlist += ("startDateFrom=" + startDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(startDateTo)) paramlist += ("startDateTo=" + startDateTo + "&");
            if (!string.IsNullOrWhiteSpace(endDateFrom)) paramlist += ("endDateFrom=" + endDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(endDateTo)) paramlist += ("endDateTo=" + endDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Crm/Activities" + paramlist));
        }

        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult ActivityDelete(int activityId)
        {
            try
            {
                CRM_Activity order = ActivityManager.CRM_Activity_Get(activityId);
                ActivityManager.CRM_Activity_Delete(activityId);
                //LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "");
                return RedirectToAction("Activities", "Crm");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }


        //######################################################### Cartable #################################################################
        public ActionResult Cartable()
        {
            List<CRM_Activity> activities = ActivityManager.CRM_Activity_Search((byte)CrmActivityState.Open, null, null, CustomAuthorizeAttribute.getCurrentUser().Id, null, null, null,
                null, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1),
                null, null);

            return View(activities);
        }

        //######################################################### Reminders #################################################################
        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult Reminders(string startDate)
        {
            ViewBag.startDate = string.IsNullOrEmpty(startDate) ? new PersianDateTime(DateTime.Today.AddDays(1)).ToString(PersianDateTimeFormat.Date) : startDate;

            List<CRM_Activity> activityList = ActivityManager.CRM_Activity_Search(CustomAuthorizeAttribute.getCurrentUser().Id,
                string.IsNullOrEmpty(startDate) ? DateTime.Today.AddDays(1) : PersianDateTime.Parse(startDate.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = activityList.Count();
            return View(activityList);
        }

        [CustomAuthorize(OPERATIONS.CRM)]
        public ActionResult SearchReminders(string startDate)
        {
            string paramlist = "";
            if (!string.IsNullOrWhiteSpace(startDate)) paramlist += ("startDate=" + startDate + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Crm/Reminders" + paramlist));
        }

        //######################################################### Messages #################################################################
        public ActionResult Messages()
        {
            return View();
        }

        //######################################################### Cartable #################################################################
        public ActionResult Notes()
        {
            return View();
        }
    }
}
