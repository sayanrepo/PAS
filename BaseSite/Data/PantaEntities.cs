namespace BaseSite.Data
{
    using BaseSite.Models.DBModel;
    using System.Data.Entity;

    public partial class PantaEntities : DbContext
    {
        public PantaEntities()
            : base("name=PantaEntities")
        {
        }

        public DbSet<Account_Categories> Account_Categories { get; set; }
        public DbSet<Account_Operations> Account_Operations { get; set; }
        public DbSet<Account_PartnerTypes> Account_PartnerTypes { get; set; }
        public DbSet<Account_PersonTypes> Account_PersonTypes { get; set; }
        public DbSet<Account_FindoutWays> Account_FindoutWays { get; set; }
        public DbSet<Account_PostOperation> Account_PostOperation { get; set; }
        public DbSet<Account_Posts> Account_Posts { get; set; }
        public DbSet<Account_UserPost> Account_UserPost { get; set; }
        public DbSet<Account_Users> Account_Users { get; set; }
        public DbSet<Account_UserStatus> Account_UserStatus { get; set; }
        public DbSet<BaseSystem_EntityStatus> BaseSystem_EntityStatus { get; set; }
        public DbSet<BaseSystem_Tables> BaseSystem_Tables { get; set; }
        public DbSet<Delivery_Delivery> Delivery_Delivery { get; set; }
        public DbSet<Delivery_DeliveryLocations> Delivery_DeliveryLocations { get; set; }
        public DbSet<Delivery_Status> Delivery_Status { get; set; }
        public DbSet<Delivery_VehicleTypes> Delivery_VehicleTypes { get; set; }
        public DbSet<Document_Categories> Document_Categories { get; set; }
        public DbSet<Document_Documents> Document_Documents { get; set; }
        public DbSet<Document_FileTypes> Document_FileTypes { get; set; }
        public DbSet<Location_Cities> Location_Cities { get; set; }
        public DbSet<Location_Countries> Location_Countries { get; set; }
        public DbSet<Location_Provinces> Location_Provinces { get; set; }
        public DbSet<Log_Stages> Log_Stages { get; set; }
        public DbSet<Order_Cabin> Order_Cabin { get; set; }
        public DbSet<Order_Deduction> Order_Deduction { get; set; }
        public DbSet<Order_DoorTop> Order_DoorTop { get; set; }
        public DbSet<Order_Hall> Order_Hall { get; set; }
        public DbSet<Order_Order> Order_Order { get; set; }
        public DbSet<Order_Panel_Addition> Order_Panel_Addition { get; set; }
        public DbSet<Order_Panel_Attachment> Order_Panel_Attachment { get; set; }
        public DbSet<Order_Process> Order_Process { get; set; }
        public DbSet<Order_ProductStatus> Order_ProductStatus { get; set; }
        public DbSet<Order_Status> Order_Status { get; set; }
        public DbSet<Payment_Babats> Payment_Babats { get; set; }
        public DbSet<Payment_Banks> Payment_Banks { get; set; }
        public DbSet<Payment_Payment> Payment_Payment { get; set; }
        public DbSet<Payment_Types> Payment_Types { get; set; }
        public DbSet<Sale_Goods> Sale_Goods { get; set; }
        public DbSet<Sale_Sale> Sale_Sale { get; set; }
        public DbSet<Service_Service> Service_Service { get; set; }
        public DbSet<Tb_Additions> Tb_Additions { get; set; }
        public DbSet<Tb_Attachments> Tb_Attachments { get; set; }
        public DbSet<Tb_CabinPanels> Tb_CabinPanels { get; set; }
        public DbSet<Tb_CabinSurfaceMetals> Tb_CabinSurfaceMetals { get; set; }
        public DbSet<Tb_CollectiveProducePercent> Tb_CollectiveProducePercent { get; set; }
        public DbSet<Tb_Deductions> Tb_Deductions { get; set; }
        public DbSet<Tb_DoorTopPanels> Tb_DoorTopPanels { get; set; }
        public DbSet<Tb_DoorTopSurfaceMetals> Tb_DoorTopSurfaceMetals { get; set; }
        public DbSet<Tb_ElevatorBoards> Tb_ElevatorBoards { get; set; }
        public DbSet<Tb_ElevatorCounts> Tb_ElevatorCounts { get; set; }
        public DbSet<Tb_HallPanels> Tb_HallPanels { get; set; }
        public DbSet<Tb_HallPushButtonCounts> Tb_HallPushButtonCounts { get; set; }
        public DbSet<Tb_HallSurfaceMetals> Tb_HallSurfaceMetals { get; set; }
        public DbSet<Tb_InstallationTypes> Tb_InstallationTypes { get; set; }
        public DbSet<Tb_Monitors> Tb_Monitors { get; set; }
        public DbSet<Tb_OrderTypes> Tb_OrderTypes { get; set; }
        public DbSet<Tb_PackTypes> Tb_PackTypes { get; set; }
        public DbSet<Tb_ProductFactorCost> Tb_ProductFactorCost { get; set; }
        public DbSet<Tb_PushButtons> Tb_PushButtons { get; set; }
        public DbSet<Tb_Speakers> Tb_Speakers { get; set; }
        public DbSet<Tb_EmergencyLights> Tb_EmergencyLights { get; set; }
        public DbSet<Tb_Costs> Tb_Costs { get; set; }
        public DbSet<Log_Logs> Log_Logs { get; set; }
        public DbSet<Payment_Status> Payment_Status { get; set; }
        public DbSet<Tb_SurfaceMetals> Tb_SurfaceMetals { get; set; }
        public DbSet<Tb_Truth> Tb_Truth { get; set; }
        public DbSet<Log_LogActivity> Log_LogActivity { get; set; }
        public DbSet<CRM_Comments> CRM_Comments { get; set; }
        public DbSet<CRM_Activity> CRM_Activity { get; set; }
        public DbSet<CRM_ActivityState> CRM_ActivityState { get; set; }
        public DbSet<CRM_ActivityType> CRM_ActivityType { get; set; }
        public DbSet<CRM_Priority> CRM_Priority { get; set; }
        public DbSet<Tb_Products> Tb_Products { get; set; }
        public DbSet<Tb_Stores> Tb_Stores { get; set; }
        public DbSet<Tb_TradeTypes> Tb_TradeTypes { get; set; }
    }
}