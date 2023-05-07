using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_OrderTypes
    {
        public Tb_OrderTypes()
        {
            this.Order_Order = new HashSet<Order_Order>();
            this.Sale_Sale = new HashSet<Sale_Sale>();
            this.Service_Service = new HashSet<Service_Service>();
        }
    
        public byte Id { get; set; }
        public string Name { get; set; }


        [ForeignKey("OrderTypeId")]
        public virtual ICollection<Order_Order> Order_Order { get; set; }
        [ForeignKey("OrderTypeId")]
        public virtual ICollection<Sale_Sale> Sale_Sale { get; set; }
        [ForeignKey("OrderTypeId")]
        public virtual ICollection<Service_Service> Service_Service { get; set; }
    }
}
