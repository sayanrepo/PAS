using System.ComponentModel.DataAnnotations;

namespace BaseSite.Models.DBModel
{

    public class BaseSystem_EntityStatus
    {
        [Key]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
