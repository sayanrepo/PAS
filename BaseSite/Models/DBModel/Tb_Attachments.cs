using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Attachments
    {
        public Tb_Attachments()
        {
            this.Order_Panel_Attachment = new HashSet<Order_Panel_Attachment>();
        }
    
        public int TableId { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        public bool Deleted { get; set; }
        public double Cost { get; set; }
        public double ProductFactor { get; set; }
        public bool Available { get; set; }
        public bool IsDeliveryItem { get; set; }

    
        [ForeignKey("TableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("AttachmentId")]
        public virtual ICollection<Order_Panel_Attachment> Order_Panel_Attachment { get; set; }
    }
}
