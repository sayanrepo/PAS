using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BaseSite.Models.DBModel
{
    public class CheckableItem
    {
        public byte Type { get; set; }
        public int Id { get; set; }
        public bool Checked { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string Comment { get; set; }

        public string TypeName()
        {
            if (Type == 1) return "پنل داخل کابین";
            else if (Type == 11) return "ملحقات داخل کابین";
            else if (Type == 2) return "پنل طبقات";
            else if (Type == 12) return "ملحقات طبقات";
            else if (Type == 3) return "پنل سردرب";
            else if (Type == 13) return "ملحقات سردرب";
            else if (Type == 4) return "فروش قطعه";
            else return "";
        }
    }

    public class Delivery_Delivery
    {
        public Delivery_Delivery()
        {
            this.Order_Cabin = new HashSet<Order_Cabin>();
            this.Order_DoorTop = new HashSet<Order_DoorTop>();
            this.Order_Hall = new HashSet<Order_Hall>();
            this.Order_Panel_Attachment = new HashSet<Order_Panel_Attachment>();
            this.Sale_Goods = new HashSet<Sale_Goods>();
        }

        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> SaleId { get; set; }
        public short PackTypeId { get; set; }

        [MaxLength(50)]
        public string SendResponsible { get; set; }
        [MaxLength(50)]
        public string RecieveResponsible { get; set; }
        public byte DeliveryLocationId { get; set; }
        public byte VehicleTypeId { get; set; }

        [MaxLength(100)]
        public string CarierAgencyName { get; set; }
        [MaxLength(100)]
        public string CarierAgencyBill { get; set; }
        [MaxLength(50)]
        public string VehiclePlaque { get; set; }
        [MaxLength(100)]
        public string DriverName { get; set; }
        [MaxLength(50)]
        public string DriverPhone { get; set; }
        [MaxLength(100)]
        public string RecieverName { get; set; }
        [MaxLength(100)]
        public string RecieverPhone { get; set; }
        [MaxLength(100)]
        public string RecieverMobile { get; set; }

        public Nullable<byte> DestinationType { get; set; }
        public string DestinationAddress { get; set; }
        public byte StatusId { get; set; }



        [ForeignKey("DeliveryLocationId")]
        public virtual Delivery_DeliveryLocations Delivery_DeliveryLocations { get; set; }
        [ForeignKey("StatusId")]
        public virtual Delivery_Status Delivery_Status { get; set; }
        [ForeignKey("VehicleTypeId")]
        public virtual Delivery_VehicleTypes Delivery_VehicleTypes { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order_Order Order_Order { get; set; }
        [ForeignKey("SaleId")]
        public virtual Sale_Sale Sale_Sale { get; set; }
        [ForeignKey("PackTypeId")]
        public virtual Tb_PackTypes Tb_PackTypes { get; set; }
        [ForeignKey("DeliveryId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
        [ForeignKey("DeliveryId")]
        public virtual ICollection<Order_DoorTop> Order_DoorTop { get; set; }
        [ForeignKey("DeliveryId")]
        public virtual ICollection<Order_Hall> Order_Hall { get; set; }
        [ForeignKey("DeliveryId")]
        public virtual ICollection<Order_Panel_Attachment> Order_Panel_Attachment { get; set; }
        [ForeignKey("DeliveryId")]
        public virtual ICollection<Sale_Goods> Sale_Goods { get; set; }



        //---------------------------------------------------------------
        private DateTime? date;
        private string shDate;

        public Nullable<System.DateTime> Date
        {
            get { return date; }
            set { date = value; shDate = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShDate
        {
            get { return shDate; }
            set { shDate = value; if (!string.IsNullOrEmpty(value)) date = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }

        [NotMapped]
        public List<CheckableItem> Items { get; set; }

        public override string ToString()
        {
            string s = "";
            s += "شماره حواله خروج: " + DocNumber.ToString() + " \tتاریخ: " + ShDate + " \tوضعیت: " + Delivery_Status.Name + "\n\r";
            s += "شماره سند: " + (Order_Order != null ? Order_Order.DocNumber.ToString() : Sale_Sale.DocNumber.ToString()) + " \tشماره فاکتور: " + (Order_Order != null ? Order_Order.FactorNumber.ToString() : Sale_Sale.FactorNumber.ToString()) + "\n\r";
            s += "سمت فرستنده: " + SendResponsible + " \tسمت گیرنده: " + RecieveResponsible + " \tنوع بسته بندی: " + Tb_PackTypes.Name + "\n\r";
            s += "نام تحویل گیرنده: " + RecieverName + " \tتلفن: " + RecieverPhone + " \tموبایل: " + RecieverMobile + "\n\r";
            s += "ارسال به: " + this.Delivery_DeliveryLocations.Name + " \tحمل توسط: " + Delivery_VehicleTypes.Name + "\n\r";
            s += "آدرس محل تحویل کالا: " + DestinationAddress + "\n\r";
            s += "کالاهای ارسال شده:\n\r";
            if (this.Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    CheckableItem item = this.Items.ElementAt(i);
                    if (item.Checked)
                        s += "شرح کالا: " + item.Name + " \tتعداد: " + item.Count.ToString() + " \tتوضیحات: " + item.Comment + "\n\r";
                }
            }
            return s;
        }
    }
}