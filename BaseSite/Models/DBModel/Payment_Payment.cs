using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Payment_Payment
    {
        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(50)]
        public string ProjectName { get; set; }
        public byte PaymentTypeId { get; set; }
        public byte PaymentBabatId { get; set; }
        public Nullable<short> BankId { get; set; }

        [MaxLength(50)]
        public string BankBranchCode { get; set; }
        [MaxLength(50)]
        public string ShomareSanad { get; set; }
        [MaxLength(50)]
        public string ShomareHesab { get; set; }

        public double Amount { get; set; }
        public bool Bargashti { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }
        public byte StatusId { get; set; }
        public int AccepterId { get; set; }


        [ForeignKey("CustomerId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("PaymentBabatId")]
        public virtual Payment_Babats Payment_Babats { get; set; }
        [ForeignKey("BankId")]
        public virtual Payment_Banks Payment_Banks { get; set; }
        [ForeignKey("PaymentTypeId")]
        public virtual Payment_Types Payment_Types { get; set; }
        [ForeignKey("StatusId")]
        public virtual Payment_Status Payment_Status { get; set; }
        [ForeignKey("AccepterId")]
        public virtual Account_Users Accepter { get; set; }


        //-------------------------------------------------------------
        private DateTime? dateSanad;
        private DateTime? dateSarresid;
        private string shDateSanad;
        private string shDateSarresid;

        public Nullable<System.DateTime> DateSanad
        {
            get { return dateSanad; }
            set { dateSanad = value; shDateSanad = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShDateSanad
        {
            get { return shDateSanad; }
            set { shDateSanad = value; if (!string.IsNullOrEmpty(value)) dateSanad = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }
        public Nullable<System.DateTime> DateSarresid
        {
            get { return dateSarresid; }
            set { dateSarresid = value; shDateSarresid = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShDateSarresid
        {
            get { return shDateSarresid; }
            set { shDateSarresid = value; if (!string.IsNullOrEmpty(value)) dateSarresid = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }

        public override string ToString()
        {
            string s = "";
            s += "شماره سند: " + DocNumber.ToString() + " \tمشتری: " + Account_Users.FullName + " \tوضعیت: " + Payment_Status.Name + "\n\r";
            s += "نحوه وصول: " + this.Payment_Types.Name + " \tبابت: " + Payment_Babats.Name + " \tبرگشتی: " + (Bargashti ? "هست" : "نیست") + "\n\r";
            s += "تاریخ سند: " + ShDateSanad + " \tتاریخ سررسید: " + ShDateSarresid + " \tمبلغ: " + Amount.ToString() + "\n\r";
            s += "بانک: " + Payment_Banks.Name + " \tکدشعبه: " + BankBranchCode + " \tشماره چک/حواله: " + ShomareSanad + " \tشماره حساب: " + ShomareHesab + "\n\r";
            s += "توضیحات: " + Comment + "\n\r";
            return s;
        }
    }
}
