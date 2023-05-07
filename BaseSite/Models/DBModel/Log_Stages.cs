using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseSite.Models.DBModel
{
    public class Log_Stages
    {
        //[Key]
        public int EntityTableId { get; set; }
        [Key]
        public int StageId { get; set; }
        [MaxLength(255)]
        public string StageName { get; set; }
    }
}
