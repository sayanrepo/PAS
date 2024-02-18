using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Panel
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public double Depth { get; set; }
    }

    public partial class Tb_CabinPanels:Tb_Panel
    {
        public Tb_CabinPanels()
        {
            this.Order_Cabin = new HashSet<Order_Cabin>();
        }

        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public bool Deleted { get; set; }
        public double Cost { get; set; }
        public double ProductFactor { get; set; }
        public Nullable<bool> Available { get; set; }
        public byte StartFrom { get; set; }
        public double SurfaceArea { get; set; }


        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("CabinPanelId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
        [ForeignKey("StartFrom")]
        public virtual Order_ProductStatus Order_ProductStatus { get; set; }
    }
}
