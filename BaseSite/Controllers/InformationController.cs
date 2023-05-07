using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseSite.Models.Information;
using BaseSite.Models.DBModel;
using BaseSite.Models;
using BaseSite.Models.Log;
namespace BaseSite.Controllers
{
    public class InformationController : Controller
    {
        [CustomAuthorize(OPERATIONS.Setting_Cities)]
        public ActionResult Cities(int? Countryid, int? Provinceid)
        {
            List<Location_Countries> CountryList = InformationManager.Location_Country_Get().Where(x => x.Id > 0).ToList(); ;
            Countryid = (int?)(Countryid == null ? (CountryList.Count > 0 ? CountryList.First().Id : 0) : Countryid);
            List<Location_Provinces> ProvinceList = InformationManager.Location_Province_Get((int)Countryid).Where(x => x.Id > 0).ToList();
            Provinceid = (int?)(Provinceid == null ? (ProvinceList.Count > 0 ? ProvinceList.First().Id : 0) : Provinceid);
            List<Location_Cities> CityList = InformationManager.Location_City_Get((int)Provinceid).Where(x => x.Id > 0).ToList();

            ViewBag.Country = CountryList;
            ViewBag.Province = ProvinceList;
            ViewBag.City = CityList;

            ViewBag.CountryID = Countryid;
            ViewBag.ProvinceID = Provinceid;
            return View();
        }

        //################################ Cities #######################################################
        [CustomAuthorize(OPERATIONS.Setting_Cities_Delete)]
        public ActionResult CityDelete(string Countryid, string Provinceid, string Cityid)
        {
            if (!string.IsNullOrEmpty(Cityid))
            {
                string name = InformationManager.Location_City_Get().Where(m => m.Id == int.Parse(Cityid)).FirstOrDefault().Name;
                InformationManager.Location_City_Delete(int.Parse(Cityid));
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Cityid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف شهر: " + name);
                return RedirectToAction("Cities", new { Countryid = Countryid, Provinceid = Provinceid });
            }
            else if (!string.IsNullOrEmpty(Provinceid))
            {
                string name = InformationManager.Location_Province_Get().Where(m => m.Id == int.Parse(Provinceid)).FirstOrDefault().Name;
                InformationManager.Location_Province_Delete(int.Parse(Provinceid));
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Provinceid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف استان: " + name);
                return RedirectToAction("Cities", new { Countryid = Countryid });
            }
            else if (!string.IsNullOrEmpty(Countryid))
            {
                string name = InformationManager.Location_Country_Get().Where(m => m.Id == int.Parse(Countryid)).FirstOrDefault().Name;
                InformationManager.Location_Country_Delete(int.Parse(Countryid));
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Countryid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف کشور: " + name);
            }
            return RedirectToAction("Cities");
        }

