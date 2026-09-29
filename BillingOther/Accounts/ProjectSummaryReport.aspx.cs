using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using CrystalDecisions.Shared;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Net.Mime;
using BillingOther.App_Code.DAL;
using BillingOther.App_Code.BLL;

namespace BillingOther.Accounts
{
    public partial class ProjectSummaryReport : System.Web.UI.Page
    {
        SendMail SendEmail = new SendMail();
        bllTracking bllMaster = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Project Summary Report";
            if (!IsPostBack)
            {
                //BindProjectsGroup(); ddlPeriodBind();
                //if (ddlMonth.SelectedIndex == 0)
                //    BindGridSummary();
                //else
                //    BindGridSummaryMonthWise();

            }
            //string InvDetails = Convert.ToString(Session["InvDetails"]);
            //if (InvDetails != "")
            //{
            //    BindInvoiceRecievedDetails(InvDetails);
            //}

            if (Session["SummaryProjectWise"] != null)
            {

                if (!string.IsNullOrEmpty(txtSummaryFromDate.Text.Trim()) && !string.IsNullOrEmpty(txtSummaryToDate.Text.Trim()))
                {
                    BindSummaryProjectWise(txtSummaryFromDate.Text.Trim(), txtSummaryToDate.Text.Trim());
                }
            }
            else
            {

                grdSummaryDetails.DataSource = (DataTable)Session["SummaryProjectWise"];
                grdSummaryDetails.DataBind();
            }

        }

        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            // Time_To_Show_The_SummaryReport();
            BindGridSummary();
        }
        
        public void BindGridSummary()
        {
            //DataTable dt = new DataTable();
            //dt = bllMaster.BindSummaryReport((ddlprojectGroup.SelectedValue), ddlPeriod.SelectedItem.ToString());

            //grdSummaryDetails.DataSource = dt;
            //grdSummaryDetails.DataBind();


        }
    
        public DataTable GetInvoiceNumber(string ProjectNumber, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetInvocieNumber_QC");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectGroup", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, ProjectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public int UpdateInvoiceClient_QC(string GroupName, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "IB_usp_UpdateInvoiceClientId_UW");
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupName", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, GroupName);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, BillingPeriod);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            cmd.Dispose();
            return ReturnValue;
        }

    
        public void BindGroupProjects(string GroupNumber)
        {


            //ddlProjNo.DataSource = bllMaster.ViewAllGroupProjects(GroupNumber);
            //ddlProjNo.DataTextField = "ProjectName";
            //ddlProjNo.DataValueField = "ProjectID";
            //ddlProjNo.DataBind();
            //ddlProjNo.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));

        }
        protected void ddlPeriodBind()
        {
            DateTime now = DateTime.Now;
            DateTime fromDate = now.AddMonths(-5);
            DateTime fromDateNew = now.AddMonths(-5);
            for (int i = 0; i < 6; i++)
            {
                string Month = fromDate.ToString("MMM");
                var startDate = new DateTime(fromDate.Year, fromDate.Month, 1);
                string start = startDate.ToString("dd-MMM-yyyy");
                var endDate = (startDate.AddMonths(1).AddDays(-1));
                string End = endDate.ToString("dd-MMM-yyyy");

                string FirstHalf = Convert.ToString("01-" + Month + "-" + fromDate.Year + " ~ 15-" + Month + "-" + fromDate.Year);
                string secondHalf = Convert.ToString("16-" + Month + "-" + fromDate.Year + " ~ " + End);


                fromDate = fromDateNew.AddMonths(i + 1);
            }
        }
        protected void ddlprojectGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();
            //dt = BindSummaryReportMonthwise((ddlMonth.SelectedValue), ddlYear.SelectedValue);
            //txtSummaryFromDate.Text.Trim(), txtSummaryToDate.Text.Trim()
            if (!string.IsNullOrEmpty(txtSummaryFromDate.Text.Trim()) && !string.IsNullOrEmpty(txtSummaryToDate.Text.Trim()))
            {
                DataTable dt = GetALLProjectVolumeInOnlineTrackingSheet(Convert.ToDateTime(txtSummaryFromDate.Text.Trim()).ToString("dd-MMM-yyyy"), Convert.ToDateTime(txtSummaryToDate.Text.Trim()).ToString("dd-MMM-yyyy"), Convert.ToInt32(HttpContext.Current.User.Identity.Name));

                if (dt.Rows.Count > 0)
                {
                    grdSummaryDetails.DataSource = dt;
                    grdSummaryDetails.DataBind();
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", "attachment;filename=Billing Summary Report.xls");
                    using (StringWriter sw = new StringWriter())
                    using (HtmlTextWriter writer = new HtmlTextWriter(sw))
                    {
                        grdSummaryDetails.RenderControl(writer);
                        Response.Write(sw.ToString());
                    }
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }

     
      

     
        protected void lnkAttachment_Click(object sender, EventArgs e)
        {
            int InvIdPreviewEmail = Convert.ToInt32(Session["InvoiceIdPreviewEmail"]);
            DataTable dt = bllMaster.EmailDetailsByInvoiceIDGroup(InvIdPreviewEmail);
            string Filename = Convert.ToString(dt.Rows[0]["InvoiceNumber"]);
            string Invpath = Convert.ToString(dt.Rows[0]["Invpath"]);
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Filename + ".pdf");
            // String.Format("attachment;filename={0}", "attachment.pdf"));
            Response.ContentType = "application/pdf";
            Response.TransmitFile(Server.MapPath(Invpath));
            Response.End();
        }

        public DataTable GetInvoiceDetails(string ProjectNumber, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[IB_usp_GetInvoiceDetails_ForRead_QC]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectNumber", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

  

        protected void btnShowFilter_Click(object sender, EventArgs e)
        {
            //if (ddlMonth.SelectedIndex == 0)
            //    BindGridSummary();
            //else
            //    BindGridSummaryMonthWise();

            BindSummaryProjectWise(txtSummaryFromDate.Text.Trim(), txtSummaryToDate.Text.Trim());
        }




        public void BindSummaryProjectWise(string FromDate, string ToDate)
        {
            
            if (!string.IsNullOrEmpty(FromDate.Trim()) && !string.IsNullOrEmpty(ToDate.Trim()))
            {
                DataTable dt = GetALLProjectVolumeInOnlineTrackingSheet(Convert.ToDateTime(FromDate.Trim()).ToString("dd-MMM-yyyy"), Convert.ToDateTime(ToDate.Trim()).ToString("dd-MMM-yyyy"), Convert.ToInt32(HttpContext.Current.User.Identity.Name));

                if (dt.Rows.Count > 0)
                {
                    grdSummaryDetails.DataSource = dt;
                    grdSummaryDetails.DataBind();
                    Session["SummaryProjectWise"] = dt;
                }
                else
                {

                }
            }

        }

        public DataTable GetALLProjectVolumeInOnlineTrackingSheet(string FormDate, string ToDate, int EmployeeID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "WBT_usp_GetALLProjectVolume_KRL_Test_KIP_New_Costing");
            SQLHelper.AddParamToSQLCmd(cmd, "@FromDate", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, FormDate);
            SQLHelper.AddParamToSQLCmd(cmd, "@ToDate", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ToDate);
            SQLHelper.AddParamToSQLCmd(cmd, "@EmployeeID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, EmployeeID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }


      
        public DataTable BindSummaryReportMonthwise(string Month, string Year)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "WBT_usp_GetAllGroupSummaryReportForBinding_AddedDate_ForExport"); //WBT_usp_GetAllGroupSummaryReportForBinding
            SQLHelper.AddParamToSQLCmd(cmd, "@Month", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, Month);
            SQLHelper.AddParamToSQLCmd(cmd, "@Year", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, Year);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }


     

        public override void VerifyRenderingInServerForm(Control control) { }

        public void WriteResponsePdf(HttpResponse response, byte[] filearray, string type)
        {
            response.ClearContent();
            response.Buffer = true;
            response.Cache.SetCacheability(HttpCacheability.Private);
            response.ContentType = "application/pdf";
            ContentDisposition contentDisposition = new ContentDisposition();
            contentDisposition.FileName = "Billing Summary Report.pdf";
            contentDisposition.DispositionType = type;
            response.AddHeader("Content-Disposition", contentDisposition.ToString());
            response.BinaryWrite(filearray);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            try
            {
                response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
            }

        }

        public static void WriteResponseExcel(HttpResponse response, byte[] filearray, string type)
        {
            response.ClearContent();
            response.Buffer = true;
            response.Cache.SetCacheability(HttpCacheability.Private);
            response.ContentType = "application/octet-stream";
            ContentDisposition contentDisposition = new ContentDisposition();
            contentDisposition.FileName = "Billing Summary Report.xlsx";
            contentDisposition.DispositionType = type;
            response.AddHeader("Content-Disposition", contentDisposition.ToString());
            response.BinaryWrite(filearray);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            try
            {
                response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
            }

        }
    }
}
