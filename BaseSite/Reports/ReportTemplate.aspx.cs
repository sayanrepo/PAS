using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaseSite.Reports
{
    public partial class ReportTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request["ShDateFrom"] != null && Request["ShDateTo"] != null && !string.IsNullOrWhiteSpace(Request["ShDateFrom"]) && !string.IsNullOrWhiteSpace(Request["ShDateTo"]))
                    {
                        rvSiteMapping.Reset(); //important
                        rvSiteMapping.Height = Unit.Pixel(Convert.ToInt32(Request["Height"]) - 58);
                        rvSiteMapping.ShowPrintButton = true;
                        rvSiteMapping.AsyncRendering = false;

                        ////for process report from report server
                        //rvSiteMapping.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
                        //rvSiteMapping.ServerReport.ReportServerUrl = new Uri("SSRS URL"); // Add the Reporting Server URL
                        //rvSiteMapping.ServerReport.ReportPath = String.Format("/{0}/{1}", reportFolder, Request["ReportName"].ToString());
                        //rvSiteMapping.ServerReport.Refresh();


                        rvSiteMapping.ProcessingMode = ProcessingMode.Local;
                        LocalReport objReport = rvSiteMapping.LocalReport;
                        objReport.ReportPath = Server.MapPath("~/Reports/" + Request["ReportName"].ToString() + ".rdl");

                        rvSiteMapping.LocalReport.SetParameters(new ReportParameter("customerid", Request["CustomerId"].ToString(), true));
                        rvSiteMapping.LocalReport.SetParameters(new ReportParameter("shdatefrom", Request["ShDateFrom"].ToString(), true));
                        rvSiteMapping.LocalReport.SetParameters(new ReportParameter("shdateto", Request["ShDateTo"].ToString(), true));

                        string CS = System.Configuration.ConfigurationManager.ConnectionStrings["PantaEntities"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(CS))
                        {
                            SqlDataAdapter da = new SqlDataAdapter("execute " + Request["ReportName"].ToString() + " " + Request["CustomerId"].ToString() + ", '" + Request["ShDateFrom"].ToString() + "', '" + Request["ShDateTo"].ToString() + "'", con);
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            rvSiteMapping.LocalReport.DataSources.Clear();
                            ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                            rvSiteMapping.LocalReport.DataSources.Add(datasource);
                            if (ds.Tables.Count > 1)
                            {
                                ReportDataSource datasource2 = new ReportDataSource("DataSet2", ds.Tables[1]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource2);
                            }
                            if (ds.Tables.Count > 2)
                            {
                                ReportDataSource datasource3 = new ReportDataSource("DataSet3", ds.Tables[2]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource3);
                            }
                            if (ds.Tables.Count > 3)
                            {
                                ReportDataSource datasource4 = new ReportDataSource("DataSet4", ds.Tables[3]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource4);
                            }
                            if (ds.Tables.Count > 4)
                            {
                                ReportDataSource datasource5 = new ReportDataSource("DataSet5", ds.Tables[4]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource5);
                            }
                            if (ds.Tables.Count > 5)
                            {
                                ReportDataSource datasource6 = new ReportDataSource("DataSet6", ds.Tables[5]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource6);
                            }
                            if (ds.Tables.Count > 6)
                            {
                                ReportDataSource datasource7 = new ReportDataSource("DataSet7", ds.Tables[6]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource7);
                            }
                            if (ds.Tables.Count > 7)
                            {
                                ReportDataSource datasource8 = new ReportDataSource("DataSet8", ds.Tables[7]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource8);
                            }
                            if (ds.Tables.Count > 8)
                            {
                                ReportDataSource datasource9 = new ReportDataSource("DataSet9", ds.Tables[8]);
                                rvSiteMapping.LocalReport.DataSources.Add(datasource9);
                            }
                            rvSiteMapping.LocalReport.Refresh();
                        }
                    }
                }
                catch { }
            }
        }

        protected void rvSiteMapping_Load(object sender, EventArgs e)
        {
        }
    }
}