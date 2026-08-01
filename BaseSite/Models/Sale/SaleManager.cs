using BaseSite.Data;
using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.Sale
{
    public class SaleManager
    {
        private static int Sale_GenerateDocNumber()
        {
            //DocNumber format : [800000-899999]
            int num = Models.Order.OrderManager.RandomDocNumber.Next(800000, 899999);

            using (var context = new PantaEntities())
            {
                while (context.Sale_Sale.Any(m => m.DocNumber == num))
                {
                    num = Models.Order.OrderManager.RandomDocNumber.Next(800000, 899999);
                }
            }
            return num;
        }

        public static Sale_Sale Sale_Sale_Add()
        {
            using (var context = new PantaEntities())
            {
                Sale_Sale x = context.Sale_Sale.Include(m => m.Sale_Goods).Include(m => m.Account_Users)
                    .Where(m => m.Id == 0).SingleOrDefault();

                return x;
            }
        }

        public static List<Sale_Sale> Sale_Sale_Search(byte? storeId, int? docNumber, byte? status, byte? tradeTypeId, int? customerId, DateTime? orderDateFrom, DateTime? orderDateTo, DateTime? factorDateFrom, DateTime? factorDateTo)
        {
            using (var context = new PantaEntities())
            {
                if (storeId == null && docNumber == null && status == null && tradeTypeId == null && customerId == null && orderDateFrom == null && orderDateTo == null &&
                    factorDateFrom == null && factorDateTo == null)
                {
                    List<Sale_Sale> result = context.Sale_Sale.Include(m => m.Sale_Goods).Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes)
                                              .Include(m => m.Tb_TradeTypes).Include(m => m.Order_Status).Where(o => o.Id > 0).OrderByDescending(m => m.Id).Take(1000).ToList();

                    return result;
                }
                else
                {
                    var list = from p in context.Sale_Sale.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Tb_OrderTypes).Include(m => m.Tb_TradeTypes).Include(m => m.Order_Status)
                               where p.Id > 0
                               select p;

                    if (storeId != null) list = list.Where(p => p.StoreId == storeId);
                    if (docNumber != null) list = list.Where(p => p.DocNumber == docNumber);
                    if (status != null) list = list.Where(p => p.StatusId == (byte)status);
                    if (tradeTypeId != null) list = list.Where(p => p.TradeTypeId == (byte)tradeTypeId);
                    if (customerId != null) list = list.Where(p => p.CustomerId == customerId);
                    if (orderDateFrom != null) list = list.Where(p => p.DateOrder >= orderDateFrom);
                    if (orderDateTo != null) list = list.Where(p => p.DateOrder <= orderDateTo);
                    if (factorDateFrom != null) list = list.Where(p => p.DateFactor >= factorDateFrom);
                    if (factorDateTo != null) list = list.Where(p => p.DateFactor <= factorDateTo);
                    list = list.OrderByDescending(p => p.Id);

                    // Execute the query
                    List<Sale_Sale> result = list.ToList();

                    return result;
                }
            }
        }

        public static Sale_Sale Sale_Sale_Edit(Sale_Sale sale, string submit)
        {
            using (var context = new PantaEntities())
            {
                if (sale.Id == 0)
                {
                    Sale_Sale newsale = new Sale_Sale();
                    newsale.TableId = 18;
                    newsale.DocNumber = Sale_GenerateDocNumber();
                    newsale.CustomerId = sale.CustomerId;
                    newsale.ClienteleName = sale.ClienteleName;
                    newsale.OrderTypeId = sale.OrderTypeId;
                    newsale.GiveBack = sale.GiveBack;
                    newsale.DeliveryCityId = sale.DeliveryCityId;
                    newsale.DeliveryAddress = sale.DeliveryAddress;
                    newsale.Tax = sale.Tax;
                    newsale.StatusId = (byte)OrderStatus.PishFactor;
                    newsale.DateOrder = DateTime.Now;              // sale.DateOrder;
                    newsale.DateDelivery = null;                   // sale.DateDelivery;
                    newsale.DateFactor = sale.DateFactor;
                    newsale.DeliveryCost = sale.DeliveryCost;
                    newsale.Discount = sale.Discount;
                    newsale.Cost = 0;
                    newsale.Comment = sale.Comment;
                    newsale.AccepterId = sale.AccepterId;
                    newsale.StoreId = sale.StoreId;
                    newsale.TradeTypeId = sale.TradeTypeId;

                    for (int i = 0; i < sale.Sale_Goods.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Name))
                        {
                            Sale_Goods c = new Sale_Goods();
                            c.SaleId = sale.Id;
                            c.TypeId = sale.Sale_Goods.ElementAt(i).TypeId;
                            c.Name = string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Name) ? " " : sale.Sale_Goods.ElementAt(i).Name;
                            c.ProductId = sale.Sale_Goods.ElementAt(i).ProductId >= 0 ? sale.Sale_Goods.ElementAt(i).ProductId : 0;
                            c.Phi = sale.Sale_Goods.ElementAt(i).Phi;
                            c.Count = sale.Sale_Goods.ElementAt(i).Count;
                            c.Comment = string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Comment) ? " " : sale.Sale_Goods.ElementAt(i).Comment;

                            newsale.Cost += c.Count * c.Phi;
                            newsale.Sale_Goods.Add(c);
                        }
                    }

                    newsale.Cost -= newsale.Discount;
                    newsale.Cost += (newsale.Cost * (newsale.Tax / 100));
                    newsale.Cost += sale.DeliveryCost.HasValue ? sale.DeliveryCost.Value : 0;
                    context.Sale_Sale.Add(newsale);
                    context.SaveChanges();
                    return Sale_Sale_Get(newsale.Id);
                }
                else
                {
                    Sale_Sale newsale = context.Sale_Sale.Include(m => m.Account_Users).Include(m => m.Order_Status)
                        .Include(m => m.Tb_OrderTypes).Include(m => m.Sale_Goods).Where(m => m.Id == sale.Id).SingleOrDefault();

                    newsale.CustomerId = sale.CustomerId;
                    newsale.ClienteleName = sale.ClienteleName;
                    newsale.OrderTypeId = sale.OrderTypeId;
                    newsale.GiveBack = sale.GiveBack;
                    newsale.DeliveryCityId = sale.DeliveryCityId;
                    newsale.DeliveryAddress = sale.DeliveryAddress;
                    newsale.Tax = sale.Tax;
                    newsale.StatusId = sale.StatusId;

                    if (newsale.StatusId == (byte)OrderStatus.PishFactor)
                    {
                        newsale.DateFactor = sale.DateFactor;
                    }
                    else if (newsale.StatusId == (byte)OrderStatus.DarDasteEghdam)
                    {
                        //newsale.DateOrder = DateTime.Now;              // sale.DateOrder;
                        newsale.DateDelivery = null;                   // sale.DateDelivery;
                        newsale.DateFactor = sale.DateFactor.HasValue ? sale.DateFactor : DateTime.Now.AddDays(5);  // sale.DateFactor;
                    }
                    else if (newsale.StatusId == (byte)OrderStatus.MojavezKhorooj)
                    {
                        newsale.DateDelivery = DateTime.Now;
                        newsale.DateFactor = DateTime.Now;//.AddDays(5);
                    }
                    else
                    {
                        //newsale.DateOrder = sale.DateOrder;
                        //newsale.DateDelivery = sale.DateDelivery;
                        //newsale.DateFactor = sale.DateFactor;
                    }
                    newsale.DeliveryCost = sale.DeliveryCost;
                    newsale.Discount = sale.Discount;
                    newsale.Cost = 0;
                    newsale.Comment = sale.Comment;
                    newsale.StoreId = sale.StoreId;
                    newsale.TradeTypeId = sale.TradeTypeId;

                    for (int i = 0; i < sale.Sale_Goods.Count; i++)
                    {
                        Sale_Goods c = null;
                        if (sale.Sale_Goods.ElementAt(i).Id == 0 && !string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Name))
                        {
                            // ایجاد درصورتی که نامعلوم نباشد
                            c = new Sale_Goods()
                            {
                                SaleId = sale.Id,
                                TypeId = sale.Sale_Goods.ElementAt(i).TypeId,
                                Name = string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Name) ? " " : sale.Sale_Goods.ElementAt(i).Name,
                                ProductId = sale.Sale_Goods.ElementAt(i).ProductId >= 0 ? sale.Sale_Goods.ElementAt(i).ProductId : 0,
                                Phi = sale.Sale_Goods.ElementAt(i).Phi,
                                Count = sale.Sale_Goods.ElementAt(i).Count,
                                Comment = string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Comment) ? " " : sale.Sale_Goods.ElementAt(i).Comment,
                            };

                            newsale.Cost += c.Count * c.Phi;
                            newsale.Sale_Goods.Add(c);
                        }
                        else if (sale.Sale_Goods.ElementAt(i).Id > 0)
                        {
                            c = newsale.Sale_Goods.Where(m => m.Id == sale.Sale_Goods.ElementAt(i).Id).SingleOrDefault();
                            if (string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Name)) // حذف نامعلوم
                            {
                                //newsale.Sale_Goods.Remove(c);
                                context.Entry(c).State = System.Data.Entity.EntityState.Deleted;
                            }
                            else // ویرایش
                            {
                                c.SaleId = sale.Id;
                                c.TypeId = sale.Sale_Goods.ElementAt(i).TypeId;
                                c.Name = string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Name) ? " " : sale.Sale_Goods.ElementAt(i).Name;
                                c.ProductId = sale.Sale_Goods.ElementAt(i).ProductId >= 0 ? sale.Sale_Goods.ElementAt(i).ProductId : 0;
                                c.Phi = sale.Sale_Goods.ElementAt(i).Phi;
                                c.Count = sale.Sale_Goods.ElementAt(i).Count;
                                c.Comment = string.IsNullOrWhiteSpace(sale.Sale_Goods.ElementAt(i).Comment) ? " " : sale.Sale_Goods.ElementAt(i).Comment;

                                newsale.Cost += c.Count * c.Phi;
                                newsale.Sale_Goods.Add(c);
                            }
                        }
                    }

                    newsale.Cost -= newsale.Discount;
                    newsale.Cost += (newsale.Cost * (newsale.Tax / 100));
                    newsale.Cost += sale.DeliveryCost.HasValue ? sale.DeliveryCost.Value : 0;
                    //context.Sale_Sale.Add(neworder);
                    context.SaveChanges();
                    return Sale_Sale_Get(newsale.Id);
                }
            }
        }

        public static Sale_Sale Sale_Sale_Get(int saleId)
        {
            using (var context = new PantaEntities())
            {
                Sale_Sale sale = context.Sale_Sale.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Order_Status)
                    .Include(m => m.Tb_OrderTypes).Include(m => m.Sale_Goods).Include(m => m.Tb_Stores).Include(m => m.Tb_TradeTypes).Where(m => m.Id == saleId).SingleOrDefault();

                sale.SumCostGoods = 0;
                foreach (Sale_Goods c in sale.Sale_Goods) sale.SumCostGoods += (c.Phi * c.Count);
                sale.SumCostTax = (sale.SumCostGoods - sale.Discount) * (sale.Tax / 100);

                return sale;
            }
        }

        public static int Sale_Sale_GetLastFactorNumber(bool giveBack)
        {
            using (var context = new PantaEntities())
            {
                int Sale_LastFactorNumber = context.Sale_Sale.Where(m => m.GiveBack == giveBack).Max(m => m.FactorNumber);
                if (!giveBack)
                {
                    if (Sale_LastFactorNumber < 800000) Sale_LastFactorNumber = 800000;
                }
                else
                {
                    if (Sale_LastFactorNumber < 850000) Sale_LastFactorNumber = 850000;
                }
                return Sale_LastFactorNumber;
            }
        }

        public static Sale_Sale Sale_Sale_ChangeStatus(int saleId, Models.OrderStatus newStatus, bool onlyChangeStatus = false)
        {
            using (var context = new PantaEntities())
            {
                Sale_Sale sale = context.Sale_Sale.Include(m => m.Account_Users).Include(m => m.Order_Status)
                    .Include(m => m.Tb_OrderTypes).Where(m => m.Id == saleId).SingleOrDefault();

                if (onlyChangeStatus)
                {
                    sale.StatusId = (byte)newStatus;
                }
                else
                {
                    if (sale.StatusId < (byte)newStatus)
                    {
                        sale.StatusId = (byte)newStatus;

                        if ((byte)newStatus == (byte)OrderStatus.MojavezKhorooj)
                        {
                            sale.DateFactor = DateTime.Now;
                        }
                        else if ((byte)newStatus == (byte)OrderStatus.ErsalShode)
                        {
                            if (sale.FactorNumber == 0) sale.FactorNumber = Sale_Sale_GetLastFactorNumber(sale.GiveBack) + 1;
                        }
                    }
                }
                context.SaveChanges();
                return Sale_Sale_Get(sale.Id);
            }
        }

        public static void Sale_Sale_Delete(int saleId)
        {
            using (var context = new PantaEntities())
            {
                Sale_Sale sale = context.Sale_Sale.Include(m => m.Account_Users).Include(m => m.Account_Users1).Include(m => m.Order_Status)
                    .Include(m => m.Tb_OrderTypes).Include(m => m.Sale_Goods).Where(m => m.Id == saleId).SingleOrDefault();

                for (int i = 0; i < sale.Sale_Goods.Count; i++)
                {
                    Sale_Goods item = sale.Sale_Goods.ElementAt(i);
                    context.Sale_Goods.Remove(item);
                    i--;
                }
                for (int i = 0; i < sale.Delivery_Delivery.Count; i++)
                {
                    Delivery_Delivery delivery = sale.Delivery_Delivery.ElementAt(i);
                    context.Delivery_Delivery.Remove(delivery);
                    i--;
                }
                context.SaveChanges();

                context.Sale_Sale.Remove(sale);
                context.SaveChanges();
            }
        }
    }
}