using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class CRM_Priority
    {
        public CRM_Priority()
        {
            this.CRM_Activity = new HashSet<CRM_Activity>();
        }
    
        [Key]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

    
        [ForeignKey("PriorityId")]
        public virtual ICollection<CRM_Activity> CRM_Activity { get; set; }
    }
}
