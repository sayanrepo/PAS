using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseSite.Models.Payment;
using BaseSite.Models.DBModel;
using BaseSite.Models.Account;
using BaseSite.Models;
using BaseSite.Models.Log;

namespace BaseSite.Controllers
{
    public class PaymentController : Controller
    {
        [CustomAuthorize(OPERATIONS.Payment)]
        public ActionResult PaymentList(int? docNumber, byte? paymentStatusId, byte? bargashti, int? customerId, byte? paymentTypeId, byte? babatId, string sanadDateFrom, string sanadDateTo, string sarresidDateFrom, string sarresidDateTo)
        {
            customerId = (int?)Session["customerId"];

            ViewBag.docNumber = docNumber;
            ViewBag.paymentStatusId = paymentStatusId;
            ViewBag.bargashti = bargashti;
            ViewBag.customerId = customerId;
            ViewBag.CustomerName = (ViewBag.customerId == null) ? "" : AccountManager.Account_User_Get((int)(ViewBag.customerId)).FullName;
            ViewBag.paymentTypeId = paymentTypeId;
            ViewBag.babatId = babatId;
            ViewBag.sanadDateFrom = sanadDateFrom;
            ViewBag.sanadDateTo = sanadDateTo;
            ViewBag.sarresidDateFrom = sarresidDateFrom;
            ViewBag.sarresidDateTo = sarresidDateTo;

            List<Payment_Payment> paymentList = PaymentManager.Payment_Payment_Search(docNumber, paymentStatusId, bargashti, customerId, paymentTypeId, babatId,
                string.IsNullOrEmpty(sanadDateFrom) ? null : (DateTime?)PersianDateTime.Parse(sanadDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(sanadDateTo) ? null : (DateTime?)PersianDateTime.Parse(sanadDateTo.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(sarresidDateFrom) ? null : (DateTime?)PersianDateTime.Parse(sarresidDateFrom.Replace('-', '/')).ToDateTime(),
                string.IsNullOrEmpty(sarresidDateTo) ? null : (DateTime?)PersianDateTime.Parse(sarresidDateTo.Replace('-', '/')).ToDateTime());

            ViewBag.RowCount = paymentList.Count();
            return View(paymentList);
        }

        [CustomAuthorize(OPERATIONS.Payment_Add)]
        public ActionResult AddPayment()
        {
            Payment_Payment obj = new Payment_Payment();
            obj.DateSanad = obj.DateSarresid = DateTime.Now;
            obj.PaymentBabatId = 14;
            obj.StatusId = (byte)PaymentStatus.TayidNashode;
            obj.Payment_Status = new Payment_Status() { Id = (byte)Models.PaymentStatus.TayidNashode, Name = "تایید نشده" };

            if (Session["customerId"] != null)
            {
                try
                {
                    Account_Users user = AccountManager.Account_User_Get((int)(Session["customerId"]));
                    obj.CustomerId = user.Id;
                    ViewBag.CustomerName = user.FullName;
                }
                catch { }
            }

            return View("PaymentDetail", obj);
        }

        [CustomAuthorize(OPERATIONS.Payment_Detail)]
        public ActionResult PaymentDetail(string paymentId)
        {
            Payment_Payment payment = PaymentManager.Payment_Payment_Get(int.Parse(paymentId));
            ViewBag.CustomerName = AccountManager.Account_User_Get(payment.CustomerId).FullName;
            return View("PaymentDetail", payment);
        }

        [HttpPost]
        [CustomAuthorize(OPERATIONS.Payment_Add)]
        public ActionResult PaymentDetail(Payment_Payment model, string Amount, string submit)
        {
            model.Amount = string.IsNullOrEmpty(Amount) ? 0 : double.Parse(Amount.Replace(",", ""));
            if (submit.ToLower() == "submit")
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Payment_Add))
                    return RedirectToAction("AccessDenied", "Home");
                else
                    model.StatusId = (byte)PaymentStatus.TayidNashode;
            }
            if (submit.ToLower() == "foroshconfirm")
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Payment_ForoshConfirm))
                    return RedirectToAction("AccessDenied", "Home");
                else
                    model.StatusId = (byte)PaymentStatus.TayidForosh;
            }
            if (submit.ToLower() == "maliconfirm")
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Payment_MaliConfirm))
                    return RedirectToAction("AccessDenied", "Home");
                else
                    model.StatusId = (byte)PaymentStatus.TayidMali;
            }
            if (submit.ToLower() == "malireject")
            {
                if (!CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Payment_MaliConfirm))
                    return RedirectToAction("AccessDenied", "Home");
                else
                    model.StatusId = (byte)PaymentStatus.TayidNashode;
            }
            bool isNew = false;
            if (model.Id == 0)
            {
                isNew = true;
                model.AccepterId = Session["PantaUser"] == null ? 0 : (Session["PantaUser"] as BaseSite.Models.DBModel.Account_Users).Id;
            }
            Payment_Payment x = PaymentManager.Payment_Payment_Edit(model, submit);
            LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, x.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, isNew ? (int)LogActivity.Add : (int)LogActivity.Edit, x.ToString(), x.Amount);
            return RedirectToAction("PaymentDetail", new { paymentId = x.Id });
        }

        [CustomAuthorize(OPERATIONS.Payment_Search)]
        public ActionResult SearchPayment(int? docNumber, byte? paymentStatusId, byte? bargashti, int? customerId, string Customer, byte? paymentTypeId, byte? babatId, string sanadDateFrom, string sanadDateTo, string sarresidDateFrom, string sarresidDateTo)
        {
            if (String.IsNullOrWhiteSpace(Customer)) customerId = null;
            Session["customerId"] = customerId;

            string paramlist = "";
            if (docNumber.HasValue) paramlist += ("docNumber=" + docNumber.Value.ToString() + "&");
            if (paymentStatusId.HasValue) paramlist += ("paymentStatusId=" + paymentStatusId.Value.ToString() + "&");
            if (bargashti.HasValue) paramlist += ("bargashti=" + bargashti.Value.ToString() + "&");
            if (customerId.HasValue) paramlist += ("customerId=" + customerId.Value.ToString() + "&");
            if (paymentTypeId.HasValue) paramlist += ("paymentTypeId=" + paymentTypeId.Value.ToString() + "&");
            if (babatId.HasValue) paramlist += ("babatId=" + babatId.Value.ToString() + "&");
            if (!string.IsNullOrWhiteSpace(sanadDateFrom)) paramlist += ("sanadDateFrom=" + sanadDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(sanadDateTo)) paramlist += ("sanadDateTo=" + sanadDateTo + "&");
            if (!string.IsNullOrWhiteSpace(sarresidDateFrom)) paramlist += ("sarresidDateFrom=" + sarresidDateFrom + "&");
            if (!string.IsNullOrWhiteSpace(sarresidDateTo)) paramlist += ("sarresidDateTo=" + sarresidDateTo + "&");

            if (!string.IsNullOrWhiteSpace(paramlist)) paramlist = "?" + paramlist.Remove(paramlist.Length - 1);

            return Redirect(Url.Content("~/Payment/PaymentList" + paramlist));
        }

        [CustomAuthorize(OPERATIONS.Payment_Print)]
        public ActionResult Print(string doc, int id)
        {
            if (doc == "payment-accounting")
            {
                ViewBag.Accounting = true;
                Payment_Payment pay = PaymentManager.Payment_Payment_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, pay.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ نسخه حسابداری");
                return View("PrintPayment", pay);
            }
            else if (doc == "payment-customer")
            {
                ViewBag.Accounting = false;
                Payment_Payment pay = PaymentManager.Payment_Payment_Get(id);
                LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, pay.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Print, "چاپ نسخه مشتری");
                return View("PrintPayment", pay);
            }
            else
                return View("Error");
        }

        [CustomAuthorize(OPERATIONS.Payment_Delete)]
        public ActionResult PaymentDelete(int paymentId)
        {
            //List<Payment_Payment> PayList = PaymentManager.Payment_Payment_Search(null, null, null, null, null, null,
            //    null, (DateTime?)PersianDateTime.Parse("1396/12/29".Replace('-', '/')).ToDateTime(),
            //    null, null);
            //for (int i = 0; i < PayList.Count; i++)
            //{
            //    PaymentManager.Payment_Payment_Delete(PayList.ElementAt(i).Id);
            //}

            try
            {
                Payment_Payment pay = PaymentManager.Payment_Payment_Get(paymentId);
                PaymentManager.Payment_Payment_Delete(paymentId);
                LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, pay.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.Delete, "");
                return RedirectToAction("PaymentList", "Payment");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [CustomAuthorize(OPERATIONS.Payment_ChangeStatus)]
        public ActionResult PaymentChangeStatus(int paymentId, byte newStatusId)
        {
            try
            {
                Payment_Payment pay = PaymentManager.Payment_Payment_ChangeStatus(paymentId, (PaymentStatus)newStatusId);
                LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, pay.DocNumber, CustomAuthorizeAttribute.getCurrentUser().Id, Request.UserHostAddress, (int)LogActivity.ChangeStatus, pay.ToString(), pay.Amount);
                return RedirectToAction("PaymentDetail", new { paymentId = paymentId });
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
    }
}
