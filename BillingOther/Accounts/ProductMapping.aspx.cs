using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace BillingOther.Accounts
{
    public partial class ProductMapping : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Product Mapping Master";

            if (!IsPostBack)
            {
                BindProject();
            }
        }

        public void BindProject()
        {
            DataTable dt = new bllTracking().GetAllProjectByDomainWise(4, int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            ddlProject.DataSource = dt;
            ddlProject.DataTextField = "ProjectName";
            ddlProject.DataValueField = "ProjectID";
            ddlProject.DataBind();
            ddlProject.Items.Insert(0, new ListItem("Select"));
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string Products = "";
            foreach (ListItem item in ddlProductERP.Items)
            {
                if (item.Selected)
                {
                    Products += item.Value + ",";
                }
            }
            try
            {
                Products = Products.Substring(0, Products.LastIndexOf(','));
            }
            catch { }
            if (Products != "")
            {
                Hashtable htParam = new Hashtable();
                htParam.Add("ProjectID", Convert.ToInt32(ddlProject.SelectedValue));
                htParam.Add("ERPProductTypes", Products);
                htParam.Add("BillingProduct", Convert.ToString(ddlProductBilling.SelectedValue));
                htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                int ReturnValue = InsertProductMapping(htParam);
                if (ReturnValue > 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Product types mapped successfully.";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    ddlProject.SelectedIndex = 0;
                    ddlProductBilling.SelectedIndex = 0;
                    ddlProductERP.Items.Clear();
                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error occured while mapping product types.";                   
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }

        }
        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProject.SelectedIndex > 0)
            {
                DataTable dt = GetAllProductProjectWiseByID(Convert.ToInt32(ddlProject.SelectedValue));
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ddlProductERP.Items.Insert(i, new ListItem(Convert.ToString(dt.Rows[i]["ProductType"]), Convert.ToString(dt.Rows[i]["IPM_Id"])));
                        }
                    }
                }
                ddlProductERP.ClearSelection();
                dt = GetProductsMapped(Convert.ToInt32(ddlProject.SelectedValue), Convert.ToString(ddlProductBilling.SelectedValue));
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        // ddlProductBilling.SelectedValue = Convert.ToString(dt.Rows[0]["BillingProduct"]);
                        string ProductList = Convert.ToString(dt.Rows[0]["ERPProductTypes"]);
                        string[] Products = ProductList.Split(',');
                        foreach (string prd in Products)
                        {
                            try
                            {
                                ddlProductERP.Items.FindByValue(prd.Trim()).Selected = true;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        #region Database
        public DataTable GetAllProductProjectWiseByID(int Product_Id)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetProductByProjectID");
            SQLHelper.AddParamToSQLCmd(cmd, "@Product_Id", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, Product_Id);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetProductsMapped(int ProjectId, string BillingProduct)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetProductMapped");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingProduct", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, BillingProduct);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public int InsertProductMapping(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_InsertProductTypeMapping");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.Int, 10, System.Data.ParameterDirection.Input, htParam["ProjectID"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ERPProductTypes", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["ERPProductTypes"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingProduct", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, htParam["BillingProduct"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        #endregion
    }
}
