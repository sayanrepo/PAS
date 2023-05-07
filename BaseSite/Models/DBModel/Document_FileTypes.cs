using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Document_FileTypes
    {
        public Document_FileTypes()
        {
            this.Document_Documents = new HashSet<Document_Documents>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        public string Extensions { get; set; }


        [ForeignKey("FileTypeId")]
        public virtual ICollection<Document_Documents> Document_Documents { get; set; }
    }
}
