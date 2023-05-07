using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Order_Process
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductTableId { get; set; }
        public int ProductDocNumber { get; set; }
        public int UserId { get; set; }
        public byte ProductStatusId { get; set; }

        public double Percent { get; set; }
        public int Count { get; set; }
        public double ProductFactor { get; set; }
        public double ProductFactorCost { get; set; }
        public double CollectiveProducePercent { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
        public double CalculatedFactor { get; set; }


        [ForeignKey("UserId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("ProductTableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order_Order Order_Order { get; set; }
        [ForeignKey("ProductStatusId")]
        public virtual Order_ProductStatus Order_ProductStatus { get; set; }


        //--------------------------------------------------------------------------
        private DateTime? ctime;
        private string shTime;

        public Nullable<System.DateTime> PTime
        {
            get { return ctime; }
            set { ctime = value; shTime = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }

        [NotMapped]
        public string ShTime
        {
            get { return shTime; }
            set { shTime = value; if (!string.IsNullOrEmpty(value)) ctime = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }
    }
}
