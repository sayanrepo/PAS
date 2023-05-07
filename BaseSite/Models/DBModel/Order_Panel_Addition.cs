using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_Panel_Addition
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Nullable<int> CabinPanelId { get; set; }
        public Nullable<int> HallPanelId { get; set; }
        public Nullable<int> DoorTopPanelId { get; set; }
        public int AdditionId { get; set; }
        public double Cost { get; set; }


        [ForeignKey("CabinPanelId")]
        public virtual Order_Cabin Order_Cabin { get; set; }
        [ForeignKey("DoorTopPanelId")]
        public virtual Order_DoorTop Order_DoorTop { get; set; }
        [ForeignKey("HallPanelId")]
        public virtual Order_Hall Order_Hall { get; set; }
        [ForeignKey("AdditionId")]
        public virtual Tb_Additions Tb_Additions { get; set; }
    }
}
