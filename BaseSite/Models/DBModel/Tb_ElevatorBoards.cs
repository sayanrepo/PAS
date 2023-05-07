using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_ElevatorBoards
    {
        public Tb_ElevatorBoards()
        {
            this.Order_Order = new HashSet<Order_Order>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("ElevatorBoardId")]
        public virtual ICollection<Order_Order> Order_Order { get; set; }
    }
}
