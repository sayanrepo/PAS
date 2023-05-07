using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BaseSite.Services.SmsService
{
    public abstract class ISmsService
    {
        /// <summary>
        /// شماره تماس های واحد پشتیبانی
        /// </summary>
        public List<string> SupportPhones;

        public ISmsService()
        {
            //SupportPhones = new List<string>
            //{
            //    "09124713367", "09121153473", "09125225949"
            //};

            string s = ConfigurationManager.AppSettings["SupportPhones"];
            if (!String.IsNullOrEmpty(s))
                SupportPhones = s.Split(',').ToList();
            else SupportPhones = new List<string>();
        }
    }
}
