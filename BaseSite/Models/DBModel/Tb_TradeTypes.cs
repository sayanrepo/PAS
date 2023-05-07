using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_TradeTypes
    {
        public Tb_TradeTypes()
        {
            this.Order_Order = new HashSet<Order_Order>();
            this.Sale_Sale = new HashSet<Sale_Sale>();
        }
    
        public byte Id { get; set; }
        public string Name { get; set; }


        [ForeignKey("TradeTypeId")]
        public virtual ICollection<Order_Order> Order_Order { get; set; }
        [ForeignKey("TradeTypeId")]
        public virtual ICollection<Sale_Sale> Sale_Sale { get; set; }
    }
}
