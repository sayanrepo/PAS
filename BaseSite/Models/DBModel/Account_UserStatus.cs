using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_UserStatus
    {
        public Account_UserStatus()
        {
            this.Account_Users = new HashSet<Account_Users>();
        }
    
        [Key]
        public byte Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [ForeignKey("Status")]
        public virtual ICollection<Account_Users> Account_Users { get; set; }
    }
}
