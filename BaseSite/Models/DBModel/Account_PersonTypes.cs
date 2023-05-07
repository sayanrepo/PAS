using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_PersonTypes
    {
        public Account_PersonTypes()
        {
            this.Account_Users = new HashSet<Account_Users>();
        }
    
        [Key]
        public byte Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }
    
        [ForeignKey("PersonTypeId")]
        public virtual ICollection<Account_Users> Account_Users { get; set; }
    }
}
