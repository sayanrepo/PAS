using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Service_Service
    {
        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public int FactorNumber { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(50)]
        public string ClienteleName { get; set; }
        public byte OrderTypeId { get; set; }
        public int DeliveryCityId { get; set; }

        [MaxLength(255)]
        public string DeliveryAddress { get; set; }

        public double ServiceCost { get; set; }
        public Nullable<double> DeliveryCost { get; set; }
        public double Tax { get; set; }
        public double Discount { get; set; }
        public byte StatusId { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }
        public double Cost { get; set; }
        public int AccepterId { get; set; }


        [ForeignKey("CustomerId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("AccepterId")]
        public virtual Account_Users Account_Users1 { get; set; }
        [ForeignKey("DeliveryCityId")]
        public virtual Location_Cities Location_Cities { get; set; }
        [ForeignKey("StatusId")]
        public virtual Order_Status Order_Status { get; set; }
        [ForeignKey("OrderTypeId")]
        public virtual Tb_OrderTypes Tb_OrderTypes { get; set; }


        //-----------------------------------------------------------------
        private DateTime? dateOrder;
        private DateTime? dateFactor;
        private DateTime? dateDelivery;
        private string shDateOrder;
        private string shDateFactor;
        private string shDateDelivery;

        public Nullable<System.DateTime> DateOrder
        {
            get { return dateOrder; }
            set { dateOrder = value; shDateOrder = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShDateOrder
        {
            get { return shDateOrder; }
            set { shDateOrder = value; if (!string.IsNullOrEmpty(value)) dateOrder = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }
        public Nullable<System.DateTime> DateFactor
        {
            get { return dateFactor; }
            set { dateFactor = value; shDateFactor = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShDateFactor
        {
            get { return shDateFactor; }
            set { shDateFactor = value; if (!string.IsNullOrEmpty(value)) dateFactor = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }
        public Nullable<System.DateTime> DateDelivery
        {
            get { return dateDelivery; }
            set { dateDelivery = value; shDateDelivery = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShDateDelivery
        {
            get { return shDateDelivery; }
            set { shDateDelivery = value; if (!string.IsNullOrEmpty(value)) dateDelivery = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }

        [NotMapped]
        public double SumCostTax { get; set; }

        public override string ToString()
        {
            string s = "";
            s += "شماره سند: " + DocNumber.ToString() + " \tوضعیت: " + Order_Status.Name + "\n\r";
            s += "نوع سفارش: " + Tb_OrderTypes.Name + " \tمشتری: " + Account_Users.FullName + " \tسفارش دهنده: " + ClienteleName + "\n\r";
            s += "تاریخ سفارش: " + ShDateOrder + " \tتاریخ تحویل: " + ShDateFactor + "\n\r";
            s += "جمع کل مبلغ سفارش: " + Cost.ToString() + " \tجمع مبلغ خدمات: " + ServiceCost.ToString() + "\n\r";
            s += "کرایه حمل: " + (DeliveryCost.HasValue ? DeliveryCost.Value.ToString() : "0") + " \tعوارض: " + Tax.ToString() + " \tتخفیف: " + Discount.ToString() + "\n\r";
            s += "محل ارسال: " + DeliveryAddress + "\n\r";
            s += "شرح خدمات: " + Comment + "\n\r";
            return s;
        }
    }
}