        [CustomAuthorize(OPERATIONS.Setting_Cities_Edit)]
        public ActionResult CityEdit(string Countryid, string CountryName, string Provinceid, string ProvinceName, string Cityid, string CityName)
        {
            if (!string.IsNullOrEmpty(Cityid))
            {
                InformationManager.Location_City_Edit(int.Parse(Cityid), CityName);
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Cityid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, "تغییر نام شهر به " + CityName);
                return RedirectToAction("Cities", new { Countryid = Countryid, Provinceid = Provinceid });
            }
            else if (!string.IsNullOrEmpty(Provinceid))
            {
                InformationManager.Location_Province_Edit(int.Parse(Provinceid), ProvinceName);
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Provinceid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, "تغییر نام استان به " + ProvinceName);
                return RedirectToAction("Cities", new { Countryid = Countryid, Provinceid = Provinceid });
            }
            else if (!string.IsNullOrEmpty(Countryid))
            {
                InformationManager.Location_Country_Edit(int.Parse(Countryid), CountryName);
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Countryid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, "تغییر نام کشور به " + CountryName);
            }
            return RedirectToAction("Cities", new { Countryid = Countryid });
        }

        [CustomAuthorize(OPERATIONS.Setting_Cities_Add)]
        public ActionResult CityAdd(string Countryid, string CountryName, string Provinceid, string ProvinceName, string Cityid, string CityName)
        {
            if (!string.IsNullOrEmpty(CountryName))
            {
                Countryid = InformationManager.Location_Country_Add(countryName: CountryName).ToString();
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Countryid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, "افزودن کشور " + CountryName);
                return RedirectToAction("Cities", new { Countryid = Countryid });
            }
            else if (!string.IsNullOrEmpty(ProvinceName))
            {
                Provinceid = InformationManager.Location_Province_Add(countryId: int.Parse(Countryid), provinceName: ProvinceName).ToString();
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Provinceid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, "افزودن استان " + ProvinceName);
                return RedirectToAction("Cities", new { Countryid = Countryid, Provinceid = Provinceid });
            }
            else if (!string.IsNullOrEmpty(CityName))
            {
                Cityid = InformationManager.Location_City_Add(provinceId: int.Parse(Provinceid), cityName: CityName).ToString();
                LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(Cityid), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, "افزودن شهر " + CityName);
            }
            return RedirectToAction("Cities", new { Countryid = Countryid, Provinceid = Provinceid });
        }



        [CustomAuthorize(OPERATIONS.Setting_Order)]
        public ActionResult Order()
        {
            var Pack = InformationManager.Order_Pack_Get();
            ViewBag.Pack = Pack;
            var ElevatorBoard = InformationManager.Order_ElevatorBoard_Get();
            ViewBag.ElevatorBoard = ElevatorBoard;
            var Deduction = InformationManager.Order_Deduction_Get();
            ViewBag.Deduction = Deduction;
            var Addition = InformationManager.Order_Addition_Get();
            ViewBag.Addition = Addition;
            return View();
        }

        //######################################### Pack #########################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Packet_Add)]
        public ActionResult AddPack(string PackName)
        {
            var PackId = InformationManager.Order_Pack_Add(PackName);
            ViewBag.PackId = PackId.ToString();
            LogManager.Log_Logs_Add((int)DB_Table.Others, PackId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, "افزودن بسته بندی " + PackName);
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Packet_Delete)]
        public ActionResult DeletePack(string PackId)
        {
            InformationManager.Order_Pack_Delete(packId: int.Parse(PackId));
            LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(PackId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف بسته بندی");
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Packet_Edit)]
        public ActionResult EditPack(string PackId, string PackName)
        {
            InformationManager.Order_Pack_Edit(packId: int.Parse(PackId), newName: PackName);
            LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(PackId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, "تغییر نام بسته بندی به " + PackName);
            return RedirectToAction("Order");
        }

        //######################################### ElevatorBoard #########################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_ElevatorBoard_Add)]
        public ActionResult AddElevatorBoard(string ElevatorBoardName)
        {
            var ElevatorBoardId = InformationManager.Order_ElevatorBoard_Add(ElevatorBoardName);
            LogManager.Log_Logs_Add((int)DB_Table.Others, ElevatorBoardId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, "افزودن تابلوی " + ElevatorBoardName);
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_ElevatorBoard_Delete)]
        public ActionResult DeleteElevatorBoard(string ElevatorBoardId)
        {
            InformationManager.Order_ElevatorBoard_Delete(int.Parse(ElevatorBoardId));
            LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(ElevatorBoardId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف تابلو");
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_ElevatorBoard_Edit)]
        public ActionResult EditElevatorBoard(string ElevatorBoardId, string ElevatorBoardName)
        {
            InformationManager.Order_ElevatorBoard_Edit(int.Parse(ElevatorBoardId), ElevatorBoardName);
            LogManager.Log_Logs_Add((int)DB_Table.Others, int.Parse(ElevatorBoardId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, "تغییر نام تابلو به " + ElevatorBoardName);
            return RedirectToAction("Order");
        }

        //####################################### Deduction #############################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Deduction_Add)]
        public ActionResult AddDeduction(string DeductionName)
        {
            int deductionId = InformationManager.Order_Deduction_Add(deductionName: DeductionName);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Deductions, deductionId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, "افزودن کسورات " + DeductionName);
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Deduction_Delete)]
        public ActionResult DeductionDelete(string DeductionId)
        {
            InformationManager.Order_Deduction_Delete(deductionId: int.Parse(DeductionId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Deductions, int.Parse(DeductionId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف کسورات");
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Deduction_Edit)]
        public ActionResult DeductionEdit(string DeductionId, string DeductionName)
        {
            InformationManager.Order_Deduction_Edit(deductionId: int.Parse(DeductionId), newName: DeductionName);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Deductions, int.Parse(DeductionId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, "تغییر نام کسورات به " + DeductionName);
            return RedirectToAction("Order");
        }

        //########################################## Addition ############################################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Addition_Add)]
        public ActionResult AddAddition(string AdditionName)
        {
            int AdditionId = InformationManager.Order_Addition_Add(AdditionName);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Additions, AdditionId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, "افزودن اضافات " + AdditionName);
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Addition_Delete)]
        public ActionResult AdditionDelete(string AdditionId)
        {
            InformationManager.Order_Addition_Delete(additionId: int.Parse(AdditionId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Additions, int.Parse(AdditionId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف اضافات");
            return RedirectToAction("Order");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Order_Addition_Edit)]
        public ActionResult AdditionEdit(string AdditionId, string AdditionName)
        {
            InformationManager.Order_Addition_Edit(additionId: int.Parse(AdditionId), newName: AdditionName);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Additions, int.Parse(AdditionId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, "تغییر نام اضافات به " + AdditionName);
            return RedirectToAction("Order");
        }



        [CustomAuthorize(OPERATIONS.Setting_Attachment)]
        public ActionResult Attachment()
        {
            var Attachment = InformationManager.Order_Attachment_Get();
            return View(Attachment);
        }

        //######################################### Attachment #########################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Attachment_Add)]
        public ActionResult AddAttachment(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable, bool? IsDeliveryItem)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            bool? isDeliveryItem = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
                if (IsDeliveryItem.HasValue) isDeliveryItem = IsDeliveryItem.Value;
                else isDeliveryItem = false;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int attachmentId = InformationManager.Order_Attachment_Add(ItemName, ItemDescription, cost, facotr, available, isDeliveryItem);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Attachments, attachmentId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.Order_Attachment_Get().Where(m => m.Id == attachmentId).FirstOrDefault().ToString());
            return RedirectToAction("Attachment");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Attachment_Delete)]
        public ActionResult DeleteAttachment(string ItemId)
        {
            InformationManager.Order_Attachment_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Attachments, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف ملحقات");
            return RedirectToAction("Attachment");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Attachment_Edit)]
        public JsonResult EditAttachment(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable, bool? IsDeliveryItem)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            bool? isDeliveryItem = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
                isDeliveryItem = IsDeliveryItem ?? false;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.Order_Attachment_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available, isDeliveryItem);
            var res = from u in InformationManager.Order_Attachment_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor, u.IsDeliveryItem };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_Attachments, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.Order_Attachment_Get().Where(m => m.Id == int.Parse(ItemId)).FirstOrDefault().ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize(OPERATIONS.Setting_PushButton)]
        public ActionResult PushButton()
        {
            var PushButton = InformationManager.Cabin_PushButton_Get();
            return View(PushButton);
        }

        //########################### PushButton ############################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_PushButton_Add)]
        public ActionResult AddPushButton(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int pushbuttonId = InformationManager.Cabin_PushButton_Add(ItemName, ItemDescription, cost, facotr, available);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_PushButtons, pushbuttonId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.Cabin_PushButton_Get(pushbuttonId).ToString());
            return RedirectToAction("PushButton");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_PushButton_Delete)]
        public ActionResult DeletePushButton(string ItemId)
        {
            InformationManager.Cabin_PushButton_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_PushButtons, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف پوش باتون: " + InformationManager.Cabin_PushButton_Get(int.Parse(ItemId)).Name);
            return RedirectToAction("PushButton");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_PushButton_Edit)]
        public JsonResult EditPushButton(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }

            InformationManager.Cabin_PushButton_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available);
            var res = from u in InformationManager.Cabin_PushButton_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_PushButtons, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.Cabin_PushButton_Get(int.Parse(ItemId)).ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }



        [CustomAuthorize(OPERATIONS.Setting_Monitor)]
        public ActionResult Monitor()
        {
            var Monitor = InformationManager.Cabin_Monitor_Get();
            return View(Monitor);
        }

        //############################## Monitor #############################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Monitor_Add)]
        public ActionResult AddMonitor(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int monitorId = InformationManager.Cabin_Monitor_Add(ItemName, ItemDescription, cost, facotr, available);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Monitors, monitorId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.Cabin_Monitor_Get(monitorId).ToString());
            return RedirectToAction("Monitor");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Monitor_Delete)]
        public ActionResult DeleteMonitor(string ItemId)
        {
            InformationManager.Cabin_Monitor_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Monitors, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف نمایشگر: " + InformationManager.Cabin_Monitor_Get(int.Parse(ItemId)).Name);
            return RedirectToAction("Monitor");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Monitor_Edit)]
        public JsonResult EditMonitor(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }

            InformationManager.Cabin_Monitor_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available);
            var res = from u in InformationManager.Cabin_Monitor_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_Monitors, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.Cabin_Monitor_Get(int.Parse(ItemId)).ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize(OPERATIONS.Setting_CabinSurfaceMetal)]
        public ActionResult SurfaceMetal()
        {
            var SurfaceMetal = InformationManager.SurfaceMetal_Get();
            return View(SurfaceMetal);
        }
        //########################### SurfaceMetal ##########################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinSurfaceMetal_Add)]
        public ActionResult _AddSurfaceMetal(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int surfacemetalId = InformationManager.SurfaceMetal_Add(ItemName, ItemDescription, cost, facotr, available);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_SurfaceMetals, surfacemetalId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.SurfaceMetal_Get(surfacemetalId).ToString());
            return RedirectToAction("SurfaceMetal");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinSurfaceMetal_Delete)]
        public ActionResult _DeleteSurfaceMetal(string ItemId)
        {
            InformationManager.SurfaceMetal_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_SurfaceMetals, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف فلزرویه: " + InformationManager.SurfaceMetal_Get(int.Parse(ItemId)).Name);
            return RedirectToAction("SurfaceMetal");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinSurfaceMetal_Edit)]
        public JsonResult _EditSurfaceMetal(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.SurfaceMetal_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available);
            var res = from u in InformationManager.SurfaceMetal_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_SurfaceMetals, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.SurfaceMetal_Get(int.Parse(ItemId)).ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }



        [CustomAuthorize(OPERATIONS.Setting_CabinPanel)]
        public ActionResult CabinPanel()
        {
            var PanelModel = InformationManager.Cabin_Panel_Get();
            ViewBag.PanelModel = PanelModel;
            var SurfaceMetal = InformationManager.Cabin_SurfaceMetal_Get();
            ViewBag.SurfaceMetal = SurfaceMetal;
            return View();
        }

        //########################### PanelModel ##########################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinPanel_Add)]
        public ActionResult AddPanelModel(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int panelId = InformationManager.Cabin_Panel_Add(ItemName, ItemDescription, cost, facotr, available);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_CabinPanels, panelId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.Cabin_Panel_Get(panelId).ToString());
            return RedirectToAction("CabinPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinPanel_Delete)]
        public ActionResult DeletePanelModel(string ItemId)
        {
            InformationManager.Cabin_Panel_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_CabinPanels, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف پنل داخل کابین: " + InformationManager.Cabin_Panel_Get(int.Parse(ItemId)).Name);
            return RedirectToAction("CabinPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinPanel_Edit)]
        public JsonResult EditPanelModel(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable, string ItemSurfaceArea, byte? ItemStartFrom)
        {
            double? cost = null;
            double? facotr = null;
            double? surfaceArea = null;
            bool? available = null;
            byte? startfrom = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
                startfrom = ItemStartFrom;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_SurfaceArea))
            {
                if (!string.IsNullOrEmpty(ItemSurfaceArea)) surfaceArea = double.Parse(ItemSurfaceArea);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }

            InformationManager.Cabin_Panel_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available, surfaceArea, startfrom);
            var res = from u in InformationManager.Cabin_Panel_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor, u.SurfaceArea, u.StartFrom };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_CabinPanels, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.Cabin_Panel_Get(int.Parse(ItemId)).ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }

        //########################### SurfaceMetal ##########################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinSurfaceMetal_Add)]
        public ActionResult AddSurfaceMetal(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.Cabin_SurfaceMetal_Add(ItemName, ItemDescription, cost, facotr, available);
            return RedirectToAction("CabinPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinSurfaceMetal_Delete)]
        public ActionResult DeleteSurfaceMetal(string ItemId)
        {
            InformationManager.Cabin_SurfaceMetal_Delete(int.Parse(ItemId));
            return RedirectToAction("CabinPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CabinSurfaceMetal_Edit)]
        public JsonResult EditSurfaceMetal(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.Cabin_SurfaceMetal_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available);
            var res = from u in InformationManager.Cabin_SurfaceMetal_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor };

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }



        [CustomAuthorize(OPERATIONS.Setting_HallPanel)]
        public ActionResult HallPanel()
        {
            var PanelModel = InformationManager.Hall_Panel_Get();
            ViewBag.PanelModel = PanelModel;
            var SurfaceMetal = InformationManager.Hall_SurfaceMetal_Get();
            ViewBag.SurfaceMetal = SurfaceMetal;
            return View();
        }

        //################################ PanelModel #######################################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_HallPanel_Add)]
        public ActionResult AddHallPanelModel(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int panelId = InformationManager.Hall_Panel_Add(ItemName, ItemDescription, cost, facotr, available);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_HallPanels, panelId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.Hall_Panel_Get(panelId).ToString());
            return RedirectToAction("HallPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_HallPanel_Delete)]
        public ActionResult DeleteHallPanelModel(string ItemId)
        {
            InformationManager.Hall_Panel_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_HallPanels, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف پنل طبقات: " + InformationManager.Hall_Panel_Get(int.Parse(ItemId)).Name);
            return RedirectToAction("HallPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_HallPanel_Edit)]
        public JsonResult EditHallPanelModel(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable, string ItemSurfaceArea, byte? ItemStartFrom)
        {
            double? cost = null;
            double? facotr = null;
            double? surfaceArea = null;
            bool? available = null;
            byte? startfrom = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
                startfrom = ItemStartFrom;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_SurfaceArea))
            {
                if (!string.IsNullOrEmpty(ItemSurfaceArea)) surfaceArea = double.Parse(ItemSurfaceArea);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.Hall_Panel_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available, surfaceArea, startfrom);
            var res = from u in InformationManager.Hall_Panel_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor, u.SurfaceArea, u.StartFrom };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_HallPanels, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.Hall_Panel_Get(int.Parse(ItemId)).ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }

        //################################# SurfaceMetal ####################################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_HallSurfaceMetal_Add)]
        public ActionResult AddHallSurfaceMetal(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.Hall_SurfaceMetal_Add(ItemName, ItemDescription, cost, facotr, available);
            return RedirectToAction("HallPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_HallSurfaceMetal_Delete)]
        public ActionResult DeleteHallSurfaceMetal(string ItemId)
        {
            InformationManager.Hall_SurfaceMetal_Delete(int.Parse(ItemId));
            return RedirectToAction("HallPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_HallSurfaceMetal_Edit)]
        public ActionResult EditHallSurfaceMetal(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.Hall_SurfaceMetal_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available);
            var res = from u in InformationManager.Hall_SurfaceMetal_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor };

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }



        [CustomAuthorize(OPERATIONS.Setting_DoorTopPanel)]
        public ActionResult DoorTopPanel()
        {
            var PanelModel = InformationManager.DoorTop_Panel_Get();
            ViewBag.PanelModel = PanelModel;
            return View();
        }

        //########################### PanelModel #############################################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_DoorTopPanel_Add)]
        public ActionResult AddDoorTopPanelModel(string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable)
        {
            double? cost = null;
            double? facotr = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int panelId = InformationManager.DoorTop_Panel_Add(ItemName, ItemDescription, cost, facotr, available);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_DoorTopPanels, panelId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.DoorTop_Panel_Get(panelId).ToString());
            return RedirectToAction("DoorTopPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_DoorTopPanel_Delete)]
        public ActionResult DeleteDoorTopPanelModel(string ItemId)
        {
            InformationManager.DoorTop_Panel_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_DoorTopPanels, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف پنل سردرب: " + InformationManager.DoorTop_Panel_Get(int.Parse(ItemId)).Name);
            return RedirectToAction("DoorTopPanel");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_DoorTopPanel_Edit)]
        public JsonResult EditDoorTopPanelModel(string ItemId, string ItemName, string ItemDescription, string ItemCost, string ItemProductFactor, bool? ItemAvailable, string ItemSurfaceArea, byte? ItemStartFrom)
        {
            double? cost = null;
            double? facotr = null;
            double? surfaceArea = null;
            bool? available = null;
            byte? startfrom = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_ProductFactor))
            {
                if (!string.IsNullOrEmpty(ItemProductFactor)) facotr = double.Parse(ItemProductFactor);
                startfrom = ItemStartFrom;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_SurfaceArea))
            {
                if (!string.IsNullOrEmpty(ItemSurfaceArea)) surfaceArea = double.Parse(ItemSurfaceArea);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.DoorTop_Panel_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, facotr, available, surfaceArea, startfrom);
            var res = from u in InformationManager.DoorTop_Panel_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available, u.ProductFactor, u.SurfaceArea, u.StartFrom };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_DoorTopPanels, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.DoorTop_Panel_Get(int.Parse(ItemId)).ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize(OPERATIONS.Setting_ProductFactorCost, OPERATIONS.Setting_CollectiveProducePercent)]
        public ActionResult ProductFactor()
        {
            if (TempData["ErrorMsg"] != null)
                ViewBag.ErrorMsg = TempData["ErrorMsg"];
            ViewBag.PFCost = InformationManager.ProductFactorCost_Get();
            ViewBag.CPPercent = InformationManager.CollectiveProducePercent_Get();
            return View();
        }

        //########################### ProductFactorCost #######################################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_ProductFactorCost_Add)]
        public ActionResult AddProductFactorCost(string shApplyDate, double cost)
        {
            try
            {
                InformationManager.ProductFactorCost_Add(PersianDateTime.Parse(shApplyDate.Replace('-', '/')).ToDateTime(), cost);
                LogManager.Log_Logs_Add((int)DB_Table.Others, 0, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, string.Format("افزودن ارزش ریالی ضریب کارکرد: اعمال از تاریخ {0} ارزش ریالی {1} ریال", shApplyDate, cost.ToString()));
                return RedirectToAction("ProductFactor");
            }
            catch (Exception ex)
            {
                TempData.Add("ErrorMsg", ex.Message);
                return RedirectToAction("ProductFactor");
            }
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_ProductFactorCost_Delete)]
        public ActionResult DeleteProductFactorCost(string shDate)
        {
            try
            {
                InformationManager.ProductFactorCost_Delete(PersianDateTime.Parse(shDate.Replace('-', '/')).ToDateTime());
                LogManager.Log_Logs_Add((int)DB_Table.Others, 0, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, string.Format("حذف ارزش ریالی ضریب کارکرد: اعمال از تاریخ {0}", shDate));
                return RedirectToAction("ProductFactor");
            }
            catch (Exception ex)
            {
                TempData.Add("ErrorMsg", ex.Message);
                return RedirectToAction("ProductFactor");
            }
        }

        //################################ CollectiveProducePercent ############################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CollectiveProducePercent_Add)]
        public ActionResult AddCollectiveProducePercent(string shApplyDate, double percent)
        {
            try
            {
                InformationManager.CollectiveProducePercent_Add(PersianDateTime.Parse(shApplyDate.Replace('-', '/')).ToDateTime(), percent);
                LogManager.Log_Logs_Add((int)DB_Table.Others, 0, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, string.Format("افزودن کاهش ضریب در تولید تیراژی: اعمال از تاریخ {0} درصد کاهش {1} درصد", shApplyDate, percent.ToString()));
                return RedirectToAction("ProductFactor");
            }
            catch (Exception ex)
            {
                TempData.Add("ErrorMsg", ex.Message);
                return RedirectToAction("ProductFactor");
            }
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_CollectiveProducePercent_Delete)]
        public ActionResult DeleteCollectiveProducePercent(string shDate)
        {
            try
            {
                InformationManager.CollectiveProducePercent_Delete(PersianDateTime.Parse(shDate.Replace('-', '/')).ToDateTime());
                LogManager.Log_Logs_Add((int)DB_Table.Others, 0, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, string.Format("حذف کاهش ضریب در تولید تیراژی: اعمال از تاریخ {0}", shDate));
                return RedirectToAction("ProductFactor");
            }
            catch (Exception ex)
            {
                TempData.Add("ErrorMsg", ex.Message);
                return RedirectToAction("ProductFactor");
            }
        }


        //####################################### TruthTable ############################################
        [CustomAuthorize(OPERATIONS.Setting_TruthTable)]
        public ActionResult TruthTable()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_TruthTable)]
        public ActionResult TruthTable(int primaryTableId, int secondaryTableId)
        {
            ViewBag.PrimaryTableId = primaryTableId;
            ViewBag.SecondaryTableId = secondaryTableId;

            switch (primaryTableId)
            {
                case (int)DB_Table.Tb_CabinPanels:
                    ViewBag.PrimaryTable = Cache.CabinPanels.Where(m => m.Key > 0).ToDictionary(m => m.Key, m => m.Value);
                    break;
                case (int)DB_Table.Tb_HallPanels:
                    ViewBag.PrimaryTable = Cache.HallPanels.Where(m => m.Key > 0).ToDictionary(m => m.Key, m => m.Value);
                    break;
                case (int)DB_Table.Tb_DoorTopPanels:
                    ViewBag.PrimaryTable = Cache.DoorTopPanels.Where(m => m.Key > 0).ToDictionary(m => m.Key, m => m.Value);
                    break;
                default:
                    ViewBag.PrimaryTable = new Dictionary<int, Models.CacheItem>();
                    break;
            }

            switch (secondaryTableId)
            {
                case (int)DB_Table.Tb_PushButtons:
                    ViewBag.SecondaryTable = Cache.PushButtons;
                    break;
                case (int)DB_Table.Tb_Monitors:
                    ViewBag.SecondaryTable = Cache.Monitors;
                    break;
                default:
                    ViewBag.SecondaryTable = new Dictionary<int, Models.CacheItem>();
                    break;
            }

            List<Tb_Truth> res = InformationManager.TruthTable_Get(primaryTableId, secondaryTableId);
            return View("TruthTableFull", res);
        }

        [CustomAuthorize(OPERATIONS.Setting_TruthTable_Edit)]
        public JsonResult TruthTableEdit(int primaryTableId, int secondaryTableId, int primaryId, int secondaryId, double value)
        {
            var res = InformationManager.TruthTable_Edit(primaryTableId, primaryId, secondaryTableId, secondaryId, value);
            var res2 = new { res.Id, res.PrimaryTableId, res.PrimaryId, res.SecondaryTableId, res.SecondaryId, res.TValue };
            return Json(res2, JsonRequestBehavior.AllowGet);
        }


        //######################################### Store Products #########################################
        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Product_Add)]
        public ActionResult AddProduct(string ItemName, string ItemDescription, string ItemCost, bool? ItemAvailable)
        {
            double? cost = null;
            bool? available = null;
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            int productId = InformationManager.Product_Add(ItemName, ItemDescription, cost, available);
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Products, productId, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Add, InformationManager.Products_Get().Where(m => m.Id == productId).FirstOrDefault().ToString());
            return RedirectToAction("Product");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Product_Delete)]
        public ActionResult DeleteProduct(string ItemId)
        {
            InformationManager.Product_Delete(int.Parse(ItemId));
            LogManager.Log_Logs_Add((int)DB_Table.Tb_Products, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "حذف محصول");
            return RedirectToAction("Product");
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Setting_Product_Edit)]
        public JsonResult EditProduct(string ItemId, string ItemName, string ItemDescription, string ItemCost, bool? ItemAvailable)
        {
            double? cost = null;
            bool? available = null;
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Name))
            {
                ItemName = null;
            }
            if (!CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Description))
            {
                ItemDescription = null;
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Cost))
            {
                if (!string.IsNullOrEmpty(ItemCost)) cost = double.Parse(ItemCost);
            }
            if (CustomAuthorizeAttribute.isAuthorize(BaseSite.Models.OPERATIONS.Setting_Available))
            {
                if (ItemAvailable.HasValue) available = ItemAvailable.Value;
                else available = false;
            }
            InformationManager.Product_Edit(int.Parse(ItemId), ItemName, ItemDescription, cost, available);
            var res = from u in InformationManager.Products_Get().Where(m => m.Id == int.Parse(ItemId))
                      select new { u.Id, u.Name, u.Description, u.Cost, u.Available };

            LogManager.Log_Logs_Add((int)DB_Table.Tb_Products, int.Parse(ItemId), CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Edit, InformationManager.Products_Get().Where(m => m.Id == int.Parse(ItemId)).FirstOrDefault().ToString());

            return Json(res.First(), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(OPERATIONS.Setting_Product)]
        public ActionResult Product()
        {
            var Product = InformationManager.Products_Get();
            return View(Product);
        }
    }
}
