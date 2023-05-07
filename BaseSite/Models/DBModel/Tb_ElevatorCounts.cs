using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_ElevatorCounts
    {
        public Tb_ElevatorCounts()
        {
            this.Order_Hall = new HashSet<Order_Hall>();
        }
    
        public short Id { get; set; }
        public string Name { get; set; }


        [ForeignKey("ElevatorTypeId")]
        public virtual ICollection<Order_Hall> Order_Hall { get; set; }
    }
}
