using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;

namespace BillingOther.Accounts
{
    public partial class ProjectDetails : System.Web.UI.Page
    {
        bllTracking bllMaster = new bllTracking();
        List<Record> list = new List<Record>();
        List<RecordCosting> listCosting = new List<RecordCosting>();
        protected void Page_Load(object sender, EventArgs e)
        {
            int ProjectApproval_Id = Convert.ToInt32(Request.QueryString["ERPProjectId"]);
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Cost Master";
            string ss = Convert.ToString(Session["formdate"]);

            if (Convert.ToString(Session["formdate"]) != "Select" && Convert.ToString(Session["formdate"]) != "" && Convert.ToInt32(Session["formdate"]) > 0)
            {
                BindParamsGrid(Convert.ToInt32(Session["formdate"]));
            }
            else
            {
                BindParamsGrid(4);
            }

            if (Convert.ToString(ddldomain.Value) != "Select" && Convert.ToString(ddldomain.Value) != "" && Convert.ToInt32(ddldomain.Value) > 0)
            {
                BindParamsGrid(Convert.ToInt32(ddldomain.Value));
            }
            else
            {
                BindParamsGrid(4);
            }
            if (!IsPostBack)
            {
                Session["ProjectId"] = "0";
                Session["formdate"] = "0";
                BindDomain();
                bindBDM();
                BindProjects();

                if (Convert.ToString(Request.Url).Contains("ERPProjectId"))
                {
                    dvSummary.Style.Add("display", "");
                    bindgrid();
                    GetAllInformationByProjectAprovalID();
                    GetInformationSalesonEdit();
                    btnsbmit.InnerText = "Update";
                    BindFTECosting(Convert.ToInt32(Request.QueryString["ERPProjectId"]));
                    BindMigratedParameterTables();
                }
                else
                {
                    //Tab1.Style.Add("margin-top", "-70px");
                }
                if (Convert.ToString(Request.QueryString["ERPProjectId"]) != "" && Convert.ToString(Request.QueryString["ERPProjectId"]) != null)
                {
                    rdButtons.Style.Add("display", "none");
                    GetAllInformationSales();
                }
                else
                {
                    rdButtons.Style.Add("display", "");
                }
            }
            if (Convert.ToString(Request.QueryString["ERPProjectId"]) == "426" || Convert.ToString(ddldomain.Value) == "36" || Convert.ToString(ddldomain.Value) == "34" || Convert.ToString(ddldomain.Value) == "7" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "87" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "435" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "440" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "40" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "414" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "50" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "51" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "391" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "184" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "205" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "203" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "400" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "205" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "373" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "353" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "123" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "354" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "203" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "392" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "386" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "280" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "337" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "388" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "373" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "385" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "393" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "39" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "395" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "352" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "155")
            {
                if (Convert.ToString(Request.QueryString["ERPProjectId"]) == "353" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "155" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "440" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "435" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "123" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "354" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "392" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "386" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "280" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "393" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "434" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "442")
                {
                    tdRate.InnerHtml = "<b>Rate Per FTE : </b>";
                }
                else if (Convert.ToString(Request.QueryString["ERPProjectId"]) == "184" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "205" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "203" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "400" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "205" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "373" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "373" || Convert.ToString(Request.QueryString["ERPProjectId"]) == "385")
                {
                    tdRate.InnerHtml = "<b>Hourly Rate : </b>";
                }
                else
                {
                    tdRate.InnerHtml = "<b>Base Rate : </b>";
                }
                this.tbpnl3.Visible = false;
                this.tbpnlFTE.Visible = true;
                this.TabPanel1.Visible = false;
                if (Convert.ToString(Request.QueryString["ERPProjectId"]) == "39")
                {
                    tblOtherFTE.Style.Add("display", "none");
                    tbl861007.Style.Add("display", "");
                    tblWholeloan.Style.Add("display", "none");
                }
                else
                {
                    tblOtherFTE.Style.Add("display", "");
                    tbl861007.Style.Add("display", "none");
                    tblWholeloan.Style.Add("display", "none");
                }
            }
            else if (Convert.ToString(Request.QueryString["ERPProjectId"]) == "620")
            {
                this.tbpnl3.Visible = false;
                this.tbpnlFTE.Visible = true;
                this.TabPanel1.Visible = false;
                tblOtherFTE.Style.Add("display", "none");
                tbl861007.Style.Add("display", "none");
                tblWholeloan.Style.Add("display", "");
                RenderWholeLoan(GetCostingDetailsWHoleLoan());
            }
            else
            {
                this.tbpnl3.Visible = true;
                this.tbpnlFTE.Visible = false;
                this.TabPanel1.Visible = true;
            }

            //  this.tbpnl34.Visible = false;





        }

