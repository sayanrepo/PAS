using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Deductions
    {
        public Tb_Deductions()
        {
            this.Order_Deduction = new HashSet<Order_Deduction>();
        }

        public int TableId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }


        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("DeductionId")]
        public virtual ICollection<Order_Deduction> Order_Deduction { get; set; }
    }
}
