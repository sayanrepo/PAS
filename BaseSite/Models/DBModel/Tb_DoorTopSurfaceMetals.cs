using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_DoorTopSurfaceMetals
    {
        public int TableId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public double Cost { get; set; }
        public double ProductFactor { get; set; }
        public Nullable<bool> Available { get; set; }

        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
    }
}
