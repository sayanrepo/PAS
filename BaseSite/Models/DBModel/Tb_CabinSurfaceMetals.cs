using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_CabinSurfaceMetals
    {
        public Tb_CabinSurfaceMetals()
        {
            this.Order_Cabin = new HashSet<Order_Cabin>();
            this.Order_Cabin1 = new HashSet<Order_Cabin>();
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


        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("SurfaceMetalId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
        [ForeignKey("SurfaceMetalId2")]
        public virtual ICollection<Order_Cabin> Order_Cabin1 { get; set; }
    }
}
