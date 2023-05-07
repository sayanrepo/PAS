using BaseSite.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseSite.Models.Order;
using BaseSite.Models.DBModel;
using BaseSite.Models.Information;
using System.Data.Entity;
using BaseSite.Models;
using BaseSite.Models.Log;

namespace BaseSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        public ActionResult Index(string returnurl)
        {
            if (TempData["FailLogin"] != null && TempData["FailLogin"].ToString() != string.Empty)
            {
                ViewBag.FailLogin = TempData["FailLogin"].ToString();
            }
            ViewBag.ReturnUrl = returnurl;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Password, string returnurl)
        {
            Account_Users user = AccountManager.Login(UserName, Password, Request.UserHostAddress);

            if (user.Id == (new Account_Users()).Id)
            {
                TempData["FailLogin"] = "FailLogin";
                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, user.Id, 0, Request.UserHostAddress, (int)LogActivity.LoginFailed, string.Format("نام کاربری وارد شده: {0}", UserName));
                return RedirectToAction("Index");
            }
            else
            {
                Session["PantaUser"] = user;
                List<OPERATIONS> oprs = user.Account_UserPost.Count > 0 ? AccountManager.Account_Operation_Get((AccountRole)user.Account_UserPost.First().PostId) : new List<OPERATIONS>();
                Session["UserOperations"] = oprs;

                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, user.Id, user.Id, Request.UserHostAddress, (int)LogActivity.Login, "");

                if (!string.IsNullOrEmpty(returnurl))
                    return Redirect(returnurl);
                else
                {
                    if (oprs.Contains(OPERATIONS.Order))
                        return RedirectToAction("OrderList", "Order");
                    else if (oprs.Contains(OPERATIONS.Sale))
                        return RedirectToAction("SaleList", "Sale");
                    else if (oprs.Contains(OPERATIONS.Store))
                        return RedirectToAction("SaleList", "Store");
                    else if (oprs.Contains(OPERATIONS.Payment))
                        return RedirectToAction("PaymentList", "Payment");
                    else if (oprs.Contains(OPERATIONS.Cartable))
                        return RedirectToAction("CartableList", "Cartable");
                    else if (oprs.Contains(OPERATIONS.Plan))
                        return RedirectToAction("PlanList", "Plan");
                    else if (oprs.Contains(OPERATIONS.Product))
                        return RedirectToAction("ProductList", "Product");
                    else if (oprs.Contains(OPERATIONS.Process))
                        return RedirectToAction("Index", "Process");
                    else if (oprs.Contains(OPERATIONS.Setting))
                        return RedirectToAction("Index", "Information");
                    else
                        return RedirectToAction("AccessDenied", "Home");
                }
            }
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Guide()
        {
            var staticPageToRender = new FilePathResult("~/GuideBook/guidebook.html", "text/html");
            return staticPageToRender;
        }
    }
}
