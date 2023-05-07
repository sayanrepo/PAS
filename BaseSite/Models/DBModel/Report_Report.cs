using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseSite.Models.DBModel
{
    public enum ReportTypes
    {
        Sale = 1,
        Financial = 2,
        KPI = 3,
        Production = 4,
        General = 5
    }

    public class Report_Report
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportDescription { get; set; }
        public ReportTypes ReportType { get; set; }
        public string ReportURL { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string ReportSummary { get; set; }

        public string ReportShDateFrom { get; set; }
        public string ReportShDateTo { get; set; }
        public int ReportArg1 { get; set; }

        public string ReportTypeName
        {
            get
            {
                if (ReportType == ReportTypes.Sale)
                    return "واحد فروش";
                else if (ReportType == ReportTypes.Financial)
                    return "واحد مالی";
                else if (ReportType == ReportTypes.KPI)
                    return "KPI";
                else if (ReportType == ReportTypes.Production)
                    return "تولید";
                else if (ReportType == ReportTypes.General)
                    return "عمومی";
                else
                    return "سایر";
            }
        }
    }
}