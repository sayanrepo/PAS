using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Payment_Banks
    {
        public Payment_Banks()
        {
            this.Payment_Payment = new HashSet<Payment_Payment>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

    
        [ForeignKey("BankId")]
        public virtual ICollection<Payment_Payment> Payment_Payment { get; set; }
    }
}
