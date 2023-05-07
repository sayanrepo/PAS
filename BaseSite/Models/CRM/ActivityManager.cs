using BaseSite.Data;
using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.CRM
{
    public class ActivityManager
    {
        //public static Random RandomDocNumber = new Random();

        //public static int Order_GenerateDocNumber()
        //{
        //    //DocNumber format : [100000-799999]
        //    int num = RandomDocNumber.Next(100000, 799999);

        //    using (var context = new PantaEntities())
        //    {
        //        while (context.Order_Order.Any(m => m.DocNumber == num))
        //        {
        //            num = RandomDocNumber.Next(100000, 799999);
        //        }
        //    }
        //    return num;
        //}

        //public static int Order_GenerateDocNumber(int orderDocNumber, int subNumber)
        //{
        //    int i = 0;
        //    int num = orderDocNumber + ((subNumber + i) * 1000000);

        //    using (var context = new PantaEntities())
        //    {
        //        if (subNumber == 2)
        //            while (context.Order_Cabin.Any(m => m.DocNumber == num))
        //            {
        //                i++;
        //                num = orderDocNumber + ((subNumber + i) * 1000000);
        //            }
        //        else if (subNumber == 3)
        //            while (context.Order_Hall.Any(m => m.DocNumber == num))
        //            {
        //                i++;
        //                num = orderDocNumber + ((subNumber + i) * 1000000);
        //            }
        //        else if (subNumber == 4)
        //            while (context.Order_DoorTop.Any(m => m.DocNumber == num))
        //            {
        //                i++;
        //                num = orderDocNumber + ((subNumber + i) * 1000000);
        //            }
        //        else if (subNumber == 5)
        //            while (context.Delivery_Delivery.Any(m => m.DocNumber == num))
        //            {
        //                i++;
        //                num = orderDocNumber + ((subNumber + i) * 1000000);
        //            }
        //    }
        //    return num;
        //}

        public static CRM_Activity CRM_Activity_Get(int activityId)
        {
            using (var context = new PantaEntities())
            {
                CRM_Activity activity = context.CRM_Activity
                    .Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Account_Users2)
                    .Include(m => m.CRM_ActivityType).Include(m => m.CRM_Priority).Include(m => m.CRM_ActivityState)
                    .Where(m => m.Id == activityId).SingleOrDefault();

                return activity;
            }
        }

        //public static int Order_Order_GetLastFactorNumber()
        //{
        //    using (var context = new PantaEntities())
        //    {
        //        int Order_LastFactorNumber = context.Order_Order.Max(m => m.FactorNumber);
        //        if (Order_LastFactorNumber < 100000) Order_LastFactorNumber = 100000;
        //        return Order_LastFactorNumber;
        //    }
        //}

        public static CRM_Activity CRM_Activity_Edit(CRM_Activity activity, string submit)
        {
            using (var context = new PantaEntities())
            {
                if (activity.Id == 0)
                {
                    CRM_Activity neworder = new CRM_Activity();
                    neworder.TypeId = activity.TypeId;
                    neworder.OwnerId = activity.OwnerId;
                    neworder.AssignedToId = activity.AssignedToId;
                    neworder.CustomerId = activity.CustomerId;
                    neworder.Subject = activity.Subject;
                    neworder.Description = activity.Description;
                    neworder.InOut = activity.InOut;
                    neworder.PriorityId = activity.PriorityId;
                    neworder.StateId = activity.StateId;
                    neworder.StartTime = activity.StartTime;
                    neworder.EndTime = activity.EndTime;
                    neworder.RepeatDays = activity.RepeatDays;

                    context.CRM_Activity.Add(neworder);
                    context.SaveChanges();
                    return CRM_Activity_Get(neworder.Id);
                }
                else
                {
                    CRM_Activity neworder = context.CRM_Activity
                    .Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Account_Users2)
                    .Include(m => m.CRM_ActivityType).Include(m => m.CRM_Priority).Include(m => m.CRM_ActivityState)
                    .Where(m => m.Id == activity.Id).SingleOrDefault();

                    //neworder.TypeId = activity.TypeId;
                    neworder.OwnerId = activity.OwnerId;
                    neworder.AssignedToId = activity.AssignedToId;
                    neworder.CustomerId = activity.CustomerId;
                    neworder.Subject = activity.Subject;
                    neworder.Description = activity.Description;
                    neworder.InOut = activity.InOut;
                    neworder.PriorityId = activity.PriorityId;
                    neworder.StateId = activity.StateId;
                    neworder.StartTime = activity.StartTime;
                    neworder.EndTime = activity.EndTime;
                    neworder.RepeatDays = activity.RepeatDays;

                    //context.Order_Order.Add(neworder);
                    context.SaveChanges();
                    return CRM_Activity_Get(neworder.Id);
                }
            }
        }

        //public static Order_Order Order_Order_ChangeStatus(int orderId, Models.OrderStatus currentStatus, bool onlyChangeStatus = false)
        //{
        //    using (var context = new PantaEntities())
        //    {
        //        Order_Order neworder = context.Order_Order
        //            .Include(m => m.Order_Status).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_PackTypes).Include(m => m.Tb_ElevatorBoards)
        //            .Where(m => m.Id == orderId).SingleOrDefault();

        //        neworder.StatusId = (byte)currentStatus;
        //        if (!onlyChangeStatus && (byte)currentStatus == (byte)OrderStatus.MojavezKhorooj)
        //        {
        //            neworder.DateFactor = DateTime.Now;
        //            if (neworder.FactorNumber == 0) neworder.FactorNumber = Order_Order_GetLastFactorNumber() + 1;
        //        }
        //        context.SaveChanges();
        //        return Order_Order_Get(neworder.Id);
        //    }
        //}

        public static List<CRM_Activity> CRM_Activity_Search(byte? status, int? customerId, int? ownerId, int? assignedToId, byte? typeId, byte? priorityId, string term, DateTime? startDateFrom, DateTime? startDateTo, DateTime? endDateFrom, DateTime? endDateTo)
        {
            using (var context = new PantaEntities())
            {
                if (status == null && customerId == null && ownerId == null && assignedToId == null && typeId == null && priorityId == null &&
                    string.IsNullOrEmpty(term) && startDateFrom == null && startDateTo == null && endDateFrom == null && endDateTo == null)
                {
                    List<CRM_Activity> result = context.CRM_Activity.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Account_Users2)
                                                .Include(m => m.CRM_ActivityType).Include(m => m.CRM_Priority).Include(m => m.CRM_ActivityState)
                                                .Where(o => o.Id > 0).OrderByDescending(m => m.Id).Take(1000).ToList();
                    return result;
                }
                else
                {
                    var list = from p in context.CRM_Activity.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Account_Users2)
                                        .Include(m => m.CRM_ActivityType).Include(m => m.CRM_Priority).Include(m => m.CRM_ActivityState)
                               where p.Id > 0
                               select p;

                    if (status != null) list = list.Where(p => p.StateId == (byte)status);
                    if (customerId != null) list = list.Where(p => p.CustomerId == customerId);
                    if (ownerId != null) list = list.Where(p => p.OwnerId == ownerId);
                    if (assignedToId != null) list = list.Where(p => p.AssignedToId == assignedToId);
                    if (typeId != null) list = list.Where(p => p.TypeId == typeId);
                    if (priorityId != null) list = list.Where(p => p.PriorityId == priorityId);
                    if (!string.IsNullOrEmpty(term)) list = list.Where(p => p.Subject.Replace(" ", "").ToLower().Contains(term.Replace(" ", "").ToLower()) || p.Description.Replace(" ", "").ToLower().Contains(term.Replace(" ", "").ToLower()));
                    if (startDateFrom != null) list = list.Where(p => p.StartTime >= startDateFrom);
                    if (startDateTo != null) list = list.Where(p => p.StartTime <= startDateTo);
                    if (endDateFrom != null) list = list.Where(p => p.EndTime >= endDateFrom);
                    if (endDateTo != null) list = list.Where(p => p.EndTime <= endDateTo);
                    list = list.OrderByDescending(p => p.Id);

                    // Execute the query
                    List<CRM_Activity> result = list.ToList();

                    return result;
                }
            }
        }

        public static List<CRM_Activity> CRM_Activity_Search(int userId, DateTime startDate)
        {
            using (var context = new PantaEntities())
            {
                var list = from p in context.CRM_Activity.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Account_Users2)
                                    .Include(m => m.CRM_ActivityType).Include(m => m.CRM_Priority).Include(m => m.CRM_ActivityState)
                           where p.Id > 0 && (p.OwnerId == userId || p.AssignedToId == userId) && p.StartTime >= startDate
                           select p;

                list = list.OrderByDescending(p => p.Id);

                // Execute the query
                List<CRM_Activity> result = list.ToList();

                return result;
            }
        }

        public static void CRM_Activity_Delete(int activityId)
        {
            using (var context = new PantaEntities())
            {
                CRM_Activity activity = context.CRM_Activity
                    .Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Account_Users2)
                    .Include(m => m.CRM_ActivityType).Include(m => m.CRM_Priority).Include(m => m.CRM_ActivityState)
                    .Where(m => m.Id == activityId).SingleOrDefault();

                context.CRM_Activity.Remove(activity);
                context.SaveChanges();
            }
        }

    }
}