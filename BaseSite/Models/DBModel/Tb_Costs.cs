using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Costs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EntityTableId { get; set; }
        public int EntityId { get; set; }
        public System.DateTime ApplyTime { get; set; }
        public double Cost { get; set; }
    

        [ForeignKey("EntityTableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
    }
}
