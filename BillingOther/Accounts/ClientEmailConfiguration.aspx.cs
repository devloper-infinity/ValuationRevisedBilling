using DevExpress.Web;
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
using System.Collections;
using BillingOther.App_Code.BLL;

namespace BillingOther.Accounts
{
    public partial class ClientEmailConfiguration : System.Web.UI.Page
    {
        bllTracking blltracking = new bllTracking();
        SendMail sendEmail = new SendMail();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Home  >  Client Email Configuration";
            if (!IsPostBack)
            {
                Session["dtReport"] = null;
                BindProjects();
                bindGridBilling();
                if (Convert.ToString(Request.Url).Contains("Edit"))
                {
                    getEmailConfigu_Id();
                   // aBack.HRef = "~/ClientEmailConfiguration.aspx";
                    btnSend.Text = "Update";
                }
            }
            bindGridBilling();

        }
        public void BindProjects()
        {
            ddlProjects.DataSource = blltracking.GetAllProjectByUserRights();
            ddlProjects.DataTextField = "ProjectName";
            ddlProjects.DataValueField = "ProjectId";
            ddlProjects.DataBind();
            ddlProjects.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));
        }
        public void BindClients(int ProjID)
        {
            DataTable dt;

            dt = blltracking.GetAllClientDetailsProjectWise(ProjID);
            ddlClientList.DataSource = dt;
            ddlClientList.DataValueField = "ClientId";
            ddlClientList.DataTextField = "ClientName";
            ddlClientList.DataBind();

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (btnSend.Text == "Save")
            {
                Hashtable htparam = new Hashtable();
                htparam.Add("ClientId", Convert.ToInt32(ddlClientList.SelectedValue));
                htparam.Add("ProjectID", Convert.ToString(ddlProjects.SelectedValue));
                htparam.Add("CEC_To", Convert.ToString(txtTo.Text));
                htparam.Add("CEC_CC", Convert.ToString(txtCC.Text.Trim()));
                htparam.Add("CEC_BCC", Convert.ToString(txtBCC.Text.Trim()));

                htparam.Add("Added_By", Convert.ToInt32(HttpContext.Current.User.Identity.Name));

                int result = blltracking.InsertEmailConfiguration(htparam);

                bindGridBilling();
                if (result > 0)
                {

                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = " Details added successfully.";                 
                    btnSend.Text = "Save";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                   
                    bindGridBilling();
                    clear1();

                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-warning background-warning");
                    dvError.InnerHtml = " Details already exists.";                    
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                    
                }
            }
            else
            {
                Hashtable htparam = new Hashtable();
                htparam.Add("ClientId", Convert.ToInt32(ddlClientList.SelectedValue));
                htparam.Add("ProjectID", Convert.ToString(ddlProjects.SelectedValue));
                htparam.Add("CEC_To", Convert.ToString(txtTo.Text));
                htparam.Add("CEC_CC", Convert.ToString(txtCC.Text.Trim()));
                htparam.Add("CEC_BCC", Convert.ToString(txtBCC.Text.Trim()));
                //htparam.Add("CEC_Body", Convert.ToString(txtbo.Text.Trim()));
                htparam.Add("Added_By", Convert.ToInt32(HttpContext.Current.User.Identity.Name));
                htparam.Add("CEC_Id", Convert.ToString(Request.QueryString["Edit"]));
                int ReturnValue = blltracking.UpdateEmailConfiguration(htparam);
                if (ReturnValue > 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = " Details updated successfully.";                    
                    btnSend.Text = "Save";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                    
                    bindGridBilling();
                    clear1();
                    Response.Redirect("~/Accounts/ClientEmailConfiguration.aspx");
                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-warning background-warning");
                    dvError.InnerHtml = " Details already exists.";                  
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                    
                }
                bindGridBilling();
                clear1();
            }
        }

        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindClients(int.Parse(ddlProjects.SelectedValue));
        }

        public void bindGridBilling()
        {
            DataTable dt = blltracking.GetAllEmailConfiguration();
            rptEmailConfigurations.DataSource = dt;
            rptEmailConfigurations.DataBind();
        }
        public void getEmailConfigu_Id()
        {
            int CCE_Id = Convert.ToInt32(Request.QueryString["Edit"]);
            DataTable dtpaids = blltracking.GetEmailConfigurationByID(CCE_Id);
            if (dtpaids.Rows.Count > 0)
            {
                ddlProjects.SelectedValue = Convert.ToString(dtpaids.Rows[0]["ProjectId"]);
                BindClients(int.Parse(ddlProjects.SelectedValue));
                ddlClientList.Text = Convert.ToString(dtpaids.Rows[0]["CEC_ClientId"]);
                txtTo.Text = Convert.ToString(dtpaids.Rows[0]["CEC_To"]);
                txtCC.Text = Convert.ToString(dtpaids.Rows[0]["CEC_CC"]);
                btnSend.Text = "Update";
                txtBCC.Text = Convert.ToString(dtpaids.Rows[0]["CEC_BCC"]);
            }

        }
        public void clear1()
        {
            txtTo.Text = "";
            txtCC.Text = "";
            txtBCC.Text = "";
            ddlProjects.SelectedIndex = 0;
        }
    }
}
