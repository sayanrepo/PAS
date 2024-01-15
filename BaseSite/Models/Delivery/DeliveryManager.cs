using BaseSite.Data;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using BaseSite.Models.Order;
using BaseSite.Models.Sale;
using BaseSite.Services.SmsService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.Delivery
{
    public class DeliveryManager
    {
        public static bool isDeliveryAttachment(int attachmentId)
        {
            if (attachmentId == 1 || attachmentId == 27 || attachmentId == 3 || attachmentId == 2 ||
                attachmentId == 21 || attachmentId == 22 || attachmentId == 23 || attachmentId == 24 ||
                attachmentId == 58 || attachmentId == 59)
                return true;
            else return false;
        }

        public static List<Delivery_Delivery> Delivery_Delivery_Search(int? docNumber, byte? status, int? customerId, DateTime? deliveryDateFrom, DateTime? deliveryDateTo)
        {
            using (var context = new PantaEntities())
            {
                if (docNumber == null && status == null && customerId == null && deliveryDateFrom == null && deliveryDateTo == null)
                {
                    List<Delivery_Delivery> result = context.Delivery_Delivery.Include(m => m.Order_Order).Include(m => m.Sale_Sale).Include(m => m.Order_Order.Account_Users).Include(m => m.Sale_Sale.Account_Users)
                                              .Include(m => m.Delivery_Status).Include(m => m.Delivery_VehicleTypes).Where(o => o.Id > 0)
                                              .OrderByDescending(m => m.Date).ThenByDescending(m => m.Order_Order.FactorNumber).ThenByDescending(m => m.Sale_Sale.FactorNumber).Take(1000).ToList();

                    return result;
                }
                else
                {
                    var list = from p in context.Delivery_Delivery.Include(m => m.Order_Order).Include(m => m.Sale_Sale).Include(m => m.Order_Order.Account_Users).Include(m => m.Sale_Sale.Account_Users)
                                              .Include(m => m.Delivery_Status).Include(m => m.Delivery_VehicleTypes)
                               where p.Id > 0
                               select p;

                    //if (docNumber != null) list = list.Where(p => p.DocNumber.ToString().Contains(docNumber.ToString()));
                    if (status != null) list = list.Where(p => p.StatusId == (byte)status);
                    if (customerId != null) list = list.Where(x => x.Order_Order != null ? x.Order_Order.CustomerId == customerId : x.Sale_Sale.CustomerId == customerId);
                    if (deliveryDateFrom != null) list = list.Where(p => p.Date >= deliveryDateFrom);
                    if (deliveryDateTo != null) list = list.Where(p => p.Date <= deliveryDateTo);
                    list = list.OrderByDescending(p => p.Date).ThenByDescending(m => m.Order_Order.FactorNumber).ThenByDescending(m => m.Sale_Sale.FactorNumber);

                    // Execute the query
                    /*List<Delivery_Delivery> result = list.ToList();
                    if (docNumber != null) result = result.Where(p => p.DocNumber.ToString().Contains(docNumber.ToString())).ToList();*/

                    if (docNumber != null)
                    {
                        int f = (int)Math.Pow(10, docNumber.ToString().Length);
                        list = list.Where(p => (p.DocNumber - docNumber) % f == 0);
                    }
                    List<Delivery_Delivery> result = list.ToList();

                    return result;
                }
            }
        }

        public static Delivery_Delivery Delivery_Delivery_Edit(Delivery_Delivery delivery, string submit)
        {
            using (var context = new PantaEntities())
            {
                if (delivery.Id == 0)
                {
                    Delivery_Delivery newdelivery = new Delivery_Delivery();
                    newdelivery.TableId = 20;
                    newdelivery.DocNumber = delivery.DocNumber;
                    newdelivery.Date = DateTime.Now.Date;
                    newdelivery.PackTypeId = delivery.PackTypeId;
                    newdelivery.SendResponsible = delivery.SendResponsible;
                    newdelivery.RecieveResponsible = delivery.RecieveResponsible;
                    newdelivery.DeliveryLocationId = delivery.DeliveryLocationId;
                    newdelivery.VehicleTypeId = delivery.VehicleTypeId;
                    newdelivery.CarierAgencyName = delivery.CarierAgencyName;
                    newdelivery.CarierAgencyBill = delivery.CarierAgencyBill;
                    newdelivery.VehiclePlaque = delivery.VehiclePlaque;
                    newdelivery.DriverName = delivery.DriverName;
                    newdelivery.DriverPhone = delivery.DriverPhone;
                    newdelivery.RecieverName = delivery.RecieverName;
                    newdelivery.RecieverPhone = delivery.RecieverPhone;
                    newdelivery.RecieverMobile = delivery.RecieverMobile;
                    newdelivery.DestinationType = delivery.DestinationType;
                    newdelivery.DestinationAddress = delivery.DestinationAddress;
                    newdelivery.StatusId = (byte)DeliveryStatus.SaderShode;

                    if (delivery.OrderId.HasValue && delivery.Items != null)
                    {
                        newdelivery.OrderId = delivery.OrderId;
                        for (int i = 0; i < delivery.Items.Count; i++)
                        {
                            int _id = delivery.Items.ElementAt(i).Id;
                            if (delivery.Items.ElementAt(i).Type == 1)
                            {
                                Order_Cabin c = context.Order_Cabin.Where(a => a.Id == _id).SingleOrDefault();
                                if (delivery.Items.ElementAt(i).Checked)
                                {
                                    c.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                    newdelivery.Order_Cabin.Add(c);
                                }
                            }
                            else if (delivery.Items.ElementAt(i).Type == 2)
                            {
                                Order_Hall h = context.Order_Hall.Where(a => a.Id == _id).SingleOrDefault();
                                if (delivery.Items.ElementAt(i).Checked)
                                {
                                    h.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                    newdelivery.Order_Hall.Add(h);
                                }
                            }
                            else if (delivery.Items.ElementAt(i).Type == 3)
                            {
                                Order_DoorTop d = context.Order_DoorTop.Where(a => a.Id == _id).SingleOrDefault();
                                if (delivery.Items.ElementAt(i).Checked)
                                {
                                    d.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                    newdelivery.Order_DoorTop.Add(d);
                                }
                            }
                            else if (delivery.Items.ElementAt(i).Type == 11 || delivery.Items.ElementAt(i).Type == 12 || delivery.Items.ElementAt(i).Type == 13)
                            {
                                Order_Panel_Attachment attach = context.Order_Panel_Attachment.Where(a => a.Id == _id).SingleOrDefault();
                                if (delivery.Items.ElementAt(i).Checked)
                                {
                                    attach.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                    newdelivery.Order_Panel_Attachment.Add(attach);
                                }
                            }
                        }
                    }
                    else if (delivery.SaleId.HasValue && delivery.Items != null)
                    {
                        newdelivery.SaleId = delivery.SaleId;
                        for (int i = 0; i < delivery.Items.Count; i++)
                        {
                            int _id = delivery.Items.ElementAt(i).Id;
                            if (delivery.Items.ElementAt(i).Type == 4)
                            {
                                Sale_Goods g = context.Sale_Goods.Where(a => a.Id == _id).SingleOrDefault();
                                if (delivery.Items.ElementAt(i).Checked)
                                {
                                    g.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                    newdelivery.Sale_Goods.Add(g);
                                }
                            }
                        }
                    }

                    context.Delivery_Delivery.Add(newdelivery);
                    context.SaveChanges();
                    return Delivery_Delivery_Get(newdelivery.Id);
                }
                else
                {
                    /////////////////////////////////////////////// Get Delivery ///////////////////////////////////////////
                    Delivery_Delivery newdelivery = context.Delivery_Delivery.Include(m => m.Order_Order).Include(m => m.Sale_Sale)
                                        .Include(m => m.Delivery_Status).Include(m => m.Delivery_VehicleTypes).Include(m => m.Tb_PackTypes).Where(m => m.Id == delivery.Id).SingleOrDefault();

                    if (newdelivery.OrderId.HasValue)
                    {
                        newdelivery.Order_Order = context.Order_Order
                        .Include(m => m.Order_Cabin).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinPanels)).Include(m => m.Order_Cabin.Select(n => n.Tb_Monitors)).Include(m => m.Order_Cabin.Select(n => n.Tb_CabinSurfaceMetals)).Include(m => m.Order_Cabin.Select(n => n.Tb_InstallationTypes)).Include(m => m.Order_Cabin.Select(n => n.Tb_PushButtons)).Include(m => m.Order_Cabin.Select(n => n.Tb_Speakers))
                        .Include(m => m.Order_Hall).Include(m => m.Order_Hall.Select(n => n.Tb_HallPanels)).Include(m => m.Order_Hall.Select(n => n.Tb_ElevatorCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_Monitors)).Include(m => m.Order_Hall.Select(n => n.Tb_HallPushButtonCounts)).Include(m => m.Order_Hall.Select(n => n.Tb_HallSurfaceMetals)).Include(m => m.Order_Hall.Select(n => n.Tb_PushButtons))
                        .Include(m => m.Order_DoorTop).Include(m => m.Order_DoorTop.Select(n => n.Tb_DoorTopPanels))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Cabin.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_Hall.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Addition.Select(o => o.Tb_Additions)))
                        .Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment)).Include(m => m.Order_DoorTop.Select(n => n.Order_Panel_Attachment.Select(o => o.Tb_Attachments)))
                        .Include(m => m.Order_Deduction).Include(m => m.Order_Deduction.Select(n => n.Tb_Deductions)).Include(m => m.Account_Users)
                        .Include(m => m.Order_Status).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_PackTypes).Include(m => m.Tb_ElevatorBoards)
                        .Where(m => m.Id == newdelivery.OrderId.Value).SingleOrDefault();

                        newdelivery.Items = new List<CheckableItem>();

                        foreach (Order_Cabin c in newdelivery.Order_Order.Order_Cabin)
                        {
                            if (c.Tb_CabinPanels.Id > 0)
                            {
                                if (!c.DeliveryId.HasValue || c.DeliveryId.Value == newdelivery.Id)
                                {
                                    newdelivery.Items.Add(new CheckableItem() { Type = 1, Id = c.Id, Checked = (c.DeliveryId.HasValue && c.DeliveryId.Value == newdelivery.Id), Name = "پنل داخل کابین: " + c.Tb_CabinPanels.Name + " " + c.Tb_CabinPanels.Description, Count = c.Count, Comment = c.DeliveryComment });
                                }
                                foreach (var attach in c.Order_Panel_Attachment)
                                {
                                    if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && (!attach.DeliveryId.HasValue || attach.DeliveryId == newdelivery.Id))
                                    {
                                        newdelivery.Items.Add(new CheckableItem() { Type = 11, Id = attach.Id, Checked = (attach.DeliveryId.HasValue && attach.DeliveryId == newdelivery.Id), Name = "ملحقات داخل کابین: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                                    }
                                }
                            }
                        }
                        foreach (Order_Hall h in newdelivery.Order_Order.Order_Hall)
                        {
                            if (h.Tb_HallPanels.Id > 0)
                            {
                                if (!h.DeliveryId.HasValue || h.DeliveryId.Value == newdelivery.Id)
                                {
                                    newdelivery.Items.Add(new CheckableItem() { Type = 2, Id = h.Id, Checked = (h.DeliveryId.HasValue && h.DeliveryId.Value == newdelivery.Id), Name = "پنل طبقات: " + h.Tb_HallPanels.Name + " " + h.Tb_HallPanels.Description, Count = h.Count, Comment = h.DeliveryComment });
                                }
                                foreach (var attach in h.Order_Panel_Attachment)
                                {
                                    if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && (!attach.DeliveryId.HasValue || attach.DeliveryId == newdelivery.Id))
                                    {
                                        newdelivery.Items.Add(new CheckableItem() { Type = 12, Id = attach.Id, Checked = (attach.DeliveryId.HasValue && attach.DeliveryId == newdelivery.Id), Name = "ملحقات طبقات: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                                    }
                                }
                            }
                        }
                        foreach (Order_DoorTop d in newdelivery.Order_Order.Order_DoorTop)
                        {
                            if (d.Tb_DoorTopPanels.Id > 0)
                            {
                                if (!d.DeliveryId.HasValue || d.DeliveryId.Value == newdelivery.Id)
                                {
                                    newdelivery.Items.Add(new CheckableItem() { Type = 3, Id = d.Id, Checked = (d.DeliveryId.HasValue && d.DeliveryId.Value == newdelivery.Id), Name = "پنل سردرب: " + d.Tb_DoorTopPanels.Name + " " + d.Tb_DoorTopPanels.Description, Count = d.Count, Comment = d.DeliveryComment });
                                }
                                foreach (var attach in d.Order_Panel_Attachment)
                                {
                                    if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && (!attach.DeliveryId.HasValue || attach.DeliveryId == newdelivery.Id))
                                    {
                                        newdelivery.Items.Add(new CheckableItem() { Type = 13, Id = attach.Id, Checked = (attach.DeliveryId.HasValue && attach.DeliveryId == newdelivery.Id), Name = "ملحقات سردرب: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                                    }
                                }
                            }
                        }
                    }
                    else if (newdelivery.SaleId.HasValue)
                    {
                        newdelivery.Sale_Sale = context.Sale_Sale.Include(m => m.Account_Users).Include(m => m.Order_Status)
                        .Include(m => m.Tb_OrderTypes).Where(m => m.Id == newdelivery.SaleId.Value).SingleOrDefault();

                        newdelivery.Items = new List<CheckableItem>();

                        foreach (Sale_Goods c in newdelivery.Sale_Sale.Sale_Goods)
                        {
                            if (!c.DeliveryId.HasValue || c.DeliveryId.Value == newdelivery.Id)
                            {
                                newdelivery.Items.Add(new CheckableItem() { Type = 4, Id = c.Id, Checked = (c.DeliveryId.HasValue && c.DeliveryId.Value == newdelivery.Id), Name = c.Name, Count = c.Count, Comment = c.DeliveryComment });
                            }
                        }
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (newdelivery.StatusId == (byte)DeliveryStatus.SaderShode)
                    {
                        //newdelivery.TableId = 20;
                        //newdelivery.DocNumber = sale.DocNumber;
                        //newdelivery.Date = DateTime.Now.Date;
                        newdelivery.PackTypeId = delivery.PackTypeId;
                        newdelivery.SendResponsible = delivery.SendResponsible;
                        newdelivery.RecieveResponsible = delivery.RecieveResponsible;
                        newdelivery.RecieverName = delivery.RecieverName;
                        newdelivery.RecieverPhone = delivery.RecieverPhone;
                        newdelivery.RecieverMobile = delivery.RecieverMobile;
                        newdelivery.DestinationType = delivery.DestinationType;
                        newdelivery.DestinationAddress = delivery.DestinationAddress;

                        newdelivery.DeliveryLocationId = delivery.DeliveryLocationId;
                        newdelivery.VehicleTypeId = delivery.VehicleTypeId;
                        newdelivery.CarierAgencyName = delivery.CarierAgencyName;
                        newdelivery.CarierAgencyBill = delivery.CarierAgencyBill;
                        newdelivery.VehiclePlaque = delivery.VehiclePlaque;
                        newdelivery.DriverName = delivery.DriverName;
                        newdelivery.DriverPhone = delivery.DriverPhone;

                        if (delivery.OrderId.HasValue && delivery.Items != null)
                        {
                            //newdelivery.OrderId = sale.OrderId;
                            for (int i = 0; i < delivery.Items.Count; i++)
                            {
                                int _id = delivery.Items.ElementAt(i).Id;
                                if (delivery.Items.ElementAt(i).Type == 1)
                                {
                                    Order_Cabin c = context.Order_Cabin.Where(a => a.Id == _id).SingleOrDefault();
                                    if (delivery.Items.ElementAt(i).Checked)
                                    {
                                        c.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                        newdelivery.Order_Cabin.Add(c);
                                    }
                                    else c.DeliveryId = null;
                                }
                                else if (delivery.Items.ElementAt(i).Type == 2)
                                {
                                    Order_Hall h = context.Order_Hall.Where(a => a.Id == _id).SingleOrDefault();
                                    if (delivery.Items.ElementAt(i).Checked)
                                    {
                                        h.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                        newdelivery.Order_Hall.Add(h);
                                    }
                                    else h.DeliveryId = null;
                                }
                                else if (delivery.Items.ElementAt(i).Type == 3)
                                {
                                    Order_DoorTop d = context.Order_DoorTop.Where(a => a.Id == _id).SingleOrDefault();
                                    if (delivery.Items.ElementAt(i).Checked)
                                    {
                                        d.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                        newdelivery.Order_DoorTop.Add(d);
                                    }
                                    else d.DeliveryId = null;
                                }
                                else if (delivery.Items.ElementAt(i).Type == 11 || delivery.Items.ElementAt(i).Type == 12 || delivery.Items.ElementAt(i).Type == 13)
                                {
                                    Order_Panel_Attachment att = context.Order_Panel_Attachment.Where(a => a.Id == _id).SingleOrDefault();
                                    if (delivery.Items.ElementAt(i).Checked)
                                    {
                                        att.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                        newdelivery.Order_Panel_Attachment.Add(att);
                                    }
                                    else att.DeliveryId = null;
                                }
                            }
                        }
                        else if (delivery.SaleId.HasValue && delivery.Items != null)
                        {
                            //newdelivery.SaleId = sale.SaleId;
                            for (int i = 0; i < delivery.Items.Count; i++)
                            {
                                int _id = delivery.Items.ElementAt(i).Id;
                                if (delivery.Items.ElementAt(i).Type == 4)
                                {
                                    Sale_Goods g = context.Sale_Goods.Where(a => a.Id == _id).SingleOrDefault();
                                    if (delivery.Items.ElementAt(i).Checked)
                                    {
                                        g.DeliveryComment = delivery.Items.ElementAt(i).Comment;
                                        newdelivery.Sale_Goods.Add(g);
                                    }
                                    else g.DeliveryId = null;
                                }
                            }
                        }
                    }
                    else if (newdelivery.StatusId == (byte)DeliveryStatus.TayidShode)
                    {
                        newdelivery.DeliveryLocationId = delivery.DeliveryLocationId;
                        newdelivery.VehicleTypeId = delivery.VehicleTypeId;
                        newdelivery.CarierAgencyName = delivery.CarierAgencyName;
                        newdelivery.CarierAgencyBill = delivery.CarierAgencyBill;
                        newdelivery.VehiclePlaque = delivery.VehiclePlaque;
                        newdelivery.DriverName = delivery.DriverName;
                        newdelivery.DriverPhone = delivery.DriverPhone;

                        if (delivery.StatusId == (byte)DeliveryStatus.ErsalShode)
                        {
                            if (delivery.OrderId.HasValue)
                            {
                                OrderManager.Order_Order_ChangeStatus(delivery.OrderId.Value, OrderStatus.ErsalShode);
                            }
                            else if (delivery.SaleId.HasValue)
                            {
                                SaleManager.Sale_Sale_ChangeStatus(delivery.SaleId.Value, OrderStatus.ErsalShode);
                            }
                        }
                    }

                    bool sendSurvey = false;
                    if (newdelivery.StatusId < delivery.StatusId)
                    {
                        newdelivery.Date = DateTime.Now.Date;
                        if (delivery.StatusId == (byte)DeliveryStatus.ErsalShode) sendSurvey = true;
                    }
                    newdelivery.StatusId = delivery.StatusId;

                    //context.Delivery_Delivery.Add(newdelivery);
                    context.SaveChanges();

                    if (sendSurvey)
                    {
                        try
                        {
                            var customer = AccountManager.Account_User_Get(newdelivery.Order_Order.CustomerId);
                            string mobile = customer.GetMobile();
                            if (!string.IsNullOrEmpty(mobile))
                            {
                                SmsKavenegar sk = new SmsKavenegar();
                                sk.SendSms(mobile, newdelivery.Order_Order.DocNumber.ToString(), "Survey", "", "", customer.FullName);
                            }
                        }
                        catch { }
                    }

                    return Delivery_Delivery_Get(newdelivery.Id);
                }
            }
        }

        public static Delivery_Delivery Delivery_Delivery_Get(int deliveryId)
        {
            using (var context = new PantaEntities())
            {
                Delivery_Delivery delivery = context.Delivery_Delivery.Include(m => m.Order_Order).Include(m => m.Sale_Sale)
                                        .Include(m => m.Delivery_Status).Include(m => m.Delivery_DeliveryLocations).Include(m => m.Delivery_VehicleTypes).Include(m => m.Tb_PackTypes).Where(m => m.Id == deliveryId).SingleOrDefault();

                if (delivery.OrderId.HasValue)
                {
                    delivery.Order_Order = OrderManager.Order_Order_Get(delivery.OrderId.Value);
                    delivery.Items = new List<CheckableItem>();

                    foreach (Order_Cabin c in delivery.Order_Order.Order_Cabin)
                    {
                        if (c.Tb_CabinPanels.Id > 0)
                        {
                            if (!c.DeliveryId.HasValue || c.DeliveryId.Value == delivery.Id)
                            {
                                delivery.Items.Add(new CheckableItem() { Type = 1, Id = c.Id, Checked = (c.DeliveryId.HasValue && c.DeliveryId.Value == delivery.Id), Model = c.Tb_CabinPanels.Name, Name = "پنل داخل کابین: " + c.Tb_CabinPanels.Name + " " + c.Tb_CabinPanels.Description, Count = c.Count, Comment = c.DeliveryComment });
                            }
                            foreach (var attach in c.Order_Panel_Attachment)
                            {
                                if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && (!attach.DeliveryId.HasValue || attach.DeliveryId == delivery.Id))
                                {
                                    delivery.Items.Add(new CheckableItem() { Type = 11, Id = attach.Id, Checked = (attach.DeliveryId.HasValue && attach.DeliveryId == delivery.Id), Model = attach.Tb_Attachments.Name, Name = "ملحقات داخل کابین: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                                }
                            }
                        }
                    }
                    foreach (Order_Hall h in delivery.Order_Order.Order_Hall)
                    {
                        if (h.Tb_HallPanels.Id > 0)
                        {
                            if (!h.DeliveryId.HasValue || h.DeliveryId.Value == delivery.Id)
                            {
                                delivery.Items.Add(new CheckableItem() { Type = 2, Id = h.Id, Checked = (h.DeliveryId.HasValue && h.DeliveryId.Value == delivery.Id), Model = h.Tb_HallPanels.Name, Name = "پنل طبقات: " + h.Tb_HallPanels.Name + " " + h.Tb_HallPanels.Description, Count = h.Count, Comment = h.DeliveryComment });
                            }
                            foreach (var attach in h.Order_Panel_Attachment)
                            {
                                if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && (!attach.DeliveryId.HasValue || attach.DeliveryId == delivery.Id))
                                {
                                    delivery.Items.Add(new CheckableItem() { Type = 12, Id = attach.Id, Checked = (attach.DeliveryId.HasValue && attach.DeliveryId == delivery.Id), Model = attach.Tb_Attachments.Name, Name = "ملحقات طبقات: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                                }
                            }
                        }
                    }
                    foreach (Order_DoorTop d in delivery.Order_Order.Order_DoorTop)
                    {
                        if (d.Tb_DoorTopPanels.Id > 0)
                        {
                            if (!d.DeliveryId.HasValue || d.DeliveryId.Value == delivery.Id)
                            {
                                delivery.Items.Add(new CheckableItem() { Type = 3, Id = d.Id, Checked = (d.DeliveryId.HasValue && d.DeliveryId.Value == delivery.Id), Model = d.Tb_DoorTopPanels.Name, Name = "پنل سردرب: " + d.Tb_DoorTopPanels.Name + " " + d.Tb_DoorTopPanels.Description, Count = d.Count, Comment = d.DeliveryComment });
                            }
                            foreach (var attach in d.Order_Panel_Attachment)
                            {
                                if ((attach.Tb_Attachments.IsDeliveryItem || DeliveryManager.isDeliveryAttachment(attach.AttachmentId)) && (!attach.DeliveryId.HasValue || attach.DeliveryId == delivery.Id))
                                {
                                    delivery.Items.Add(new CheckableItem() { Type = 13, Id = attach.Id, Checked = (attach.DeliveryId.HasValue && attach.DeliveryId == delivery.Id), Model = attach.Tb_Attachments.Name, Name = "ملحقات سردرب: " + attach.Tb_Attachments.Name + " " + attach.Tb_Attachments.Description, Count = attach.Count, Comment = attach.DeliveryComment });
                                }
                            }
                        }
                    }
                }
                else if (delivery.SaleId.HasValue)
                {
                    delivery.Sale_Sale = SaleManager.Sale_Sale_Get(delivery.SaleId.Value);
                    delivery.Items = new List<CheckableItem>();

                    foreach (Sale_Goods c in delivery.Sale_Sale.Sale_Goods)
                    {
                        if (!c.DeliveryId.HasValue || c.DeliveryId.Value == delivery.Id)
                        {
                            delivery.Items.Add(new CheckableItem() { Type = 4, Id = c.Id, Checked = (c.DeliveryId.HasValue && c.DeliveryId.Value == delivery.Id), Model = c.Name, Name = c.Name, Count = c.Count, Comment = (!c.DeliveryId.HasValue ? c.Comment : c.DeliveryComment) });
                        }
                    }
                }

                return delivery;
            }
        }
    }
}