using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_Categories
    {
        public Account_Categories()
        {
            this.Account_UserPost = new HashSet<Account_UserPost>();
        }
    
        public int TableId { get; set; }

        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public byte Status { get; set; }
    
        [ForeignKey("CategoryId")]
        public virtual ICollection<Account_UserPost> Account_UserPost { get; set; }
    }
}
