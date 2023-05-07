using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_ProductStatus
    {
        public Order_ProductStatus()
        {
            this.Order_Cabin = new HashSet<Order_Cabin>();
            this.Order_DoorTop = new HashSet<Order_DoorTop>();
            this.Order_Hall = new HashSet<Order_Hall>();
            this.Order_Process = new HashSet<Order_Process>();
            this.Tb_CabinPanels = new HashSet<Tb_CabinPanels>();
            this.Tb_DoorTopPanels = new HashSet<Tb_DoorTopPanels>();
            this.Tb_HallPanels = new HashSet<Tb_HallPanels>();
        }

        [Key]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }


        [ForeignKey("ProductStatusId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
        [ForeignKey("ProductStatusId")]
        public virtual ICollection<Order_DoorTop> Order_DoorTop { get; set; }
        [ForeignKey("ProductStatusId")]
        public virtual ICollection<Order_Hall> Order_Hall { get; set; }
        [ForeignKey("ProductStatusId")]
        public virtual ICollection<Order_Process> Order_Process { get; set; }
        [ForeignKey("StartFrom")]
        public virtual ICollection<Tb_CabinPanels> Tb_CabinPanels { get; set; }
        [ForeignKey("StartFrom")]
        public virtual ICollection<Tb_DoorTopPanels> Tb_DoorTopPanels { get; set; }
        [ForeignKey("StartFrom")]
        public virtual ICollection<Tb_HallPanels> Tb_HallPanels { get; set; }
    }
}
