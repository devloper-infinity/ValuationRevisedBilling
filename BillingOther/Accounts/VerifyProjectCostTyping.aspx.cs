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
    public partial class VerifyProjectCostTyping : System.Web.UI.Page
    {
        bllTracking bllMaster = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Verify Project Cost";
            if (!Page.IsPostBack)
            {
                bindgrid();
                //BindCostDetails(int.Parse(Request.QueryString["ProjectId"]));
                BindBaseProductType(Convert.ToString(Request.QueryString["ProjectId"]));
                BindTypingCosting(int.Parse(Request.QueryString["ProjectId"]));
            }

        }



        protected void btnBack_Click(object sender, EventArgs e)
        {
            //Session["CostId"] = null;
            Response.Redirect("~/Accounts/VerifyCostMaster.aspx");
        }

        public void BindBaseProductType(string ProjectID)
        {
            ddlBaseProductType.DataSource = GetProductTypeByProject(Convert.ToInt32(ProjectID));
            ddlBaseProductType.DataValueField = "Product_Type";
            ddlBaseProductType.DataTextField = "Product_Type";
            ddlBaseProductType.DataBind();
            ddlBaseProductType.Items.Insert(0, new ListItem("Select"));
        }

        public void BindTypingCosting(int ProjectId)
        {
            DataTable dt = GetCostingDetailsTyping(ProjectId);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    string BillingBase = Convert.ToString(dt.Rows[0]["BillingBase"]);
                    if (BillingBase == "Base")
                    {
                        ddlBillingBase.SelectedIndex = 1;
                        trBaseRate.Style.Add("display", "");
                        trProductType.Style.Add("display", "none");
                        trOrderType.Style.Add("display", "none");

                        string find = "ProjectColumn in ('Order#','Dispatched Record','No Of Records') and AdditionalConditions=0 ";
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
                    else if (BillingBase == "Product Type")
                    {
                        ddlBillingBase.SelectedIndex = 2;

                        trBaseRate.Style.Add("display", "none");
                        trProductType.Style.Add("display", "");
                        trOrderType.Style.Add("display", "none");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "Purchase")
                            {
                                ddlIsApplicablePurchase.Value = "Yes";
                                ddlChargeTypePurchase.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPricePurchase.Value = Convert.ToString(dt.Rows[i]["RateForOthers"]);
                                ddlBillingHeaderPurchase.Value = Convert.ToString(dt.Rows[i]["ConditionalColumn"]);
                            }
                            else if (Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "Refinance")
                            {
                                ddlIsApplicableRefinance.Value = "Yes";
                                ddlChargeTypeRefinance.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPriceRefinance.Value = Convert.ToString(dt.Rows[i]["RateForOthers"]);
                                ddlBillingHeaderRefinance.Value = Convert.ToString(dt.Rows[i]["ConditionalColumn"]);
                            }
                            else if (Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "Current Owner")
                            {
                                ddlIsApplicableCurrentOwner.Value = "Yes";
                                ddlChargeTypeCurrentOwner.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPriceCurrentOwner.Value = Convert.ToString(dt.Rows[i]["RateForOthers"]);
                                ddlBillingHeaderCurrentOwner.Value = Convert.ToString(dt.Rows[i]["ConditionalColumn"]);
                            }
                            else if (Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "Full Search")
                            {
                                ddlIsApplicableFullSearch.Value = "Yes";
                                ddlChargeTypeFullSearch.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPriceFullSearch.Value = Convert.ToString(dt.Rows[i]["RateForOthers"]);
                                ddlBillingHeaderFullSearch.Value = Convert.ToString(dt.Rows[i]["ConditionalColumn"]);
                            }
                            else if (Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "Two Owner")
                            {
                                ddlIsApplicableTwoOwner.Value = "Yes";
                                ddlChargeTypeTwoOwner.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPriceTwoOwner.Value = Convert.ToString(dt.Rows[i]["RateForOthers"]);
                                ddlBillingHeaderTwoOwner.Value = Convert.ToString(dt.Rows[i]["ConditionalColumn"]);
                            }
                            else if (Convert.ToString(dt.Rows[i]["ConditionalValue"]) == "" && Convert.ToString(dt.Rows[i]["ProjectColumn"]) == "# of Character")
                            {
                                ddlIsApplicableCharacter.Value = "Yes";
                                ddlChargeTypeCharacter.Value = Convert.ToString(dt.Rows[i]["Updatelevel"]);
                                txtPriceCharacter.Value = Convert.ToString(dt.Rows[i]["RateForOthers"]);
                                ddlBillingHeaderCharacterNew.Value = Convert.ToString(dt.Rows[i]["ConditionalColumn"]);
                            }
                        }
                      
                    }
                    else if (BillingBase == "Order Type")
                    {
                        ddlBillingBase.SelectedIndex = 3;

                        trBaseRate.Style.Add("display", "none");
                        trProductType.Style.Add("display", "none");
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
                        if (foundRows1.Length > 0)
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
                    else if (BillingBase == "Character Based")
                    {
                        ddlBillingBase.SelectedIndex = 4;

                        trBaseRate.Style.Add("display", "none");
                        trProductType.Style.Add("display", "none");
                        trOrderType.Style.Add("display", "none");
                        trCharacter.Style.Add("display", "");
                        string find = "ProjectColumn ='# of Character'";
                        DataRow[] foundRows = dt.Select(find);
                        if (foundRows.Length > 0)
                        {
                            txtPerCharacter.Value = Convert.ToString(foundRows[0]["RateForOthers"]);
                            txtPricePerCharacter.Value = Convert.ToString(foundRows[0]["Rate"]);
                            ddlBillingHeaderCharacter.Value = Convert.ToString(foundRows[0]["ProjectColumn"]);
                        }
                    }
                    else if (BillingBase == "Page Based")
                    {
                        #region Character Based
                        string find = "ProjectColumn in ('No of Pages') and AdditionalConditions=0 and Updatelevel='Fix Amount'";
                        DataRow[] foundRows = dt.Select(find);
                        ddlBillingBase.SelectedIndex = 5;

                        trBaseRate.Style.Add("display", "none");
                        trProductType.Style.Add("display", "none");
                        trOrderType.Style.Add("display", "none");
                        trCharacter.Style.Add("display", "none");
                        tbl379003.Style.Add("display", "");
                        if (foundRows.Length > 0)
                        {
                            txtPrice771ScanPages.Value = Convert.ToString(dt.Rows[0]["RateForOthers"]);
                            ddlChargeType771ScanPages.Value = Convert.ToString(dt.Rows[0]["Updatelevel"]);
                            ddlBillingHeader771ScanPages.Value = Convert.ToString(dt.Rows[0]["ProjectColumn"]);
                        }
                        #endregion
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

        public DataTable GetCostingDetailsTyping(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetTypingProjectCosting");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        #endregion

        public int InsertCostingWithParameters_Typing(Hashtable htParam)
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
            int result = ApproveBillParameter_Typing(htParam);
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

            BindTypingCosting(Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
        }

        public int ApproveBillParameter_Typing(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_ApproveBillingParametersForCM_Typing]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["ProjectId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
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
                SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_ApproveBillingParametersForCM_Typing");
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
