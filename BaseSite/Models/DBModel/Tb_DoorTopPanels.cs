using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_DoorTopPanels
    {
        public Tb_DoorTopPanels()
        {
            this.Order_DoorTop = new HashSet<Order_DoorTop>();
        }
    
        public int TableId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public double Cost { get; set; }
        public double ProductFactor { get; set; }
        public Nullable<bool> Available { get; set; }
        public byte StartFrom { get; set; }
        public double SurfaceArea { get; set; }


        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("DoorTopPanelId")]
        public virtual ICollection<Order_DoorTop> Order_DoorTop { get; set; }
        [ForeignKey("StartFrom")]
        public virtual Order_ProductStatus Order_ProductStatus { get; set; }
    }
}
