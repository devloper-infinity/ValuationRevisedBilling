using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using DevExpress.Web;
using System.Collections;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using CrystalDecisions.Shared;
using System.IO;
using BillingOther.App_Code.DAL;
using BillingOther.App_Code.BLL;
using System.ComponentModel;

namespace BillingOther.Accounts
{
    public partial class InvoiceViewer : System.Web.UI.Page
    {
        private bool IsInlinePreview
        {
            get { return string.Equals(Request.QueryString["Preview"], "1", StringComparison.Ordinal); }
        }

        private void SendGeneratedPdf(string filePath, string fileName)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", (IsInlinePreview ? "inline" : "attachment") + "; filename=\"" + fileName.Replace("\"", string.Empty) + ".pdf\"");
            Response.TransmitFile(filePath);
            Context.ApplicationInstance.CompleteRequest();
        }

        public string ProjectID;
        public string BillingPeriod;
        public string ProjectName;
        public string InvoiceId;
        public string InvoiceNumber;
        public static string DomainId;
        bllTracking blltracking = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            ProjectID = Convert.ToString(Request.QueryString["ProjectID"]);
            BillingPeriod = Convert.ToString(Request.QueryString["BillingPeriod"]);
            InvoiceId = Convert.ToString(Request.QueryString["InvoiceId"]);
            ProjectName = Convert.ToString(Request.QueryString["ProjectName"]);
            //BindXML();
            //// Time_To_Show_The_Report();
            // //GenerateReport(int.Parse(ProjectID),  BillingPeriod);
            DataTable dt = GetProjectClientConfiguration(int.Parse(ProjectID));

            if (dt.Rows.Count > 0)
            {
                string InvoiceConfig = dt.Rows[0]["InvoiceConfiguration"].ToString();
                DomainId = dt.Rows[0]["DomainId"].ToString();
                //ProjectName = dt.Rows[0]["ProjectName"].ToString();
                if (int.Parse(ProjectID) == 391)
                    Time_To_Show_The_DetailedReport_183002();
                else if (int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 87)
                    BindFTEReport();
                else if (DomainId == "19")
                    GetTypingInvoice();
                else if (int.Parse(DomainId) == 36 || int.Parse(DomainId) == 31 || int.Parse(DomainId) == 35 || int.Parse(DomainId) == 18 || int.Parse(DomainId) == 34)
                {
                    if (int.Parse(ProjectID) == 395 || int.Parse(ProjectID) == 391 || int.Parse(ProjectID) == 331 || int.Parse(ProjectID) == 337 || int.Parse(ProjectID) == 352 || int.Parse(ProjectID) == 414 || int.Parse(ProjectID) == 456 || int.Parse(ProjectID) == 328 || int.Parse(ProjectID) == 613 || int.Parse(ProjectID) == 626)
                        Time_To_Show_The_DetailedReport_183002();
                    else
                        BindFTEReport();
                }
                else
                    Time_To_Show_The_DetailedReport_183002();
            }
            else
            {
                Time_To_Show_The_DetailedReport();
            }
        }

        public void Time_To_Show_The_DetailedReport_183002()
        {
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
                else if (ProjectName == "733-002" || ProjectName == "772" || ProjectName == "736" || ProjectName == "791-002" || ProjectName == "409-002" || ProjectName == "409-005" || ProjectName == "572" || ProjectName == "733-004")
                    rpt.Load(Server.MapPath("~/Reports/Freight/733-002.rpt"));
                else if (ProjectName == "736-002")
                    rpt.Load(Server.MapPath("~/Reports/Freight/733-002.rpt"));
                else if (ProjectName == "754")
                    rpt.Load(Server.MapPath("~/Reports/Freight/754.rpt"));
                else if (ProjectName == "757-003")
                    rpt.Load(Server.MapPath("~/757-003.rpt"));
                else if (ProjectName == "694-008" || ProjectName == "694-005" || ProjectName == "715")
                    rpt.Load(Server.MapPath("~/Reports/Title_Other/694-008.rpt"));
                else if (int.Parse(ProjectID) == 620)
                    rpt.Load(Server.MapPath("~/Reports/WholeLoan/WholeLoan.rpt"));
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


                ILSReport.RefreshReport();
                ILSReport.Visible = true;
                ILSReport.HasExportButton = false;
                ILSReport.HasPrintButton = false;
                ILSReport.HasPageNavigationButtons = true;
                ILSReport.HasCrystalLogo = false;
                ILSReport.HasDrillUpButton = false;
                ILSReport.HasSearchButton = false;

                ILSReport.HasToggleGroupTreeButton = false;
                ILSReport.HasZoomFactorList = false;
                ILSReport.ToolbarStyle.Width = new Unit("750px");
                ILSReport.ReportSource = rpt;
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
                int result = blltracking.UpdateInvoicePath(int.Parse(InvoiceId), Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"));

                SendGeneratedPdf(filePath, filename);
            }
            catch (Exception ex) { throw ex; }

        }
        public DataTable GetBillingBase(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_BillingBase");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
        public void GetTypingInvoice()
        {
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
                        if (int.Parse(ProjectID) == 11 || int.Parse(ProjectID) == 154 || int.Parse(ProjectID) == 453 || int.Parse(ProjectID) == 177 || int.Parse(ProjectID) == 175 || int.Parse(ProjectID) == 370 || int.Parse(ProjectID) == 455 || int.Parse(ProjectID) == 421 || int.Parse(ProjectID) == 454 || int.Parse(ProjectID) == 474 || int.Parse(ProjectID) == 279)
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


                        //if (ProjectName == "711")
                        //    rpt.Load(Server.MapPath("~/711_Preview.rpt"));
                        //else if (ProjectName == "722")
                        //    rpt.Load(Server.MapPath("~/722.rpt"));
                        //else if (ProjectName == "733-002" || ProjectName == "772")
                        //    rpt.Load(Server.MapPath("~/733-002.rpt"));
                        //else if (ProjectName == "754")
                        //    rpt.Load(Server.MapPath("~/754.rpt"));
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


                        ILSReport.RefreshReport();
                        ILSReport.Visible = true;
                        ILSReport.HasExportButton = false;
                        ILSReport.HasPrintButton = false;
                        ILSReport.HasPageNavigationButtons = true;
                        ILSReport.HasCrystalLogo = false;
                        ILSReport.HasDrillUpButton = false;
                        ILSReport.HasSearchButton = false;

                        ILSReport.HasToggleGroupTreeButton = false;
                        ILSReport.HasZoomFactorList = false;
                        ILSReport.ToolbarStyle.Width = new Unit("750px");
                        ILSReport.ReportSource = rpt;
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
                        int result = blltracking.UpdateInvoicePath(int.Parse(InvoiceId), Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"));

                        SendGeneratedPdf(filePath, filename);
                        // Response.Redirect("SentToClient.aspx");
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }
        public DataSet GetFTEWeekendHours(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetFTEWeekendHours]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
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

        public string GetBillableHours_FTE(int ProjectID)
        {
            string BillableHours = "";
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetBillableHours]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            BillableHours = Convert.ToString(SQLHelper.ExecuteScalarCmdBilling(cmd));
            //DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return BillableHours;
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
        public string GetApprovedFTECount(int ProjectID)
        {
            string BillableHours = "";
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetApprovedFTECount]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            BillableHours = Convert.ToString(SQLHelper.ExecuteScalarCmdBilling(cmd));
            //DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return BillableHours;
        }
        public static DataTable PropertiesToDataTable<T>(List<T> source)
        {
            DataTable dt = new DataTable();
            var props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                DataColumn dc = dt.Columns.Add(prop.Name, prop.PropertyType);
                dc.Caption = prop.DisplayName;
                dc.ReadOnly = prop.IsReadOnly;
            }
            foreach (T item in source)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyDescriptor prop in props)
                {
                    dr[prop.Name] = prop.GetValue(item);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public void BindFTEReport()
        {
            DataTable dtDyn = new DataTable();
            dtDyn.Columns.Add("Description");
            dtDyn.Columns.Add("Total # of Hours");
            dtDyn.Columns.Add("Rate in USD");
            dtDyn.Columns.Add("Total Charges in US $");
            dtDyn.Columns.Add("Before Decimal");
            dtDyn.Columns.Add("After Decimal");
            DataSet dtAmt = GetTotalProjectAmount_FTE(int.Parse(ProjectID), BillingPeriod);
            string BillableHours = GetBillableHours_FTE(int.Parse(ProjectID));
            string ApprovedFTECount = GetApprovedFTECount(int.Parse(ProjectID));
            BillableHours = BillableHours == "" ? "0" : BillableHours;
            ApprovedFTECount = ApprovedFTECount == "" ? "0" : ApprovedFTECount;
            //            BillableHours = BillableHours == "" ? "0" : BillableHours;
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
                                TotalHours1 = Convert.ToDecimal(ApprovedFTECount) * Convert.ToDecimal(Average);
                        }
                        //TotalHours1 = Convert.ToDecimal(3) * Convert.ToDecimal(Average);
                        else
                        {
                            if (BillingPeriod == "01-Mar-2022 ~ 31-Mar-2022")
                                TotalHours1 = 738;
                            else if (BillingPeriod == "01-Sep-2022 ~ 30-Sep-2022")
                                TotalHours1 = 1080;
                            else
                                TotalHours1 = Convert.ToDecimal(ApprovedFTECount) * Convert.ToDecimal(Average);
                        }
                        string TotalFTEHours = TotalHours1.ToString();
                        //decimal TotalHours2 = Convert.ToDecimal(2) * Convert.ToDecimal(Average);
                        string[] Period = BillingPeriod.Split('~');
                        string From = Period[0].Trim();
                        string To = Period[1].Trim();
                        object RowCount = dtTitle.AsEnumerable()
                          .Count(r => r.Field<string>("Auditor1 (Hours)") != "");
                        if (int.Parse(ProjectID) == 400)
                            dr["Description"] = ApprovedFTECount + " FTE charges between " + From + " to " + To + "";
                        else
                            dr["Description"] = ApprovedFTECount + " FTE charges between " + From + " to " + To + "";
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
            else if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 337 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 632)
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
                                    //dtTitle.Rows[i]["Total Charges"] = (Convert.ToDecimal(BillableHours) * Convert.ToDecimal(PCount)) * Rate;
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
            else if (int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 442 || int.Parse(ProjectID) == 531 || int.Parse(ProjectID) == 584)
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
                decimal Average;
                if (int.Parse(ProjectID) == 584)
                {
                    object Average1 = dtAmt.Tables[0].AsEnumerable()
                        .Where(r => r.Field<string>("Operator1(Hours)") != "Holiday" && r.Field<string>("Operator1(Hours)") != "" && r.Field<string>("Operator1(Hours)") != null)
                                  .Sum(x => Convert.ToDecimal(x["Operator1(Hours)"].ToString().Replace(":00:00", "")));

                    Average = Convert.ToDecimal(Average1);// *Convert.ToDecimal(MonthCont);
                    TotalFTEHours = Convert.ToString(Average1);
                    dr["Description"] = "Operator charges between " + From + " to " + To + " (" + Convert.ToString(Average1) + "/FTE Hours)";
                    //dr["Description"] = Convert.ToString(Average1);
                    dr["Total # of Hours"] = Convert.ToString(Average1);
                }
                else if (int.Parse(ProjectID) == 442)
                {
                    dr["Description"] = "Operator charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                    TotalFTEHours = Convert.ToString(1 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    dr["Total # of Hours"] = Convert.ToString(1 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
                else if (int.Parse(ProjectID) == 385)
                {
                    dr["Description"] = ApprovedFTECount + "  Operator charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                    TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    if (BillingPeriod == "01-May-2023 ~ 31-May-2023")
                    {
                        TotalFTEHours = "2583";
                        dr["Total # of Hours"] = "2583";
                    }
                    else if (BillingPeriod == "01-Jul-2023 ~ 31-Jul-2023")
                    {
                        TotalFTEHours = "1710";
                        dr["Total # of Hours"] = "1710";
                    }
                    else if (BillingPeriod == "01-Feb-2024 ~ 29-Feb-2024")
                    {
                        TotalFTEHours = "621";
                        dr["Total # of Hours"] = "621";
                    }
                    else if (BillingPeriod == "01-Jun-2024 ~ 30-Jun-2024")
                    {
                        TotalFTEHours = "549";
                        dr["Total # of Hours"] = "549";
                    }
                    else if (BillingPeriod == "01-Jul-2024 ~ 31-Jul-2024")
                    {
                        TotalFTEHours = "603";
                        dr["Total # of Hours"] = "603";
                    }
                    else if (BillingPeriod == "01-Aug-2024 ~ 31-Aug-2024")
                    {
                        TotalFTEHours = "630";
                        dr["Total # of Hours"] = "630";
                    }
                    else if (BillingPeriod == "01-Oct-2024 ~ 31-Oct-2024")
                    {
                        TotalFTEHours = "657";
                        dr["Total # of Hours"] = "657";
                    }
                    else if (BillingPeriod == "01-Nov-2024 ~ 30-Nov-2024")
                    {
                        TotalFTEHours = "549";
                        dr["Total # of Hours"] = "549";
                    }
                    else if (BillingPeriod == "01-Dec-2024 ~ 31-Dec-2024")
                    {
                        TotalFTEHours = "612";
                        dr["Total # of Hours"] = "612";
                    }
                    else if (BillingPeriod == "01-Feb-2025 ~ 28-Feb-2025")
                    {
                        TotalFTEHours = "576";
                        dr["Total # of Hours"] = "576";
                    }
                    else if (BillingPeriod == "01-Apr-2026 ~ 30-Apr-2026")
                    { 
                          TotalFTEHours = "738";
                       dr["Total # of Hours"] = "738";
                    }

                else
                {
                        TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                        dr["Total # of Hours"] = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    }
                }
                else if (int.Parse(ProjectID) == 531)
                {
                    if (BillingPeriod == "01-May-2023 ~ 31-May-2023")
                    {
                        dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                        //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                        dr["Total # of Hours"] = "387";
                        TotalFTEHours = "387";
                    }
                    else if (BillingPeriod == "01-Jun-2025 ~ 30-Jun-2025")
                    {
                        dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                        //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                        dr["Total # of Hours"] = "351";
                        TotalFTEHours = "351";
                    }
                    else
                    {
                        dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                        dr["Total # of Hours"] = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                        TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    }
                }
                else
                {
                    dr["Description"] = "2 Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                    TotalFTEHours = Convert.ToString(2 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
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
                decimal Average = 0;
                if (int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 205)
                {
                    var query = dt1.AsEnumerable()
                          .Where(r => r.Field<string>("BilledFTE") != "Holiday" && r.Field<string>("BilledFTE") != "" && r.Field<string>("BilledFTE") != null)
                          .GroupBy(dr251 => Convert.ToString(dr251["BilledFTE"]))
                          .Select(g => new
                          {
                              BilledFTE = g.Key,
                              MonthlyAverage = g.Sum(x => Convert.ToDecimal(x["BilledFTE"])),
                          }).ToList();

                    DataTable dtDistinct = PropertiesToDataTable(query);
                    object MonthlyAverageDistinct = dtDistinct.AsEnumerable()
                                  .Where(r => r.Field<string>("BilledFTE") != "Holiday" && r.Field<string>("BilledFTE") != "" && r.Field<string>("BilledFTE") != null)
                                  .Distinct().Average(x => Convert.ToDecimal(x["BilledFTE"]));

                    Average = Convert.ToDecimal(MonthlyAverageDistinct);
                    //Average = Convert.ToDecimal(MonthlyAverageDistinct) / Convert.ToDecimal(Cont);
                }
                else
                {
                    Average = Convert.ToDecimal(MonthlyAverage) / Convert.ToDecimal(Cont);
                }
                //decimal Average = Convert.ToDecimal(MonthlyAverage) / Convert.ToDecimal(Cont);
                if (int.Parse(ProjectID) == 205)
                {
                    if (BillingPeriod == "01-Oct-2022 ~ 31-Oct-2022")
                        dr["Description"] = Math.Round(Convert.ToDecimal(7.22), 2).ToString() + " FTE charges towards data entry process for the period " + From + " to " + To;
                    else
                        dr["Description"] = Math.Round(Average, 2).ToString() + " FTE charges towards data entry process for the period " + From + " to " + To;
                }
                else if (int.Parse(ProjectID) == 184)
                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont)) + "/FTE Hours) Legacy and Global";
                else
                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                //dr["MonthlyAverage"] = Math.Round(Average, 2).ToString();
                //lblMontlyAverage.Text = Math.Round(Convert.ToDecimal(MonthlyAverage), 2).ToString();
                // dr["FTEHours"] = Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont));
                string TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                if (int.Parse(ProjectID) == 205)
                {
                    if (BillingPeriod == "01-Oct-2022 ~ 31-Oct-2022")
                        TotalFTEHours = Convert.ToString(Math.Round(Convert.ToDecimal(7.22), 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    else
                        TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
                if (int.Parse(ProjectID) == 205)
                {
                    if (BillingPeriod == "01-Oct-2022 ~ 31-Oct-2022")
                        dr["Total # of Hours"] = Convert.ToString(Math.Round(Convert.ToDecimal(7.22), 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    else
                        dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
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
                else if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 442 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 205 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 337 || int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 531 || int.Parse(ProjectID) == 584 || int.Parse(ProjectID) == 632)
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


                ILSReport.RefreshReport();
                ILSReport.Visible = true;
                ILSReport.HasExportButton = false;
                ILSReport.HasPrintButton = false;
                ILSReport.HasPageNavigationButtons = true;
                ILSReport.HasCrystalLogo = false;
                ILSReport.HasDrillUpButton = false;
                ILSReport.HasSearchButton = false;

                ILSReport.HasToggleGroupTreeButton = false;
                ILSReport.HasZoomFactorList = false;
                ILSReport.ToolbarStyle.Width = new Unit("750px");
                ILSReport.ReportSource = rpt;
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

                SendGeneratedPdf(filePath, filename);
            }
            catch (Exception ex) { throw ex; }
        }

        public void Time_To_Show_The_DetailedReport()
        {
            try
            {
                DataSet dt = new DataSet();

                ReportDocument rpt = new ReportDocument();
                rpt.Load(Server.MapPath("~/DetailedReportNew.rpt"));
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = InvoiceId;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@InvoiceId"].ApplyCurrentValues(pval1);

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


                ILSReport.RefreshReport();
                ILSReport.Visible = true;
                ILSReport.HasExportButton = false;
                ILSReport.HasPrintButton = false;
                ILSReport.HasPageNavigationButtons = true;
                ILSReport.HasCrystalLogo = false;
                ILSReport.HasDrillUpButton = false;
                ILSReport.HasSearchButton = false;

                ILSReport.HasToggleGroupTreeButton = false;
                ILSReport.HasZoomFactorList = false;
                ILSReport.ToolbarStyle.Width = new Unit("750px");
                ILSReport.ReportSource = rpt;
                string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                string strTime = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string filename = ProjectID + "_" + BillingPeriod + "_" + strDate + strTime;
                //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);

                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath =
                    Server.MapPath("~/BillingDocuments/") + filename + ".pdf";

                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                int result = blltracking.UpdateInvoicePath(int.Parse(InvoiceId), Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"));

                SendGeneratedPdf(filePath, filename);
            }
            catch (Exception ex) { throw ex; }

        }
        public void Time_To_Show_The_SummaryReport()
        {
            try
            {
                DataSet dt = new DataSet();


                ReportDocument rpt = new ReportDocument();
                rpt.Load(Server.MapPath("/SummaryReport.rpt"));
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = InvoiceId;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@InvoiceId"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = ProjectID;
                pval2.Add(pdisval2);

                ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                pdisval3.Value = BillingPeriod;
                pval3.Add(pdisval3);


                //rpt.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval2);
                //rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval3);

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

                ILSReport.RefreshReport();
                ILSReport.Visible = true;
                ILSReport.HasExportButton = false;
                ILSReport.HasPrintButton = false;
                ILSReport.HasPageNavigationButtons = true;
                ILSReport.HasCrystalLogo = false;
                ILSReport.HasDrillUpButton = false;
                ILSReport.HasSearchButton = false;

                ILSReport.HasToggleGroupTreeButton = false;
                ILSReport.HasZoomFactorList = false;
                ILSReport.ToolbarStyle.Width = new Unit("750px");
                ILSReport.ReportSource = rpt;
                // string filename = ProjectNo + "_" + OrderNo + "_" + strDate + strTime;
                rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, @"D:\Invoice.pdf");

                // rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"D:\myReport.pdf");
            }
            catch (Exception ex) { throw ex; }

        }

        public void BindXML()
        {
            DataSet myDS = new DataSet();
            DataTable dtMyTable = new DataTable("preview");

            DataColumn myCol0 = new DataColumn("facility");
            myCol0.DataType = System.Type.GetType("System.String");
            myCol0.MaxLength = 256;
            myCol0.AllowDBNull = true;

            DataColumn myCol1 = new DataColumn("doctype");
            myCol1.DataType = System.Type.GetType("System.String");
            myCol1.MaxLength = 256;
            myCol1.AllowDBNull = true;

            dtMyTable.Columns.Add(myCol0);
            dtMyTable.Columns.Add(myCol1);
            dtMyTable.AcceptChanges();

            DataRow myNewRow = dtMyTable.NewRow();
            myNewRow["facility"] = "MyFacility Works Great!";
            myNewRow["doctype"] = "MyDocType Field Does Too!";
            dtMyTable.Rows.Add(myNewRow);
            dtMyTable.AcceptChanges();

            myDS.Tables.Add(dtMyTable);
            DataSet dt;
            dt = blltracking.GetAllProjectSendToAccounts(int.Parse(ProjectID), BillingPeriod);//blltracking.GetAllProjectSendToAccounts(20,  '16-Apr-2018 ~ 30-Apr-2018');
            foreach (DataColumn column in dt.Tables[0].Columns)
            {
                string cName = dt.Tables[0].Rows[0][column.ColumnName].ToString();
                if (!dt.Tables[0].Columns.Contains(cName) && cName != "")
                {
                    column.ColumnName = cName;
                }

            }

            dt.Tables[0].Rows[0].Delete();
            dt.WriteXml(@"C:\Users\WKS\Downloads\tryXML.xml");

            CrystalDecisions.CrystalReports.Engine.ReportDocument myReportDocument;
            myReportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //myReportDocument.SetDatabaseLogon("sa", "idt15central", "192.168.1.12,8989", "InfinityBilling");

            //myReportDocument.SetDataSource(dt.Tables[0]);
            myReportDocument.Load(Server.MapPath("/TestReport.rpt"));
            CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();
            CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
            CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

            ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
            pdisval1.Value = InvoiceId;
            pval1.Add(pdisval1);

            ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
            pdisval2.Value = ProjectID;
            pval2.Add(pdisval2);

            ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
            pdisval3.Value = BillingPeriod;
            pval3.Add(pdisval3);

            myReportDocument.DataDefinition.ParameterFields["@InvoiceId"].ApplyCurrentValues(pval1);
            myReportDocument.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval2);
            myReportDocument.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval3);


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

            CrTables = myReportDocument.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }

            ILSReport.ReportSource = dt;
            ILSReport.DataBind();
            ILSReport.DisplayToolbar = false;

            ILSReport.RefreshReport();
            ILSReport.Visible = true;
            ILSReport.HasExportButton = false;
            ILSReport.HasPrintButton = false;
            ILSReport.HasPageNavigationButtons = true;
            ILSReport.HasCrystalLogo = false;
            ILSReport.HasDrillUpButton = false;
            ILSReport.HasSearchButton = false;

            ILSReport.HasToggleGroupTreeButton = false;
            ILSReport.HasZoomFactorList = false;
            ILSReport.ToolbarStyle.Width = new Unit("750px");

            // string filename = ProjectNo + "_" + OrderNo + "_" + strDate + strTime;
            myReportDocument.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, @"D:\Invoice.pdf");
        }

        public DataTable GetProjectClientConfiguration(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "IB_usp_GetProjectWiseClientInvoiceConfiguration_1");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetInvoiceNumber(string ProjectNumber, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetInvocieNumber_QC");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectGroup", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, ProjectNumber);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }
    }
}
