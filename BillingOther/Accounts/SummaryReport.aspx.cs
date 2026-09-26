using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace BillingOther.Accounts
{
    public partial class SummaryReport : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = Master.FindControl("lblHeader") as HtmlGenericControl;
            if (h4 != null) h4.InnerText = "Summary Report";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetSummaryRows(string month, string year)
        {
            try
            {
                DataTable dt;
                bllTracking bll = new bllTracking();
                if (string.IsNullOrWhiteSpace(month) || month == "Select")
                {
                    dt = bll.BindSummaryReport("0", "Select");
                }
                else
                {
                    dt = BindSummaryReportMonthwiseStatic(month, year);
                }
                return new { Success = true, Rows = ToSummaryRows(dt) };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = ex.Message, Rows = new List<object>() };
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetInvoiceForUpdate(string projectNumber, string billingPeriod)
        {
            try
            {
                DataTable dt = GetInvoiceDetailsStatic(projectNumber, billingPeriod);
                if (dt == null || dt.Rows.Count == 0) return new { Success = false, Message = "Invoice details not found." };
                DataRow r = dt.Rows[0];
                return new
                {
                    Success = true,
                    InvoiceReceivedDate = ToHtmlDate(r["InvoiceRecievedByClientDate"]),
                    NoDisputeDate = ToHtmlDate(r["NoDisputeConfirmByClient"]),
                    CommunicationReceipt = Convert.ToString(r["CommunicationReciept"]),
                    Remark = Convert.ToString(r["Remark"]),
                    CompletedDate = ToHtmlDate(r["InvCompleteDate"])
                };
            }
            catch (Exception ex) { return new { Success = false, Message = ex.Message }; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object UpdateInvoiceAjax(string projectNumber, string billingPeriod, string invoiceReceivedDate, string noDisputeDate, string communicationReceipt, string remark, string completedDate)
        {
            try
            {
                DataTable dt = GetInvoiceDetailsStatic(projectNumber, billingPeriod);
                if (dt == null || dt.Rows.Count == 0) return new { Success = false, Message = "Invoice details not found." };
                int invoiceId = Convert.ToInt32(dt.Rows[0]["InvoiceID"]);
                Hashtable p = new Hashtable();
                p.Add("InvoiceID", invoiceId);
                DateTime d;
                if (DateTime.TryParse(invoiceReceivedDate, out d)) p.Add("InvoiceRecievedByClientDate", d);
                p.Add("CommunicationReciept", communicationReceipt ?? "");
                p.Add("Remark", (remark ?? "").Trim());
                if (DateTime.TryParse(noDisputeDate, out d)) p.Add("NoDisputeConfirmByClient", d.ToString("yyyy-MM-dd"));
                if (DateTime.TryParse(completedDate, out d)) p.Add("InvCompleteDate", d.ToString("yyyy-MM-dd"));
                new bllTracking().UpdateInvoiceDetailsGroup(p);
                return new { Success = true };
            }
            catch (Exception ex) { return new { Success = false, Message = ex.Message }; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object PrepareReportPreview(string groupName, string billingPeriod)
        {
            try
            {
                string virtualPath = GenerateSummaryReport(groupName, billingPeriod);
                string physicalPath = HttpContext.Current.Server.MapPath(virtualPath);
                if (!File.Exists(physicalPath))
                    return new { Success = false, Message = "Generated PDF file was not found." };

                string url = VirtualPathUtility.ToAbsolute(virtualPath);
                return new
                {
                    Success = true,
                    Url = url,
                    FileName = Path.GetFileName(physicalPath)
                };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetSendTemplate(string groupName, string billingPeriod)
        {
            try { return new { Success = true, Html = BuildEmailTemplate(groupName, billingPeriod) }; }
            catch (Exception ex) { return new { Success = false, Message = ex.Message }; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object SendToClientAjax(string groupName, string billingPeriod)
        {
            try
            {
                GenerateSummaryReport(groupName, billingPeriod);
                UpdateInvoiceClientQc(groupName, billingPeriod);
                new SendMail().SendInvoiceCreationEmailForSummaryReport(groupName, billingPeriod, 0);
                return new { Success = true };
            }
            catch (Exception ex) { return new { Success = false, Message = ex.Message }; }
        }

        private static DataTable BindSummaryReportMonthwiseStatic(string month, string year)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "WBT_usp_GetAllGroupSummaryReportForBinding_AddedDate_ForExport");
            SQLHelper.AddParamToSQLCmd(cmd, "@Month", SqlDbType.NVarChar, 1000, ParameterDirection.Input, month);
            SQLHelper.AddParamToSQLCmd(cmd, "@Year", SqlDbType.NVarChar, 1000, ParameterDirection.Input, year);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        private static DataTable GetInvoiceDetailsStatic(string projectNumber, string billingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "[IB_usp_GetInvoiceDetails_ForRead_QC]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectNumber", SqlDbType.NVarChar, 100, ParameterDirection.Input, projectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 200, ParameterDirection.Input, billingPeriod);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        private static int UpdateInvoiceClientQc(string groupName, string billingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "IB_usp_UpdateInvoiceClientId_UW");
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupName", SqlDbType.NVarChar, 200, ParameterDirection.Input, groupName);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 200, ParameterDirection.Input, billingPeriod);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", SqlDbType.BigInt, 0, ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int value = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            cmd.Dispose();
            return value;
        }

        private static DataTable GetInvoiceNumberStatic(string projectGroup, string billingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetInvocieNumber_QC");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectGroup", SqlDbType.NVarChar, 1000, ParameterDirection.Input, projectGroup);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 1000, ParameterDirection.Input, billingPeriod);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        private static string GenerateSummaryReport(string projectGroup, string billingPeriod)
        {
            DataTable inv = GetInvoiceNumberStatic(projectGroup, billingPeriod);
            string invoiceNumber = inv != null && inv.Rows.Count > 0 ? Convert.ToString(inv.Rows[0]["InvoiceNumber"]) : "";
            ReportDocument rpt = new ReportDocument();
            try
            {
                rpt.Load(HttpContext.Current.Server.MapPath("~/Reports/Valuation/SummaryReport.rpt"));
                ParameterValues p1 = new ParameterValues();
                ParameterDiscreteValue v1 = new ParameterDiscreteValue();
                v1.Value = projectGroup; p1.Add(v1);
                rpt.DataDefinition.ParameterFields["@GroupName"].ApplyCurrentValues(p1);
                ParameterValues p2 = new ParameterValues();
                ParameterDiscreteValue v2 = new ParameterDiscreteValue();
                v2.Value = billingPeriod; p2.Add(v2);
                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(p2);

                ConnectionInfo ci = new ConnectionInfo();
                ci.ServerName = ConfigurationManager.AppSettings["ServerName"];
                ci.DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                ci.UserID = ConfigurationManager.AppSettings["UserID"];
                ci.Password = ConfigurationManager.AppSettings["Password"];
                foreach (CrystalDecisions.CrystalReports.Engine.Table table in rpt.Database.Tables)
                {
                    TableLogOnInfo li = table.LogOnInfo; li.ConnectionInfo = ci; table.ApplyLogOnInfo(li);
                }

                string filename = string.IsNullOrWhiteSpace(invoiceNumber)
                    ? projectGroup + "_" + billingPeriod + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                    : invoiceNumber.Replace(",", "_");
                filename = MakeSafeFileName(filename);
                string folder = HttpContext.Current.Server.MapPath("~/BillingDocuments/");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string filePath = Path.Combine(folder, filename + ".pdf");
                string virtualPath = "~/BillingDocuments/" + filename + ".pdf";
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                new bllTracking().InsertGroupAttachmentPath_QC(projectGroup, billingPeriod, virtualPath, invoiceNumber);
                return virtualPath;
            }
            finally { rpt.Close(); rpt.Dispose(); }
        }

        private static string MakeSafeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "BillingSummary_" + DateTime.Now.ToString("yyyyMMddHHmmss");

            foreach (char c in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');

            fileName = fileName.Replace('/', '_').Replace('\\', '_')
                               .Replace('#', '_').Replace('%', '_')
                               .Replace('?', '_').Replace(':', '_')
                               .Replace('*', '_').Replace('"', '_')
                               .Replace('<', '_').Replace('>', '_')
                               .Replace('|', '_');
            return fileName.Trim().TrimEnd('.');
        }

        private static string BuildEmailTemplate(string groupName, string billingPeriod)
        {
            DataTable dtClientDetails = new bllTracking().GetSummaryReportInvoice(groupName, billingPeriod);
            DataTable dtClientInfo = new bllTracking().GetClientDetails(groupName, billingPeriod);
            string clientName = "";
            if (dtClientInfo != null && dtClientInfo.Rows.Count > 0) clientName = Convert.ToString(dtClientInfo.Rows[0]["PAI_Contact_Person"]);
            if (!string.IsNullOrWhiteSpace(clientName) && clientName.Contains(" ")) clientName = clientName.Substring(0, clientName.IndexOf(" "));
            string invNo = dtClientDetails != null && dtClientDetails.Rows.Count > 0 ? Convert.ToString(dtClientDetails.Rows[0]["InvNo"]) : "";
            StringBuilder b = new StringBuilder();
            b.Append("<div style='font-family:Arial,sans-serif;font-size:13px'>");
            b.Append("<p><b>Hello " + HttpUtility.HtmlEncode(clientName) + ",</b></p>");
            b.Append("<p>Good Morning!!</p><p>Please find attached invoice# " + HttpUtility.HtmlEncode(invNo) + " for your review.</p>");
            b.Append("<p>Kindly, for all invoice related queries direct email to:<br/>anita@Infinity-data.com<br/>mdk@Infinity-data.com</p>");
            b.Append("<p>Thanks,<br/>Anita Londhe<br/>VP Controller<br/>Infinity IPS</p>");
            b.Append("<p style='color:red'><b>**WE DO NOT ACCEPT OR REQUEST CHANGES TO WIRING INSTRUCTION VIA EMAIL - Always call to verify**</b></p>");
            b.Append("</div>");
            return b.ToString();
        }

        private static List<object> ToSummaryRows(DataTable dt)
        {
            List<object> rows = new List<object>();
            if (dt == null) return rows;
            foreach (DataRow r in dt.Rows)
            {
                rows.Add(new
                {
                    GroupNumber = Col(r, "GroupNumber"),
                    BillingPeriod = Col(r, "BillingPeriod"),
                    InvoiceOrderCount = Col(r, "InvoiceOrderCount"),
                    InvoiceTotal = Col(r, "InvoiceTotal"),
                    AddedDate = FormatDate(r, "AddedDate"),
                    InvoiceSent = Col(r, "InvoiceSent"),
                    DeliveredDate = FormatDate(r, "DeliveredDate"),
                    ReadDate = FormatDate(r, "ReadDate")
                });
            }
            return rows;
        }

        private static string Col(DataRow r, string name)
        {
            return r.Table.Columns.Contains(name) && r[name] != DBNull.Value ? Convert.ToString(r[name]) : "";
        }
        private static string FormatDate(DataRow r, string name)
        {
            if (!r.Table.Columns.Contains(name) || r[name] == DBNull.Value) return "";
            DateTime d; return DateTime.TryParse(Convert.ToString(r[name]), out d) ? d.ToString("dd-MMM-yyyy") : Convert.ToString(r[name]);
        }
        private static string ToHtmlDate(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(value))) return "";
            DateTime d; return DateTime.TryParse(Convert.ToString(value), out d) ? d.ToString("yyyy-MM-dd") : "";
        }
    }
}
