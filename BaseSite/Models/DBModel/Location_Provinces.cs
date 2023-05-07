using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Location_Provinces
    {
        public Location_Provinces()
        {
            this.Location_Cities = new HashSet<Location_Cities>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        public int CountryId { get; set; }
        public bool Deleted { get; set; }


        [ForeignKey("ProvinceId")]
        public virtual ICollection<Location_Cities> Location_Cities { get; set; }
        [ForeignKey("CountryId")]
        public virtual Location_Countries Location_Countries { get; set; }
    }
}
