using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BaseSite.Models.DBModel
{
    public class Order_Order
    {
        public Order_Order()
        {
            this.Delivery_Delivery = new HashSet<Delivery_Delivery>();
            this.Order_Cabin = new HashSet<Order_Cabin>();
            this.Order_Deduction = new HashSet<Order_Deduction>();
            this.Order_DoorTop = new HashSet<Order_DoorTop>();
            this.Order_Hall = new HashSet<Order_Hall>();
            this.Order_Process = new HashSet<Order_Process>();
        }

        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public int FactorNumber { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(50)]
        public string ClienteleName { get; set; }

        [MaxLength(50)]
        public string ProjectName { get; set; }

        public byte OrderTypeId { get; set; }
        public int ElevatorBoardId { get; set; }
        public short PackTypeId { get; set; }
        public int DeliveryCityId { get; set; }

        [MaxLength(255)]
        public string DeliveryAddress { get; set; }

        public Nullable<double> DeliveryCost { get; set; }
        public double Tax { get; set; }

        public byte StatusId { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }

        public double Cost { get; set; }
        public double DiscountRate { get; set; }
        public int AccepterId { get; set; }
        public byte TradeTypeId { get; set; }
        public byte StoreId { get; set; }


        [ForeignKey("CustomerId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("AccepterId")]
        public virtual Account_Users Account_Users1 { get; set; }
        [ForeignKey("OrderId")]
        public virtual ICollection<Delivery_Delivery> Delivery_Delivery { get; set; }
        [ForeignKey("DeliveryCityId")]
        public virtual Location_Cities Location_Cities { get; set; }
        [ForeignKey("OrderId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
        [ForeignKey("OrderId")]
        public virtual ICollection<Order_Deduction> Order_Deduction { get; set; }
        [ForeignKey("OrderId")]
        public virtual ICollection<Order_DoorTop> Order_DoorTop { get; set; }
        [ForeignKey("OrderId")]
        public virtual ICollection<Order_Hall> Order_Hall { get; set; }
        [ForeignKey("StatusId")]
        public virtual Order_Status Order_Status { get; set; }
        [ForeignKey("ElevatorBoardId")]
        public virtual Tb_ElevatorBoards Tb_ElevatorBoards { get; set; }
        [ForeignKey("OrderTypeId")]
        public virtual Tb_OrderTypes Tb_OrderTypes { get; set; }
        [ForeignKey("PackTypeId")]
        public virtual Tb_PackTypes Tb_PackTypes { get; set; }
        [ForeignKey("OrderId")]
        public virtual ICollection<Order_Process> Order_Process { get; set; }
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
        public double SumCostPanel { get; set; }
        [NotMapped]
        public double SumCostHall { get; set; }
        [NotMapped]
        public double SumCostDoorTop { get; set; }
        [NotMapped]
        public double SumCostAttachment { get; set; }
        [NotMapped]
        public double SumCostAddition { get; set; }
        [NotMapped]
        public double SumCostDeduction { get; set; }
        [NotMapped]
        public double SumCostTax { get; set; }
        [NotMapped]
        public double SumCostDiscountRate { get; set; }

        public override string ToString()
        {
            string s = "";
            s += "شماره سند: " + DocNumber.ToString() + " \tشماره فاکتور: " + FactorNumber.ToString() + " \tوضعیت: " + Order_Status.Name + "\n\r";
            s += "نوع سفارش: " + Tb_OrderTypes.Name + " \tمشتری: " + Account_Users.FullName + " \tسفارش دهنده: " + ClienteleName + "\n\r";
            s += "تاریخ سفارش: " + ShDateOrder + " \tتاریخ درخواست تولید: " + ShDateDelivery + " \tتاریخ تحویل: " + ShDateFactor + "\n\r";
            s += "جمع کل مبلغ سفارش: " + Cost.ToString() + " \tکرایه حمل: " + (DeliveryCost.HasValue ? DeliveryCost.Value.ToString() : "0") + "\n\r";
            s += "عوارض: " + Tax.ToString() + " \tتخفیف درصدی: " + DiscountRate.ToString() + "\n\r";
            s += "محل ارسال: " + DeliveryAddress + "\n\r";
            if (!string.IsNullOrWhiteSpace(Comment)) s += "توضیحات: " + Comment + "\n\r";
            if (Order_Cabin.Count > 0)
            {
                s += "پنل داخل کابین: ( جمع مبلغ: " + SumCostPanel.ToString() + ")\n\r";
                for (int i = 0; i < Order_Cabin.Count; i++)
                {
                    Order_Cabin cabin = this.Order_Cabin.ElementAt(i);
                    s += "پنل داخل کابین: " + cabin.Tb_CabinPanels.Name + " \tپوش باتون: " + cabin.Tb_PushButtons.Name + " \tنمایشگر: " + cabin.Tb_Monitors.Name + "\n\r";
                    s += "بارکد: " + cabin.DocNumber.ToString() + " \tتعداد: " + cabin.Count.ToString() + " \tتعداد طبقات: " + cabin.FloorCount.ToString() + " \tطبقات: " + cabin.FloorNames + "\n\r";
                    s += "طبقات زیرین: " + cabin.UGFloorNames + " \tکلید آیفون:" + (cabin.PhoneCallButton ? "دارد" : "ندارد") + " \tDO:" + (cabin.DO ? "دارد" : "ندارد") + " \tDC:" + (cabin.DC ? "دارد" : "ندارد") + "\n\r";
                    s += "نحوه نصب: " + cabin.Tb_InstallationTypes.Name + " \tکد بلندگو: " + cabin.Tb_Speakers.Name + " \tفلز رویه: " + cabin.Tb_CabinSurfaceMetals.Name + "\n\r";
                    s += "فلز رویه2: " + cabin.Tb_CabinSurfaceMetals1.Name + "\n\r";
                    if (!string.IsNullOrWhiteSpace(cabin.LaserCuttingText)) s += "متن برش لیزری: " + cabin.LaserCuttingText + "\n\r";
                    if (!string.IsNullOrWhiteSpace(cabin.LaserEngravingText)) s += "متن حک لیزری: " + cabin.LaserEngravingText + "\n\r";
                    if (!string.IsNullOrWhiteSpace(cabin.Comment)) s += "توضیحات: " + cabin.Comment + "\n\r";
                    if (cabin.Order_Panel_Attachment.Count > 0 || cabin.Order_Panel_Addition.Count > 0)
                    {
                        s += "ملحقات و اضافات:" + "\n\r";
                        for (int j = 0; j < cabin.Order_Panel_Attachment.Count; j++)
                            s += "نام:" + cabin.Order_Panel_Attachment.ElementAt(j).Tb_Attachments.Name + " \tتعداد:" + cabin.Order_Panel_Attachment.ElementAt(j).Count.ToString() + " \tمبلغ:" + cabin.Order_Panel_Attachment.ElementAt(j).Cost.ToString() + "\n\r";
                        for (int j = 0; j < cabin.Order_Panel_Addition.Count; j++)
                            s += "نام:" + cabin.Order_Panel_Addition.ElementAt(j).Tb_Additions.Name + " \tمبلغ:" + cabin.Order_Panel_Addition.ElementAt(j).Cost.ToString() + "\n\r";
                    }
                }
            }
            if (Order_Hall.Count > 0)
            {
                s += "پنل طبقات: ( جمع مبلغ: " + SumCostHall.ToString() + ")\n\r";
                for (int i = 0; i < Order_Hall.Count; i++)
                {
                    Order_Hall hall = this.Order_Hall.ElementAt(i);
                    s += "پنل طبقات: " + hall.Tb_HallPanels.Name + " \tپوش باتون: " + hall.Tb_PushButtons.Name + " \tنمایشگر: " + hall.Tb_Monitors.Name + "\n\r";
                    s += "بارکد: " + hall.DocNumber.ToString() + " \tتعداد: " + hall.Count.ToString() + " \tآسانسور: " + hall.Tb_ElevatorCounts.Name + " \tتعداد شاسی: " + hall.Tb_HallPushButtonCounts.Name + " \tفلز رویه: " + hall.Tb_HallSurfaceMetals.Name + "\n\r";
                    if (!string.IsNullOrWhiteSpace(hall.Comment)) s += "توضیحات: " + hall.Comment + "\n\r";
                    if (hall.Order_Panel_Attachment.Count > 0 || hall.Order_Panel_Addition.Count > 0)
                    {
                        s += "ملحقات و اضافات:" + "\n\r";
                        for (int j = 0; j < hall.Order_Panel_Attachment.Count; j++)
                            s += "نام:" + hall.Order_Panel_Attachment.ElementAt(j).Tb_Attachments.Name + " \tتعداد:" + hall.Order_Panel_Attachment.ElementAt(j).Count.ToString() + " \tمبلغ:" + hall.Order_Panel_Attachment.ElementAt(j).Cost.ToString() + "\n\r";
                        for (int j = 0; j < hall.Order_Panel_Addition.Count; j++)
                            s += "نام:" + hall.Order_Panel_Addition.ElementAt(j).Tb_Additions.Name + " \tمبلغ:" + hall.Order_Panel_Addition.ElementAt(j).Cost.ToString() + "\n\r";
                    }
                }
            }
            if (Order_DoorTop.Count > 0)
            {
                s += "پنل سردرب: ( جمع مبلغ: " + SumCostDoorTop.ToString() + ")\n\r";
                for (int i = 0; i < Order_DoorTop.Count; i++)
                {
                    Order_DoorTop doortop = this.Order_DoorTop.ElementAt(i);
                    s += "پنل سردرب: " + doortop.Tb_DoorTopPanels.Name + " \tنمایشگر: " + doortop.Tb_Monitors.Name + " \tتعداد: " + doortop.Count.ToString() + "\n\r";
                    s += "بارکد: " + doortop.DocNumber.ToString() + " \tفلز رویه: " + doortop.Tb_SurfaceMetals.Name + "\n\r";
                    if (!string.IsNullOrWhiteSpace(doortop.Comment)) s += "توضیحات: " + doortop.Comment + "\n\r";
                    if (doortop.Order_Panel_Attachment.Count > 0 || doortop.Order_Panel_Addition.Count > 0)
                    {
                        s += "ملحقات و اضافات:" + "\n\r";
                        for (int j = 0; j < doortop.Order_Panel_Attachment.Count; j++)
                            s += "نام:" + doortop.Order_Panel_Attachment.ElementAt(j).Tb_Attachments.Name + " \tتعداد:" + doortop.Order_Panel_Attachment.ElementAt(j).Count.ToString() + " \tمبلغ:" + doortop.Order_Panel_Attachment.ElementAt(j).Cost.ToString() + "\n\r";
                        for (int j = 0; j < doortop.Order_Panel_Addition.Count; j++)
                            s += "نام:" + doortop.Order_Panel_Addition.ElementAt(j).Tb_Additions.Name + " \tمبلغ:" + doortop.Order_Panel_Addition.ElementAt(j).Cost.ToString() + "\n\r";
                    }
                }
            }
            return s;
        }
    }
}
