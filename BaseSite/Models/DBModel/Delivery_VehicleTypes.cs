using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Delivery_VehicleTypes
    {
        public Delivery_VehicleTypes()
        {
            this.Delivery_Delivery = new HashSet<Delivery_Delivery>();
        }
    
        [Key]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

        [ForeignKey("VehicleTypeId")]
        public virtual ICollection<Delivery_Delivery> Delivery_Delivery { get; set; }
    }
}
