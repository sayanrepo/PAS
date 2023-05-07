using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_Posts
    {
        public Account_Posts()
        {
            this.Account_PostOperation = new HashSet<Account_PostOperation>();
            this.Account_UserPost = new HashSet<Account_UserPost>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }


        [ForeignKey("PostId")]
        public virtual ICollection<Account_PostOperation> Account_PostOperation { get; set; }
        [ForeignKey("PostId")]
        public virtual ICollection<Account_UserPost> Account_UserPost { get; set; }
    }
}
