using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;

namespace BaseSite.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseSiteController
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
        public async Task<IActionResult> Index(string UserName, string Password, string returnurl)
        {
            Account_Users user = AccountManager.Login(UserName, Password, HttpContext.Connection.RemoteIpAddress?.ToString());

            if (user.Id == (new Account_Users()).Id)
            {
                TempData["FailLogin"] = "FailLogin";
                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, user.Id, 0, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.LoginFailed, string.Format("نام کاربری وارد شده: {0}", UserName));
                return RedirectToAction("Index");
            }
            else
            {
                List<OPERATIONS> oprs = user.Account_UserPost.Count > 0 ? AccountManager.Account_Operation_Get((AccountRole)user.Account_UserPost.First().PostId) : new List<OPERATIONS>();
                await AuthenticationClaims.SignInAsync(HttpContext, user);

                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, user.Id, user.Id, HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Login, "");

                if (!string.IsNullOrEmpty(returnurl) && Url.IsLocalUrl(returnurl))
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
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "GuideBook", "guidebook.html"), "text/html");
        }
    }
}
