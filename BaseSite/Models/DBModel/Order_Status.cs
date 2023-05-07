using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_Status
    {
        public Order_Status()
        {
            this.Order_Order = new HashSet<Order_Order>();
            this.Sale_Sale = new HashSet<Sale_Sale>();
            this.Service_Service = new HashSet<Service_Service>();
        }

        [Key]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }


        [ForeignKey("StatusId")]
        public virtual ICollection<Order_Order> Order_Order { get; set; }
        [ForeignKey("StatusId")]
        public virtual ICollection<Sale_Sale> Sale_Sale { get; set; }
        [ForeignKey("StatusId")]
        public virtual ICollection<Service_Service> Service_Service { get; set; }
    }
}
