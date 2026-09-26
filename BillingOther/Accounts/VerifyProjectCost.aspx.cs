using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;

namespace BillingOther.Accounts
{
    public partial class VerifyProjectCost : Page
    {
        private readonly bllTracking bllMaster = new bllTracking();

        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = Master.FindControl("lblHeader") as HtmlGenericControl;
            if (h4 != null) h4.InnerText = "Verify Project Cost";

            int projectId;
            int domainId;
            if (!int.TryParse(Request.QueryString["ProjectId"], out projectId)) projectId = 0;
            if (!int.TryParse(Request.QueryString["DomainId"], out domainId)) domainId = 0;

            if (!IsPostBack)
            {
                RenderSummary(projectId);
                ConfigurePage(projectId, domainId);
            }
        }

        private void RenderSummary(int projectId)
        {
            DataTable dt = bllMaster.BindVerifyProjectById(projectId);
            summaryContainer.InnerHtml = BuildSummary(dt);
        }

        private void ConfigurePage(int projectId, int domainId)
        {
            bool isFte = IsFteProject(projectId, domainId);
            dvFTE.Style["display"] = isFte ? "" : "none";
            dvGen.Style["display"] = isFte ? "none" : "";

            if (isFte)
            {
                BindFteCosting(projectId);
            }
            else
            {
                billingParamsBody.InnerHtml = RenderBillingParameters(projectId, domainId);
            }
        }

        private static bool IsFteProject(int projectId, int domainId)
        {
            int[] ids = {426,87,40,414,435,440,50,155,51,391,184,205,203,400,353,123,354,392,386,280,337,388,373,385,393,39,395,352};
            return Array.IndexOf(ids, projectId) >= 0 || domainId == 36 || domainId == 34 || domainId == 7;
        }

        private void BindFteCosting(int projectId)
        {
            if (projectId == 39)
            {
                tblOtherFTE.Style["display"] = "none";
                tbl861007.Style["display"] = "";
                DataTable dt = GetCostingDetails861(projectId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    txtfirst1200Rate.Value = Convert.ToString(dt.Rows[0]["Rate"]);
                    txtAdditionalBallotsRate.Value = Convert.ToString(dt.Rows[0]["RateForOthers"]);
                    txtWireTransferCharges.Value = Convert.ToString(dt.Rows[0]["ConditionalValue"]);
                }
            }
            else
            {
                tblOtherFTE.Style["display"] = "";
                tbl861007.Style["display"] = "none";
                if (new[] {353,123,435,440,155,354,392,386,280,393,434,442}.Contains(projectId)) tdRate.InnerHtml = "Rate Per FTE";
                else if (new[] {184,205,203,400,373,385}.Contains(projectId)) tdRate.InnerHtml = "Hourly Rate";
                else tdRate.InnerHtml = "Base Rate";
                DataTable dt = GetCostingDetailsFTE(projectId);
                if (dt != null && dt.Rows.Count > 0) txtHourlyRate.Value = Convert.ToString(dt.Rows[0]["Rate"]);
            }
        }

        private string RenderBillingParameters(int projectId, int domainId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_getallbillingParametersForCostingForCM");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 0, ParameterDirection.Input, projectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@DomainId", SqlDbType.Int, 0, ParameterDirection.Input, domainId);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            DataTable ddlValues = GetDDLValues();

