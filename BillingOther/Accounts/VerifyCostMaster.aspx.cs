using BillingOther.App_Code.BLL;
using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace BillingOther.Accounts
{
    public partial class VerifyCostMaster : Page
    {
        private readonly bllTracking bllMaster = new bllTracking();

        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = Master.FindControl("lblHeader") as HtmlGenericControl;
            if (h4 != null) h4.InnerText = "Verify Project Price";

            if (!IsPostBack)
            {
                BindProjectTables();
            }
        }

        private void BindProjectTables()
        {
            newPriceBody.InnerHtml = RenderRows(bllMaster.BindVerifyProject());
            modifiedPriceBody.InnerHtml = RenderRows(bllMaster.BindVerifyProjectForModifiedRates());
        }

        private static string RenderRows(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return string.Empty;

            StringBuilder html = new StringBuilder();
            int sr = 1;
            foreach (DataRow row in dt.Rows)
            {
                string projectId = Convert.ToString(row["ProjectId"]);
                string domainId = Convert.ToString(row["DomainId"]);
                string url = GetVerifyUrl(projectId, domainId);

                html.Append("<tr>");
                html.Append("<td><a class='verify-action-btn' href='").Append(HttpUtility.HtmlAttributeEncode(url)).Append("' onclick=\"showStandardLoading('Opening project cost...')\"><i class='fa fa-eye'></i> View</a></td>");
                html.Append("<td>").Append(sr++).Append("</td>");
                html.Append(Cell(row, "ProjectName"));
                html.Append(Cell(row, "DomainName"));
                html.Append(Cell(row, "ProjectStartDate"));
                html.Append(Cell(row, "BillingCycle"));
                html.Append(Cell(row, "BDM"));
                html.Append(Cell(row, "CRM"));
                html.Append(Cell(row, "UH"));
                html.Append(Cell(row, "AddedBy"));
                html.Append(Cell(row, "AddedDate"));
                html.Append("</tr>");
            }
            return html.ToString();
        }

        private static string Cell(DataRow row, string column)
        {
            string value = row.Table.Columns.Contains(column) ? Convert.ToString(row[column]) : string.Empty;
            return "<td>" + HttpUtility.HtmlEncode(value) + "</td>";
        }

        private static string GetVerifyUrl(string projectId, string domainId)
        {
            string page;
            if (projectId == "373" || projectId == "385" || projectId == "203" || projectId == "400" || projectId == "87" || domainId == "36" || projectId == "426")
                page = "VerifyProjectCost.aspx";
            else if (domainId == "19")
                page = "VerifyProjectCostTyping.aspx";
            else if (domainId == "2")
                page = "VerifyProjectCostFreight.aspx";
            else
                page = "VerifyProjectCost.aspx";

            return page + "?ProjectId=" + HttpUtility.UrlEncode(projectId) + "&DomainId=" + HttpUtility.UrlEncode(domainId);
        }
    }
}
