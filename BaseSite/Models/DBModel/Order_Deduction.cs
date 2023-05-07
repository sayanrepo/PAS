using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_Deduction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int DeductionId { get; set; }
        public double Cost { get; set; }


        [ForeignKey("OrderId")]
        public virtual Order_Order Order_Order { get; set; }
        [ForeignKey("DeductionId")]
        public virtual Tb_Deductions Tb_Deductions { get; set; }
    }
}
