using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_PostOperation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PostId { get; set; }
        public string OperationCode { get; set; }


        [ForeignKey("OperationCode")]
        public virtual Account_Operations Account_Operations { get; set; }
        [ForeignKey("PostId")]
        public virtual Account_Posts Account_Posts { get; set; }
    }
}
