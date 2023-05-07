using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_Operations
    {
        public Account_Operations()
        {
            this.Account_PostOperation = new HashSet<Account_PostOperation>();
        }
    
        [Key, MaxLength(100)]
        public string OperationCode { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
    
        [ForeignKey("OperationCode")]
        public virtual ICollection<Account_PostOperation> Account_PostOperation { get; set; }
    }
}
