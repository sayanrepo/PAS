using BaseSite.Data;
using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.Payment
{
    public class PaymentManager
    {
        private static int Payment_GenerateDocNumber()
        {
            //DocNumber format : [9][100000-999999]
            int num = Models.Order.OrderManager.RandomDocNumber.Next(100000, 999999) + 9000000;

            using (var context = new PantaEntities())
            {
                while (context.Payment_Payment.Any(m => m.DocNumber == num))
                {
                    num = Models.Order.OrderManager.RandomDocNumber.Next(100000, 999999) + 9000000;
                }
            }
            return num;
        }

        public static Payment_Payment Payment_Payment_Add()
        {
            using (var context = new PantaEntities())
            {
                Payment_Payment x = context.Payment_Payment.Include(m => m.Account_Users).Include(m => m.Payment_Babats)
                    .Include(m => m.Payment_Banks).Include(m => m.Payment_Types).Include(m => m.Payment_Status).Where(m => m.Id == 0).SingleOrDefault();

                return x;
            }
        }

        public static List<Payment_Payment> Payment_Payment_Search(int? docNumber, byte? status, byte? bargashti, int? customerId, byte? typeId, byte? babatId, DateTime? sanadDateFrom, DateTime? sanadDateTo, DateTime? sarresidDateFrom, DateTime? sarresidDateTo)
        {
            using (var context = new PantaEntities())
            {
                if (docNumber == null && status == null && customerId == null && bargashti == null && typeId == null && babatId == null &&
                   sanadDateFrom == null && sanadDateTo == null && sarresidDateFrom == null && sarresidDateTo == null)
                {
                    List<Payment_Payment> result = context.Payment_Payment.Include(m => m.Account_Users).Include(m => m.Accepter)
                        .Include(m => m.Payment_Babats).Include(m => m.Payment_Banks).Include(m => m.Payment_Types).Include(m => m.Payment_Status)
                    .Where(o => o.Id > 0).OrderByDescending(m => m.DateSanad).ThenByDescending(m => m.Id).Take(1000).ToList();

                    return result;
                }
                else
                {
                    var list = from p in context.Payment_Payment.Include(m => m.Account_Users).Include(m => m.Accepter)
                               .Include(m => m.Payment_Babats).Include(m => m.Payment_Banks).Include(m => m.Payment_Types).Include(m => m.Payment_Status)
                               where p.Id > 0
                               select p;

                    if (docNumber != null) list = list.Where(p => p.DocNumber == docNumber);
                    if (status != null) list = list.Where(p => p.StatusId == status.Value);
                    if (customerId != null) list = list.Where(p => p.CustomerId == customerId);
                    if (bargashti != null) list = list.Where(p => p.Bargashti == (bargashti.Value > 0));
                    if (typeId != null) list = list.Where(p => p.PaymentTypeId == typeId);
                    if (babatId != null) list = list.Where(p => p.PaymentBabatId == babatId);
                    if (sanadDateFrom != null) list = list.Where(p => p.DateSanad >= sanadDateFrom);
                    if (sanadDateTo != null) list = list.Where(p => p.DateSanad <= sanadDateTo);
                    if (sarresidDateFrom != null) list = list.Where(p => p.DateSarresid >= sarresidDateFrom);
                    if (sarresidDateTo != null) list = list.Where(p => p.DateSarresid <= sarresidDateTo);
                    list = list.OrderByDescending(m => m.DateSanad).ThenByDescending(m => m.Id);

                    // Execute the query
                    List<Payment_Payment> result = list.ToList();

                    return result;
                }
            }
        }

        public static Payment_Payment Payment_Payment_Edit(Payment_Payment payment, string submit)
        {
            using (var context = new PantaEntities())
            {
                if (payment.Id == 0)
                {
                    Payment_Payment newpayment = new Payment_Payment();
                    newpayment.DocNumber = Payment_GenerateDocNumber();
                    newpayment.CustomerId = payment.CustomerId;
                    newpayment.ProjectName = payment.ProjectName;
                    newpayment.PaymentTypeId = payment.PaymentTypeId;
                    newpayment.PaymentBabatId = payment.PaymentBabatId;
                    newpayment.BankId = payment.BankId;
                    newpayment.BankBranchCode = payment.BankBranchCode;
                    newpayment.ShomareSanad = payment.ShomareSanad;
                    newpayment.ShomareHesab = payment.ShomareHesab;
                    newpayment.DateSanad = payment.DateSanad;
                    newpayment.DateSarresid = payment.DateSarresid;
                    newpayment.Amount = payment.Amount;
                    newpayment.StatusId = (byte)PaymentStatus.TayidNashode;
                    newpayment.Bargashti = payment.Bargashti;
                    newpayment.Comment = payment.Comment;
                    newpayment.AccepterId = payment.AccepterId;

                    context.Payment_Payment.Add(newpayment);
                    context.SaveChanges();
                    return PaymentManager.Payment_Payment_Get(newpayment.Id);
                }
                else
                {
                    Payment_Payment newpayment = context.Payment_Payment.Include(m => m.Account_Users).Include(m => m.Payment_Babats)
                    .Include(m => m.Payment_Banks).Include(m => m.Payment_Types).Include(m => m.Payment_Status).Where(m => m.Id == payment.Id).SingleOrDefault();

                    newpayment.CustomerId = payment.CustomerId;
                    newpayment.ProjectName = payment.ProjectName;
                    newpayment.PaymentTypeId = payment.PaymentTypeId;
                    newpayment.PaymentBabatId = payment.PaymentBabatId;
                    newpayment.BankId = payment.BankId;
                    newpayment.BankBranchCode = payment.BankBranchCode;
                    newpayment.ShomareSanad = payment.ShomareSanad;
                    newpayment.ShomareHesab = payment.ShomareHesab;
                    newpayment.DateSanad = payment.DateSanad;
                    newpayment.DateSarresid = payment.DateSarresid;
                    newpayment.Amount = payment.Amount;
                    newpayment.StatusId = payment.StatusId;
                    newpayment.Bargashti = payment.Bargashti;
                    newpayment.Comment = payment.Comment;

                    context.SaveChanges();
                    return PaymentManager.Payment_Payment_Get(newpayment.Id);
                }
            }
        }

        public static Payment_Payment Payment_Payment_Get(int paymentId)
        {
            using (var context = new PantaEntities())
            {
                Payment_Payment pay = context.Payment_Payment.Include(m => m.Account_Users).Include(m => m.Accepter).Include(m => m.Payment_Babats)
                    .Include(m => m.Payment_Banks).Include(m => m.Payment_Types).Include(m => m.Payment_Status).Where(m => m.Id == paymentId).SingleOrDefault();

                return pay;
            }
        }

        public static Payment_Payment Payment_Payment_ChangeStatus(int paymentId, Models.PaymentStatus currentStatus)
        {
            using (var context = new PantaEntities())
            {
                Payment_Payment pay = context.Payment_Payment.Include(m => m.Account_Users).Include(m => m.Payment_Babats)
                    .Include(m => m.Payment_Banks).Include(m => m.Payment_Types).Include(m => m.Payment_Status).Where(m => m.Id == paymentId).SingleOrDefault();

                pay.StatusId = (byte)currentStatus;
                context.SaveChanges();
                return Payment_Payment_Get(pay.Id);
            }
        }

        public static void Payment_Payment_Delete(int paymentId)
        {
            using (var context = new PantaEntities())
            {
                Payment_Payment payment = context.Payment_Payment.Where(m => m.Id == paymentId).SingleOrDefault();
                context.Payment_Payment.Remove(payment);
                context.SaveChanges();
            }
        }
    }
}