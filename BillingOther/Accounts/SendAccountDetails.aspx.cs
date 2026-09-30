using BillingOther.App_Code.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace BillingOther.Accounts
{
    public partial class SendAccountDetails : System.Web.UI.Page
    {

        bllTracking blltracking = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Yet To Be Billed";

            if (!IsPostBack)
            {
                BindDomain();
            }
        }

        private int SelectedDomainId
        {
            get { return ViewState["SelectedDomainId"] == null ? 0 : Convert.ToInt32(ViewState["SelectedDomainId"]); }
            set { ViewState["SelectedDomainId"] = value; }
        }

        public void BindDomain()
        {
            DataTable cachedDomains = Session["PendingBillingDomains"] as DataTable;
            DataTable cachedRows = Session["PendingBillingRows"] as DataTable;
            DateTime cachedAt = Session["PendingBillingCachedAt"] is DateTime ? (DateTime)Session["PendingBillingCachedAt"] : DateTime.MinValue;
            if (cachedDomains != null && cachedRows != null && DateTime.Now.Subtract(cachedAt).TotalMinutes < 2)
            {
                pnlNoDomains.Visible = cachedDomains.Rows.Count == 0;
                SelectedDomainId = cachedDomains.Rows.Count == 0 ? 0 : Convert.ToInt32(cachedDomains.Rows[0]["DomainID"]);
                rptDomains.DataSource = cachedDomains;
                rptDomains.DataBind();
                rptBillingDetails.DataSource = cachedRows;
                rptBillingDetails.DataBind();
                lblRecords.Text = cachedRows.Rows.Count + " pending";
                return;
            }

            DataTable domains = blltracking.getalldomains(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            if (domains == null) domains = new DataTable();
            DataTable pendingDomains = domains.Clone();
            DataTable allPending = null;
            DataTable combined = null;
            try { combined = blltracking.GetAllProjectSendToAccountsDetailsBasedonDomain(0, null, 0); }
            catch { combined = null; }
            if (combined != null && combined.Rows.Count > 0 && combined.Columns.Contains("DomainID"))
            {
                HashSet<int> allowedDomains = new HashSet<int>(domains.AsEnumerable().Select(row => Convert.ToInt32(row["DomainID"])));
                IEnumerable<DataRow> permittedRows = combined.AsEnumerable().Where(row => allowedDomains.Contains(Convert.ToInt32(row["DomainID"])));
                allPending = permittedRows.Any() ? permittedRows.CopyToDataTable() : combined.Clone();
                HashSet<int> pendingIds = new HashSet<int>(allPending.AsEnumerable().Select(row => Convert.ToInt32(row["DomainID"])));
                foreach (DataRow domain in domains.Rows)
                    if (pendingIds.Contains(Convert.ToInt32(domain["DomainID"]))) pendingDomains.ImportRow(domain);
            }
            else
            {
                // Compatibility fallback for older versions of the existing procedure.
                foreach (DataRow domain in domains.Rows)
                {
                    int domainId;
                    if (!int.TryParse(Convert.ToString(domain["DomainID"]), out domainId)) continue;
                    DataTable pending = blltracking.GetAllProjectSendToAccountsDetailsBasedonDomain(0, null, domainId);
                    if (pending == null || pending.Rows.Count == 0) continue;
                    pendingDomains.ImportRow(domain);
                    DataTable domainRows = pending.Copy();
                    if (!domainRows.Columns.Contains("DomainID")) domainRows.Columns.Add("DomainID", typeof(int));
                    foreach (DataRow row in domainRows.Rows) row["DomainID"] = domainId;
                    if (allPending == null) allPending = domainRows.Clone();
                    allPending.Merge(domainRows, true, MissingSchemaAction.Add);
                }
            }

            pnlNoDomains.Visible = pendingDomains.Rows.Count == 0;
            SelectedDomainId = pendingDomains.Rows.Count == 0 ? 0 : Convert.ToInt32(pendingDomains.Rows[0]["DomainID"]);
            rptDomains.DataSource = pendingDomains;
            rptDomains.DataBind();
            rptBillingDetails.DataSource = allPending;
            rptBillingDetails.DataBind();
            lblRecords.Text = (allPending == null ? 0 : allPending.Rows.Count) + " pending";
            Session["PendingBillingDomains"] = pendingDomains.Copy();
            Session["PendingBillingRows"] = allPending == null ? new DataTable() : allPending.Copy();
            Session["PendingBillingCachedAt"] = DateTime.Now;
        }

        protected string GetDomainCss(object domainId)
        {
            return Convert.ToInt32(domainId) == SelectedDomainId ? "billing-domain-link active" : "billing-domain-link";
        }

        public void BindGridDomainWise(int DomainID)
        {
            DataTable dt = blltracking.GetAllProjectSendToAccountsDetailsBasedonDomain(0, null, DomainID);
            if (dt != null && dt.Rows.Count > 0)
            {
                rptBillingDetails.DataSource = dt;
                rptBillingDetails.DataBind();

                lblRecords.Text = dt.Rows.Count + " pending";
            }
            else
            {
                rptBillingDetails.DataSource = null;
                rptBillingDetails.DataBind();
                lblRecords.Text = "0 pending";
            }
        }


        protected string GetDetailsUrl(object projectId, object billingPeriod, object projectName)
        {
            return ResolveUrl("~/Accounts/SendToAccounts.aspx") + "?ProjectID=" + Server.UrlEncode(Convert.ToString(projectId))
                + "&BillingPeriod=" + Server.UrlEncode(Convert.ToString(billingPeriod))
                + "&ProjectName=" + Server.UrlEncode(Convert.ToString(projectName));
        }

        protected string GetFteDisplay(object projectIdValue, object projectNameValue, object billingPeriodValue, object typeValue)
        {
            if (Convert.ToString(typeValue).IndexOf("FTE", StringComparison.OrdinalIgnoreCase) < 0) return "—";
            int projectId;
            if (!int.TryParse(Convert.ToString(projectIdValue), out projectId)) return "FTE";
            string projectName = Convert.ToString(projectNameValue);
            string billingPeriod = Convert.ToString(billingPeriodValue).Replace(" to ", " ~ ");
            StringBuilder html = new StringBuilder("<details class=\"fte-details\"><summary>View FTE details</summary>");
            try
            {
                System.Data.SqlClient.SqlCommand cmd = BillingOther.App_Code.DAL.SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFETBillingData");
                BillingOther.App_Code.DAL.SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 10, ParameterDirection.Input, projectId);
                BillingOther.App_Code.DAL.SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 500, ParameterDirection.Input, billingPeriod);
                DataTable details = BillingOther.App_Code.DAL.SQLHelper.ExecuteDataTableCmd(cmd);
                if (projectName.IndexOf("791-001", StringComparison.OrdinalIgnoreCase) >= 0) AppendExtraFte(details, projectId, billingPeriod);
                if (projectId == 203 || projectId == 400) details = BuildAverageFteDisplay(details, projectId, billingPeriod);
                html.Append(RenderTable(details));
            }
            catch { html.Append("<p class=\"portal-muted\">FTE details are unavailable.</p>"); }
            return html.Append("</details>").ToString();
        }

        private static void AppendExtraFte(DataTable details, int projectId, string billingPeriod)
        {
            if (details == null || !details.Columns.Contains("Date")) return;
            System.Data.SqlClient.SqlCommand cmd = BillingOther.App_Code.DAL.SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFTEBillingExtraOperators");
            BillingOther.App_Code.DAL.SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 10, ParameterDirection.Input, projectId);
            BillingOther.App_Code.DAL.SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 500, ParameterDirection.Input, billingPeriod);
            DataTable extra = BillingOther.App_Code.DAL.SQLHelper.ExecuteDataTableCmd(cmd);
            if (extra == null || extra.Rows.Count == 0) return;
            int maxOperator = extra.AsEnumerable().Max(r => Convert.ToInt32(r["ExtraOperatorNo"]));
            for (int i = 1; i <= maxOperator; i++) if (!details.Columns.Contains("Extra Operator " + i + " (Hours)")) details.Columns.Add("Extra Operator " + i + " (Hours)");
            foreach (DataRow row in details.Rows)
            {
                DateTime date;
                if (!DateTime.TryParse(Convert.ToString(row["Date"]), out date)) continue;
                foreach (DataRow item in extra.AsEnumerable().Where(r => Convert.ToDateTime(r["WorkDate"]).Date == date.Date))
                {
                    int number = Convert.ToInt32(item["ExtraOperatorNo"]);
                    row["Extra Operator " + number + " (Hours)"] = Convert.ToDecimal(item["Hours"]).ToString("0.##");
                }
            }
        }

        private static DataTable BuildAverageFteDisplay(DataTable source, int projectId, string billingPeriod)
        {
            DataTable result = new DataTable();
            foreach (string name in new[] { "S.No", "Date", "Target Assigned", "Total invoices", "Average FTE", "Billed FTE" }) result.Columns.Add(name);
            if (source == null) return result;
            System.Data.SqlClient.SqlCommand cmd = BillingOther.App_Code.DAL.SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFTEAverageBillingData");
            BillingOther.App_Code.DAL.SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 10, ParameterDirection.Input, projectId);
            BillingOther.App_Code.DAL.SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 500, ParameterDirection.Input, billingPeriod);
            DataTable averages = BillingOther.App_Code.DAL.SQLHelper.ExecuteDataTableCmd(cmd);
            int serial = 1;
            foreach (DataRow item in source.Rows)
            {
                DataRow row = result.NewRow();
                string dateText = source.Columns.Contains("Date") ? Convert.ToString(item["Date"]) : "";
                string process = source.Columns.Contains("Process") ? Convert.ToString(item["Process"]) : "";
                string target = source.Columns.Contains("AssignTarget") ? Convert.ToString(item["AssignTarget"]) : "";
                string invoices = source.Columns.Contains("TotalInvoices") ? Convert.ToString(item["TotalInvoices"]) : "";
                row["S.No"] = serial++; row["Date"] = dateText; row["Target Assigned"] = target; row["Total invoices"] = invoices;
                DateTime date;
                DataRow average = DateTime.TryParse(dateText, out date) && averages != null
                    ? averages.AsEnumerable().FirstOrDefault(a => Convert.ToDateTime(a["Date"]).Date == date.Date && string.Equals(Convert.ToString(a["ProcessName"]), process, StringComparison.OrdinalIgnoreCase)) : null;
                bool holiday = string.Equals(target, "Holiday", StringComparison.OrdinalIgnoreCase) || string.Equals(invoices, "Holiday", StringComparison.OrdinalIgnoreCase);
                row["Average FTE"] = holiday ? "Holiday" : average == null ? "" : Convert.ToDecimal(average["AverageFTE"]).ToString("0.00");
                row["Billed FTE"] = holiday ? "Holiday" : average == null ? "" : Convert.ToDecimal(average["BilledFTE"]).ToString("0");
                result.Rows.Add(row);
            }
            return result;
        }

        private static string RenderTable(DataTable table)
        {
            if (table == null || table.Rows.Count == 0) return "<p class=\"portal-muted\">No FTE details found.</p>";
            StringBuilder html = new StringBuilder("<div class=\"fte-details-scroll\"><table class=\"table table-condensed\"><thead><tr>");
            foreach (DataColumn column in table.Columns) html.Append("<th>").Append(HttpUtility.HtmlEncode(column.ColumnName)).Append("</th>");
            html.Append("</tr></thead><tbody>");
            foreach (DataRow row in table.Rows)
            {
                html.Append("<tr>");
                foreach (DataColumn column in table.Columns) html.Append("<td>").Append(HttpUtility.HtmlEncode(Convert.ToString(row[column]))).Append("</td>");
                html.Append("</tr>");
            }
            return html.Append("</tbody></table></div>").ToString();
        }

    }
}
