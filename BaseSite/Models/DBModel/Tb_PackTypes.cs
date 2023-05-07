using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_PackTypes
    {
        public Tb_PackTypes()
        {
            this.Delivery_Delivery = new HashSet<Delivery_Delivery>();
            this.Order_Order = new HashSet<Order_Order>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }


        [ForeignKey("PackTypeId")]
        public virtual ICollection<Delivery_Delivery> Delivery_Delivery { get; set; }
        [ForeignKey("PackTypeId")]
        public virtual ICollection<Order_Order> Order_Order { get; set; }
    }
}
