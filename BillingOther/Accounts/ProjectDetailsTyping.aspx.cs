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
using System.IO;
using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;

namespace BillingOther.Accounts
{
    public partial class ProjectDetailsTyping : System.Web.UI.Page
    {
        bllTracking bllMaster = new bllTracking();

        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Cost Master";
            int ProjectApproval_Id = Convert.ToInt32(Request.QueryString["ERPProjectId"]);
            
            string ss = Convert.ToString(Session["formdate"]);


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
                    //aBack.HRef = "~/ProjectDetails.aspx";
                    btnsbmit.InnerText = "Update";
                }
                else
                {
                   // Tab1.Style.Add("margin-top", "-70px");
                }
                if (Convert.ToString(Request.QueryString["ERPProjectId"]) != "" && Convert.ToString(Request.QueryString["ERPProjectId"]) != null)
                {
                    rdButtons.Style.Add("display", "none");
                    GetAllInformationSales();
                    BindBaseProductType(Convert.ToString(Request.QueryString["ERPProjectId"]));
                    BindTypingCosting(Convert.ToInt32(Request.QueryString["ERPProjectId"]));
                }
                else
                {
                    rdButtons.Style.Add("display", "");
                }
            }



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
            DataTable dtCrt = GetBillingCriteria(ProjectId);
            if (dtCrt != null)
            {
                if (dtCrt.Rows.Count > 0)
                {
                    ddlBillingCriteria.Value = Convert.ToString(dtCrt.Rows[0]["Criteria"]);
                }
            }
            DataTable dt = GetCostingDetailsTyping(ProjectId);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    string BillingBase = Convert.ToString(dt.Rows[0]["BillingBase"]);
                    if (BillingBase == "Base")
                    {
                        #region Base Rate Only
                        ddlBillingBase.SelectedIndex = 1;
                        trBaseRate.Style.Add("display", "");
                        trProductType.Style.Add("display", "none");
                        trOrderType.Style.Add("display", "none");
                        trCharacter.Style.Add("display", "none");
                        tbl379003.Style.Add("display", "none");

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
                        #endregion
                    }
                    else if (BillingBase == "Product Type")
                    {
                        #region Product Type
                        ddlBillingBase.SelectedIndex = 2;
                        string BaseProductType = "";
                        trBaseRate.Style.Add("display", "none");
                        trProductType.Style.Add("display", "");
                        trOrderType.Style.Add("display", "none");
                        trCharacter.Style.Add("display", "none");
                        tbl379003.Style.Add("display", "none");
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
                        #region OLD Code for Costing
                        //string find = "ProjectColumn in ('Order#') and AdditionalConditions=1 and Updatelevel='Fix Amount'";
                        //DataRow[] foundRows = dt.Select(find);
                        //if (foundRows.Length > 0)
                        //{
                        //    ddlBaseProductType.Value = Convert.ToString(foundRows[0]["ConditionalValue"]);
                        //    hdProductBaseLabel.Value = Convert.ToString(foundRows[0]["ConditionalValue"]);
                        //    lblProdcutBase.InnerText = Convert.ToString(foundRows[0]["ConditionalValue"]);
                        //    ddlIsApplicableProductBase.Value = "Yes";
                        //    ddlChargeTypeProductBase.Value = Convert.ToString(foundRows[0]["Updatelevel"]);
                        //    txtPriceProductBase.Value = Convert.ToString(foundRows[0]["RateForOthers"]);
                        //    ddlBillingHEaderProductBase.Value = Convert.ToString(foundRows[0]["ConditionalColumn"]);
                        //}
                        //find = "ProjectColumn in ('Order#') and AdditionalConditions=1 and ConditionalValue not in ('" + ddlBaseProductType.Value + "')";
                        //DataRow[] foundRows1 = dt.Select(find);
                        //if (foundRows1.Length > 0)
                        //{
                        //    lblProdcutOther.InnerText = Convert.ToString(foundRows1[0]["ConditionalValue"]);
                        //    hdProductOtherLabel.Value = Convert.ToString(foundRows1[0]["ConditionalValue"]);
                        //    ddlIsApplicableProductOther.Value = "Yes";
                        //    ddlChargeTypeProductOther.Value = Convert.ToString(foundRows1[0]["Updatelevel"]);
                        //    txtPriceProductOther.Value = Convert.ToString(foundRows1[0]["RateForOthers"]);
                        //    ddlBillingHEaderProductOther.Value = Convert.ToString(foundRows1[0]["ConditionalColumn"]);
                        //}
                        //find = "ProjectColumn = '# of Character'";
                        //DataRow[] foundRows2 = dt.Select(find);
                        //if (foundRows2.Length > 0)
                        //{
                        //    ddlIsApplicableProductCharacter.Value = "Yes";
                        //    ddlChargeTypeProductCharacter.Value = Convert.ToString(foundRows2[0]["Updatelevel"]);
                        //    txtPriceProductCharacter.Value = Convert.ToString(foundRows2[0]["RateForOthers"]);
                        //    ddlBillingHeaderproductCharacter.Value = Convert.ToString(foundRows2[0]["ProjectColumn"]);
                        //}
                        //else
                        //{
                        //    ddlIsApplicableProductCharacter.Value = "No";
                        //    ddlChargeTypeProductCharacter.Value = Convert.ToString("Select");
                        //    txtPriceProductCharacter.Value = Convert.ToString("");
                        //    ddlBillingHeaderproductCharacter.Value = Convert.ToString("Select");
                        //}
                        #endregion
                        //}
                        #endregion
                    }
                    else if (BillingBase == "Order Type")
                    {
                        #region Order Type
                        ddlBillingBase.SelectedIndex = 3;

                        trBaseRate.Style.Add("display", "none");
                        trProductType.Style.Add("display", "none");
                        trOrderType.Style.Add("display", "");
                        trCharacter.Style.Add("display", "none");
                        tbl379003.Style.Add("display", "none");

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
                        #endregion
                    }
                    else if (BillingBase == "Character Based")
                    {
                        #region Character Based
                        ddlBillingBase.SelectedIndex = 4;

                        trBaseRate.Style.Add("display", "none");
                        trProductType.Style.Add("display", "none");
                        trOrderType.Style.Add("display", "none");
                        trCharacter.Style.Add("display", "");
                        tbl379003.Style.Add("display", "none");
                        string find = "ProjectColumn ='# of Character'";
                        DataRow[] foundRows = dt.Select(find);
                        if (foundRows.Length > 0)
                        {
                            txtPerCharacter.Value = Convert.ToString(foundRows[0]["RateForOthers"]);
                            txtPricePerCharacter.Value = Convert.ToString(foundRows[0]["Rate"]);
                            ddlBillingHeaderCharacter.Value = Convert.ToString(foundRows[0]["ProjectColumn"]);
                        }
                        #endregion
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

        public void bindgrid()
        {

            DataTable dt = bllMaster.BindVerifyProjectById(int.Parse(Request.QueryString["ERPProjectId"]));
            RenderProjectSummary(dt);
            //clear();
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
                    //lblSalesProjectNumber.Text = txtProjectname.Value;
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Project information added successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    txtProjectname.Focus();                                     
                    hdndomain.Value = ddldomain.Value;
                    Response.Redirect("~/ProjectDetails.aspx?ERPProjectId=" + ReturnValue + "&DomainID=" + ddldomain.Value);
                    //clear();
                }
                else if (ReturnValue != -1)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Project information already exist!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                   
                }
                else if (ReturnValue == 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error occured while adding project information!";
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
                    dvError.InnerHtml = "Project information updated successfully!";
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
                    dvError.InnerHtml = "Sales information added successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);                   
                    new SendMail().SendProjectCreationEmail(Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    bindgrid();
                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error occured while adding sales information!";
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
                    dvError.InnerHtml = "Error occured while adding sales information!";
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
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
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
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
                dvError.InnerHtml = "No document is uploaded!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);               
            }

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

        }

        protected void grdAllFeedbcak_BeforePerformDataSelect(object sender, EventArgs e)
        {

        }

        #endregion

        protected void btnBack_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Accounts/ProjectDetailsMaster.aspx");
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

        public int InsertBillingBase(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_InsertBillingBase");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, htParam["ProjectId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingBase", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["BillingBase"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        public int InsertBillingCriteria(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_InsertBillingCriteria");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, htParam["ProjectId"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingCriteria", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, htParam["BillingCriteria"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        public DataTable GetBillingCriteria(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetBillingCriteria");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        protected void btnShubmitTyping_Click(object sender, EventArgs e)
        {
            int result = 0;
            Hashtable htBill = new Hashtable();
            htBill.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
            htBill.Add("BillingBase", Convert.ToString(ddlBillingBase.Value));
            int ReturnValue = InsertBillingBase(htBill);
            Hashtable htCrt = new Hashtable();
            htCrt.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
            htCrt.Add("BillingCriteria", Convert.ToString(ddlBillingCriteria.Value));
            int ReturnValue1 = InsertBillingCriteria(htCrt);
            if (ddlBillingBase.Items[ddlBillingBase.SelectedIndex].Text == "Base Rate Only")
            {
                #region Base Rate Only
                if (ddlIsApplicableBaseRate.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderBaseRate.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceBaseRate.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", Convert.ToString(""));
                    htParam.Add("ChargeType", ddlChargeTypeBaseRate.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceBaseRate.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableBaseRateRush.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderBaseRateRush.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceBaseRateRush.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", Convert.ToString(""));
                    htParam.Add("ChargeType", ddlChargeTypeBaseRateRush.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceBaseRateRush.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableBaseRateCharacter.Value == "Yes")
                {
                    if (ddlChargeTypeBaseRateCharacter.Value == "Fix Amount")
                    {
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHeaderBaseRateCharacter.Value);
                        htParam.Add("RemarkCost", Convert.ToString(txtPriceBaseRateCharacter.Value));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(""));
                        htParam.Add("ChargeType", ddlChargeTypeBaseRateCharacter.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceBaseRateCharacter.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                    else if (ddlChargeTypeBaseRateCharacter.Value == "Incremental")
                    {
                        decimal rate = 0;
                        rate = Convert.ToDecimal(txtPriceBaseRate.Value) + Convert.ToDecimal(txtPriceBaseRateCharacter.Value);
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHeaderBaseRateCharacter.Value);
                        htParam.Add("RemarkCost", Convert.ToString(rate));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(""));
                        htParam.Add("ChargeType", ddlChargeTypeBaseRateCharacter.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceBaseRateCharacter.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                    else if (ddlChargeTypeBaseRateCharacter.Value == "Percentage")
                    {
                        decimal rate = 0;
                        rate = ((Convert.ToDecimal(txtPriceBaseRate.Value) * Convert.ToDecimal(txtPriceBaseRateCharacter.Value)) / 100) + Convert.ToDecimal(txtPriceBaseRate.Value);
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHeaderBaseRateCharacter.Value);
                        htParam.Add("RemarkCost", Convert.ToString(rate));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(""));
                        htParam.Add("ChargeType", ddlChargeTypeBaseRateCharacter.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceBaseRateCharacter.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                }
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
                #endregion
            }
            else if (ddlBillingBase.Items[ddlBillingBase.SelectedIndex].Text == "Product Type")
            {
                #region Product Type
                if (ddlIsApplicablePurchase.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderPurchase.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPricePurchase.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", "Purchase");
                    htParam.Add("ChargeType", ddlChargeTypePurchase.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPricePurchase.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableRefinance.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderRefinance.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceRefinance.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", "Refinance");
                    htParam.Add("ChargeType", ddlChargeTypeRefinance.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceRefinance.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableCurrentOwner.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderCurrentOwner.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceCurrentOwner.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", "Current Owner");
                    htParam.Add("ChargeType", ddlChargeTypeCurrentOwner.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceCurrentOwner.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableFullSearch.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderFullSearch.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceFullSearch.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", "Full Search");
                    htParam.Add("ChargeType", ddlChargeTypeFullSearch.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceFullSearch.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableTwoOwner.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderTwoOwner.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceTwoOwner.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", "Two Owner");
                    htParam.Add("ChargeType", ddlChargeTypeTwoOwner.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceTwoOwner.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableCharacter.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderCharacterNew.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceCharacter.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", "");
                    htParam.Add("ChargeType", ddlChargeTypeCharacter.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceCharacter.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                
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
                #endregion
            }
            else if (ddlBillingBase.Items[ddlBillingBase.SelectedIndex].Text == "Order Type")
            {
                #region Order Type
                if (ddlIsApplicableOrderBase.Value == "Yes")
                {
                    Hashtable htParam = new Hashtable();
                    htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                    htParam.Add("CostingColumn", ddlBillingHeaderOrderBase.Value);
                    htParam.Add("RemarkCost", Convert.ToString(txtPriceOrderBase.Value));
                    htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                    htParam.Add("CostingColumnValue", Convert.ToString(hdOrderBaseLabel.Value));
                    htParam.Add("ChargeType", ddlChargeTypeOrderBase.Value);
                    htParam.Add("RateForOthers", Convert.ToString(txtPriceOrderBase.Value));
                    result = InsertCostingWithParameters_Typing(htParam);
                }
                if (ddlIsApplicableOrderOther.Value == "Yes")
                {
                    if (ddlChargeTypeOrderOther.Value == "Fix Amount")
                    {
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHeaderOrderOther.Value);
                        htParam.Add("RemarkCost", Convert.ToString(txtPriceOrderOther.Value));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(hdOrderOtherLabel.Value));
                        htParam.Add("ChargeType", ddlChargeTypeOrderOther.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceOrderOther.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                    else if (ddlChargeTypeOrderOther.Value == "Incremental")
                    {
                        decimal rate = 0;
                        rate = Convert.ToDecimal(txtPriceOrderBase.Value) + Convert.ToDecimal(txtPriceOrderOther.Value);
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHeaderOrderOther.Value);
                        htParam.Add("RemarkCost", Convert.ToString(rate));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(hdOrderOtherLabel.Value));
                        htParam.Add("ChargeType", ddlChargeTypeOrderOther.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceOrderOther.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                    else if (ddlChargeTypeOrderOther.Value == "Percentage")
                    {
                        decimal rate = 0;
                        rate = ((Convert.ToDecimal(txtPriceOrderBase.Value) * Convert.ToDecimal(txtPriceOrderOther.Value)) / 100) + Convert.ToDecimal(txtPriceOrderBase.Value);
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHeaderOrderOther.Value);
                        htParam.Add("RemarkCost", Convert.ToString(rate));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(hdOrderOtherLabel.Value));
                        htParam.Add("ChargeType", ddlChargeTypeOrderOther.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceOrderOther.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                }
                if (ddlIsApplicableOrderCharacter.Value == "Yes")
                {
                    if (ddlChargeTypeOrderCharacter.Value == "Fix Amount")
                    {
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHEaderOrderCharacter.Value);
                        htParam.Add("RemarkCost", Convert.ToString(txtPriceOrderCharacter.Value));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(""));
                        htParam.Add("ChargeType", ddlChargeTypeOrderCharacter.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceOrderCharacter.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                    else if (ddlChargeTypeOrderCharacter.Value == "Incremental")
                    {
                        decimal rate = 0;
                        rate = Convert.ToDecimal(txtPriceOrderBase.Value) + Convert.ToDecimal(txtPriceOrderCharacter.Value);
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHEaderOrderCharacter.Value);
                        htParam.Add("RemarkCost", Convert.ToString(rate));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(""));
                        htParam.Add("ChargeType", ddlChargeTypeOrderCharacter.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceOrderCharacter.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                    else if (ddlChargeTypeOrderCharacter.Value == "Percentage")
                    {
                        decimal rate = 0;
                        rate = ((Convert.ToDecimal(txtPriceOrderBase.Value) * Convert.ToDecimal(txtPriceOrderCharacter.Value)) / 100) + Convert.ToDecimal(txtPriceOrderBase.Value);
                        Hashtable htParam = new Hashtable();
                        htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                        htParam.Add("CostingColumn", ddlBillingHEaderOrderCharacter.Value);
                        htParam.Add("RemarkCost", Convert.ToString(rate));
                        htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                        htParam.Add("CostingColumnValue", Convert.ToString(""));
                        htParam.Add("ChargeType", ddlChargeTypeOrderCharacter.Value);
                        htParam.Add("RateForOthers", Convert.ToString(txtPriceOrderCharacter.Value));
                        result = InsertCostingWithParameters_Typing(htParam);
                    }
                }
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
                #endregion
            }
            else if (ddlBillingBase.Items[ddlBillingBase.SelectedIndex].Text == "Character Based")
            {
                #region Character Based
                Hashtable htParam = new Hashtable();
                htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                htParam.Add("CostingColumn", ddlBillingHeaderCharacter.Value);
                htParam.Add("RemarkCost", Convert.ToString(txtPricePerCharacter.Value));
                htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                htParam.Add("CostingColumnValue", Convert.ToString(""));
                htParam.Add("ChargeType", "Fix Amount");
                htParam.Add("RateForOthers", Convert.ToString(txtPerCharacter.Value));
                result = InsertCostingWithParameters_Typing(htParam);
                #endregion
            }
            else if (ddlBillingBase.Items[ddlBillingBase.SelectedIndex].Text == "Page Based")
            {
                #region Page Based
                Hashtable htParam = new Hashtable();
                htParam.Add("ProjectId", Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
                htParam.Add("CostingColumn", ddlBillingHeader771ScanPages.Value);
                htParam.Add("RemarkCost", Convert.ToString(txtPrice771ScanPages.Value));
                htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                htParam.Add("CostingColumnValue", Convert.ToString(""));
                htParam.Add("ChargeType", "Fix Amount");
                htParam.Add("RateForOthers", Convert.ToString(txtPrice771ScanPages.Value));
                result = InsertCostingWithParameters_Typing(htParam);
                #endregion
            }
            BindTypingCosting(Convert.ToInt32(Convert.ToString(Request.QueryString["ERPProjectId"])));
        }

        protected void ddlChargeType_Typing_Init(object sender, EventArgs e)
        {

        }

        protected void txtRemark_Typing_Init(object sender, EventArgs e)
        {

        }


        protected void grdBillingParams_Typing_BeforePerformDataSelect(object sender, EventArgs e)
        {

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

    }
}
