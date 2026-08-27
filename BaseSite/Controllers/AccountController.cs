using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using BaseSite.Models;
using BaseSite.Models.DBModel;
using BaseSite.Models.Account;
using BaseSite.Models.Information;
using BaseSite.Models.Log;


namespace BaseSite.Controllers
{
    public class AccountController : BaseSiteController
    {
        //######################################################### Profile #################################################################
        public ActionResult LogOut()
        {
            LogManager.Log_Logs_Add((int)DB_Table.Account_Users, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.LogOut, " ");
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string newUserName, string currentPassword, string newPassword)
        {
            if (CustomAuthorizeAttribute.getCurrentUser(HttpContext) == null) return RedirectToAction("Index", "Home");
            ViewBag.Msg = AccountManager.Account_User_ChangePassword(CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, newUserName, currentPassword, newPassword);
            Session["PantaUser"] = AccountManager.Account_User_Get(CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id);
            LogManager.Log_Logs_Add((int)DB_Table.Account_Users, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, "تغییر رمز عبور حساب کاربری خود");
            return View("Message");
        }

        public ActionResult ChangeImage()
        {
            if (CustomAuthorizeAttribute.getCurrentUser(HttpContext) == null) return RedirectToAction("Index", "Home");
            ViewBag.Image = CustomAuthorizeAttribute.getCurrentUser(HttpContext).ImagePath == null ? "profile.png" : CustomAuthorizeAttribute.getCurrentUser(HttpContext).ImagePath;
            return View();
        }

        [HttpPost]
        public ActionResult ChangeImage(IFormFile files)
        {
            if (CustomAuthorizeAttribute.getCurrentUser(HttpContext) == null) return RedirectToAction("Index", "Home");
            if (files != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "System", CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id + "_" + files.FileName);
                using var stream = System.IO.File.Create(filePath);
                files.CopyTo(stream);
                ViewBag.Msg = AccountManager.Account_User_ChangeImage(CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id + "_" + files.FileName);
                Session["PantaUser"] = AccountManager.Account_User_Get(CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id);
                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, "تغییر موفق تصویر پروفایل خود");
            }
            else
            {
                ViewBag.Msg = "ذخیره فایل با مشکل مواجه شده است. لطفا فایل خود را بازبینی نموده و مجددا تلاش نمایید";
                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, "خطا در تغییر تصویر پروفایل خود - ذخیره فایل با مشکل مواجه شده است");
            }

            return View("Message");
        }


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

            return Redirect(Url.Content("~/Account/Persons" + paramlist));
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
                TempData["BackUrl"] = Request.Headers.Referer.ToString();
            }
            Account_Users list = AccountManager.Account_User_Get(PersonId);

            //LogManager.Log_Logs_Add((int)DB_Table.Account_Users, list.Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.View, string.Format("مشاهده مشخصات شخص - {0}", list.FullName));
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
                if (!CustomAuthorizeAttribute.isAuthorize(HttpContext, OPERATIONS.Setting_Persons_Add))
                    return RedirectToAction("AccessDenied", "Home");
                
                    user.RegistrarId = Session["PantaUser"] == null ? 0 : (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id;
                    user.RegistrationDate = DateTime.Now;
            }
            if (user.Id > 0)
            {
                if (!CustomAuthorizeAttribute.isAuthorize(HttpContext, OPERATIONS.Setting_Persons_Edit))
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

                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, usrId, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), newUser ? (int)LogActivity.Add : (int)LogActivity.Edit, user.ToString());

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

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Persons_AssignAccess)]
        public ActionResult EditAccess(int Id, byte Status, int PostId)
        {
            AccountManager.Account_User_EditAccess(Id, Status, PostId);
            Account_Users user = AccountManager.Account_User_Get(Id);

            string postname = Cache.Posts.Where(m => m.Id == PostId).FirstOrDefault().Name;
            LogManager.Log_Logs_Add((int)DB_Table.Account_Users, Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, string.Format("تغییر دسترسی {0} - دسترسی جدید: {1} - مسئولیت جدید: {2}", user.FullName, Cache.UserStatuses[(UserStatus)Status], postname));

            return RedirectToAction("PersonDetail", new { PersonId = Id });
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Persons_AssignUserName)]
        public ActionResult AssignUserName(int Id, string UserName, string Password)
        {
            try
            {
                AccountManager.Account_User_AssignUserName(Id, UserName, Password);
                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, Id, CustomAuthorizeAttribute.getCurrentUser(HttpContext).Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Edit, string.Format("اختصاص نام کاربری به {0}", AccountManager.Account_User_Get(Id).FullName));
            }
            catch (Exception ex)
            {
                TempData["Result"] = "error";
                TempData["ResultMessge"] = ex.Message;
            }
            return RedirectToAction("PersonDetail", new { PersonId = Id });
        }
    }
}
