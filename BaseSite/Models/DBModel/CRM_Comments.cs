using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class CRM_Comments
    {
        public CRM_Comments()
        {
            this.CRM_Comments1 = new HashSet<CRM_Comments>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Nullable<int> TrunkId { get; set; }
        public Nullable<short> TrunkTableId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> OwnerId { get; set; }

        [MaxLength(50)]
        public string OwnerName { get; set; }

        [MaxLength(255)]
        public string OwnerEmail { get; set; }

        public string Comment { get; set; }


        [ForeignKey("OwnerId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("ParentId")]
        public virtual ICollection<CRM_Comments> CRM_Comments1 { get; set; }
        [ForeignKey("ParentId")]
        public virtual CRM_Comments CRM_Comments2 { get; set; }



        //------------------------------------------------------------------
        private DateTime createDate;
        private string shCreateDate;

        public System.DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; shCreateDate = new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.DateTime); }
        }

        [NotMapped]
        public string ShCreateDate
        {
            get { return shCreateDate; }
            set { shCreateDate = value; if (!string.IsNullOrEmpty(value)) createDate = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }
    }
}
