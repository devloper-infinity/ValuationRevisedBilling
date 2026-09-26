using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using BillingOther.App_Code.DAL;
using BillingOther.App_Code.BLL;
using System.Net.Mail;
using System.Web.Services;

namespace BillingOther.Accounts
{
    public partial class SendToClient : System.Web.UI.Page
    {
        SendMail SendEmail = new SendMail();
        bllTracking blltracking = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Sent To Client";
            if (!IsPostBack)
            {

                ddlPeriodBind();
                BindProjects();
                if (ddlMonth.SelectedIndex == 0)
                    BindGrid();
                else
                    BindGridSummaryMonthWise();

            }
            int InvId = Session["InvoiceIdRemark"] == null ? 0 : Convert.ToInt32(Session["InvoiceIdRemark"]);
            //BindGrid();
            if (InvId != 0)
            {
                BindInvoiceRemarks(InvId);
            }

        }
        public void BindGrid()
        {

            DataTable dt = new DataTable();
            dt = blltracking.GetAllProjectforClientApproved(int.Parse(ddlProjects.Value), ddlDatePeriod.Value);
            RenderClientGrid(dt);
        }
        protected void btnShow_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        public void BindProjects()
        {
            ddlProjects.DataSource = blltracking.GetAllProjectByUserRights();
            ddlProjects.DataTextField = "ProjectName";
            ddlProjects.DataValueField = "ProjectId";
            ddlProjects.DataBind();
            ddlProjects.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));
        }
        protected void grdBilling_DataBound(object sender, EventArgs e)
        {

        }

        protected void ddlPeriodBind()
        {
            DateTime now = DateTime.Now;
            DateTime fromDate = now.AddMonths(-5);
            DateTime fromDateNew = now.AddMonths(-5);
            ddlDatePeriod.Items.Clear();
            ddlDatePeriod.Items.Add("Select");
            for (int i = 0; i < 6; i++)
            {
                string Month = fromDate.ToString("MMM");
                var startDate = new DateTime(fromDate.Year, fromDate.Month, 1);
                string start = startDate.ToString("dd-MMM-yyyy");
                var endDate = (startDate.AddMonths(1).AddDays(-1));
                string End = endDate.ToString("dd-MMM-yyyy");

                string FirstHalf = Convert.ToString("01-" + Month + "-" + fromDate.Year + " ~ 15-" + Month + "-" + fromDate.Year);
                string secondHalf = Convert.ToString("16-" + Month + "-" + fromDate.Year + " ~ " + End);

                ddlDatePeriod.Items.Add(FirstHalf);
                ddlDatePeriod.Items.Add(secondHalf);

                fromDate = fromDateNew.AddMonths(i + 1);
            }

        }

        #region FinalSummary

        public DataTable GetInvoiceDetails(string ProjectNumber, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[IB_usp_GetInvoiceDetails_ForRead]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectNumber", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public void BindInvoiceRecievedDetails(string InvDetails)
        {
            string[] Details = InvDetails.Split('|');
            if (Details.Length > 1)
            {
                string ProjectNumber = Details[0];
                string BillingPeriod = Details[1];
                lblProjectPopup.InnerText = ProjectNumber;
                lblInvoiceNoPopup.InnerText = BillingPeriod;
                DataTable dt = new DataTable();
                //dt = blltracking.GetInvoiceDetails(InvoiceId);
                dt = GetInvoiceDetails(ProjectNumber, BillingPeriod);
                if (Convert.ToString(dt.Rows[0]["InvoiceRecievedByClientDate"]) != "")
                    dxInvrecieved.Value = Convert.ToDateTime(dt.Rows[0]["InvoiceRecievedByClientDate"]).ToString("yyyy-MM-dd");
                else
                    dxInvrecieved.Value = "";
                if (Convert.ToString(dt.Rows[0]["CommunicationReciept"]) == "")
                    ddlReciep.SelectedIndex = 0;
                else
                    ddlReciep.Value = Convert.ToString(dt.Rows[0]["CommunicationReciept"]);
                txtInvRemark.Value = Convert.ToString(dt.Rows[0]["Remark"]);
                if (Convert.ToString(dt.Rows[0]["NoDisputeConfirmByClient"]) != "")
                    dxInvConfirm.Value = Convert.ToDateTime(dt.Rows[0]["NoDisputeConfirmByClient"]).ToString("yyyy-MM-dd");
                else
                    dxInvConfirm.Value = "";
                if (Convert.ToString(dt.Rows[0]["InvCompleteDate"]) != "")
                    dxDate.Value = Convert.ToDateTime(dt.Rows[0]["InvCompleteDate"]).ToString("yyyy-MM-dd");
                else
                    dxDate.Value = "";
            }
        }

        protected void btnUpdateInvoice_Click(object sender, EventArgs e)
        {
            string InvDetails = Convert.ToString(Session["InvDetails"]);
            string[] Details = InvDetails.Split('|');
            if (Details.Length > 1)
            {
                string ProjectNumber = Details[0];
                string BillingPeriod = Details[1];
                DataTable dt = GetInvoiceDetails(ProjectNumber, BillingPeriod);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        int InvoiceId = Convert.ToInt32(dt.Rows[0]["InvoiceID"]);
                        Hashtable htparam = new Hashtable();
                        htparam.Add("InvoiceID", InvoiceId);
                        if (dxInvrecieved.Value != "")
                        {
                            htparam.Add("InvoiceRecievedByClientDate", Convert.ToDateTime(dxInvrecieved.Value.Trim()));
                        }
                        htparam.Add("CommunicationReciept", Convert.ToString(ddlReciep.Items[ddlReciep.SelectedIndex].Text));
                        htparam.Add("Remark", Convert.ToString(txtInvRemark.Value.Trim()));
                        if (dxInvConfirm.Value != "")
                        {
                            htparam.Add("NoDisputeConfirmByClient", Convert.ToString(dxInvConfirm.Value.Trim()));
                        }
                        if (dxDate.Value != "")
                        {
                            htparam.Add("InvCompleteDate", Convert.ToString(dxDate.Value.Trim()));
                        }
                        int result = blltracking.UpdateInvoiceDetails(htparam);
                        dvError.Style.Add("display", "");
                        dvError.Attributes.Add("class", "alert alert-success background-success");
                        dvError.InnerHtml = " Details added Successfully.";
                        dvError.Visible = true;

                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                        BindGrid();
                        BindInvoiceRecievedDetails(InvDetails);
                    }
                }
            }


        }

        #endregion

        # region ExcelReport
        public void BindInvoiceRemarks(int InvoiceId)
        {
            DataTable dt = blltracking.GetInvoiceRemarkDetails(InvoiceId);
            RenderRemarkGrid(dt);
        }

        protected void btnUpdateRemark_Click(object sender, EventArgs e)
        {
            if (txtRemark.Value.Trim() == "")
            {
                dvError.Style.Add("display", "");
                lblerrorRemark.InnerText = "Please enter remark";
                dvError.Visible = true;
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                lblerrorRemark.Style["color"] = "red";
                return;
            }

            Hashtable htparam = new Hashtable();
            htparam.Add("InvoiceID", Convert.ToInt32(Session["InvoiceIdRemark"]));
            htparam.Add("InvoiceRemark", Convert.ToString(txtRemark.Value.Trim()));
            htparam.Add("AddedBy", Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            int result = blltracking.UpdateInvoiceRemark(htparam);
            BindInvoiceRemarks(Convert.ToInt32(Session["InvoiceIdRemark"]));
            lblerrorRemark.InnerText = "";
            txtRemark.Value = "";
        }



        #endregion


        public DataTable GetSummaryReportInvoice(string GroupName, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[WBT_usp_GetAllGroupwiseInvoiceNumber_ForOtherDomain]");
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupName", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, GroupName);

            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetClientDetails(string GroupName, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetClientInformation_OtherDomain");
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupName", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, GroupName);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);

            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        public DataTable GetAutoManualInvoice(string GroupName, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetInvoiceNumberAutoManual]");
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupName", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, GroupName);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);

            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        public void BindEmilTemplateForSendMail(string Groupname, string BillingPeriod, int ClientId)
        {


            string Header = "<html><head><meta content='text/html; charset=utf-8' http-equiv='Content-Type'><title></title><style type='text/css'>a:hover { text-decoration: none !important; }.header h1 {color: #fff !important; font: normal 33px Georgia, serif; margin: 0; padding: 0; line-height: 33px;}.header p {color: #dfa575; font: normal 11px Georgia, serif; margin: 0; padding: 0; line-height: 11px; letter-spacing: 2px}.content h2 {color:#8598a3 !important; font-weight: normal; margin: 0; padding: 0; font-style: italic; line-height: 30px; font-size: 30px;font-family: Georgia, serif; }.content p {color:#767676; font-weight: normal; margin: 0; padding: 0; line-height: 20px; font-size: 12px;font-family: Georgia, serif;}.content a {color: #d18648; text-decoration: none;}.footer p {padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;}.footer a {color: #f7a766; text-decoration: none;}</style></head><body><table cellpadding='0' cellspacing='0' border='1'><tr><td ><table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif;' class='header'><tr><td bgcolor='#16a085' height='70' align='center'><h1 style='color: #fff; font: normal 25px Verdana; margin: 0; padding: 0; line-height: 33px;'>Infinity Invoice</h1></td></tr><tr><td style='font-size: 1px; height: 5px; line-height: 1px;' height='5'>&nbsp;</td></tr></table>";

            string Footer = "<table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif; line-height: 10px; margin-top:30px;' bgcolor='#16a085' class='footer'><tr><td bgcolor='#16a085'  align='center' style='padding: 15px 0 10px; font-size: 11px; color:#fff; margin: 0; line-height: 1.2;font-family: Verdana;' valign='top'><p style='padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;'>!!! This is software generated e-mail...Please do not reply. !!</p></td></tr> </table></td></tr></table></body></html>";
            StringBuilder htmlBody_Service = new StringBuilder();
            DataTable DTClientDetails = GetSummaryReportInvoice(Groupname, BillingPeriod);
            DataTable dtClientInfo = GetClientDetails(Groupname, BillingPeriod);
            DataTable DtInvoice = new bllTracking().GetSummaryReportAttachments(Groupname, BillingPeriod);
            // string ClientEmail = Convert.ToString(DTClientDetails.Rows[0]["EmailID"]);
            string clientName = "";
            if (dtClientInfo != null)
            {
                if (dtClientInfo.Rows.Count > 0)
                {
                    clientName = Convert.ToString(dtClientInfo.Rows[0]["PAI_Contact_Person"]);
                }
            }
            try
            {
                clientName = clientName.Substring(0, clientName.IndexOf(" "));
            }
            catch { }
            htmlBody_Service.Append(Header);
            htmlBody_Service.Append("<table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\"><b>Hello " + clientName + ",</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Good Morning!! <br /><br />Please find attached invoice# " + Convert.ToString(DTClientDetails.Rows[0]["InvNo"]) + " for your review.");
            htmlBody_Service.Append("<br /><br />Kindly, For all invoices related queries direct email to following.");
            //htmlBody_Service.Append("<br /><br />Just a note, regarding invoice related queries, please deal with only myself and Mahesh");
            htmlBody_Service.Append("<br /><br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a>");
            htmlBody_Service.Append("<br /><a href='mailTo:jim@Infinity-data.com'>jim@Infinity-data.com</a>");
            htmlBody_Service.Append("</td></tr></table><br /><br /><br /><br /><table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\">Thanks,<br />Anita Londhe<br />VP Controller<br />Infinity IPS<br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a><br /><a href='www.infinity-data.com'>www.infinity-data.com</a></td></tr> </table>");

            htmlBody_Service.Append("<table width=\"650px\"><tr><td align=\"left\"></td></tr></table>");
            htmlBody_Service.Append("<br /><span style='color:red; font-size:14px;'>**WE DO NOT ACCEPT OR REQUEST CHANGES TO WIRING INSTRUCTION VIA EMAIL - Always call to verify**</span>");
            htmlBody_Service.Append("<br /><span>**********************************************************************************************************</span>");
            // htmlBody_Service.Append("<br /><br /><span style='font-size:11px;'>Disclaimer: The information contained in this e-mail and any attachments may be confidential or privileged under applicable law, or otherwise may be protected from disclosure to anyone other than the intended recipient(s). Any use, distribution, or copying of this e-mail, including any of its contents or attachments by any person other than the intended recipient, or for any purpose other than its intended use, is strictly prohibited. If you believe you have received this e-mail in error, please notify us by e-mail and permanently delete the e-mail and any attachments, and do not save, copy, disclose, or reply on any part of the information contained in this e-mail or its attachments</span>");

            htmlBody_Service.Append(Footer);
            dvEmailTemplate.InnerHtml = htmlBody_Service.ToString();
            lblpath.InnerText = Server.MapPath("~/Reports/Freight/711.rpt");
        }
        public DataTable GetCostingDetailsFTE(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetFTEProjectCosting");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        public DataSet GetTotalProjectAmount_FTE(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectCost3_HighestFirst_FTE]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
        }
        public DataSet GetFTEWeekendHours(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetFTEWeekendHours]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
        }
        public void Time_To_Show_The_DetailedReport_183002(string ProjectName, string BillingPeriod, string ProjectID, string InvoiceId)
        {
            string InvoiceNumber = "";
            try
            {
                DataSet dt = new DataSet();
                DataTable dtNew = GetInvoiceNumber(ProjectName, BillingPeriod);
                if (dtNew != null)
                {
                    if (dtNew.Rows.Count > 0)
                    {
                        InvoiceNumber = Convert.ToString(dtNew.Rows[0]["InvoiceNumber"]);
                    }
                    else
                    {
                        InvoiceNumber = "";
                    }
                }
                else
                {
                    InvoiceNumber = "";
                }
                ReportDocument rpt = new ReportDocument();


                if (ProjectName == "861-007")
                    rpt.Load(Server.MapPath("~/Reports/Others/861-007.rpt"));
                else if (ProjectName == "771")
                    rpt.Load(Server.MapPath("~/771.rpt"));
                else if (ProjectName == "711")
                    rpt.Load(Server.MapPath("~/Reports/Freight/711.rpt"));
                else if (ProjectName == "722")
                    rpt.Load(Server.MapPath("~/Reports/Freight/722.rpt"));
                else if (ProjectName == "712")
                    rpt.Load(Server.MapPath("~/Reports/Freight/712.rpt"));
                else if (ProjectName == "791")
                    rpt.Load(Server.MapPath("~/Reports/Freight/791.rpt"));
                else if (ProjectName == "733-002" || ProjectName == "772" || ProjectName == "736" || ProjectName == "791-002" || ProjectName == "409-002" || ProjectName == "409-005" || ProjectName == "572")
                    rpt.Load(Server.MapPath("~/Reports/Freight/733-002.rpt"));
                else if (ProjectName == "736-002")
                    rpt.Load(Server.MapPath("~/Reports/Freight/733-002.rpt"));
                else if (ProjectName == "754")
                    rpt.Load(Server.MapPath("~/Reports/Freight/754.rpt"));
                else if (ProjectName == "757-003")
                    rpt.Load(Server.MapPath("~/757-003.rpt"));
                else if (ProjectName == "694-008" || ProjectName == "694-005" || ProjectName == "715")
                    rpt.Load(Server.MapPath("~/Reports/Title_Other/694-008.rpt"));
                else
                    rpt.Load(Server.MapPath("~/Reports/Commitment/183-002.rpt"));

                //if (ProjectName == "861-007")
                //    rpt.Load(Server.MapPath("~/861007.rpt"));
                //else if (ProjectName == "771")
                //    rpt.Load(Server.MapPath("~/771.rpt"));
                //else if (ProjectName == "711")
                //    rpt.Load(Server.MapPath("~/711_Preview.rpt"));
                //else if (ProjectName == "722")
                //    rpt.Load(Server.MapPath("~/722.rpt"));
                //else if (ProjectName == "712")
                //    rpt.Load(Server.MapPath("~/712.rpt"));
                //else if (ProjectName == "791")
                //    rpt.Load(Server.MapPath("~/791.rpt"));
                //else if (ProjectName == "733-002" || ProjectName == "772" || ProjectName == "736" || ProjectName == "791-002" || ProjectName == "409-002" || ProjectName == "409-005" || ProjectName == "572")
                //    rpt.Load(Server.MapPath("~/733-002.rpt"));
                //else if (ProjectName == "736-002")
                //    rpt.Load(Server.MapPath("~/736-002.rpt"));
                //else if (ProjectName == "754")
                //    rpt.Load(Server.MapPath("~/754.rpt"));
                //else if (ProjectName == "757-003")
                //    rpt.Load(Server.MapPath("~/757-003.rpt"));
                //else if (ProjectName == "694-008" || ProjectName == "694-005")
                //    rpt.Load(Server.MapPath("~/694-008.rpt"));
                //else
                //    rpt.Load(Server.MapPath("~/183-002.rpt"));
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = ProjectName;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@GroupName"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = ProjectID;
                pval2.Add(pdisval2);

                ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                pdisval3.Value = BillingPeriod;
                pval3.Add(pdisval3);

                if (ProjectName != "712" && ProjectName != "757-003" && ProjectName != "791")
                    rpt.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval2);
                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval3);

                // rpt.SetDataSource(dt);
                //rpt.Subreports[0].SetDataSource(dt.Tables[0]);
                CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;
                CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos;
                CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo;
                CrystalDecisions.CrystalReports.Engine.Tables CrTables;
                crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
                crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();

                crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"];
                crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserID"];
                crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];

                CrTables = rpt.Database.Tables;

                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }



                string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                string strTime = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string filename = ProjectID + "_" + BillingPeriod + "_" + strDate + strTime;
                if (InvoiceNumber != "")
                {
                    filename = InvoiceNumber.Replace(",", "_");
                }
                else
                {
                    filename = filename + "_" + BillingPeriod + "_" + strDate + strTime;
                }
                //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);

                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath = Server.MapPath("~/BillingDocuments/") + filename + ".pdf";

                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                //int result = blltracking.UpdateInvoicePath(int.Parse(InvoiceId), Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"));
                int result = new bllTracking().InsertGroupAttachmentPath_QC(ProjectName, BillingPeriod, Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"), InvoiceNumber);

                // rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);
            }
            catch (Exception ex) { throw ex; }

        }
        public int UpdateInvoiceAutoManual(string GroupName, string BillingPeriod, string InvoiceNumber)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_UpdateInvoiceAutoManual");
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupName", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, GroupName);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, BillingPeriod);
            SQLHelper.AddParamToSQLCmd(cmd, "@InvoiceNumber", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, InvoiceNumber);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            cmd.Dispose();
            return ReturnValue;
        }

        public DataTable GetCrystalReport(int ProjectID, string BillingPeriod, string DomainId)
        {
            DataTable dt = null;
            string procedureName = "";
            if (DomainId == "Title-Typing")
            {
                DataTable dtBase = GetBillingBase(ProjectID);
                if (dtBase != null)
                {
                    if (dtBase.Rows.Count > 0)
                    {
                        string BillingBase = Convert.ToString(dtBase.Rows[0]["BAseType"]);
                        if (ProjectID == 248)
                            procedureName = "usp_GetExcel694";
                        else if (BillingBase == "Base Rate")
                            procedureName = "usp_GetExcelBaseRate";
                        else if (BillingBase == "Product Type")
                            procedureName = "usp_GetExcelProductType";
                        else if (BillingBase == "Order Type")
                            procedureName = "usp_GetExcelOrderType";
                    }
                }

            }
            else if (DomainId == "Freight")
            {
                if (ProjectID == 137)
                    procedureName = "usp_GetProjectCost3_HighestFirst_ForExcel_711";
                else if (ProjectID == 47)
                    procedureName = "usp_GetProjectCost3_HighestFirst_ForExcel_733-002";
                else
                    procedureName = "usp_GetProjectCost3_HighestFirst_ForExcel_183_FR";
            }
            else
            {
                if (ProjectID == 137)
                    procedureName = "usp_GetProjectCost3_HighestFirst_ForExcel_711";
                else if (ProjectID == 47)
                    procedureName = "usp_GetProjectCost3_HighestFirst_ForExcel_733-002";
                else
                    procedureName = "usp_GetProjectCost3_HighestFirst_ForExcel_183";
            }

            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, procedureName);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            string GroupName = Session["GroupName"].ToString();
            string billingPeriod = Session["billingPeriod"].ToString();
            string ProjectID = Session["ProjectID"].ToString();
            string InvoiceId = Session["InvoiceID"].ToString();
            // int UpdateValue = UpdateInvoiceAutoManual(GroupName, billingPeriod, Convert.ToString(lblInvoiceNo.Text));
            //  string InvId = Session["InvoiceIdEmailSent"].ToString();
            string strClientid = Convert.ToString(hfClient.Value); //ddlClientList.SelectedItem.Value;
            if (strClientid == "") { strClientid = "0"; }
            int result = UpdateInvoiceClient_OtherDomain(GroupName, billingPeriod);
            //Time_To_Show_The_SummaryReport();

            if (int.Parse(Convert.ToString(Session["ProjectID"])) == 391)
            {
                Time_To_Show_The_DetailedReport_183002(GroupName, billingPeriod, ProjectID, InvoiceId);
                SendInvoiceCreationEmailForSummaryReport(GroupName, billingPeriod, 0);
            }
            else if (int.Parse(Convert.ToString(Session["ProjectID"])) == 203 || int.Parse(Convert.ToString(Session["ProjectID"])) == 400 || int.Parse(Convert.ToString(Session["ProjectID"])) == 373 || int.Parse(Convert.ToString(Session["ProjectID"])) == 385 || int.Parse(Convert.ToString(Session["ProjectID"])) == 87)
            {
                BindFTEReport(GroupName, billingPeriod, ProjectID);
                SendInvoiceCreationEmailForSummaryReport(GroupName, billingPeriod, 0);
            }
            else if (Convert.ToString(Session["DomainName"]) == "Title-Typing")
            {
                if (ProjectID == "11" || ProjectID == "13" || ProjectID == "154" || ProjectID == "175" || ProjectID == "177" || ProjectID == "248" || ProjectID == "331" || ProjectID == "370" || ProjectID == "395" || ProjectID == "414" || ProjectID == "421")
                {
                    SendInvoiceCreationEmailForSummaryReport(GroupName, billingPeriod, 0);
                }
                else
                {
                    string ExcelName = GetTypingInvoice(GroupName, billingPeriod, ProjectID, InvoiceId);
                    SendInvoiceCreationEmailForSummaryReport(GroupName, billingPeriod, 0);
                    //SendInvoiceCreationEmailForSummaryReportWithExcel(GroupName, billingPeriod, 0, ExcelName);
                }
            }
            else if (Convert.ToString(Session["DomainName"]) == "FTE" || Convert.ToString(Session["DomainName"]) == "Freight-706" || Convert.ToString(Session["DomainName"]) == "Title-FTE" || Convert.ToString(Session["DomainName"]) == "Vendor Management" || Convert.ToString(Session["DomainName"]) == "Title-Other")
            {
                if (int.Parse(Convert.ToString(Session["ProjectID"])) == 391 || int.Parse(Convert.ToString(Session["ProjectID"])) == 331 || int.Parse(Convert.ToString(Session["ProjectID"])) == 337 || int.Parse(Convert.ToString(Session["ProjectID"])) == 352 || int.Parse(Convert.ToString(Session["ProjectID"])) == 395 || int.Parse(Convert.ToString(Session["ProjectID"])) == 414 || int.Parse(Convert.ToString(Session["ProjectID"])) == 456)
                {
                    Time_To_Show_The_DetailedReport_183002(GroupName, billingPeriod, ProjectID, InvoiceId);
                    SendInvoiceCreationEmailForSummaryReport(GroupName, billingPeriod, 0);
                }
                else
                {
                    BindFTEReport(GroupName, billingPeriod, ProjectID);
                    SendInvoiceCreationEmailForSummaryReport(GroupName, billingPeriod, 0);
                }
            }
            else
            {
                Time_To_Show_The_DetailedReport_183002(GroupName, billingPeriod, ProjectID, InvoiceId);
                SendInvoiceCreationEmailForSummaryReport(GroupName, billingPeriod, 0);
            }

            // string ClientId = "1";
            hfClient.Value = "";
            //BindClients(int.Parse(InvId));

            dvError.Style.Add("display", "");
            dvError.Attributes.Add("class", "alert alert-success background-success");
            dvError.InnerHtml = "Email sent successfully!";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            RegisterModalClose("modalSendInvoice");
            BindGrid();
        }

        public string GetFTEHours(int ProjectID, string Employee, string Process, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[GetFTEHours]");//IB_usp_UpdateProjectApprovedforClient
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@Employee", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, Employee);
            SQLHelper.AddParamToSQLCmd(cmd, "@Process", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Process);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);

            string ReturnValue = Convert.ToString(SQLHelper.ExecuteScalarCmdBilling(cmd));
            return ReturnValue;
        }

        public DataTable GetBillingBase(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_BillingBase");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        public string GetTypingInvoice(string ProjectName, string BillingPeriod, string ProjectID, string InvoiceId)
        {
            string InvoiceNumber = "";
            string filePath = "";
            string filename = "";
            string filePathExcel = "";
            try
            {
                DataSet dt = new DataSet();
                DataTable dtBase = GetBillingBase(int.Parse(ProjectID));
                if (dtBase != null)
                {
                    if (dtBase.Rows.Count > 0)
                    {
                        string BillingBase = Convert.ToString(dtBase.Rows[0]["BAseType"]);
                        DataTable dtNew = GetInvoiceNumber(ProjectName, BillingPeriod);
                        if (dtNew != null)
                        {
                            if (dtNew.Rows.Count > 0)
                            {
                                InvoiceNumber = Convert.ToString(dtNew.Rows[0]["InvoiceNumber"]);
                            }
                            else
                            {
                                InvoiceNumber = "";
                            }
                        }
                        else
                        {
                            InvoiceNumber = "";
                        }
                        ReportDocument rpt = new ReportDocument();
                        if (int.Parse(ProjectID) == 13)
                            rpt.Load(Server.MapPath("~/Reports/Commitment/888.rpt"));
                        if (int.Parse(ProjectID) == 11 || int.Parse(ProjectID) == 154 || int.Parse(ProjectID) == 453 || int.Parse(ProjectID) == 177 || int.Parse(ProjectID) == 175 || int.Parse(ProjectID) == 370 || int.Parse(ProjectID) == 455 || int.Parse(ProjectID) == 421 || int.Parse(ProjectID) == 454 || int.Parse(ProjectID) == 474)
                            rpt.Load(Server.MapPath("~/Reports/Commitment/364.rpt"));
                        else if (int.Parse(ProjectID) == 248)
                            rpt.Load(Server.MapPath("~/Reports/Commitment/694.rpt"));
                        else if (int.Parse(ProjectID) == 482)
                            rpt.Load(Server.MapPath("~/Reports/Commitment/694-010.rpt"));
                        else if (BillingBase == "Base Rate")
                            rpt.Load(Server.MapPath("~/Reports/Commitment/BaseRateSummary.rpt"));
                        else if (BillingBase == "Product Type")
                            rpt.Load(Server.MapPath("~/Reports/Commitment/ProductTypeSummary.rpt"));
                        else if (BillingBase == "Order Type")
                            rpt.Load(Server.MapPath("~/Reports/Commitment/OrderTypeSummary.rpt"));

                        CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                        ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                        pdisval1.Value = ProjectName;
                        pval1.Add(pdisval1);

                        rpt.DataDefinition.ParameterFields["@GroupName"].ApplyCurrentValues(pval1);

                        CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                        CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                        ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                        pdisval2.Value = ProjectID;
                        pval2.Add(pdisval2);

                        ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                        pdisval3.Value = BillingPeriod;
                        pval3.Add(pdisval3);


                        rpt.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval2);
                        rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval3);

                        CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                        CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;
                        CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos;
                        CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo;
                        CrystalDecisions.CrystalReports.Engine.Tables CrTables;
                        crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                        crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
                        crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();

                        crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"];
                        crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                        crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserID"];
                        crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];

                        CrTables = rpt.Database.Tables;

                        foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                        {
                            crtableLogoninfo = CrTable.LogOnInfo;
                            crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                            CrTable.ApplyLogOnInfo(crtableLogoninfo);
                        }


                        string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                        string strTime = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                        filename = ProjectID + "_" + BillingPeriod + "_" + strDate + strTime;
                        if (InvoiceNumber != "")
                        {
                            filename = InvoiceNumber.Replace(",", "_");
                        }
                        else
                        {
                            filename = filename + "_" + BillingPeriod + "_" + strDate + strTime;
                        }

                        if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                        {
                            Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                        }
                        filePath = Server.MapPath("~/BillingDocuments/") + filename + ".pdf";
                        filePathExcel = filePath.Replace(".pdf", ".xls");
                        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                        int result = new bllTracking().InsertGroupAttachmentPath_QC(ProjectName, BillingPeriod, Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"), InvoiceNumber);

                        DataTable dtExcel = GetCrystalReport(int.Parse(ProjectID), BillingPeriod, Convert.ToString(Session["DomainName"]));
                        filePathExcel = filePath.Replace(".pdf", ".xls");
                        SaveDataTableAsExcelHtml(dtExcel, filePathExcel, ProjectName + "_" + BillingPeriod);
                    }
                }
            }
            catch (Exception ex) { return filePathExcel; }
            return filePathExcel;

        }
        public string GetBillableHours_FTE(int ProjectID)
        {
            string BillableHours = "";
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetBillableHours]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            BillableHours = Convert.ToString(SQLHelper.ExecuteScalarCmdBilling(cmd));
            //DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return BillableHours;
        }
        public void BindFTEReport(string ProjectName, string BillingPeriod, string ProjectID)
        {
            string InvoiceNumber = "";
            DataTable dtDyn = new DataTable();
            dtDyn.Columns.Add("Description");
            dtDyn.Columns.Add("Total # of Hours");
            dtDyn.Columns.Add("Rate in USD");
            dtDyn.Columns.Add("Total Charges in US $");
            dtDyn.Columns.Add("Before Decimal");
            dtDyn.Columns.Add("After Decimal");
            DataSet dtAmt = GetTotalProjectAmount_FTE(int.Parse(ProjectID), BillingPeriod);
            string BillableHours = GetBillableHours_FTE(int.Parse(ProjectID));
            BillableHours = BillableHours == "" ? "0" : BillableHours;
            if (int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400)
            {
                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                if (dtRate != null)
                {
                    if (dtRate.Rows.Count > 0)
                    {
                        DataRow dr = dtDyn.NewRow();
                        DataTable dtTitle = dtAmt.Tables[0].Copy();
                        DataTable dt1 = dtTitle.DefaultView.ToTable(true, "Auditor1 (Hours)");
                        object MonthCont = dtTitle.AsEnumerable()
                                          .Count(r => r.Field<string>("Auditor1 (Hours)") != "Holiday" && r.Field<string>("Auditor1 (Hours)") != "");
                        decimal Average = Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont);
                        decimal TotalHours1 = 0;
                        if (int.Parse(ProjectID) == 400)
                        {
                            if (BillingPeriod == "01-Sep-2022 ~ 30-Sep-2022")
                                TotalHours1 = 972;
                            else
                                TotalHours1 = Convert.ToDecimal(6) * Convert.ToDecimal(Average);
                        }
                        //TotalHours1 = Convert.ToDecimal(3) * Convert.ToDecimal(Average);
                        else
                        {
                            if (BillingPeriod == "01-Mar-2022 ~ 31-Mar-2022")
                                TotalHours1 = 738;
                            else if (BillingPeriod == "01-Sep-2022 ~ 30-Sep-2022")
                                TotalHours1 = 1080;
                            else
                                TotalHours1 = Convert.ToDecimal(6) * Convert.ToDecimal(Average);
                        }
                        string TotalFTEHours = TotalHours1.ToString();
                        //decimal TotalHours2 = Convert.ToDecimal(2) * Convert.ToDecimal(Average);
                        string[] Period = BillingPeriod.Split('~');
                        string From = Period[0].Trim();
                        string To = Period[1].Trim();
                        object RowCount = dtTitle.AsEnumerable()
                          .Count(r => r.Field<string>("Auditor1 (Hours)") != "");
                        if (int.Parse(ProjectID) == 400)
                            dr["Description"] = "6 FTE charges between " + From + " to " + To + "";
                        else
                            dr["Description"] = "6 FTE charges between " + From + " to " + To + "";
                        dr["Total # of Hours"] = Convert.ToString(Convert.ToDecimal(TotalHours1));
                        // sumAmount = "5258.21";
                        try
                        {
                            dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                            dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]), 2));
                            int Before = (int)(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                            dr["Before Decimal"] = Before.ToString();
                            dr["After Decimal"] = Math.Round((Convert.ToDecimal(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"])) - Before) * 100, 0).ToString();
                        }

                        catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                        dtDyn.Rows.Add(dr);
                    }
                }
            }
            else if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 337 || int.Parse(ProjectID) == 393)
            {
                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                if (dtRate != null)
                {
                    if (dtRate.Rows.Count > 0)
                    {
                        DataRow dr = dtDyn.NewRow();
                        decimal Rate = Convert.ToDecimal(dtRate.Rows[0]["Rate"]);
                        DataTable dtTitle = dtAmt.Tables[0].Copy();
                        if (int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 386)
                        {
                        }
                        else
                        {
                            dtTitle.Columns.Add("No Of Hours");
                        }
                        dtTitle.Columns.Add("Price");
                        dtTitle.Columns.Add("Total Charges");
                        dtTitle.Columns.Add("TrackingSheetID");
                        int PCount = 0;
                        int ColCount = dtTitle.Columns.Count;
                        if (int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 386)
                        {
                            int FTCount = dtTitle.Rows.Count;
                            for (int i = 0; i < dtTitle.Rows.Count; i++)
                            {
                                dtTitle.Rows[i]["Total Charges"] = Convert.ToString(Convert.ToDecimal(Rate) * Convert.ToDecimal(FTCount));
                                dtTitle.Rows[i]["Price"] = Rate;
                                dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < dtTitle.Rows.Count; i++)
                            {
                                PCount = 0;
                                for (int j = 2; j < ColCount; j++)
                                {
                                    if (Convert.ToString(dtTitle.Rows[i][j]) == "P")
                                        PCount++;
                                }
                                if (int.Parse(ProjectID) == 123)
                                {
                                    dtTitle.Rows[i]["Price"] = Rate;
                                    //if ((Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) > 80)
                                    //{
                                    //    dtTitle.Rows[i]["No Of Hours"] = "80";
                                    //    dtTitle.Rows[i]["Total Charges"] = 80 * Rate;
                                    //}
                                    //else if ((Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) < 71)
                                    //{
                                    //    dtTitle.Rows[i]["No Of Hours"] = "71";
                                    //    dtTitle.Rows[i]["Total Charges"] = 71 * Rate;
                                    //}
                                    //else
                                    //{
                                    //    dtTitle.Rows[i]["No Of Hours"] = Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount);
                                    //    dtTitle.Rows[i]["Total Charges"] = (Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) * Rate;
                                    //}
                                    if ((Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) > 80)
                                    {
                                        dtTitle.Rows[i]["No Of Hours"] = "80";
                                        dtTitle.Rows[i]["Total Charges"] = 80 * Rate;
                                    }
                                    else
                                    {
                                        string NoOfHours = "";
                                        try
                                        {
                                            NoOfHours = GetFTEHours(int.Parse(ProjectID), Convert.ToString(dtTitle.Rows[i]["Employee Name"]), Convert.ToString(dtTitle.Rows[i]["Process"]), Convert.ToString(BillingPeriod));
                                        }
                                        catch { }
                                        if (NoOfHours != "" && NoOfHours != "0")
                                        {
                                            dtTitle.Rows[i]["No Of Hours"] = NoOfHours;
                                            dtTitle.Rows[i]["Total Charges"] = Convert.ToDecimal(NoOfHours) * Rate;
                                        }
                                        else
                                        {
                                            dtTitle.Rows[i]["No Of Hours"] = Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount);
                                            dtTitle.Rows[i]["Total Charges"] = (Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) * Rate;
                                        }
                                    }
                                    dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                }
                                else
                                {
                                    string NoOfHours = "";
                                    try
                                    {
                                        NoOfHours = GetFTEHours(int.Parse(ProjectID), Convert.ToString(dtTitle.Rows[i]["Employee Name"]), Convert.ToString(dtTitle.Rows[i]["Process"]), Convert.ToString(BillingPeriod));
                                    }
                                    catch { }
                                    if (NoOfHours != "" && NoOfHours != "0")
                                    {
                                        dtTitle.Rows[i]["No Of Hours"] = NoOfHours;
                                        dtTitle.Rows[i]["Total Charges"] = Convert.ToDecimal(NoOfHours) * Rate;
                                    }
                                    else
                                    {
                                        dtTitle.Rows[i]["No Of Hours"] = Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount);
                                        dtTitle.Rows[i]["Total Charges"] = (Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) * Rate;
                                    }
                                    //dtTitle.Rows[i]["No Of Hours"] = Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount);
                                    dtTitle.Rows[i]["Price"] = Rate;
                                    // dtTitle.Rows[i]["Total Charges"] = (Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) * Rate;
                                    dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                }
                                //dtTitle.Rows[i]["No Of Hours"] = Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount);
                                //dtTitle.Rows[i]["Price"] = Rate;
                                //dtTitle.Rows[i]["Total Charges"] = (Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) * Rate;
                                //dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                //dtTitle.Rows.Add(dr);
                            }
                        }
                        string[] Period = BillingPeriod.Split('~');
                        string From = Period[0].Trim();
                        string To = Period[1].Trim();
                        if (int.Parse(ProjectID) == 354)
                        {
                            Object NoofFTEBO = dtTitle.AsEnumerable()
                                             .Where(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Back Office FTE")
                                             .Sum(x => Convert.ToDecimal(x["No Of Hours"])).ToString();
                            Object NoofFTEVM = dtTitle.AsEnumerable()
                              .Where(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Vendor Management")
                              .Sum(x => Convert.ToDecimal(x["No Of Hours"])).ToString();
                            object sumAmountBO = dtTitle.AsEnumerable()
                              .Where(r => r.Field<string>("Total Charges") != "" && r.Field<string>("Process") == "Back Office FTE")
                              .Sum(x => Convert.ToDecimal(x["Total Charges"])).ToString();

                            Object CountBO = dtTitle.AsEnumerable()
                              .Count(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Back Office FTE");
                            Object CountVM = dtTitle.AsEnumerable()
                              .Count(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Vendor Management");
                            object sumAmountVm = dtTitle.AsEnumerable()
                              .Where(r => r.Field<string>("Total Charges") != "" && r.Field<string>("Process") == "Vendor Management")
                              .Sum(x => Convert.ToDecimal(x["Total Charges"])).ToString();

                            dr["Description"] = CountBO.ToString() + " FTE charges between " + From + " to " + To + " (Back-Office)";
                            dr["Total # of Hours"] = NoofFTEBO.ToString();
                            try
                            {
                                dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                                dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(sumAmountBO), 2));
                                dr["Before Decimal"] = Convert.ToInt32(Convert.ToDecimal(sumAmountBO)).ToString();
                                dr["After Decimal"] = Math.Round(((Convert.ToDecimal(sumAmountBO) - Convert.ToInt32(Convert.ToDecimal(sumAmountBO))) * 100), 0).ToString();
                            }

                            catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                            dtDyn.Rows.Add(dr);

                            dr = dtDyn.NewRow();
                            dr["Description"] = CountVM.ToString() + " FTE charges between " + From + " to " + To + " (VM Process)";
                            dr["Total # of Hours"] = NoofFTEVM.ToString();
                            try
                            {
                                dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                                dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(sumAmountVm), 2));
                                dr["Before Decimal"] = Convert.ToInt32(Convert.ToDecimal(sumAmountVm)).ToString();
                                dr["After Decimal"] = Math.Round(((Convert.ToDecimal(sumAmountVm) - Convert.ToInt32(Convert.ToDecimal(sumAmountVm))) * 100), 0).ToString();
                            }

                            catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                            dtDyn.Rows.Add(dr);
                            try
                            {
                                object TotalAmountDec = dtDyn.AsEnumerable()
                                              .Where(r => r.Field<string>("Total Charges in US $") != "")
                                              .Sum(x => Convert.ToDecimal(x["Total Charges in US $"]));
                                int BeforeDec = (int)Convert.ToDecimal(TotalAmountDec);
                                decimal AfterDec = Math.Round((Convert.ToDecimal(TotalAmountDec) - BeforeDec) * 100, 0);

                                for (int i = 1; i < dtDyn.Rows.Count; i++)
                                {
                                    dtDyn.Rows[i]["Before Decimal"] = BeforeDec.ToString();
                                    dtDyn.Rows[i]["After Decimal"] = AfterDec.ToString();
                                }
                            }
                            catch { }
                        }
                        else if (int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 386)
                        {
                            object RowCount = dtTitle.AsEnumerable()
                          .Count(r => r.Field<string>("Total Charges") != "");
                            object sumAmount = dtTitle.Rows[0]["Total Charges"];
                            //.AsEnumerable()
                            //  .Where(r => r.Field<string>("Total Charges") != "")
                            //  .Sum(x => Convert.ToDecimal(x["Total Charges"])).ToString();

                            dr["Description"] = RowCount.ToString() + " FTE charges between " + From + " to " + To + "";
                            // sumAmount = "5258.21";
                            try
                            {
                                dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                                dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(sumAmount), 2));
                                dr["Before Decimal"] = Convert.ToInt32(Convert.ToDecimal(sumAmount)).ToString();
                                dr["After Decimal"] = Math.Round(((Convert.ToDecimal(sumAmount) - Convert.ToInt32(Convert.ToDecimal(sumAmount))) * 100), 0).ToString();
                            }

                            catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                            dtDyn.Rows.Add(dr);
                        }
                        else
                        {
                            object RowCount = dtTitle.AsEnumerable()
                              .Count(r => r.Field<string>("Total Charges") != "");
                            object sumAmount = dtTitle.AsEnumerable()
                              .Where(r => r.Field<string>("Total Charges") != "")
                              .Sum(x => Convert.ToDecimal(x["Total Charges"])).ToString();
                            object NoOfHours = dtTitle.AsEnumerable()
                              .Where(r => r.Field<string>("No Of Hours") != "")
                              .Sum(x => Convert.ToDecimal(x["No Of Hours"])).ToString();
                            if (int.Parse(ProjectID) == 353)
                                dr["Description"] = RowCount.ToString() + " FTE charges between " + From + " to " + To + " (Package Review)";
                            else
                                dr["Description"] = RowCount.ToString() + " FTE charges between " + From + " to " + To + "";
                            dr["Total # of Hours"] = Convert.ToString(Convert.ToDecimal(NoOfHours));
                            // sumAmount = "5258.21";
                            try
                            {
                                dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                                dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(sumAmount), 2));
                                dr["Before Decimal"] = Convert.ToInt32(Convert.ToDecimal(sumAmount)).ToString();
                                dr["After Decimal"] = Math.Round(((Convert.ToDecimal(sumAmount) - Convert.ToInt32(Convert.ToDecimal(sumAmount))) * 100), 0).ToString();
                            }

                            catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                            dtDyn.Rows.Add(dr);
                        }
                    }
                }
            }
            else if (int.Parse(ProjectID) == 87)
            {
                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                if (dtRate != null)
                {
                    if (dtRate.Rows.Count > 0)
                    {
                        decimal Rate = Convert.ToDecimal(dtRate.Rows[0]["Rate"]);
                        dtDyn.Columns.Clear();
                        dtDyn.Columns.Add("Description");
                        dtDyn.Columns.Add("Total # of Invoices");
                        dtDyn.Columns.Add("Total # of Hours");
                        dtDyn.Columns.Add("Rate/Hour");
                        dtDyn.Columns.Add("Total Charges in US $");
                        dtDyn.Columns.Add("Before Decimal");
                        dtDyn.Columns.Add("After Decimal");

                        object InvoiceCount = dtAmt.Tables[0].AsEnumerable()
                              .Where(r => r.Field<string>("# of Invoices") != "Holiday" && r.Field<string>("# of Invoices") != "" && r.Field<string>("# of Invoices") != null)
                              .Sum(x => Convert.ToDecimal(x["# of Invoices"]));
                        object TimeSpentMins = dtAmt.Tables[0].AsEnumerable()
                                      .Where(r => r.Field<string>("Time Spent (Mins)") != "Holiday" && r.Field<string>("Time Spent (Mins)") != "" && r.Field<string>("Time Spent (Mins)") != null)
                                      .Sum(x => Convert.ToDecimal(x["Time Spent (Mins)"]));
                        object TimeSpentHrs = dtAmt.Tables[0].AsEnumerable()
                                      .Where(r => r.Field<string>("Total Time Spent (Hrs)") != "Holiday" && r.Field<string>("Total Time Spent (Hrs)") != "" && r.Field<string>("Total Time Spent (Hrs)") != null)
                                      .Sum(x => Convert.ToDecimal(x["Total Time Spent (Hrs)"]));
                        DataRow dr = dtDyn.NewRow();
                        decimal TotalCharges = 0;
                        dr["Description"] = "Invoice for Audit invoices";
                        dr["Total # of Invoices"] = Convert.ToString(InvoiceCount);
                        dr["Total # of Hours"] = Convert.ToString(Math.Round(Convert.ToDecimal(TimeSpentHrs), 2));
                        try
                        {
                            dr["Rate/Hour"] = Math.Round(Convert.ToDecimal(dtRate.Rows[0]["Rate"]), 2);
                            TotalCharges = Math.Round(Convert.ToDecimal(TimeSpentHrs), 2) * Math.Round(Convert.ToDecimal(dtRate.Rows[0]["Rate"]), 2);
                            dr["Total Charges in US $"] = Convert.ToString(Math.Round(TotalCharges, 2));
                            dr["Before Decimal"] = Math.Truncate(TotalCharges).ToString();
                            dr["After Decimal"] = Math.Round(((TotalCharges - Math.Truncate(TotalCharges)) * 100), 0).ToString();
                        }

                        catch { dr["Rate/Hour"] = 0; dr["Total Charges in US $"] = 0; }
                        dtDyn.Rows.Add(dr);
                    }
                }
            }
            else if (int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 442)
            {
                string[] Period = BillingPeriod.Split('~');
                string From = Period[0].Trim();
                string To = Period[1].Trim();
                DataTable dt1 = dtAmt.Tables[0].DefaultView.ToTable(true, "Operator1(Hours)");

                object Cont = dt1.AsEnumerable()
                              .Count(r => r.Field<string>("Operator1(Hours)") != "Holiday");
                object MonthCont = dtAmt.Tables[0].AsEnumerable()
                              .Count(r => r.Field<string>("Operator1(Hours)") != "Holiday");
                DataRow dr = dtDyn.NewRow();
                //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont));
                string TotalFTEHours = "";// Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont))));
                //if (int.Parse(ProjectID) == 373)
                //{
                if (int.Parse(ProjectID) == 442)
                    dr["Description"] = "Operator charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                else if (int.Parse(ProjectID) == 385)
                    dr["Description"] = "10 Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                else
                    dr["Description"] = "2 Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                TotalFTEHours = Convert.ToString(2 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                if (int.Parse(ProjectID) == 442)
                {
                    dr["Total # of Hours"] = Convert.ToString(1 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
                else if (int.Parse(ProjectID) == 385)
                {
                    if (BillingPeriod == "01-Jul-2022 ~ 31-Jul-2022")
                    {
                        dr["Total # of Hours"] = "1350";
                    }
                    else if (BillingPeriod == "01-Nov-2022 ~ 30-Nov-2022")
                    {
                        dr["Total # of Hours"] = "1773";
                    }
                    else
                    {
                        dr["Total # of Hours"] = Convert.ToString(8 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    }
                }
                else
                {
                    dr["Total # of Hours"] = Convert.ToString(2 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
                //}
                //else
                //{
                //    dr["Description"] = "4 Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                //    TotalFTEHours = Convert.ToString(4 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                //    dr["Total # of Hours"] = Convert.ToString(4 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                //}
                try
                {
                    DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                    if (dtRate != null)
                    {
                        if (dtRate.Rows.Count > 0)
                        {
                            dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                            dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]), 2));
                            int Before = (int)(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                            dr["Before Decimal"] = Before.ToString();
                            dr["After Decimal"] = Math.Round((Convert.ToDecimal(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"])) - Before) * 100, 0).ToString();
                        }
                    }
                }
                catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                dtDyn.Rows.Add(dr);
            }
            else
            {
                //dtDyn.Columns.Add("Rate");
                //DataSet dtAmt = GetTotalProjectAmount_FTE(int.Parse(ProjectID), BillingPeriod);
                DataTable dt1 = dtAmt.Tables[0].DefaultView.ToTable(true, "BilledFTE");

                object Cont = dt1.AsEnumerable()
                              .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                object MonthCont = dtAmt.Tables[0].AsEnumerable()
                              .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                object MonthlyAverage = dt1.AsEnumerable()
                              .Where(r => r.Field<string>("BilledFTE") != "Holiday")
                              .Sum(x => Convert.ToDecimal(x["BilledFTE"]));
                //tdProcess.InnerHtml = "Average Billed FTE";
                DataRow dr = dtDyn.NewRow();
                string[] Period = BillingPeriod.Split('~');
                string From = Period[0].Trim();
                string To = Period[1].Trim();
                decimal Average = Convert.ToDecimal(MonthlyAverage) / Convert.ToDecimal(Cont);
                if (int.Parse(ProjectID) == 205)
                    dr["Description"] = Math.Round(Average, 2).ToString() + " FTE charges towards data entry process for the period " + From + " to " + To;
                else if (int.Parse(ProjectID) == 184)
                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont)) + "/FTE Hours) Legacy and Global";
                else
                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                //dr["MonthlyAverage"] = Math.Round(Average, 2).ToString();
                //lblMontlyAverage.Text = Math.Round(Convert.ToDecimal(MonthlyAverage), 2).ToString();
                // dr["FTEHours"] = Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont));
                string TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                if (int.Parse(ProjectID) == 205)
                    TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                if (int.Parse(ProjectID) == 205)
                    dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                else
                    dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                //string TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont))));
                //dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont))));

                try
                {
                    DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                    if (dtRate != null)
                    {
                        if (dtRate.Rows.Count > 0)
                        {
                            dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                            dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]), 2));
                            int Before = (int)(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                            dr["Before Decimal"] = Before.ToString();
                            dr["After Decimal"] = Math.Round((Convert.ToDecimal(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"])) - Before) * 100, 0).ToString();
                        }
                    }
                }
                catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                dtDyn.Rows.Add(dr);
                if (int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 205)
                {
                    try
                    {
                        DataTable dt1Weekend1 = GetFTEWeekendHours(int.Parse(ProjectID), BillingPeriod).Tables[0].DefaultView.ToTable(true, "Date");
                        DataTable dt1Weekend = GetFTEWeekendHours(int.Parse(ProjectID), BillingPeriod).Tables[0].DefaultView.ToTable(true, "BilledFTE");
                        if (dt1Weekend != null)
                        {
                            if (dt1Weekend.Rows.Count > 0)
                            {
                                object ContWeekend = dt1Weekend.AsEnumerable()
                                             .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                                object MonthContWeekend = dt1Weekend.AsEnumerable()
                                              .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                                object MonthlyAverageWeekend = dt1Weekend.AsEnumerable()
                                              .Where(r => r.Field<string>("BilledFTE") != "Holiday")
                                              .Sum(x => Convert.ToDecimal(x["BilledFTE"]));

                                //dtDyn.Rows.Add(dr);
                                dr = dtDyn.NewRow();
                                Average = Convert.ToDecimal(MonthlyAverageWeekend) / Convert.ToDecimal(ContWeekend);
                                if (int.Parse(ProjectID) == 205)
                                    dr["Description"] = Math.Round(Average, 2).ToString() + " FTE charges towards data entry process for the period " + From + " to " + To + " - Weekend";
                                else
                                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthContWeekend)) + "/FTE Hours) Legacy and Global - Wekend";
                                //dr["MonthlyAverage"] = Math.Round(Average, 2).ToString();
                                //lblMontlyAverage.Text = Math.Round(Convert.ToDecimal(MonthlyAverage), 2).ToString();
                                // dr["FTEHours"] = Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont));
                                if (int.Parse(ProjectID) == 205)
                                {
                                    MonthContWeekend = dt1Weekend1.AsEnumerable()
                                              .Count(r => r.Field<string>("Date") != "Holiday");
                                    TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthContWeekend))));
                                    dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthContWeekend))));
                                }
                                else
                                {
                                    TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthContWeekend))));
                                    dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthContWeekend))));
                                }
                                try
                                {
                                    DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                                    if (dtRate != null)
                                    {
                                        if (dtRate.Rows.Count > 0)
                                        {
                                            dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                                            dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]), 2));
                                            int Before = (int)(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                                            dr["Before Decimal"] = Before.ToString();
                                            dr["After Decimal"] = Math.Round((Convert.ToDecimal(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"])) - Before) * 100, 0).ToString();
                                        }
                                    }
                                }
                                catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                                dtDyn.Rows.Add(dr);
                                object TotalAmountDec = dtDyn.AsEnumerable()
                                              .Where(r => r.Field<string>("Total Charges in US $") != "")
                                              .Sum(x => Convert.ToDecimal(x["Total Charges in US $"]));
                                int BeforeDec = (int)Convert.ToDecimal(TotalAmountDec);
                                decimal AfterDec = Math.Round((Convert.ToDecimal(TotalAmountDec) - BeforeDec) * 100, 0);

                                for (int i = 1; i < dtDyn.Rows.Count; i++)
                                {
                                    dtDyn.Rows[i]["Before Decimal"] = BeforeDec.ToString();
                                    dtDyn.Rows[i]["After Decimal"] = AfterDec.ToString();
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            DataTable dtNew1 = new DataTable();
            dtNew1.Columns.Add("DataColumn1");
            dtNew1.Columns.Add("DataColumn2");
            dtNew1.Columns.Add("DataColumn3");
            dtNew1.Columns.Add("DataColumn4");
            dtNew1.Columns.Add("DataColumn5");
            dtNew1.Columns.Add("DataColumn6");
            if (int.Parse(ProjectID) == 87)
                dtNew1.Columns.Add("DataColumn7");
            DataRow drCol = dtNew1.NewRow();
            try
            {
                drCol["DataColumn1"] = dtDyn.Columns[0].Caption;
                drCol["DataColumn2"] = dtDyn.Columns[1].Caption;
                drCol["DataColumn3"] = dtDyn.Columns[2].Caption;
                drCol["DataColumn4"] = dtDyn.Columns[3].Caption;
                drCol["DataColumn5"] = dtDyn.Columns[4].Caption;
                drCol["DataColumn6"] = dtDyn.Columns[5].Caption;
                if (int.Parse(ProjectID) == 87)
                    drCol["DataColumn7"] = dtDyn.Columns[6].Caption;
                dtNew1.Rows.Add(drCol);
            }
            catch { dtNew1.Rows.Add(drCol); }

            foreach (DataRow drN in dtDyn.Rows)
            {
                DataRow dr1 = dtNew1.NewRow();
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        dr1[i] = drN[i];
                    }
                    dtNew1.Rows.Add(dr1);
                }

                catch { dtNew1.Rows.Add(dr1); }

            }

            try
            {
                DataSet dt = new DataSet();
                DataTable dtNew = GetInvoiceNumber(ProjectName, BillingPeriod);
                if (dtNew != null)
                {
                    if (dtNew.Rows.Count > 0)
                    {
                        InvoiceNumber = Convert.ToString(dtNew.Rows[0]["InvoiceNumber"]);
                    }
                    else
                    {
                        InvoiceNumber = "";
                    }
                }
                else
                {
                    InvoiceNumber = "";
                }
                ReportDocument rpt = new ReportDocument();
                if (int.Parse(ProjectID) == 87)
                    rpt.Load(Server.MapPath("~/Reports/FTE/733-003.rpt"));
                else if (int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 386)
                    rpt.Load(Server.MapPath("~/Reports/FTE/534.rpt"));
                else if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 442 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 205 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 337 || int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385)
                    rpt.Load(Server.MapPath("~/Reports/FTE/706.rpt"));

                dtNew1.Rows.RemoveAt(0);
                rpt.Database.Tables["FTEData"].SetDataSource(dtNew1);
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                //pdisval1.Value = ProjectName;
                pdisval1.Value = ProjectName;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@GroupName"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = BillingPeriod;
                pval2.Add(pdisval2);

                //ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                //pdisval3.Value = txtAmount.Text;
                //pval3.Add(pdisval3);


                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval2);
                //rpt.DataDefinition.ParameterFields["@Amount"].ApplyCurrentValues(pval3);

                // rpt.SetDataSource(dt);
                //rpt.Subreports[0].SetDataSource(dt.Tables[0]);
                CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;
                CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos;
                CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo;
                CrystalDecisions.CrystalReports.Engine.Tables CrTables;
                crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
                crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();

                crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"];
                crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserID"];
                crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];

                CrTables = rpt.Database.Tables;

                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }




                string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                string strTime = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string filename = ProjectID + "_" + BillingPeriod + "_" + strDate + strTime;
                if (InvoiceNumber != "")
                {
                    filename = InvoiceNumber.Replace(",", "_");
                }
                else
                {
                    filename = filename + "_" + BillingPeriod + "_" + strDate + strTime;
                }
                //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);

                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath = Server.MapPath("~/BillingDocuments/") + filename + ".pdf";
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                int result = new bllTracking().InsertGroupAttachmentPath_QC(ProjectName, BillingPeriod, Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"), InvoiceNumber);

                // rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);
            }
            catch (Exception ex) { throw ex; }
        }

        public void SendInvoiceCreationEmailForSummaryReportWithExcel(string Groupname, string BillingPeriod, int ClientId, string ExcelPath)
        {
            string Header = "<html><head><meta content='text/html; charset=utf-8' http-equiv='Content-Type'><title></title><style type='text/css'>a:hover { text-decoration: none !important; }.header h1 {color: #fff !important; font: normal 33px Georgia, serif; margin: 0; padding: 0; line-height: 33px;}.header p {color: #dfa575; font: normal 11px Georgia, serif; margin: 0; padding: 0; line-height: 11px; letter-spacing: 2px}.content h2 {color:#8598a3 !important; font-weight: normal; margin: 0; padding: 0; font-style: italic; line-height: 30px; font-size: 30px;font-family: Georgia, serif; }.content p {color:#767676; font-weight: normal; margin: 0; padding: 0; line-height: 20px; font-size: 12px;font-family: Georgia, serif;}.content a {color: #d18648; text-decoration: none;}.footer p {padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;}.footer a {color: #f7a766; text-decoration: none;}</style></head><body><table cellpadding='0' cellspacing='0' border='1'><tr><td ><table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif;' class='header'><tr><td bgcolor='#16a085' height='70' align='center'><h1 style='color: #fff; font: normal 25px Verdana; margin: 0; padding: 0; line-height: 33px;'>Infinity Invoice</h1></td></tr><tr><td style='font-size: 1px; height: 5px; line-height: 1px;' height='5'>&nbsp;</td></tr></table>";

            string Footer = "<table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif; line-height: 10px; margin-top:30px;' bgcolor='#16a085' class='footer'><tr><td bgcolor='#16a085'  align='center' style='padding: 15px 0 10px; font-size: 11px; color:#fff; margin: 0; line-height: 1.2;font-family: Verdana;' valign='top'><p style='padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;'>!!! This is software generated e-mail...Please do not reply. !!</p></td></tr> </table></td></tr></table></body></html>";
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";

            DataTable DTClientDetails = GetSummaryReportInvoice(Groupname, BillingPeriod);
            DataTable dtClientInfo = GetClientDetails(Groupname, BillingPeriod);
            DataTable DtInvoice = new bllTracking().GetSummaryReportAttachments(Groupname, BillingPeriod);
            string ClientEmail = Convert.ToString(dtClientInfo.Rows[0]["PAI_Email_Id"]);
            string ToCCs = Convert.ToString(dtClientInfo.Rows[0]["CEC_CC"]);
            try
            {
                if (ToCCs.Contains(","))
                {
                    string[] ToCCStr = ToCCs.Split(',');
                    foreach (string cc in ToCCStr)
                    {
                        if (cc != "")
                        {
                            ToCC += cc + ".pointofmail.com,";
                        }
                    }
                }
                else
                {
                    ToCC = Convert.ToString(dtClientInfo.Rows[0]["CEC_CC"]);
                }
                ToCC = ToCC.Substring(0, ToCC.LastIndexOf(','));
            }
            catch { }
            string clientName = "";
            if (dtClientInfo != null)
            {
                if (dtClientInfo.Rows.Count > 0)
                {
                    clientName = Convert.ToString(dtClientInfo.Rows[0]["PAI_Contact_Person"]);
                }
            }
            try
            {
                clientName = clientName.Substring(0, clientName.IndexOf(" "));
            }
            catch { }

            htmlBody_Service.Append("<table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\"><b>Hello " + clientName + ",</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Good Morning!! <br /><br />Please find attached invoice# " + Convert.ToString(DTClientDetails.Rows[0]["InvNo"]) + " for your review.");
            htmlBody_Service.Append("<br /><br />Kindly, For all invoices related queries direct email to following.");
            htmlBody_Service.Append("<br /><br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a>");
            htmlBody_Service.Append("<br /><a href='mailTo:jim@Infinity-data.com'>jim@Infinity-data.com</a>");
            htmlBody_Service.Append("</td></tr></table><br /><br /><br /><br /><table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\">Thanks,<br />Anita Londhe<br />VP Controller<br />Infinity IPS<br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a><br /><a href='www.infinity-data.com'>www.infinity-data.com</a></td></tr> </table>");

            htmlBody_Service.Append("<table width=\"650px\"><tr><td align=\"left\"></td></tr></table>");
            htmlBody_Service.Append("<br /><span style='color:red; font-size:14px;'>**WE DO NOT ACCEPT OR REQUEST CHANGES TO WIRING INSTRUCTION VIA EMAIL - Always call to verify**</span>");
            htmlBody_Service.Append("<br /><span>**********************************************************************************************************</span>");
            htmlBody_Service.Append("<br /><br /><span style='font-size:11px;'>Disclaimer: The information contained in this e-mail and any attachments may be confidential or privileged under applicable law, or otherwise may be protected from disclosure to anyone other than the intended recipient(s). Any use, distribution, or copying of this e-mail, including any of its contents or attachments by any person other than the intended recipient, or for any purpose other than its intended use, is strictly prohibited. If you believe you have received this e-mail in error, please notify us by e-mail and permanently delete the e-mail and any attachments, and do not save, copy, disclose, or reply on any part of the information contained in this e-mail or its attachments</span>");

            //string path = "E:/EmailPages/HRMS_BillingSummaryReportEmail_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            //FileStream fp = File.Create(path);
            //fp.Close();
            //using (StreamWriter w = new StreamWriter(path, true))
            //{
            //    w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            //}

            string Subject = "Infinity invoice " + Convert.ToString(DtInvoice.Rows[0]["Invoice_GroupNumber"]);
            //ToAddress = ClientEmail + ".pointofmail.com";

            if (ToAddress == "")
            {
                ToAddress = "anita@infinity-data.com";
            }
            //ToAddress = "n.nilkanth@infinityinternationals.us";
            ToBCC = "n.nilkanth@infinityinternationals.us";

            StringBuilder body = new StringBuilder();
            body.Append(Header);
            body.Append(htmlBody_Service);
            body.Append(Footer);

            string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["PGA_AttachmentsPDF"].ToString());
            MailMessage mail = new MailMessage();

            mail.To.Add(ToAddress);
            //if (ToCC != "")
            //    mail.CC.Add(ToCC);
            if (ToBCC != "")
                mail.Bcc.Add(ToBCC);

            mail.From = new MailAddress("AR@infinity-data.com", "Infinity Billing", System.Text.Encoding.UTF8);
            mail.Subject = Subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = body.ToString();
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = System.Net.Mail.MailPriority.High;
            // mail.Attachments.Add(new Attachment(Attachment));
            if (Attachment != "")
            {
                mail.Attachments.Add(new Attachment(Attachment));
            }
            if (ExcelPath != "")
            {
                mail.Attachments.Add(new Attachment(ExcelPath));
            }
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("AR@infinity-data.com", "Billing@0504");
            client.Port = 587;
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(mail);
                // return true;
            }
            catch
            {
                // return false;
            }
            //Hashtable htParam = new Hashtable();
            //htParam.Add("EmailType", "");
            //htParam.Add("To", Convert.ToString(ToAddress));
            //htParam.Add("CC", ToCC);
            //htParam.Add("BCC", ToBCC);
            //htParam.Add("Subject", Subject);
            //htParam.Add("Body", path);
            //htParam.Add("Attachment", Attachment);
            //htParam.Add("DraftNo", "1");
            //int ReturnValue = new SendMail().InsertAutoEmailTracking(htParam);
        }
        public void SendInvoiceCreationEmailForSummaryReport(string Groupname, string BillingPeriod, int ClientId)
        {
            string Header = "<html><head><meta content='text/html; charset=utf-8' http-equiv='Content-Type'><title></title><style type='text/css'>a:hover { text-decoration: none !important; }.header h1 {color: #fff !important; font: normal 33px Georgia, serif; margin: 0; padding: 0; line-height: 33px;}.header p {color: #dfa575; font: normal 11px Georgia, serif; margin: 0; padding: 0; line-height: 11px; letter-spacing: 2px}.content h2 {color:#8598a3 !important; font-weight: normal; margin: 0; padding: 0; font-style: italic; line-height: 30px; font-size: 30px;font-family: Georgia, serif; }.content p {color:#767676; font-weight: normal; margin: 0; padding: 0; line-height: 20px; font-size: 12px;font-family: Georgia, serif;}.content a {color: #d18648; text-decoration: none;}.footer p {padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;}.footer a {color: #f7a766; text-decoration: none;}</style></head><body><table cellpadding='0' cellspacing='0' border='1'><tr><td ><table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif;' class='header'><tr><td bgcolor='#16a085' height='70' align='center'><h1 style='color: #fff; font: normal 25px Verdana; margin: 0; padding: 0; line-height: 33px;'>Infinity Invoice</h1></td></tr><tr><td style='font-size: 1px; height: 5px; line-height: 1px;' height='5'>&nbsp;</td></tr></table>";

            string Footer = "<table cellpadding='0' cellspacing='0' border='0' align='center' width='100%' style='font-family: Georgia, serif; line-height: 10px; margin-top:30px;' bgcolor='#16a085' class='footer'><tr><td bgcolor='#16a085'  align='center' style='padding: 15px 0 10px; font-size: 11px; color:#fff; margin: 0; line-height: 1.2;font-family: Verdana;' valign='top'><p style='padding: 0; font-size: 11px; color:#fff; margin: 0; font-family: Georgia, serif;'>!!! This is software generated e-mail...Please do not reply. !!</p></td></tr> </table></td></tr></table></body></html>";
            StringBuilder htmlBody = new StringBuilder();
            StringBuilder htmlBody_Service = new StringBuilder();
            string ToAddress = "";
            string ToCC = "";
            string sub = "";
            string ToBCC = "";

            DataTable DTClientDetails = GetSummaryReportInvoice(Groupname, BillingPeriod);
            DataTable dtClientInfo = GetClientDetails(Groupname, BillingPeriod);
            DataTable DtInvoice = new bllTracking().GetSummaryReportAttachments(Groupname, BillingPeriod);
            string ClientEmail = Convert.ToString(dtClientInfo.Rows[0]["PAI_Email_Id"]);
            string ToCCs = Convert.ToString(dtClientInfo.Rows[0]["CEC_CC"]);
            try
            {
                if (ToCCs.Contains(","))
                {
                    string[] ToCCStr = ToCCs.Split(',');
                    foreach (string cc in ToCCStr)
                    {
                        if (cc != "")
                        {
                            ToCC += cc + ".pointofmail.com,";
                        }
                    }
                }
                else
                {
                    ToCC = Convert.ToString(dtClientInfo.Rows[0]["CEC_CC"]);
                }
                ToCC = ToCC.Substring(0, ToCC.LastIndexOf(','));
            }
            catch { }
            string clientName = "";
            if (dtClientInfo != null)
            {
                if (dtClientInfo.Rows.Count > 0)
                {
                    clientName = Convert.ToString(dtClientInfo.Rows[0]["PAI_Contact_Person"]);
                }
            }
            try
            {
                clientName = clientName.Substring(0, clientName.IndexOf(" "));
            }
            catch { }

            htmlBody_Service.Append("<table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\"><b>Hello " + clientName + ",</b></td></tr><tr>");
            htmlBody_Service.Append("<td align=\"left\">Good Morning!! <br /><br />Please find attached invoice# " + Convert.ToString(DTClientDetails.Rows[0]["InvNo"]) + " for your review.");
            htmlBody_Service.Append("<br /><br />Kindly, For all invoices related queries direct email to following.");
            htmlBody_Service.Append("<br /><br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a>");
            htmlBody_Service.Append("<br /><a href='mailTo:jim@Infinity-data.com'>jim@Infinity-data.com</a>");
            htmlBody_Service.Append("</td></tr></table><br /><br /><br /><br /><table width=\"650px\" style='margin-left:10px;'><tr><td align=\"left\">Thanks,<br />Anita Londhe<br />VP Controller<br />Infinity IPS<br /><a href='mailTo:anita@Infinity-data.com'>anita@Infinity-data.com</a><br /><a href='www.infinity-data.com'>www.infinity-data.com</a></td></tr> </table>");

            htmlBody_Service.Append("<table width=\"650px\"><tr><td align=\"left\"></td></tr></table>");
            htmlBody_Service.Append("<br /><span style='color:red; font-size:14px;'>**WE DO NOT ACCEPT OR REQUEST CHANGES TO WIRING INSTRUCTION VIA EMAIL - Always call to verify**</span>");
            htmlBody_Service.Append("<br /><span>**********************************************************************************************************</span>");
            htmlBody_Service.Append("<br /><br /><span style='font-size:11px;'>Disclaimer: The information contained in this e-mail and any attachments may be confidential or privileged under applicable law, or otherwise may be protected from disclosure to anyone other than the intended recipient(s). Any use, distribution, or copying of this e-mail, including any of its contents or attachments by any person other than the intended recipient, or for any purpose other than its intended use, is strictly prohibited. If you believe you have received this e-mail in error, please notify us by e-mail and permanently delete the e-mail and any attachments, and do not save, copy, disclose, or reply on any part of the information contained in this e-mail or its attachments</span>");

            string path = "E:/EmailPages/HRMS_BillingSummaryReportEmail_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".html";
            FileStream fp = File.Create(path);
            fp.Close();
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(Header + htmlBody_Service + Footer); // Write the text
            }
            string Subject = "Infinity invoice " + Convert.ToString(DtInvoice.Rows[0]["Invoice_GroupNumber"]);
            ToAddress = ClientEmail + ".pointofmail.com";

            if (ToAddress == "")
            {
                ToAddress = "anita@infinity-data.com.pointofmail.com";
            }

            ToBCC = "";
            StringBuilder body = new StringBuilder();
            body.Append(Header);
            body.Append(htmlBody_Service);
            body.Append(Footer);

            string Attachment = System.Web.HttpContext.Current.Server.MapPath(DtInvoice.Rows[0]["PGA_AttachmentsPDF"].ToString());

            Hashtable htParam = new Hashtable();
            htParam.Add("EmailType", "");
            htParam.Add("To", Convert.ToString(ToAddress));
            htParam.Add("CC", ToCC);
            htParam.Add("BCC", ToBCC);
            htParam.Add("Subject", Subject);
            htParam.Add("Body", path);
            htParam.Add("Attachment", Attachment);
            htParam.Add("DraftNo", "1");
            int ReturnValue = new SendMail().InsertAutoEmailTracking(htParam);
        }

        public bool sendMailWithAttach_WithExcel(string EmailType, string Subject, StringBuilder htmlBody, string strEmailTo, string Attachment, string ExcelAttachment)
        {
            string ToAddress = string.Empty;
            string ToCC = string.Empty;
            string ToBCC = string.Empty;

            String Body = htmlBody.ToString();
            StringBuilder template = new StringBuilder();
            template.Append("<html><head></head><body>");
            template.Append(Body);
            template.Append("</body></html>");
            MailMessage mail = new MailMessage();
            if (strEmailTo != "")
                ToAddress = ToAddress + ',' + strEmailTo;

            //ToAddress = "jim@infinity-data.com.pointofmail.com";
            //ToAddress = "n.nilkanth@infinityinternationals.us";
            mail.To.Add(ToAddress);
            if (ToCC != "")
                mail.CC.Add(ToCC);
            if (ToBCC != "")
                mail.Bcc.Add(ToBCC);

            mail.From = new MailAddress("AR@infinity-data.com", "Infinity Billing", System.Text.Encoding.UTF8);
            mail.Subject = Subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = template.ToString();
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = System.Net.Mail.MailPriority.High;
            mail.Attachments.Add(new Attachment(Attachment));
            if (ExcelAttachment != "")
            {
                mail.Attachments.Add(new Attachment(ExcelAttachment));
            }
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("AR@infinity-data.com", "Billing@0504");
            client.Port = 587;
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataTable GetInvoiceNumber(string ProjectNumber, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetInvocieNumber_QC");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectGroup", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, ProjectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public void Time_To_Show_The_SummaryReport()
        {
            try
            {
                string ProjectName = Session["GroupName"].ToString();
                string BillingPeriod = Session["billingPeriod"].ToString();
                string InvoiceNumber = "";
                int ProjectID = 0;
                DataSet dt = new DataSet();
                DataTable dtNew = GetInvoiceNumber(ProjectName, BillingPeriod);
                if (dtNew != null)
                {
                    if (dtNew.Rows.Count > 0)
                    {
                        InvoiceNumber = Convert.ToString(dtNew.Rows[0]["InvoiceNumber"]);
                        ProjectID = Convert.ToInt32(dtNew.Rows[0]["ProjectID"]);
                    }
                    else
                    {
                        InvoiceNumber = "";
                    }
                }
                else
                {
                    InvoiceNumber = "";
                }
                ReportDocument rpt = new ReportDocument();

                if (ProjectName == "711")
                    rpt.Load(Server.MapPath("~/Reports/Freight/711.rpt"));
                else if (ProjectName == "722")
                    rpt.Load(Server.MapPath("~/Reports/Freight/722.rpt"));
                else if (ProjectName == "733-002" || ProjectName == "772")
                    rpt.Load(Server.MapPath("~/Reports/Freight/733-002.rpt"));
                else if (ProjectName == "754")
                    rpt.Load(Server.MapPath("~/Reports/Freight/754.rpt"));
                else
                    rpt.Load(Server.MapPath("~/Reports/Commitment/183-002.rpt"));
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = ProjectName;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@GroupName"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = ProjectID;
                pval2.Add(pdisval2);

                ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                pdisval3.Value = BillingPeriod;
                pval3.Add(pdisval3);


                rpt.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval2);
                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval3);

                // rpt.SetDataSource(dt);
                //rpt.Subreports[0].SetDataSource(dt.Tables[0]);
                CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;
                CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos;
                CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo;
                CrystalDecisions.CrystalReports.Engine.Tables CrTables;
                crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
                crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();

                crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"];
                crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserID"];
                crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"];

                CrTables = rpt.Database.Tables;

                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }



                string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                string strTime = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string filename = ProjectID + "_" + BillingPeriod + "_" + strDate + strTime;
                if (InvoiceNumber != "")
                {
                    filename = InvoiceNumber.Replace(",", "_");
                }
                else
                {
                    filename = filename + "_" + BillingPeriod + "_" + strDate + strTime;
                }
                //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);

                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath = Server.MapPath("~/BillingDocuments/") + filename + ".pdf";

                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                int result = new bllTracking().InsertGroupAttachmentPath_QC(ProjectName, BillingPeriod, Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"), InvoiceNumber);
                //int result = blltracking.UpdateInvoicePath(int.Parse(InvoiceId), Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"));

                //  rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);
            }
            catch (Exception ex) { throw ex; }

        }

        public int UpdateInvoiceClient_OtherDomain(string GroupName, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "IB_usp_UpdateInvoiceClientId_OtherDomain");
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupName", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, GroupName);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, BillingPeriod);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            cmd.Dispose();
            return ReturnValue;
        }
        public void BindClients(int InvoiceID)
        {

        }
        public void BindClientstest(int InvoiceID)
        {

        }


        protected void btnsendEmail_Click(object sender, EventArgs e)
        {
            string InvId = Session["InvoiceIdTestEmail"].ToString();
            string ClientId = "1";
            SendEmail.SendTestEmail(int.Parse(InvId), int.Parse(HttpContext.Current.User.Identity.Name.ToString()), int.Parse(ClientId), txttestEmailids.Value);
            dvError.Style.Add("display", "");
            dvError.Attributes.Add("class", "alert alert-success background-success");
            dvError.InnerHtml = "Email sent successfully!";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            RegisterModalClose("modalTestEmail");
        }


        protected void btnPreview_Click(object sender, EventArgs e)
        {

        }

        public void BindEmailDetails(int InvoiceIdPreviewEmail)
        {
            DataTable dt = blltracking.EmailDetailsByInvoiceID(InvoiceIdPreviewEmail);
            if (dt.Rows.Count > 0)
            {
                txtTo.InnerText = Convert.ToString(dt.Rows[0]["CEC_To"]);
                txtCC.InnerText = Convert.ToString(dt.Rows[0]["CEC_CC"]);
                txtBCC.InnerText = Convert.ToString(dt.Rows[0]["CEC_BCC"]);
                lblDateSent.InnerText = Convert.ToString(dt.Rows[0]["EmailSentDate"]);

                lnkAttachment.InnerText = Convert.ToString(dt.Rows[0]["InvoiceNumber"]);
                txtSubject.InnerText = "Infinity invoice " + Convert.ToString(dt.Rows[0]["InvoiceNumber"]);
                string InvId = Session["InvoiceIdPreviewEmail"].ToString();
                string ClientId = "1";
                if (!Directory.Exists(Server.MapPath(@"~/EmailPages/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/EmailPages/"));
                }
                string strPath = Server.MapPath(@"~/EmailPages/");
                string Path = SendEmail.ViewInvoiceEmailTemplate(int.Parse(InvId), int.Parse(HttpContext.Current.User.Identity.Name.ToString()), int.Parse(ClientId), strPath);

                using (System.IO.StreamReader reader = new System.IO.StreamReader(Server.MapPath(@"~/EmailPages/" + Path)))
                {
                    dvHtml.InnerHtml = reader.ReadToEnd();
                }
            }
        }

        protected void lnkAttachment_Click(object sender, EventArgs e)
        {
            int InvIdPreviewEmail = Convert.ToInt32(Session["InvoiceIdPreviewEmail"]);
            DataTable dt = blltracking.EmailDetailsByInvoiceID(InvIdPreviewEmail);
            string Filename = Convert.ToString(dt.Rows[0]["InvoiceNumber"]);
            string Invpath = Convert.ToString(dt.Rows[0]["Invpath"]);
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Filename + ".pdf");
            // String.Format("attachment;filename={0}", "attachment.pdf"));
            Response.ContentType = "application/pdf";
            Response.TransmitFile(Server.MapPath(Invpath));
            Response.End();
        }

        protected void ddlClientList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Session["ClientID"] = ddlClientList.SelectedValue;
        }


        protected void btnShowFilter_Click(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
                BindGrid();
            else
                BindGridSummaryMonthWise();
        }

        public void BindGridSummaryMonthWise()
        {
            DataTable dt = new DataTable();
            dt = GetAllProjectforClientApproved(ddlMonth.Value, ddlYear.Value);
            RenderClientGrid(dt);
        }

        public DataTable GetAllProjectforClientApproved(string Month, string Year)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "IB_usp_GetAllProjectForClientApproval_Filter");
            SQLHelper.AddParamToSQLCmd(cmd, "@Month", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Month);
            SQLHelper.AddParamToSQLCmd(cmd, "@Year", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Year);


            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        protected void btnLoadInvoiceModal_ServerClick(object sender, EventArgs e)
        {
            string key = hfActionKey.Value;
            if (!string.IsNullOrWhiteSpace(key))
            {
                Session["InvDetails"] = key;
                BindInvoiceRecievedDetails(key);
                RegisterModalOpen("modalInvoiceUpdate");
            }
        }

        protected void btnLoadSendModal_ServerClick(object sender, EventArgs e)
        {
            string key = hfActionKey.Value;
            string[] p = (key ?? string.Empty).Split('|');
            if (p.Length >= 5)
            {
                Session["GroupName"] = p[0];
                Session["billingPeriod"] = p[1];
                Session["ProjectID"] = p[2];
                Session["InvoiceID"] = p[3];
                Session["DomainName"] = p[4];
                BindEmilTemplateForSendMail(p[0], p[1], 0);
                RegisterModalOpen("modalSendInvoice");
            }
        }

        protected void btnLoadRemarkModal_ServerClick(object sender, EventArgs e)
        {
            int invoiceId = ExtractInvoiceId(hfActionKey.Value);
            if (invoiceId > 0)
            {
                Session["InvoiceIdRemark"] = invoiceId;
                BindInvoiceRemarks(invoiceId);
                RegisterModalOpen("modalRemark");
            }
        }

        protected void btnLoadTestEmailModal_ServerClick(object sender, EventArgs e)
        {
            int invoiceId = ExtractInvoiceId(hfActionKey.Value);
            if (invoiceId > 0)
            {
                Session["InvoiceIdTestEmail"] = invoiceId;
                RegisterModalOpen("modalTestEmail");
            }
        }

        protected void btnLoadEmailPreviewModal_ServerClick(object sender, EventArgs e)
        {
            int invoiceId = ExtractInvoiceId(hfActionKey.Value);
            if (invoiceId > 0)
            {
                Session["InvoiceIdPreviewEmail"] = invoiceId;
                BindEmailDetails(invoiceId);
                RegisterModalOpen("modalEmailPreview");
            }
        }

        protected void btnAttachment_ServerClick(object sender, EventArgs e)
        {
            lnkAttachment_Click(sender, e);
        }

        protected void btnShowFilter_ServerClick(object sender, EventArgs e)
        {
            btnShowFilter_Click(sender, e);
        }

        protected void btnShow_ServerClick(object sender, EventArgs e)
        {
            btnShow_Click(sender, e);
        }

        protected void btnSendInvoice_ServerClick(object sender, EventArgs e)
        {
            Button1_Click(sender, e);
        }

        protected void btnSendTestEmail_ServerClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txttestEmailids.Value))
            {
                ShowMessage("Please enter email id", false);
                RegisterModalOpen("modalTestEmail");
                return;
            }
            btnsendEmail_Click(sender, e);
        }

        protected void btnUpdateInvoice_ServerClick(object sender, EventArgs e)
        {
            btnUpdateInvoice_Click(sender, e);
            RegisterModalOpen("modalInvoiceUpdate");
        }

        protected void btnUpdateRemark_ServerClick(object sender, EventArgs e)
        {
            btnUpdateRemark_Click(sender, e);
            RegisterModalOpen("modalRemark");
        }

        public class InvoiceAjaxResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string ProjectNumber { get; set; }
            public string BillingPeriod { get; set; }
            public string InvoiceReceivedDate { get; set; }
            public string NoDisputeDate { get; set; }
            public string CommunicationReceipt { get; set; }
            public string Remark { get; set; }
            public string CompletedDate { get; set; }
        }

        [WebMethod]
        public static InvoiceAjaxResult GetInvoiceForUpdate(string key)
        {
            try
            {
                string[] p = (key ?? string.Empty).Split('|');
                if (p.Length < 2) return new InvoiceAjaxResult { Success = false, Message = "Invalid invoice selection." };
                string projectNumber = p[0];
                string billingPeriod = p[1];
                DataTable dt = GetInvoiceDetailsAjax(projectNumber, billingPeriod);
                if (dt == null || dt.Rows.Count == 0) return new InvoiceAjaxResult { Success = false, Message = "Invoice details not found." };
                DataRow r = dt.Rows[0];
                return new InvoiceAjaxResult
                {
                    Success = true,
                    ProjectNumber = projectNumber,
                    BillingPeriod = billingPeriod,
                    InvoiceReceivedDate = ToHtmlDate(r["InvoiceRecievedByClientDate"]),
                    NoDisputeDate = ToHtmlDate(r["NoDisputeConfirmByClient"]),
                    CommunicationReceipt = Convert.ToString(r["CommunicationReciept"]),
                    Remark = Convert.ToString(r["Remark"]),
                    CompletedDate = ToHtmlDate(r["InvCompleteDate"])
                };
            }
            catch (Exception ex)
            {
                return new InvoiceAjaxResult { Success = false, Message = ex.Message };
            }
        }

        [WebMethod]
        public static InvoiceAjaxResult UpdateInvoiceAjax(string key, string invoiceReceivedDate, string noDisputeDate, string communicationReceipt, string remark, string completedDate)
        {
            try
            {
                string[] p = (key ?? string.Empty).Split('|');
                if (p.Length < 2) return new InvoiceAjaxResult { Success = false, Message = "Invalid invoice selection." };
                DataTable dt = GetInvoiceDetailsAjax(p[0], p[1]);
                if (dt == null || dt.Rows.Count == 0) return new InvoiceAjaxResult { Success = false, Message = "Invoice details not found." };

                Hashtable ht = new Hashtable();
                ht.Add("InvoiceID", Convert.ToInt32(dt.Rows[0]["InvoiceID"]));
                DateTime parsed;
                if (DateTime.TryParse(invoiceReceivedDate, out parsed)) ht.Add("InvoiceRecievedByClientDate", parsed);
                ht.Add("CommunicationReciept", communicationReceipt ?? string.Empty);
                ht.Add("Remark", (remark ?? string.Empty).Trim());
                if (DateTime.TryParse(noDisputeDate, out parsed)) ht.Add("NoDisputeConfirmByClient", noDisputeDate);
                if (DateTime.TryParse(completedDate, out parsed)) ht.Add("InvCompleteDate", completedDate);

                new bllTracking().UpdateInvoiceDetails(ht);
                return new InvoiceAjaxResult { Success = true, Message = "Details added successfully." };
            }
            catch (Exception ex)
            {
                return new InvoiceAjaxResult { Success = false, Message = ex.Message };
            }
        }

        private static DataTable GetInvoiceDetailsAjax(string projectNumber, string billingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[IB_usp_GetInvoiceDetails_ForRead]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectNumber", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, projectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, billingPeriod);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        private static string ToHtmlDate(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(value))) return string.Empty;
            DateTime d;
            return DateTime.TryParse(Convert.ToString(value), out d) ? d.ToString("yyyy-MM-dd") : string.Empty;
        }

        private int ExtractInvoiceId(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return 0;
            int id;
            if (int.TryParse(key, out id)) return id;
            string[] p = key.Split('|');
            if (p.Length > 3 && int.TryParse(p[3], out id)) return id;
            return 0;
        }

        private void RenderClientGrid(DataTable dt)
        {
            if (dt == null) dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (DataRow r in dt.Rows)
            {
                string project = GetCell(r, "ProjectName");
                string period = GetCell(r, "BillingPeriod");
                string projectId = GetCell(r, "ProjectID");
                string invoiceId = GetCell(r, "InvoiceID");
                string domain = GetCell(r, "DomainName");
                string key = project + "|" + period + "|" + projectId + "|" + invoiceId + "|" + domain;
                string invoiceUrl = "InvoiceViewer.aspx?InvoiceId=" + HttpUtility.UrlEncode(invoiceId) + "&ProjectID=" + HttpUtility.UrlEncode(projectId) + "&BillingPeriod=" + HttpUtility.UrlEncode(period) + "&ProjectName=" + HttpUtility.UrlEncode(project);
                string excelUrl = "ExcelViewer.aspx?InvoiceId=" + HttpUtility.UrlEncode(invoiceId) + "&ProjectID=" + HttpUtility.UrlEncode(projectId) + "&BillingPeriod=" + HttpUtility.UrlEncode(period);
                sb.Append("<tr>");
                sb.Append("<td>").Append(i++).Append("</td>");
                                sb.Append("<td><a href=\"javascript:void(0)\" onclick=\"openInvoiceUpdate('").Append(Js(key)).Append("')\">Update Invoice</a></td>");
                sb.Append("<td>").Append(H(GetCell(r, "DomainName"))).Append("</td>");
                sb.Append("<td>").Append(H(project)).Append("</td>");
                sb.Append("<td>").Append(H(period)).Append("</td>");
                sb.Append("<td>").Append(H(GetCell(r, "InvoiceNumber"))).Append("</td>");
                sb.Append("<td>").Append(H(GetCell(r, "InvoiceOrderCount"))).Append("</td>");
                sb.Append("<td>").Append(H(GetCell(r, "Invoice_Amount"))).Append("</td>");
                sb.Append("<td>").Append(H(FormatDateCell(r, "AddedDate"))).Append("</td>");
                sb.Append("<td><a href=\"javascript:void(0)\" onclick=\"openReportPreview('pdf','").Append(Js(invoiceUrl)).Append("')\"><img src=\"../Images/pdf.gif\" alt=\"PDF\" /></a></td>");
                sb.Append("<td><a href=\"javascript:void(0)\" onclick=\"openReportPreview('excel','").Append(Js(excelUrl)).Append("')\"><img src=\"../Images/xls.gif\" alt=\"Excel\" /></a></td>");
                sb.Append("<td>").Append(H(GetCell(r, "InvoiceSent"))).Append("</td>");
                sb.Append("<td>").Append(H(FormatDateCell(r, "DeliveredDate"))).Append("</td>");
                sb.Append("<td>").Append(H(FormatDateCell(r, "ReadDate"))).Append("</td>");
                sb.Append("<td><a href=\"javascript:void(0)\" onclick=\"loadServerModal('send','").Append(Js(key)).Append("')\">Send To Client</a></td>");
                sb.Append("</tr>");
            }
            clientRows.InnerHtml = sb.ToString();
            lblRecords.InnerText = dt.Rows.Count + " record(s)";
        }

        private void RenderRemarkGrid(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            if (dt != null)
            {
                int i = 1;
                foreach (DataRow r in dt.Rows)
                {
                    sb.Append("<tr><td>").Append(i++).Append("</td><td>")
                      .Append(H(GetCell(r, "Remark"))).Append("</td><td>")
                      .Append(H(GetCell(r, "AddedBy"))).Append("</td><td>")
                      .Append(H(FormatDateCell(r, "Addeddate"))).Append("</td></tr>");
                }
            }
            remarkRows.InnerHtml = sb.ToString();
        }

        private static string GetCell(DataRow row, string column)
        {
            return row != null && row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToString(row[column]) : string.Empty;
        }

        private static string FormatDateCell(DataRow row, string column)
        {
            string value = GetCell(row, column);
            DateTime d;
            return DateTime.TryParse(value, out d) ? d.ToString("dd-MMM-yyyy") : value;
        }

        private static string H(string value) { return HttpUtility.HtmlEncode(value ?? string.Empty); }
        private static string Js(string value) { return HttpUtility.JavaScriptStringEncode(value ?? string.Empty); }

        private void RegisterModalOpen(string id)
        {
            ClientScript.RegisterStartupScript(GetType(), "open_" + id, "window.setTimeout(function(){showPortalModal('" + id + "');},0);", true);
        }

        private void RegisterModalClose(string id)
        {
            ClientScript.RegisterStartupScript(GetType(), "close_" + id, "window.setTimeout(function(){hidePortalModal('" + id + "');},0);", true);
        }

        private void ShowMessage(string message, bool success)
        {
            dvError.Attributes["class"] = success ? "alert alert-success" : "alert alert-danger";
            dvError.InnerText = message;
            dvError.Style["display"] = "block";
        }

        private void SaveDataTableAsExcelHtml(DataTable dt, string path, string title)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><meta charset='utf-8'></head><body><table border='1'><tr>");
            foreach (DataColumn c in dt.Columns) sb.Append("<th>").Append(H(c.ColumnName)).Append("</th>");
            sb.Append("</tr>");
            foreach (DataRow r in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn c in dt.Columns) sb.Append("<td>").Append(H(Convert.ToString(r[c]))).Append("</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table></body></html>");
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

    }
}