        public void BindFTECosting(int ProjectID)
        {
            if (ProjectID == 39)
            {
                DataTable dt = GetCostingDetails861(ProjectID);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        txtfirst1200Rate.Value = Convert.ToString(dt.Rows[0]["Rate"]);
                        txtAdditionalBallotsRate.Value = Convert.ToString(dt.Rows[0]["RateForOthers"]);
                        txtWireTransferCharges.Value = Convert.ToString(dt.Rows[0]["ConditionalValue"]);
                    }
                }
            }
            else
            {
                DataTable dt = GetCostingDetailsFTE(ProjectID);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        txtHourlyRate.Value = Convert.ToString(dt.Rows[0]["Rate"]);
                    }
                }
            }
        }

        public DataTable GetallbillingParameters(int DID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_getallbillingParameters");
            SQLHelper.AddParamToSQLCmd(cmd, "@DomainId", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, DID);
            //SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, PID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public void BindParamsGrid(int DomainID)
        {
        }

        public void bindgrid()
        {
            DataTable dt = bllMaster.BindVerifyProjectById(int.Parse(Request.QueryString["ERPProjectId"]));
            RenderProjectSummary(dt);
        }

        #region Freight



        #endregion

        #region BindMethods
        public void BindProjects()
        {

            ddlprojNo.DataSource = bllMaster.GetAllProjectByUserRights();
            ddlprojNo.DataTextField = "ProjectName";
            ddlprojNo.DataValueField = "ProjectName";
            ddlprojNo.DataBind();
            ddlprojNo.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));

        }
        public void bindBDM()
        {
            DataTable dtbdm = new DataTable();
            dtbdm = bllMaster.getallMarketingEmployee();
            ddlBDM.DataSource = dtbdm;
            ddlBDM.DataValueField = "EmployeeID";
            ddlBDM.DataTextField = "Code";
            ddlBDM.DataBind();
            ddlBDM.Items.Insert(0, new ListItem("Select"));
        }
        public void BindDomain()
        {
            DataTable dtdomain = new DataTable();
            dtdomain = bllMaster.getalldomains(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            ddldomain.DataSource = dtdomain;
            ddldomain.DataValueField = "DomainID";
            ddldomain.DataTextField = "DomainName";
            ddldomain.DataBind();
            ddldomain.Items.Insert(0, new ListItem("Select"));
        }
        #endregion

        #region ClientInfo
        protected void btnsbmit_Click(object sender, EventArgs e)
        {
            int ProjectApproval_Id = Convert.ToInt32(Request.QueryString["ERPProjectId"]);
            bool NewProject = rdcardYes.Checked;
            if (btnsbmit.InnerText == "Submit")
            {
                Hashtable InsertprojectApproval = new Hashtable();
                InsertprojectApproval["Domain_Id"] = ddldomain.Value;
                InsertprojectApproval["Company_Id"] = "0";//txtcompnayId.Text;
                if (NewProject == true)
                {
                    InsertprojectApproval["ProjectName"] = txtProjectname.Value;
                    InsertprojectApproval["Type"] = NewProject;
                }
                else
                {
                    InsertprojectApproval["ProjectName"] = ddlProjects.Items[ddlProjects.SelectedIndex].Text;
                    InsertprojectApproval["ProcessName"] = txtProcess.Value;
                    InsertprojectApproval["Type"] = NewProject;
                }
                InsertprojectApproval["Company_Name"] = txtcompanyname.Value;
                InsertprojectApproval["Contact_Person"] = txtContactPerson.Value;// ddlContactperson.SelectedItem.Text;
                InsertprojectApproval["Phonenumber"] = txtphonenumber.Value;// ddlphonenumber.SelectedItem.Text;
                InsertprojectApproval["Email-Id"] = txtemail.Value;// ddlEmails.SelectedItem.Text;
                InsertprojectApproval["Url"] = txturl.Value;
                InsertprojectApproval["Address"] = txtaddress.Value;
                InsertprojectApproval["Result"] = txtresult.Value;
                InsertprojectApproval["AddedBy"] = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
                int ReturnValue = bllMaster.InsertProjectApprovalRequest(InsertprojectApproval);
                if (ReturnValue > 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Project information added successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    txtProjectname.Focus();
                    // BindProjectInformationGrid();
                    hdndomain.Value = ddldomain.Value;

                    if (ddldomain.Value == "19")
                        Response.Redirect("~/ProjectDetailsTyping.aspx?ERPProjectId=" + ReturnValue + "&DomainID=" + ddldomain.Value);
                    else if (ddldomain.Value == "2")
                        Response.Redirect("~/ProjectDetailsFreight.aspx?ERPProjectId=" + ReturnValue + "&DomainID=" + ddldomain.Value);
                    else
                        Response.Redirect("~/ProjectDetails.aspx?ERPProjectId=" + ReturnValue + "&DomainID=" + ddldomain.Value);
                    //clear();
                }
                else if (ReturnValue != -1)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-warning background-warning");
                    dvError.InnerHtml = "Project information already exist!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
                else if (ReturnValue == 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error In Project Information Inserting!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }
            else
            {
                Hashtable InsertprojectApproval = new Hashtable();
                InsertprojectApproval["ProjectApproval_Id"] = ProjectApproval_Id;
                InsertprojectApproval["Company_Id"] = "0";
                InsertprojectApproval["ProjectName"] = txtProjectname.Value;
                InsertprojectApproval["Company_Name"] = txtcompanyname.Value;
                InsertprojectApproval["Contact_Person"] = txtContactPerson.Value;
                InsertprojectApproval["Phonenumber"] = txtphonenumber.Value;
                InsertprojectApproval["Email-Id"] = txtemail.Value;
                InsertprojectApproval["Url"] = txturl.Value;
                InsertprojectApproval["Address"] = txtaddress.Value;
                InsertprojectApproval["Result"] = txtresult.Value;
                InsertprojectApproval["UpdatedBY"] = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
                InsertprojectApproval["ERPProjectId"] = Convert.ToInt32(Request.QueryString["ERPProjectId"]);
                int ReturnValue = bllMaster.UpdateProjectApprovalRequest(InsertprojectApproval);
                if (ReturnValue == 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Project Information Updated successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    txtProjectname.Focus();
                }
            }
        }

        public void GetAllInformationByProjectAprovalID()
        {
            int ApprovalId = Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"]));

            DataTable dtpaid = bllMaster.GetbyProjectApproval_IdReq(ApprovalId);
            if (dtpaid.Rows.Count > 0)
            {
                ddldomain.Value = Convert.ToString(dtpaid.Rows[0]["PAI_DomainId"]);
                txtProjectname.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]);
                if (Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]) != "")
                {
                    ddlProjects.DataSource = bllMaster.GetAllProjectByUserRights();
                    ddlProjects.DataTextField = "ProjectName";
                    ddlProjects.DataValueField = "ProjectName";
                    ddlProjects.DataBind();
                    ddlProjects.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));
                    trnewprocess.Style.Add("display", "");
                    ddlProjects.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]) + "-" + Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]);
                    tdprojtxt.Style.Add("display", "none");
                    txtProjectname.Style.Add("display", "none");
                    txtProcess.Value = Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]);
                    ddlprojNo.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]) + "-" + Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]);
                }
                else
                {
                    trnewprocess.Style.Add("display", "none");
                    tdprojtxt.Style.Add("display", "");
                    txtProjectname.Style.Add("display", "");
                }
                ///Showing Grid based on Domain
                if (ddldomain.Value == "9") //UW
                {
                    this.tbpnl3.Visible = false;
                }
                else if (ddldomain.Value == "4") //QC
                {
                    this.tbpnl3.Visible = true;
                }
                else if (ddldomain.Value == "19") //Commitment
                {
                    this.tbpnl3.Visible = true;
                }
                else
                {
                    ///Code for Freight 
                }

                txtcompanyname.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Company_Name"]);
                txtContactPerson.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Contact_Person"]);
                txtphonenumber.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Phone_Number"]);
                txtemail.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Email_Id"]);
                txturl.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Url"]);
                txtaddress.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Address"]);
                txtresult.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Remark"]);
            }
        }
        public void BindProjectInformationGrid()
        {
            DataTable dt = bllMaster.ViewAllProjectApp();
            RenderProjectRequests(dt);
            //clear();
        }
        public void clear()
        {
            txtContactPerson.Value = String.Empty;
            txtemail.Value = string.Empty;
            txtphonenumber.Value = string.Empty;
            txtProjectname.Value = " ";
            txtcompanyname.Value = " ";
            txturl.Value = " ";
            txtaddress.Value = " ";
            txtresult.Value = " ";

        }

        public DataTable GetProjectApprovalInformation(int ApprovalId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_Billing_GetProjectApprovalInformation");
            SQLHelper.AddParamToSQLCmd(cmd, "@PAI_Id", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ApprovalId);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        #endregion

        #region Sales
        public void GetAllInformationSales()
        {
            int ApprovalId = Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"]));
            try
            {
                if (txtProcess.Value != "")
                {
                    ddlprojNo.Value = txtProjectname.Value + "-" + txtProcess.Value;
                }
                else
                {
                    ddlprojNo.Value = Convert.ToString(txtProjectname.Value);
                }

            }
            catch { }
            DataTable dtpaid = GetProjectApprovalInformation(ApprovalId);
            if (dtpaid != null)
            {
                if (dtpaid.Rows.Count > 0)
                {

                    txtRateRevisionDate.Value = Convert.ToString(dtpaid.Rows[0]["RateRevisionDate"]);
                    ddlprojNo.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ProjectNo"]);
                    txtProcessNameSales.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ProcessName"]);
                    ddlBDM.Value = Convert.ToString(dtpaid.Rows[0]["IBS_BDM"]);
                    if (Convert.ToString(dtpaid.Rows[0]["IBS_RequestedDate"]) != "")
                    {
                        txtRequestedDate.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_RequestedDate"]).ToString("dd-MMM-yyyy");
                    }
                    txtProjectScope.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ScopeOfProject"]);
                    ddnda.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDASigned"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_NDASigned"]) == "True" ? "1" : "0");
                    if (ddnda.Value == "1")
                    {
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_DateOfNDAAgreement"]) != "")
                        {
                            txtDateOfAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_DateOfNDAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_ExpirationDateofNDAAgreement"]) != "")
                        {
                            txtExpirationDateOfAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_ExpirationDateofNDAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        ddlNDASignedByClient.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedByClient"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedByClient"]) == "True" ? "1" : "0");
                        ddlNDASignedByInfinity.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedBYInfinity"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedByInfinity"]) == "True" ? "1" : "0");
                    }
                    ddsla.Value = Convert.ToString(dtpaid.Rows[0]["IBS_SLASigned"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_SLASigned"]) == "True" ? "1" : "0");

                    if (ddsla.Value == "1")
                    {
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_DateOfSLAAgreement"]) != "")
                        {
                            txtDateOfSLAAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_DateOfSLAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_ExpirationDateofSLAAgreement"]) != "")
                        {
                            txtExpirationDateOfSLAAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_ExpirationDateofSLAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        ddlSLASignedByClient.Value = Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByClient"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByClient"]) == "True" ? "1" : "0");
                        ddlSLASignedByInfinity.Value = Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByInfinity"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByInfinity"]) == "True" ? "1" : "0");
                    }

                    ddlProjectStatus.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ProjectStatus"]);
                    if (Convert.ToString(dtpaid.Rows[0]["IBS_ProjectStatus"]) == "Live Stopped")
                    {
                        trlivestopped.Style.Add("display", "");
                        txtStoppedDate.Value = Convert.ToString(dtpaid.Rows[0]["IBS_StoppedDate"]);
                        txtStoppedRemark.Value = Convert.ToString(dtpaid.Rows[0]["IBS_StoppedRemark"]);
                    }
                    else
                    {
                        trlivestopped.Style.Add("display", "none");
                    }
                    txtExpectedVolume.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ExpectedVolume"]);
                    if (Convert.ToString(dtpaid.Rows[0]["IBS_ExpectedStartDate"]) != "")
                        txtExpectedStartDate.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_ExpectedStartDate"]).ToString("dd-MMM-yyyy");
                    ddlProjectDuration.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ProjectDuration"]);
                    txtSalesRemark.Value = Convert.ToString(dtpaid.Rows[0]["IBS_Remark"]);
                    hdnSLAPath.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDAFilePath"]);
                    hdnMSAPath.Value = Convert.ToString(dtpaid.Rows[0]["IBS_MSAFilePath"]);
                }
            }
        }
        protected void btnSalesInformation_Click(object sender, EventArgs e)
        {
            if (btnSalesInformation.InnerText == "Submit")
            {
                Hashtable htParam = new Hashtable();
                htParam.Add("ProjectApproval_Id", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                htParam.Add("ProcessName", txtProcessNameSales.Value);
                htParam.Add("ProjectName", ddlprojNo.Items[ddlprojNo.SelectedIndex].Text);
                htParam.Add("BDM", Convert.ToInt32(ddlBDM.Value));
                if (txtRequestedDate.Value != "")
                {
                    htParam.Add("RequestedDate", txtRequestedDate.Value);
                }
                else
                {
                    htParam.Add("RequestedDate", DateTime.Now.ToString("dd-MMM-yyyy"));
                }
                htParam.Add("ScopeOfProject", txtProjectScope.Value);
                bool nda = ddnda.Value == "1" ? true : false;
                if (ddnda.SelectedIndex == 0)
                    htParam.Add("NDASigned", null);
                else if (ddnda.SelectedIndex == 1)
                {
                    htParam.Add("NDASigned", ddnda.Value);
                    if (txtDateOfAgreement.Value != "")
                    {
                        htParam.Add("DateOfNDAAgreement", txtDateOfAgreement.Value);
                    }
                    if (txtExpirationDateOfAgreement.Value != "")
                    {
                        htParam.Add("ExpirationDateofNDAAgreement", txtExpirationDateOfAgreement.Value);
                    }
                    htParam.Add("NDASignedByClient", ddlNDASignedByClient.Value == "1" ? true : false);
                    htParam.Add("NDASignedBYInfinity", ddlNDASignedByInfinity.Value == "1" ? true : false);
                }
                else
                {
                    htParam.Add("NDASigned", false);
                    //  htParam.Add("DateOfNDAAgreement", "");
                    //  htParam.Add("ExpirationDateofNDAAgreement", "");
                    htParam.Add("NDASignedByClient", null);
                    htParam.Add("NDASignedBYInfinity", null);
                }
                if (ddsla.SelectedIndex == 0)
                    htParam.Add("SLASigned", null);
                else if (ddsla.SelectedIndex == 1)
                {
                    htParam.Add("SLASigned", ddsla.Value);
                    if (txtDateOfSLAAgreement.Value != "")
                    {
                        htParam.Add("DateOfSLAAgreement", txtDateOfSLAAgreement.Value);
                    }
                    if (txtExpirationDateOfSLAAgreement.Value != "")
                    {
                        htParam.Add("ExpirationDateofSLAAgreement", txtExpirationDateOfSLAAgreement.Value);
                    }
                    htParam.Add("SLASignedByClient", ddlSLASignedByClient.Value == "1" ? true : false);
                    htParam.Add("SLASignedByInfinity", ddlSLASignedByInfinity.Value == "1" ? true : false);
                }
                else
                {
                    htParam.Add("SLASigned", false);
                    // htParam.Add("DateOfSLAAgreement", "");
                    // htParam.Add("ExpirationDateofSLAAgreement", "");
                    htParam.Add("SLASignedByClient", null);
                    htParam.Add("SLASignedByInfinity", null);
                }
                htParam.Add("ProjectStatus", ddlProjectStatus.Value);
                if (ddlProjectStatus.Value == "Live Stopped")
                {
                    htParam.Add("StoppedDate", txtStoppedDate.Value.Trim());
                    htParam.Add("StoppedRemark", txtStoppedRemark.Value.Trim());

                }

                htParam.Add("ExpectedVolume", txtExpectedVolume.Value);
                if (txtExpectedStartDate.Value != "")
                {
                    htParam.Add("ExpectedStartDate", txtExpectedStartDate.Value);
                }
                htParam.Add("ProjectDuration", ddlProjectDuration.Value);
                htParam.Add("Remark", txtSalesRemark.Value);
                htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                htParam.Add("ERPProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                string projName = ddlprojNo.Items[ddlprojNo.SelectedIndex].Text;
                if (!Directory.Exists(Server.MapPath(@"~/ProjectDocuments/" + projName + "/NDA")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/ProjectDocuments/" + projName + "/NDA"));
                }
                try
                {
                    if ((fpNDA.PostedFile != null && fpNDA.PostedFile.ContentLength > 0))
                    {
                        string fileName = "";
                        fileName = Path.GetFileName(fpNDA.PostedFile.FileName);// + DateTime.Now.ToString("ddMMyyyyhhmmss");
                        fileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + fileName;
                        fpNDA.PostedFile.SaveAs(Server.MapPath(@"~/ProjectDocuments/" + projName + "/NDA" + "/" + fileName));
                        htParam["LogoPathNDA"] = Convert.ToString(@"~/ProjectDocuments/" + projName + "/NDA" + "/" + fileName);
                    }
                    else
                    {
                        htParam["LogoPathNDA"] = "";
                    }
                }
                catch { }

                if (!Directory.Exists(Server.MapPath(@"~/ProjectDocuments/" + projName + "/MSA")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/ProjectDocuments/" + projName + "/MSA"));
                }
                try
                {
                    if ((fpSLA.PostedFile != null && fpSLA.PostedFile.ContentLength > 0))
                    {
                        string fileName = "";
                        fileName = Path.GetFileName(fpSLA.PostedFile.FileName);// + DateTime.Now.ToString("ddMMyyyyhhmmss");
                        fileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + fileName;
                        fpSLA.PostedFile.SaveAs(Server.MapPath(@"~/ProjectDocuments/" + projName + "/MSA" + "/" + fileName));
                        htParam["LogoPathMSA"] = Convert.ToString(@"~/ProjectDocuments/" + projName + "/MSA" + "/" + fileName);
                    }
                    else
                    {
                        htParam["LogoPathMSA"] = "";
                    }
                }
                catch { }
                htParam.Add("RateRevisionDate", txtRateRevisionDate.Value);
                int ReturnValue = bllMaster.InsertSalesInformation(htParam);
                if (ReturnValue > 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Sales Information Inserted successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    new SendMail().SendProjectCreationEmail(Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    bindgrid();
                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error occured while submitting sales information!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }
            else
            {
                Hashtable htParam = new Hashtable();
                htParam.Add("ProjectApproval_Id", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                htParam.Add("ProcessName", txtProcessNameSales.Value);
                htParam.Add("ProjectName", ddlprojNo.Items[ddlprojNo.SelectedIndex].Text);
                htParam.Add("BDM", Convert.ToInt32(ddlBDM.Value));
                if (txtRequestedDate.Value != "")
                {
                    htParam.Add("RequestedDate", txtRequestedDate.Value);
                }
                else
                {
                    htParam.Add("RequestedDate", DateTime.Now.ToString("dd-MMM-yyyy"));
                }
                htParam.Add("ScopeOfProject", txtProjectScope.Value);
                bool nda = ddnda.Value == "1" ? true : false;
                if (ddnda.SelectedIndex == 0)
                    htParam.Add("NDASigned", null);
                else if (ddnda.SelectedIndex == 1)
                {

                    htParam.Add("NDASigned", ddnda.Value);
                    if (txtDateOfAgreement.Value != "")
                    {
                        htParam.Add("DateOfNDAAgreement", txtDateOfAgreement.Value);
                    }
                    if (txtExpirationDateOfAgreement.Value != "")
                    {
                        htParam.Add("ExpirationDateofNDAAgreement", txtExpirationDateOfAgreement.Value);
                    }
                    htParam.Add("NDASignedByClient", ddlNDASignedByClient.Value == "1" ? true : false);
                    htParam.Add("NDASignedBYInfinity", ddlNDASignedByInfinity.Value == "1" ? true : false);
                }
                else
                {
                    htParam.Add("NDASigned", false);
                    //  htParam.Add("DateOfNDAAgreement", "");
                    //  htParam.Add("ExpirationDateofNDAAgreement", "");
                    htParam.Add("NDASignedByClient", null);
                    htParam.Add("NDASignedBYInfinity", null);
                }
                if (ddsla.SelectedIndex == 0)
                    htParam.Add("SLASigned", null);
                else if (ddsla.SelectedIndex == 1)
                {


                    htParam.Add("SLASigned", ddsla.Value);
                    if (txtDateOfSLAAgreement.Value != "")
                    {
                        htParam.Add("DateOfSLAAgreement", txtDateOfSLAAgreement.Value);
                    }
                    if (txtExpirationDateOfSLAAgreement.Value != "")
                    {
                        htParam.Add("ExpirationDateofSLAAgreement", txtExpirationDateOfSLAAgreement.Value);
                    }
                    htParam.Add("SLASignedByClient", ddlSLASignedByClient.Value == "1" ? true : false);
                    htParam.Add("SLASignedByInfinity", ddlSLASignedByInfinity.Value == "1" ? true : false);
                }
                else
                {
                    htParam.Add("SLASigned", false);
                    // htParam.Add("DateOfSLAAgreement", "");
                    // htParam.Add("ExpirationDateofSLAAgreement", "");
                    htParam.Add("SLASignedByClient", null);
                    htParam.Add("SLASignedByInfinity", null);
                }
                htParam.Add("ProjectStatus", ddlProjectStatus.Value);
                if (ddlProjectStatus.Value == "Live Stopped")
                {
                    htParam.Add("StoppedDate", txtStoppedDate.Value.Trim());
                    htParam.Add("StoppedRemark", txtStoppedRemark.Value.Trim());

                }
                htParam.Add("ExpectedVolume", txtExpectedVolume.Value);
                if (txtExpectedStartDate.Value != "")
                {
                    htParam.Add("ExpectedStartDate", txtExpectedStartDate.Value);
                }
                htParam.Add("ProjectDuration", ddlProjectDuration.Value);
                htParam.Add("Remark", txtSalesRemark.Value);
                htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                htParam.Add("ERPProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                string projName = ddlprojNo.Items[ddlprojNo.SelectedIndex].Text;
                if (!Directory.Exists(Server.MapPath(@"~/ProjectDocuments/" + projName + "/NDA")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/ProjectDocuments/" + projName + "/NDA"));
                }
                try
                {
                    if ((fpNDA.PostedFile != null && fpNDA.PostedFile.ContentLength > 0))
                    {
                        string fileName = "";
                        fileName = Path.GetFileName(fpNDA.PostedFile.FileName);// + DateTime.Now.ToString("ddMMyyyyhhmmss");
                        fileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + fileName;
                        fpNDA.PostedFile.SaveAs(Server.MapPath(@"~/ProjectDocuments/" + projName + "/NDA" + "/" + fileName));
                        htParam["LogoPathNDA"] = Convert.ToString(@"~/ProjectDocuments/" + projName + "/NDA" + "/" + fileName);
                    }
                    else
                    {
                        htParam["LogoPathNDA"] = "";
                    }
                }
                catch { }

                if (!Directory.Exists(Server.MapPath(@"~/ProjectDocuments/" + projName + "/MSA")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/ProjectDocuments/" + projName + "/MSA"));
                }
                try
                {
                    if ((fpSLA.PostedFile != null && fpSLA.PostedFile.ContentLength > 0))
                    {
                        string fileName = "";
                        fileName = Path.GetFileName(fpSLA.PostedFile.FileName);// + DateTime.Now.ToString("ddMMyyyyhhmmss");
                        fileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + fileName;
                        fpSLA.PostedFile.SaveAs(Server.MapPath(@"~/ProjectDocuments/" + projName + "/MSA" + "/" + fileName));
                        htParam["LogoPathMSA"] = Convert.ToString(@"~/ProjectDocuments/" + projName + "/MSA" + "/" + fileName);
                    }
                    else
                    {
                        htParam["LogoPathMSA"] = "";
                    }
                }
                catch { }
                htParam.Add("RateRevisionDate", txtRateRevisionDate.Value);
                int ReturnValue = bllMaster.UpdateSalesInformation(htParam);
                if (ReturnValue > 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Sales information updated successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    bindgrid();
                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error occured while submitting sales information!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }
        }
        public void GetInformationSalesonEdit()
        {
            int ApprovalId = Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"]));
            DataTable dtpaid = bllMaster.GetProjectApprovalInformation(ApprovalId);
            if (dtpaid != null)
            {
                if (dtpaid.Rows.Count > 0)
                {
                    try
                    {
                        txtRateRevisionDate.Value = Convert.ToString(dtpaid.Rows[0]["RateRevisionDate"]);
                    }
                    catch { }
                    ddlprojNo.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ProjectNo"]);
                    ddlBDM.Value = Convert.ToString(dtpaid.Rows[0]["IBS_BDM"]);
                    if (Convert.ToString(dtpaid.Rows[0]["IBS_RequestedDate"]) != "")
                    {
                        txtRequestedDate.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_RequestedDate"]).ToString("dd-MMM-yyyy");
                    }
                    txtProjectScope.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ScopeOfProject"]);
                    ddnda.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDASigned"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_NDASigned"]) == "True" ? "1" : "0");
                    if (ddnda.Value == "1")
                    {
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_DateOfNDAAgreement"]) != "")
                        {
                            txtDateOfAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_DateOfNDAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_ExpirationDateofNDAAgreement"]) != "")
                        {
                            txtExpirationDateOfAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_ExpirationDateofNDAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        ddlNDASignedByClient.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedByClient"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedByClient"]) == "True" ? "1" : "0");
                        ddlNDASignedByInfinity.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedBYInfinity"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_NDASignedByInfinity"]) == "True" ? "1" : "0");
                    }
                    ddsla.Value = Convert.ToString(dtpaid.Rows[0]["IBS_SLASigned"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_SLASigned"]) == "True" ? "1" : "0");

                    if (ddsla.Value == "1")
                    {
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_DateOfSLAAgreement"]) != "")
                        {
                            txtDateOfSLAAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_DateOfSLAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        if (Convert.ToString(dtpaid.Rows[0]["IBS_ExpirationDateofSLAAgreement"]) != "")
                        {
                            txtExpirationDateOfSLAAgreement.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_ExpirationDateofSLAAgreement"]).ToString("dd-MMM-yyyy");
                        }
                        ddlSLASignedByClient.Value = Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByClient"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByClient"]) == "True" ? "1" : "0");
                        ddlSLASignedByInfinity.Value = Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByInfinity"]) == "" ? "Select" : (Convert.ToString(dtpaid.Rows[0]["IBS_SLASignedByInfinity"]) == "True" ? "1" : "0");
                    }

                    ddlProjectStatus.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ProjectStatus"]);
                    if (Convert.ToString(dtpaid.Rows[0]["IBS_ProjectStatus"]) == "Live Stopped")
                    {
                        trlivestopped.Style.Add("display", "");
                        txtStoppedDate.Value = Convert.ToString(dtpaid.Rows[0]["IBS_StoppedDate"]);
                        txtStoppedRemark.Value = Convert.ToString(dtpaid.Rows[0]["IBS_StoppedRemark"]);
                    }
                    else
                    {
                        trlivestopped.Style.Add("display", "none");
                    }
                    txtExpectedVolume.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ExpectedVolume"]);
                    if (Convert.ToString(dtpaid.Rows[0]["IBS_ExpectedStartDate"]) != "")
                        txtExpectedStartDate.Value = Convert.ToDateTime(dtpaid.Rows[0]["IBS_ExpectedStartDate"]).ToString("dd-MMM-yyyy");
                    ddlProjectDuration.Value = Convert.ToString(dtpaid.Rows[0]["IBS_ProjectDuration"]);
                    txtSalesRemark.Value = Convert.ToString(dtpaid.Rows[0]["IBS_Remark"]);
                    btnSalesInformation.InnerText = "Update";
                    hdnSLAPath.Value = Convert.ToString(dtpaid.Rows[0]["IBS_NDAFilePath"]);
                    hdnMSAPath.Value = Convert.ToString(dtpaid.Rows[0]["IBS_MSAFilePath"]);
                }
            }
        }
        protected void LinkButtonSLA_Click(object sender, System.EventArgs e)
        {
            if (hdnMSAPath.Value != "")
            {
                string Filename = Convert.ToString(hdnMSAPath.Value);
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + Filename.Substring(Filename.LastIndexOf("\\") + 1));
                Response.TransmitFile(Filename);
                Response.End();
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "No document is uploaded!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }

        }
        protected void LinkButtonNDA_Click(object sender, System.EventArgs e)
        {
            if (hdnSLAPath.Value != "")
            {
                string Filename = Convert.ToString(hdnSLAPath.Value);
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + Filename.Substring(Filename.LastIndexOf("\\") + 1));
                Response.TransmitFile(Filename);
                Response.End();
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "No document is uploaded!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }

        }
        #endregion

        #region Billing Parameters


        public DataTable GetDDLValues()
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_getDDLValues");
            // SQLHelper.AddParamToSQLCmd(cmd, "@ApprovalId", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ApprovalId);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public class Record
        {
            int id;
            string comment;
            string additional;
            string remark;
            string chargetype;
            string commentfrombdm;

            public Record(int id, string comment, string additional, string remark, string chargetype, string commentfrombdm)
            {
                this.id = id;
                this.comment = comment;
                this.additional = additional;
                this.remark = remark;
                this.chargetype = chargetype;
                this.commentfrombdm = commentfrombdm;
            }
            public int Id { get { return this.id; } }
            public string Comment { get { return comment; } }
            public string Additional { get { return additional; } }
            public string Remark { get { return remark; } }
            public string ChargeType { get { return chargetype; } }
            public string CommentFromBdm { get { return commentfrombdm; } }

        }

        public class RecordUW
        {
            int id;
            string comment;
            string billingType;
            string remark;
            string chargetype;
            string commentfrombdm;
            string uWVerification;

            public RecordUW(int id, string comment, string BillingType, string remark, string chargetype, string commentfrombdm, string UWVerification)
            {
                this.id = id;
                this.comment = comment;
                this.billingType = BillingType;
                this.remark = remark;
                this.chargetype = chargetype;
                this.commentfrombdm = commentfrombdm;
                this.uWVerification = UWVerification;
            }
            public int Id { get { return this.id; } }
            public string Comment { get { return comment; } }
            public string BillingType { get { return billingType; } }
            public string Remark { get { return remark; } }
            public string ChargeType { get { return chargetype; } }
            public string CommentFromBdm { get { return commentfrombdm; } }
            public string UWVerification { get { return uWVerification; } }

        }

        public class RecordCosting
        {
            int id;
            string comment;
            string additional;
            string remark;
            string chargetype;

            public RecordCosting(int id, string comment, string additional, string remark, string chargetype)
            {
                this.id = id;
                this.comment = comment;
                this.additional = additional;
                this.remark = remark;
                this.chargetype = chargetype;
            }
            public int Id { get { return this.id; } }
            public string Comment { get { return comment; } }
            public string Additional { get { return additional; } }
            public string Remark { get { return remark; } }
            public string ChargeType { get { return chargetype; } }

        }

        public class RecordCostingUW
        {

            int id;
            string comment;
            string billingType;
            string remark;
            string chargetype;
            string commentfrombdm;
            string uWVerification;

            public RecordCostingUW(int id, string comment, string BillingType, string remark, string chargetype, string commentfrombdm, string UWVerification)
            {
                this.id = id;
                this.comment = comment;
                this.billingType = BillingType;
                this.remark = remark;
                this.chargetype = chargetype;
                this.commentfrombdm = commentfrombdm;
                this.uWVerification = UWVerification;
            }
            public int Id { get { return this.id; } }
            public string Comment { get { return comment; } }
            public string BillingType { get { return billingType; } }
            public string Remark { get { return remark; } }
            public string ChargeType { get { return chargetype; } }
            public string CommentFromBdm { get { return commentfrombdm; } }
            public string UWVerification { get { return uWVerification; } }



        }

        public DataTable GetBillingPArAM()
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_getallbillingParameters");
            SQLHelper.AddParamToSQLCmd(cmd, "@DomainId", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ddldomain.Value);

            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            RenderBillingParams(dt);
            return dt;
        }
        #endregion

        #region Unused Methods
        public void BindProjects(int DomainID)
        {
            DataTable dt = bllMaster.GetAllProjectByDomainWise(DomainID, Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            if (dt.Rows.Count > 0)
            {
                ddlProjects.DataSource = dt;
                ddlProjects.DataTextField = "ProjectName";
                ddlProjects.DataValueField = "ProjectId";
                ddlProjects.DataBind();
                ddlProjects.Items.Insert(0, new ListItem("Select"));
            }
        }

        protected void ddldomain_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProjects(Convert.ToInt32(ddldomain.Value));
            if (rdcardYes.Checked == false)
            {
                trnewprocess.Style.Add("display", "");
                tdprojtxt.Style.Add("display", "none");
                txtProjectname.Style.Add("display", "none");
            }
            //BindGridDomainWise();

            if (Convert.ToString(ddldomain.Value) != "Select" && Convert.ToString(ddldomain.Value) != "" && Convert.ToInt32(ddldomain.Value) > 0)
            {
                BindParamsGrid(Convert.ToInt32(ddldomain.Value));
            }
            else
            {
                BindParamsGrid(4);
            }
        }

        protected void grdAllFeedbcak_BeforePerformDataSelect(object sender, EventArgs e)
        {

        }

        #endregion

        protected void btnBack_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Accounts/ProjectDetailsMaster.aspx");
        }


        public DataTable GetColumndata(string ProjectId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "GetColumndatabyprojectIdForCosting");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectId);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        public int DeleteExisingCostingWithParameters(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_DeleteOldPriceDetails");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        public int InsertCostingWithParameters(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_Billing_InsertCostingwithBillingParameters_Temp");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, htParam["ProjectId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@IBP_Id", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, htParam["IBP_Id"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@CostingColumn", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["CostingColumn"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@RemarkCost", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["RemarkCost"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        public int InsertBillParameterCosting(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_BillingInsertParametersCosting]");
            SQLHelper.AddParamToSQLCmd(cmd, "@IBV_ParameterId", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["IBV_ParameterId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@IBV_Comment", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["IBV_Comment"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@IBV_Additional", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["IBV_Additional"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@IBV_Remark", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["IBV_Remark"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@IBV_ChargeType", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["IBV_ChargeType"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["ProjectId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Unique", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, htParam["Unique"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Date", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, htParam["Date"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }


        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProjects.SelectedIndex > 0)
            {
                DataTable dtpaid = bllMaster.GetbyProjectApproval_IdReq(Convert.ToInt32(ddlProjects.Value));
                if (dtpaid != null)
                {
                    if (dtpaid.Rows.Count > 0)
                    {
                        ddldomain.Value = Convert.ToString(dtpaid.Rows[0]["PAI_DomainId"]);
                        txtProjectname.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]);
                        //if (Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]) != "")
                        //{
                        //    ddlProjects.DataSource = bllMaster.GetAllProjectByUserRights();
                        //    ddlProjects.DataTextField = "ProjectName";
                        //    ddlProjects.DataValueField = "ProjectName";
                        //    ddlProjects.DataBind();
                        //    ddlProjects.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));
                        //    trnewprocess.Style.Add("display", "");
                        //    ddlProjects.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]) + "-" + Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]);
                        //    tdprojtxt.Style.Add("display", "none");
                        //    txtProjectname.Style.Add("display", "none");
                        //    txtProcess.Value = Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]);
                        //    ddlprojNo.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]) + "-" + Convert.ToString(dtpaid.Rows[0]["PAI_ProcessName"]);
                        //}
                        //else
                        //{
                        //    trnewprocess.Style.Add("display", "none");
                        //    tdprojtxt.Style.Add("display", "");
                        //    txtProjectname.Style.Add("display", "");
                        //}
                        //lblSalesProjectNumber.Text = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]);
                        //txtCostingProjectNumber.Text = Convert.ToString(dtpaid.Rows[0]["PAI_Project_Name"]);
                        txtcompanyname.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Company_Name"]);
                        txtContactPerson.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Contact_Person"]);
                        txtphonenumber.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Phone_Number"]);
                        txtemail.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Email_Id"]);
                        txturl.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Url"]);
                        txtaddress.Value = Convert.ToString(dtpaid.Rows[0]["PAI_Address"]);

                    }
                }
            }
        }
        public DataTable GetCostingDetailsFTE(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetFTEProjectCosting");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        public DataTable GetCostingDetails861(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetFTEProjectCostingFor861");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public int InsertCostingWithParameters_FTE(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_Billing_InsertCostingwithBillingParameters_FTE_Temp]");
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

        public DataTable GetCostingDetailsWHoleLoan()
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetWholeLoanCosting");
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public int InsertCostingWithParameters_WholeLoan(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[up_InsertWholeLoanCost]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, htParam["ProjectID"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillTo", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["BillTo"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Price", System.Data.SqlDbType.Decimal, 10, System.Data.ParameterDirection.Input, htParam["Price"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }


        protected void btnFTESubmit_Click(object sender, EventArgs e)
        {
            Hashtable htParam = new Hashtable();
            htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
            htParam.Add("CostingColumn", "Order#");
            htParam.Add("RemarkCost", Convert.ToString(txtHourlyRate.Value));
            htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            htParam.Add("CostingColumnValue", Convert.ToString(""));
            htParam.Add("ChargeType", "Fix Amount");
            htParam.Add("RateForOthers", "");
            int result = InsertCostingWithParameters_FTE(htParam);
            if (result > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "Costing added successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
                dvError.InnerHtml = "Error occured while adding costing!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                txtProjectname.Focus();
            }
        }

        protected void btnSubmit861_Click(object sender, EventArgs e)
        {
            Hashtable htParam = new Hashtable();
            htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
            htParam.Add("CostingColumn", "Order#");
            htParam.Add("RemarkCost", Convert.ToString(txtfirst1200Rate.Value));
            htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            htParam.Add("CostingColumnValue", Convert.ToString(txtWireTransferCharges.Value));
            htParam.Add("ChargeType", "Fix Amount");
            htParam.Add("RateForOthers", txtAdditionalBallotsRate.Value);
            int result = InsertCostingWithParameters_FTE(htParam);
            if (result > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "Costing added successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
                dvError.InnerHtml = "Error occured while adding costing!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                txtProjectname.Focus();
            }
        }

        protected void btnwholeloansubmit_Click(object sender, EventArgs e)
        {
            Hashtable htParam = new Hashtable();
            htParam.Add("ProjectID", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
            htParam.Add("BillTo", txtBilledTo.Value.Trim());
            htParam.Add("Price", txtWholeloanPrice.Value);
            htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            int result = InsertCostingWithParameters_WholeLoan(htParam);
            if (result > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "Costing added successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                RenderWholeLoan(GetCostingDetailsWHoleLoan());
                txtWholeloanPrice.Value = "";
                txtBilledTo.Value = "";
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
                dvError.InnerHtml = "Error occured while adding costing!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                txtProjectname.Focus();
            }
        }
        private static string H(object value) { return HttpUtility.HtmlEncode(Convert.ToString(value)); }
        private static string JsValue(object value) { return HttpUtility.JavaScriptStringEncode(Convert.ToString(value)); }
        private static string Cell(DataRow row, string column) { return row != null && row.Table.Columns.Contains(column) ? Convert.ToString(row[column]) : string.Empty; }

        private void RenderProjectSummary(DataTable dt)
        {
            if (projectSummaryRows == null) return;
            StringBuilder sb = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                string[] labels = { "Project #", "Domain", "Start Date", "Billing Cycle", "BDM", "CRM", "Unit Head", "Record #" };
                string[] values = { Cell(r,"ProjectName"), Cell(r,"DomainName"), Cell(r,"ProjectStartDate"), Cell(r,"BillingCycle"), Cell(r,"BDM"), Cell(r,"CRM"), Cell(r,"UH"), "1" };
                for (int i = 0; i < labels.Length; i++)
                {
                    sb.Append("<div class=\"summary-item");
                    if (i == 0) sb.Append(" summary-primary");
                    sb.Append("\"><span class=\"summary-label\">").Append(H(labels[i])).Append("</span><span class=\"summary-value\">")
                      .Append(H(values[i])).Append("</span></div>");
                }
            }
            projectSummaryRows.InnerHtml = sb.ToString();
        }

        private void RenderProjectRequests(DataTable dt)
        {
            if (projectRequestRows == null) return;
            StringBuilder sb = new StringBuilder(); int n = 1;
            if (dt != null) foreach (DataRow r in dt.Rows)
            {
                string id = Cell(r,"PAI_Id");
                string url = "ProjectDetails.aspx?ProjectApproval_Id=" + HttpUtility.UrlEncode(id);
                sb.Append("<tr><td>").Append(n++).Append("</td><td>").Append(H(Cell(r,"PAI_Project_Name"))).Append("</td><td>").Append(H(Cell(r,"PAI_Company_Name"))).Append("</td><td>").Append(H(Cell(r,"PAI_Contact_Person"))).Append("</td><td>").Append(H(Cell(r,"PAI_Phone_Number"))).Append("</td><td>").Append(H(Cell(r,"PAI_Email_Id"))).Append("</td><td>").Append(H(Cell(r,"PAI_Url"))).Append("</td><td>").Append(H(Cell(r,"PAI_Address"))).Append("</td><td>").Append(H(Cell(r,"PAI_Remark"))).Append("</td><td><a class='btn btn-sm btn-outline-primary' href='").Append(url).Append("'>Edit</a></td></tr>");
            }
            projectRequestRows.InnerHtml = sb.ToString();
        }


        private void BindMigratedParameterTables()
        {
            int projectId;
            if (!int.TryParse(Convert.ToString(Request.QueryString["ERPProjectId"]), out projectId) || projectId <= 0) return;
            int domainId;
            if (!int.TryParse(Convert.ToString(ddldomain.Value), out domainId) || domainId <= 0) domainId = 4;
            Session["ProjectId"] = projectId;
            Session["formdate"] = domainId;
            RenderBillingParams(GetParameterTable("usp_getallbillingParameters", projectId, domainId));
            RenderCostingParams(GetParameterTable("usp_getallbillingParametersForCosting", projectId, domainId));
        }

        private static DataTable GetParameterTable(string procedure, int projectId, int domainId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, procedure);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 0, ParameterDirection.Input, projectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@DomainId", SqlDbType.Int, 0, ParameterDirection.Input, domainId);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }
        public class BillingParameterRow
        {
            public int Id { get; set; }
            public string ParameterName { get; set; }
            public string Comment { get; set; }
            public string Additional { get; set; }
            public string ChargeType { get; set; }
            public string Remark { get; set; }
            public string CommentFromBdm { get; set; }
            public string BillingColumn { get; set; }
        }
        public class AjaxResult { public bool Success { get; set; } public string Message { get; set; } public string Field { get; set; } }

        private void RenderBillingParams(DataTable dt)
        {
            if (billingParamRows == null) return;
            DataTable choices = GetDDLValues(); StringBuilder options = BuildOptions(choices);
            StringBuilder sb = new StringBuilder(); int n=0;
            if (dt != null) foreach(DataRow r in dt.Rows)
            {
                string id=Cell(r,"IBP_Id"), parameter=Cell(r,"IBP_ParameterName"), comment=Cell(r,"IBV_Comment"), additional=Cell(r,"IBV_Additional"), charge=Cell(r,"IBV_ChargeType"), remark=Cell(r,"IBV_Remark"), bdm=Cell(r,"IBV_CommentFromBDM");
                sb.Append("<tr data-id='").Append(H(id)).Append("' data-name='").Append(H(parameter)).Append("'><td>").Append(++n).Append("</td><td>").Append(H(parameter)).Append("</td>");
                sb.Append("<td><select class='form-control bp-comment'>").Append(SelectOptions(options.ToString(),comment)).Append("</select></td>");
                sb.Append("<td><select class='form-control bp-additional' ").Append(n==1?"disabled":"").Append(">").Append(SelectOptions(options.ToString(),additional)).Append("</select></td>");
                sb.Append("<td><select class='form-control bp-charge' ").Append(n==1?"disabled":"").Append(">").Append(StaticChargeOptions(charge)).Append("</select></td>");
                sb.Append("<td><input class='form-control bp-remark' value='").Append(H(remark=="0"?"":remark)).Append("'/></td><td><textarea class='form-control bp-bdm'>").Append(H(bdm=="0"?"":bdm)).Append("</textarea></td></tr>");
            }
            billingParamRows.InnerHtml=sb.ToString();
        }
        private void RenderCostingParams(DataTable dt)
        {
            if (costingParamRows == null) return;
            DataTable choices = GetDDLValues(); StringBuilder options = BuildOptions(choices);
            StringBuilder sb=new StringBuilder(); int n=0;
            if(dt!=null) foreach(DataRow r in dt.Rows)
            {
                string id=Cell(r,"IBP_Id"), parameter=Cell(r,"IBP_ParameterName"), comment=Cell(r,"IBV_Comment"), additional=Cell(r,"IBV_Additional"), charge=Cell(r,"IBV_ChargeType"), remark=Cell(r,"IBV_Remark"), bdm=Cell(r,"IBV_CommentFromBDM"), col=Cell(r,"IBV_CostingColumn");
                sb.Append("<tr data-id='").Append(H(id)).Append("' data-name='").Append(H(parameter)).Append("'><td>").Append(++n).Append("</td><td>").Append(H(parameter)).Append("</td>");
                sb.Append("<td><select class='form-control cp-comment'>").Append(SelectOptions(options.ToString(),comment)).Append("</select></td><td><select class='form-control cp-additional'>").Append(SelectOptions(options.ToString(),additional)).Append("</select></td><td><select class='form-control cp-charge'>").Append(StaticChargeOptions(charge)).Append("</select></td><td><input class='form-control cp-remark' value='").Append(H(remark=="0"?"":remark)).Append("'/></td><td>").Append(H(bdm)).Append("</td><td><select class='form-control cp-column'>").Append(BillingColumnOptions(col)).Append("</select></td></tr>");
            }
            costingParamRows.InnerHtml=sb.ToString();
        }
        private void RenderWholeLoan(DataTable dt)
        {
            if(wholeLoanRows==null)return; StringBuilder sb=new StringBuilder(); int n=1;
            if(dt!=null) foreach(DataRow r in dt.Rows) sb.Append("<tr><td>").Append(n++).Append("</td><td>").Append(H(Cell(r,"BilledTo"))).Append("</td><td>").Append(H(Cell(r,"Price"))).Append("</td></tr>");
            wholeLoanRows.InnerHtml=sb.ToString();
        }
        private static StringBuilder BuildOptions(DataTable dt){var sb=new StringBuilder("<option value='Select'>Select</option>"); if(dt!=null)foreach(DataRow r in dt.Rows){string v=Convert.ToString(r["Text"]);sb.Append("<option value='").Append(HttpUtility.HtmlAttributeEncode(v)).Append("'>").Append(HttpUtility.HtmlEncode(v)).Append("</option>");}return sb;}
        private static string SelectOptions(string htmlOptions,string selected){ if(string.IsNullOrEmpty(selected)||selected=="0") selected="Select"; return Regex.Replace(htmlOptions,"value='"+Regex.Escape(HttpUtility.HtmlAttributeEncode(selected))+"'","value='"+HttpUtility.HtmlAttributeEncode(selected)+"' selected='selected'",RegexOptions.IgnoreCase); }
        private static string StaticChargeOptions(string selected){string[] a={"Select","Fix Amount","Incremental","Percentage"};var sb=new StringBuilder();foreach(string v in a)sb.Append("<option value='").Append(v).Append("'").Append(v==selected?" selected='selected'":"").Append(">").Append(v).Append("</option>");return sb.ToString();}
        private static string BillingColumnOptions(string selected){string[] a={"Select","Invoice#","Dispatched Record","No Of Records","Order#","Rush","Weekday","Weekend","Product Type","All","Update","Update 1","Update 2","Update 3","Update 4","Update 5","Status","After Hours","Checklist to Client","Delivery Procedure to client"};var sb=new StringBuilder();foreach(string v in a)sb.Append("<option value='").Append(HttpUtility.HtmlAttributeEncode(v)).Append("'").Append(v==selected?" selected='selected'":"").Append(">").Append(HttpUtility.HtmlEncode(v)).Append("</option>");return sb.ToString();}

        [System.Web.Services.WebMethod]
        public static AjaxResult UpdateBillingParametersAjax(int projectId, BillingParameterRow[] rows)
        {
            try
            {
                if(rows==null) rows=new BillingParameterRow[0];
                foreach(var row in rows){if(row.Comment=="Select") return new AjaxResult{Success=false,Message="Please select Is Applicable.",Field=row.ParameterName}; if(row.Comment=="Yes" && string.IsNullOrWhiteSpace(row.Remark)) return new AjaxResult{Success=false,Message="Please enter price.",Field=row.ParameterName}; if(row.Additional=="Yes" && row.ChargeType=="Select") return new AjaxResult{Success=false,Message="Please select charge type.",Field=row.ParameterName};}
                int index=0;
                foreach(var row in rows)
                {
                    Hashtable h=new Hashtable(); h["IBV_ParameterId"]=row.Id;h["IBV_Comment"]=row.Comment;h["IBV_Additional"]=row.Additional;h["IBV_Remark"]=row.Remark;h["IBV_ChargeType"]=(index++==0?"Fix Amount":row.ChargeType);h["IBV_CommentFromBDM"]=row.CommentFromBdm;h["AddedBy"]=Convert.ToInt32(HttpContext.Current.User.Identity.Name);h["ProjectId"]=projectId;
                    new bllTracking().InsertBillParameter(h);
                }
                return new AjaxResult{Success=true,Message="Billing parameters updated successfully."};
            }catch(Exception ex){return new AjaxResult{Success=false,Message=ex.Message};}
        }

        [System.Web.Services.WebMethod]
        public static AjaxResult UpdateCostingParametersAjax(int projectId, BillingParameterRow[] rows)
        {
            try
            {
                if(rows==null) rows=new BillingParameterRow[0];
                foreach(var row in rows){if(row.Comment=="Select") return new AjaxResult{Success=false,Message="Please select Is Applicable.",Field=row.ParameterName}; if(row.Comment=="Yes" && string.IsNullOrWhiteSpace(row.Remark) && row.Additional=="Yes") return new AjaxResult{Success=false,Message="Please enter price.",Field=row.ParameterName}; if(row.Comment=="Yes" && row.BillingColumn=="Select") return new AjaxResult{Success=false,Message="Please select billing header.",Field=row.ParameterName};}
                string unique=DateTime.Now.ToString("ddMMyyyyhhmmss"), date=DateTime.Now.ToString("dd-MMM-yyyy");
                foreach(var row in rows)
                {
                    Hashtable h=new Hashtable();h["IBV_ParameterId"]=row.Id;h["IBV_Comment"]=row.Comment;h["IBV_Additional"]=row.Additional;h["IBV_Remark"]=row.Remark;h["IBV_ChargeType"]=row.ChargeType;h["ProjectId"]=projectId;h["Unique"]=unique;h["Date"]=date;h["AddedBy"]=Convert.ToInt32(HttpContext.Current.User.Identity.Name);
                    InsertBillParameterCostingAjax(h);
                }
                string baseRate = rows.Length>0 && !string.IsNullOrWhiteSpace(rows[0].Remark)?rows[0].Remark:"0";
                string updateBase = rows.Length>4 && !string.IsNullOrWhiteSpace(rows[4].Remark)?rows[4].Remark:"0";
                string fha="0",multi="0",fhaMulti="0",fhaInc="0",multiInc="0",fhaMultiInc="0",fhaUpdate="0",multiUpdate="0",fhaMultiUpdate="0";
                if(rows.Length>11 && !string.IsNullOrWhiteSpace(rows[11].Remark)) fha=rows[11].Remark;if(rows.Length>15&&!string.IsNullOrWhiteSpace(rows[15].Remark))multi=rows[15].Remark;if(rows.Length>9&&!string.IsNullOrWhiteSpace(rows[9].Remark))fhaMultiUpdate=rows[9].Remark;if(rows.Length>13&&!string.IsNullOrWhiteSpace(rows[13].Remark))fhaUpdate=rows[13].Remark;if(rows.Length>17&&!string.IsNullOrWhiteSpace(rows[17].Remark))multiUpdate=rows[17].Remark;
                foreach(var row in rows)
                {
                    string cost=row.Remark??""; double c=Num(cost), b=Num(baseRate), ub=Num(updateBase); string computed=cost;
                    if(row.ChargeType=="Incremental") { if(row.Id==6)computed=(c+ub).ToString(); else if(row.Id==8){fhaInc=(c+b).ToString();computed=fhaInc;} else if(row.Id==29){fhaMultiInc=(c+b).ToString();computed=fhaMultiInc;} else if(row.Id==23)computed=(c+Num(fhaInc)).ToString(); else if(row.Id==31)computed=(c+Num(fhaMultiInc)).ToString(); else if(row.Id==9){multiInc=(c+b).ToString();computed=multiInc;} else if(row.Id==24)computed=(c+Num(multiInc)).ToString(); else if(row.Id==26)computed=(c+Num(fhaUpdate)).ToString(); else if(row.Id==28)computed=(c+Num(multiUpdate)).ToString(); else if(row.Id==33)computed=(c+Num(fhaMultiUpdate)).ToString(); else computed=(c+b).ToString(); }
                    else if(row.ChargeType=="Percentage") { if(row.Id==6)computed=(ub*c/100+ub).ToString(); else if(row.Id==8){fha=(b*c/100+b).ToString();computed=fha;} else if(row.Id==9){multi=(b*c/100+b).ToString();computed=multi;} else if(row.Id==29){fhaMulti=(b*c/100+b).ToString();computed=fhaMulti;} else if(row.Id==23)computed=(Num(fha)*c/100+Num(fha)).ToString(); else if(row.Id==24)computed=(Num(multi)*c/100+Num(multi)).ToString(); else if(row.Id==26)computed=(Num(fhaUpdate)*c/100+Num(fhaUpdate)).ToString(); else if(row.Id==28)computed=(Num(multiUpdate)*c/100+Num(multiUpdate)).ToString(); else if(row.Id==31)computed=(Num(fhaMulti)*c/100+Num(fhaMulti)).ToString(); else computed=(b*c/100+b).ToString(); }
                    Hashtable h2=new Hashtable();h2["ProjectId"]=projectId;h2["IBP_Id"]=row.Id;h2["CostingColumn"]=row.BillingColumn;h2["RemarkCost"]=computed;h2["AddedBy"]=Convert.ToInt32(HttpContext.Current.User.Identity.Name);InsertCostingWithParametersAjax(h2);
                }
                return new AjaxResult{Success=true,Message="Costing parameters updated successfully."};
            }catch(Exception ex){return new AjaxResult{Success=false,Message=ex.Message};}
        }
        private static double Num(string s){double d;return double.TryParse(s,out d)?d:0;}
        private static int InsertBillParameterCostingAjax(Hashtable h){SqlCommand cmd=SQLHelper.GetCommand(CommandType.StoredProcedure,"[usp_BillingInsertParametersCosting]");SQLHelper.AddParamToSQLCmd(cmd,"@IBV_ParameterId",SqlDbType.BigInt,0,ParameterDirection.Input,h["IBV_ParameterId"]);SQLHelper.AddParamToSQLCmd(cmd,"@IBV_Comment",SqlDbType.NVarChar,500,ParameterDirection.Input,h["IBV_Comment"]);SQLHelper.AddParamToSQLCmd(cmd,"@IBV_Additional",SqlDbType.NVarChar,500,ParameterDirection.Input,h["IBV_Additional"]);SQLHelper.AddParamToSQLCmd(cmd,"@IBV_Remark",SqlDbType.NVarChar,500,ParameterDirection.Input,h["IBV_Remark"]);SQLHelper.AddParamToSQLCmd(cmd,"@IBV_ChargeType",SqlDbType.NVarChar,500,ParameterDirection.Input,h["IBV_ChargeType"]);SQLHelper.AddParamToSQLCmd(cmd,"@ProjectId",SqlDbType.BigInt,0,ParameterDirection.Input,h["ProjectId"]);SQLHelper.AddParamToSQLCmd(cmd,"@Unique",SqlDbType.NVarChar,100,ParameterDirection.Input,h["Unique"]);SQLHelper.AddParamToSQLCmd(cmd,"@Date",SqlDbType.NVarChar,100,ParameterDirection.Input,h["Date"]);SQLHelper.AddParamToSQLCmd(cmd,"@AddedBy",SqlDbType.BigInt,0,ParameterDirection.Input,h["AddedBy"]);SQLHelper.AddParamToSQLCmd(cmd,"@ReturnValue",SqlDbType.BigInt,0,ParameterDirection.ReturnValue,null);SQLHelper.ExecuteNonQueryCmdBilling(cmd);return Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);}
        private static int InsertCostingWithParametersAjax(Hashtable h){SqlCommand cmd=SQLHelper.GetCommand(CommandType.StoredProcedure,"usp_Billing_InsertCostingwithBillingParameters_Temp");SQLHelper.AddParamToSQLCmd(cmd,"@ProjectId",SqlDbType.BigInt,10,ParameterDirection.Input,h["ProjectId"]);SQLHelper.AddParamToSQLCmd(cmd,"@IBP_Id",SqlDbType.BigInt,10,ParameterDirection.Input,h["IBP_Id"]);SQLHelper.AddParamToSQLCmd(cmd,"@CostingColumn",SqlDbType.NVarChar,4000,ParameterDirection.Input,h["CostingColumn"]);SQLHelper.AddParamToSQLCmd(cmd,"@RemarkCost",SqlDbType.NVarChar,4000,ParameterDirection.Input,h["RemarkCost"]);SQLHelper.AddParamToSQLCmd(cmd,"@AddedBy",SqlDbType.BigInt,0,ParameterDirection.Input,h["AddedBy"]);SQLHelper.AddParamToSQLCmd(cmd,"@ReturnValue",SqlDbType.BigInt,0,ParameterDirection.ReturnValue,null);SQLHelper.ExecuteNonQueryCmdBilling(cmd);return Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);}

    }
}
