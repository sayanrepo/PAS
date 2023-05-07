using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Additions
    {
        public Tb_Additions()
        {
            this.Order_Panel_Addition = new HashSet<Order_Panel_Addition>();
        }

        public int TableId { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        public bool Deleted { get; set; }


        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("AdditionId")]
        public virtual ICollection<Order_Panel_Addition> Order_Panel_Addition { get; set; }
    }
}
