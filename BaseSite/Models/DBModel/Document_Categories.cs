using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Document_Categories
    {
        public Document_Categories()
        {
            //this.Document_Documents = new HashSet<Document_Documents>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }

        //public virtual ICollection<Document_Documents> Document_Documents { get; set; }
    }
}
