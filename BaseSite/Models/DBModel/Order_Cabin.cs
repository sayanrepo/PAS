using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_Cabin
    {
        public Order_Cabin()
        {
            this.Order_Panel_Addition = new HashSet<Order_Panel_Addition>();
            this.Order_Panel_Attachment = new HashSet<Order_Panel_Attachment>();
        }

        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int DocNumber { get; set; }
        public int Count { get; set; }
        public int CabinPanelId { get; set; }
        public short SpeakerId { get; set; }
        public short EmergencyLightId { get; set; }
        public int FloorCount { get; set; }
        [MaxLength(50)]
        public string FloorNames { get; set; }
        public int UGFloorCount { get; set; }
        [MaxLength(50)]
        public string UGFloorNames { get; set; }
        public int PushButtonId { get; set; }
        public int SurfaceMetalId { get; set; }
        public short InstallationTypeId { get; set; }
        public int MonitorId { get; set; }
        public Nullable<int> SheetNumber { get; set; }
        public bool PhoneCallButton { get; set; }
        public bool DO { get; set; }
        public bool DC { get; set; }

        [MaxLength(255)]
        public string LaserCuttingText { get; set; }

        [MaxLength(255)]
        public string LaserEngravingText { get; set; }
        public byte ProductStatusId { get; set; }
        public Nullable<byte> ProductPriority { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }
        public double Cost { get; set; }
        public Nullable<int> DeliveryId { get; set; }

        [MaxLength(255)]
        public string DeliveryComment { get; set; }

        public double CostCabinPanel { get; set; }
        public double CostSurfaceMetal { get; set; }
        public double CostMonitor { get; set; }
        public double CostPushButton { get; set; }
        public Nullable<int> SurfaceMetalId2 { get; set; }



        [ForeignKey("DeliveryId")]
        public virtual Delivery_Delivery Delivery_Delivery { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order_Order Order_Order { get; set; }
        [ForeignKey("ProductStatusId")]
        public virtual Order_ProductStatus Order_ProductStatus { get; set; }
        [ForeignKey("CabinPanelId")]
        public virtual Tb_CabinPanels Tb_CabinPanels { get; set; }
        [ForeignKey("SurfaceMetalId")]
        public virtual Tb_CabinSurfaceMetals Tb_CabinSurfaceMetals { get; set; }
        [ForeignKey("InstallationTypeId")]
        public virtual Tb_InstallationTypes Tb_InstallationTypes { get; set; }
        [ForeignKey("MonitorId")]
        public virtual Tb_Monitors Tb_Monitors { get; set; }
        [ForeignKey("PushButtonId")]
        public virtual Tb_PushButtons Tb_PushButtons { get; set; }
        [ForeignKey("SpeakerId")]
        public virtual Tb_Speakers Tb_Speakers { get; set; }
        [ForeignKey("EmergencyLightId")]
        public virtual Tb_EmergencyLights EmergencyLigh { get; set; }
        [ForeignKey("CabinPanelId")]
        public virtual ICollection<Order_Panel_Addition> Order_Panel_Addition { get; set; }
        [ForeignKey("CabinPanelId")]
        public virtual ICollection<Order_Panel_Attachment> Order_Panel_Attachment { get; set; }
        [ForeignKey("SurfaceMetalId2")]
        public virtual Tb_CabinSurfaceMetals Tb_CabinSurfaceMetals1 { get; set; }
    }
}
