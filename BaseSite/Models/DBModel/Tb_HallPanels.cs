using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_HallPanels: Tb_Panel
    {
        public Tb_HallPanels()
        {
            this.Order_Hall = new HashSet<Order_Hall>();
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
        [ForeignKey("HallPanelId")]
        public virtual ICollection<Order_Hall> Order_Hall { get; set; }
        [ForeignKey("StartFrom")]
        public virtual Order_ProductStatus Order_ProductStatus { get; set; }
    }
}
