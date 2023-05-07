using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class BaseSystem_Tables
    {
        public BaseSystem_Tables()
        {
            this.Order_Process = new HashSet<Order_Process>();
            this.Tb_Additions = new HashSet<Tb_Additions>();
            this.Tb_Attachments = new HashSet<Tb_Attachments>();
            this.Tb_CabinPanels = new HashSet<Tb_CabinPanels>();
            this.Tb_CabinSurfaceMetals = new HashSet<Tb_CabinSurfaceMetals>();
            this.Tb_Costs = new HashSet<Tb_Costs>();
            this.Tb_Deductions = new HashSet<Tb_Deductions>();
            this.Tb_DoorTopPanels = new HashSet<Tb_DoorTopPanels>();
            this.Tb_DoorTopSurfaceMetals = new HashSet<Tb_DoorTopSurfaceMetals>();
            this.Tb_HallPanels = new HashSet<Tb_HallPanels>();
            this.Tb_HallSurfaceMetals = new HashSet<Tb_HallSurfaceMetals>();
            this.Tb_Monitors = new HashSet<Tb_Monitors>();
            this.Tb_PushButtons = new HashSet<Tb_PushButtons>();
            this.Tb_SurfaceMetals = new HashSet<Tb_SurfaceMetals>();
            this.Tb_Truth = new HashSet<Tb_Truth>();
            this.Tb_Truth1 = new HashSet<Tb_Truth>();
            this.Log_Logs = new HashSet<Log_Logs>();
            this.Tb_Products = new HashSet<Tb_Products>();
        }

        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Label { get; set; }


        [ForeignKey("ProductTableId")]
        public virtual ICollection<Order_Process> Order_Process { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_Additions> Tb_Additions { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_Attachments> Tb_Attachments { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_CabinPanels> Tb_CabinPanels { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_CabinSurfaceMetals> Tb_CabinSurfaceMetals { get; set; }
        [ForeignKey("EntityTableId")]
        public virtual ICollection<Tb_Costs> Tb_Costs { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_Deductions> Tb_Deductions { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_DoorTopPanels> Tb_DoorTopPanels { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_DoorTopSurfaceMetals> Tb_DoorTopSurfaceMetals { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_HallPanels> Tb_HallPanels { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_HallSurfaceMetals> Tb_HallSurfaceMetals { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_Monitors> Tb_Monitors { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_PushButtons> Tb_PushButtons { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_SurfaceMetals> Tb_SurfaceMetals { get; set; }
        [ForeignKey("PrimaryTableId")]
        public virtual ICollection<Tb_Truth> Tb_Truth { get; set; }
        [ForeignKey("SecondaryTableId")]
        public virtual ICollection<Tb_Truth> Tb_Truth1 { get; set; }
        [ForeignKey("EntityTableId")]
        public virtual ICollection<Log_Logs> Log_Logs { get; set; }
        [ForeignKey("TableId")]
        public virtual ICollection<Tb_Products> Tb_Products { get; set; }
    }
}
