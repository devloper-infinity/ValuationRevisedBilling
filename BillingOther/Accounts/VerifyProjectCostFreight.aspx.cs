using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using BillingOther.App_Code.DAL;
using BillingOther.App_Code.BLL;

namespace BillingOther.Accounts
{
    public partial class VerifyProjectCostFreight : System.Web.UI.Page
    {
        bllTracking bllMaster = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Verify Project Cost";

            if (Convert.ToString(Request.Url).Contains("ProjectId"))
            {
                if (Convert.ToInt32(Request.QueryString["ProjectId"]) == 48)
                {
                    tbl771.Style.Add("display", "");
                    tblOther.Style.Add("display", "none");
                }
                else
                {
                    tbl771.Style.Add("display", "none");
                    tblOther.Style.Add("display", "");
                }
            }
            if (!Page.IsPostBack)
            {
                bindgrid();
                BindBillingHeaderBase(Convert.ToInt32(Request.QueryString["ProjectId"]));
                BindFreightCosting(int.Parse(Request.QueryString["ProjectId"]));
            }

        }
        public void BindBillingHeaderBase(int ProjectID)
        {
            if (ProjectID == 46 || ProjectID == 227)
            {
                ddlBillingHeaderBaseRate.Items.Clear();
                ddlBillingHeaderBaseRate.Items.Insert(0, new ListItem("Select"));
                ddlBillingHeaderBaseRate.Items.Insert(1, new ListItem("No of Invoices Delivered"));
            }

        }



        protected void btnBack_Click(object sender, EventArgs e)
        {
            //Session["CostId"] = null;
            Response.Redirect("~/Accounts/VerifyCostMaster.aspx");
        }

