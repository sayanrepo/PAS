using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BaseSite.Models
{
    public enum DB_Table : int
    {
        Others = 0,
        Account_Users = 1,
        Account_Categories = 2,
        Tb_CabinPanels = 3,
        Tb_PushButtons = 4,
        Tb_SurfaceMetals = 5,
        Tb_Monitors = 6,
        Tb_HallPanels = 7,
        Tb_DoorTopPanels = 10,
        Tb_Attachments = 11,
        Tb_Deductions = 12,
        Tb_Additions = 13,
        Order_Order = 14,
        Order_Cabin = 15,
        Order_Hall = 16,
        Order_DoorTop = 17,
        Sale_Sale = 18,
        Payment_Payment = 19,
        Delivery_Delivery = 20,
        Service_Service = 21,
        Tb_Products = 22
    }

    public enum Status : byte
    {
        Active = 1,
        DeActive = 2,
        Deleted = 3
    }

    public enum OrderStatus : byte
    {
        PishFactor = 1,
        DarDasteEghdam = 2,
        DarkhasteTolid = 3,
        DarJaryaneTolid = 4,
        TolidShode = 5,
        AmadeTahvil = 6,
        MojavezKhorooj = 7,
        ErsalShode = 8,
        TahvilShode = 9,
        Raked = 10
    }

    public enum ProductStatus : byte
    {
        NagsheKeshi = 1,
        MashinkariTarh = 2,
        Anbar = 3,
        Montaj = 4,
        QC = 5,
        BasteBandi = 6,
        AmadeErsal = 7
    }

    public enum PaymentStatus : byte
    {
        TayidNashode = 1,
        TayidForosh = 2,
        TayidMali = 3
    }

    public enum DeliveryStatus : byte
    {
        SaderShode = 1,
        TayidShode = 2,
        ErsalShode = 3
    }

    public enum Department : byte
    {
        Unknown = 0,
        Foroosh = 1,
        Tolid = 2,
    }

    public enum AccountRole : int
    {
        Unknown = 0,
        Foroosh_InternetCustomer = 1,
        Foroosh_Operator = 2,
        Foroosh_Assistant = 3,
        Foroosh_Mali = 4,
        Foroosh_Manager = 5,
        Foroosh_Admin = 6,
        Product_Operator = 7,
        Product_Assistant = 8,
        Product_Manager = 9,
        Product_Mechanical_Assembler = 10
    }

    public enum LogActivity : int
    {
        LoginFailed = 1,
        Login = 2,
        LogOut = 3,
        SessionTimeout = 4,
        Add = 5,
        Edit = 6,
        Delete = 7,
        View = 8,
        Print = 9,
        ChangeStatus = 10
    }

    public enum OPERATIONS : byte
    {
        //----------------------Sale-------------------------
        Order,                  // منوی سفارشات را می بیند
        Order_Search,           // پنل جستجوی سفارشات را مشاهده می کند
        Order_Add,              // می تواند پیش فاکتور اضافه و یا ویرایش کند
        Order_Detail,           // دکمه جزئیات را مشاهده می کند و با کلیک روی آن جزئیات سفارش را می بیند
        Order_Delete,           // دکمه حذف سفارش را در حالت پیش فاکتور مشاهده می کند و با کلیک روی آن، سفارش حذف می شود
        Order_Print,            // دکمه پرینت سفارش را می بیند و می تواند پرینت کند
        Order_Edit_Factor,      // فاکتور را جاری می کند و تا درخواست تولید جلو می برد
        Order_ChangeStatus,     // می تواند وضعیت سفارش را به هر وضعیتی که خواست تغییر دهد

        Sale,
        Sale_Search,
        Sale_Add,
        Sale_Detail,
        Sale_Delete,
        Sale_Print,
        Sale_Edit,
        Sale_ChangeStatus,

        Store,
        Store_Search,
        Store_Add,
        Store_Detail,
        Store_Delete,
        Store_Print,
        Store_Edit,
        Store_ChangeStatus,

        Service,
        Service_Search,
        Service_Add,
        Service_Detail,
        Service_Delete,
        Service_Print,
        Service_Edit,
        Service_ChangeStatus,

        Payment,
        Payment_Search,
        Payment_Add,
        Payment_Detail,
        Payment_Delete,
        Payment_Print,
        Payment_ForoshConfirm,
        Payment_MaliConfirm,
        Payment_ChangeStatus,

        Delivery,
        Delivery_Search,
        Delivery_Add,
        Delivery_Detail,
        Delivery_Print,
        Delivery_ChangeStatus,
        Delivery_Confirm,

        //--------------------Product-------------------------
        Cartable,
        Cartable_Search,
        Cartable_Detail,

        Plan,
        Plan_Search,
        Plan_Detail,
        Plan_Print,
        Plan_StartCommand,
        Plan_FinishCommand,

        Product,
        Product_Search,
        Product_Detail,
        Product_Print,

        Process,
        Process_Backward,
        Process_Project,

        //--------------------Reports-------------------------
        Report,
        Report_ProductFactor,
        Report_productFactor_AllOperators,
        Report_CustomerBill,
        Report_CustomersBill,
        Report_Statistic,
        Report_Statistic2,
        Report_SaleControlling,
        Report_Orders_Monthly_OrderDate,
        Report_Orders_Monthly_FactorDate,
        Report_Sales_Payments_Monthly,
        Report_Lending,
        Report_KPI,
        Report_CustomersInfo,

        //--------------------Setting-------------------------
        Setting,
        Setting_Cities,
        Setting_Cities_Add,
        Setting_Cities_Edit,
        Setting_Cities_Delete,

        Setting_Persons,
        Setting_Persons_Customer,
        Setting_Persons_Foroosh,
        Setting_Persons_Tolid,
        Setting_Persons_Search,
        Setting_Persons_Detail,
        Setting_Persons_Edit,
        Setting_Persons_Add,
        Setting_Persons_AssignUserName,
        Setting_Persons_AssignAccess,

        Setting_Order,
        Setting_Order_Packet,
        Setting_Order_Packet_Add,
        Setting_Order_Packet_Edit,
        Setting_Order_Packet_Delete,

        Setting_Order_ElevatorBoard,
        Setting_Order_ElevatorBoard_Add,
        Setting_Order_ElevatorBoard_Edit,
        Setting_Order_ElevatorBoard_Delete,

        Setting_Order_Deduction,
        Setting_Order_Deduction_Add,
        Setting_Order_Deduction_Edit,
        Setting_Order_Deduction_Delete,

        Setting_Order_Addition,
        Setting_Order_Addition_Add,
        Setting_Order_Addition_Edit,
        Setting_Order_Addition_Delete,

        Setting_Attachment,
        Setting_Attachment_Add,
        Setting_Attachment_Edit,
        Setting_Attachment_Delete,

        Setting_PushButton,
        Setting_PushButton_Add,
        Setting_PushButton_Edit,
        Setting_PushButton_Delete,

        Setting_Monitor,
        Setting_Monitor_Add,
        Setting_Monitor_Edit,
        Setting_Monitor_Delete,

        Setting_CabinPanel,
        Setting_CabinPanel_Add,
        Setting_CabinPanel_Edit,
        Setting_CabinPanel_Delete,

        Setting_CabinSurfaceMetal,
        Setting_CabinSurfaceMetal_Add,
        Setting_CabinSurfaceMetal_Edit,
        Setting_CabinSurfaceMetal_Delete,

        Setting_HallPanel,
        Setting_HallPanel_Add,
        Setting_HallPanel_Edit,
        Setting_HallPanel_Delete,

        Setting_HallSurfaceMetal,
        Setting_HallSurfaceMetal_Add,
        Setting_HallSurfaceMetal_Edit,
        Setting_HallSurfaceMetal_Delete,

        Setting_DoorTopPanel,
        Setting_DoorTopPanel_Add,
        Setting_DoorTopPanel_Edit,
        Setting_DoorTopPanel_Delete,

        Setting_DoorTopSurfaceMetal,
        Setting_DoorTopSurfaceMetal_Add,
        Setting_DoorTopSurfaceMetal_Edit,
        Setting_DoorTopSurfaceMetal_Delete,

        Setting_ProductFactorCost,
        Setting_ProductFactorCost_Add,
        Setting_ProductFactorCost_Edit,
        Setting_ProductFactorCost_Delete,

        Setting_CollectiveProducePercent,
        Setting_CollectiveProducePercent_Add,
        Setting_CollectiveProducePercent_Edit,
        Setting_CollectiveProducePercent_Delete,

        Setting_Product,
        Setting_Product_Add,
        Setting_Product_Edit,
        Setting_Product_Delete,

        Setting_Name,
        Setting_Description,
        Setting_Cost,
        Setting_ProductFactor,
        Setting_Available,
        Setting_SurfaceArea,

        Setting_TruthTable,
        Setting_TruthTable_Edit,

        //--------------------Logs-------------------------
        Logs_Logs,
        Logs_Search,
        Logs_Detail,

        //--------------------Help-------------------------
        Help_Help,

        //--------------------CRM-------------------------
        CRM,
        CRM_Persons
    }

    public enum CrmActivityState : byte
    {
        Open = 1,
        Done = 2,
        Canceled = 3
    }

    public enum CrmActivityType : byte
    {
        PhoneCall = 1,
        Message = 2,
        Fax = 3,
        Email = 4,
        Note = 5,
        Meeting = 6,
        Support = 7,
        General = 10
    }

    public class Panel
    {
        public string ShDateOrder { get; set; }
        public string ShDateDelivery { get; set; }
        public string ShDateFactor { get; set; }
        public string CustomerName { get; set; }
        public string AccepterName { get; set; }
        public string ProjectName { get; set; }
        public int TableId { get; set; }
        public string Type { get; set; }
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderStatusName { get; set; }
        public int Count { get; set; }
        public int PanelId { get; set; }
        public string PanelName { get; set; }
        public int PushButtonId { get; set; }
        public string PushButtonName { get; set; }
        public int SurfaceMetalId { get; set; }
        public string SurfaceMetalName { get; set; }
        public int MonitorId { get; set; }
        public string MonitorName { get; set; }
        public byte StartFrom { get; set; }
        public string Comment { get; set; }
        public double Cost { get; set; }
        public int DocNumber { get; set; }
        public byte ProductStatusId { get; set; }
        public string ProductStatusName { get; set; }
        public Nullable<byte> ProductPriority { get; set; }

        public Panel(Models.DBModel.Order_Cabin cabin)
        {
            this.TableId = cabin.TableId;
            this.Type = "داخل کابین";
            this.Id = cabin.Id;
            this.OrderId = cabin.OrderId;
            this.Count = cabin.Count;
            this.PanelId = cabin.CabinPanelId;
            this.PanelName = cabin.Tb_CabinPanels.Name;
            this.PushButtonId = cabin.PushButtonId;
            this.PushButtonName = cabin.Tb_PushButtons.Name;
            this.SurfaceMetalId = cabin.SurfaceMetalId;
            this.SurfaceMetalName = cabin.Tb_CabinSurfaceMetals.Name;
            this.MonitorId = cabin.MonitorId;
            this.MonitorName = cabin.Tb_Monitors.Name;
            this.StartFrom = cabin.Tb_CabinPanels.StartFrom;
            this.Comment = cabin.Comment;
            this.Cost = cabin.Cost;
            this.DocNumber = cabin.DocNumber;
            this.ProductStatusId = cabin.ProductStatusId;
            this.ProductStatusName = cabin.Order_ProductStatus.Name;
            this.ProductPriority = cabin.ProductPriority;
        }

        public Panel(Models.DBModel.Order_Hall hall)
        {
            this.TableId = hall.TableId;
            this.Type = "طبقات";
            this.Id = hall.Id;
            this.OrderId = hall.OrderId;
            this.Count = hall.Count;
            this.PanelId = hall.HallPanelId;
            this.PanelName = hall.Tb_HallPanels.Name;
            this.PushButtonId = hall.PushButtonId;
            this.PushButtonName = hall.Tb_PushButtons.Name;
            this.SurfaceMetalId = hall.SurfaceMetalId;
            this.SurfaceMetalName = hall.Tb_HallSurfaceMetals.Name;
            this.MonitorId = hall.MonitorId;
            this.MonitorName = hall.Tb_Monitors.Name;
            this.StartFrom = hall.Tb_HallPanels.StartFrom;
            this.Comment = hall.Comment;
            this.Cost = hall.Cost;
            this.DocNumber = hall.DocNumber;
            this.ProductStatusId = hall.ProductStatusId;
            this.ProductStatusName = hall.Order_ProductStatus.Name;
            this.ProductPriority = hall.ProductPriority;
        }

        public Panel(Models.DBModel.Order_DoorTop doorTop)
        {
            this.TableId = doorTop.TableId;
            this.Type = "سردرب";
            this.Id = doorTop.Id;
            this.OrderId = doorTop.OrderId;
            this.Count = doorTop.Count;
            this.PanelId = doorTop.DoorTopPanelId;
            this.PanelName = doorTop.Tb_DoorTopPanels.Name;
            this.PushButtonId = 0;
            this.PushButtonName = "";
            this.SurfaceMetalId = 0;
            this.SurfaceMetalName = "";
            this.MonitorId = 0;
            this.MonitorName = "";
            this.StartFrom = doorTop.Tb_DoorTopPanels.StartFrom;
            this.Comment = doorTop.Comment;
            this.Cost = doorTop.Cost;
            this.DocNumber = doorTop.DocNumber;
            this.ProductStatusId = doorTop.ProductStatusId;
            this.ProductStatusName = doorTop.Order_ProductStatus.Name;
            this.ProductPriority = doorTop.ProductPriority;
        }
    }

    [NotMapped]
    public class ZaribKarkard : Models.DBModel.Order_Process
    {
        public string UserFullName { get; set; }
    }

    public static class StringExtensions
    {
        public static string WithMaxLength(this string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}