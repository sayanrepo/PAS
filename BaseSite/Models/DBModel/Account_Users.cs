using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Account_Users
    {
        public Account_Users()
        {
            this.Account_UserPost = new HashSet<Account_UserPost>();
            this.Order_Order = new HashSet<Order_Order>();
            this.Order_Order1 = new HashSet<Order_Order>();
            this.Order_Process = new HashSet<Order_Process>();
            this.Payment_Payment = new HashSet<Payment_Payment>();
            this.Sale_Sale = new HashSet<Sale_Sale>();
            this.Sale_Sale1 = new HashSet<Sale_Sale>();
            this.Service_Service = new HashSet<Service_Service>();
            this.Service_Service1 = new HashSet<Service_Service>();
            this.Log_Logs = new HashSet<Log_Logs>();
            this.CRM_Comments = new HashSet<CRM_Comments>();
            this.CRM_Activity = new HashSet<CRM_Activity>();
            this.CRM_Activity1 = new HashSet<CRM_Activity>();
            this.CRM_Activity2 = new HashSet<CRM_Activity>();
            this.Account_Users1 = new HashSet<Account_Users>();
        }

        public int TableId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public byte PersonTypeId { get; set; }
        public byte PartnerTypeId { get; set; }
        public string FindoutWay { get; set; }
        public Nullable<byte> DepartmentId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string LastName { get; set; }

        [MaxLength(255)]
        public string FatherName { get; set; }

        [MaxLength(15)]
        public string NationalNumber { get; set; }

        [MaxLength(255)]
        public string Website { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Fax { get; set; }

        [MaxLength(50)]
        public string Phone1 { get; set; }

        [MaxLength(50)]
        public string Phone2 { get; set; }

        [MaxLength(50)]
        public string Mobile1 { get; set; }

        [MaxLength(50)]
        public string Mobile2 { get; set; }
        public Nullable<int> CityId1 { get; set; }
        public string Address1 { get; set; }
        public Nullable<int> CityId2 { get; set; }
        public string Address2 { get; set; }

        [MaxLength(255)]
        public string UserName { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        public string ImagePath { get; set; }

        public string Comment { get; set; }
        public byte Status { get; set; }

        [MaxLength(50)]
        public string Responsible1 { get; set; }

        [MaxLength(50)]
        public string ResponsiblePhone1 { get; set; }

        [MaxLength(50)]
        public string Responsible2 { get; set; }

        [MaxLength(50)]
        public string ResponsiblePhone2 { get; set; }

        [MaxLength(50)]
        public string Responsible3 { get; set; }

        [MaxLength(50)]
        public string ResponsiblePhone3 { get; set; }

        [MaxLength(15)]
        public string EconomicalNumber { get; set; }

        [MaxLength(15)]
        public string PostalCode1 { get; set; }

        [MaxLength(15)]
        public string PostalCode2 { get; set; }

        public Nullable<int> RegistrarId { get; set; }



        [ForeignKey("PartnerTypeId")]
        public virtual Account_PartnerTypes Account_PartnerTypes { get; set; }
        [ForeignKey("PersonTypeId")]
        public virtual Account_PersonTypes Account_PersonTypes { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<Account_UserPost> Account_UserPost { get; set; }
        [ForeignKey("Status")]
        public virtual Account_UserStatus Account_UserStatus { get; set; }
        [ForeignKey("CityId1")]
        public virtual Location_Cities Location_Cities { get; set; }
        [ForeignKey("CityId2")]
        public virtual Location_Cities Location_Cities1 { get; set; }
        [ForeignKey("CustomerId")]
        public virtual ICollection<Order_Order> Order_Order { get; set; }
        [ForeignKey("AccepterId")]
        public virtual ICollection<Order_Order> Order_Order1 { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<Order_Process> Order_Process { get; set; }
        [ForeignKey("CustomerId")]
        public virtual ICollection<Payment_Payment> Payment_Payment { get; set; }
        [ForeignKey("CustomerId")]
        public virtual ICollection<Sale_Sale> Sale_Sale { get; set; }
        [ForeignKey("AccepterId")]
        public virtual ICollection<Sale_Sale> Sale_Sale1 { get; set; }
        [ForeignKey("CustomerId")]
        public virtual ICollection<Service_Service> Service_Service { get; set; }
        [ForeignKey("AccepterId")]
        public virtual ICollection<Service_Service> Service_Service1 { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<Log_Logs> Log_Logs { get; set; }
        [ForeignKey("OwnerId")]
        public virtual ICollection<CRM_Comments> CRM_Comments { get; set; }
        [ForeignKey("OwnerId")]
        public virtual ICollection<CRM_Activity> CRM_Activity { get; set; }
        [ForeignKey("AssignedToId")]
        public virtual ICollection<CRM_Activity> CRM_Activity1 { get; set; }
        [ForeignKey("CustomerId")]
        public virtual ICollection<CRM_Activity> CRM_Activity2 { get; set; }
        [ForeignKey("RegistrarId")]
        public virtual ICollection<Account_Users> Account_Users1 { get; set; }
        [ForeignKey("RegistrarId")]
        public virtual Account_Users Account_Users2 { get; set; }


        //---------------------------------------------------------------------------
        private DateTime? dateRegistration;
        private string shDateRegistration;

        public Nullable<System.DateTime> RegistrationDate
        {
            get { return dateRegistration; }
            set { dateRegistration = value; shDateRegistration = value.HasValue ? new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.Date) : ""; }
        }
        [NotMapped]
        public string ShRegistrationDate
        {
            get { return shDateRegistration; }
            set { shDateRegistration = value; if (!string.IsNullOrEmpty(value)) dateRegistration = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }

        [NotMapped]
        public string FullName
        {
            get { return ((Name ?? "") + " " + (LastName ?? "")).Trim(); }
        }

        public override string ToString()
        {
            string s = "";
            s += "نوع شخص: " + Cache.Account_PersonTypes[this.PersonTypeId] + " نام: " + FullName + " شناسه ملی/شماره ثبت: " + NationalNumber + "\n\r";
            s += "نوع همکاری: " + Cache.Account_PartnerTypes[this.PartnerTypeId] + " بخش: " + (DepartmentId == 1 ? "فروش" : DepartmentId == 2 ? "تولید" : "نامعلوم") + " تلفن: " + this.Phone1 + "\n\r";
            s += " فکس: " + this.Fax + " نام مدیرعامل: " + Responsible1 + " شماره مدیرعامل: " + ResponsiblePhone1 + "\n\r";
            s += " آدرس منزل/دفتر: " + Address1 + "\n\r";
            s += " آدرس محل کار/کارخانه: " + Address2 + "\n\r";
            return s;
        }

        public string GetMobile()
        {
            string res = Mobile1;
            if (string.IsNullOrWhiteSpace(res) == false)
            {
                res = res.Replace(" ", "");
                if (res.StartsWith("09"))
                {
                    res = res.Split(',')[0];
                    return res;
                }
            }

            res = Mobile2;
            if (string.IsNullOrWhiteSpace(res) == false)
            {
                res = res.Replace(" ", "");
                if (res.StartsWith("09"))
                {
                    res = res.Split(',')[0];
                    return res;
                }
            }

            res = ResponsiblePhone1;
            if (string.IsNullOrWhiteSpace(res) == false)
            {
                res = res.Replace(" ", "");
                if (res.StartsWith("09"))
                {
                    res = res.Split(',')[0];
                    return res;
                }
            }

            return string.Empty;
        }
    }
}
