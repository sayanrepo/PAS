using BaseSite.Data;
using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.Information
{
    public class InformationManager
    {
        public static List<Location_Countries> Location_Country_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Countries.Where(o => o.Deleted == false).OrderBy(o => o.Name).ToList();
            }
        }

        //استان در کدام کشور قرار دارد
        public static int Location_Country_Get(int ProvinceId)
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Provinces.Where(p => p.Id == ProvinceId).Select(p => p.CountryId).SingleOrDefault();
            }
        }


        public static int Location_Country_Add(string countryName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Location_Countries c = new Location_Countries { Name = countryName };
                    context.Location_Countries.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static void Location_Country_Delete(int countryId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    var provinces = context.Location_Provinces.Where(x => x.CountryId == countryId).ToList();
                    provinces.ForEach(x => x.Deleted = true);

                    var provinceIds = provinces.Select(x => x.Id).ToList();
                    var cities = context.Location_Cities.Where(x => provinceIds.Contains(x.ProvinceId)).ToList();
                    cities.ForEach(x => x.Deleted = true);

                    Location_Countries c = context.Location_Countries.SingleOrDefault(x => x.Id == countryId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static void Location_Country_Edit(int countryId, string newName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Location_Countries c = context.Location_Countries.SingleOrDefault(x => x.Id == countryId);
                    c.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static List<Location_Provinces> Location_Province_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Provinces.Where(x => x.Deleted == false).ToList();
            }
        }

        public static List<Location_Provinces> Location_Province_Get(int countryId)
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Provinces.Where(x => x.CountryId == countryId && x.Deleted == false).OrderBy(o => o.Name).ToList();
            }
        }

        //شهر در کدام استان قراردارد
        public static int Location_ProvinceID_Get(int CityId)
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Cities.Where(c => c.Id == CityId).Select(c => c.ProvinceId).SingleOrDefault();
            }
        }

        public static int Location_Province_Add(int countryId, string provinceName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Location_Provinces c = new Location_Provinces { CountryId = countryId, Name = provinceName };
                    context.Location_Provinces.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static void Location_Province_Delete(int provinceId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    var cities = context.Location_Cities.Where(x => x.ProvinceId == provinceId).ToList();
                    cities.ForEach(x => x.Deleted = true);

                    Location_Provinces p = context.Location_Provinces.SingleOrDefault(x => x.Id == provinceId);
                    p.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static void Location_Province_Edit(int provinceId, string newName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Location_Provinces p = context.Location_Provinces.SingleOrDefault(x => x.Id == provinceId);
                    p.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static List<Location_Cities> Location_City_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Cities.Where(x => x.Deleted == false).ToList();
            }
        }

        public static List<Location_Cities> Location_City_Get(int provinceId)
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Cities.Where(x => x.ProvinceId == provinceId && x.Deleted == false).OrderBy(o => o.Name).ToList();
            }
        }

        public static Location_Cities Location_City_GetProvince_and_Country(int cityId)
        {
            using (var context = new PantaEntities())
            {
                return context.Location_Cities.Include(c => c.Location_Provinces).Include(p => p.Location_Provinces.Location_Countries).SingleOrDefault(x => x.Id == cityId);
            }
        }
        public static int Location_City_Add(int provinceId, string cityName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Location_Cities c = new Location_Cities { ProvinceId = provinceId, Name = cityName };
                    context.Location_Cities.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static void Location_City_Delete(int cityId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Location_Cities c = context.Location_Cities.SingleOrDefault(x => x.Id == cityId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        public static void Location_City_Edit(int cityId, string newName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Location_Cities c = context.Location_Cities.SingleOrDefault(x => x.Id == cityId);
                    c.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }


        //انواع سفارشات
        public static List<Tb_OrderTypes> Order_Type_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_OrderTypes.OrderBy(o => o.Name).ToList();
            }
        }


        //وضعیت سفارشات
        public static List<Order_Status> Order_Status_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Order_Status.OrderBy(o => o.Name).ToList();
            }
        }

        //نوع بسته بندی
        public static List<Tb_PackTypes> Order_Pack_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_PackTypes.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static int Order_Pack_Add(string packName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_PackTypes c = new Tb_PackTypes { Name = packName };
                    context.Tb_PackTypes.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Pack_Delete(int packId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_PackTypes c = context.Tb_PackTypes.SingleOrDefault(x => x.Id == packId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Pack_Edit(int packId, string newName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_PackTypes c = context.Tb_PackTypes.SingleOrDefault(x => x.Id == packId);
                    c.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        //نوع تابلو
        public static List<Tb_ElevatorBoards> Order_ElevatorBoard_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_ElevatorBoards.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static int Order_ElevatorBoard_Add(string boardName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_ElevatorBoards c = new Tb_ElevatorBoards { Name = boardName };
                    context.Tb_ElevatorBoards.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_ElevatorBoard_Delete(int boardId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_ElevatorBoards c = context.Tb_ElevatorBoards.SingleOrDefault(x => x.Id == boardId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_ElevatorBoard_Edit(int boardId, string newName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_ElevatorBoards c = context.Tb_ElevatorBoards.SingleOrDefault(x => x.Id == boardId);
                    c.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // کسورات
        public static List<Tb_Deductions> Order_Deduction_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Deductions.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static int Order_Deduction_Add(string deductionName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Deductions c = new Tb_Deductions { TableId = 1, Name = deductionName };
                    context.Tb_Deductions.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Deduction_Delete(int deductionId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Deductions c = context.Tb_Deductions.SingleOrDefault(x => x.Id == deductionId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Deduction_Edit(int deductionId, string newName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Deductions c = context.Tb_Deductions.SingleOrDefault(x => x.Id == deductionId);
                    c.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // اضافات
        public static List<Tb_Additions> Order_Addition_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Additions.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static int Order_Addition_Add(string additionName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Additions c = new Tb_Additions { TableId = 1, Name = additionName };
                    context.Tb_Additions.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Addition_Delete(int additionId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Additions c = context.Tb_Additions.SingleOrDefault(x => x.Id == additionId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Addition_Edit(int additionId, string newName)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Additions c = context.Tb_Additions.SingleOrDefault(x => x.Id == additionId);
                    c.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }


        // ملحقات
        public static List<Tb_Attachments> Order_Attachment_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Attachments.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static int Order_Attachment_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable, bool? IsDeliveryItem)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Attachments c = new Tb_Attachments
                    {
                        TableId = 4,
                        Name = ItemName,
                        Description = ItemDescription,
                        Cost = ItemCost.HasValue ? ItemCost.Value : 0,
                        ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0,
                        Available = ItemAvailable ?? true,
                        IsDeliveryItem = IsDeliveryItem ?? false
                    };
                    context.Tb_Attachments.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Attachment_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Attachments c = context.Tb_Attachments.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Order_Attachment_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable, bool? IsDeliveryItem)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Attachments c = context.Tb_Attachments.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    if (IsDeliveryItem.HasValue) c.IsDeliveryItem = IsDeliveryItem.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // پوش باتون
        public static List<Tb_PushButtons> Cabin_PushButton_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_PushButtons.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_PushButtons Cabin_PushButton_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_PushButtons.Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int Cabin_PushButton_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_PushButtons c = new Tb_PushButtons { TableId = (int)DB_Table.Tb_PushButtons, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true };
                    context.Tb_PushButtons.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_PushButton_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_PushButtons c = context.Tb_PushButtons.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_PushButton_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_PushButtons c = context.Tb_PushButtons.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // نمایشگر
        public static List<Tb_Monitors> Cabin_Monitor_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Monitors.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_Monitors Cabin_Monitor_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Monitors.Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int Cabin_Monitor_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Monitors c = new Tb_Monitors { TableId = (int)DB_Table.Tb_Monitors, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true };
                    context.Tb_Monitors.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_Monitor_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Monitors c = context.Tb_Monitors.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_Monitor_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Monitors c = context.Tb_Monitors.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }


        // فلز رویه
        public static List<Tb_SurfaceMetals> SurfaceMetal_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_SurfaceMetals.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_SurfaceMetals SurfaceMetal_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_SurfaceMetals.Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int SurfaceMetal_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_SurfaceMetals c = new Tb_SurfaceMetals { TableId = 4, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true };
                    context.Tb_SurfaceMetals.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void SurfaceMetal_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_SurfaceMetals c = context.Tb_SurfaceMetals.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void SurfaceMetal_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_SurfaceMetals c = context.Tb_SurfaceMetals.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }


        // پنل داخل کابین-مدل پنل
        public static List<Tb_CabinPanels> Cabin_Panel_Get(bool all = false)
        {
            using (var context = new PantaEntities())
            {
                if (all) return context.Tb_CabinPanels.Include(m => m.Order_ProductStatus).Where(x => x.Deleted == false).OrderBy(o => o.Name).ToList();
                else return context.Tb_CabinPanels.Include(m => m.Order_ProductStatus).Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_CabinPanels Cabin_Panel_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_CabinPanels.Include(m => m.Order_ProductStatus).Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int Cabin_Panel_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_CabinPanels c = new Tb_CabinPanels { TableId = (int)DB_Table.Tb_CabinPanels, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true, SurfaceArea = 1, StartFrom = (byte)ProductStatus.NagsheKeshi };
                    context.Tb_CabinPanels.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_Panel_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_CabinPanels c = context.Tb_CabinPanels.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_Panel_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable, double? ItemSurfaceArea, byte? ItemStartFrom)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_CabinPanels c = context.Tb_CabinPanels.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    if (ItemSurfaceArea.HasValue) c.SurfaceArea = ItemSurfaceArea.Value;
                    if (ItemStartFrom.HasValue) c.StartFrom = ItemStartFrom.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // پنل داخل کابین-فلز رویه
        public static List<Tb_CabinSurfaceMetals> Cabin_SurfaceMetal_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_CabinSurfaceMetals.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_CabinSurfaceMetals Cabin_SurfaceMetal_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_CabinSurfaceMetals.Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int Cabin_SurfaceMetal_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_CabinSurfaceMetals c = new Tb_CabinSurfaceMetals { TableId = 4, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true };
                    context.Tb_CabinSurfaceMetals.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_SurfaceMetal_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_CabinSurfaceMetals c = context.Tb_CabinSurfaceMetals.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Cabin_SurfaceMetal_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_CabinSurfaceMetals c = context.Tb_CabinSurfaceMetals.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }


        // پنل طبقات-مدل پنل
        public static List<Tb_HallPanels> Hall_Panel_Get(bool all = false)
        {
            using (var context = new PantaEntities())
            {
                if (all) return context.Tb_HallPanels.Include(m => m.Order_ProductStatus).Where(x => x.Deleted == false).OrderBy(o => o.Name).ToList();
                else return context.Tb_HallPanels.Include(m => m.Order_ProductStatus).Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_HallPanels Hall_Panel_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_HallPanels.Include(m => m.Order_ProductStatus).Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int Hall_Panel_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_HallPanels c = new Tb_HallPanels { TableId = (int)DB_Table.Tb_HallPanels, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true, SurfaceArea = 1, StartFrom = (byte)ProductStatus.NagsheKeshi };
                    context.Tb_HallPanels.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Hall_Panel_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_HallPanels c = context.Tb_HallPanels.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Hall_Panel_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable, double? ItemSurfaceArea, byte? ItemStartFrom)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_HallPanels c = context.Tb_HallPanels.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    if (ItemSurfaceArea.HasValue) c.SurfaceArea = ItemSurfaceArea.Value;
                    if (ItemStartFrom.HasValue) c.StartFrom = ItemStartFrom.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // پنل طبقات-فلز رویه
        public static List<Tb_HallSurfaceMetals> Hall_SurfaceMetal_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_HallSurfaceMetals.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_HallSurfaceMetals Hall_SurfaceMetal_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_HallSurfaceMetals.Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int Hall_SurfaceMetal_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_HallSurfaceMetals c = new Tb_HallSurfaceMetals { TableId = 4, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true };
                    context.Tb_HallSurfaceMetals.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Hall_SurfaceMetal_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_HallSurfaceMetals c = context.Tb_HallSurfaceMetals.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Hall_SurfaceMetal_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_HallSurfaceMetals c = context.Tb_HallSurfaceMetals.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }


        // پنل سردرب-مدل پنل
        public static List<Tb_DoorTopPanels> DoorTop_Panel_Get(bool all = false)
        {
            using (var context = new PantaEntities())
            {
                if (all) return context.Tb_DoorTopPanels.Include(m => m.Order_ProductStatus).Where(x => x.Deleted == false).OrderBy(o => o.Name).ToList();
                else return context.Tb_DoorTopPanels.Include(m => m.Order_ProductStatus).Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static Tb_DoorTopPanels DoorTop_Panel_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_DoorTopPanels.Include(m => m.Order_ProductStatus).Where(x => x.Id == id).SingleOrDefault();
            }
        }
        public static int DoorTop_Panel_Add(string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_DoorTopPanels c = new Tb_DoorTopPanels { TableId = (int)DB_Table.Tb_DoorTopPanels, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, ProductFactor = ItemFactor.HasValue ? ItemFactor.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true, SurfaceArea = 1, StartFrom = (byte)ProductStatus.NagsheKeshi };
                    context.Tb_DoorTopPanels.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void DoorTop_Panel_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_DoorTopPanels c = context.Tb_DoorTopPanels.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void DoorTop_Panel_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, double? ItemFactor, bool? ItemAvailable, double? ItemSurfaceArea, byte? ItemStartFrom)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_DoorTopPanels c = context.Tb_DoorTopPanels.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemFactor.HasValue) c.ProductFactor = ItemFactor.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    if (ItemSurfaceArea.HasValue) c.SurfaceArea = ItemSurfaceArea.Value;
                    if (ItemStartFrom.HasValue) c.StartFrom = ItemStartFrom.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }


        // ارزش ریالی ضریب کارکرد
        public static List<Tb_ProductFactorCost> ProductFactorCost_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_ProductFactorCost.OrderBy(o => o.ApplyDate).ToList();
            }
        }
        public static Tb_ProductFactorCost ProductFactorCost_Add(DateTime applyDate, double cost)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    DateTime? maxRcordedDate = context.Order_Process.Max(x => x.PTime);

                    if (maxRcordedDate.HasValue && applyDate <= maxRcordedDate)
                    {
                        throw new Exception(string.Format("تاریخ اعمال باید بعد از تاریخ آخرین ثبت بارکد ({0}) باشد", new PersianDateTime(maxRcordedDate.Value).ToString(PersianDateTimeFormat.Date)));
                    }
                    else
                    {
                        Tb_ProductFactorCost c = new Tb_ProductFactorCost { ApplyDate = applyDate, Cost = cost };
                        context.Tb_ProductFactorCost.Add(c);
                        context.SaveChanges();
                        return c;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void ProductFactorCost_Delete(DateTime date)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    DateTime? maxRcordedDate = context.Order_Process.Max(x => x.PTime);

                    if (maxRcordedDate.HasValue && date <= maxRcordedDate)
                    {
                        throw new Exception(string.Format("تاریخ های قبل از آخرین ثبت بارکد ({0}) را نمی توان حذف کرد", new PersianDateTime(maxRcordedDate.Value).ToString(PersianDateTimeFormat.Date)));
                    }
                    else
                    {
                        Tb_ProductFactorCost c = context.Tb_ProductFactorCost.SingleOrDefault(x => x.ApplyDate == date);
                        context.Tb_ProductFactorCost.Remove(c);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // کاهش ضریب در تولید تیراژی
        public static List<Tb_CollectiveProducePercent> CollectiveProducePercent_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_CollectiveProducePercent.OrderBy(o => o.ApplyDate).ToList();
            }
        }
        public static Tb_CollectiveProducePercent CollectiveProducePercent_Add(DateTime applyDate, double percent)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    DateTime? maxRcordedDate = context.Order_Process.Max(x => x.PTime);

                    if (maxRcordedDate.HasValue && applyDate <= maxRcordedDate)
                    {
                        throw new Exception(string.Format("تاریخ اعمال باید بعد از تاریخ آخرین ثبت بارکد ({0}) باشد", new PersianDateTime(maxRcordedDate.Value).ToString(PersianDateTimeFormat.Date)));
                    }
                    else
                    {
                        Tb_CollectiveProducePercent c = new Tb_CollectiveProducePercent { ApplyDate = applyDate, Amount = percent };
                        context.Tb_CollectiveProducePercent.Add(c);
                        context.SaveChanges();
                        return c;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void CollectiveProducePercent_Delete(DateTime date)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    DateTime? maxRcordedDate = context.Order_Process.Max(x => x.PTime);

                    if (maxRcordedDate.HasValue && date <= maxRcordedDate)
                    {
                        throw new Exception(string.Format("تاریخ های قبل از آخرین ثبت بارکد ({0}) را نمی توان حذف کرد", new PersianDateTime(maxRcordedDate.Value).ToString(PersianDateTimeFormat.Date)));
                    }
                    else
                    {
                        Tb_CollectiveProducePercent c = context.Tb_CollectiveProducePercent.SingleOrDefault(x => x.ApplyDate == date);
                        context.Tb_CollectiveProducePercent.Remove(c);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }

        // جداول سازگاری
        public static List<Tb_Truth> TruthTable_Get(int primaryTableId, int secondaryTableId)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Truth.Where(m => m.PrimaryTableId == primaryTableId && m.SecondaryTableId == secondaryTableId).ToList();
            }
        }

        public static List<Tb_Truth> TruthTable_Get2(int primaryTableId, int primaryId)
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Truth.Where(m => m.PrimaryTableId == primaryTableId && m.PrimaryId == primaryId /*&& m.TValue > 0*/).ToList();
            }
        }

        public static Tb_Truth TruthTable_Edit(int ptableId, int pId, int stableId, int sId, double value)
        {
            using (var context = new PantaEntities())
            {
                if (context.Tb_Truth.Any(m => m.PrimaryTableId == ptableId && m.PrimaryId == pId && m.SecondaryTableId == stableId && m.SecondaryId == sId))
                {
                    Tb_Truth obj = context.Tb_Truth.Where(m => m.PrimaryTableId == ptableId && m.PrimaryId == pId && m.SecondaryTableId == stableId && m.SecondaryId == sId).SingleOrDefault();

                    if (value > 0)
                    {
                        obj.TValue = value;
                        context.SaveChanges();
                        return obj;
                    }
                    else
                    {
                        context.Tb_Truth.Remove(obj);
                        context.SaveChanges();
                        return new Tb_Truth() { PrimaryTableId = ptableId, PrimaryId = pId, SecondaryTableId = stableId, SecondaryId = sId, TValue = 0 };
                    }
                }
                else
                {
                    if (value > 0)
                    {
                        Tb_Truth obj = new Tb_Truth() { PrimaryTableId = ptableId, PrimaryId = pId, SecondaryTableId = stableId, SecondaryId = sId, TValue = value };
                        context.Tb_Truth.Add(obj);
                        context.SaveChanges();
                        return obj;
                    }
                    else
                    {
                        return new Tb_Truth() { PrimaryTableId = ptableId, PrimaryId = pId, SecondaryTableId = stableId, SecondaryId = sId, TValue = 0 };
                    }
                }
            }
        }


        // محصولات فروشگاه
        public static List<Tb_Products> Products_Get()
        {
            using (var context = new PantaEntities())
            {
                return context.Tb_Products.Where(x => x.Deleted == false && x.Id > 0).OrderBy(o => o.Name).ToList();
            }
        }
        public static int Product_Add(string ItemName, string ItemDescription, double? ItemCost, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Products c = new Tb_Products { TableId = 22, Name = ItemName, Description = ItemDescription, Cost = ItemCost.HasValue ? ItemCost.Value : 0, Available = ItemAvailable.HasValue ? ItemAvailable.Value : true };
                    context.Tb_Products.Add(c);
                    context.SaveChanges();
                    return c.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Product_Delete(int ItemId)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Products c = context.Tb_Products.SingleOrDefault(x => x.Id == ItemId);
                    c.Deleted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
        public static void Product_Edit(int ItemId, string ItemName, string ItemDescription, double? ItemCost, bool? ItemAvailable)
        {
            try
            {
                using (var context = new PantaEntities())
                {
                    Tb_Products c = context.Tb_Products.SingleOrDefault(x => x.Id == ItemId);
                    if (ItemName != null) c.Name = ItemName;
                    if (ItemDescription != null) c.Description = ItemDescription;
                    if (ItemCost.HasValue) c.Cost = ItemCost.Value;
                    if (ItemAvailable.HasValue) c.Available = ItemAvailable.Value;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cache.Update();
            }
        }
    }
}