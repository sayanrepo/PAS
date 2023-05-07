using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Log_LogActivity
    {
        public Log_LogActivity()
        {
            this.Log_Logs = new HashSet<Log_Logs>();
        }
    
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

    
        [ForeignKey("ActivityId")]
        public virtual ICollection<Log_Logs> Log_Logs { get; set; }
    }
}
