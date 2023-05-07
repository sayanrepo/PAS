using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Order;
using BaseSite.Models;

namespace BaseSite.Controllers
{
    public class ProcessController : Controller
    {
        [CustomAuthorize(OPERATIONS.Process)]
        public ActionResult Index()
        {
            ViewBag.Operators = AccountManager.Account_User_Get().Where(u => u.DepartmentId == (byte)Models.Department.Tolid).ToDictionary(u => u.Id, u => u.FullName);

            if (TempData.ContainsKey("process") && TempData["process"] != null)
            {
                Models.DBModel.Order_Process process = (Models.DBModel.Order_Process)TempData["process"];
                ViewBag.status = (TempData.ContainsKey("status") && TempData["status"] != null) ? TempData["status"] : 1;
                ViewBag.Message = (TempData.ContainsKey("message") && TempData["message"] != null) ? TempData["message"] : "";
                ViewBag.lastPercent = (TempData.ContainsKey("lastPercent") && TempData["lastPercent"] != null) ? TempData["lastPercent"] : 0;
                ViewBag.lastStatusId = (TempData.ContainsKey("lastStatusId") && TempData["lastStatusId"] != null) ? TempData["lastStatusId"] : 1;

                process.Percent = 100 - ViewBag.lastPercent;
                process.ProductStatusId = (byte)ViewBag.lastStatusId;
                if (ViewBag.status == 2)
                {
                    process.ProductDocNumber = 0;
                    if (ViewBag.Message == "")
                    {
                        ViewBag.Message = "ثبت با موفقیت انجام شد";
                    }
                }
                return View(process);
            }
            else
            {
                Models.DBModel.Order_Process process = new Models.DBModel.Order_Process();
                process.UserId = (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id;
                process.PTime = DateTime.Now;
                ViewBag.status = 0;
                ViewBag.Message = "";
                ViewBag.lastPercent = 0;
                ViewBag.lastStatusId = 1;
                process.Percent = 100 - ViewBag.lastPercent;
                process.ProductStatusId = (byte)ViewBag.lastStatusId;

                return View(process);
            }
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Process)]
        public ActionResult Index(Models.DBModel.Order_Process model, string submit)
        {
            try
            {
                if (submit == "submit")
                {
                    List<Order_Process> processlist = Models.Order.OrderManager.Order_Process_Get(model.ProductDocNumber);

                    double lastPercent = 0;
                    byte lastStatusId = (byte)Models.ProductStatus.NagsheKeshi;

                    if (model.ProductDocNumber.ToString().StartsWith("2")) //20-29
                    {
                        Order_Cabin c = Models.Order.OrderManager.Order_Cabin_Get(model.ProductDocNumber);
                        lastStatusId = (byte)c.Tb_CabinPanels.StartFrom;
                    }
                    else if (model.ProductDocNumber.ToString().StartsWith("3")) //30-39
                    {
                        Order_Hall c = Models.Order.OrderManager.Order_Hall_Get(model.ProductDocNumber);
                        lastStatusId = (byte)c.Tb_HallPanels.StartFrom;
                    }
                    else if (model.ProductDocNumber.ToString().StartsWith("4")) //40-49
                    {
                        Order_DoorTop c = Models.Order.OrderManager.Order_DoorTop_Get(model.ProductDocNumber);
                        lastStatusId = (byte)c.Tb_DoorTopPanels.StartFrom;
                    }

                    foreach (Order_Process p in processlist)
                    {
                        if (p.ProductStatusId == model.ProductStatusId) lastPercent += p.Percent;
                        if (p.ProductStatusId > lastStatusId) lastStatusId = p.ProductStatusId;
                    }
                    TempData["lastPercent"] = lastPercent;
                    TempData["lastStatusId"] = lastStatusId;

                    if (model.Percent + lastPercent > 100)
                    {
                        throw new Exception("مجموع درصد انجام کار نباید بیشتر از 100 باشد");
                    }
                    if (model.ProductStatusId > lastStatusId + 1)
                    {
                        throw new Exception("مراحل باید به ترتیب انجام شوند");
                    }
                    if (model.ProductStatusId < lastStatusId)
                    {
                        if (!CustomAuthorizeAttribute.isAuthorize(Models.OPERATIONS.Process_Backward))
                        {
                            throw new Exception("شما اجازه دسترسی به این بخش را ندارید");
                        }
                    }

                    Models.DBModel.Order_Process res = Models.Order.OrderManager.Order_Process_Add(model);
                    Models.Order.OrderManager.Order_Process_UpdateStatus(res.OrderId);
                    TempData["status"] = 2; //process saved successfully
                    TempData["process"] = res;
                    return RedirectToAction("Index");
                }
                else
                {
                    List<Order_Process> processlist = Models.Order.OrderManager.Order_Process_Get(model.ProductDocNumber);

                    double lastPercent = 0;
                    byte lastStatusId = (byte)Models.ProductStatus.NagsheKeshi;

                    if (model.ProductDocNumber.ToString().StartsWith("2")) //20-29
                    {
                        Order_Cabin c = Models.Order.OrderManager.Order_Cabin_Get(model.ProductDocNumber);
                        lastStatusId = (byte)c.Tb_CabinPanels.StartFrom;
                    }
                    else if (model.ProductDocNumber.ToString().StartsWith("3")) //30-39
                    {
                        Order_Hall c = Models.Order.OrderManager.Order_Hall_Get(model.ProductDocNumber);
                        lastStatusId = (byte)c.Tb_HallPanels.StartFrom;
                    }
                    else if (model.ProductDocNumber.ToString().StartsWith("4")) //40-49
                    {
                        Order_DoorTop c = Models.Order.OrderManager.Order_DoorTop_Get(model.ProductDocNumber);
                        lastStatusId = (byte)c.Tb_DoorTopPanels.StartFrom;
                    }

                    foreach (Order_Process p in processlist)
                    {
                        if (p.ProductStatusId > lastStatusId) lastStatusId = p.ProductStatusId;
                    }
                    foreach (Order_Process p in processlist)
                    {
                        if (p.ProductStatusId == lastStatusId) lastPercent += p.Percent;
                    }
                    if (lastPercent >= 100)
                    {
                        if (lastStatusId < (byte)Models.ProductStatus.AmadeErsal)
                        {
                            lastStatusId++;
                            lastPercent = 0;
                        }
                    }
                    TempData["lastPercent"] = lastPercent;
                    TempData["lastStatusId"] = lastStatusId;

                    TempData["status"] = 1; //porcess form data filled
                    TempData["process"] = model;
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                TempData["status"] = -1; //error.
                TempData["process"] = model;
                TempData["message"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return RedirectToAction("Index");
            }
        }

        [CustomAuthorize(OPERATIONS.Process)]
        public JsonResult GetProcessList(int docNumber)
        {
            List<Order_Process> ObjList = OrderManager.Order_Process_Get(docNumber);

            var res = (from u in ObjList
                       select new { u.Id, u.ProductDocNumber, u.ShTime, StatusName = u.Order_ProductStatus.Name, u.Percent, OperatorName = u.Account_Users.FullName });
            return Json(res, JsonRequestBehavior.AllowGet);
        }



        [CustomAuthorize(OPERATIONS.Process_Project)]
        public ActionResult Project()
        {
            ViewBag.Operators = AccountManager.Account_User_Get().Where(u => u.DepartmentId == (byte)Models.Department.Tolid).ToDictionary(u => u.Id, u => u.FullName);

            if (TempData.ContainsKey("process") && TempData["process"] != null)
            {
                Models.DBModel.Order_Process process = (Models.DBModel.Order_Process)TempData["process"];
                int status = (TempData.ContainsKey("status") && TempData["status"] != null) ? (int)TempData["status"] : 1;
                ViewBag.Message = (TempData.ContainsKey("message") && TempData["message"] != null) ? TempData["message"] : "";
                if (status == 2)
                {
                    process = new Order_Process();
                    process.UserId = process.UserId;
                    process.PTime = DateTime.Now;
                    process.Percent = 100;
                    process.ProductStatusId = process.ProductStatusId;

                    if (ViewBag.Message == "")
                    {
                        ViewBag.Message = "ثبت با موفقیت انجام شد";
                    }
                }
                return View(process);
            }
            else
            {
                Models.DBModel.Order_Process process = new Order_Process();
                process.UserId = (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id;
                process.PTime = DateTime.Now;
                process.Percent = 100;
                process.ProductStatusId = (byte)ProductStatus.Montaj;
                ViewBag.Message = "";

                return View(process);
            }
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Process_Project)]
        public ActionResult Project(Order_Process model, string submit)
        {
            try
            {
                Order_Process res = Models.Order.OrderManager.Project_Process_Add(model);
                TempData["status"] = 2; //process saved successfully
                TempData["process"] = res;
                return RedirectToAction("Project");
            }
            catch (Exception ex)
            {
                TempData["status"] = -1; //error.
                TempData["process"] = model;
                TempData["message"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return RedirectToAction("Project");
            }
        }
    }
}
