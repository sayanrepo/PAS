using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_ProductFactorCost
    {
        public double Cost { get; set; }

        //---------------------------------------------------

        private DateTime? applyDate;
        private string shApplyDate;

        [Key]
        public Nullable<System.DateTime> ApplyDate
        {
            get { return applyDate; }
            set { applyDate = value; shApplyDate = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShApplyDate
        {
            get { return shApplyDate; }
            set { shApplyDate = value; if (!string.IsNullOrEmpty(value)) applyDate = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }
    }
}
