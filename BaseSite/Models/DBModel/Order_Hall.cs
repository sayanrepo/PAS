using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_Hall
    {
        public Order_Hall()
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
        public short ElevatorTypeId { get; set; }
        public short PushButtonCountId { get; set; }
        public int HallPanelId { get; set; }
        public int PushButtonId { get; set; }
        public int SurfaceMetalId { get; set; }
        public int MonitorId { get; set; }

        public int FloorCount { get; set; }
        [MaxLength(50)]
        public string FloorNames { get; set; }
        public int UGFloorCount { get; set; }
        [MaxLength(50)]
        public string UGFloorNames { get; set; }

        public byte ProductStatusId { get; set; }
        public Nullable<byte> ProductPriority { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }

        public double Cost { get; set; }
        public Nullable<int> DeliveryId { get; set; }

        [MaxLength(255)]
        public string DeliveryComment { get; set; }

        public double CostHallPanel { get; set; }
        public double CostSurfaceMetal { get; set; }
        public double CostMonitor { get; set; }
        public double CostPushButton { get; set; }


        [ForeignKey("DeliveryId")]
        public virtual Delivery_Delivery Delivery_Delivery { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order_Order Order_Order { get; set; }
        [ForeignKey("ProductStatusId")]
        public virtual Order_ProductStatus Order_ProductStatus { get; set; }
        [ForeignKey("ElevatorTypeId")]
        public virtual Tb_ElevatorCounts Tb_ElevatorCounts { get; set; }
        [ForeignKey("HallPanelId")]
        public virtual Tb_HallPanels Tb_HallPanels { get; set; }
        [ForeignKey("PushButtonCountId")]
        public virtual Tb_HallPushButtonCounts Tb_HallPushButtonCounts { get; set; }
        [ForeignKey("SurfaceMetalId")]
        public virtual Tb_HallSurfaceMetals Tb_HallSurfaceMetals { get; set; }
        [ForeignKey("MonitorId")]
        public virtual Tb_Monitors Tb_Monitors { get; set; }
        [ForeignKey("PushButtonId")]
        public virtual Tb_PushButtons Tb_PushButtons { get; set; }
        [ForeignKey("HallPanelId")]
        public virtual ICollection<Order_Panel_Addition> Order_Panel_Addition { get; set; }
        [ForeignKey("HallPanelId")]
        public virtual ICollection<Order_Panel_Attachment> Order_Panel_Attachment { get; set; }
    }
}
