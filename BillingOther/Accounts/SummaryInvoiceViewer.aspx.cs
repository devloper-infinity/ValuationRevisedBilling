using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BillingOther.Accounts
{
    public partial class SummaryInvoiceViewer : System.Web.UI.Page
    {
        public string ProjectGroup;
        public string BillingPeriod;
        public string InvoiceId;
        public string InvoiceNumber;
        public static string DomainId;
        bllTracking blltracking = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            ProjectGroup = Convert.ToString(Request.QueryString["ProjectGroup"]);
            BillingPeriod = Convert.ToString(Request.QueryString["BillingPeriod"]);
            //InvoiceId = Convert.ToString(Request.QueryString["InvoiceId"]);
            Time_To_Show_The_SummaryReport();

        }


        public void Time_To_Show_The_SummaryReport()
        {
            try
            {
                DataSet dt = new DataSet();

                DataTable dtNew = GetInvoiceNumber(ProjectGroup, BillingPeriod);
                if (dtNew != null)
                {
                    if (dtNew.Rows.Count > 0)
                    {
                        InvoiceNumber = Convert.ToString(dtNew.Rows[0]["InvoiceNumber"]);
                    }
                    else
                    {
                        InvoiceNumber = "";
                    }
                }
                else
                {
                    InvoiceNumber = "";
                }


                ReportDocument rpt = new ReportDocument();
                rpt.Load(Server.MapPath("~/Reports/Valuation/SummaryReport.rpt"));
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = ProjectGroup;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@GroupName"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = BillingPeriod;
                pval2.Add(pdisval2);



                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval2);
                // rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval3);

                // rpt.SetDataSource(dt);
                //rpt.Subreports[0].SetDataSource(dt.Tables[0]);
                CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;
                CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos;
                CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo;
                CrystalDecisions.CrystalReports.Engine.Tables CrTables;
                crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
                crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();

                crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"];
                crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserID"];
                crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];

                CrTables = rpt.Database.Tables;

                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }
                // rpt.DataSourceConnections=

                ILSReport.RefreshReport();
                ILSReport.Visible = true;
                ILSReport.HasExportButton = false;
                ILSReport.HasPrintButton = false;
                ILSReport.HasPageNavigationButtons = true;
                ILSReport.HasCrystalLogo = false;
                ILSReport.HasDrillUpButton = false;
                ILSReport.HasSearchButton = false;

                ILSReport.HasToggleGroupTreeButton = false;
                ILSReport.HasZoomFactorList = false;
                ILSReport.ToolbarStyle.Width = new Unit("750px");
                ILSReport.ReportSource = rpt;
                string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                string strTime = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string filename = "";
                if (InvoiceNumber != "")
                {
                    filename = InvoiceNumber.Replace(",", "_");
                }
                else
                {
                    filename = ProjectGroup + "_" + BillingPeriod + "_" + strDate + strTime;
                }

                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath =
                    Server.MapPath("~/BillingDocuments/") + filename + ".pdf";

                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                //int result = blltracking.InsertGroupAttachmentPath(ProjectGroup, BillingPeriod, Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"));
                int result = blltracking.InsertGroupAttachmentPath_QC(ProjectGroup, BillingPeriod, Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"), InvoiceNumber);

                // string filename = ProjectNo + "_" + OrderNo + "_" + strDate + strTime;
                rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);

                // rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"D:\myReport.pdf");
            }
            catch (Exception ex) { throw ex; }

        }

        public DataTable GetInvoiceNumber(string ProjectNumber, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetInvocieNumber_QC");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectGroup", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, ProjectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
    }
}