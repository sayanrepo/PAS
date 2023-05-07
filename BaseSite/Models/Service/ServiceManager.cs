using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BaseSite.Data;

namespace BaseSite.Models.Service
{
    public class ServiceManager
    {
        private static int Service_GenerateDocNumber()
        {
            //DocNumber format : [900000-999999]
            int num = Models.Order.OrderManager.RandomDocNumber.Next(900000, 999999);

            using (var context = new PantaEntities())
            {
                while (context.Service_Service.Any(m => m.DocNumber == num))
                {
                    num = Models.Order.OrderManager.RandomDocNumber.Next(900000, 999999);
                }
            }
            return num;
        }

        public static Service_Service Service_Service_Add()
        {
            using (var context = new PantaEntities())
            {
                Service_Service x = context.Service_Service.Include(m => m.Account_Users)
                    .Where(m => m.Id == 0).SingleOrDefault();

                return x;
            }
        }

        public static List<Service_Service> Service_Service_Search(int? docNumber, byte? status, int? customerId, DateTime? orderDateFrom, DateTime? orderDateTo, DateTime? factorDateFrom, DateTime? factorDateTo)
        {
            using (var context = new PantaEntities())
            {
                if (docNumber == null && status == null && customerId == null && orderDateFrom == null && orderDateTo == null &&
                   factorDateFrom == null && factorDateTo == null)
                {
                    List<Service_Service> result = context.Service_Service.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes)
                                              .Include(m => m.Order_Status).Where(o => o.Id > 0).OrderByDescending(m => m.Id).Take(1000).ToList();

                    return result;
                }
                else
                {
                    var list = from p in context.Service_Service.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes).Include(m => m.Order_Status)
                               where p.Id > 0
                               select p;

                    if (docNumber != null) list = list.Where(p => p.DocNumber == docNumber);
                    if (status != null) list = list.Where(p => p.StatusId == (byte)status);
                    if (customerId != null) list = list.Where(p => p.CustomerId == customerId);
                    if (orderDateFrom != null) list = list.Where(p => p.DateOrder >= orderDateFrom);
                    if (orderDateTo != null) list = list.Where(p => p.DateOrder <= orderDateTo);
                    if (factorDateFrom != null) list = list.Where(p => p.DateFactor >= factorDateFrom);
                    if (factorDateTo != null) list = list.Where(p => p.DateFactor <= factorDateTo);
                    list = list.OrderByDescending(p => p.Id);

                    // Execute the query
                    List<Service_Service> result = list.ToList();

                    return result;
                }
            }
        }

        public static Service_Service Service_Service_Edit(Service_Service service, string submit)
        {
            using (var context = new PantaEntities())
            {
                if (service.Id == 0)
                {
                    Service_Service newservice = new Service_Service();
                    newservice.TableId = 21;
                    newservice.DocNumber = Service_GenerateDocNumber();
                    newservice.CustomerId = service.CustomerId;
                    newservice.ClienteleName = service.ClienteleName;
                    newservice.OrderTypeId = service.OrderTypeId;
                    newservice.DeliveryCityId = service.DeliveryCityId;
                    newservice.DeliveryAddress = service.DeliveryAddress;
                    newservice.Tax = service.Tax;
                    newservice.StatusId = (byte)OrderStatus.PishFactor;
                    newservice.DateOrder = service.DateOrder; //DateTime.Now;
                    newservice.DateDelivery = service.DateOrder;  //null;
                    newservice.DateFactor = service.DateFactor;  //null; // DateTime.Now.AddDays(5);
                    newservice.ServiceCost = service.ServiceCost;
                    newservice.DeliveryCost = service.DeliveryCost;
                    newservice.Discount = service.Discount;
                    newservice.Cost = service.ServiceCost + (service.DeliveryCost.HasValue ? (double)service.DeliveryCost : 0);
                    newservice.Comment = service.Comment;
                    newservice.AccepterId = service.AccepterId;

                    newservice.Cost += (newservice.Cost * (newservice.Tax / 100));
                    newservice.Cost -= newservice.Discount;
                    context.Service_Service.Add(newservice);
                    context.SaveChanges();
                    return Service_Service_Get(newservice.Id);
                }
                else
                {
                    Service_Service newservice = context.Service_Service.Include(m => m.Account_Users).Include(m => m.Order_Status)
                        .Include(m => m.Tb_OrderTypes).Where(m => m.Id == service.Id).SingleOrDefault();

                    newservice.CustomerId = service.CustomerId;
                    newservice.ClienteleName = service.ClienteleName;
                    newservice.OrderTypeId = service.OrderTypeId;
                    newservice.DeliveryCityId = service.DeliveryCityId;
                    newservice.DeliveryAddress = service.DeliveryAddress;
                    newservice.Tax = service.Tax;
                    newservice.StatusId = service.StatusId;

                    newservice.DateOrder = service.DateOrder; //DateTime.Now;
                    newservice.DateDelivery = service.DateOrder;  //null;
                    newservice.DateFactor = service.DateFactor;  //null; // DateTime.Now.AddDays(5);

                    //if (newservice.StatusId == (byte)OrderStatus.DarDasteEghdam)
                    //{
                    //    //newservice.DateOrder = DateTime.Now;              // service.DateOrder;
                    //    newservice.DateDelivery = null;                   // service.DateDelivery;
                    //    newservice.DateFactor = DateTime.Now.AddDays(5);  // service.DateFactor;
                    //}
                    //else if (newservice.StatusId == (byte)OrderStatus.TahvilShode)
                    //{
                    //    newservice.DateDelivery = DateTime.Now;
                    //    newservice.DateFactor = DateTime.Now;//.AddDays(5);
                    //}
                    //else
                    //{
                    //    //newservice.DateOrder = service.DateOrder;
                    //    //newservice.DateDelivery = service.DateDelivery;
                    //    //newservice.DateFactor = service.DateFactor;
                    //}
                    newservice.ServiceCost = service.ServiceCost;
                    newservice.DeliveryCost = service.DeliveryCost;
                    newservice.Discount = service.Discount;
                    newservice.Cost = service.ServiceCost + (service.DeliveryCost.HasValue ? (double)service.DeliveryCost : 0);
                    newservice.Comment = service.Comment;

                    newservice.Cost += (newservice.Cost * (newservice.Tax / 100));
                    newservice.Cost -= newservice.Discount;
                    //context.Service_Service.Add(neworder);
                    context.SaveChanges();
                    return Service_Service_Get(newservice.Id);
                }
            }
        }

        public static Service_Service Service_Service_Get(int serviceId)
        {
            using (var context = new PantaEntities())
            {
                Service_Service service = context.Service_Service.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Order_Status)
                    .Include(m => m.Tb_OrderTypes).Where(m => m.Id == serviceId).SingleOrDefault();

                service.SumCostTax = (service.ServiceCost + (service.DeliveryCost.HasValue ? service.DeliveryCost.Value : 0)) * (service.Tax / 100);

                return service;
            }
        }

        public static int Service_Service_GetLastFactorNumber()
        {
            using (var context = new PantaEntities())
            {
                int Service_LastFactorNumber = context.Service_Service.Max(m => m.FactorNumber);
                if (Service_LastFactorNumber < 900000) Service_LastFactorNumber = 900000;
                return Service_LastFactorNumber;
            }
        }

        public static Service_Service Service_Service_ChangeStatus(int serviceId, Models.OrderStatus currentStatus, bool onlyChangeStatus = false)
        {
            using (var context = new PantaEntities())
            {
                Service_Service service = context.Service_Service.Include(m => m.Account_Users).Include(m => m.Order_Status)
                    .Include(m => m.Tb_OrderTypes).Where(m => m.Id == serviceId).SingleOrDefault();

                service.StatusId = (byte)currentStatus;
                if (!onlyChangeStatus && (byte)currentStatus == (byte)OrderStatus.MojavezKhorooj)
                {
                    service.DateFactor = DateTime.Now;
                    if (service.FactorNumber == 0) service.FactorNumber = Service_Service_GetLastFactorNumber() + 1;
                }
                context.SaveChanges();
                return Service_Service_Get(service.Id);
            }
        }

        public static void Service_Service_Delete(int serviceId)
        {
            using (var context = new PantaEntities())
            {
                Service_Service service = context.Service_Service.Where(m => m.Id == serviceId).SingleOrDefault();
                context.Service_Service.Remove(service);
                context.SaveChanges();
            }
        }
    }
}