using BaseSite.Data;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Services.SmsService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.Order
{
    public class OrderManager
    {
        public static Random RandomDocNumber = new Random();

        public static int Order_GenerateDocNumber()
        {
            //DocNumber format : [100000-799999]
            int num = RandomDocNumber.Next(100000, 799999);

            using (var context = new PantaEntities())
            {
                while (context.Order_Order.Any(m => m.DocNumber == num))
                {
                    num = RandomDocNumber.Next(100000, 799999);
                }
            }
            return num;
        }

        public static int Order_GenerateDocNumber(int orderDocNumber, int subNumber)
        {
            int i = 0;
            int num = orderDocNumber + ((subNumber + i) * 1000000);

            using (var context = new PantaEntities())
            {
                if (subNumber == 2)
                    while (context.Order_Cabin.Any(m => m.DocNumber == num))
                    {
                        i++;
                        num = orderDocNumber + ((subNumber + i) * 1000000);
                    }
                else if (subNumber == 3)
                    while (context.Order_Hall.Any(m => m.DocNumber == num))
                    {
                        i++;
                        num = orderDocNumber + ((subNumber + i) * 1000000);
                    }
                else if (subNumber == 4)
                    while (context.Order_DoorTop.Any(m => m.DocNumber == num))
                    {
                        i++;
                        num = orderDocNumber + ((subNumber + i) * 1000000);
                    }
                else if (subNumber == 5)
                    while (context.Delivery_Delivery.Any(m => m.DocNumber == num))
                    {
                        i++;
                        num = orderDocNumber + ((subNumber + i) * 1000000);
                    }
            }
            return num;
        }

        public static Order_Order Order_Order_Get(int orderId)
        {
            using (var context = new PantaEntities())
            {
                Order_Order order = context.Order_Order
                    .Include(m => m.Order_Cabin).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinPanels)).Include(m => m.Order_Cabin.Select(n => n.Tb_Monitors)).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinSurfaceMetals)).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinSurfaceMetals1)).Include(m => m.Order_Cabin.Select(n => n.Tb_InstallationTypes)).Include(m => m.Order_Cabin.Select(n => n.Tb_PushButtons))
                    .Include(m => m.Order_Cabin.Select(n => n.Tb_Speakers)).Include(m => m.Order_Cabin.Select(n => n.EmergencyLigh))
                    .Include(m => m.Order_Hall).Include(m => m.Order_Hall.Select(n => n.Tb_HallPanels)).Include(m => m.Order_Hall.Select(n => n.Tb_ElevatorCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_Monitors)).Include(m => m.Order_Hall.Select(n => n.Tb_HallPushButtonCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_HallSurfaceMetals)).Include(m => m.Order_Hall.Select(n => n.Tb_PushButtons))
                    .Include(m => m.Order_DoorTop).Include(m => m.Order_DoorTop.Select(n => n.Tb_DoorTopPanels)).Include(m => m.Order_DoorTop.Select(n => n.Tb_Monitors)).Include(m => m.Order_DoorTop.Select(n => n.Tb_SurfaceMetals))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Cabin.Select(n => n.Tb_CabinPanels.Order_ProductStatus))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Hall.Select(n => n.Tb_HallPanels.Order_ProductStatus))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Tb_DoorTopPanels.Order_ProductStatus))
                        .Include(m => m.Order_Deduction).Include(m => m.Order_Deduction.Select(n => n.Tb_Deductions))
                        .Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_TradeTypes)
                        .Include(m => m.Order_Status).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_PackTypes).Include(m => m.Tb_ElevatorBoards)
                        .Where(m => m.Id == orderId).SingleOrDefault();

                order.SumCostPanel = order.SumCostHall = order.SumCostDoorTop = order.SumCostAttachment = order.SumCostAddition = order.SumCostDeduction = order.SumCostTax = order.SumCostDiscountRate = 0;
                foreach (Order_Cabin c in order.Order_Cabin)
                {
                    order.SumCostPanel += c.Cost;
                    foreach (Order_Panel_Attachment a in c.Order_Panel_Attachment) order.SumCostAttachment += a.Cost;
                    foreach (Order_Panel_Addition a in c.Order_Panel_Addition) order.SumCostAddition += a.Cost;
                }
                foreach (Order_Hall h in order.Order_Hall)
                {
                    order.SumCostHall += h.Cost;
                    foreach (Order_Panel_Attachment a in h.Order_Panel_Attachment) order.SumCostAttachment += a.Cost;
                    foreach (Order_Panel_Addition a in h.Order_Panel_Addition) order.SumCostAddition += a.Cost;
                }
                foreach (Order_DoorTop d in order.Order_DoorTop)
                {
                    order.SumCostDoorTop += d.Cost;
                    foreach (Order_Panel_Attachment a in d.Order_Panel_Attachment) order.SumCostAttachment += a.Cost;
                    foreach (Order_Panel_Addition a in d.Order_Panel_Addition) order.SumCostAddition += a.Cost;
                }
                foreach (Order_Deduction d in order.Order_Deduction) order.SumCostDeduction += d.Cost;

                order.SumCostDiscountRate = (order.SumCostPanel + order.SumCostHall + order.SumCostDoorTop - order.SumCostDeduction) * (order.DiscountRate / 100);
                order.SumCostTax = (order.SumCostPanel + order.SumCostHall + order.SumCostDoorTop - order.SumCostDeduction - order.SumCostDiscountRate) * (order.Tax / 100);

                return order;
            }
        }

        public static Order_Cabin Order_Cabin_Get(int docNumber)
        {
            using (var context = new PantaEntities())
            {
                Order_Cabin c = context.Order_Cabin.Include(m => m.Order_Order).Include(m => m.Tb_CabinPanels).Include(m => m.Tb_PushButtons)
                            .Include(m => m.Order_Panel_Attachment).Include(m => m.Order_Panel_Attachment.Select(x => x.Tb_Attachments))
                            .Where(m => m.DocNumber == docNumber).SingleOrDefault();
                return c;
            }
        }

        public static Order_Hall Order_Hall_Get(int docNumber)
        {
            using (var context = new PantaEntities())
            {
                Order_Hall c = context.Order_Hall.Include(m => m.Tb_HallPanels).Where(m => m.DocNumber == docNumber).SingleOrDefault();
                return c;
            }
        }

        public static Order_DoorTop Order_DoorTop_Get(int docNumber)
        {
            using (var context = new PantaEntities())
            {
                Order_DoorTop c = context.Order_DoorTop.Include(m => m.Tb_DoorTopPanels).Where(m => m.DocNumber == docNumber).SingleOrDefault();
                return c;
            }
        }

        public static int Order_Order_GetLastFactorNumber()
        {
            using (var context = new PantaEntities())
            {
                int Order_LastFactorNumber = context.Order_Order.Max(m => m.FactorNumber);
                if (Order_LastFactorNumber < 100000) Order_LastFactorNumber = 100000;
                return Order_LastFactorNumber;
            }
        }

        public static Order_Order Order_Order_Edit(Order_Order order, string submit)
        {
            using (var context = new PantaEntities())
            {
                if (order.Id == 0)
                {
                    Order_Order neworder = new Order_Order();
                    neworder.TableId = 14;
                    neworder.DocNumber = Order_GenerateDocNumber();
                    neworder.CustomerId = order.CustomerId;
                    neworder.ClienteleName = order.ClienteleName;
                    neworder.ProjectName = order.ProjectName;
                    neworder.OrderTypeId = order.OrderTypeId;
                    neworder.ElevatorBoardId = order.ElevatorBoardId;
                    neworder.PackTypeId = order.PackTypeId;
                    neworder.DeliveryCityId = order.DeliveryCityId;
                    neworder.DeliveryAddress = order.DeliveryAddress;
                    neworder.Tax = order.Tax;
                    neworder.DiscountRate = order.DiscountRate;
                    neworder.StatusId = (byte)OrderStatus.PishFactor;
                    neworder.DateOrder = DateTime.Now;              // order.DateOrder;
                    neworder.DateDelivery = null;                   // order.DateDelivery;
                    neworder.DateFactor = null; // DateTime.Now.AddDays(10); // order.DateFactor;
                    neworder.DeliveryCost = order.DeliveryCost;
                    neworder.Cost = 0;
                    neworder.Comment = order.Comment;
                    neworder.AccepterId = order.AccepterId;
                    neworder.StoreId = order.StoreId;
                    neworder.TradeTypeId = order.TradeTypeId;

                    for (int i = 0; i < order.Order_Cabin.Count; i++)
                    {
                        Order_Cabin c = new Order_Cabin();
                        c.TableId = 15;
                        c.DocNumber = neworder.DocNumber + ((2 + i) * 1000000);
                        c.Count = order.Order_Cabin.ElementAt(i).Count;
                        c.CabinPanelId = order.Order_Cabin.ElementAt(i).CabinPanelId;
                        c.SpeakerId = order.Order_Cabin.ElementAt(i).SpeakerId;
                        c.EmergencyLightId = order.Order_Cabin.ElementAt(i).EmergencyLightId;
                        c.FloorCount = order.Order_Cabin.ElementAt(i).FloorCount;
                        c.FloorNames = order.Order_Cabin.ElementAt(i).FloorNames;
                        c.UGFloorCount = order.Order_Cabin.ElementAt(i).UGFloorCount;
                        c.UGFloorNames = order.Order_Cabin.ElementAt(i).UGFloorNames;
                        c.PushButtonId = order.Order_Cabin.ElementAt(i).PushButtonId;
                        c.SurfaceMetalId = order.Order_Cabin.ElementAt(i).SurfaceMetalId;
                        c.SurfaceMetalId2 = order.Order_Cabin.ElementAt(i).SurfaceMetalId2;
                        c.InstallationTypeId = order.Order_Cabin.ElementAt(i).InstallationTypeId;
                        c.MonitorId = order.Order_Cabin.ElementAt(i).MonitorId;
                        c.SheetNumber = order.Order_Cabin.ElementAt(i).SheetNumber;
                        c.PhoneCallButton = order.Order_Cabin.ElementAt(i).PhoneCallButton;
                        c.DO = order.Order_Cabin.ElementAt(i).DO;
                        c.DC = order.Order_Cabin.ElementAt(i).DC;
                        c.LaserCuttingText = order.Order_Cabin.ElementAt(i).LaserCuttingText;
                        c.LaserEngravingText = order.Order_Cabin.ElementAt(i).LaserEngravingText;
                        c.Comment = order.Order_Cabin.ElementAt(i).Comment;
                        c.ProductStatusId = (byte)Information.InformationManager.Cabin_Panel_Get(c.CabinPanelId).StartFrom;

                        int buttons = c.FloorCount + (c.DC ? 1 : 0) + (c.DO ? 1 : 0) + (c.PhoneCallButton ? 1 : 0) + 2;
                        if (c.CabinPanelId == 0)
                        {
                            c.CostCabinPanel = c.CostSurfaceMetal = c.CostMonitor = c.CostPushButton = c.Cost = 0;
                        }
                        else
                        {
                            c.CostCabinPanel = Cache.CabinPanels.ContainsKey(c.CabinPanelId) ? Cache.CabinPanels[c.CabinPanelId].Cost : 0;
                            c.CostSurfaceMetal = Cache.CabinSurfaceMetals.ContainsKey(c.SurfaceMetalId) ? Cache.CabinSurfaceMetals[c.SurfaceMetalId].Cost : 0;
                            c.CostMonitor = Cache.Monitors.ContainsKey(c.MonitorId) ? Cache.Monitors[c.MonitorId].Cost : 0;
                            c.CostPushButton = Cache.PushButtons.ContainsKey(c.PushButtonId) ? Cache.PushButtons[c.PushButtonId].Cost : 0;

                            c.Cost = (c.CostCabinPanel + c.CostSurfaceMetal + c.CostMonitor + (c.CostPushButton * buttons)
                               ) * c.Count;
                        }

                        for (int j = 0; j < order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.Count; j++)
                        {
                            if (c.CabinPanelId > 0 && order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId > 0)
                            {
                                Order_Panel_Attachment a = new Order_Panel_Attachment();
                                a.CabinPanelId = order.Order_Cabin.ElementAt(i).Id;
                                a.Count = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count;
                                a.AttachmentId = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId;
                                a.Cost = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Cost;

                                a.Cost = (
                                    (Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0)
                                    ) * a.Count;

                                c.Cost += a.Cost;
                                c.Order_Panel_Attachment.Add(a);
                            }
                        }

                        for (int j = 0; j < order.Order_Cabin.ElementAt(i).Order_Panel_Addition.Count; j++)
                        {
                            if (c.CabinPanelId > 0 && order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId > 0)
                            {
                                Order_Panel_Addition a = new Order_Panel_Addition();
                                a.CabinPanelId = order.Order_Cabin.ElementAt(i).Id;
                                a.AdditionId = order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId;
                                a.Cost = order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost;

                                c.Cost += a.Cost;
                                c.Order_Panel_Addition.Add(a);
                            }
                        }
                        neworder.Cost += c.Cost;
                        neworder.Order_Cabin.Add(c);
                    }

                    for (int i = 0; i < order.Order_Hall.Count; i++)
                    {
                        Order_Hall h = new Order_Hall();
                        h.TableId = 16;
                        h.DocNumber = neworder.DocNumber + ((3 + i) * 1000000);
                        h.Count = order.Order_Hall.ElementAt(i).Count;
                        h.ElevatorTypeId = order.Order_Hall.ElementAt(i).ElevatorTypeId;
                        h.PushButtonCountId = order.Order_Hall.ElementAt(i).PushButtonCountId;
                        h.HallPanelId = order.Order_Hall.ElementAt(i).HallPanelId;
                        h.PushButtonId = order.Order_Hall.ElementAt(i).PushButtonId;
                        h.SurfaceMetalId = order.Order_Hall.ElementAt(i).SurfaceMetalId;
                        h.MonitorId = order.Order_Hall.ElementAt(i).MonitorId;
                        h.FloorCount = order.Order_Hall.ElementAt(i).FloorCount;
                        h.FloorNames = order.Order_Hall.ElementAt(i).FloorNames;
                        h.UGFloorCount = order.Order_Hall.ElementAt(i).UGFloorCount;
                        h.UGFloorNames = order.Order_Hall.ElementAt(i).UGFloorNames;
                        h.Comment = order.Order_Hall.ElementAt(i).Comment;
                        h.ProductStatusId = (byte)Information.InformationManager.Hall_Panel_Get(h.HallPanelId).StartFrom;

                        if (h.HallPanelId == 0)
                        {
                            h.CostHallPanel = h.CostSurfaceMetal = h.CostMonitor = h.CostPushButton = h.Cost = 0;
                        }
                        else
                        {
                            h.CostHallPanel = Cache.HallPanels.ContainsKey(h.HallPanelId) ? Cache.HallPanels[h.HallPanelId].Cost : 0;
                            h.CostSurfaceMetal = Cache.HallSurfaceMetals.ContainsKey(h.SurfaceMetalId) ? Cache.HallSurfaceMetals[h.SurfaceMetalId].Cost : 0;
                            h.CostMonitor = Cache.Monitors.ContainsKey(h.MonitorId) ? Cache.Monitors[h.MonitorId].Cost : 0;
                            h.CostPushButton = Cache.PushButtons.ContainsKey(h.PushButtonId) ? Cache.PushButtons[h.PushButtonId].Cost : 0;

                            h.Cost = (
                               h.CostHallPanel + h.CostSurfaceMetal * h.ElevatorTypeId + h.CostMonitor * h.ElevatorTypeId +
                               (h.CostPushButton * h.PushButtonCountId)
                               ) * h.Count;
                        }

                        for (int j = 0; j < order.Order_Hall.ElementAt(i).Order_Panel_Attachment.Count; j++)
                        {
                            if (h.HallPanelId > 0 && order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId > 0)
                            {
                                Order_Panel_Attachment a = new Order_Panel_Attachment();
                                a.HallPanelId = order.Order_Hall.ElementAt(i).Id;
                                a.Count = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count;
                                a.AttachmentId = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId;
                                a.Cost = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Cost;

                                a.Cost = (
                                    (Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0)
                                    ) * a.Count;

                                h.Cost += a.Cost;
                                h.Order_Panel_Attachment.Add(a);
                            }
                        }

                        for (int j = 0; j < order.Order_Hall.ElementAt(i).Order_Panel_Addition.Count; j++)
                        {
                            if (h.HallPanelId > 0 && order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId > 0)
                            {
                                Order_Panel_Addition a = new Order_Panel_Addition();
                                a.HallPanelId = order.Order_Hall.ElementAt(i).Id;
                                a.AdditionId = order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId;
                                a.Cost = order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost;

                                h.Cost += a.Cost;
                                h.Order_Panel_Addition.Add(a);
                            }
                        }

                        neworder.Cost += h.Cost;
                        neworder.Order_Hall.Add(h);
                    }

                    for (int i = 0; i < order.Order_DoorTop.Count; i++)
                    {
                        Order_DoorTop d = new Order_DoorTop();
                        d.TableId = 17;
                        d.DocNumber = neworder.DocNumber + ((4 + i) * 1000000);
                        d.Count = order.Order_DoorTop.ElementAt(i).Count;
                        d.DoorTopPanelId = order.Order_DoorTop.ElementAt(i).DoorTopPanelId;
                        d.MonitorId = order.Order_DoorTop.ElementAt(i).MonitorId;
                        d.SurfaceMetalId = order.Order_DoorTop.ElementAt(i).SurfaceMetalId;
                        d.Comment = order.Order_DoorTop.ElementAt(i).Comment;
                        d.ProductStatusId = (byte)Information.InformationManager.DoorTop_Panel_Get(d.DoorTopPanelId).StartFrom;

                        if (d.DoorTopPanelId == 0)
                        {
                            d.CostDoorTopPanel = d.CostMonitor = d.CostSurfaceMetal = d.SurfaceMetalDosage = d.Cost = 0;
                        }
                        else
                        {
                            d.CostDoorTopPanel = Cache.DoorTopPanels.ContainsKey(d.DoorTopPanelId) ? Cache.DoorTopPanels[d.DoorTopPanelId].Cost : 0;
                            d.CostMonitor = Cache.Monitors.ContainsKey(d.MonitorId) ? Cache.Monitors[d.MonitorId].Cost : 0;
                            d.CostSurfaceMetal = Cache.SurfaceMetals.ContainsKey(d.SurfaceMetalId) ? Cache.SurfaceMetals[d.SurfaceMetalId].Cost : 0;
                            d.SurfaceMetalDosage = Cache.DoorTopPanels.ContainsKey(d.DoorTopPanelId) ? Cache.DoorTopPanels[d.DoorTopPanelId].val1 : 0;

                            d.Cost = (d.CostDoorTopPanel + d.CostMonitor + (d.SurfaceMetalId > Cache.SurfaceMetalsStartId ? Math.Round(d.CostSurfaceMetal * d.SurfaceMetalDosage) : d.CostSurfaceMetal)) * d.Count;
                        }

                        for (int j = 0; j < order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.Count; j++)
                        {
                            if (d.DoorTopPanelId > 0 && order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId > 0)
                            {
                                Order_Panel_Attachment a = new Order_Panel_Attachment();
                                a.DoorTopPanelId = order.Order_DoorTop.ElementAt(i).Id;
                                a.Count = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count;
                                a.AttachmentId = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId;
                                a.Cost = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Cost;

                                a.Cost = (
                                    (Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0)
                                    ) * a.Count;

                                d.Cost += a.Cost;
                                d.Order_Panel_Attachment.Add(a);
                            }
                        }

                        for (int j = 0; j < order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.Count; j++)
                        {
                            if (d.DoorTopPanelId > 0 && order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId > 0)
                            {
                                Order_Panel_Addition a = new Order_Panel_Addition();
                                a.DoorTopPanelId = order.Order_DoorTop.ElementAt(i).Id;
                                a.AdditionId = order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId;
                                a.Cost = order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost;

                                d.Cost += a.Cost;
                                d.Order_Panel_Addition.Add(a);
                            }
                        }

                        neworder.Cost += d.Cost;
                        neworder.Order_DoorTop.Add(d);
                    }

                    for (int i = 0; i < order.Order_Deduction.Count; i++)
                    {
                        if (order.Order_Deduction.ElementAt(i).DeductionId > 0)
                        {
                            Order_Deduction a = new Order_Deduction();
                            a.DeductionId = order.Order_Deduction.ElementAt(i).DeductionId;
                            a.Cost = order.Order_Deduction.ElementAt(i).Cost;

                            neworder.Cost -= a.Cost;
                            neworder.Order_Deduction.Add(a);
                        }
                    }

                    neworder.Cost -= (neworder.Cost * (neworder.DiscountRate / 100));
                    neworder.Cost += (neworder.Cost * (neworder.Tax / 100));
                    neworder.Cost += order.DeliveryCost.HasValue ? order.DeliveryCost.Value : 0;
                    context.Order_Order.Add(neworder);
                    context.SaveChanges();
                    return Order_Order_Get(neworder.Id);
                }
                else
                {
                    //context.Entry<Order_Order>(order).State = System.Data.EntityState.Modified;
                    //context.Order_Order.Attach(order);
                    //var entry = context.Entry(order);

                    //entry.Property(e => e.CustomerId).IsModified = true;
                    //entry.Property(e => e.ClienteleName).IsModified = true;
                    //entry.Property(e => e.ProjectName).IsModified = true;
                    //entry.Property(e => e.OrderTypeId).IsModified = true;
                    //entry.Property(e => e.ElevatorBoardId).IsModified = true;
                    //entry.Property(e => e.PackTypeId).IsModified = true;
                    //entry.Property(e => e.DeliveryCityId).IsModified = true;
                    //entry.Property(e => e.DeliveryAddress).IsModified = true;
                    //entry.Property(e => e.Tax).IsModified = true;
                    //entry.Property(e => e.Comment).IsModified = true;

                    //entry.Property(e => e.Order_Cabin).IsModified = true;
                    //entry.Property(e => e.Order_Hall).IsModified = true;
                    //entry.Property(e => e.Order_DoorTop).IsModified = true;
                    //entry.Property(e => e.Order_Attachment).IsModified = true;
                    //entry.Property(e => e.Order_Deduction).IsModified = true;
                    //entry.Property(e => e.Order_Addition).IsModified = true;

                    //Order_Order neworder = Order_Order_Get(order.Id);
                    //////////////////////////////////////////// Get Order ////////////////////////////////////
                    Order_Order neworder = context.Order_Order
                    .Include(m => m.Order_Cabin).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinPanels)).Include(m => m.Order_Cabin.Select(n => n.Tb_Monitors)).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinSurfaceMetals)).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinSurfaceMetals1)).Include(m => m.Order_Cabin.Select(n => n.Tb_InstallationTypes)).Include(m => m.Order_Cabin.Select(n => n.Tb_PushButtons))
                    .Include(m => m.Order_Cabin.Select(n => n.Tb_Speakers)).Include(m => m.Order_Cabin.Select(n => n.EmergencyLigh))
                    .Include(m => m.Order_Hall).Include(m => m.Order_Hall.Select(n => n.Tb_HallPanels)).Include(m => m.Order_Hall.Select(n => n.Tb_ElevatorCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_Monitors)).Include(m => m.Order_Hall.Select(n => n.Tb_HallPushButtonCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_HallSurfaceMetals)).Include(m => m.Order_Hall.Select(n => n.Tb_PushButtons))
                    .Include(m => m.Order_DoorTop).Include(m => m.Order_DoorTop.Select(n => n.Tb_DoorTopPanels)).Include(m => m.Order_DoorTop.Select(n => n.Tb_Monitors)).Include(m => m.Order_DoorTop.Select(n => n.Tb_SurfaceMetals))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Deduction).Include(m => m.Order_Deduction.Select(n => n.Tb_Deductions)).Include(m => m.Account_Users).Include(m => m.Account_Users1)
                        .Include(m => m.Order_Status).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_PackTypes).Include(m => m.Tb_ElevatorBoards)
                        .Where(m => m.Id == order.Id).SingleOrDefault();
                    //////////////////////////////////////////////////////////////////////////////////////////

                    neworder.CustomerId = order.CustomerId;
                    neworder.ClienteleName = order.ClienteleName;
                    neworder.ProjectName = order.ProjectName;
                    neworder.OrderTypeId = order.OrderTypeId;
                    neworder.ElevatorBoardId = order.ElevatorBoardId;
                    neworder.PackTypeId = order.PackTypeId;
                    neworder.DeliveryCityId = order.DeliveryCityId;
                    neworder.DeliveryAddress = order.DeliveryAddress;
                    neworder.Tax = order.Tax;
                    neworder.DiscountRate = order.DiscountRate;
                    neworder.StatusId = order.StatusId; //(byte)OrderStatus.PishFactor;
                    /*if (neworder.StatusId <= (byte)OrderStatus.DarDasteEghdam)
                    {
                        neworder.DateOrder = DateTime.Now;              // order.DateOrder;
                        neworder.DateDelivery = null;                   // order.DateDelivery;
                        neworder.DateFactor = DateTime.Now.AddDays(10); // order.DateFactor;
                    }
                    else*/
                    if (neworder.StatusId == (byte)OrderStatus.DarkhasteTolid)
                    {
                        neworder.DateDelivery = DateTime.Now;
                        neworder.DateFactor = DateTime.Now.AddDays(10);

                        try
                        {
                            var customer = AccountManager.Account_User_Get(neworder.CustomerId);
                            string mobile = customer.GetMobile();
                            if (!string.IsNullOrEmpty(mobile))
                            {
                                SmsKavenegar sk = new SmsKavenegar();
                                sk.SendSms(mobile, neworder.DocNumber.ToString(), "DarkhasteTolid", "", "", customer.FullName);
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        //neworder.DateOrder = order.DateOrder;
                        //neworder.DateDelivery = order.DateDelivery;
                        //neworder.DateFactor = order.DateFactor;
                    }
                    neworder.DeliveryCost = order.DeliveryCost;
                    neworder.Cost = 0;
                    neworder.Comment = order.Comment;
                    neworder.StoreId = order.StoreId;
                    neworder.TradeTypeId = order.TradeTypeId;

                    for (int i = 0; i < order.Order_Cabin.Count; i++)
                    {
                        Order_Cabin c = neworder.Order_Cabin.Where(m => m.Id == order.Order_Cabin.ElementAt(i).Id).SingleOrDefault();

                        if (neworder.StatusId >= (byte)OrderStatus.DarkhasteTolid && order.Order_Cabin.ElementAt(i).CabinPanelId == 0)
                        {
                            //Delete cabin panel
                            for (int j = 0; j < c.Order_Panel_Attachment.Count; j++)
                            {
                                Order_Panel_Attachment a = c.Order_Panel_Attachment.ElementAt(j);
                                context.Entry(a).State = System.Data.EntityState.Deleted;
                                j--;
                            }
                            for (int j = 0; j < c.Order_Panel_Addition.Count; j++)
                            {
                                Order_Panel_Addition a = c.Order_Panel_Addition.ElementAt(j);
                                context.Entry(a).State = System.Data.EntityState.Deleted;
                                j--;
                            }
                            context.Entry(c).State = System.Data.EntityState.Deleted;
                        }
                        else
                        {
                            //Edit cabin panel
                            c.Count = order.Order_Cabin.ElementAt(i).Count;
                            c.CabinPanelId = order.Order_Cabin.ElementAt(i).CabinPanelId;
                            c.SpeakerId = order.Order_Cabin.ElementAt(i).SpeakerId;
                            c.EmergencyLightId = order.Order_Cabin.ElementAt(i).EmergencyLightId;
                            c.FloorCount = order.Order_Cabin.ElementAt(i).FloorCount;
                            c.FloorNames = order.Order_Cabin.ElementAt(i).FloorNames;
                            c.UGFloorCount = order.Order_Cabin.ElementAt(i).UGFloorCount;
                            c.UGFloorNames = order.Order_Cabin.ElementAt(i).UGFloorNames;
                            c.PushButtonId = order.Order_Cabin.ElementAt(i).PushButtonId;
                            c.SurfaceMetalId = order.Order_Cabin.ElementAt(i).SurfaceMetalId;
                            c.SurfaceMetalId2 = order.Order_Cabin.ElementAt(i).SurfaceMetalId2;
                            c.InstallationTypeId = order.Order_Cabin.ElementAt(i).InstallationTypeId;
                            c.MonitorId = order.Order_Cabin.ElementAt(i).MonitorId;
                            c.SheetNumber = order.Order_Cabin.ElementAt(i).SheetNumber;
                            c.PhoneCallButton = order.Order_Cabin.ElementAt(i).PhoneCallButton;
                            c.DO = order.Order_Cabin.ElementAt(i).DO;
                            c.DC = order.Order_Cabin.ElementAt(i).DC;
                            c.ProductStatusId = (byte)Information.InformationManager.Cabin_Panel_Get(c.CabinPanelId).StartFrom;
                            c.LaserCuttingText = order.Order_Cabin.ElementAt(i).LaserCuttingText;
                            c.LaserEngravingText = order.Order_Cabin.ElementAt(i).LaserEngravingText;
                            c.Comment = order.Order_Cabin.ElementAt(i).Comment;

                            int buttons = c.FloorCount + (c.DC ? 1 : 0) + (c.DO ? 1 : 0) + (c.PhoneCallButton ? 1 : 0) + 2;
                            if (c.CabinPanelId == 0)
                            {
                                c.CostCabinPanel = c.CostSurfaceMetal = c.CostMonitor = c.CostPushButton = c.Cost = 0;
                            }
                            else
                            {
                                if (neworder.StatusId == (byte)OrderStatus.PishFactor)
                                {
                                    c.CostCabinPanel = Cache.CabinPanels.ContainsKey(c.CabinPanelId) ? Cache.CabinPanels[c.CabinPanelId].Cost : 0;
                                    c.CostSurfaceMetal = Cache.CabinSurfaceMetals.ContainsKey(c.SurfaceMetalId) ? Cache.CabinSurfaceMetals[c.SurfaceMetalId].Cost : 0;
                                    c.CostMonitor = Cache.Monitors.ContainsKey(c.MonitorId) ? Cache.Monitors[c.MonitorId].Cost : 0;
                                    c.CostPushButton = Cache.PushButtons.ContainsKey(c.PushButtonId) ? Cache.PushButtons[c.PushButtonId].Cost : 0;
                                }
                                else if (neworder.StatusId == (byte)OrderStatus.DarDasteEghdam)
                                {
                                    if (c.CabinPanelId != order.Order_Cabin.ElementAt(i).CabinPanelId)
                                        c.CostCabinPanel = Cache.CabinPanels.ContainsKey(c.CabinPanelId) ? Cache.CabinPanels[c.CabinPanelId].Cost : 0;
                                    if (c.SurfaceMetalId != order.Order_Cabin.ElementAt(i).SurfaceMetalId)
                                        c.CostSurfaceMetal = Cache.CabinSurfaceMetals.ContainsKey(c.SurfaceMetalId) ? Cache.CabinSurfaceMetals[c.SurfaceMetalId].Cost : 0;
                                    if (c.MonitorId != order.Order_Cabin.ElementAt(i).MonitorId)
                                        c.CostMonitor = Cache.Monitors.ContainsKey(c.MonitorId) ? Cache.Monitors[c.MonitorId].Cost : 0;
                                    if (c.PushButtonId != order.Order_Cabin.ElementAt(i).PushButtonId)
                                        c.CostPushButton = Cache.PushButtons.ContainsKey(c.PushButtonId) ? Cache.PushButtons[c.PushButtonId].Cost : 0;
                                }

                                c.Cost = (c.CostCabinPanel + c.CostSurfaceMetal + c.CostMonitor + (c.CostPushButton * buttons)
                                   ) * c.Count;
                            }

                            for (int j = 0; j < order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.Count; j++)
                            {
                                Order_Panel_Attachment a = null;
                                if (c.CabinPanelId > 0 && order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id == 0 && order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId > 0)
                                {
                                    // ایجاد درصورتی که نامعلوم نباشد
                                    a = new Order_Panel_Attachment()
                                    {
                                        CabinPanelId = c.Id,
                                        Count = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count,
                                        AttachmentId = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId,
                                        Cost = 0
                                    };
                                    a.Cost = (
                                   (Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0)
                                   ) * a.Count;

                                    c.Cost += a.Cost;
                                    c.Order_Panel_Attachment.Add(a);
                                }
                                else if (order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id > 0)
                                {
                                    a = c.Order_Panel_Attachment.Where(m => m.Id == order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id).SingleOrDefault();
                                    if (c.CabinPanelId == 0 || order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId == 0) // حذف نامعلوم
                                    {
                                        c.Order_Panel_Attachment.Remove(a);
                                    }
                                    else // ویرایش
                                    {
                                        double attachmentPhi = a.Count > 0 ? a.Cost / a.Count : 0;
                                        if (neworder.StatusId == (byte)OrderStatus.PishFactor || (neworder.StatusId == (byte)OrderStatus.DarDasteEghdam && a.AttachmentId != order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId))
                                        {
                                            a.AttachmentId = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId;
                                            attachmentPhi = Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0;
                                        }

                                        a.CabinPanelId = c.Id;
                                        a.Count = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count;
                                        a.Cost = order.Order_Cabin.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Cost;

                                        a.Cost = attachmentPhi * a.Count;

                                        c.Cost += a.Cost;
                                        c.Order_Panel_Attachment.Add(a);
                                    }
                                }
                            }
                            for (int j = 0; j < order.Order_Cabin.ElementAt(i).Order_Panel_Addition.Count; j++)
                            {
                                Order_Panel_Addition a = null;
                                if (c.CabinPanelId > 0 && order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id == 0 && order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId > 0)
                                {
                                    // ایجاد درصورتی که نامعلوم نباشد
                                    a = new Order_Panel_Addition()
                                    {
                                        CabinPanelId = c.Id,
                                        AdditionId = order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId,
                                        Cost = order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost
                                    };

                                    c.Cost += a.Cost;
                                    c.Order_Panel_Addition.Add(a);
                                }
                                else if (order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id > 0)
                                {
                                    a = c.Order_Panel_Addition.Where(m => m.Id == order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id).SingleOrDefault();
                                    if (c.CabinPanelId == 0 || order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId == 0) // حذف نامعلوم
                                    {
                                        c.Order_Panel_Addition.Remove(a);
                                    }
                                    else // ویرایش
                                    {
                                        a.CabinPanelId = c.Id;
                                        a.AdditionId = order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId;
                                        a.Cost = order.Order_Cabin.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost;

                                        c.Cost += a.Cost;
                                        c.Order_Panel_Addition.Add(a);
                                    }
                                }
                            }

                            neworder.Cost += c.Cost;
                            neworder.Order_Cabin.Add(c);
                        }
                    }

                    for (int i = 0; i < order.Order_Hall.Count; i++)
                    {
                        Order_Hall h = neworder.Order_Hall.Where(m => m.Id == order.Order_Hall.ElementAt(i).Id).SingleOrDefault();
                        if (neworder.StatusId >= (byte)OrderStatus.DarkhasteTolid && order.Order_Hall.ElementAt(i).HallPanelId == 0)
                        {
                            //Delete hall panel
                            for (int j = 0; j < h.Order_Panel_Attachment.Count; j++)
                            {
                                Order_Panel_Attachment a = h.Order_Panel_Attachment.ElementAt(j);
                                context.Entry(a).State = System.Data.EntityState.Deleted;
                                j--;
                            }
                            for (int j = 0; j < h.Order_Panel_Addition.Count; j++)
                            {
                                Order_Panel_Addition a = h.Order_Panel_Addition.ElementAt(j);
                                context.Entry(a).State = System.Data.EntityState.Deleted;
                                j--;
                            }
                            context.Entry(h).State = System.Data.EntityState.Deleted;
                        }
                        else
                        {
                            //Edit hall panel
                            h.Count = order.Order_Hall.ElementAt(i).Count;
                            h.ElevatorTypeId = order.Order_Hall.ElementAt(i).ElevatorTypeId;
                            h.PushButtonCountId = order.Order_Hall.ElementAt(i).PushButtonCountId;
                            h.HallPanelId = order.Order_Hall.ElementAt(i).HallPanelId;
                            h.PushButtonId = order.Order_Hall.ElementAt(i).PushButtonId;
                            h.SurfaceMetalId = order.Order_Hall.ElementAt(i).SurfaceMetalId;
                            h.MonitorId = order.Order_Hall.ElementAt(i).MonitorId;
                            h.FloorCount = order.Order_Hall.ElementAt(i).FloorCount;
                            h.FloorNames = order.Order_Hall.ElementAt(i).FloorNames;
                            h.UGFloorCount = order.Order_Hall.ElementAt(i).UGFloorCount;
                            h.UGFloorNames = order.Order_Hall.ElementAt(i).UGFloorNames;
                            h.ProductStatusId = (byte)Information.InformationManager.Hall_Panel_Get(h.HallPanelId).StartFrom;
                            h.Comment = order.Order_Hall.ElementAt(i).Comment;

                            if (h.HallPanelId == 0)
                            {
                                h.CostHallPanel = h.CostSurfaceMetal = h.CostMonitor = h.CostPushButton = h.Cost = 0;
                            }
                            else
                            {
                                if (neworder.StatusId == (byte)OrderStatus.PishFactor)
                                {
                                    h.CostHallPanel = Cache.HallPanels.ContainsKey(h.HallPanelId) ? Cache.HallPanels[h.HallPanelId].Cost : 0;
                                    h.CostSurfaceMetal = Cache.HallSurfaceMetals.ContainsKey(h.SurfaceMetalId) ? Cache.HallSurfaceMetals[h.SurfaceMetalId].Cost : 0;
                                    h.CostMonitor = Cache.Monitors.ContainsKey(h.MonitorId) ? Cache.Monitors[h.MonitorId].Cost : 0;
                                    h.CostPushButton = Cache.PushButtons.ContainsKey(h.PushButtonId) ? Cache.PushButtons[h.PushButtonId].Cost : 0;
                                }
                                else if (neworder.StatusId == (byte)OrderStatus.DarDasteEghdam)
                                {
                                    if (h.HallPanelId != order.Order_Hall.ElementAt(i).HallPanelId)
                                        h.CostHallPanel = Cache.HallPanels.ContainsKey(h.HallPanelId) ? Cache.HallPanels[h.HallPanelId].Cost : 0;
                                    if (h.SurfaceMetalId != order.Order_Hall.ElementAt(i).SurfaceMetalId)
                                        h.CostSurfaceMetal = Cache.HallSurfaceMetals.ContainsKey(h.SurfaceMetalId) ? Cache.HallSurfaceMetals[h.SurfaceMetalId].Cost : 0;
                                    if (h.MonitorId != order.Order_Hall.ElementAt(i).MonitorId)
                                        h.CostMonitor = Cache.Monitors.ContainsKey(h.MonitorId) ? Cache.Monitors[h.MonitorId].Cost : 0;
                                    if (h.PushButtonId != order.Order_Hall.ElementAt(i).PushButtonId)
                                        h.CostPushButton = Cache.PushButtons.ContainsKey(h.PushButtonId) ? Cache.PushButtons[h.PushButtonId].Cost : 0;
                                }

                                h.Cost = (
                                   h.CostHallPanel + h.CostSurfaceMetal * h.ElevatorTypeId + h.CostMonitor * h.ElevatorTypeId +
                                   (h.CostPushButton * h.PushButtonCountId)
                                   ) * h.Count;
                            }

                            for (int j = 0; j < order.Order_Hall.ElementAt(i).Order_Panel_Attachment.Count; j++)
                            {
                                Order_Panel_Attachment a = null;
                                if (h.HallPanelId > 0 && order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id == 0 && order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId > 0)
                                {
                                    // ایجاد درصورتی که نامعلوم نباشد
                                    a = new Order_Panel_Attachment()
                                    {
                                        HallPanelId = h.Id,
                                        Count = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count,
                                        AttachmentId = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId,
                                        Cost = 0
                                    };
                                    a.Cost = (
                                   (Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0)
                                   ) * a.Count;

                                    h.Cost += a.Cost;
                                    h.Order_Panel_Attachment.Add(a);
                                }
                                else if (order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id > 0)
                                {
                                    a = h.Order_Panel_Attachment.Where(m => m.Id == order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id).SingleOrDefault();
                                    if (h.HallPanelId == 0 || order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId == 0) // حذف نامعلوم
                                    {
                                        h.Order_Panel_Attachment.Remove(a);
                                    }
                                    else // ویرایش
                                    {
                                        double attachmentPhi = a.Count > 0 ? a.Cost / a.Count : 0;
                                        if (neworder.StatusId == (byte)OrderStatus.PishFactor || (neworder.StatusId == (byte)OrderStatus.DarDasteEghdam && a.AttachmentId != order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId))
                                        {
                                            a.AttachmentId = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId;
                                            attachmentPhi = Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0;
                                        }

                                        a.HallPanelId = h.Id;
                                        a.Count = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count;
                                        a.Cost = order.Order_Hall.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Cost;

                                        a.Cost = attachmentPhi * a.Count;

                                        h.Cost += a.Cost;
                                        h.Order_Panel_Attachment.Add(a);
                                    }
                                }
                            }
                            for (int j = 0; j < order.Order_Hall.ElementAt(i).Order_Panel_Addition.Count; j++)
                            {
                                Order_Panel_Addition a = null;
                                if (h.HallPanelId > 0 && order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id == 0 && order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId > 0)
                                {
                                    // ایجاد درصورتی که نامعلوم نباشد
                                    a = new Order_Panel_Addition()
                                    {
                                        HallPanelId = h.Id,
                                        AdditionId = order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId,
                                        Cost = order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost
                                    };

                                    h.Cost += a.Cost;
                                    h.Order_Panel_Addition.Add(a);
                                }
                                else if (order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id > 0)
                                {
                                    a = h.Order_Panel_Addition.Where(m => m.Id == order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id).SingleOrDefault();
                                    if (h.HallPanelId == 0 || order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId == 0) // حذف نامعلوم
                                    {
                                        h.Order_Panel_Addition.Remove(a);
                                    }
                                    else // ویرایش
                                    {
                                        a.HallPanelId = h.Id;
                                        a.AdditionId = order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId;
                                        a.Cost = order.Order_Hall.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost;

                                        h.Cost += a.Cost;
                                        h.Order_Panel_Addition.Add(a);
                                    }
                                }
                            }

                            neworder.Cost += h.Cost;
                            neworder.Order_Hall.Add(h);
                        }
                    }

                    for (int i = 0; i < order.Order_DoorTop.Count; i++)
                    {
                        Order_DoorTop d = neworder.Order_DoorTop.Where(m => m.Id == order.Order_DoorTop.ElementAt(i).Id).SingleOrDefault();
                        if (neworder.StatusId >= (byte)OrderStatus.DarkhasteTolid && order.Order_DoorTop.ElementAt(i).DoorTopPanelId == 0)
                        {
                            ////Delete doortop panel
                            for (int j = 0; j < d.Order_Panel_Attachment.Count; j++)
                            {
                                Order_Panel_Attachment a = d.Order_Panel_Attachment.ElementAt(j);
                                context.Entry(a).State = System.Data.EntityState.Deleted;
                                j--;
                            }
                            for (int j = 0; j < d.Order_Panel_Addition.Count; j++)
                            {
                                Order_Panel_Addition a = d.Order_Panel_Addition.ElementAt(j);
                                context.Entry(a).State = System.Data.EntityState.Deleted;
                                j--;
                            }
                            context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                        else
                        {
                            //Edit doortop panel
                            d.Count = order.Order_DoorTop.ElementAt(i).Count;
                            d.DoorTopPanelId = order.Order_DoorTop.ElementAt(i).DoorTopPanelId;
                            d.MonitorId = order.Order_DoorTop.ElementAt(i).MonitorId;
                            d.SurfaceMetalId = order.Order_DoorTop.ElementAt(i).SurfaceMetalId;
                            d.ProductStatusId = (byte)Information.InformationManager.DoorTop_Panel_Get(d.DoorTopPanelId).StartFrom;
                            d.Comment = order.Order_DoorTop.ElementAt(i).Comment;

                            if (d.DoorTopPanelId == 0)
                            {
                                d.CostDoorTopPanel = d.CostMonitor = d.CostSurfaceMetal = d.SurfaceMetalDosage = d.Cost = 0;
                            }
                            else
                            {
                                if (neworder.StatusId == (byte)OrderStatus.PishFactor)
                                {
                                    d.CostDoorTopPanel = Cache.DoorTopPanels.ContainsKey(d.DoorTopPanelId) ? Cache.DoorTopPanels[d.DoorTopPanelId].Cost : 0;
                                    d.CostMonitor = Cache.Monitors.ContainsKey(d.MonitorId) ? Cache.Monitors[d.MonitorId].Cost : 0;
                                    d.CostSurfaceMetal = Cache.SurfaceMetals.ContainsKey(d.SurfaceMetalId) ? Cache.SurfaceMetals[d.SurfaceMetalId].Cost : 0;
                                    d.SurfaceMetalDosage = Cache.DoorTopPanels.ContainsKey(d.DoorTopPanelId) ? Cache.DoorTopPanels[d.DoorTopPanelId].val1 : 0;
                                }
                                else if (neworder.StatusId == (byte)OrderStatus.DarDasteEghdam)
                                {
                                    if (d.DoorTopPanelId != order.Order_DoorTop.ElementAt(i).DoorTopPanelId)
                                    {
                                        d.CostDoorTopPanel = Cache.DoorTopPanels.ContainsKey(d.DoorTopPanelId) ? Cache.DoorTopPanels[d.DoorTopPanelId].Cost : 0;
                                        d.SurfaceMetalDosage = Cache.DoorTopPanels.ContainsKey(d.DoorTopPanelId) ? Cache.DoorTopPanels[d.DoorTopPanelId].val1 : 0;
                                    }
                                    if (d.MonitorId != order.Order_DoorTop.ElementAt(i).MonitorId)
                                        d.CostMonitor = Cache.Monitors.ContainsKey(d.MonitorId) ? Cache.Monitors[d.MonitorId].Cost : 0;
                                    if (d.SurfaceMetalId != order.Order_DoorTop.ElementAt(i).SurfaceMetalId)
                                        d.CostSurfaceMetal = Cache.SurfaceMetals.ContainsKey(d.SurfaceMetalId) ? Cache.SurfaceMetals[d.SurfaceMetalId].Cost : 0;
                                }

                                d.Cost = (d.CostDoorTopPanel + d.CostMonitor + (d.SurfaceMetalId > Cache.SurfaceMetalsStartId ? Math.Round(d.CostSurfaceMetal * d.SurfaceMetalDosage) : d.CostSurfaceMetal)) * d.Count;
                            }

                            for (int j = 0; j < order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.Count; j++)
                            {
                                Order_Panel_Attachment a = null;
                                if (d.DoorTopPanelId > 0 && order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id == 0 && order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId > 0)
                                {
                                    // ایجاد درصورتی که نامعلوم نباشد
                                    a = new Order_Panel_Attachment()
                                    {
                                        DoorTopPanelId = d.Id,
                                        Count = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count,
                                        AttachmentId = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId,
                                        Cost = 0
                                    };
                                    a.Cost = (
                                   (Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0)
                                   ) * a.Count;

                                    d.Cost += a.Cost;
                                    d.Order_Panel_Attachment.Add(a);
                                }
                                else if (order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id > 0)
                                {
                                    a = d.Order_Panel_Attachment.Where(m => m.Id == order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Id).SingleOrDefault();
                                    if (d.DoorTopPanelId == 0 || order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId == 0) // حذف نامعلوم
                                    {
                                        d.Order_Panel_Attachment.Remove(a);
                                    }
                                    else // ویرایش
                                    {
                                        double attachmentPhi = a.Count > 0 ? a.Cost / a.Count : 0;
                                        if (neworder.StatusId == (byte)OrderStatus.PishFactor || (neworder.StatusId == (byte)OrderStatus.DarDasteEghdam && a.AttachmentId != order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId))
                                        {
                                            a.AttachmentId = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).AttachmentId;
                                            attachmentPhi = Cache.Order_Attachments.ContainsKey(a.AttachmentId) ? Cache.Order_Attachments[a.AttachmentId].Cost : 0;
                                        }

                                        a.DoorTopPanelId = d.Id;
                                        a.Count = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Count;
                                        a.Cost = order.Order_DoorTop.ElementAt(i).Order_Panel_Attachment.ElementAt(j).Cost;

                                        a.Cost = attachmentPhi * a.Count;

                                        d.Cost += a.Cost;
                                        d.Order_Panel_Attachment.Add(a);
                                    }
                                }
                            }
                            for (int j = 0; j < order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.Count; j++)
                            {
                                Order_Panel_Addition a = null;
                                if (d.DoorTopPanelId > 0 && order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id == 0 && order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId > 0)
                                {
                                    // ایجاد درصورتی که نامعلوم نباشد
                                    a = new Order_Panel_Addition()
                                    {
                                        DoorTopPanelId = d.Id,
                                        AdditionId = order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId,
                                        Cost = order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost
                                    };

                                    d.Cost += a.Cost;
                                    d.Order_Panel_Addition.Add(a);
                                }
                                else if (order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id > 0)
                                {
                                    a = d.Order_Panel_Addition.Where(m => m.Id == order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).Id).SingleOrDefault();
                                    if (d.DoorTopPanelId == 0 || order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId == 0) // حذف نامعلوم
                                    {
                                        d.Order_Panel_Addition.Remove(a);
                                    }
                                    else // ویرایش
                                    {
                                        a.DoorTopPanelId = d.Id;
                                        a.AdditionId = order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).AdditionId;
                                        a.Cost = order.Order_DoorTop.ElementAt(i).Order_Panel_Addition.ElementAt(j).Cost;

                                        d.Cost += a.Cost;
                                        d.Order_Panel_Addition.Add(a);
                                    }
                                }
                            }

                            neworder.Cost += d.Cost;
                            neworder.Order_DoorTop.Add(d);
                        }
                    }

                    for (int j = 0; j < order.Order_Deduction.Count; j++)
                    {
                        Order_Deduction a = null;
                        if (order.Order_Deduction.ElementAt(j).Id == 0 && order.Order_Deduction.ElementAt(j).DeductionId > 0)
                        {
                            // ایجاد درصورتی که نامعلوم نباشد
                            a = new Order_Deduction()
                            {
                                DeductionId = order.Order_Deduction.ElementAt(j).DeductionId,
                                Cost = order.Order_Deduction.ElementAt(j).Cost
                            };
                            neworder.Cost -= a.Cost;
                            neworder.Order_Deduction.Add(a);
                        }
                        else if (order.Order_Deduction.ElementAt(j).Id > 0)
                        {
                            a = neworder.Order_Deduction.Where(m => m.Id == order.Order_Deduction.ElementAt(j).Id).SingleOrDefault();
                            if (order.Order_Deduction.ElementAt(j).DeductionId == 0) // حذف نامعلوم
                            {
                                context.Entry(a).State = System.Data.EntityState.Deleted;
                            }
                            else // ویرایش
                            {
                                a.DeductionId = order.Order_Deduction.ElementAt(j).DeductionId;
                                a.Cost = order.Order_Deduction.ElementAt(j).Cost;

                                neworder.Cost -= a.Cost;
                                neworder.Order_Deduction.Add(a);
                            }
                        }
                    }

                    neworder.Cost -= (neworder.Cost * (neworder.DiscountRate / 100));
                    neworder.Cost += (neworder.Cost * (neworder.Tax / 100));
                    neworder.Cost += order.DeliveryCost.HasValue ? order.DeliveryCost.Value : 0;

                    //context.Order_Order.Add(neworder);
                    context.SaveChanges();
                    return Order_Order_Get(neworder.Id);
                }
            }
        }

        public static Order_Order Order_Order_ChangeStatus(int orderId, Models.OrderStatus currentStatus, bool onlyChangeStatus = false)
        {
            using (var context = new PantaEntities())
            {
                Order_Order neworder = context.Order_Order
                    .Include(m => m.Order_Status).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_PackTypes).Include(m => m.Tb_ElevatorBoards)
                    .Where(m => m.Id == orderId).SingleOrDefault();

                if (onlyChangeStatus)
                {
                    neworder.StatusId = (byte)currentStatus;
                }
                else
                {
                    if (neworder.StatusId < (byte)currentStatus)
                    {
                        neworder.StatusId = (byte)currentStatus;

                        if ((byte)currentStatus == (byte)OrderStatus.MojavezKhorooj)
                        {
                            neworder.DateFactor = DateTime.Now;
                        }
                        else if ((byte)currentStatus == (byte)OrderStatus.ErsalShode)
                        {
                            if (neworder.FactorNumber == 0) neworder.FactorNumber = Order_Order_GetLastFactorNumber() + 1;
                        }
                    }
                }
                context.SaveChanges();
                return Order_Order_Get(neworder.Id);
            }
        }

        public static List<Order_Order> Order_Order_Search(int? docNumber, byte? status, byte? tradeTypeId, int? customerId, DateTime? orderDateFrom, DateTime? orderDateTo, DateTime? factorDateFrom, DateTime? factorDateTo, DateTime? deliveryDateFrom, DateTime? deliveryDateTo)
        {
            using (var context = new PantaEntities())
            {
                if (docNumber == null && status == null && tradeTypeId == null && customerId == null && orderDateFrom == null && orderDateTo == null &&
                    factorDateFrom == null && factorDateTo == null && deliveryDateFrom == null && deliveryDateTo == null)
                {
                    List<Order_Order> result = context.Order_Order.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_TradeTypes).Include(m => m.Order_Status)
                                              .Where(o => o.Id > 0).OrderByDescending(m => m.Id).Take(1000).ToList();

                    return result;
                }
                else
                {
                    var list = from p in context.Order_Order.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_TradeTypes).Include(m => m.Order_Status)
                               where p.Id > 0
                               select p;

                    if (docNumber != null) list = list.Where(p => p.DocNumber == docNumber);
                    if (status != null) list = list.Where(p => p.StatusId == (byte)status);
                    if (tradeTypeId != null) list = list.Where(p => p.TradeTypeId == (byte)tradeTypeId);
                    if (customerId != null) list = list.Where(p => p.CustomerId == customerId);
                    if (orderDateFrom != null) list = list.Where(p => p.DateOrder >= orderDateFrom);
                    if (orderDateTo != null) list = list.Where(p => p.DateOrder <= orderDateTo);
                    if (factorDateFrom != null) list = list.Where(p => p.DateFactor >= factorDateFrom);
                    if (factorDateTo != null) list = list.Where(p => p.DateFactor <= factorDateTo);
                    if (deliveryDateFrom != null) list = list.Where(p => p.DateDelivery >= deliveryDateFrom);
                    if (deliveryDateTo != null) list = list.Where(p => p.DateDelivery <= deliveryDateTo);
                    list = list.OrderByDescending(p => p.Id);

                    // Execute the query
                    List<Order_Order> result = list.ToList();

                    return result;
                }
            }
        }

        public static List<Panel> Order_Plan_Search(int? docNumber, byte? status, int? customerId, DateTime? orderDateFrom, DateTime? orderDateTo, DateTime? factorDateFrom, DateTime? factorDateTo, DateTime? deliveryDateFrom, DateTime? deliveryDateTo)
        {
            using (var context = new PantaEntities())
            {
                List<Panel> result = new List<Panel>();
                List<Order_Order> orders;

                if (docNumber == null && status == null && customerId == null && orderDateFrom == null && orderDateTo == null &&
                    factorDateFrom == null && factorDateTo == null && deliveryDateFrom == null && deliveryDateTo == null)
                {
                    orders = context.Order_Order.Include(m => m.Order_Cabin).Include(m => m.Order_Hall).Include(m => m.Order_DoorTop)
                            .Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_TradeTypes).Include(m => m.Order_Status)
                            .Where(o => o.Id > 0).Where(o => o.StatusId >= (byte)OrderStatus.DarJaryaneTolid)
                            .OrderByDescending(m => m.DateOrder).Take(1000).ToList();
                }
                else
                {
                    var list = from p in context.Order_Order.Include(m => m.Order_Cabin).Include(m => m.Order_Hall).Include(m => m.Order_DoorTop)
                                .Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_TradeTypes).Include(m => m.Order_Status)
                               where p.Id > 0 && p.StatusId >= (byte)OrderStatus.DarJaryaneTolid
                               select p;

                    if (status != null) list = list.Where(p => p.StatusId == (byte)status);
                    if (customerId != null) list = list.Where(p => p.CustomerId == customerId);
                    if (orderDateFrom != null) list = list.Where(p => p.DateOrder >= orderDateFrom);
                    if (orderDateTo != null) list = list.Where(p => p.DateOrder <= orderDateTo);
                    if (factorDateFrom != null) list = list.Where(p => p.DateFactor >= factorDateFrom);
                    if (factorDateTo != null) list = list.Where(p => p.DateFactor <= factorDateTo);
                    if (deliveryDateFrom != null) list = list.Where(p => p.DateDelivery >= deliveryDateFrom);
                    if (deliveryDateTo != null) list = list.Where(p => p.DateDelivery <= deliveryDateTo);
                    list = list.OrderByDescending(p => p.DateOrder);

                    // Execute the query
                    orders = list.ToList();
                }

                foreach (Order_Order ord in orders)
                {
                    foreach (Order_Cabin c in ord.Order_Cabin)
                    {
                        if (docNumber == null || c.DocNumber.ToString().Contains(docNumber.ToString()))
                        {
                            result.Add(new Panel(c) { OrderStatusName = ord.Order_Status.Name, CustomerName = ord.Account_Users.FullName, AccepterName = ord.Account_Users1.FullName, ProjectName = ord.ProjectName, ShDateOrder = ord.ShDateOrder, ShDateDelivery = ord.ShDateDelivery, ShDateFactor = ord.ShDateFactor });
                        }
                    }

                    foreach (Order_Hall c in ord.Order_Hall)
                    {
                        if (docNumber == null || c.DocNumber.ToString().Contains(docNumber.ToString()))
                        {
                            result.Add(new Panel(c) { OrderStatusName = ord.Order_Status.Name, CustomerName = ord.Account_Users.FullName, AccepterName = ord.Account_Users1.FullName, ProjectName = ord.ProjectName, ShDateOrder = ord.ShDateOrder, ShDateDelivery = ord.ShDateDelivery, ShDateFactor = ord.ShDateFactor });
                        }
                    }

                    foreach (Order_DoorTop c in ord.Order_DoorTop)
                    {
                        if (docNumber == null || c.DocNumber.ToString().Contains(docNumber.ToString()))
                        {
                            result.Add(new Panel(c) { OrderStatusName = ord.Order_Status.Name, CustomerName = ord.Account_Users.FullName, AccepterName = ord.Account_Users1.FullName, ProjectName = ord.ProjectName, ShDateOrder = ord.ShDateOrder, ShDateDelivery = ord.ShDateDelivery, ShDateFactor = ord.ShDateFactor });
                        }
                    }
                }

                return result;
            }
        }

        public static List<Panel> Order_Product_Search(int? docNumber, byte? status, int? customerId, DateTime? orderDateFrom, DateTime? orderDateTo, DateTime? factorDateFrom, DateTime? factorDateTo, DateTime? deliveryDateFrom, DateTime? deliveryDateTo)
        {
            using (var context = new PantaEntities())
            {
                List<Panel> result = new List<Panel>();

                var list = from p in context.Order_Order.Include(m => m.Order_Cabin).Include(m => m.Order_Hall).Include(m => m.Order_DoorTop)
                            .Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_TradeTypes).Include(m => m.Order_Status)
                           where p.Id > 0 && p.StatusId >= (byte)OrderStatus.DarJaryaneTolid && p.StatusId < (byte)OrderStatus.AmadeTahvil
                           select p;

                if (customerId != null) list = list.Where(p => p.CustomerId == customerId);
                if (orderDateFrom != null) list = list.Where(p => p.DateOrder >= orderDateFrom);
                if (orderDateTo != null) list = list.Where(p => p.DateOrder <= orderDateTo);
                if (factorDateFrom != null) list = list.Where(p => p.DateFactor >= factorDateFrom);
                if (factorDateTo != null) list = list.Where(p => p.DateFactor <= factorDateTo);
                if (deliveryDateFrom != null) list = list.Where(p => p.DateDelivery >= deliveryDateFrom);
                if (deliveryDateTo != null) list = list.Where(p => p.DateDelivery <= deliveryDateTo);
                list = list.OrderByDescending(p => p.DateOrder);

                // Execute the query
                List<Order_Order> orders = list.ToList();


                foreach (Order_Order ord in orders)
                {
                    foreach (Order_Cabin c in ord.Order_Cabin)
                    {
                        if (docNumber == null || c.DocNumber.ToString().Contains(docNumber.ToString()))
                        {
                            if (status == null || c.ProductStatusId == status)
                            {
                                result.Add(new Panel(c) { OrderStatusName = ord.Order_Status.Name, CustomerName = ord.Account_Users.FullName, ProjectName = ord.ProjectName, ShDateOrder = ord.ShDateOrder, ShDateDelivery = ord.ShDateDelivery, ShDateFactor = ord.ShDateFactor });
                            }
                        }
                    }

                    foreach (Order_Hall c in ord.Order_Hall)
                    {
                        if (docNumber == null || c.DocNumber.ToString().Contains(docNumber.ToString()))
                        {
                            if (status == null || c.ProductStatusId == status)
                            {
                                result.Add(new Panel(c) { OrderStatusName = ord.Order_Status.Name, CustomerName = ord.Account_Users.FullName, ProjectName = ord.ProjectName, ShDateOrder = ord.ShDateOrder, ShDateDelivery = ord.ShDateDelivery, ShDateFactor = ord.ShDateFactor });
                            }
                        }
                    }

                    foreach (Order_DoorTop c in ord.Order_DoorTop)
                    {
                        if (docNumber == null || c.DocNumber.ToString().Contains(docNumber.ToString()))
                        {
                            if (status == null || c.ProductStatusId == status)
                            {
                                result.Add(new Panel(c) { OrderStatusName = ord.Order_Status.Name, CustomerName = ord.Account_Users.FullName, ProjectName = ord.ProjectName, ShDateOrder = ord.ShDateOrder, ShDateDelivery = ord.ShDateDelivery, ShDateFactor = ord.ShDateFactor });
                            }
                        }
                    }
                }

                return result;
            }
        }

        public static void Order_Order_Delete(int orderId)
        {
            using (var context = new PantaEntities())
            {
                Order_Order order = context.Order_Order
                    .Include(m => m.Order_Cabin).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinPanels)).Include(m => m.Order_Cabin.Select(n => n.Tb_Monitors)).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinSurfaceMetals)).Include(m => m.Order_Cabin.Select(n => n.Tb_InstallationTypes)).Include(m => m.Order_Cabin.Select(n => n.Tb_PushButtons))
                    .Include(m => m.Order_Cabin.Select(n => n.Tb_Speakers)).Include(m => m.Order_Cabin.Select(n => n.EmergencyLigh))
                    .Include(m => m.Order_Hall).Include(m => m.Order_Hall.Select(n => n.Tb_HallPanels)).Include(m => m.Order_Hall.Select(n => n.Tb_ElevatorCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_Monitors)).Include(m => m.Order_Hall.Select(n => n.Tb_HallPushButtonCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_HallSurfaceMetals)).Include(m => m.Order_Hall.Select(n => n.Tb_PushButtons))
                    .Include(m => m.Order_DoorTop).Include(m => m.Order_DoorTop.Select(n => n.Tb_DoorTopPanels)).Include(m => m.Order_DoorTop.Select(n => n.Tb_SurfaceMetals))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Cabin.Select(n => n.Tb_CabinPanels.Order_ProductStatus))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Hall.Select(n => n.Tb_HallPanels.Order_ProductStatus))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Tb_DoorTopPanels.Order_ProductStatus))
                        .Include(m => m.Order_Deduction).Include(m => m.Order_Deduction.Select(n => n.Tb_Deductions))
                        .Include(m => m.Account_Users).Include(m => m.Account_Users1)
                        .Include(m => m.Order_Status).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_TradeTypes).Include(m => m.Tb_PackTypes).Include(m => m.Tb_ElevatorBoards)
                        .Include(m => m.Order_Process).Include(m => m.Delivery_Delivery)
                        .Where(m => m.Id == orderId).SingleOrDefault();

                for (int i = 0; i < order.Order_Cabin.Count; i++)
                {
                    Order_Cabin cabin = order.Order_Cabin.ElementAt(i);

                    for (int j = 0; j < cabin.Order_Panel_Attachment.Count; j++)
                    {
                        Order_Panel_Attachment attachment = cabin.Order_Panel_Attachment.ElementAt(j);
                        context.Order_Panel_Attachment.Remove(attachment);
                        j--;
                    }
                    for (int j = 0; j < cabin.Order_Panel_Addition.Count; j++)
                    {
                        Order_Panel_Addition addition = cabin.Order_Panel_Addition.ElementAt(j);
                        context.Order_Panel_Addition.Remove(addition);
                        j--;
                    }
                    context.SaveChanges();
                    context.Order_Cabin.Remove(cabin);
                    i--;
                }
                context.SaveChanges();
                for (int i = 0; i < order.Order_Hall.Count; i++)
                {
                    Order_Hall hall = order.Order_Hall.ElementAt(i);

                    for (int j = 0; j < hall.Order_Panel_Attachment.Count; j++)
                    {
                        Order_Panel_Attachment attachment = hall.Order_Panel_Attachment.ElementAt(j);
                        context.Order_Panel_Attachment.Remove(attachment);
                        j--;
                    }
                    for (int j = 0; j < hall.Order_Panel_Addition.Count; j++)
                    {
                        Order_Panel_Addition addition = hall.Order_Panel_Addition.ElementAt(j);
                        context.Order_Panel_Addition.Remove(addition);
                        j--;
                    }
                    context.SaveChanges();
                    context.Order_Hall.Remove(hall);
                    i--;
                }
                context.SaveChanges();
                for (int i = 0; i < order.Order_DoorTop.Count; i++)
                {
                    Order_DoorTop doortop = order.Order_DoorTop.ElementAt(i);

                    for (int j = 0; j < doortop.Order_Panel_Attachment.Count; j++)
                    {
                        Order_Panel_Attachment attachment = doortop.Order_Panel_Attachment.ElementAt(j);
                        context.Order_Panel_Attachment.Remove(attachment);
                        j--;
                    }
                    for (int j = 0; j < doortop.Order_Panel_Addition.Count; j++)
                    {
                        Order_Panel_Addition addition = doortop.Order_Panel_Addition.ElementAt(j);
                        context.Order_Panel_Addition.Remove(addition);
                        j--;
                    }
                    context.SaveChanges();
                    context.Order_DoorTop.Remove(doortop);
                    i--;
                }
                context.SaveChanges();
                for (int i = 0; i < order.Order_Deduction.Count; i++)
                {
                    Order_Deduction deduction = order.Order_Deduction.ElementAt(i);
                    context.Order_Deduction.Remove(deduction);
                    i--;
                }
                for (int i = 0; i < order.Order_Process.Count; i++)
                {
                    Order_Process process = order.Order_Process.ElementAt(i);
                    context.Order_Process.Remove(process);
                    i--;
                }
                for (int i = 0; i < order.Delivery_Delivery.Count; i++)
                {
                    Delivery_Delivery delivery = order.Delivery_Delivery.ElementAt(i);
                    context.Delivery_Delivery.Remove(delivery);
                    i--;
                }
                context.SaveChanges();

                context.Order_Order.Remove(order);
                context.SaveChanges();
            }
        }


        public static List<Order_Process> Order_Process_Get(int productDocNumber)
        {
            using (var context = new PantaEntities())
            {
                return context.Order_Process.Include(m => m.Order_ProductStatus).Include(m => m.Account_Users).Where(x => x.ProductDocNumber == productDocNumber).ToList();
            }
        }

        public static Order_Process Order_Process_Add(Order_Process process)
        {
            double pf_Cost = ProductFactorCost_Get(process.PTime.Value);
            double cp_Percent = CollectiveProducePercent_Get(process.PTime.Value);

            using (var context = new PantaEntities())
            {
                List<Order_Process> plist = context.Order_Process.Where(m => m.ProductDocNumber == process.ProductDocNumber && m.ProductStatusId > process.ProductStatusId).ToList();
                foreach (Order_Process p in plist)
                    context.Order_Process.Remove(p);
                context.SaveChanges();

                List<Order_Process> currentStatusList = context.Order_Process.Where(m => m.ProductDocNumber == process.ProductDocNumber && m.ProductStatusId == process.ProductStatusId).ToList();
                double sumPercent = 0;
                foreach (Order_Process p in currentStatusList)
                    sumPercent += p.Percent;

                Order_Process newPrc = new Order_Process();
                newPrc.PTime = process.PTime;
                newPrc.UserId = process.UserId;
                newPrc.ProductStatusId = process.ProductStatusId;
                newPrc.ProductFactorCost = pf_Cost;
                newPrc.CollectiveProducePercent = cp_Percent;
                newPrc.Percent = process.Percent;
                newPrc.Description = null;
                newPrc.CalculatedFactor = 0;
                sumPercent += process.Percent;

                newPrc.ProductDocNumber = process.ProductDocNumber;
                if (newPrc.ProductDocNumber.ToString().StartsWith("2")) //20-29
                {
                    if (context.Order_Cabin.Any(m => m.DocNumber == newPrc.ProductDocNumber))
                    {
                        Order_Cabin c = context.Order_Cabin.Include(m => m.Order_Order).Include(m => m.Tb_CabinPanels).Include(m => m.Tb_Monitors).Include(m => m.Tb_PushButtons)
                            .Include(m => m.Order_Panel_Attachment).Include(m => m.Order_Panel_Attachment.Select(x => x.Tb_Attachments))
                            .Where(m => m.DocNumber == newPrc.ProductDocNumber).SingleOrDefault();

                        if (c.Order_Order.StatusId < (byte)Models.OrderStatus.DarJaryaneTolid)
                        {
                            throw new Exception("فرایند تولید این محصول شروع نشده است");
                        }
                        else if (c.Order_Order.StatusId > (byte)Models.OrderStatus.AmadeTahvil)
                        {
                            throw new Exception("مجوز خروج این محصول صادر شده است");
                        }
                        else
                        {
                            if (sumPercent >= 100) c.ProductStatusId = process.ProductStatusId < (byte)Models.ProductStatus.AmadeErsal ? (process.ProductStatusId++) : (byte)Models.ProductStatus.AmadeErsal;

                            newPrc.OrderId = c.OrderId;
                            newPrc.ProductTableId = c.TableId;
                            newPrc.Count = c.Count;

                            if (newPrc.ProductStatusId == (byte)ProductStatus.Montaj)
                            {
                                double floorCountPF = 0;
                                double temp = (c.Tb_CabinPanels.Name.ToLower() == "gpc-2528" || c.Tb_CabinPanels.Name.ToLower() == "gpc-2533") ? 1.75 : 0.75;
                                if (c.FloorCount >= 9) floorCountPF += temp;
                                if (c.FloorCount >= 12) floorCountPF += temp;
                                if (c.FloorCount >= 16) floorCountPF += temp;
                                if (c.FloorCount >= 20) floorCountPF += temp;

                                newPrc.ProductFactor = (c.Tb_CabinPanels.ProductFactor + c.Tb_Monitors.ProductFactor + c.Tb_PushButtons.ProductFactor + floorCountPF) * c.Count;
                                foreach (var attachment in c.Order_Panel_Attachment)
                                {
                                    newPrc.ProductFactor += (attachment.Tb_Attachments.ProductFactor * attachment.Count);
                                }
                                newPrc.ProductFactor = newPrc.ProductFactor / c.Count;

                                int cnt = newPrc.Count;
                                if (cnt > 12)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 12) * newPrc.ProductFactor * ((100 - 3 * newPrc.CollectiveProducePercent) / 100));
                                    cnt = 12;
                                }
                                if (cnt > 8)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 8) * newPrc.ProductFactor * ((100 - 2 * newPrc.CollectiveProducePercent) / 100));
                                    cnt = 8;
                                }
                                if (cnt > 3)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 3) * newPrc.ProductFactor * ((100 - newPrc.CollectiveProducePercent) / 100));
                                    cnt = 3;
                                }
                                newPrc.CalculatedFactor += (cnt * newPrc.ProductFactor);
                            }
                            else
                            {
                                newPrc.ProductFactor = newPrc.CalculatedFactor = 0;
                            }

                            context.Order_Process.Add(newPrc);
                            context.SaveChanges();
                            return newPrc;
                        }
                    }
                    else
                    {
                        throw new Exception("بارکد محصول وارد شده وجود ندارد");
                    }
                }
                else if (newPrc.ProductDocNumber.ToString().StartsWith("3")) //30-39
                {
                    if (context.Order_Hall.Any(m => m.DocNumber == newPrc.ProductDocNumber))
                    {
                        Order_Hall c = context.Order_Hall.Include(m => m.Order_Order).Include(m => m.Tb_HallPanels).Include(m => m.Tb_Monitors).Include(m => m.Tb_PushButtons)
                            .Include(m => m.Order_Panel_Attachment).Include(m => m.Order_Panel_Attachment.Select(x => x.Tb_Attachments))
                            .Where(m => m.DocNumber == newPrc.ProductDocNumber).SingleOrDefault();

                        if (c.Order_Order.StatusId < (byte)Models.OrderStatus.DarJaryaneTolid)
                        {
                            throw new Exception("فرایند تولید این محصول شروع نشده است");
                        }
                        else if (c.Order_Order.StatusId > (byte)Models.OrderStatus.AmadeTahvil)
                        {
                            throw new Exception("مجوز خروج این محصول صادر شده است");
                        }
                        else
                        {
                            if (sumPercent >= 100) c.ProductStatusId = process.ProductStatusId < (byte)Models.ProductStatus.AmadeErsal ? (process.ProductStatusId++) : (byte)Models.ProductStatus.AmadeErsal;

                            newPrc.OrderId = c.OrderId;
                            newPrc.ProductTableId = c.TableId;
                            newPrc.Count = c.Count;

                            if (newPrc.ProductStatusId == (byte)ProductStatus.Montaj)
                            {
                                newPrc.ProductFactor = (c.Tb_HallPanels.ProductFactor + c.Tb_Monitors.ProductFactor + (c.PushButtonId == 48 ? c.Tb_PushButtons.ProductFactor : 0)) * c.Count; //سیستم کارت خوان طبقه تک خروجی
                                foreach (var attachment in c.Order_Panel_Attachment)
                                {
                                    newPrc.ProductFactor += (attachment.Tb_Attachments.ProductFactor * attachment.Count);
                                }
                                newPrc.ProductFactor = newPrc.ProductFactor / c.Count;

                                int cnt = newPrc.Count;
                                if (cnt > 50)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 50) * newPrc.ProductFactor * ((100 - 3 * newPrc.CollectiveProducePercent) / 100));
                                    cnt = 50;
                                }
                                if (cnt > 30)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 30) * newPrc.ProductFactor * ((100 - 2 * newPrc.CollectiveProducePercent) / 100));
                                    cnt = 30;
                                }
                                if (cnt > 16)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 16) * newPrc.ProductFactor * ((100 - newPrc.CollectiveProducePercent) / 100));
                                    cnt = 16;
                                }
                                newPrc.CalculatedFactor += (cnt * newPrc.ProductFactor);
                            }
                            else
                            {
                                newPrc.ProductFactor = newPrc.CalculatedFactor = 0;
                            }

                            context.Order_Process.Add(newPrc);
                            context.SaveChanges();
                            return newPrc;
                        }
                    }
                    else
                    {
                        throw new Exception("بارکد محصول وارد شده وجود ندارد");
                    }
                }
                else if (newPrc.ProductDocNumber.ToString().StartsWith("4")) //40-49
                {
                    if (context.Order_DoorTop.Any(m => m.DocNumber == newPrc.ProductDocNumber))
                    {
                        Order_DoorTop c = context.Order_DoorTop.Include(m => m.Order_Order).Include(m => m.Tb_DoorTopPanels).Include(m => m.Tb_Monitors)
                            .Include(m => m.Order_Panel_Attachment).Include(m => m.Order_Panel_Attachment.Select(x => x.Tb_Attachments))
                            .Where(m => m.DocNumber == newPrc.ProductDocNumber).SingleOrDefault();

                        if (c.Order_Order.StatusId < (byte)Models.OrderStatus.DarJaryaneTolid)
                        {
                            throw new Exception("فرایند تولید این محصول شروع نشده است");
                        }
                        else if (c.Order_Order.StatusId > (byte)Models.OrderStatus.AmadeTahvil)
                        {
                            throw new Exception("مجوز خروج این محصول صادر شده است");
                        }
                        else
                        {
                            if (sumPercent >= 100) c.ProductStatusId = process.ProductStatusId < (byte)Models.ProductStatus.AmadeErsal ? (process.ProductStatusId++) : (byte)Models.ProductStatus.AmadeErsal;

                            newPrc.OrderId = c.OrderId;
                            newPrc.ProductTableId = c.TableId;
                            newPrc.Count = c.Count;

                            if (newPrc.ProductStatusId == (byte)ProductStatus.Montaj)
                            {
                                newPrc.ProductFactor = c.Tb_DoorTopPanels.ProductFactor * c.Count;
                                foreach (var attachment in c.Order_Panel_Attachment)
                                {
                                    newPrc.ProductFactor += (attachment.Tb_Attachments.ProductFactor * attachment.Count);
                                }
                                newPrc.ProductFactor = newPrc.ProductFactor / c.Count;

                                int cnt = newPrc.Count;
                                if (cnt > 50)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 50) * newPrc.ProductFactor * ((100 - 3 * newPrc.CollectiveProducePercent) / 100));
                                    cnt = 50;
                                }
                                if (cnt > 30)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 30) * newPrc.ProductFactor * ((100 - 2 * newPrc.CollectiveProducePercent) / 100));
                                    cnt = 30;
                                }
                                if (cnt > 16)
                                {
                                    newPrc.CalculatedFactor += ((cnt - 16) * newPrc.ProductFactor * ((100 - newPrc.CollectiveProducePercent) / 100));
                                    cnt = 16;
                                }
                                newPrc.CalculatedFactor += (cnt * newPrc.ProductFactor);
                            }
                            else
                            {
                                newPrc.ProductFactor = newPrc.CalculatedFactor = 0;
                            }

                            context.Order_Process.Add(newPrc);
                            context.SaveChanges();
                            return newPrc;
                        }
                    }
                    else
                    {
                        throw new Exception("بارکد محصول وارد شده وجود ندارد");
                    }
                }
                else
                {
                    throw new Exception("بارکد محصول وارد شده وجود ندارد");
                }
            }
        }

        public static Order_Process Project_Process_Add(Order_Process process)
        {
            double pf_Cost = ProductFactorCost_Get(process.PTime.Value);
            double cp_Percent = CollectiveProducePercent_Get(process.PTime.Value);

            using (var context = new PantaEntities())
            {
                Order_Process newPrc = new Order_Process();
                newPrc.OrderId = 0;
                newPrc.ProductDocNumber = process.ProductTableId == 3 ? 20 : process.ProductTableId == 7 ? 30 : process.ProductTableId == 10 ? 40 : 0;
                newPrc.ProductTableId = process.ProductTableId;
                newPrc.Description = process.Description;
                newPrc.UserId = process.UserId;
                newPrc.ProductStatusId = process.ProductStatusId;
                newPrc.PTime = process.PTime;
                newPrc.Percent = process.Percent;
                newPrc.Count = process.Count;
                newPrc.ProductFactor = process.ProductFactor;
                newPrc.ProductFactorCost = pf_Cost;
                newPrc.CollectiveProducePercent = cp_Percent;
                newPrc.CalculatedFactor = 0;

                if (newPrc.ProductTableId == 3) //Cabin panel
                {
                    int cnt = newPrc.Count;
                    if (cnt > 15)
                    {
                        newPrc.CalculatedFactor += ((cnt - 15) * newPrc.ProductFactor * ((100 - 3 * newPrc.CollectiveProducePercent) / 100));
                        cnt = 15;
                    }
                    if (cnt > 10)
                    {
                        newPrc.CalculatedFactor += ((cnt - 10) * newPrc.ProductFactor * ((100 - 2 * newPrc.CollectiveProducePercent) / 100));
                        cnt = 10;
                    }
                    if (cnt > 6)
                    {
                        newPrc.CalculatedFactor += ((cnt - 6) * newPrc.ProductFactor * ((100 - newPrc.CollectiveProducePercent) / 100));
                        cnt = 6;
                    }
                    newPrc.CalculatedFactor += (cnt * newPrc.ProductFactor);
                }
                else if (newPrc.ProductTableId == 7 || newPrc.ProductTableId == 10) //Hall or Doortop panel
                {
                    int cnt = newPrc.Count;
                    if (cnt > 50)
                    {
                        newPrc.CalculatedFactor += ((cnt - 50) * newPrc.ProductFactor * ((100 - 3 * newPrc.CollectiveProducePercent) / 100));
                        cnt = 50;
                    }
                    if (cnt > 30)
                    {
                        newPrc.CalculatedFactor += ((cnt - 30) * newPrc.ProductFactor * ((100 - 2 * newPrc.CollectiveProducePercent) / 100));
                        cnt = 30;
                    }
                    if (cnt > 16)
                    {
                        newPrc.CalculatedFactor += ((cnt - 16) * newPrc.ProductFactor * ((100 - newPrc.CollectiveProducePercent) / 100));
                        cnt = 16;
                    }
                    newPrc.CalculatedFactor += (cnt * newPrc.ProductFactor);
                }
                else
                {
                    newPrc.CalculatedFactor += (newPrc.Count * newPrc.ProductFactor);
                }

                context.Order_Process.Add(newPrc);
                context.SaveChanges();
                return newPrc;
            }
        }

        public static void Order_Process_UpdateStatus(int orderId)
        {
            using (var context = new PantaEntities())
            {
                //if (!context.Order_Cabin.Any(m => m.OrderId == orderId && m.ProductStatusId < (byte)Models.ProductStatus.AmadeErsal))
                //    if (!context.Order_Hall.Any(m => m.OrderId == orderId && m.ProductStatusId < (byte)Models.ProductStatus.AmadeErsal))
                //        if (!context.Order_DoorTop.Any(m => m.OrderId == orderId && m.ProductStatusId < (byte)Models.ProductStatus.AmadeErsal))
                //        {
                //            Order_Order order = context.Order_Order.Where(o => o.Id == orderId).SingleOrDefault();
                //            order.StatusId = (byte)Models.OrderStatus.TolidShode;
                //            context.SaveChanges();
                //        }

                // گزینه "اتمام فرایند تولید" از سامانه حذف شد و سفارشات به طور خودکار آماده تحویل می شوند
                if (!context.Order_Cabin.Any(m => m.OrderId == orderId && m.ProductStatusId < (byte)Models.ProductStatus.BasteBandi))
                    if (!context.Order_Hall.Any(m => m.OrderId == orderId && m.ProductStatusId < (byte)Models.ProductStatus.BasteBandi))
                        if (!context.Order_DoorTop.Any(m => m.OrderId == orderId && m.ProductStatusId < (byte)Models.ProductStatus.BasteBandi))
                        {
                            Order_Order order = context.Order_Order.Where(o => o.Id == orderId).SingleOrDefault();

                            if (order.StatusId != (byte)Models.OrderStatus.AmadeTahvil)
                            {
                                order.StatusId = (byte)Models.OrderStatus.AmadeTahvil;
                                context.SaveChanges();

                                try
                                {
                                    var customer = AccountManager.Account_User_Get(order.CustomerId);
                                    string mobile = customer.GetMobile();
                                    if (!string.IsNullOrEmpty(mobile))
                                    {
                                        SmsKavenegar sk = new SmsKavenegar();
                                        sk.SendSms(mobile, order.DocNumber.ToString(), "AmadeTahvil", "", "", customer.FullName);
                                    }
                                }
                                catch { }
                            }
                        }
            }
        }

        public static List<ZaribKarkard> Order_Process_Report(List<int> userIdList, DateTime reportDateFrom, DateTime reportDateTo)
        {
            using (var context = new PantaEntities())
            {
                return context.Order_Process.Include(m => m.Account_Users).Where(x => x.PTime >= reportDateFrom && x.PTime <= reportDateTo).Where(x => userIdList.Contains(x.UserId))
                    .GroupBy(m => new { m.UserId, m.Account_Users.Name, m.Account_Users.LastName }).Select(g => new ZaribKarkard() { UserId = g.Key.UserId, UserFullName = g.Key.Name + " " + g.Key.LastName, CalculatedFactor = g.Sum(i => i.CalculatedFactor * (i.Percent / 100)) }).ToList();
            }
        }

        public static List<Order_Process> Order_Process_Report(int userId, DateTime reportDateFrom, DateTime reportDateTo)
        {
            using (var context = new PantaEntities())
            {
                List<Order_Process> res = context.Order_Process.Include(m => m.Order_ProductStatus).Include(m => m.Account_Users)
                    .Include(m => m.Order_Order)
                    .Include(m => m.Order_Order.Order_Cabin).Include(m => m.Order_Order.Order_Cabin.Select(n => n.Tb_CabinPanels))
                    .Include(m => m.Order_Order.Order_Hall).Include(m => m.Order_Order.Order_Hall.Select(n => n.Tb_HallPanels))
                    .Include(m => m.Order_Order.Order_DoorTop).Include(m => m.Order_Order.Order_DoorTop.Select(n => n.Tb_DoorTopPanels))
                    .Include(m => m.Order_Order.Account_Users).Include(m => m.Order_Order.Account_Users1)
                    .Where(x => x.PTime >= reportDateFrom && x.PTime <= reportDateTo).Where(x => x.UserId == userId).Where(x => x.OrderId != 0)
                    .OrderBy(x => x.ProductTableId).ThenBy(x => x.PTime).ToList();

                res.AddRange(context.Order_Process.Include(m => m.Order_ProductStatus).Include(m => m.Account_Users)
                    .Include(m => m.Order_Order)
                    .Include(m => m.Order_Order.Order_Cabin).Include(m => m.Order_Order.Order_Cabin.Select(n => n.Tb_CabinPanels))
                    .Include(m => m.Order_Order.Order_Hall).Include(m => m.Order_Order.Order_Hall.Select(n => n.Tb_HallPanels))
                    .Include(m => m.Order_Order.Order_DoorTop).Include(m => m.Order_Order.Order_DoorTop.Select(n => n.Tb_DoorTopPanels))
                    .Include(m => m.Order_Order.Account_Users).Include(m => m.Order_Order.Account_Users1)
                    .Where(x => x.PTime >= reportDateFrom && x.PTime <= reportDateTo).Where(x => x.UserId == userId).Where(x => x.OrderId == 0)
                    .OrderBy(x => x.ProductTableId).ThenBy(x => x.PTime).ToList());

                return res;
            }
        }

        public static double CollectiveProducePercent_Get(DateTime reportDate)
        {
            using (var context = new PantaEntities())
            {
                if (context.Tb_CollectiveProducePercent.Any(x => x.ApplyDate <= reportDate))
                    return context.Tb_CollectiveProducePercent.Where(x => x.ApplyDate <= reportDate).OrderByDescending(x => x.ApplyDate).First().Amount;
                else return 20;
            }
        }

        public static double ProductFactorCost_Get(DateTime reportDate)
        {
            using (var context = new PantaEntities())
            {
                if (context.Tb_ProductFactorCost.Any(x => x.ApplyDate <= reportDate))
                    return context.Tb_ProductFactorCost.Where(x => x.ApplyDate <= reportDate).OrderByDescending(x => x.ApplyDate).First().Cost;
                else
                    return 0;
            }
        }
    }
}