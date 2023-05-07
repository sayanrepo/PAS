using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_UserPost
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public int CategoryId { get; set; }


        [ForeignKey("CategoryId")]
        public virtual Account_Categories Account_Categories { get; set; }
        [ForeignKey("PostId")]
        public virtual Account_Posts Account_Posts { get; set; }
        [ForeignKey("UserId")]
        public virtual Account_Users Account_Users { get; set; }
    }
}
