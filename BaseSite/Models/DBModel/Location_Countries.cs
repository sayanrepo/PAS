using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Location_Countries
    {
        public Location_Countries()
        {
            this.Location_Provinces = new HashSet<Location_Provinces>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("CountryId")]
        public virtual ICollection<Location_Provinces> Location_Provinces { get; set; }
    }
}
