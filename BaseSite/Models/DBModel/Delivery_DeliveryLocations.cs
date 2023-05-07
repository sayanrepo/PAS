using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Delivery_DeliveryLocations
    {
        public Delivery_DeliveryLocations()
        {
            this.Delivery_Delivery = new HashSet<Delivery_Delivery>();
        }
    
        [Key]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }


        [ForeignKey("DeliveryLocationId")]
        public virtual ICollection<Delivery_Delivery> Delivery_Delivery { get; set; }
    }
}
