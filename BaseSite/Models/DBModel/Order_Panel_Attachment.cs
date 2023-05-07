using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_Panel_Attachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Nullable<int> CabinPanelId { get; set; }
        public Nullable<int> HallPanelId { get; set; }
        public Nullable<int> DoorTopPanelId { get; set; }
        public int Count { get; set; }
        public int AttachmentId { get; set; }
        public double Cost { get; set; }
        public Nullable<int> DeliveryId { get; set; }
        [MaxLength(255)]
        public string DeliveryComment { get; set; }


        [ForeignKey("DeliveryId")]
        public virtual Delivery_Delivery Delivery_Delivery { get; set; }
        [ForeignKey("CabinPanelId")]
        public virtual Order_Cabin Order_Cabin { get; set; }
        [ForeignKey("DoorTopPanelId")]
        public virtual Order_DoorTop Order_DoorTop { get; set; }
        [ForeignKey("HallPanelId")]
        public virtual Order_Hall Order_Hall { get; set; }
        [ForeignKey("AttachmentId")]
        public virtual Tb_Attachments Tb_Attachments { get; set; }
    }
}
