using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class CRM_Activity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int AssignedToId { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string Subject { get; set; }
        public string Description { get; set; }

        public bool InOut { get; set; }
        public byte PriorityId { get; set; }
        public byte StateId { get; set; }

        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public Nullable<byte> RepeatDays { get; set; }
        public byte TypeId { get; set; }


        [ForeignKey("OwnerId")]
        public virtual Account_Users Account_Users { get; set; }
        [ForeignKey("AssignedToId")]
        public virtual Account_Users Account_Users1 { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Account_Users Account_Users2 { get; set; }
        [ForeignKey("StateId")]
        public virtual CRM_ActivityState CRM_ActivityState { get; set; }
        [ForeignKey("PriorityId")]
        public virtual CRM_Priority CRM_Priority { get; set; }
        [ForeignKey("TypeId")]
        public virtual CRM_ActivityType CRM_ActivityType { get; set; }


        //-----------------------------------------------------------------
        [NotMapped]
        public string ShStartDate
        {
            get { return StartTime == default(DateTime) ? "" : new PersianDateTime((DateTime)StartTime).ToString(PersianDateTimeFormat.Date); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {

                    DateTime temp = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime();
                    StartTime = new DateTime(temp.Year, temp.Month, temp.Day, StartTime.Hour, StartTime.Minute, StartTime.Second);
                }
            }
        }

        [NotMapped]
        public string ShStartTime
        {
            get { return StartTime == default(DateTime) ? "" : StartTime.ToString("HH:mm:ss"); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime temp = DateTime.Parse(value);
                    StartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, temp.Hour, temp.Minute, temp.Second);
                }
            }
        }

        [NotMapped]
        public string ShEndDate
        {
            get { return EndTime == default(DateTime) ? "" : new PersianDateTime((DateTime)EndTime).ToString(PersianDateTimeFormat.Date); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {

                    DateTime temp = PersianDateTime.Parse(value.Replace('-', '/')).ToDateTime();
                    EndTime = new DateTime(temp.Year, temp.Month, temp.Day, EndTime.Hour, EndTime.Minute, EndTime.Second);
                }
            }
        }

        [NotMapped]
        public string ShEndTime
        {
            get { return EndTime == default(DateTime) ? "" : EndTime.ToString("HH:mm:ss"); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime temp = DateTime.Parse(value);
                    EndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, temp.Hour, temp.Minute, temp.Second);
                }
            }
        }
    }
}
