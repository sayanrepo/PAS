using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseSite.Models.Account;
using BaseSite.Models;
using BaseSite.Models.Log;

namespace BaseSite.Controllers
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly OPERATIONS[] allowedoperations;
        public CustomAuthorizeAttribute(params OPERATIONS[] operations)
        {
            this.allowedoperations = operations;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            List<OPERATIONS> userOperations;

            if (HttpContext.Current.Session["UserOperations"] != null)
            {
                userOperations = (List<OPERATIONS>)HttpContext.Current.Session["UserOperations"];
                foreach (var operation in allowedoperations)
                {
                    if (userOperations.Contains(operation))
                    {
                        authorize = true;
                    }
                }
            }
            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session["PantaUser"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index?returnurl=" + HttpContext.Current.Request.Url.ToString());
                LogManager.Log_Logs_Add((int)DB_Table.Account_Users, 0, 0, HttpContext.Current.Request.UserHostAddress, (int)LogActivity.SessionTimeout, " ");
            }
            else
                //filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result = new RedirectResult("~/Home/AccessDenied");
        }

        public static bool isAuthorize(OPERATIONS operation)
        {
            List<OPERATIONS> userOperations = (List<OPERATIONS>)HttpContext.Current.Session["UserOperations"];
            return userOperations.Contains(operation);
        }

        public static Models.DBModel.Account_Users getCurrentUser()
        {
            return HttpContext.Current.Session["PantaUser"] as Models.DBModel.Account_Users;
        }
    }
}