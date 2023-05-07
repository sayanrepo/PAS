using BaseSite.Data;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BaseSite.Models
{
    public class CacheItem
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public double Cost { set; get; }
        public double val1 { set; get; }

        public CacheItem()
        {
            this.Name = "";
            this.Description = "";
            this.Cost = 0;
            this.val1 = 0;
        }

        public CacheItem(string name, string description, double cost)
        {
            this.Name = name;
            this.Description = description;
            this.Cost = cost;
            this.val1 = 0;
        }

        public CacheItem(string name, string description, double cost, double value1)
        {
            this.Name = name;
            this.Description = description;
            this.Cost = cost;
            this.val1 = value1;
        }
    }
    public class Cache
    {
        /// <summary>
        /// لیست اشخاص
        /// </summary>
        public static Dictionary<int, string> Account_Users;
        /// <summary>
        /// بخش های سیستم
        /// </summary>
        public static List<Account_Categories> Categories;
        /// <summary>
        /// عملیات های سیستم
        /// </summary>
        public static List<Account_Operations> Operations;
        /// <summary>
        /// پست های سیستم
        /// </summary>
        public static List<Account_Posts> Posts;
        /// <summary>
        /// وضعیت موجودیت ها
        /// </summary>
        public static Dictionary<Status, string> EntityStatuses;
        /// <summary>
        /// وضعیت کاربران/اشخاص
        /// </summary>
        public static Dictionary<UserStatus, string> UserStatuses;
        /// <summary>
        /// نوع شخص
        /// </summary>
        public static Dictionary<byte, string> Account_PersonTypes;
        /// <summary>
        /// نوع همکاری، نوع رابطه
        /// </summary>
        public static Dictionary<byte, string> Account_PartnerTypes;
        /// <summary>
        /// جداول اصلی سیستم
        /// </summary>
        public static List<BaseSystem_Tables> Tables;


        /// <summary>
        /// تابلوهای آسانسور
        /// </summary>
        public static Dictionary<int, string> ElevatorBoards;
        /// <summary>
        /// نوع بسته بندی
        /// </summary>
        public static Dictionary<short, string> PackTypes;


        /// <summary>
        /// نوع پنل داخل کابین
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> CabinPanels;
        /// <summary>
        /// بلندگو
        /// </summary>
        public static Dictionary<short, string> Speakers;
        /// <summary>
        /// چراغ اضطراری
        /// </summary>
        public static Dictionary<short, string> EmergencyLights;
        /// <summary>
        /// پوش باتون
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> PushButtons;
        /// <summary>
        /// فلز رویه داخل کابین
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> CabinSurfaceMetals;
        /// <summary>
        /// نحوه نصب
        /// </summary>
        public static Dictionary<short, string> InstallationTypes;
        /// <summary>
        /// نمایشگر داخل کابین
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> Monitors;


        /// <summary>
        /// تعداد آسانسور
        /// </summary>
        public static Dictionary<short, string> ElevatorCounts;
        /// <summary>
        /// تعداد شاسی طبقات
        /// </summary>
        public static Dictionary<short, string> HallPushButtonCount;
        /// <summary>
        /// مدل پنل طبقات
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> HallPanels;
        /// <summary>
        /// فلز رویه طبقات
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> HallSurfaceMetals;


        /// <summary>
        /// مدل پنل سردرب
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> DoorTopPanels;
        /// <summary>
        /// فلز رویه
        /// (id,name,description)
        /// </summary>
        public static Dictionary<int, CacheItem> SurfaceMetals;
        /// <summary>
        /// شناسه شروع فلزهای رویه، قبل از این شناسه مواردی مانند تامین توسط مشتری قرار دارد
        /// </summary>
        public static int SurfaceMetalsStartId = 10;


        /// <summary>
        /// وضعیت سفارش
        /// </summary>
        public static Dictionary<byte, string> Order_OrderStatus;
        /// <summary>
        /// نوع معامله
        /// </summary>
        public static Dictionary<byte, string> Order_TradeTypes;
        /// <summary>
        /// وضعیت تولید سفارش
        /// </summary>
        public static Dictionary<byte, string> Order_ProductStatus;
        /// <summary>
        /// وضعیت سنددریافتی
        /// </summary>
        public static Dictionary<byte, string> Payment_PaymentStatus;
        /// <summary>
        /// نوع سفارش
        /// </summary>
        public static Dictionary<byte, string> Order_OrderTypes;

        /// <summary>
        /// ملحقات
        /// </summary>
        public static Dictionary<int, CacheItem> Order_Attachments;
        /// <summary>
        /// کسورات
        /// </summary>
        public static Dictionary<int, string> Order_Deductions;
        /// <summary>
        /// اضافات
        /// </summary>
        public static Dictionary<int, string> Order_Additions;


        /// <summary>
        /// نحوه وصول
        /// </summary>
        public static Dictionary<byte, string> Payment_Types;
        /// <summary>
        /// بابت
        /// </summary>
        public static Dictionary<byte, string> Payment_Babats;
        /// <summary>
        /// بانک ها
        /// </summary>
        public static Dictionary<short, string> Payment_Banks;


        /// <summary>
        /// وضعیت تحویل
        /// </summary>
        public static Dictionary<byte, string> Delivery_DeliveryStatus;
        /// <summary>
        /// وسایل نقلیه
        /// </summary>
        public static Dictionary<byte, string> Delivery_VehicleTypes;
        /// <summary>
        /// محل تحویل کالا
        /// </summary>
        public static Dictionary<byte, string> Delivery_DeliveryLocations;

        /// <summary>
        /// اولویت
        /// </summary>
        public static Dictionary<byte, string> CRM_Priorities;
        /// <summary>
        /// نوع فعالیت
        /// </summary>
        public static Dictionary<byte, string> CRM_ActivityTypes;
        /// <summary>
        /// وضعیت فعالیت
        /// </summary>
        public static Dictionary<byte, string> CRM_ActivityStates;

        static Cache()
        {
            Update();
        }

        public static void Update()
        {
            using (var context = new PantaEntities())
            {
                Categories = context.Account_Categories.ToList();
                Posts = context.Account_Posts.ToList();
                EntityStatuses = context.BaseSystem_EntityStatus.ToDictionary(x => (Status)x.Id, x => x.Name);
                UserStatuses = context.Account_UserStatus.ToDictionary(x => (UserStatus)x.Id, x => x.Name);
                Account_Users = context.Account_Users.Where(x => x.Id > 0).Where(x => x.Status != (byte)UserStatus.Deleted).OrderBy(x => x.Name).ToDictionary(x => x.Id, x => x.FullName);
                Account_PersonTypes = context.Account_PersonTypes.ToDictionary(x => x.Id, x => x.Name);
                Account_PartnerTypes = context.Account_PartnerTypes.ToDictionary(x => x.Id, x => x.Name);
                Tables = context.BaseSystem_Tables.ToList();

                ElevatorBoards = context.Tb_ElevatorBoards.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => x.Name);
                PackTypes = context.Tb_PackTypes.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => x.Name);
                CabinPanels = context.Tb_CabinPanels.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));
                Speakers = context.Tb_Speakers.ToDictionary(x => x.Id, x => x.Name);
                EmergencyLights = context.Tb_EmergencyLights.ToDictionary(x => x.Id, x => x.Name);
                PushButtons = context.Tb_PushButtons.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));
                CabinSurfaceMetals = context.Tb_CabinSurfaceMetals.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));
                InstallationTypes = context.Tb_InstallationTypes.ToDictionary(x => x.Id, x => x.Name);
                Monitors = context.Tb_Monitors.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));

                ElevatorCounts = context.Tb_ElevatorCounts.ToDictionary(x => x.Id, x => x.Name);
                HallPushButtonCount = context.Tb_HallPushButtonCounts.ToDictionary(x => x.Id, x => x.Name);
                HallPanels = context.Tb_HallPanels.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));
                HallSurfaceMetals = context.Tb_HallSurfaceMetals.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));

                DoorTopPanels = context.Tb_DoorTopPanels.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost, x.SurfaceArea));
                SurfaceMetals = context.Tb_SurfaceMetals.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));

                Order_OrderStatus = context.Order_Status.ToDictionary(x => x.Id, x => x.Name);
                Order_TradeTypes = context.Tb_TradeTypes.ToDictionary(x => x.Id, x => x.Name);
                Order_ProductStatus = context.Order_ProductStatus.ToDictionary(x => x.Id, x => x.Name);
                Payment_PaymentStatus = context.Payment_Status.ToDictionary(x => x.Id, x => x.Name);
                Order_OrderTypes = context.Tb_OrderTypes.ToDictionary(x => x.Id, x => x.Name);
                Order_Attachments = context.Tb_Attachments.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => new CacheItem(x.Name, x.Description, x.Cost));
                Order_Deductions = context.Tb_Deductions.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => x.Name);
                Order_Additions = context.Tb_Additions.Where(x => x.Deleted == false).ToDictionary(x => x.Id, x => x.Name);

                Payment_Types = context.Payment_Types.ToDictionary(x => x.Id, x => x.Name);
                Payment_Babats = context.Payment_Babats.ToDictionary(x => x.Id, x => x.Name);
                Payment_Banks = context.Payment_Banks.ToDictionary(x => x.Id, x => x.Name);

                Delivery_DeliveryStatus = context.Delivery_Status.ToDictionary(x => x.Id, x => x.Name);
                Delivery_VehicleTypes = context.Delivery_VehicleTypes.ToDictionary(x => x.Id, x => x.Name);
                Delivery_DeliveryLocations = context.Delivery_DeliveryLocations.ToDictionary(x => x.Id, x => x.Name);

                CRM_Priorities = context.CRM_Priority.ToDictionary(x => x.Id, x => x.Name);
                CRM_ActivityTypes = context.CRM_ActivityType.ToDictionary(x => x.Id, x => x.Name);
                CRM_ActivityStates = context.CRM_ActivityState.ToDictionary(x => x.Id, x => x.Name);
            }
        }

    }
}