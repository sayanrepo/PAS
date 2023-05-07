using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_DoorTop
    {
        public Order_DoorTop()
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
        public int DoorTopPanelId { get; set; }
        public int SurfaceMetalId { get; set; }
        public byte ProductStatusId { get; set; }
        public Nullable<byte> ProductPriority { get; set; }
        [MaxLength(255)]
        public string Comment { get; set; }
        public double Cost { get; set; }
        public Nullable<int> DeliveryId { get; set; }

        [MaxLength(255)]
        public string DeliveryComment { get; set; }

        public double CostDoorTopPanel { get; set; }
        public double CostSurfaceMetal { get; set; }

        public int MonitorId { get; set; }

        public double SurfaceMetalDosage { get; set; }
        public double CostMonitor { get; set; }



        [ForeignKey("DeliveryId")]
        public virtual Delivery_Delivery Delivery_Delivery { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order_Order Order_Order { get; set; }
        [ForeignKey("ProductStatusId")]
        public virtual Order_ProductStatus Order_ProductStatus { get; set; }
        [ForeignKey("DoorTopPanelId")]
        public virtual Tb_DoorTopPanels Tb_DoorTopPanels { get; set; }
        [ForeignKey("DoorTopPanelId")]
        public virtual ICollection<Order_Panel_Addition> Order_Panel_Addition { get; set; }
        [ForeignKey("DoorTopPanelId")]
        public virtual ICollection<Order_Panel_Attachment> Order_Panel_Attachment { get; set; }
        [ForeignKey("MonitorId")]
        public virtual Tb_Monitors Tb_Monitors { get; set; }
        [ForeignKey("SurfaceMetalId")]
        public virtual Tb_SurfaceMetals Tb_SurfaceMetals { get; set; }
    }
}
