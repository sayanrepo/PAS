using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BaseSite.Models.DBModel
{
    public class Sale_Sale
    {
        public Sale_Sale()
        {
            this.Delivery_Delivery = new HashSet<Delivery_Delivery>();
            this.Sale_Goods = new HashSet<Sale_Goods>();
        }

        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public int FactorNumber { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(50)]
        public string ClienteleName { get; set; }
        public byte OrderTypeId { get; set; }
        public bool GiveBack { get; set; }
        public int DeliveryCityId { get; set; }

        [MaxLength(255)]
        public string DeliveryAddress { get; set; }

        public Nullable<double> DeliveryCost { get; set; }
        public double Tax { get; set; }
        public double Discount { get; set; }
        public byte StatusId { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }
        public double Cost { get; set; }
        public int AccepterId { get; set; }
        public byte TradeTypeId { get; set; }
        public byte StoreId { get; set; }


        [ForeignKey("CustomerId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("AccepterId")]
        public virtual Account_Users Account_Users1 { get; set; }
        [ForeignKey("SaleId")]
        public virtual ICollection<Delivery_Delivery> Delivery_Delivery { get; set; }
        [ForeignKey("DeliveryCityId")]
        public virtual Location_Cities Location_Cities { get; set; }
        [ForeignKey("StatusId")]
        public virtual Order_Status Order_Status { get; set; }
        [ForeignKey("SaleId")]
        public virtual ICollection<Sale_Goods> Sale_Goods { get; set; }
        [ForeignKey("OrderTypeId")]
        public virtual Tb_OrderTypes Tb_OrderTypes { get; set; }
        [ForeignKey("StoreId")]
        public virtual Tb_Stores Tb_Stores { get; set; }
        [ForeignKey("TradeTypeId")]
        public virtual Tb_TradeTypes Tb_TradeTypes { get; set; }


        //-------------------------------------------------------------------
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
        public double SumCostGoods { get; set; }

        [NotMapped]
        public double SumCostTax { get; set; }

        public override string ToString()
        {
            string s = "";
            s += "شماره سند: " + DocNumber.ToString() + " \tشماره فاکتور: " + FactorNumber.ToString() + " \tوضعیت: " + Order_Status.Name + "\n\r";
            s += "نوع سفارش: " + Tb_OrderTypes.Name + " \tمشتری: " + Account_Users.FullName + " \tسفارش دهنده: " + ClienteleName + "\n\r";
            s += "تاریخ سفارش: " + ShDateOrder + " \tتاریخ تحویل: " + ShDateFactor + " \tمرجوعی: " + (GiveBack ? "هست" : "نیست") + "\n\r";
            s += "جمع کل مبلغ سفارش: " + Cost.ToString() + " \tجمع مبلغ کالاها: " + SumCostGoods.ToString() + "\n\r";
            s += "کرایه حمل: " + (DeliveryCost.HasValue ? DeliveryCost.Value.ToString() : "0") + " \tعوارض: " + Tax.ToString() + " \tتخفیف: " + Discount.ToString() + "\n\r";
            s += "محل ارسال: " + DeliveryAddress + "\n\r";
            if (Sale_Goods.Count > 0)
            {
                s += "کالاها:\n\r";
                for (int i = 0; i < Sale_Goods.Count; i++)
                {
                    Sale_Goods goods = this.Sale_Goods.ElementAt(i);
                    s += "نوع کالا: " + goods.TypeId.ToString() + " \tشرح: " + goods.Name + " \tتعداد: " + goods.Count.ToString() + " \tفی: " + goods.Phi.ToString() + " \tتوضیحات: " + goods.Comment + "\n\r";
                }
            }
            return s;
        }
    }
}
