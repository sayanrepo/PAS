using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Document_Documents
    {
        public Document_Documents()
        {
            //this.Document_Categories = new HashSet<Document_Categories>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
        public int EntityTableId { get; set; }
        public int EntityId { get; set; }
        public int FileTypeId { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }


        [ForeignKey("FileTypeId")]
        public virtual Document_FileTypes Document_FileTypes { get; set; }
        //public virtual ICollection<Document_Categories> Document_Categories { get; set; }
    }
}