        public void BindFreightCosting(int ProjectId)
        {
            DataTable dt = GetCostingDetailsFreight(ProjectId);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    string BillingBase = Convert.ToString(dt.Rows[0]["BillingBase"]);
                    if (ProjectId == 48)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //No of Pages - Scan
                            if (Convert.ToString(dt.Rows[i]["ProjectColumn"]) == "No of Pages")
                            {
                                ddlChargeType771ScanPages.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPrice771ScanPages.Value = Convert.ToString(dt.Rows[i]["Rate"]);
                                ddlBillingHeader771ScanPages.Value = Convert.ToString(dt.Rows[i]["ProjectColumn"]);
                            }
                            else if (Convert.ToString(dt.Rows[i]["ProjectColumn"]) == "No of Records" && Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "Scan")
                            {
                                //No of Records - Scan
                                ddlChargeType771ScanRecords.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPrice771ScanRecords.Value = Convert.ToString(dt.Rows[i]["Rate"]);
                                ddlBillingHeader771ScanRecords.Value = Convert.ToString(dt.Rows[i]["ProjectColumn"]);
                            }
                            else if (Convert.ToString(dt.Rows[i]["ProjectColumn"]) == "No of Records" && Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "Mail")
                            {
                                //No of Records - Mail
                                ddlChargeType771MailRecords.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPrice771MailRecords.Value = Convert.ToString(dt.Rows[i]["Rate"]);
                                ddlBillingHeader771MailRecords.Value = Convert.ToString(dt.Rows[i]["ProjectColumn"]);
                            }
                        }
                    }
                    else if (BillingBase == "Base")
                    {
                        ddlBillingBase.SelectedIndex = 1;
                        trBaseRate.Style.Add("display", "");
                        trOrderType.Style.Add("display", "none");

                        string find = "ProjectColumn in ('Order#','Dispatched Record','No Of Records','No of Invoices Delivered') and AdditionalConditions=0 ";
                        DataRow[] foundRows = dt.Select(find);
                        if (foundRows.Length > 0)
                        {
                            ddlIsApplicableBaseRate.Value = "Yes";
                            ddlChargeTypeBaseRate.Value = Convert.ToString(foundRows[0]["Updatelevel"]);
                            txtPriceBaseRate.Value = Convert.ToString(foundRows[0]["RateForOthers"]);
                            ddlBillingHeaderBaseRate.Value = Convert.ToString(foundRows[0]["ProjectColumn"]);
                        }
                        find = "ProjectColumn = '# of Character'";
                        DataRow[] foundRows2 = dt.Select(find);
                        if (foundRows2.Length > 0)
                        {
                            ddlIsApplicableBaseRateCharacter.Value = "Yes";
                            ddlChargeTypeBaseRateCharacter.Value = Convert.ToString(foundRows2[0]["Updatelevel"]);
                            txtPriceBaseRateCharacter.Value = Convert.ToString(foundRows2[0]["RateForOthers"]);
                            ddlBillingHeaderBaseRateCharacter.Value = Convert.ToString(foundRows2[0]["ProjectColumn"]);
                        }
                        else
                        {
                            ddlIsApplicableBaseRateCharacter.Value = "No";
                            ddlChargeTypeBaseRateCharacter.Value = Convert.ToString("Select");
                            txtPriceBaseRateCharacter.Value = Convert.ToString("");
                            ddlBillingHeaderBaseRateCharacter.Value = Convert.ToString("Select");
                        }
                        find = "ProjectColumn = 'Rush'";
                        DataRow[] foundRows3 = dt.Select(find);
                        if (foundRows3.Length > 0)
                        {
                            ddlIsApplicableBaseRateRush.Value = "Yes";
                            ddlChargeTypeBaseRateRush.Value = Convert.ToString(foundRows3[0]["Updatelevel"]);
                            txtPriceBaseRateRush.Value = Convert.ToString(foundRows3[0]["RateForOthers"]);
                            ddlBillingHeaderBaseRateRush.Value = Convert.ToString(foundRows3[0]["ProjectColumn"]);
                        }
                        else
                        {
                            ddlIsApplicableBaseRateRush.Value = "No";
                            ddlChargeTypeBaseRateRush.Value = Convert.ToString("Select");
                            txtPriceBaseRateRush.Value = Convert.ToString("");
                            ddlBillingHeaderBaseRateRush.Value = Convert.ToString("Select");
                        }
                        //}
                    }

                    else if (BillingBase == "Invoice Type")
                    {
                        ddlBillingBase.SelectedIndex = 3;

                        trBaseRate.Style.Add("display", "none");
                        trOrderType.Style.Add("display", "");

                        string find = "ProjectColumn in ('Order#') and AdditionalConditions=1 and Updatelevel='Fix Amount'";
                        DataRow[] foundRows = dt.Select(find);
                        if (foundRows.Length > 0)
                        {
                            ddlBaseOrderType.Value = Convert.ToString(foundRows[0]["ConditionalValue"]);
                            hdOrderBaseLabel.Value = Convert.ToString(foundRows[0]["ConditionalValue"]);
                            lblOrderTypeBase.InnerText = Convert.ToString(foundRows[0]["ConditionalValue"]);
                            ddlIsApplicableOrderBase.Value = "Yes";
                            ddlChargeTypeOrderBase.Value = Convert.ToString(foundRows[0]["Updatelevel"]);
                            txtPriceOrderBase.Value = Convert.ToString(foundRows[0]["RateForOthers"]);
                            ddlBillingHeaderOrderBase.Value = Convert.ToString(foundRows[0]["ConditionalColumn"]);
                        }
                        find = "ProjectColumn in ('Order#') and AdditionalConditions=1 and ConditionalValue not in ('" + ddlBaseOrderType.Value + "')";
                        DataRow[] foundRows1 = dt.Select(find);
                        if (foundRows.Length > 0)
                        {
                            lblOrderTypeOther.InnerText = Convert.ToString(foundRows1[0]["ConditionalValue"]);
                            hdOrderOtherLabel.Value = Convert.ToString(foundRows1[0]["ConditionalValue"]);
                            ddlIsApplicableOrderOther.Value = "Yes";
                            ddlChargeTypeOrderOther.Value = Convert.ToString(foundRows1[0]["Updatelevel"]);
                            txtPriceOrderOther.Value = Convert.ToString(foundRows1[0]["RateForOthers"]);
                            ddlBillingHeaderOrderOther.Value = Convert.ToString(foundRows1[0]["ConditionalColumn"]);
                        }
                        find = "ProjectColumn = '# of Character'";
                        DataRow[] foundRows2 = dt.Select(find);
                        if (foundRows2.Length > 0)
                        {
                            ddlIsApplicableOrderCharacter.Value = "Yes";
                            ddlChargeTypeOrderCharacter.Value = Convert.ToString(foundRows2[0]["Updatelevel"]);
                            txtPriceOrderCharacter.Value = Convert.ToString(foundRows2[0]["RateForOthers"]);
                            ddlBillingHEaderOrderCharacter.Value = Convert.ToString(foundRows2[0]["ProjectColumn"]);
                        }
                        else
                        {
                            ddlIsApplicableOrderCharacter.Value = "No";
                            ddlChargeTypeOrderCharacter.Value = Convert.ToString("Select");
                            txtPriceOrderCharacter.Value = Convert.ToString("");
                            ddlBillingHEaderOrderCharacter.Value = Convert.ToString("Select");
                        }
                        //}
                    }
                }
            }
        }

        #region Database New - For Typing specific projects
        public DataTable GetProductTypeByProject(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "WBT_usp_GetProductTypeByProject");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        public DataTable GetCostingDetailsFreight(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetFreightProjectCosting");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        #endregion

        public int InsertCostingWithParameters_Freight(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_Billing_InsertCostingwithBillingParameters_Typing_Temp");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, htParam["ProjectId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@CostingColumn", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["CostingColumn"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@RemarkCost", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["RemarkCost"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@CostingColumnValue", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["CostingColumnValue"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ChargeType", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, htParam["ChargeType"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@RateForOthers", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, htParam["RateForOthers"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        protected void btnShubmitTyping_Click(object sender, EventArgs e)
        {
            Hashtable htParam = new Hashtable();
            htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ProjectId"])));
            htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            int result = ApproveBillParameter_Freight(htParam);
            if (result > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "Costing approved successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);              
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
                dvError.InnerHtml = "Error occured while approving costing!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                
            }
            BindFreightCosting(Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
        }

        public int ApproveBillParameter_Freight(Hashtable htParam)
        {
            string ProcedureName = "";
            if (Convert.ToString(htParam["ProjectId"]) == "227")
                ProcedureName = "usp_ApproveBillingParametersForCM_Freight736";
            else
                ProcedureName = "usp_ApproveBillingParametersForCM_Freight";
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, ProcedureName);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["ProjectId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        protected void btnSubmit771_Click(object sender, EventArgs e)
        {
            Hashtable htParam = new Hashtable();
            htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ProjectId"])));
            htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            int result = ApproveBillParameter_Freight(htParam);
            if (result > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "Costing approved successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);             
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
                dvError.InnerHtml = "Error occured while approving costing!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);              
            }
            BindFreightCosting(Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));

        }        public void bindgrid()
        {
            DataTable dt = bllMaster.BindVerifyProjectById(int.Parse(Request.QueryString["ProjectId"]));
            summaryContainer.InnerHtml = RenderSummary(dt);
        }

        private static string RenderSummary(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return "<div class=\"verify-summary-item\"><span class=\"verify-summary-value\">No project summary available.</span></div>";
            DataRow r = dt.Rows[0];
            string[] cols = { "ProjectName", "DomainName", "ProjectStartDate", "BillingCycle", "BDM", "CRM", "UH" };
            string[] labels = { "Project #", "Domain", "Start Date", "Billing Cycle", "BDM", "CRM", "Unit Head" };
            StringBuilder sb = new StringBuilder("<div class='verify-summary-grid'>");
            for (int i = 0; i < cols.Length; i++)
            {
                string v = r.Table.Columns.Contains(cols[i]) ? Convert.ToString(r[cols[i]]) : "";
                sb.Append("<div class='verify-summary-item'><span class='verify-summary-label'>").Append(HttpUtility.HtmlEncode(labels[i])).Append("</span><span class='verify-summary-value'>").Append(HttpUtility.HtmlEncode(v)).Append("</span></div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }
        public class AjaxResult { public bool Success { get; set; } public string Message { get; set; } }

        [WebMethod]
        public static AjaxResult ApproveCosting(int projectId)
        {
            try
            {
                string procedureName = projectId == 227 ? "usp_ApproveBillingParametersForCM_Freight736" : "usp_ApproveBillingParametersForCM_Freight";
                SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, procedureName);
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
