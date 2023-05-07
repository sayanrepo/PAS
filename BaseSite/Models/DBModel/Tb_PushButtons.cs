using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_PushButtons
    {
        public Tb_PushButtons()
        {
            this.Order_Cabin = new HashSet<Order_Cabin>();
            this.Order_Hall = new HashSet<Order_Hall>();
        }
    
        public int TableId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public double Cost { get; set; }
        public double ProductFactor { get; set; }
        public Nullable<bool> Available { get; set; }


        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("PushButtonId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
        [ForeignKey("PushButtonId")]
        public virtual ICollection<Order_Hall> Order_Hall { get; set; }
    }
}
