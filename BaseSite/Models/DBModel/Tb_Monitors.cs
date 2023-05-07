using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Monitors
    {
        public Tb_Monitors()
        {
            this.Order_Cabin = new HashSet<Order_Cabin>();
            this.Order_Hall = new HashSet<Order_Hall>();
            this.Order_DoorTop = new HashSet<Order_DoorTop>();
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
        [ForeignKey("MonitorId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
        [ForeignKey("MonitorId")]
        public virtual ICollection<Order_Hall> Order_Hall { get; set; }
        [ForeignKey("MonitorId")]
        public virtual ICollection<Order_DoorTop> Order_DoorTop { get; set; }
    }
}
