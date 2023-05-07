using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseSite.Models.Account
{
    public enum UserStatus : byte
    {
        Active = 1,
        DeActive = 2,
        Deleted = 3
    }

    public class DetailAccount
    {
        public string PersonType { get; set; }
        public string RelationType { get; set; }
        public string NationalNumber { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string FatherName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Site { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Responsible1 { get; set; }
        public string Responsible1Mobile { get; set; }
        public string Responsible2 { get; set; }
        public string Responsible2Mobile { get; set; }
        public string Responsible3 { get; set; }
        public string Responsible3Mobile { get; set; }
        public string Comment { get; set; }
    }
}