using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Location_Cities
    {
        public Location_Cities()
        {
            this.Account_Users = new HashSet<Account_Users>();
            this.Account_Users1 = new HashSet<Account_Users>();
            this.Order_Order = new HashSet<Order_Order>();
            this.Sale_Sale = new HashSet<Sale_Sale>();
            this.Service_Service = new HashSet<Service_Service>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        public int ProvinceId { get; set; }
        public bool Deleted { get; set; }



        [ForeignKey("CityId1")]
        public virtual ICollection<Account_Users> Account_Users { get; set; }
        [ForeignKey("CityId2")]
        public virtual ICollection<Account_Users> Account_Users1 { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Location_Provinces Location_Provinces { get; set; }
        [ForeignKey("DeliveryCityId")]
        public virtual ICollection<Order_Order> Order_Order { get; set; }
        [ForeignKey("DeliveryCityId")]
        public virtual ICollection<Sale_Sale> Sale_Sale { get; set; }
        [ForeignKey("DeliveryCityId")]
        public virtual ICollection<Service_Service> Service_Service { get; set; }
    }
}
