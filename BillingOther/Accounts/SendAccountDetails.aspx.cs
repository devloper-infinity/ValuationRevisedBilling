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
                ddlPeriodBind();
                BindDomain();
            }
        }

        public void BindDomain()
        {
            DataTable dtdomain = new DataTable();
            dtdomain = blltracking.getalldomains(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            ddldomain.DataSource = dtdomain;
            ddldomain.DataValueField = "DomainID";
            ddldomain.DataTextField = "DomainName";
            ddldomain.DataBind();
            ddldomain.Items.Insert(0, new ListItem("Select"));
        }

        public void BindProjects(int DomainID)
        {
            DataTable dt = blltracking.GetAllProjectByDomainWise(DomainID, Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            if (dt.Rows.Count > 0)
            {
                ddlProjects.DataSource = dt;
                ddlProjects.DataTextField = "ProjectName";
                ddlProjects.DataValueField = "ProjectId";
                ddlProjects.DataBind();
                ddlProjects.Items.Insert(0, new ListItem("Select"));
            }
            else
            {
                ddlProjects.Items.Clear();
            }

        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            string BillingPeriod = (ddlPeriod.SelectedItem.Text == "Select" ? "Select" : ddlPeriod.SelectedItem.Text);
            BindGridDomainWise(Convert.ToInt32(ddlProjects.SelectedValue), Convert.ToInt32(ddldomain.SelectedValue), BillingPeriod);

        }

        public void BindGridDomainWise(int ProjectId, int DomainID, string BillingPeriod)
        {
            DataTable dt = new DataTable();
            dt = blltracking.GetAllProjectSendToAccountsDetailsBasedonDomain(ProjectId, BillingPeriod, DomainID);
            if (dt.Rows.Count > 0)
            {
                rptBillingDetails.DataSource = dt;
                rptBillingDetails.DataBind();

                lblRecords.Text = "Total Records : " + dt.Rows.Count.ToString();
            }
            else
            {
                rptBillingDetails.DataSource = null;
                rptBillingDetails.DataBind();
                lblRecords.Text = "Total Records : " + dt.Rows.Count.ToString();
            }
        }

        protected void ddlPeriodBind()
        {
            DateTime now = DateTime.Now;
            DateTime fromDate = now.AddMonths(-5);
            DateTime fromDateNew = now.AddMonths(-5);
            ddlPeriod.Items.Clear();
            ddlPeriod.Items.Add("Select");
            for (int i = 0; i < 6; i++)
            {
                string Month = fromDate.ToString("MMM");
                var startDate = new DateTime(fromDate.Year, fromDate.Month, 1);
                string start = startDate.ToString("dd-MMM-yyyy");
                var endDate = (startDate.AddMonths(1).AddDays(-1));
                string End = endDate.ToString("dd-MMM-yyyy");

                string FirstHalf = Convert.ToString("01-" + Month + "-" + fromDate.Year + " ~ 15-" + Month + "-" + fromDate.Year);
                string secondHalf = Convert.ToString("16-" + Month + "-" + fromDate.Year + " ~ " + End);

                ddlPeriod.Items.Add(FirstHalf);
                ddlPeriod.Items.Add(secondHalf);
                fromDate = fromDateNew.AddMonths(i + 1);
            }
        }

        public void BindGridDomainWise(int DomainID)
        {
            DataTable dt = new DataTable();
            dt = blltracking.GetAllProjectSendToAccountsDetailsBasedonDomain(0, null, DomainID);
            if (dt.Rows.Count > 0)
            {
                rptBillingDetails.DataSource = dt;
                rptBillingDetails.DataBind();

                lblRecords.Text = "Total Records : " + dt.Rows.Count.ToString();
            }
            else
            {
                rptBillingDetails.DataSource = null;
                rptBillingDetails.DataBind();
                lblRecords.Text = "Total Records : " + dt.Rows.Count.ToString();
            }
        }

        public void BindGrid()
        {
            DataTable dt = new DataTable();
            //dt = blltracking.GetAllProjectSendToAccountsDetailsBasedonDomain(int.Parse(ddlProjects.SelectedValue), "Select",0);
            dt = blltracking.GetAllProjectSendToAccountsDetails(int.Parse(ddlProjects.SelectedValue), ddlPeriod.SelectedItem.ToString());

            rptBillingDetails.DataSource = dt;
            rptBillingDetails.DataBind();

            lblRecords.Text = "Total Records : " + dt.Rows.Count.ToString();
        }

        public void BindProjects()
        {
            ddlProjects.DataSource = blltracking.GetAllProjectByUserRights();
            ddlProjects.DataTextField = "ProjectName";
            ddlProjects.DataValueField = "ProjectId";
            ddlProjects.DataBind();
            ddlProjects.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));
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

        protected void ddldomain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddldomain.SelectedValue != "Select")
            {
                BindProjects(Convert.ToInt32(ddldomain.SelectedValue));
                BindGridDomainWise(Convert.ToInt32(ddldomain.SelectedValue));
                Session["DomainID"] = ddldomain.SelectedValue;
            }
            else
            {
                Session["DomainID"] = null;
                ddlProjects.Items.Clear();
                //ddlFieldName.Items.Clear();
            }
        }
    }
}
