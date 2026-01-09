using System.ComponentModel.DataAnnotations;

namespace BaseSite.Models.DBModel
{
    public class Account_FindoutWays
    {
        [Key]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
    }
}