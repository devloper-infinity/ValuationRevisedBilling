using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections;
using System.Net;
using System.Data;
using System.Drawing;
using BillingOther.App_Code.BLL;

namespace BillingOther.Accounts
{
    public partial class ProductMaster : System.Web.UI.Page
    {
        bllTracking blltracking = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Product Master";
            if (!IsPostBack)
            {
                BindProjects();
                bindGridBilling();
                if (Convert.ToString(Request.Url).Contains("Edit"))
                {
                    getbyproduct_Id();
                    //aBack.HRef = "~/ProductMaster.aspx";
                    btnSave.Text = "Update";
                }
            }
            bindGridBilling();
        }

        public void bindGridBilling()
        {
            DataTable dt = blltracking.GetAllProductProjectWise();
            rptProducts.DataSource = dt;
            rptProducts.DataBind();
        }
        public void getbyproduct_Id()
        {
            int Client_Id = Convert.ToInt32(Request.QueryString["Edit"]);
            DataTable dtpaids = blltracking.GetAllProductProjectWiseByID(Client_Id);
            if (dtpaids.Rows.Count > 0)
            {
                ddlProjects.SelectedValue = Convert.ToString(dtpaids.Rows[0]["IPM_ProjectID"]);
                txtProductType.Text = Convert.ToString(dtpaids.Rows[0]["ProductType"]);

                btnSave.Text = "Update";


            }

        }
        public void BindProjects()
        {
            ddlProjects.DataSource = blltracking.GetAllProjectByUserRights();
            ddlProjects.DataTextField = "ProjectName";
            ddlProjects.DataValueField = "ProjectId";
            ddlProjects.DataBind();
            ddlProjects.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));
        }
        public void clear1()
        {
            ddlProjects.SelectedIndex = 0;
            txtProductType.Text = "";

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "Save")
            {

                Hashtable Htparam = new Hashtable();
                Htparam.Add("ProjectID", Convert.ToString(ddlProjects.SelectedValue));
                Htparam.Add("Productname", Convert.ToString(txtProductType.Text));

                Htparam.Add("Added_By", Convert.ToInt32(HttpContext.Current.User.Identity.Name));

                int ReturnValue = blltracking.InsertProduct(Htparam);
                bindGridBilling();
                clear1();
                if (ReturnValue > 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Product Information Inserted successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    //txtProjectname.Focus();
                    bindGridBilling();
                    clear1();
                }
                else if (ReturnValue == 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error occured while inserting product information!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
                //else (ReturnValue != -1)
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Project information already exist!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }
            else
            {
                Hashtable Htparam = new Hashtable();
                Htparam.Add("ProjectID", Convert.ToString(ddlProjects.SelectedValue));
                Htparam.Add("Productname", Convert.ToString(txtProductType.Text));
                Htparam.Add("IPM_Id", Convert.ToString(Request.QueryString["Edit"]));
                Htparam.Add("Added_By", Convert.ToInt32(HttpContext.Current.User.Identity.Name));

                int ReturnValue = blltracking.UpdateProduct(Htparam);
                if (ReturnValue > 0)
                {

                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Details Updated Successfully.";                  
                    btnSave.Text = "Save";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                   
                    bindGridBilling();
                    clear1();
                    Response.Redirect("~/Accounts/ProductMaster.aspx");//BindProjectsTabFirst();
                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Details already exists.";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
                bindGridBilling();
                clear1();

            }
        }
    }
}
