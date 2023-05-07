using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Truth
    {
        public int PrimaryTableId { get; set; }
        public int PrimaryId { get; set; }
        public int SecondaryTableId { get; set; }
        public int SecondaryId { get; set; }
        public double TValue { get; set; }
        public int Id { get; set; }


        [ForeignKey("PrimaryTableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("SecondaryTableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables1 { get; set; }
    }
}
