using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Order;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace BaseSite.Controllers
{
    public class ReportController : Controller
    {
        [CustomAuthorize(OPERATIONS.Report_ProductFactor)]
        public ActionResult ProductFactors(string reportDateFrom, string reportDateTo)
        {
            ViewBag.reportDateFrom = string.IsNullOrWhiteSpace(reportDateFrom) ? new PersianDateTime(DateTime.Today).ToString(PersianDateTimeFormat.Date) : reportDateFrom;
            ViewBag.reportDateTo = string.IsNullOrWhiteSpace(reportDateTo) ? new PersianDateTime(DateTime.Today).ToString(PersianDateTimeFormat.Date) : reportDateTo;

            List<int> userIdList = new List<int>();
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Report_productFactor_AllOperators))
            {
                userIdList.AddRange(AccountManager.Account_User_Get().Where(u => u.DepartmentId == (byte)Models.Department.Tolid).Select(u => u.Id).ToList());
            }
            else
            {
                userIdList.Add((Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id);
            }

            List<ZaribKarkard> ProcessList = OrderManager.Order_Process_Report(userIdList,
                PersianDateTime.Parse(ViewBag.reportDateFrom.Replace('-', '/')).ToDateTime(),
                PersianDateTime.Parse(ViewBag.reportDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = ProcessList.Count();
            return View(ProcessList);
        }

        [CustomAuthorize(OPERATIONS.Report_ProductFactor)]
        public ActionResult SearchProductFactors(string reportDateFrom, string reportDateTo)
        {
            string paramlist = "";
            if (!string.IsNullOrWhiteSpace(reportDateFrom)) paramlist += ("reportDateFrom=" + reportDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(reportDateTo)) paramlist += ("reportDateTo=" + reportDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Report/ProductFactors" + paramlist));
        }

        [CustomAuthorize(OPERATIONS.Report_ProductFactor)]
        public JsonResult GetProcessList(int userId, string dateFrom, string dateTo)
        {
            List<Order_Process> ObjList = OrderManager.Order_Process_Report(userId,
                string.IsNullOrEmpty(dateFrom) ? DateTime.Today : PersianDateTime.Parse(dateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(dateTo) ? DateTime.Today : PersianDateTime.Parse(dateTo.Replace('-', '/')).ToDateTime());

            var res = (from u in ObjList
                       select new
                       {
                           u.Id,
                           u.ShTime,
                           DocNumber = u.Order_Order.DocNumber,
                           u.ProductDocNumber,
                           Description = u.OrderId == 0 ? u.Description : u.Order_Order.Account_Users.FullName,
                           ItemName = u.ProductDocNumber.ToString().StartsWith("2") ? "داخل کابین" : (u.ProductDocNumber.ToString().StartsWith("3") ? "طبقات" : (u.ProductDocNumber.ToString().StartsWith("4") ? "سردرب" : "سایر")),
                           ItemModel = u.OrderId == 0 ? "-" :
                                       u.ProductDocNumber.ToString().StartsWith("2") ? u.Order_Order.Order_Cabin.Where(x => x.DocNumber == u.ProductDocNumber).SingleOrDefault().Tb_CabinPanels.Name :
                                       u.ProductDocNumber.ToString().StartsWith("3") ? u.Order_Order.Order_Hall.Where(x => x.DocNumber == u.ProductDocNumber).SingleOrDefault().Tb_HallPanels.Name :
                                       u.ProductDocNumber.ToString().StartsWith("4") ? u.Order_Order.Order_DoorTop.Where(x => x.DocNumber == u.ProductDocNumber).SingleOrDefault().Tb_DoorTopPanels.Name : "-",
                           ItemPFactor = u.ProductFactor,
                           u.Count,
                           StatusName = u.Order_ProductStatus.Name,
                           u.Percent,
                           OperatorName = u.Account_Users.FullName,
                           u.CalculatedFactor
                       });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(OPERATIONS.Report_ProductFactor)]
        public ActionResult PrintProductFactors(int userId, string dateFrom, string dateTo)
        {
            List<Order_Process> ObjList = OrderManager.Order_Process_Report(userId,
                string.IsNullOrEmpty(dateFrom) ? DateTime.Today : PersianDateTime.Parse(dateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(dateTo) ? DateTime.Today : PersianDateTime.Parse(dateTo.Replace('-', '/')).ToDateTime());

            //var res = (from u in ObjList
            //           select new
            //           {
            //               u.Id,
            //               DocNumber = u.Order_Order.DocNumber,
            //               u.ProductDocNumber,
            //               ItemName = u.ProductDocNumber.ToString().StartsWith("2") ? "داخل کابین" : (u.ProductDocNumber.ToString().StartsWith("3") ? "طبقات" : (u.ProductDocNumber.ToString().StartsWith("4") ? "سردرب" : "-")),
            //               u.ShTime,
            //               StatusName = u.Order_ProductStatus.Name,
            //               u.Percent,
            //               OperatorName = u.Account_Users.Name + " " + u.Account_Users.LastName,
            //               PFactor = u.ProductFactor * (100 - u.CollectiveProducePercent) / 100
            //           });

            Account_Users user = AccountManager.Account_User_Get(userId);
            ViewBag.UserFullName = user.FullName;
            ViewBag.ShDateFrom = dateFrom;
            ViewBag.ShDateTo = dateTo;

            return View(ObjList);
        }


        [CustomAuthorize(OPERATIONS.Report)]
        public ActionResult ReportList()
        {
            List<Report_Report> ReportList = new List<Report_Report>();

            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_CustomerBill))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 1,
                    ReportName = "RCustomerBill",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "RCustomerBill", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "صورتحساب مشتری خاص"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_SaleControlling))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 2,
                    ReportName = "RSaleControlling",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "RSaleControlling", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "گزارش کنترلی(فروش روزانه)"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_CustomersBill))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 3,
                    ReportName = "RCustomersBill",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "RCustomersBill", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "صورتحساب کلی"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_Lending))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 4,
                    ReportName = "RLending",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "RLending", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "کالای امانی ما نزد دیگران"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_Statistic))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 5,
                    ReportName = "RStatistic",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "RStatistic", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "گزارشات آماری"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_Statistic2))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 6,
                    ReportName = "RStatistic2",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "RStatistic2", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "گزارشات آماری - ریز مصرف"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_Orders_Monthly_OrderDate))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 7,
                    ReportName = "ROrders_Monthly_OrderDate",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "ROrders_Monthly_OrderDate", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "گزارش ماهانه سفارشات درحال تولید یا تحویل شده -بر اساس تاریخ سفارش"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_Orders_Monthly_FactorDate))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 8,
                    ReportName = "ROrders_Monthly_FactorDate",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "ROrders_Monthly_FactorDate", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "گزارش ماهانه سفارشات درحال تولید یا تحویل شده -بر اساس تاریخ فاکتور"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_Sales_Payments_Monthly))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 9,
                    ReportName = "RSales_Payments_Monthly",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("ReportTemplate", "Report", new { ReportName = "RSales_Payments_Monthly", ReportDescription = "گزارشات", Width = 100, Height = 640 }),
                    ReportDescription = "نمودار وصول / فروش ماهانه"
                });
            }


            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_KPI))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 10,
                    ReportName = "RNewCustomers",
                    ReportType = ReportTypes.KPI,
                    ReportURL = Url.Action("ReportTemplate2", "Report", new { ReportName = "RNewCustomers", ReportDescription = "گزارشات", Width = 900, Height = 650 }),
                    ReportDescription = "نمودار تعداد مشتریان جدید در ماه"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_KPI))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 11,
                    ReportName = "RSalesToNewCustomers",
                    ReportType = ReportTypes.KPI,
                    ReportURL = Url.Action("ReportTemplate2", "Report", new { ReportName = "RSalesToNewCustomers", ReportDescription = "گزارشات", Width = 900, Height = 650 }),
                    ReportDescription = "نمودار نسبت فروش به مشتریان جدید و مشتریان قبلی در هر ماه"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_KPI))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 12,
                    ReportName = "ROrder_PreparationDays",
                    ReportType = ReportTypes.KPI,
                    ReportURL = Url.Action("ReportTemplate2", "Report", new { ReportName = "ROrder_PreparationDays", ReportDescription = "گزارشات", Width = 900, Height = 650 }),
                    ReportDescription = "مدت زمان تحویل سفارشات"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_KPI))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 13,
                    ReportName = "RSalesByUsers",
                    ReportType = ReportTypes.KPI,
                    ReportURL = Url.Action("ReportTemplate2", "Report", new { ReportName = "RSalesByUsers", ReportDescription = "گزارشات", Width = 900, Height = 650 }),
                    ReportDescription = "نمودار رتبه بندی فروش کارشناسان فروش"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_KPI))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 14,
                    ReportName = "RBuyersRank",
                    ReportType = ReportTypes.KPI,
                    ReportURL = Url.Action("ReportTemplate2", "Report", new { ReportName = "RBuyersRank", ReportDescription = "گزارشات", Width = 900, Height = 650 }),
                    ReportDescription = "جدول رتبه بندی خرید مشتریان"
                });
            }


            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Logs_Logs))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 15,
                    ReportName = "Logs_Logs",
                    ReportType = ReportTypes.Sale,
                    ReportURL = Url.Action("LogList", "Log"),
                    ReportDescription = "تاریخچه فعالیت ها"
                });
            }
            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_ProductFactor))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 16,
                    ReportName = "ProductFactors",
                    ReportType = ReportTypes.Production,
                    ReportURL = Url.Action("ProductFactors", "Report"),
                    ReportDescription = "ضرایب کارکرد"
                });
            }

            if (((List<BaseSite.Models.OPERATIONS>)Session["UserOperations"]).Contains(BaseSite.Models.OPERATIONS.Report_CustomersInfo))
            {
                ReportList.Add(new Report_Report()
                {
                    ReportId = 17,
                    ReportName = "RCustomersInfo",
                    ReportType = ReportTypes.General,
                    ReportURL = Url.Action("ReportTemplate2", "Report", new { ReportName = "RCustomersInfo", ReportDescription = "گزارشات", Width = 900, Height = 650 }),
                    ReportDescription = "جدول اطلاعات مشتریان"
                });
            }

            ViewBag.RowCount = ReportList.Count();
            return View(ReportList);
        }

        [CustomAuthorize(OPERATIONS.Report)]
        public ActionResult ReportTemplate(string ReportName, string ReportDescription, int Width, int Height, string Customer, string reportShDateFrom = "", string reportShDateTo = "", int customerId = 0)
        {
            if (ReportName != "RStatistic2")
            {
                if (customerId == 0 && Session["customerId"] != null) customerId = (int)Session["customerId"];
                ViewBag.CustomerName = (customerId == 0 ? "" : AccountManager.Account_User_Get(customerId).FullName);

                Session["customerId"] = customerId;
            }

            if (string.IsNullOrWhiteSpace(reportShDateFrom) && string.IsNullOrWhiteSpace(reportShDateTo))
            {
                if (ReportName == "RSaleControlling")
                {
                    reportShDateFrom = reportShDateTo = new PersianDateTime(DateTime.Now).ToString(PersianDateTimeFormat.Date);
                }
                else
                {
                    reportShDateFrom = new PersianDateTime(DateTime.Now).FirstDayOfYear.ToString(PersianDateTimeFormat.Date);
                    reportShDateTo = new PersianDateTime(DateTime.Now).LastDayOfYear.ToString(PersianDateTimeFormat.Date);
                }
            }

            var rptInfo = new Report_Report
            {
                ReportName = ReportName,
                ReportDescription = ReportDescription,
                ReportURL = String.Format("../../Reports/ReportTemplate.aspx?ReportName={0}&Height={1}&ShDateFrom={2}&ShDateTo={3}&CustomerId={4}", ReportName, Height, reportShDateFrom, reportShDateTo, customerId),
                Width = Width,
                Height = Height,
                ReportShDateFrom = reportShDateFrom,
                ReportShDateTo = reportShDateTo,
                ReportArg1 = customerId
            };

            return View(rptInfo);
        }

        [CustomAuthorize(OPERATIONS.Report)]
        public ActionResult ReportTemplate2(string ReportName, string ReportDescription, int Width, int Height, string Customer, string reportShDateFrom = "", string reportShDateTo = "", int customerId = 0)
        {
            if (ReportName != "RStatistic2")
            {
                if (customerId == 0 && Session["customerId"] != null) customerId = (int)Session["customerId"];
                ViewBag.CustomerName = (customerId == 0 ? "" : AccountManager.Account_User_Get(customerId).FullName);

                Session["customerId"] = customerId;
            }

            if (string.IsNullOrWhiteSpace(reportShDateFrom) && string.IsNullOrWhiteSpace(reportShDateTo))
            {
                if (ReportName == "RSaleControlling")
                {
                    reportShDateFrom = reportShDateTo = new PersianDateTime(DateTime.Now).ToString(PersianDateTimeFormat.Date);
                }
                else
                {
                    reportShDateFrom = new PersianDateTime(DateTime.Now).FirstDayOfYear.ToString(PersianDateTimeFormat.Date);
                    reportShDateTo = new PersianDateTime(DateTime.Now).LastDayOfYear.ToString(PersianDateTimeFormat.Date);
                }
            }


            WebReport webReport = new WebReport()
            {
                Width = Width,
                Height = Height,
                //AutoWidth = true,
                //AutoHeight = true,
                ReportFile = HostingEnvironment.MapPath("~/Reports/" + ReportName + ".frx") // load the report from the file
            };

            webReport.Report.SetParameterValue("shdatefrom", reportShDateFrom);
            webReport.Report.SetParameterValue("shdateto", reportShDateTo);
            webReport.Report.SetParameterValue("customerid", customerId);

            ViewBag.WebReport = webReport; // send object to the View

            var rptInfo = new Report_Report
            {
                ReportName = ReportName,
                ReportDescription = ReportDescription,
                Width = Width,
                Height = Height,
                ReportShDateFrom = reportShDateFrom,
                ReportShDateTo = reportShDateTo,
                ReportArg1 = customerId
            };

            return View(rptInfo);
        }
    }
}