            StringBuilder sb = new StringBuilder();
            int sr = 1;
            foreach (DataRow r in dt.Rows)
            {
                string id = Convert.ToString(r["IBP_Id"]);
                sb.Append("<tr data-id='").Append(HttpUtility.HtmlAttributeEncode(id)).Append("'>");
                sb.Append("<td>").Append(sr++).Append("</td><td>").Append(HttpUtility.HtmlEncode(Convert.ToString(r["IBP_ParameterName"]))).Append("</td>");
                sb.Append("<td>").Append(BuildSelect("js-comment verify-control", ddlValues, Convert.ToString(r["IBV_Comment"]))).Append("</td>");
                sb.Append("<td>").Append(BuildSelect("js-additional verify-control", ddlValues, Convert.ToString(r["IBV_Additional"]))).Append("</td>");
                sb.Append("<td>").Append(BuildChargeSelect(Convert.ToString(r["IBV_ChargeType"]))).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(Convert.ToString(r["PriceFromBDM"]))).Append("</td>");
                sb.Append("<td><input type='text' class='js-remark verify-control' value='").Append(HttpUtility.HtmlAttributeEncode(Convert.ToString(r["IBV_Remark"]))).Append("' /></td></tr>");
            }
            return sb.ToString();
        }

        private static string BuildSelect(string css, DataTable values, string selected)
        {
            StringBuilder sb = new StringBuilder("<select class='").Append(css).Append("'><option value='Select'>Select</option>");
            if (values != null)
            {
                foreach (DataRow row in values.Rows)
                {
                    string v = Convert.ToString(row["Text"]);
                    sb.Append("<option value='").Append(HttpUtility.HtmlAttributeEncode(v)).Append("'");
                    if (string.Equals(v, selected, StringComparison.OrdinalIgnoreCase)) sb.Append(" selected");
                    sb.Append(">").Append(HttpUtility.HtmlEncode(v)).Append("</option>");
                }
            }
            return sb.Append("</select>").ToString();
        }

        private static string BuildChargeSelect(string selected)
        {
            string[] values = {"Select","Fix Amount","Incremental","Percentage"};
            StringBuilder sb = new StringBuilder("<select class='js-charge-type verify-control'>");
            foreach (string v in values)
            {
                sb.Append("<option value='").Append(HttpUtility.HtmlAttributeEncode(v)).Append("'");
                if (string.Equals(v, selected, StringComparison.OrdinalIgnoreCase)) sb.Append(" selected");
                sb.Append(">").Append(HttpUtility.HtmlEncode(v)).Append("</option>");
            }
            return sb.Append("</select>").ToString();
        }

        private static string BuildSummary(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return "<div class='verify-summary-item'><span class='verify-summary-value'>No project summary available.</span></div>";
            DataRow r = dt.Rows[0];
            string[] cols = {"ProjectName","DomainName","ProjectStartDate","BillingCycle","BDM","CRM","UH"};
            string[] labels = {"Project #","Domain","Start Date","Billing Cycle","BDM","CRM","Unit Head"};
            StringBuilder sb = new StringBuilder("<div class='verify-summary-grid'>");
            for (int i = 0; i < cols.Length; i++)
            {
                string value = r.Table.Columns.Contains(cols[i]) ? Convert.ToString(r[cols[i]]) : "";
                sb.Append("<div class='verify-summary-item'><span class='verify-summary-label'>").Append(labels[i]).Append("</span><span class='verify-summary-value'>").Append(HttpUtility.HtmlEncode(value)).Append("</span></div>");
            }
            return sb.Append("</div>").ToString();
        }

        public DataTable GetDDLValues()
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_getDDLValues");
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        public DataTable GetCostingDetailsFTE(int projectId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFTEProjectCosting");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.BigInt, 0, ParameterDirection.Input, projectId);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        public DataTable GetCostingDetails861(int projectId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFTEProjectCostingFor861");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.BigInt, 0, ParameterDirection.Input, projectId);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        public class BillingRow
        {
            public int Id { get; set; }
            public string Comment { get; set; }
            public string Additional { get; set; }
            public string Remark { get; set; }
            public string ChargeType { get; set; }
        }
        public class AjaxResult { public bool Success { get; set; } public string Message { get; set; } }

        [WebMethod]
        public static AjaxResult ApproveBillingParameters(int projectId, List<BillingRow> rows)
        {
            try
            {
                bllTracking bll = new bllTracking();
                int updated = 0;
                foreach (BillingRow row in rows ?? new List<BillingRow>())
                {
                    if (string.IsNullOrWhiteSpace(row.Comment) || row.Comment == "Select") continue;
                    Hashtable ht = new Hashtable();
                    ht["IBV_ParameterId"] = row.Id;
                    ht["IBV_Comment"] = row.Comment;
                    ht["IBV_Additional"] = row.Additional;
                    ht["IBV_Remark"] = row.Remark;
                    ht["IBV_ChargeType"] = row.ChargeType;
                    ht["AddedBy"] = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
                    ht["ProjectId"] = projectId;
                    if (bll.ApproveBillParameter(ht) > 0) updated++;
                }
                return new AjaxResult { Success = updated > 0, Message = updated > 0 ? "Parameters approved successfully." : "No parameters were updated." };
            }
            catch (Exception ex) { return new AjaxResult { Success = false, Message = ex.Message }; }
        }

        [WebMethod]
        public static AjaxResult ApproveFteCost(int projectId)
        {
            try
            {
                SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_ApproveBillingParametersForCM_FTE");
                SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", SqlDbType.BigInt, 0, ParameterDirection.Input, projectId);
                SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", SqlDbType.BigInt, 0, ParameterDirection.Input, int.Parse(HttpContext.Current.User.Identity.Name));
                SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", SqlDbType.BigInt, 0, ParameterDirection.ReturnValue, null);
                SQLHelper.ExecuteNonQueryCmdBilling(cmd);
                int result = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                return new AjaxResult { Success = result > 0, Message = result > 0 ? "Costing approved successfully!" : "Unable to approve costing." };
            }
            catch (Exception ex) { return new AjaxResult { Success = false, Message = ex.Message }; }
        }
    }
}
