using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Log_Logs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int EntityTableId { get; set; }
        public int EntityId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public Nullable<double> LogData1 { get; set; }
        [MaxLength(25)]
        public string IPAddress { get; set; }
        public int ActivityId { get; set; }
        public string Description { get; set; }


        [ForeignKey("UserId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("EntityTableId")]
        public virtual BaseSystem_Tables BaseSystem_Tables { get; set; }
        [ForeignKey("ActivityId")]
        public virtual Log_LogActivity Log_LogActivity { get; set; }


        //-------------------------------------------------------------
        private DateTime eventTime;
        private string shEventTime;

        public System.DateTime EventTime
        {
            get { return eventTime; }
            set { eventTime = value; shEventTime = new PersianDateTime((DateTime)value).ToString(PersianDateTimeFormat.DateTime); }
        }

        [NotMapped]
        public string ShEventTime
        {
            get { return shEventTime; }
            set { shEventTime = value; if (!string.IsNullOrEmpty(value)) eventTime = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime(); }
        }
    }
}
