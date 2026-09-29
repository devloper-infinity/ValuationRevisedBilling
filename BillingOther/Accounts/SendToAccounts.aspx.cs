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
using System.Data.SqlClient;
using System.Configuration;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;
using System.ComponentModel;

namespace BillingOther.Accounts
{
    public partial class SendToAccounts : System.Web.UI.Page
    {
        public string ProjectID;
        public string BillingPeriod;
        public string ProjectName;
        public string OrderNo;
        public string InvoiceNumber;
        DataTable dt = new DataTable();
        DataTable dtSelectedRows1 = new DataTable();
        DataTable dtSelectedRows = new DataTable();
        bllTracking blltracking = new bllTracking();
        SendMail sendEmail = new SendMail();
        public string OrderDateTempColumn = string.Empty;
        public string TempColumnForOrderNumber = string.Empty;
        List<object> SelectedRows;
        public int SelectedRowsForClient = 0;
        public int verifiedorders = 0;
        public int SelectedOrders = 0;
        List<object> SendToVerify;
        Decimal TotalPrice = 0;
        int Index = 0;
        string ProjectGroup = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Order Details";
            ProjectID = Convert.ToString(Request.QueryString["ProjectID"]);
            BillingPeriod = Convert.ToString(Request.QueryString["BillingPeriod"]);
            ProjectName = Convert.ToString(Request.QueryString["ProjectName"]);

            lblProject.Text = ProjectName;
            lblBillingPeriod.Text = BillingPeriod;

            if (Session["dtReport"] != null)
            {
                DataTable dttt = (DataTable)Session["dtReport"];
                try
                {
                    grdBilling.DataSource = (DataTable)Session["dtReport"];
                    grdBilling.DataBind();

                    GridViewDataHyperLinkColumn Edit = new GridViewDataHyperLinkColumn();
                    Edit.Caption = "Edit";
                    Edit.DataItemTemplate = new MyHyperlinkTemplate();
                    Edit.Settings.FilterMode = ColumnFilterMode.DisplayText;
                    Edit.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
                    try
                    {
                        GridViewColumn obj = grdBilling.Columns["Edit"];
                        if (obj != null)
                        {
                            grdBilling.Columns.Remove(obj);
                        }
                    }
                    catch { }
                    grdBilling.Columns.Add(Edit);
                    Edit.VisibleIndex = 0;
                }
                catch { }
            }
            if (!IsPostBack)
            {
                Session["dtReport"] = null;
                // BindGrid();
                btnShowVerified_Click(null, null);
                int DomainId = GetDomainIdfromProject(int.Parse(ProjectID));
                if (DomainId == 2 || DomainId == 19 || DomainId == 4 || DomainId == 31 || DomainId == 35 || DomainId == 18 || DomainId == 34 || DomainId == 7 || DomainId == 3 || DomainId == 5 || DomainId == 17 || DomainId == 36)
                {
                    invoice.Style.Add("display", "");
                    sendback.Style.Add("display", "none");
                }
                else
                {
                    invoice.Style.Add("display", "none");
                    sendback.Style.Add("display", "");
                }
            }
        }

        public void BindGrid()
        {
            #region Bind Billing Orders
            grdBilling.Columns.Clear();
            DataSet dt = new DataSet();
            dt = blltracking.GetAllProjectSendToAccounts(int.Parse(ProjectID), BillingPeriod);
            OrderDateTempColumn = blltracking.getActualColumnName("Final Status", int.Parse(ProjectID));
            grdBilling.AutoGenerateColumns = true;
            dt.Tables[0].AcceptChanges();
            grdBilling.DataSource = dt.Tables[0];
            grdBilling.DataBind();
            lblRecords.Text = dt.Tables[0].Rows.Count.ToString();
            grdBilling.Visible = true;

            grdBilling.DataSource = dt;
            grdBilling.DataBind();
            grdBilling.Visible = true;
            for (int i = 0; i < dt.Tables[0].Columns.Count; i++)
            {
                grdBilling.Columns[i].Caption = Convert.ToString(dt.Tables[0].Rows[0][i]);
                grdBilling.Columns[i].Width = Unit.Pixel(150);
            }
            dt.Tables[0].Rows.RemoveAt(0);
            dt.Tables[0].AcceptChanges();
            grdBilling.DataSource = dt.Tables[0];
            grdBilling.DataBind();
            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]) != -1)
            {
                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]));
            }
            lblRecords.Text = "Total Orders:" + Convert.ToString(dt.Tables[0].Rows.Count);
            txtOrder.Text = Convert.ToString(dt.Tables[0].Rows.Count);
            hdnTotalRows.Value = Convert.ToString(dt.Tables[0].Rows.Count);
            Session["dtReport"] = dt.Tables[0];
            #endregion
            grdBilling.Selection.SelectAll();

            #region count the Staus wise orders from selected orders
            grdTemplate.Columns.Clear();
            grdTemplate.AutoGenerateColumns = true;
            grdTemplate.DataSource = GetProjectwiseSummaryDetails(ProjectID, BillingPeriod);
            grdTemplate.DataBind();
            #endregion
        }

        public DataSet GetTotalProjectAmount_Freight736(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectCost3_HighestFirst_Freight_Revised_Test736]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
        }
        public DataSet GetTotalProjectAmount_WholeLoan(int ProjectID, string BillingPeriod, string ProjectName)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectCost3_HighestFirst_WholeLoan]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectName", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, ProjectName);
            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
        }
        public DataSet GetTotalProjectAmount_Search(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectCost3_HighestFirst_Search]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);


            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
        }

        public void BindGridTest()
        {
            try
            {
                DataTable dtNew = GetInvoiceNumber(int.Parse(ProjectID), ProjectName, BillingPeriod);
                if (dtNew != null)
                {
                    if (dtNew.Rows.Count > 0)
                    {
                        lblInvoiceNo.Text = Convert.ToString(dtNew.Rows[0]["InvoiceNumber"]);
                    }
                    else
                    {
                        lblInvoiceNo.Text = "";
                    }
                }
            }
            catch { }
            grdBilling.Columns.Clear();
            DataSet dt = new DataSet();
            DataSet dtAmt = new DataSet();
            // dt = blltracking.GetAllProjectSendToAccounts(int.Parse(ProjectID), BillingPeriod);

            DataTable dtDomain = blltracking.GetDomianFromProject(Convert.ToInt32(ProjectID));
            if (Convert.ToInt32(ProjectID) == 337 || Convert.ToInt32(ProjectID) == 388)
            {
                dvStatus.Style.Add("display", "none");
                dtAmt = GetTotalProjectAmount_Search(int.Parse(ProjectID), BillingPeriod);
                if (dtAmt != null)
                {
                    if (dtAmt.Tables[1].Rows.Count > 0)
                    {
                        if (Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]) != "")
                        {
                            txtAmount.Text = Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]);
                        }

                        #region Bind Billing Orders

                        txtOrder.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        int ColCount = dtAmt.Tables[0].Columns.Count;
                        dtAmt.Tables[0].Columns["TotalCharges"].SetOrdinal(ColCount - 1);
                        OrderDateTempColumn = blltracking.getActualColumnName("Final Status", int.Parse(ProjectID));
                        grdBilling.AutoGenerateColumns = true;
                        // dt.Tables[0].AcceptChanges();
                        grdBilling.DataSource = dtAmt.Tables[0];
                        grdBilling.DataBind();
                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        grdBilling.Visible = true;

                        GridViewDataTextColumn sr = new GridViewDataTextColumn();
                        sr.FieldName = "Number";
                        sr.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        sr.Caption = "Sr. #";
                        sr.VisibleIndex = 0;
                        grdBilling.Columns.Add(sr);

                        for (int i = 0; i < dtAmt.Tables[0].Columns.Count; i++)
                        {
                            grdBilling.Columns[i].Width = Unit.Pixel(150);
                        }
                        try
                        {
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]) != -1)
                            {
                                grdBilling.Columns["TrackingSheetID"].Visible = false;
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]));
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]));
                            }
                            lblRecords.Text = "Total Orders:" + Convert.ToString(dtAmt.Tables[0].Rows.Count);
                            if (Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]) != "")
                            {
                                txtOrder.Text = Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]);
                            }
                            hdnTotalRows.Value = Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        }
                        catch { }
                        Session["dtReport"] = dtAmt.Tables[0];

                        DataSet ds2 = new DataSet();

                        ds2.Tables.Add(dtAmt.Tables[0].Copy());
                        #endregion


                    }
                }
            }
            else if (Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 39)
            {
                dvStatus.Style.Add("display", "none");
                dtAmt = GetTotalProjectAmount_WholeLoan(int.Parse(ProjectID), BillingPeriod, ProjectName);
                if (dtAmt != null)
                {
                    if (dtAmt.Tables[1].Rows.Count > 0)
                    {
                        if (Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]) != "")
                        {
                            txtAmount.Text = Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]);
                        }
                        txtOrder.Text = dtAmt.Tables[0].Rows.Count.ToString();

                        grdBilling.AutoGenerateColumns = true;
                        grdBilling.DataSource = dtAmt.Tables[0];
                        grdBilling.DataBind();
                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        grdBilling.Visible = true;

                        Session["dtReport"] = dtAmt.Tables[0];
                    }
                }
            }
            else if (Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 2 && Convert.ToInt32(ProjectID) != 87 && Convert.ToInt32(ProjectID) != 203 && Convert.ToInt32(ProjectID) != 400 && Convert.ToInt32(ProjectID) != 373 && Convert.ToInt32(ProjectID) != 385)
            {
                dvStatus.Style.Add("display", "none");
                if (int.Parse(ProjectID) == 227)
                    dtAmt = GetTotalProjectAmount_Freight736(int.Parse(ProjectID), BillingPeriod);
                else
                    dtAmt = blltracking.GetTotalProjectAmount_Freight(int.Parse(ProjectID), BillingPeriod);
                if (dtAmt != null)
                {
                    if (dtAmt.Tables[1].Rows.Count > 0)
                    {
                        if (Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]) != "")
                        {
                            txtAmount.Text = Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]);
                        }

                        #region Bind Billing Orders

                        txtOrder.Text = dtAmt.Tables[0].Rows.Count.ToString();

                        OrderDateTempColumn = blltracking.getActualColumnName("Final Status", int.Parse(ProjectID));
                        grdBilling.AutoGenerateColumns = true;
                        grdBilling.DataSource = dtAmt.Tables[0];
                        grdBilling.DataBind();
                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        grdBilling.Visible = true;

                        GridViewDataTextColumn sr = new GridViewDataTextColumn();
                        sr.FieldName = "Number";
                        sr.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        sr.Caption = "Sr. #";
                        sr.VisibleIndex = 0;
                        grdBilling.Columns.Add(sr);

                        for (int i = 0; i < dtAmt.Tables[0].Columns.Count; i++)
                        {
                            grdBilling.Columns[i].Width = Unit.Pixel(150);
                        }
                        try
                        {
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]) != -1)
                            {
                                grdBilling.Columns["TrackingSheetID"].Visible = false;
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]));
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]));
                            }
                            lblRecords.Text = "Total Orders:" + Convert.ToString(dtAmt.Tables[0].Rows.Count);
                            if (Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]) != "")
                            {
                                txtOrder.Text = Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]);
                            }
                            hdnTotalRows.Value = Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        }
                        catch { }
                        Session["dtReport"] = dtAmt.Tables[0];

                        GridViewDataHyperLinkColumn Edit = new GridViewDataHyperLinkColumn();
                        Edit.Caption = "Edit";
                        Edit.DataItemTemplate = new MyHyperlinkTemplate();
                        Edit.Settings.FilterMode = ColumnFilterMode.DisplayText;
                        Edit.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
                        grdBilling.Columns.Add(Edit);
                        Edit.VisibleIndex = 0;
                        DataSet ds2 = new DataSet();
                        ds2.Tables.Add(dtAmt.Tables[0].Copy());
                        #endregion

                        #region count the Staus wise orders from selected orders
                        //DataTable dtTemplate = GetProjectwiseSummaryDetails_Freight(ProjectID, BillingPeriod);
                        //if (dtTemplate != null)
                        //{
                        //    if (dtTemplate.Rows.Count > 0)
                        //    {
                        //        HtmlTableRow tr = new HtmlTableRow();
                        //        HtmlTableCell td = new HtmlTableCell();
                        //        for (int i = 0; i < dtTemplate.Rows.Count; i++)
                        //        {
                        //            tr = new HtmlTableRow();
                        //            for (int col = 0; col < dtTemplate.Columns.Count; col++)
                        //            {
                        //                td = new HtmlTableCell();
                        //                td.Style.Add("font-weight", "bold");
                        //                td.Attributes.Add("align", "right");
                        //                td.Style.Add("width", "50%");
                        //                td.InnerHtml = Convert.ToString(dtTemplate.Columns[col].Caption);
                        //                tr.Cells.Add(td);
                        //                td = new HtmlTableCell();
                        //                td.Style.Add("width", "50%");
                        //                td.InnerHtml = Convert.ToString(dtTemplate.Rows[i][col]);
                        //                tr.Cells.Add(td);
                        //            }
                        //            tblTemplate.Rows.Add(tr);
                        //        }
                        //    }
                        //}
                        grdTemplate.Columns.Clear();
                        grdTemplate.AutoGenerateColumns = true;
                        grdTemplate.DataSource = GetProjectwiseSummaryDetails_Freight(ProjectID, BillingPeriod);
                        grdTemplate.DataBind();
                        #endregion
                    }
                }
            }
            else if (Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 36)
            {
                dvStatus.Style.Add("display", "none");
                dtAmt = GetTotalProjectAmount_FTE(int.Parse(ProjectID), BillingPeriod);
                string BillableHours = GetBillableHours_FTE(int.Parse(ProjectID));
                string ApprovedFTECount = GetApprovedFTECount(int.Parse(ProjectID));
                BillableHours = BillableHours == "" ? "0" : BillableHours;
                ApprovedFTECount = ApprovedFTECount == "" ? "0" : ApprovedFTECount;
                if (dtAmt != null)
                {
                    if (dtAmt.Tables[1].Rows.Count > 0)
                    {
                        if (Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]) != "")
                        {
                            txtAmount.Text = Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]);
                        }

                        #region Bind Billing Orders

                        txtOrder.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        int ColCount = dtAmt.Tables[0].Columns.Count;
                        grdBilling.AutoGenerateColumns = true;
                        if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 327 || int.Parse(ProjectID) == 632)
                        {
                            try
                            {
                                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                                if (dtRate != null)
                                {
                                    if (dtRate.Rows.Count > 0)
                                    {
                                        decimal Rate = Convert.ToDecimal(dtRate.Rows[0]["Rate"]);
                                        DataTable dtTitle = dtAmt.Tables[0].Copy();
                                        dtTitle.Columns.Add("No Of Hours");
                                        dtTitle.Columns.Add("Price");
                                        dtTitle.Columns.Add("Total Charges");
                                        dtTitle.Columns.Add("TrackingSheetID");
                                        int PCount = 0;
                                        if (int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 386)
                                        {
                                            for (int i = 0; i < dtTitle.Rows.Count; i++)
                                            {
                                                dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                            }
                                            int FTCount = dtTitle.Rows.Count;
                                            txtAmount.Text = Convert.ToString(Convert.ToDecimal(Rate) * Convert.ToDecimal(FTCount));
                                        }
                                        else
                                        {
                                            for (int i = 0; i < dtTitle.Rows.Count; i++)
                                            {
                                                PCount = 0;
                                                for (int j = 2; j < ColCount; j++)
                                                {
                                                    if (Convert.ToString(dtAmt.Tables[0].Rows[i][j]) == "P")
                                                        PCount++;
                                                }
                                                if (int.Parse(ProjectID) == 123)
                                                {
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
                                                            NoOfHours = GetFTEHours(int.Parse(ProjectID), Convert.ToString(dtTitle.Rows[i]["Employee Name"]), Convert.ToString(dtTitle.Rows[i]["Process"]), Convert.ToString(lblBillingPeriod.Text));
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

                                                    dtTitle.Rows[i]["Price"] = Rate;
                                                    dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                                }
                                                else
                                                {
                                                    string NoOfHours = "";
                                                    try
                                                    {
                                                        NoOfHours = GetFTEHours(int.Parse(ProjectID), Convert.ToString(dtTitle.Rows[i]["Employee Name"]), Convert.ToString(dtTitle.Rows[i]["Process"]), Convert.ToString(lblBillingPeriod.Text));
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
                                                    dtTitle.Rows[i]["Price"] = Rate;
                                                    dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                                }
                                            }
                                            txtAmount.Text = dtTitle.AsEnumerable()
                                                            .Where(r => r.Field<string>("Total Charges") != "")
                                                            .Sum(x => Convert.ToDecimal(x["Total Charges"])).ToString();
                                        }

                                        Session["dtReport"] = dtTitle;
                                        grdBilling.DataSource = dtTitle;
                                        grdBilling.DataBind();
                                        GridViewDataHyperLinkColumn Edit = new GridViewDataHyperLinkColumn();
                                        Edit.Caption = "Edit";
                                        Edit.DataItemTemplate = new MyHyperlinkTemplateFTE();
                                        Edit.Settings.FilterMode = ColumnFilterMode.DisplayText;
                                        Edit.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
                                        grdBilling.Columns.Add(Edit);
                                        Edit.VisibleIndex = 0;

                                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                                        grdBilling.Visible = true;
                                        if (int.Parse(ProjectID) == 354)
                                        {
                                            Object NoofFTEBO = dtTitle.AsEnumerable()
                                              .Where(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Back Office FTE")
                                              .Sum(x => Convert.ToDecimal(x["No Of Hours"])).ToString();
                                            Object NoofFTEVM = dtTitle.AsEnumerable()
                                              .Where(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Vendor Management")
                                              .Sum(x => Convert.ToDecimal(x["No Of Hours"])).ToString();

                                            Object CountBO = dtTitle.AsEnumerable()
                                              .Count(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Back Office FTE");
                                            Object CountVM = dtTitle.AsEnumerable()
                                              .Count(r => r.Field<string>("No Of Hours") != "" && r.Field<string>("Process") == "Vendor Management");

                                            DataTable dtDyn = new DataTable();
                                            dtDyn.Columns.Add("Process");
                                            dtDyn.Columns.Add("FTE");
                                            dtDyn.Columns.Add("Total Hours");

                                            DataRow dr = dtDyn.NewRow();
                                            dr["Process"] = "Back Office";
                                            dr["FTE"] = CountBO.ToString();
                                            dr["Total Hours"] = NoofFTEBO.ToString();
                                            dtDyn.Rows.Add(dr);

                                            dr = dtDyn.NewRow();
                                            dr["Process"] = "Vendor Management";
                                            dr["FTE"] = CountVM.ToString();
                                            dr["Total Hours"] = NoofFTEVM.ToString();
                                            dtDyn.Rows.Add(dr);

                                            grdTemplate.Columns.Clear();
                                            grdTemplate.AutoGenerateColumns = true;
                                            grdTemplate.DataSource = dtDyn;// GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                                            grdTemplate.DataBind();
                                        }
                                        else
                                        {
                                            Object NoofFTE = dtTitle.AsEnumerable()
                                              .Where(r => r.Field<string>("No Of Hours") != "")
                                              .Sum(x => Convert.ToDecimal(x["No Of Hours"])).ToString();

                                            DataTable dtDyn = new DataTable();
                                            dtDyn.Columns.Add("FTE");
                                            dtDyn.Columns.Add("Total Hours");
                                            DataRow dr = dtDyn.NewRow();
                                            dr["FTE"] = dtAmt.Tables[0].Rows.Count.ToString();
                                            dr["Total Hours"] = NoofFTE.ToString();
                                            dtDyn.Rows.Add(dr);
                                            grdTemplate.Columns.Clear();
                                            grdTemplate.AutoGenerateColumns = true;
                                            grdTemplate.DataSource = dtDyn;// GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                                            grdTemplate.DataBind();
                                        }
                                    }
                                    else
                                    {
                                        grdBilling.DataSource = null;
                                        grdBilling.DataBind();
                                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                                        grdBilling.Visible = true;
                                    }
                                }
                                else
                                {
                                    grdBilling.DataSource = null;
                                    grdBilling.DataBind();
                                    lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                                    grdBilling.Visible = true;
                                }
                            }
                            catch { }
                        }
                        else if (int.Parse(ProjectID) == 87)
                        {
                            Session["dtReport"] = dtAmt.Tables[0];
                            grdBilling.DataSource = dtAmt.Tables[0];
                            grdBilling.DataBind();
                            lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                            grdBilling.Visible = true;
                            DataTable dtDyn = new DataTable();
                            dtDyn.Columns.Add("InvoiceCount");
                            dtDyn.Columns.Add("TimeSpentMins");
                            dtDyn.Columns.Add("TimeSpentHrs");

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
                            dr["InvoiceCount"] = Convert.ToString(InvoiceCount);
                            dr["TimeSpentMins"] = Convert.ToString(TimeSpentMins);
                            dr["TimeSpentHrs"] = Convert.ToString(Math.Round(Convert.ToDecimal(TimeSpentHrs), 2));
                            dtDyn.Rows.Add(dr);
                            dr = dtDyn.NewRow();
                            grdTemplate.Columns.Clear();
                            grdTemplate.AutoGenerateColumns = true;
                            grdTemplate.DataSource = dtDyn;// GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                            grdTemplate.DataBind();
                            DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                            if (dtRate != null)
                            {
                                if (dtRate.Rows.Count > 0)
                                {
                                    decimal Rate = Convert.ToDecimal(dtRate.Rows[0]["Rate"]);
                                    txtAmount.Text = Convert.ToString(Math.Round(Convert.ToDecimal(TimeSpentHrs), 2) * Math.Round(Rate, 2));
                                }
                            }
                        }
                        else
                        {
                            Session["dtReport"] = dtAmt.Tables[0];
                            grdBilling.DataSource = dtAmt.Tables[0];
                            grdBilling.DataBind();
                            lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                            grdBilling.Visible = true;
                        }
                        if (int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 205)
                        {
                            // Standard HTML grid renders the calculated FTE values without DevExpress cell merging.
                        }
                        if (int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393)
                        {
                            for (int i = 0; i < grdBilling.Columns.Count; i++)
                            {
                                try
                                {
                                    grdBilling.Columns[i].Width = Unit.Pixel(150);
                                }
                                catch
                                {
                                    dvError.Style.Add("display", "");
                                    dvError.Attributes.Add("class", "alert alert-warning background-warning");
                                    dvError.InnerHtml = "Please configure project.";
                                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < dtAmt.Tables[0].Columns.Count; i++)
                            {
                                try
                                {
                                    //grdBilling.Columns[i].Caption = Convert.ToString(dt.Tables[0].Rows[0][i]);
                                    grdBilling.Columns[i].Width = Unit.Pixel(150);
                                }
                                catch
                                {
                                    dvError.Style.Add("display", "");
                                    dvError.Attributes.Add("class", "alert alert-warning background-warning");
                                    dvError.InnerHtml = "Please configure project.";
                                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                                }
                            }
                        }
                        GridViewDataTextColumn sr = new GridViewDataTextColumn();
                        sr.FieldName = "Number";
                        sr.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        sr.Caption = "Sr. #";
                        sr.VisibleIndex = 0;
                        grdBilling.Columns.Add(sr);

                        try
                        {
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]) != -1)
                            {
                                grdBilling.Columns["TrackingSheetID"].Visible = false;
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]));
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]));
                            }
                            lblRecords.Text = "Total Orders:" + Convert.ToString(dtAmt.Tables[0].Rows.Count);
                            if (Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]) != "")
                            {
                                txtOrder.Text = Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]);
                            }
                            hdnTotalRows.Value = Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        }
                        catch { }
                        DataSet ds2 = new DataSet();

                        ds2.Tables.Add(dtAmt.Tables[0].Copy());
                        #endregion
                        if (int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400)
                        {
                            DataTable dtTitle = dtAmt.Tables[0].Copy();
                            DataTable averageDetails = GetFTEAverageBillingDetails(int.Parse(ProjectID), BillingPeriod);
                            int MonthCont = CountFteWorkingRows(dtTitle, averageDetails, "Auditor1 (Hours)");
                            decimal Average = Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont);
                            decimal TotalHours1 = 0;
                            if (int.Parse(ProjectID) == 400)
                            {
                                if (BillingPeriod == "01-Sep-2022 ~ 30-Sep-2022")
                                    TotalHours1 = 972;
                                else
                                    TotalHours1 = Convert.ToDecimal(ApprovedFTECount) * Convert.ToDecimal(Average);
                            }
                            else
                            {
                                if (BillingPeriod == "01-Mar-2022 ~ 31-Mar-2022")
                                    TotalHours1 = 738;
                                else if (BillingPeriod == "01-Sep-2022 ~ 30-Sep-2022")
                                    TotalHours1 = 1080;
                                else
                                    TotalHours1 = Convert.ToDecimal(6) * Convert.ToDecimal(Average);
                            }
                            DataTable dtDyn = new DataTable();
                            dtDyn.Columns.Add("Billable Standard Hours");
                            dtDyn.Columns.Add("Total FTE Hours");
                            DataRow dr = dtDyn.NewRow();
                            dr["Billable Standard Hours"] = Average.ToString();
                            dr["Total FTE Hours"] = TotalHours1.ToString();
                            dtDyn.Rows.Add(dr);
                            dr = dtDyn.NewRow();

                            grdTemplate.Columns.Clear();
                            grdTemplate.AutoGenerateColumns = true;
                            grdTemplate.DataSource = dtDyn;// GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                            grdTemplate.DataBind();
                            try
                            {
                                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                                if (dtRate != null)
                                {
                                    if (dtRate.Rows.Count > 0)
                                    {
                                        txtAmount.Text = Convert.ToString(Convert.ToDecimal(TotalHours1) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                                    }
                                }
                            }
                            catch { }
                        }
                        else if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 87 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 327 || int.Parse(ProjectID) == 632)
                        {

                        }
                        else if (int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 442 || int.Parse(ProjectID) == 531 || int.Parse(ProjectID) == 584)
                        {
                            DataTable dtDyn = new DataTable();
                            dtDyn.Columns.Add("FTE Hours");
                            dtDyn.Columns.Add("Total Hours");
                            #region count the Criteria wise orders from selected orders
                            int MonthCont = CountFteWorkingRows(dtAmt.Tables[0], null, "Operator1(Hours)");

                            DataRow dr = dtDyn.NewRow();

                            string TotalFTEHours = "";// Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont))));
                            decimal Average;
                            if (int.Parse(ProjectID) == 584)
                            {
                                decimal Average1 = SumFteHours(dtAmt.Tables[0], "Operator1(Hours)");

                                Average = Convert.ToDecimal(Average1);// *Convert.ToDecimal(MonthCont);
                                TotalFTEHours = Convert.ToString(Average1);
                                dr["FTE Hours"] = Convert.ToString(Average1);
                                dr["Total Hours"] = Convert.ToString(Average1);
                            }

                            else if (int.Parse(ProjectID) == 442)
                            {
                                TotalFTEHours = Convert.ToString(1 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                dr["Total Hours"] = Convert.ToString(1 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                            }
                            else
                            {
                                if (int.Parse(ProjectID) == 385)
                                {
                                    if (BillingPeriod == "01-Jul-2022 ~ 31-Jul-2022")
                                    {
                                        TotalFTEHours = "1350";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "1350";
                                    }
                                    else if (BillingPeriod == "01-Nov-2022 ~ 30-Nov-2022")
                                    {
                                        TotalFTEHours = "1773";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "1773";
                                    }
                                    else if (BillingPeriod == "01-May-2023 ~ 31-May-2023")
                                    {
                                        TotalFTEHours = "2583";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "2583";
                                    }
                                    else if (BillingPeriod == "01-Jul-2023 ~ 31-Jul-2023")
                                    {
                                        TotalFTEHours = "1710";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "1710";
                                    }
                                    else if (BillingPeriod == "01-Feb-2024 ~ 29-Feb-2024")
                                    {
                                        TotalFTEHours = "621";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "621";
                                    }
                                    else if (BillingPeriod == "01-Jun-2024 ~ 30-Jun-2024")
                                    {
                                        TotalFTEHours = "549";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "549";
                                    }
                                    else if (BillingPeriod == "01-Jul-2024 ~ 31-Jul-2024")
                                    {
                                        TotalFTEHours = "603";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "603";
                                    }
                                    else if (BillingPeriod == "01-Aug-2024 ~ 31-Aug-2024")
                                    {
                                        TotalFTEHours = "630";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "630";
                                    }
                                    else if (BillingPeriod == "01-Oct-2024 ~ 31-Oct-2024")
                                    {
                                        TotalFTEHours = "657";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "657";
                                    }
                                    else if (BillingPeriod == "01-Nov-2024 ~ 30-Nov-2024")
                                    {
                                        TotalFTEHours = "549";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "549";
                                    }
                                    else if (BillingPeriod == "01-Dec-2024 ~ 31-Dec-2024")
                                    {
                                        TotalFTEHours = "612";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "612";
                                    }
                                    else if (BillingPeriod == "01-Feb-2025 ~ 28-Feb-2025")
                                    {
                                        TotalFTEHours = "576";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "576";
                                    }
                                    else if (BillingPeriod == "01-Apr-2026 ~ 30-Apr-2026")
                                    {
                                        TotalFTEHours = "738";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "738";
                                    }
                                    else
                                    {
                                        TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                    }

                                }
                                else if (int.Parse(ProjectID) == 531)
                                {
                                    if (BillingPeriod == "01-May-2023 ~ 31-May-2023")
                                    {
                                        TotalFTEHours = "387";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "387";
                                    }
                                    else
                                    if (BillingPeriod == "01-Jun-2025 ~ 30-Jun-2025")
                                    {
                                        TotalFTEHours = "351";
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = "351";
                                    }
                                    else
                                    {
                                        TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                        dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                        dr["Total Hours"] = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                    }

                                }
                                else
                                {
                                    TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                    dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                                    dr["Total Hours"] = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                }

                            }
                            dtDyn.Rows.Add(dr);
                            grdTemplate.Columns.Clear();
                            grdTemplate.AutoGenerateColumns = true;
                            grdTemplate.DataSource = dtDyn;// GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                            grdTemplate.DataBind();
                            try
                            {
                                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                                if (dtRate != null)
                                {
                                    if (dtRate.Rows.Count > 0)
                                    {
                                        txtAmount.Text = Convert.ToString(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                                    }
                                }
                            }
                            catch { }
                            #endregion
                        }
                        else
                        {
                            DataTable dtDyn = new DataTable();
                            dtDyn.Columns.Add("Monthly Average");
                            dtDyn.Columns.Add("FTE Hours");
                            dtDyn.Columns.Add("Total Hours");
                            #region count the Criteria wise orders from selected orders
                            DataTable dt1 = dtAmt.Tables[0].DefaultView.ToTable(false, "BilledFTE");

                            object Cont = dt1.AsEnumerable()
                                          .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                            object MonthCont = dtAmt.Tables[0].AsEnumerable()
                                          .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                            object MonthlyAverage = dt1.AsEnumerable()
                                          .Where(r => r.Field<string>("BilledFTE") != "Holiday")
                                          .Sum(x => Convert.ToDecimal(x["BilledFTE"]));
                            DataRow dr = dtDyn.NewRow();
                            decimal Average = 0;
                            if (int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 205)
                            {
                                var query = dt1.AsEnumerable()
                                      .Where(r => r.Field<string>("BilledFTE") != "Holiday" && r.Field<string>("BilledFTE") != "" && r.Field<string>("BilledFTE") != null)
                                      .GroupBy(dr25 => Convert.ToString(dr25["BilledFTE"]))
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
                                    dr["Monthly Average"] = "7.22";
                                else
                                    dr["Monthly Average"] = Math.Round(Average, 3).ToString();
                                dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            }
                            else
                            {
                                dr["Monthly Average"] = Math.Round(Average, 2).ToString();
                                dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            }
                            string TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                            if (int.Parse(ProjectID) == 205)
                            {
                                if (BillingPeriod == "01-Oct-2022 ~ 31-Oct-2022")
                                    TotalFTEHours = Convert.ToString(Convert.ToDecimal(7.22) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                else
                                    TotalFTEHours = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                            }
                            if (int.Parse(ProjectID) == 205)
                            {
                                if (BillingPeriod == "01-Oct-2022 ~ 31-Oct-2022")
                                    dr["Total Hours"] = Convert.ToString(Convert.ToDecimal(7.22) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                else
                                    dr["Total Hours"] = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                            }
                            else
                                dr["Total Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                            dtDyn.Rows.Add(dr);
                            grdTemplate.Columns.Clear();
                            grdTemplate.AutoGenerateColumns = true;
                            grdTemplate.DataSource = dtDyn;// GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                            grdTemplate.DataBind();
                            try
                            {
                                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                                if (dtRate != null)
                                {
                                    if (dtRate.Rows.Count > 0)
                                    {
                                        txtAmount.Text = Convert.ToString(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                                    }
                                }
                            }
                            catch { }
                            #endregion
                        }
                        #region count the Staus wise orders from selected orders
                        if (int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 205)
                        {
                            try
                            {
                                DataTable dtWeekend1 = GetFTEWeekendHours(int.Parse(ProjectID), BillingPeriod).Tables[0].DefaultView.ToTable(true, "Date");
                                DataTable dtWeekend = GetFTEWeekendHours(int.Parse(ProjectID), BillingPeriod).Tables[0].DefaultView.ToTable(true, "BilledFTE");
                                if (dtWeekend != null)
                                {
                                    if (dtWeekend.Rows.Count > 0)
                                    {

                                        DataTable dt1 = dtWeekend.DefaultView.ToTable(true, "BilledFTE");

                                        object Cont = dt1.AsEnumerable()
                                                      .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                                        object MonthCont = dtWeekend.AsEnumerable()
                                                      .Count(r => r.Field<string>("BilledFTE") != "Holiday");
                                        object MonthlyAverage = dt1.AsEnumerable()
                                                      .Where(r => r.Field<string>("BilledFTE") != "Holiday")
                                                      .Sum(x => Convert.ToDecimal(x["BilledFTE"]));
                                        DataTable dtDyn = new DataTable();
                                        dtDyn.Columns.Add("Weekend Monthly Average");
                                        dtDyn.Columns.Add("Weekend FTE Hours");
                                        dtDyn.Columns.Add("Weekend Total Hours");
                                        DataRow dr = dtDyn.NewRow();
                                        decimal Average = Convert.ToDecimal(MonthlyAverage) / Convert.ToDecimal(Cont);
                                        string TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                        if (int.Parse(ProjectID) == 205)
                                        {
                                            MonthCont = dtWeekend1.AsEnumerable()
                                                      .Count(r => r.Field<string>("Date") != "Holiday");
                                            TotalFTEHours = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                            dr["Weekend Monthly Average"] = Math.Round(Average, 3).ToString();
                                            dr["Weekend FTE Hours"] = Convert.ToString(Convert.ToDecimal(8) * Convert.ToDecimal(MonthCont));
                                            dr["Weekend Total Hours"] = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                        }
                                        else
                                        {
                                            dr["Weekend Monthly Average"] = Math.Round(Average, 2).ToString();
                                            dr["Weekend FTE Hours"] = Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont));
                                            dr["Weekend Total Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                                        }
                                        dtDyn.Rows.Add(dr);
                                        dvStatus.Style.Add("display", "");
                                        grdStatus.Columns.Clear();
                                        grdStatus.AutoGenerateColumns = true;
                                        grdStatus.DataSource = dtDyn;
                                        grdStatus.DataBind();
                                        string WeekendHours = "";
                                        try
                                        {
                                            DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                                            if (dtRate != null)
                                            {
                                                if (dtRate.Rows.Count > 0)
                                                {
                                                    WeekendHours = Convert.ToString(Convert.ToDecimal(TotalFTEHours) * Convert.ToDecimal(dtRate.Rows[0]["Rate"]));
                                                }
                                            }
                                            if (WeekendHours != "")
                                            {
                                                txtAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text) + Convert.ToDecimal(WeekendHours));
                                            }
                                        }
                                        catch { }

                                    }
                                }
                            }
                            catch { }

                        }
                        #endregion

                    }
                }
            }
            else if (Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 19)
            {
                dvStatus.Style.Add("display", "");
                dtAmt = blltracking.GetTotalProjectAmount_Typing(int.Parse(ProjectID), BillingPeriod);
                if (dtAmt != null)
                {
                    if (dtAmt.Tables[1].Rows.Count > 0)
                    {
                        if (Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]) != "")
                        {
                            txtAmount.Text = Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]);
                        }

                        #region Bind Billing Orders

                        txtOrder.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        int ColCount = dtAmt.Tables[0].Columns.Count;
                        dtAmt.Tables[0].Columns["TotalCharges"].SetOrdinal(ColCount - 1);
                        OrderDateTempColumn = blltracking.getActualColumnName("Final Status", int.Parse(ProjectID));
                        grdBilling.AutoGenerateColumns = true;
                        grdBilling.DataSource = dtAmt.Tables[0];
                        grdBilling.DataBind();
                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        grdBilling.Visible = true;

                        GridViewDataTextColumn sr = new GridViewDataTextColumn();
                        sr.FieldName = "Number";
                        sr.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        sr.Caption = "Sr. #";
                        sr.VisibleIndex = 0;
                        grdBilling.Columns.Add(sr);

                        for (int i = 0; i < dtAmt.Tables[0].Columns.Count; i++)
                        {
                            grdBilling.Columns[i].Width = Unit.Pixel(150);
                        }
                        try
                        {
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]) != -1)
                            {
                                grdBilling.Columns["TrackingSheetID"].Visible = false;
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]));
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]));
                            }
                            lblRecords.Text = "Total Orders:" + Convert.ToString(dtAmt.Tables[0].Rows.Count);
                            if (Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]) != "")
                            {
                                txtOrder.Text = Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalInvoice"]);
                            }
                            hdnTotalRows.Value = Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        }
                        catch { }
                        Session["dtReport"] = dtAmt.Tables[0];

                        GridViewDataHyperLinkColumn Edit = new GridViewDataHyperLinkColumn();
                        Edit.Caption = "Edit";
                        Edit.DataItemTemplate = new MyHyperlinkTemplate();
                        Edit.Settings.FilterMode = ColumnFilterMode.DisplayText;
                        Edit.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
                        grdBilling.Columns.Add(Edit);
                        Edit.VisibleIndex = 0;

                        DataSet ds2 = new DataSet();

                        ds2.Tables.Add(dtAmt.Tables[0].Copy());
                        #endregion

                        #region count the Criteria wise orders from selected orders

                        grdTemplate.Columns.Clear();
                        grdTemplate.AutoGenerateColumns = true;
                        grdTemplate.DataSource = GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                        grdTemplate.DataBind();

                        #endregion

                        #region count the Staus wise orders from selected orders

                        grdStatus.Columns.Clear();
                        grdStatus.AutoGenerateColumns = true;
                        grdStatus.DataSource = GetStatuswiseSummaryDetails(ProjectID, BillingPeriod);
                        grdStatus.DataBind();

                        #endregion

                    }
                }
            }
            else if (Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 3 || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 17 || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 5)
            {
                dvStatus.Style.Add("display", "none");
                if (int.Parse(ProjectID) == 39)
                    dtAmt = GetTotalProjectAmount_Others(int.Parse(ProjectID), BillingPeriod);
                else
                    dtAmt = GetTotalProjectAmount_Insurance(int.Parse(ProjectID), BillingPeriod);
                if (dtAmt != null)
                {
                    if (dtAmt.Tables[1].Rows.Count > 0)
                    {
                        if (Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]) != "")
                        {
                            if (int.Parse(ProjectID) == 39)
                                txtAmount.Text = Convert.ToString(dtAmt.Tables[0].Rows[0]["TotalCharges"]);
                            else
                                txtAmount.Text = Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]);
                        }

                        #region Bind Billing Orders

                        txtOrder.Text = dtAmt.Tables[0].Rows.Count.ToString();

                        OrderDateTempColumn = blltracking.getActualColumnName("Final Status", int.Parse(ProjectID));
                        grdBilling.AutoGenerateColumns = true;
                        grdBilling.DataSource = dtAmt.Tables[0];
                        grdBilling.DataBind();
                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        grdBilling.Visible = true;

                        GridViewDataTextColumn sr = new GridViewDataTextColumn();
                        sr.FieldName = "Number";
                        sr.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        sr.Caption = "Sr. #";
                        sr.VisibleIndex = 0;
                        grdBilling.Columns.Add(sr);

                        for (int i = 0; i < dtAmt.Tables[0].Columns.Count; i++)
                        {
                            grdBilling.Columns[i].Width = Unit.Pixel(150);
                        }
                        try
                        {
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]) != -1)
                            {
                                grdBilling.Columns["TrackingSheetID"].Visible = false;
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TotalInvoice"]) != -1)
                            {
                                grdBilling.Columns["TotalInvoice"].Visible = false;
                            }

                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]));
                            }
                            if (int.Parse(ProjectID) == 39)
                            {
                                if (grdBilling.Columns.IndexOf(grdBilling.Columns["AdditionalRate"]) != -1)
                                {
                                    grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["AdditionalRate"]));
                                }
                                if (grdBilling.Columns.IndexOf(grdBilling.Columns["WireTransfer"]) != -1)
                                {
                                    grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["WireTransfer"]));
                                }
                                if (grdBilling.Columns.IndexOf(grdBilling.Columns["TotalCharges"]) != -1)
                                {
                                    grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["TotalCharges"]));
                                }
                            }
                        }
                        catch { }
                        if (int.Parse(ProjectID) == 39)
                        {
                            lblRecords.Text = "Total Orders:" + Convert.ToString(dtAmt.Tables[0].Rows[dtAmt.Tables[0].Rows.Count - 1]["TotalInvoice"]);
                            txtOrder.Text = Convert.ToString(dtAmt.Tables[0].Rows[dtAmt.Tables[0].Rows.Count - 1]["TotalInvoice"]);
                        }
                        else
                        {
                            lblRecords.Text = "Total Orders:" + Convert.ToString(dtAmt.Tables[0].Rows[dtAmt.Tables[0].Rows.Count - 1]["Total"]);
                            txtOrder.Text = Convert.ToString(dtAmt.Tables[0].Rows[dtAmt.Tables[0].Rows.Count - 1]["Total"]);
                        }
                        hdnTotalRows.Value = Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        Session["dtReport"] = dtAmt.Tables[0];

                        DataSet ds2 = new DataSet();

                        ds2.Tables.Add(dtAmt.Tables[0].Copy());
                        #endregion

                    }
                }
            }
            else
            {
                dvStatus.Style.Add("display", "none");
                dtAmt = blltracking.GetTotalProjectAmount(int.Parse(ProjectID), BillingPeriod);
                if (dtAmt != null)
                {
                    if (dtAmt.Tables[1].Rows.Count > 0)
                    {
                        if (Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]) != "")
                        {
                            txtAmount.Text = Convert.ToString(dtAmt.Tables[1].Rows[0]["TotalCost"]);
                        }

                        #region Bind Billing Orders

                        txtOrder.Text = dtAmt.Tables[0].Rows.Count.ToString();

                        OrderDateTempColumn = blltracking.getActualColumnName("Final Status", int.Parse(ProjectID));
                        grdBilling.AutoGenerateColumns = true;
                        grdBilling.DataSource = dtAmt.Tables[0];
                        grdBilling.DataBind();
                        lblRecords.Text = dtAmt.Tables[0].Rows.Count.ToString();
                        grdBilling.Visible = true;

                        GridViewDataTextColumn sr = new GridViewDataTextColumn();
                        sr.FieldName = "Number";
                        sr.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        sr.Caption = "Sr. #";
                        sr.VisibleIndex = 0;
                        grdBilling.Columns.Add(sr);

                        for (int i = 0; i < dtAmt.Tables[0].Columns.Count; i++)
                        {
                            grdBilling.Columns[i].Width = Unit.Pixel(150);

                        }
                        try
                        {
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["TrackingSheetID"]) != -1)
                            {
                                grdBilling.Columns["TrackingSheetID"].Visible = false;
                            }
                            if (grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]) != -1)
                            {
                                grdBilling.Columns.RemoveAt(grdBilling.Columns.IndexOf(grdBilling.Columns["ORDERNo"]));
                            }
                        }
                        catch { }

                        lblRecords.Text = "Total Orders:" + Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        txtOrder.Text = Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        hdnTotalRows.Value = Convert.ToString(dtAmt.Tables[0].Rows.Count);
                        Session["dtReport"] = dtAmt.Tables[0];

                        GridViewDataHyperLinkColumn Edit = new GridViewDataHyperLinkColumn();
                        Edit.Caption = "Edit";
                        Edit.DataItemTemplate = new MyHyperlinkTemplate();
                        Edit.Settings.FilterMode = ColumnFilterMode.DisplayText;
                        Edit.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
                        grdBilling.Columns.Add(Edit);
                        Edit.VisibleIndex = 0;
                        DataSet ds2 = new DataSet();

                        ds2.Tables.Add(dtAmt.Tables[0].Copy());
                        #endregion

                        #region count the Staus wise orders from selected orders

                        grdTemplate.Columns.Clear();
                        grdTemplate.AutoGenerateColumns = true;
                        grdTemplate.DataSource = GetProjectwiseSummaryDetails(ProjectID, BillingPeriod);
                        grdTemplate.DataBind();

                        #endregion
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

        public DataSet GetTotalProjectAmount_FTE(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectCost3_HighestFirst_FTE]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
        }
        public DataTable GetFTEAverageBillingDetails(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFTEAverageBillingData");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 10, ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 500, ParameterDirection.Input, Convert.ToString(BillingPeriod).Replace(" to ", " ~ "));
            return SQLHelper.ExecuteDataTableCmd(cmd);
        }
        private static int CountFteWorkingRows(DataTable billing, DataTable averages, string legacyColumn)
        {
            if (billing != null && billing.Columns.Contains(legacyColumn))
                return billing.AsEnumerable().Count(r => !String.Equals(Convert.ToString(r[legacyColumn]), "Holiday", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrWhiteSpace(Convert.ToString(r[legacyColumn])));
            if (averages != null && averages.Rows.Count > 0)
                return averages.AsEnumerable().Count(r => !averages.Columns.Contains("AverageFTE") || r["AverageFTE"] != DBNull.Value);
            if (billing != null && billing.Columns.Contains("AssignTarget"))
                return billing.AsEnumerable().Count(r => !String.Equals(Convert.ToString(r["AssignTarget"]), "Holiday", StringComparison.OrdinalIgnoreCase));
            return billing == null ? 0 : billing.Rows.Count;
        }
        private static decimal SumFteHours(DataTable billing, string columnName)
        {
            if (billing == null || !billing.Columns.Contains(columnName)) return 0;
            decimal total = 0;
            foreach (DataRow row in billing.Rows)
            {
                string value = Convert.ToString(row[columnName]);
                decimal number;
                if (!String.Equals(value, "Holiday", StringComparison.OrdinalIgnoreCase) && Decimal.TryParse(value.Replace(":00:00", ""), out number)) total += number;
            }
            return total;
        }
        public DataSet GetTotalProjectAmount_Insurance(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectCost3_HighestFirst_Insurance]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return dt;
        }
        public DataSet GetTotalProjectAmount_Others(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectCost3_HighestFirst_Freight_Revised_Test_2]");
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

        class MyHyperlinkTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                ASPxHyperLink link = new ASPxHyperLink();
                GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
                link.NavigateUrl = "javascript:void(0);";
                link.ID = "Link_" + gridContainer.KeyValue;
                link.ClientSideEvents.Click = string.Format("function(s, e) {{ EditOrder('{0}','{1}'); }}", link, gridContainer.KeyValue);// "EditOrder(" + link + ", 'TrackingSheetID')";
                link.Text = string.Format("Edit", gridContainer.KeyValue);
                container.Controls.Add(link);
            }
        }

        class MyHyperlinkTemplateFTE : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                ASPxHyperLink link = new ASPxHyperLink();
                GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
                link.NavigateUrl = "javascript:void(0);";
                link.ID = "Link_" + gridContainer.KeyValue;
                link.ClientSideEvents.Click = string.Format("function(s, e) {{ EditOrderFTE('{0}','{1}'); }}", link, gridContainer.KeyValue);// "EditOrder(" + link + ", 'TrackingSheetID')";
                link.Text = string.Format("Edit", gridContainer.KeyValue);
                container.Controls.Add(link);
            }
        }

        protected void grdBilling_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            ASPxGridView grdBilling = sender as ASPxGridView;
            int Id = Convert.ToInt32(e.Keys[0]);
            int ReturnValue = 0;// blltracking.DeleteOrderInTrackingBy_TrackingSheetID(Id);
            if (ReturnValue > 0)
            {
            }
            e.Cancel = true;
        }

        protected void btnBackToProd_Click(object sender, EventArgs e)
        {

        }
        protected void btnShowVerified_Click(object sender, EventArgs e)
        {
            int result = blltracking.InsertAllProjectSendToAccountsDetails(int.Parse(ProjectID), BillingPeriod, int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            //BindGrid();
            BindGridTest();
        }

        protected void grdBilling_DataBound(object sender, EventArgs e)
        {
            ASPxGridView grdBilling = sender as ASPxGridView;
            grdBilling.SettingsBehavior.AllowSelectByRowClick = true;

            if (grdBilling.Columns.IndexOf(grdBilling.Columns["CheckBox"]) != -1)
            {
                return;
            }
            GridViewCommandColumn col = new GridViewCommandColumn();
            col.ShowSelectCheckbox = true;
            col.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.AllPages;
            col.Width = 50;
            col.VisibleIndex = 0;
            col.Caption = "CheckBox";
            grdBilling.Columns.Add(col);
        }

        protected void grdReport_HtmlRowCreated(object sender, DevExpress.Web.ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType == GridViewRowType.Data)
            {
                e.Row.Height = Unit.Pixel(15);
            }

        }

        protected void grdReport_HtmlCommandCellPrepared(object sender, DevExpress.Web.ASPxGridViewTableCommandCellEventArgs e)
        {
        }

        protected void grdReport_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxGridViewTableDataCellEventArgs e)
        {


            if (e.DataColumn.Caption.Contains("Rejections"))
            {
                e.Cell.ToolTip = e.CellValue.ToString();
                e.Cell.Height = Unit.Pixel(20);
                e.Cell.Width = Unit.Pixel(120);
            }
            if (e.DataColumn.Caption.ToString() == "Criteria")
            {
                e.Cell.BackColor = Color.Red;
                e.Cell.ForeColor = Color.White;
            }

        }

        public void GetselectedRows()
        {
            #region get selected rows
            List<string> fieldNames = new List<string>();
            foreach (GridViewColumn column in grdBilling.Columns)
            {
                string co = Convert.ToString(column);
                string capname = Convert.ToString(grdBilling.Columns[co].Caption);
                if (column is GridViewDataColumn)
                {
                    if (((GridViewDataColumn)column).FieldName != "")
                    {
                        fieldNames.Add(((GridViewDataColumn)column).FieldName);
                        dtSelectedRows.Columns.Add(co);
                    }
                }
                SendToVerify = grdBilling.GetSelectedFieldValues(fieldNames.ToArray());
            }

            #endregion

            #region create datatable from selected list<object> i.e selected rows
            int row = 0;
            foreach (object[] item in SendToVerify)
            {
                DataRow dr = dtSelectedRows.NewRow();
                dtSelectedRows.Rows.Add(dr);
                int column = 0;
                foreach (object value in item)
                {
                    dtSelectedRows.Rows[row][column] = value.ToString();
                    string v = value.ToString();
                    column++;
                }
                row++;
            }
            int OrderDatecolumnindex = dtSelectedRows.Columns.Contains("order Date") ? Convert.ToInt32(dtSelectedRows.Columns["order Date"].Ordinal) : -1;
            try
            {
                if (OrderDatecolumnindex >= 0)
                {
                    dtSelectedRows.Columns["Delivered Date"].SetOrdinal(OrderDatecolumnindex + 1);//set delivered date column next to order date column;
                }
            }
            catch { }
            dtSelectedRows.AcceptChanges();
            Session["dtSelectedRows"] = dtSelectedRows;
            SelectedOrders = dtSelectedRows.Rows.Count;
            #endregion
        }

        public void GetselectedRowsTosendtoClient()
        {
            #region get selected rows
            List<string> fieldNames = new List<string>();
            foreach (GridViewColumn column in grdBilling.Columns)
            {
                string co = Convert.ToString(column);
                string capname = Convert.ToString(grdBilling.Columns[co].Caption);
                if (column is GridViewDataColumn)
                {
                    fieldNames.Add(((GridViewDataColumn)column).FieldName);
                    dtSelectedRows.Columns.Add(co);
                }
                SelectedRows = grdBilling.GetSelectedFieldValues(fieldNames.ToArray());
            }

            #endregion

            #region create datatable from selected list<object> i.e selected rows
            int row = 0;
            foreach (object[] item in SelectedRows)
            {
                DataRow dr = dtSelectedRows.NewRow();
                dtSelectedRows.Rows.Add(dr);
                int column = 0;
                foreach (object value in item)
                {
                    dtSelectedRows.Rows[row][column] = value.ToString();
                    string v = value.ToString();
                    column++;
                }
                row++;
            }
            dtSelectedRows.AcceptChanges();
            Session["dtSelectedRows"] = dtSelectedRows;
            SelectedRowsForClient = dtSelectedRows.Rows.Count;
            #endregion
        }

        protected void btnSaveVerified_Click(object sender, EventArgs e)
        {
            GetselectedRows();
            if (SelectedOrders <= 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "Please select at least one order.";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                return;
            }

            dtSelectedRows = (DataTable)Session["dtSelectedRows"];

            if (dtSelectedRows != null)
            {
                #region bind project and temp column
                string UniqueColumn = "";
                try
                {
                    if (int.Parse(ProjectID) == 171)
                    {
                        OrderDateTempColumn = blltracking.getActualColumnName("order date", 83);
                        DataTable DtUniqueColumn = blltracking.GetIsUniqueColumnForHeader(83);
                        UniqueColumn = Convert.ToString(DtUniqueColumn.Rows[0][0]);
                        TempColumnForOrderNumber = blltracking.getActualColumnName(UniqueColumn, 83);
                    }
                    else if (int.Parse(ProjectID) == 350 || int.Parse(ProjectID) == 48 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 205 || int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 531 || int.Parse(ProjectID) == 620 || int.Parse(ProjectID) == 632)
                    {
                    }
                    else
                    {
                        OrderDateTempColumn = blltracking.getActualColumnName("order date", int.Parse(ProjectID));
                        DataTable DtUniqueColumn = blltracking.GetIsUniqueColumnForHeader(int.Parse(ProjectID));
                        UniqueColumn = Convert.ToString(DtUniqueColumn.Rows[0][0]);
                        TempColumnForOrderNumber = blltracking.getActualColumnName(UniqueColumn, int.Parse(ProjectID));
                    }
                }
                catch { }

                #endregion

                #region Update isverify flag in wbttracking sheet table
                try
                {
                    if (int.Parse(ProjectID) == 350 || int.Parse(ProjectID) == 48)
                    {
                    }
                    else
                    {
                        UpdateBulkOrders(dtSelectedRows);
                    }
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Order's has been verified successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
                catch { }
                #endregion
            }
        }

        public void SaveVerified_Merge(string NewBillingPeriod)
        {
            GetselectedRows();
            if (SelectedOrders <= 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "Please select at least one order.";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                return;
            }

            dtSelectedRows = (DataTable)Session["dtSelectedRows"];

            if (dtSelectedRows != null)
            {
                #region bind project and temp column
                string UniqueColumn = "";
                try
                {
                    if (int.Parse(ProjectID) == 171)
                    {
                        OrderDateTempColumn = blltracking.getActualColumnName("order date", 83);
                        DataTable DtUniqueColumn = blltracking.GetIsUniqueColumnForHeader(83);
                        UniqueColumn = Convert.ToString(DtUniqueColumn.Rows[0][0]);
                        TempColumnForOrderNumber = blltracking.getActualColumnName(UniqueColumn, 83);
                    }
                    else if (int.Parse(ProjectID) == 350 || int.Parse(ProjectID) == 48 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 205 || int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 531)
                    {
                    }
                    else
                    {
                        OrderDateTempColumn = blltracking.getActualColumnName("order date", int.Parse(ProjectID));
                        DataTable DtUniqueColumn = blltracking.GetIsUniqueColumnForHeader(int.Parse(ProjectID));
                        UniqueColumn = Convert.ToString(DtUniqueColumn.Rows[0][0]);
                        TempColumnForOrderNumber = blltracking.getActualColumnName(UniqueColumn, int.Parse(ProjectID));
                    }
                }
                catch { }

                #endregion

                #region Update isverify flag in wbttracking sheet table
                try
                {
                    {
                        UpdateBulkOrders_Merge(dtSelectedRows, NewBillingPeriod);
                    }
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Order's has been verified successfully!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
                catch { }
                #endregion
            }
        }

        public void GetOrderApproved()
        {
            DataTable dt = new DataTable();
            dt = blltracking.GetAllProjectApprovedforClient(int.Parse(ProjectID), BillingPeriod);

            verifiedorders = dt.Rows.Count;
        }

        public void UpdateBulkOrders(DataTable dt)
        {

            DataTable newTable = dt.DefaultView.ToTable(false, "Tracking Sheet ID");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["InfinityBilling"].ConnectionString))
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["InfinityBilling"].ConnectionString);
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();
                        //Creating temp table on database
                        command.CommandText = "usp_DropAllTypes";
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();

                        string sqlInsert = "IB_usp_SetIsverifyTrueForBillingOrder_Bulk";
                        connection.Open();
                        SqlCommand insertCommand = new SqlCommand(sqlInsert, connection);
                        insertCommand.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = insertCommand.Parameters.AddWithValue("@tmpInsert", newTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.tmpInsert";
                        insertCommand.ExecuteNonQuery();


                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                        connection.Close();
                    }

                }

            }
        }

        public void UpdateBulkOrders_Merge(DataTable dt, string NewBillingPeiod)
        {

            DataTable newTable = dt.DefaultView.ToTable(false, "Tracking Sheet ID");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["InfinityBilling"].ConnectionString))
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["InfinityBilling"].ConnectionString);
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();
                        //Creating temp table on database
                        command.CommandText = "usp_DropAllTypes_Merge";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("NewBillingPeriod", NewBillingPeiod);
                        command.Parameters.AddWithValue("ProjectID", int.Parse(ProjectID));
                        command.ExecuteNonQuery();

                        string sqlInsert = "IB_usp_SetIsverifyTrueForBillingOrder_Bulk_Merge";
                        connection.Open();
                        SqlCommand insertCommand = new SqlCommand(sqlInsert, connection);
                        insertCommand.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = insertCommand.Parameters.AddWithValue("@tmpInsert", newTable);
                        SqlParameter BParam = insertCommand.Parameters.AddWithValue("@NewBillingPeriod", NewBillingPeiod);
                        SqlParameter PParam = insertCommand.Parameters.AddWithValue("@ProjectID", int.Parse(ProjectID));
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.tmpInsert_Merge";
                        insertCommand.ExecuteNonQuery();


                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                        connection.Close();
                    }

                }

            }
        }

        protected void btnSendToClient_Click(object sender, EventArgs e)
        {
            int isPending = GetAllPendingPeriods(int.Parse(ProjectID), Convert.ToString(BillingPeriod));
            if (isPending == 0)
            {
                DataTable dtPending = GetActualPendingBillingPeriods(ProjectID, Convert.ToString(BillingPeriod));
                if (dtPending != null)
                {
                    grdPendingPeriod.DataSource = dtPending;
                    grdPendingPeriod.DataBind();

                }
                mdlBillingMerger.Show();

                return;
            }
            btnSaveVerified_Click(null, null);
            if (SelectedOrders <= 0)
            {
                if (grdBilling.VisibleRowCount <= 0)
                {
                }
                else
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-warning background-warning");
                    dvError.InnerHtml = "Please select at least one order.";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    return;
                }
            }
            if (grdBilling.VisibleRowCount > 0)
                GetOrderApproved();

            object SumAmount;
            try
            {
                if (grdBilling.VisibleRowCount > 0)
                {
                    if (dtSelectedRows != null)
                    {
                        if (ProjectID == "248")
                        {
                            SumAmount = dtSelectedRows.AsEnumerable()
                          .Where(r => r.Field<string>("Total Charges") != "")
                          .Sum(x => Convert.ToDecimal(x["Total Charges"]));
                        }
                        else
                        {
                            //SumAmount = dtSelectedRows.Compute("Sum(Price)", string.Empty);
                            SumAmount = dtSelectedRows.AsEnumerable()
                               .Where(r => r.Field<string>("Price") != "")
                               .Sum(x => Convert.ToDecimal(x["Price"]));
                        }
                    }
                    else
                    {
                        if (ProjectID == "248")
                        {
                            dtSelectedRows = (DataTable)Session["dtSelectedRows"];
                            //SumAmount = dtSelectedRows.Compute("Sum(Price)", string.Empty);
                            SumAmount = dtSelectedRows.AsEnumerable()
                              .Where(r => r.Field<string>("Total Charges") != "")
                              .Sum(x => Convert.ToDecimal(x["Total Charges"]));
                        }
                        else
                        {
                            dtSelectedRows = (DataTable)Session["dtSelectedRows"];
                            //SumAmount = dtSelectedRows.Compute("Sum(Price)", string.Empty);
                            SumAmount = dtSelectedRows.AsEnumerable()
                              .Where(r => r.Field<string>("Price") != "")
                              .Sum(x => Convert.ToDecimal(x["Price"]));
                        }
                    }
                }
                else
                    SumAmount = 0;
            }
            catch { SumAmount = txtAmount.Text; }
            if (ProjectID == "350" || ProjectID == "39" || ProjectID == "620")
                SumAmount = txtAmount.Text;
            try
            {
                DataTable dtDomain = blltracking.GetDomianFromProject(Convert.ToInt32(ProjectID));

                if (Convert.ToInt32(ProjectID) == 40 || Convert.ToInt32(ProjectID) == 203 || Convert.ToInt32(ProjectID) == 400 || Convert.ToInt32(ProjectID) == 373 || Convert.ToInt32(ProjectID) == 385 || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 31 || (Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 34 && int.Parse(ProjectID) != 391 && int.Parse(ProjectID) != 337) || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 35 || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 18 || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 36)
                    SumAmount = txtAmount.Text;
            }
            catch { }

            Hashtable Htparam = new Hashtable();

            Htparam.Add("ProjectID", Convert.ToString(ProjectID));
            Htparam.Add("BillingPeriod", Convert.ToString(BillingPeriod));
            Htparam.Add("ProjectName", Convert.ToString(Request.QueryString["ProjectName"]));
            Htparam.Add("Amount", Convert.ToString(SumAmount));
            Htparam.Add("Added_By", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            Htparam.Add("IsManual", chkInvoiceNo.Checked);
            Htparam.Add("InvoiceNoManual", lblInvoiceNo.Text);

            //Htparam.Add("OrderCount", Convert.ToString(txtOrder.Text));
            Htparam.Add("OrderCount", Convert.ToString(dtSelectedRows.Rows.Count));
            int result = 0;
            if (grdBilling.VisibleRowCount > 0)
            {
                result = blltracking.UpdateSendToclient(Htparam);
            }
            else
            {
                result = UpdateSendToclient_ZeroBilling(Htparam);
            }
            if (result > 0)
            {
                DataSet dt = blltracking.GetTotalProjectAmountForReport(int.Parse(ProjectID), BillingPeriod);
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "Project has been successfully sent to client!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                Response.Redirect("SendAccountDetails.aspx");
            }
            else if (result == 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "Error occured while sending project data to client!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
        }

        public int UpdateSendToclient_ZeroBilling(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[IB_usp_UpdateProjectApprovedforClient_ZeroBilling]");//IB_usp_UpdateProjectApprovedforClient
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["ProjectID"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, htParam["BillingPeriod"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectName", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, htParam["ProjectName"]);

            SQLHelper.AddParamToSQLCmd(cmd, "@Amount", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, htParam["Amount"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@OrderNo", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, htParam["OrderCount"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, htParam["Added_By"]);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Accounts/SendAccountDetails.aspx");
        }

        #region Database
        public int GetAllPendingPeriods(int ProjectID, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[GetPendingPeriod]");//IB_usp_UpdateProjectApprovedforClient
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        public int GetDomainIdfromProject(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetDomainIdfromProject]");//IB_usp_UpdateProjectApprovedforClient
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);

            int ReturnValue = Convert.ToInt32(SQLHelper.ExecuteScalarCmdBilling(cmd));
            return ReturnValue;
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

        public DataTable GetProjectwiseSummaryDetails(string ProjectId, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectSummaryReportForBilling]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetActualPendingBillingPeriods(string ProjectId, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[GetAllPendingPeriod]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetProjectwiseSummaryDetails_Typing(string ProjectId, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectSummaryReportForBilling_Typing]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetStatuswiseSummaryDetails(string ProjectId, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectStatuswiseReportForBilling_Freight]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetProjectwiseSummaryDetails_Freight(string ProjectId, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetProjectSummaryReportForBilling_Freight]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, ProjectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, BillingPeriod);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public int ChangeOrderNumber(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_UpdateOrderNumber]");//IB_usp_UpdateProjectApprovedforClient
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["ProjectID"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@OldOrderNo", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["OldOrderNo"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@OrderNo", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["OrderNo"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Remark", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["Remark"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.Int, 10, System.Data.ParameterDirection.Input, htParam["AddedBy"]);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        public int ChangeFTEHours(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[InsertFTEHours]");//IB_usp_UpdateProjectApprovedforClient
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htParam["ProjectID"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Employee", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["Employee"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Process", System.Data.SqlDbType.NVarChar, 200, System.Data.ParameterDirection.Input, htParam["Process"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@NoOfHours", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, htParam["NoOfHours"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 500, System.Data.ParameterDirection.Input, htParam["BillingPeriod"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Remark", System.Data.SqlDbType.NVarChar, 4000, System.Data.ParameterDirection.Input, htParam["Remark"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.Int, 10, System.Data.ParameterDirection.Input, htParam["AddedBy"]);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }
        #endregion

        protected void btnSendBackToProduction_Click(object sender, EventArgs e)
        {
            GetselectedRows();
            if (SelectedOrders != int.Parse(hdnTotalRows.Value))
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "Selected order's count does not match with total order's count";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                return;
            }

            int result = blltracking.InsertAllProjectSendToAccountsDetailsBackToProduction(int.Parse(ProjectID), BillingPeriod, int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            try
            {
                sendEmail.SendProductionData(ProjectName, BillingPeriod, txtSendBackToProductionRemark.Text.Trim());
            }
            catch { }
            dvError.Style.Add("display", "");
            dvError.Attributes.Add("class", "alert alert-success background-success");
            dvError.InnerHtml = "Project has been sent back to production!";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            DataTable export = grdBilling.GetDataTable();
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Regex.Replace(lblProject.Text + "_" + BillingPeriod, "[^A-Za-z0-9_-]", "_") + ".xls");
            StringBuilder output = new StringBuilder("<table border='1'><thead><tr>");
            foreach (DataColumn column in export.Columns) output.Append("<th>").Append(HttpUtility.HtmlEncode(column.ColumnName)).Append("</th>");
            output.Append("</tr></thead><tbody>");
            foreach (DataRow row in export.Rows)
            {
                output.Append("<tr>");
                foreach (DataColumn column in export.Columns) output.Append("<td>").Append(HttpUtility.HtmlEncode(Convert.ToString(row[column]))).Append("</td>");
                output.Append("</tr>");
            }
            Response.Write(output.Append("</tbody></table>").ToString());
            Response.End();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Hashtable htParam = new Hashtable();
            htParam.Add("ProjectID", int.Parse(ProjectID));
            htParam.Add("OldOrderNo", Convert.ToString(Session["OrderNo"]));
            htParam.Add("OrderNo", txtOrderNo.Text);
            htParam.Add("Remark", txtRemark.Text);
            htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            int ReturnValue = ChangeOrderNumber(htParam);
            if (ReturnValue > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "Order # changed successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "Error occured while changing order #.";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            BindGridTest();
        }

        protected void CallbackPanelTaxDetails_Callback(object sender, CallbackEventArgsBase e)
        {
            LoadOrderEditor(e.Parameter);
        }

        protected void btnLoadOrder_Click(object sender, EventArgs e)
        {
            LoadOrderEditor(hdnEditKey.Value);
            ClientScript.RegisterStartupScript(GetType(), "showOrderEditor", "$('#popupExcelReport').modal('show');", true);
        }

        private void LoadOrderEditor(string key)
        {
            int index = grdBilling.FindVisibleIndexByKeyValue(key);
            string OldOrderNo = Convert.ToString(grdBilling.GetRowValues(index, "Order#"));
            Session["OrderNo"] = OldOrderNo;
            txtOrderNo.Text = OldOrderNo;
            lblProjectNo.Text = lblProject.Text;
        }

        protected void btnPreviewInvoice_Click(object sender, EventArgs e)
        {
            BindPreview();
        }

        public void BindPreview()
        {
            DataTable dt = GetProjectClientConfiguration(int.Parse(ProjectID));
            if (dt.Rows.Count > 0)
            {
                string InvoiceConfig = dt.Rows[0]["InvoiceConfiguration"].ToString();
                int DomainId = Convert.ToInt32(dt.Rows[0]["DomainId"]);
                if (int.Parse(ProjectID) == 391)
                    Time_To_Show_The_DetailedReport_183002();
                else if (int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 87)
                    BindFTEReport();
                else if (DomainId == 4)
                    Time_To_Show_The_SummaryReport();
                else if (DomainId == 19)
                    BindTypingReport();
                else if (DomainId == 36 || DomainId == 31 || DomainId == 35 || DomainId == 18 || (DomainId == 34 && int.Parse(ProjectID) != 391 && int.Parse(ProjectID) != 331 && int.Parse(ProjectID) != 337 && int.Parse(ProjectID) != 352 && int.Parse(ProjectID) != 395 && int.Parse(ProjectID) != 414))
                    BindFTEReport();
                else
                    Time_To_Show_The_DetailedReport_183002();
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-warning");
                dvError.InnerHtml = "Project in not configured for billing.";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                return;
            }
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

        public string GetApprovedFTECount(int ProjectID)
        {
            string BillableHours = "";
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetApprovedFTECount]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            BillableHours = Convert.ToString(SQLHelper.ExecuteScalarCmdBilling(cmd));
            //DataSet dt = SQLHelper.ExecuteDataSetCmd_Billing(cmd);
            return BillableHours;
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
            if (int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400)
            {
                DataTable dtRate = GetCostingDetailsFTE(int.Parse(ProjectID));
                if (dtRate != null)
                {
                    if (dtRate.Rows.Count > 0)
                    {
                        DataRow dr = dtDyn.NewRow();
                        DataTable dtTitle = dtAmt.Tables[0].Copy();
                        DataTable averageDetails = GetFTEAverageBillingDetails(int.Parse(ProjectID), BillingPeriod);
                        int MonthCont = CountFteWorkingRows(dtTitle, averageDetails, "Auditor1 (Hours)");
                        decimal Average = Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont);
                        decimal TotalHours1 = 0;
                        if (int.Parse(ProjectID) == 400)
                            TotalHours1 = Convert.ToDecimal(ApprovedFTECount) * Convert.ToDecimal(Average);
                        else
                        {
                            if (BillingPeriod == "01-Mar-2022 ~ 31-Mar-2022")
                                TotalHours1 = 738;
                            else
                                TotalHours1 = Convert.ToDecimal(4) * Convert.ToDecimal(Average);
                        }
                        //decimal TotalHours2 = Convert.ToDecimal(2) * Convert.ToDecimal(Average);
                        string[] Period = BillingPeriod.Split('~');
                        string From = Period[0].Trim();
                        string To = Period[1].Trim();
                        int RowCount = CountFteWorkingRows(dtTitle, averageDetails, "Auditor1 (Hours)");
                        if (int.Parse(ProjectID) == 400)
                            dr["Description"] = ApprovedFTECount + " FTE charges between " + From + " to " + To + "";
                        else
                            dr["Description"] = ApprovedFTECount + " FTE charges between " + From + " to " + To + "";
                        dr["Total # of Hours"] = Convert.ToString(Convert.ToDecimal(TotalHours1));
                        // sumAmount = "5258.21";
                        try
                        {
                            dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                            dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(txtAmount.Text), 2));
                            dr["Before Decimal"] = Convert.ToInt32(Convert.ToDecimal(txtAmount.Text)).ToString();
                            dr["After Decimal"] = Math.Round(((Convert.ToDecimal(txtAmount.Text) - Convert.ToInt32(Convert.ToDecimal(txtAmount.Text))) * 100), 0).ToString();
                        }

                        catch { dr["Rate in USD"] = 0; dr["Total Charges in US $"] = 0; }
                        dtDyn.Rows.Add(dr);
                    }
                }
            }
            else if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 386 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 632)
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
                        if (int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400)
                        {
                        }
                        else
                        {
                            dtTitle.Columns.Add("TrackingSheetID");
                        }
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
                                            NoOfHours = GetFTEHours(int.Parse(ProjectID), Convert.ToString(dtTitle.Rows[i]["Employee Name"]), Convert.ToString(dtTitle.Rows[i]["Process"]), Convert.ToString(lblBillingPeriod.Text));
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
                                    //dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                }
                                else
                                {
                                    string NoOfHours = "";
                                    try
                                    {
                                        NoOfHours = GetFTEHours(int.Parse(ProjectID), Convert.ToString(dtTitle.Rows[i]["Employee Name"]), Convert.ToString(dtTitle.Rows[i]["Process"]), Convert.ToString(lblBillingPeriod.Text));
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

                                    dtTitle.Rows[i]["Price"] = Rate;
                                }
                                if (int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400)
                                {
                                }
                                else
                                {
                                    dtTitle.Rows[i]["TrackingSheetID"] = (i + 1);
                                }
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
                            object sumAmount = dtTitle.AsEnumerable()
                              .Where(r => r.Field<string>("Total Charges") != "")
                              .Sum(x => Convert.ToDecimal(x["Total Charges"])).ToString();

                            dr["Description"] = RowCount.ToString() + " FTE charges between " + From + " to " + To + "";
                            // sumAmount = "5258.21";
                            try
                            {
                                dr["Rate in USD"] = Convert.ToString(dtRate.Rows[0]["Rate"]);
                                dr["Total Charges in US $"] = Convert.ToString(Math.Round(Convert.ToDecimal(txtAmount.Text), 2));
                                dr["Before Decimal"] = Convert.ToInt32(Convert.ToDecimal(txtAmount.Text)).ToString();
                                dr["After Decimal"] = Math.Round(((Convert.ToDecimal(txtAmount.Text) - Convert.ToInt32(Convert.ToDecimal(txtAmount.Text))) * 100), 0).ToString();
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

                        //DataTable dt733 = new DataTable();
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
                int MonthCont = CountFteWorkingRows(dtAmt.Tables[0], null, "Operator1(Hours)");
                DataRow dr = dtDyn.NewRow();
                string TotalFTEHours = "";// Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont))));
                decimal Average;
                if (int.Parse(ProjectID) == 584)
                {
                    decimal Average1 = SumFteHours(dtAmt.Tables[0], "Operator1(Hours)");

                    Average = Convert.ToDecimal(Average1);// *Convert.ToDecimal(MonthCont);
                    TotalFTEHours = Convert.ToString(Average1);
                    dr["Description"] = "Operator charges between " + From + " to " + To + " (" + Convert.ToString(Average1) + "/FTE Hours)";
                    //dr["Description"] = Convert.ToString(Average1);
                    dr["Total # of Hours"] = Convert.ToString(Average1);
                }
                else if (int.Parse(ProjectID) == 442)
                {
                    dr["Description"] = "Operator charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                    dr["Total # of Hours"] = Convert.ToString(1 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    TotalFTEHours = Convert.ToString(1 * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
                else
                {
                    if (int.Parse(ProjectID) == 385)
                    {
                        if (BillingPeriod == "01-Jul-2022 ~ 31-Jul-2022")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "1350";
                            TotalFTEHours = "1350";
                        }
                        else if (BillingPeriod == "01-Nov-2022 ~ 30-Nov-2022")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "1773";
                            TotalFTEHours = "1773";
                        }
                        else if (BillingPeriod == "01-May-2023 ~ 31-May-2023")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "2583";
                            TotalFTEHours = "2583";
                        }
                        else if (BillingPeriod == "01-Jul-2023 ~ 31-Jul-2023")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "1710";
                            TotalFTEHours = "1710";
                        }
                        else if (BillingPeriod == "01-Feb-2024 ~ 29-Feb-2024")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "621";
                            TotalFTEHours = "621";
                        }
                        else if (BillingPeriod == "01-Jun-2024 ~ 30-Jun-2024")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "549";
                            TotalFTEHours = "549";
                        }
                        else if (BillingPeriod == "01-Jul-2024 ~ 31-Jul-2024")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "603";
                            TotalFTEHours = "603";
                        }
                        else if (BillingPeriod == "01-Aug-2024 ~ 31-Aug-2024")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "630";
                            TotalFTEHours = "630";
                        }
                        else if (BillingPeriod == "01-Oct-2024 ~ 31-Oct-2024")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "657";
                            TotalFTEHours = "657";
                        }
                        else if (BillingPeriod == "01-Nov-2024 ~ 30-Nov-2024")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "549";
                            TotalFTEHours = "549";
                        }
                        else if (BillingPeriod == "01-Dec-2024 ~ 31-Dec-2024")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "612";
                            TotalFTEHours = "612";
                        }
                        else if (BillingPeriod == "01-Feb-2025 ~ 28-Feb-2025")
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "576";
                            TotalFTEHours = "576";
                        }
                        else if (BillingPeriod == "01-Apr-2026 ~ 30-Apr-2026")
                        {
                            dr["Description"] =  "5 Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            //dr["FTE Hours"] = Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont));
                            dr["Total # of Hours"] = "738";
                            TotalFTEHours = "738";
                        }
                        else
                        {
                            dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                            dr["Total # of Hours"] = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                            TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
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
                        dr["Description"] = ApprovedFTECount + " Operator(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                        dr["Total # of Hours"] = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                        TotalFTEHours = Convert.ToString(Convert.ToDecimal(ApprovedFTECount) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    }
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
            }
            else
            {
                DataTable dt1 = dtAmt.Tables[0].DefaultView.ToTable(false, "BilledFTE");

                object Cont = dt1.AsEnumerable()
                              .Count(r => r.Field<string>("BilledFTE") != "Holiday");


                object MonthCont = dtAmt.Tables[0].AsEnumerable()
                              .Count(r => r.Field<string>("BilledFTE") != "Holiday");

                object MonthlyAverage = dt1.AsEnumerable()
                              .Where(r => r.Field<string>("BilledFTE") != "Holiday")
                              .Sum(x => Convert.ToDecimal(x["BilledFTE"]));

                DataRow dr = dtDyn.NewRow();
                string[] Period = BillingPeriod.Split('~');
                string From = Period[0].Trim();
                string To = Period[1].Trim();
                decimal Average = 0;
                if (int.Parse(ProjectID) == 184)
                {
                    var query = dt1.AsEnumerable()
                          .Where(r => r.Field<string>("BilledFTE") != "Holiday" && r.Field<string>("BilledFTE") != "" && r.Field<string>("BilledFTE") != null)
                          .GroupBy(dr23 => Convert.ToString(dr23["BilledFTE"]))
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
                        dr["Description"] = Math.Round(Average, 3).ToString() + " FTE charges towards data entry process for the period " + From + " to " + To;
                }
                else if (int.Parse(ProjectID) == 184)
                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont)) + "/FTE Hours) Legacy and Global";
                else
                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthCont)) + "/FTE Hours)";
                string TotalFTEHours = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                if (int.Parse(ProjectID) == 205)
                {
                    if (BillingPeriod == "01-Oct-2022 ~ 31-Oct-2022")
                        TotalFTEHours = Convert.ToString(Math.Round(Convert.ToDecimal(7.22), 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    else
                        TotalFTEHours = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
                if (int.Parse(ProjectID) == 205)
                {
                    if (BillingPeriod == "01-Oct-2022 ~ 31-Oct-2022")
                        dr["Total # of Hours"] = Convert.ToString(Math.Round(Convert.ToDecimal(7.22), 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                    else
                        dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                }
                else
                    dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 2) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthCont))));
                dtDyn.Rows.Add(dr);
                grdTemplate.Columns.Clear();
                grdTemplate.AutoGenerateColumns = true;
                grdTemplate.DataSource = dtDyn;// GetProjectwiseSummaryDetails_Typing(ProjectID, BillingPeriod);
                grdTemplate.DataBind();
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
                                    dr["Description"] = Math.Round(Average, 3).ToString() + " FTE charges towards data entry process for the period " + From + " to " + To + " - Weekend";
                                else
                                    dr["Description"] = Math.Round(Average, 2).ToString() + " Auditor(s) charges between " + From + " to " + To + " (" + Convert.ToString(Convert.ToDecimal(9) * Convert.ToDecimal(MonthContWeekend)) + "/FTE Hours) Legacy and Global - Wekend";
                                if (int.Parse(ProjectID) == 205)
                                {
                                    MonthContWeekend = dt1Weekend1.AsEnumerable()
                                              .Count(r => r.Field<string>("Date") != "Holiday");
                                    TotalFTEHours = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthContWeekend))));
                                    dr["Total # of Hours"] = Convert.ToString(Math.Round(Average, 3) * (Convert.ToDecimal(Convert.ToDecimal(BillableHours) * Convert.ToDecimal(MonthContWeekend))));
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
                drCol["DataColumn7"] = dtDyn.Columns[5].Caption;
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
                DataTable dtNew = GetInvoiceNumber(int.Parse(ProjectID), ProjectName, BillingPeriod);
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
                    rpt.Load(Server.MapPath("~/733003Preview.rpt"));
                else if (int.Parse(ProjectID) == 280 || int.Parse(ProjectID) == 393 || int.Parse(ProjectID) == 386)
                    rpt.Load(Server.MapPath("~/534Preview.rpt"));
                else if (int.Parse(ProjectID) == 465 || int.Parse(ProjectID) == 184 || int.Parse(ProjectID) == 203 || int.Parse(ProjectID) == 400 || int.Parse(ProjectID) == 434 || int.Parse(ProjectID) == 442 || int.Parse(ProjectID) == 435 || int.Parse(ProjectID) == 440 || int.Parse(ProjectID) == 205 || int.Parse(ProjectID) == 353 || int.Parse(ProjectID) == 123 || int.Parse(ProjectID) == 155 || int.Parse(ProjectID) == 354 || int.Parse(ProjectID) == 392 || int.Parse(ProjectID) == 373 || int.Parse(ProjectID) == 385 || int.Parse(ProjectID) == 531 || int.Parse(ProjectID) == 632)
                    rpt.Load(Server.MapPath("~/Reports/FTE/706_Preview.rpt"));
                dtNew1.Rows.RemoveAt(0);
                rpt.Database.Tables["FTEData"].SetDataSource(dtNew1);
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = ProjectID;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = BillingPeriod;
                pval2.Add(pdisval2);

                ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                pdisval3.Value = txtAmount.Text;
                pval3.Add(pdisval3);


                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval2);
                rpt.DataDefinition.ParameterFields["@Amount"].ApplyCurrentValues(pval3);

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

                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath = Server.MapPath("~/BillingDocuments/") + filename + ".pdf";
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);

                rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);
            }
            catch (Exception ex) { throw ex; }
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
        public void Time_To_Show_The_DetailedReport_183002()
        {
            try
            {
                DataSet dt = new DataSet();
                DataTable dtNew = GetInvoiceNumber(int.Parse(ProjectID), ProjectName, BillingPeriod);
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
                    rpt.Load(Server.MapPath("~/861007_Preview.rpt"));
                else if (ProjectName == "771")
                    rpt.Load(Server.MapPath("~/771_Preview.rpt"));
                else if (ProjectName == "711")
                    rpt.Load(Server.MapPath("~/Reports/Freight/711.rpt"));
                else if (ProjectName == "722")
                    rpt.Load(Server.MapPath("~/Reports/Freight/722_Preview.rpt"));
                else if (ProjectName == "791")
                    rpt.Load(Server.MapPath("~/791_Preview.rpt"));
                else if (ProjectName == "712")
                    rpt.Load(Server.MapPath("~/Reports/Freight/712_Preview.rpt"));
                else if (ProjectName == "733-002" || ProjectName == "772" || ProjectName == "791-002" || ProjectName == "736" || ProjectName == "409-002" || ProjectName == "409-005" || ProjectName == "572" || ProjectName == "733-004")
                    rpt.Load(Server.MapPath("~/Reports/Freight/733-002_Preview.rpt"));
                else if (ProjectName == "736-002")
                    rpt.Load(Server.MapPath("~/Reports/Freight/733-002_Preview.rpt"));
                else if (ProjectName == "754")
                    rpt.Load(Server.MapPath("~/Reports/Freight/754_Preview.rpt"));
                else if (ProjectName == "757-003")
                    rpt.Load(Server.MapPath("~/757-003_Preview.rpt"));
                else if (ProjectName == "694-008" || ProjectName == "694-005")
                    rpt.Load(Server.MapPath("~/694-008_Preview.rpt"));
                else if (int.Parse(ProjectID) == 620)
                    rpt.Load(Server.MapPath("~/Reports/WholeLoan/WholeLoan_Preview.rpt"));
                else
                    rpt.Load(Server.MapPath("~/Reports/Commitment/183-002_Preview.rpt"));
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = ProjectID;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = BillingPeriod;
                pval2.Add(pdisval2);

                ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                pdisval3.Value = txtAmount.Text;
                pval3.Add(pdisval3);


                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval2);
                rpt.DataDefinition.ParameterFields["@Amount"].ApplyCurrentValues(pval3);

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
                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath = Server.MapPath("~/BillingDocuments/") + filename + ".pdf";
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);

                rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);
            }
            catch (Exception ex) { throw ex; }

        }

        public void BindTypingReport()
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
                        DataTable dtNew = GetInvoiceNumber(int.Parse(ProjectID), ProjectName, BillingPeriod);
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
                            rpt.Load(Server.MapPath("~/888Preview.rpt"));
                        else if (int.Parse(ProjectID) == 13)
                            return;
                        else if (int.Parse(ProjectID) == 248)
                            rpt.Load(Server.MapPath("~/694Preview.rpt"));
                        else if (BillingBase == "Base Rate")
                            rpt.Load(Server.MapPath("~/Reports/Commitment/BaseRateSummary_Preview.rpt"));
                        else if (BillingBase == "Product Type")
                            rpt.Load(Server.MapPath("~/Reports/Commitment/ProductTypeSummary_Preview.rpt"));
                        else if (BillingBase == "Order Type")
                            rpt.Load(Server.MapPath("~/Reports/Commitment/OrderTypeSummary_Preview.rpt"));

                        CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                        ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                        //pdisval1.Value = ProjectName;
                        pdisval1.Value = ProjectID;
                        pval1.Add(pdisval1);

                        rpt.DataDefinition.ParameterFields["@ProjectID"].ApplyCurrentValues(pval1);

                        CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                        CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                        ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                        pdisval2.Value = BillingPeriod;
                        pval2.Add(pdisval2);

                        ParameterDiscreteValue pdisval3 = new ParameterDiscreteValue();
                        pdisval3.Value = txtAmount.Text;
                        pval3.Add(pdisval3);


                        rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval2);
                        rpt.DataDefinition.ParameterFields["@Amount"].ApplyCurrentValues(pval3);

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
                        if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                        {
                            Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                        }
                        string filePath = Server.MapPath("~/BillingDocuments/") + filename + ".pdf";
                        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);

                        rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public void Time_To_Show_The_SummaryReport()
        {
            try
            {
                DataSet dt = new DataSet();

                DataTable dtNew = GetInvoiceNumber(int.Parse(ProjectID), ProjectName, BillingPeriod);// GetInvoiceNumber(ProjectGroup, BillingPeriod);
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
                rpt.Load(Server.MapPath("~/Reports/Valuation/SummaryReport_Preview.rpt"));
                CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();
                pdisval1.Value = ProjectID;
                pval1.Add(pdisval1);

                rpt.DataDefinition.ParameterFields["@GroupName"].ApplyCurrentValues(pval1);

                CrystalDecisions.Shared.ParameterValues pval2 = new ParameterValues();
                CrystalDecisions.Shared.ParameterValues pval3 = new ParameterValues();

                ParameterDiscreteValue pdisval2 = new ParameterDiscreteValue();
                pdisval2.Value = BillingPeriod;
                pval2.Add(pdisval2);



                rpt.DataDefinition.ParameterFields["@BillingPeriod"].ApplyCurrentValues(pval2);
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
                string filename = "";
                if (InvoiceNumber != "")
                {
                    filename = InvoiceNumber.Replace(",", "_");
                }
                else
                {
                    filename = ProjectGroup + "_" + BillingPeriod + "_" + strDate + strTime;
                }

                if (!Directory.Exists(Server.MapPath(@"~/BillingDocuments/")))
                {
                    Directory.CreateDirectory(Server.MapPath(@"~/BillingDocuments/"));
                }
                string filePath =
                    Server.MapPath("~/BillingDocuments/") + filename + ".pdf";

                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, filePath);
                int result = blltracking.InsertGroupAttachmentPath_QC(ProjectGroup, BillingPeriod, Convert.ToString(@"~/BillingDocuments/" + filename + ".pdf"), InvoiceNumber);

                rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, filename);

            }
            catch (Exception ex) { throw ex; }

        }

        public DataTable GetProjectClientConfiguration(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "IB_usp_GetProjectWiseClientInvoiceConfiguration_2");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetBillingBase(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_BillingBase");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetInvoiceNumber(int ProjectID, string ProjectName, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GenerateInvoiceNumber]");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectName", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, ProjectName);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        protected void grdBilling_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Number")
                {
                    e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
        public int UpdateMergeBillingPeriod(int ProjectId, string BillingPeriod)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[UpdateMergeBIllingPeriod]");//IB_usp_UpdateProjectApprovedforClient
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }
        protected void btnContinueMerge_Click(object sender, EventArgs e)
        {
            List<string> periods = new List<string>();
            DataTable dtDates = new DataTable();
            dtDates.Columns.Add("BillingDate");
            dtDates.Columns["BillingDate"].DataType = System.Type.GetType("System.DateTime");
            DataRow dr;
            foreach (GridViewRow row in grdPendingPeriod.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                    if (chkRow != null && chkRow.Checked)
                    {
                        Label billingPeriodLabel = row.Cells[1].FindControl("lblBillingPeriod") as Label;
                        if (billingPeriodLabel == null)
                            continue;
                        string PrevBillingPeriod = billingPeriodLabel.Text;
                        string[] Billing = PrevBillingPeriod.Split('~');
                        for (int i = 0; i < Billing.Length; i++)
                        {
                            dr = dtDates.NewRow();
                            dr["BillingDate"] = Billing[i];
                            dtDates.Rows.Add(dr);
                        }
                    }
                }
            }
            if (dtDates.Rows.Count > 0)
            {
                DataView dv = dtDates.DefaultView;
                dv.Sort = "BillingDate asc";
                DataTable dtNew = dv.ToTable();
                string StartDate = Convert.ToDateTime(dtNew.Rows[0]["BillingDate"]).ToString("dd-MMM-yyyy");
                string[] ForEndDate = BillingPeriod.Split('~');
                string EndDate = ForEndDate[1].Trim();
                string NewBillingperiod = StartDate + " ~ " + EndDate;

                //Normal Procedure with merge changes

                SaveVerified_Merge(NewBillingperiod);
                if (SelectedOrders <= 0)
                {
                    if (grdBilling.VisibleRowCount <= 0)
                    {
                    }
                    else
                    {
                        dvError.Style.Add("display", "");
                        dvError.Attributes.Add("class", "alert alert-warning background-warning");
                        dvError.InnerHtml = "Please select at least one order.";
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                        return;
                    }
                }
                if (grdBilling.VisibleRowCount > 0)
                    GetOrderApproved();

                object SumAmount;
                try
                {
                    if (grdBilling.VisibleRowCount > 0)
                    {
                        if (dtSelectedRows != null)
                        {
                            if (ProjectID == "248")
                            {
                                SumAmount = dtSelectedRows.AsEnumerable()
                              .Where(r => r.Field<string>("Total Charges") != "")
                              .Sum(x => Convert.ToDecimal(x["Total Charges"]));
                            }
                            else
                            {
                                //SumAmount = dtSelectedRows.Compute("Sum(Price)", string.Empty);
                                SumAmount = dtSelectedRows.AsEnumerable()
                                   .Where(r => r.Field<string>("Price") != "")
                                   .Sum(x => Convert.ToDecimal(x["Price"]));
                            }
                        }
                        else
                        {
                            if (ProjectID == "248")
                            {
                                dtSelectedRows = (DataTable)Session["dtSelectedRows"];
                                //SumAmount = dtSelectedRows.Compute("Sum(Price)", string.Empty);
                                SumAmount = dtSelectedRows.AsEnumerable()
                                  .Where(r => r.Field<string>("Total Charges") != "")
                                  .Sum(x => Convert.ToDecimal(x["Total Charges"]));
                            }
                            else
                            {
                                dtSelectedRows = (DataTable)Session["dtSelectedRows"];
                                //SumAmount = dtSelectedRows.Compute("Sum(Price)", string.Empty);
                                SumAmount = dtSelectedRows.AsEnumerable()
                                  .Where(r => r.Field<string>("Price") != "")
                                  .Sum(x => Convert.ToDecimal(x["Price"]));
                            }
                        }
                    }
                    else
                        SumAmount = 0;
                }
                catch { SumAmount = txtAmount.Text; }
                if (ProjectID == "350")
                    SumAmount = txtAmount.Text;
                try
                {
                    DataTable dtDomain = blltracking.GetDomianFromProject(Convert.ToInt32(ProjectID));

                    if (Convert.ToInt32(ProjectID) == 203 || Convert.ToInt32(ProjectID) == 400 || Convert.ToInt32(ProjectID) == 373 || Convert.ToInt32(ProjectID) == 385 || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 31 || (Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 34 && int.Parse(ProjectID) != 391 && int.Parse(ProjectID) != 337) || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 35 || Convert.ToInt32(dtDomain.Rows[0]["DomainID"]) == 18)
                        SumAmount = txtAmount.Text;
                }
                catch { }

                Hashtable Htparam = new Hashtable();

                Htparam.Add("ProjectID", Convert.ToString(ProjectID));
                Htparam.Add("BillingPeriod", Convert.ToString(NewBillingperiod));
                Htparam.Add("ProjectName", Convert.ToString(Request.QueryString["ProjectName"]));
                Htparam.Add("Amount", Convert.ToString(SumAmount));
                Htparam.Add("Added_By", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
                Htparam.Add("IsManual", chkInvoiceNo.Checked);
                Htparam.Add("InvoiceNoManual", lblInvoiceNo.Text);

                //Htparam.Add("OrderCount", Convert.ToString(txtOrder.Text));
                Htparam.Add("OrderCount", Convert.ToString(dtSelectedRows.Rows.Count));
                int result = 0;
                if (grdBilling.VisibleRowCount > 0)
                {
                    result = blltracking.UpdateSendToclient(Htparam);
                    foreach (GridViewRow row in grdPendingPeriod.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                            if (chkRow != null && chkRow.Checked)
                            {
                                Label billingPeriodLabel = row.Cells[1].FindControl("lblBillingPeriod") as Label;
                                if (billingPeriodLabel == null)
                                    continue;
                                string PrevBillingPeriod = billingPeriodLabel.Text;
                                int mergeOut = UpdateMergeBillingPeriod(int.Parse(ProjectID), PrevBillingPeriod);
                            }
                        }
                    }
                    int mergeOutCurrent = UpdateMergeBillingPeriod(int.Parse(ProjectID), BillingPeriod);

                }
                else
                {
                    result = UpdateSendToclient_ZeroBilling(Htparam);
                }
                if (result > 0)
                {
                    DataSet dt = blltracking.GetTotalProjectAmountForReport(int.Parse(ProjectID), NewBillingperiod);
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-success background-success");
                    dvError.InnerHtml = "Project has been successfully sent to client!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                    Response.Redirect("SendAccountDetails.aspx");
                }
                else if (result == 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Error occured while sending project data to clien!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }
        }

        protected void OrderChangeFTE_Callback(object sender, CallbackEventArgsBase e)
        {
            LoadFTEEditor(e.Parameter);
        }

        protected void btnLoadFTE_Click(object sender, EventArgs e)
        {
            LoadFTEEditor(hdnEditFTEKey.Value);
            ClientScript.RegisterStartupScript(GetType(), "showFTEEditor", "$('#popupExcelReportFTE').modal('show');", true);
        }

        private void LoadFTEEditor(string key)
        {
            int index = grdBilling.FindVisibleIndexByKeyValue(key);
            string OldFTEHours = Convert.ToString(grdBilling.GetRowValues(index, "No Of Hours"));
            string Employee = Convert.ToString(grdBilling.GetRowValues(index, "Employee Name"));
            string Process = Convert.ToString(grdBilling.GetRowValues(index, "Process"));
            Session["FTEHours"] = OldFTEHours;
            Session["Process"] = Process;
            Session["Employee"] = Employee;
            lblProjectNoFTE.Text = lblProject.Text;
            lblEmployee.Text = Employee;
            lblProcess.Text = Process;
            lblExistingFTEHours.Text = OldFTEHours;

        }

        protected void btnUpdateFTE_Click(object sender, EventArgs e)
        {
            Hashtable htParam = new Hashtable();
            htParam.Add("ProjectID", int.Parse(ProjectID));
            htParam.Add("Employee", Convert.ToString(Session["Employee"]));
            htParam.Add("Process", Convert.ToString(Session["Process"]));
            htParam.Add("NoOfHours", Convert.ToString(txtFTEHours.Text));
            htParam.Add("BillingPeriod", lblBillingPeriod.Text);
            htParam.Add("Remark", txtRemarkFTE.Text.Trim());
            htParam.Add("AddedBy", int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            int ReturnValue = ChangeFTEHours(htParam);
            if (ReturnValue > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-success background-success");
                dvError.InnerHtml = "FTE hours changed successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-danger background-danger");
                dvError.InnerHtml = "Error occured while changing FTE hours.";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            BindGridTest();
            Response.Redirect("SendToAccounts.aspx?ProjectID=" + Convert.ToString(ProjectID) + "&BillingPeriod=" + BillingPeriod + "&ProjectName=" + ProjectName);
        }



    }
}

public class CellMergerInternalDifferenece
{
    public CellMergerInternalDifferenece()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    ASPxGridView grid;
    Dictionary<GridViewDataColumn, TableCell> mergedCells = new Dictionary<GridViewDataColumn, TableCell>();
    Dictionary<TableCell, int> cellRowSpans = new Dictionary<TableCell, int>();

    public CellMergerInternalDifferenece(ASPxGridView grid)
    {
        this.grid = grid;
        Grid.HtmlRowCreated += new ASPxGridViewTableRowEventHandler(grid_HtmlRowCreated);
        Grid.HtmlDataCellPrepared += new ASPxGridViewTableDataCellEventHandler(grid_HtmlDataCellPrepared);
    }

    public ASPxGridView Grid { get { return grid; } }
    void grid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        //add the attribute that will be used to find which column the cell belongs to
        e.Cell.Attributes.Add("ci", e.DataColumn.VisibleIndex.ToString());

        if (cellRowSpans.ContainsKey(e.Cell))
        {
            e.Cell.RowSpan = cellRowSpans[e.Cell];
        }
    }
    void grid_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
    {
        if (Grid.GetRowLevel(e.VisibleIndex) != Grid.GroupCount) return;
        for (int i = e.Row.Cells.Count - 1; i >= 0; i--)
        {
            DevExpress.Web.Rendering.GridViewTableDataCell dataCell = e.Row.Cells[i] as DevExpress.Web.Rendering.GridViewTableDataCell;
            if (dataCell != null)
            {
                MergeCells(dataCell.DataColumn, e.VisibleIndex, dataCell);
            }
        }
    }

    void MergeCells(GridViewDataColumn column, int visibleIndex, TableCell cell)
    {
        bool isNextTheSame = IsNextRowHasSameData(column, visibleIndex);
        if (isNextTheSame)
        {
            if (!mergedCells.ContainsKey(column))
            {

                mergedCells[column] = cell;
            }
        }
        if (IsPrevRowHasSameData(column, visibleIndex))
        {
            ((System.Web.UI.WebControls.TableRow)cell.Parent).Cells.Remove(cell);
            if (mergedCells.ContainsKey(column))
            {
                TableCell mergedCell = mergedCells[column];
                if (!cellRowSpans.ContainsKey(mergedCell))
                {
                    cellRowSpans[mergedCell] = 1;
                }
                cellRowSpans[mergedCell] = cellRowSpans[mergedCell] + 1;
            }
        }
        if (!isNextTheSame)
        {
            mergedCells.Remove(column);
        }
    }
    bool IsNextRowHasSameData(GridViewDataColumn column, int visibleIndex)
    {
        //is it the last visible row
        if (visibleIndex >= Grid.VisibleRowCount - 1)
            return false;

        return IsSameData(column.FieldName, visibleIndex, visibleIndex + 1);
    }
    bool IsPrevRowHasSameData(GridViewDataColumn column, int visibleIndex)
    {
        ASPxGridView grid = column.Grid;
        //is it the first visible row
        if (visibleIndex <= Grid.VisibleStartIndex)
            return false;

        //if (Convert.ToString(Grid.GetRowValues(visibleIndex, "UserCode")) != Convert.ToString(Grid.GetRowValues(visibleIndex - 1, "UserCode")))
        //    return false;
        return IsSameData(column.FieldName, visibleIndex, visibleIndex - 1);
    }
    bool IsSameData(string fieldName, int visibleIndex1, int visibleIndex2)
    {
        // is it a group row?
        if (Grid.GetRowLevel(visibleIndex2) != Grid.GroupCount)
            return false;
        if (fieldName == "WeeklyAverage" || fieldName == "BilledFTE")
        {
            if (string.Equals(Convert.ToString(Grid.GetRowValues(visibleIndex1, "WeeklyAverage")), "Holiday", StringComparison.OrdinalIgnoreCase))
                return false;
            if (Convert.ToString(Grid.GetRowValues(visibleIndex1, "WeeklyAverage")) == Convert.ToString(Grid.GetRowValues(visibleIndex2, "WeeklyAverage")))
                return object.Equals(Grid.GetRowValues(visibleIndex1, fieldName), Grid.GetRowValues(visibleIndex2, fieldName));
            else
            {
                return false;
            }
        }
        else
            return false;
    }
}

namespace BillingOther.Accounts
{
    public sealed class PortalDataGrid : Control
    {
        private DataTable data = new DataTable();
        private readonly PortalGridColumnCollection columns = new PortalGridColumnCollection();
        private PortalGridSelection selection;

        public PortalDataGrid() { selection = new PortalGridSelection(this); }
        public object DataSource { get; set; }
        public bool AutoGenerateColumns { get; set; }
        public bool EnableSelection { get; set; }
        public string KeyFieldName { get; set; }
        public string CssClass { get; set; }
        public new bool Visible { get; set; } = true;
        public PortalGridColumnCollection Columns { get { return columns; } }
        public PortalGridSelection Selection { get { return selection; } }
        public int VisibleRowCount { get { return data == null ? 0 : data.Rows.Count; } }

        public override void DataBind()
        {
            DataTable source = DataSource as DataTable;
            DataSet set = DataSource as DataSet;
            if (source == null && set != null && set.Tables.Count > 0) source = set.Tables[0];
            data = source == null ? new DataTable() : source.Copy();
            SendToAccounts page = Page as SendToAccounts;
            if (ID == "grdBilling" && page != null)
            {
                int projectId;
                if ((page.ProjectName ?? "").IndexOf("791-001", StringComparison.OrdinalIgnoreCase) >= 0 && Int32.TryParse(page.ProjectID, out projectId)) AppendExtraFte(data, projectId, page.BillingPeriod ?? "");
            }
            if (AutoGenerateColumns)
            {
                foreach (DataColumn field in data.Columns)
                    if (Columns[field.ColumnName] == null) Columns.Add(new GridViewDataTextColumn { FieldName = field.ColumnName, Caption = field.ColumnName });
            }
            base.DataBind();
        }

        public int FindVisibleIndexByKeyValue(object key)
        {
            if (data == null || String.IsNullOrWhiteSpace(KeyFieldName) || !data.Columns.Contains(KeyFieldName)) return -1;
            for (int i = 0; i < data.Rows.Count; i++) if (Convert.ToString(data.Rows[i][KeyFieldName]) == Convert.ToString(key)) return i;
            return -1;
        }

        public object GetRowValues(int index, string fieldName)
        {
            return index >= 0 && index < VisibleRowCount && data.Columns.Contains(fieldName) ? data.Rows[index][fieldName] : null;
        }

        public List<object> GetSelectedFieldValues(params string[] fieldNames)
        {
            List<object> result = new List<object>();
            HashSet<string> selected = GetSelectedKeys();
            foreach (DataRow row in data.Rows)
            {
                string key = HasKeyField() ? Convert.ToString(row[KeyFieldName]) : data.Rows.IndexOf(row).ToString();
                if (!selected.Contains(key)) continue;
                result.Add(fieldNames.Select(name => data.Columns.Contains(name) ? row[name] : null).ToArray());
            }
            return result;
        }

        internal HashSet<string> GetSelectedKeys()
        {
            string marker = Page.Request.Form[ClientID + "_selectionPosted"];
            string[] posted = Page.Request.Form.GetValues(ClientID + "_select") ?? new string[0];
            if (marker == "1") return new HashSet<string>(posted);
            HashSet<string> all = new HashSet<string>();
            if (data != null) foreach (DataRow row in data.Rows) all.Add(HasKeyField() ? Convert.ToString(row[KeyFieldName]) : data.Rows.IndexOf(row).ToString());
            return all;
        }

        public DataTable GetDataTable() { return data == null ? new DataTable() : data.Copy(); }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Visible) return;
            List<GridViewColumn> visible = Columns.Where(c => c.Visible).OrderBy(c => c.VisibleIndex < 0 ? Int32.MaxValue : c.VisibleIndex).ToList();
            writer.AddAttribute(HtmlTextWriterAttribute.Class, String.IsNullOrWhiteSpace(CssClass) ? "table portal-datatable" : CssClass);
            writer.AddAttribute("data-datatable", "true");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Thead); writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            if (EnableSelection) writer.Write("<th><input type=\"checkbox\" checked=\"checked\" onclick=\"var c=this.checked;this.closest('table').querySelectorAll('.portal-row-select').forEach(function(x){x.checked=c;});\" aria-label=\"Select all\" /></th>");
            foreach (GridViewColumn column in visible) { writer.RenderBeginTag(HtmlTextWriterTag.Th); writer.WriteEncodedText(String.IsNullOrWhiteSpace(column.Caption) ? GetFieldName(column) : column.Caption); writer.RenderEndTag(); }
            writer.RenderEndTag(); writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
            HashSet<string> selected = GetSelectedKeys();
            for (int rowIndex = 0; rowIndex < data.Rows.Count; rowIndex++)
            {
                DataRow row = data.Rows[rowIndex]; writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                string key = HasKeyField() ? Convert.ToString(row[KeyFieldName]) : rowIndex.ToString();
                if (EnableSelection) writer.Write("<td><input class=\"portal-row-select\" type=\"checkbox\" name=\"" + HttpUtility.HtmlAttributeEncode(ClientID + "_select") + "\" value=\"" + HttpUtility.HtmlAttributeEncode(key) + "\" " + (selected.Contains(key) ? "checked=\"checked\"" : "") + " /></td>");
                foreach (GridViewColumn column in visible)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    string field = GetFieldName(column);
                    if (String.Equals(column.Caption, "Edit", StringComparison.OrdinalIgnoreCase))
                    {
                        bool fte = data.Columns.Contains("No Of Hours") || data.Columns.Contains("BilledFTE");
                        string function = fte ? "EditOrderFTE" : "EditOrder";
                        writer.Write("<a class=\"portal-action\" id=\"Link_" + HttpUtility.HtmlAttributeEncode(key) + "\" href=\"javascript:void(0)\" onclick=\"" + function + "(this,'" + HttpUtility.JavaScriptStringEncode(key) + "');return false;\"><i class=\"fa fa-pencil\"></i> Edit</a>");
                    }
                    else writer.WriteEncodedText(field == "Number" ? Convert.ToString(rowIndex + 1) : data.Columns.Contains(field) ? Convert.ToString(row[field]) : String.Empty);
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag(); writer.RenderEndTag();
            if (EnableSelection) writer.Write("<input type=\"hidden\" name=\"" + HttpUtility.HtmlAttributeEncode(ClientID + "_selectionPosted") + "\" value=\"1\" />");
        }

        private static string GetFieldName(GridViewColumn column)
        {
            GridViewDataColumn dataColumn = column as GridViewDataColumn;
            return dataColumn == null ? String.Empty : dataColumn.FieldName;
        }

        private bool HasKeyField()
        {
            return data != null && !String.IsNullOrWhiteSpace(KeyFieldName) && data.Columns.Contains(KeyFieldName);
        }

        private static void AppendExtraFte(DataTable table, int projectId, string billingPeriod)
        {
            if (table == null || !table.Columns.Contains("Date")) return;
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFTEBillingExtraOperators");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 10, ParameterDirection.Input, projectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 500, ParameterDirection.Input, billingPeriod.Replace(" to ", " ~ "));
            DataTable extra = SQLHelper.ExecuteDataTableCmd(cmd);
            if (extra == null || extra.Rows.Count == 0) return;
            int maximum = extra.AsEnumerable().Max(r => Convert.ToInt32(r["ExtraOperatorNo"]));
            for (int number = 1; number <= maximum; number++) if (!table.Columns.Contains("Extra Operator " + number + " (Hours)")) table.Columns.Add("Extra Operator " + number + " (Hours)");
            foreach (DataRow row in table.Rows)
            {
                DateTime date;
                if (!DateTime.TryParse(Convert.ToString(row["Date"]), out date)) continue;
                foreach (DataRow item in extra.AsEnumerable().Where(r => Convert.ToDateTime(r["WorkDate"]).Date == date.Date))
                    row["Extra Operator " + item["ExtraOperatorNo"] + " (Hours)"] = Convert.ToDecimal(item["Hours"]).ToString("0.##");
            }
        }

        private static DataTable BuildAverageFteDisplay(DataTable source, int projectId, string billingPeriod)
        {
            if (source == null || !source.Columns.Contains("Date") || !source.Columns.Contains("AssignTarget")) return source;
            DataTable result = new DataTable();
            foreach (string name in new[] { "S.No", "Date", "Target Assigned", "Total invoices", "Average FTE", "Billed FTE" }) result.Columns.Add(name);
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetFTEAverageBillingData");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 10, ParameterDirection.Input, projectId);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", SqlDbType.NVarChar, 500, ParameterDirection.Input, billingPeriod.Replace(" to ", " ~ "));
            DataTable averages = SQLHelper.ExecuteDataTableCmd(cmd);
            int serial = 1;
            foreach (DataRow item in source.Rows)
            {
                DataRow row = result.NewRow(); DateTime date;
                string dateText = Convert.ToString(item["Date"]), process = source.Columns.Contains("Process") ? Convert.ToString(item["Process"]) : "";
                string target = Convert.ToString(item["AssignTarget"]), invoices = source.Columns.Contains("TotalInvoices") ? Convert.ToString(item["TotalInvoices"]) : "";
                DataRow average = DateTime.TryParse(dateText, out date) && averages != null ? averages.AsEnumerable().FirstOrDefault(a => Convert.ToDateTime(a["Date"]).Date == date.Date && String.Equals(Convert.ToString(a["ProcessName"]), process, StringComparison.OrdinalIgnoreCase)) : null;
                bool holiday = String.Equals(target, "Holiday", StringComparison.OrdinalIgnoreCase) || String.Equals(invoices, "Holiday", StringComparison.OrdinalIgnoreCase);
                row[0] = serial++; row[1] = dateText; row[2] = target; row[3] = invoices;
                row[4] = holiday ? "Holiday" : average == null ? "" : Convert.ToDecimal(average["AverageFTE"]).ToString("0.00");
                row[5] = holiday ? "Holiday" : average == null ? "" : Convert.ToDecimal(average["BilledFTE"]).ToString("0"); result.Rows.Add(row);
            }
            return result;
        }
    }

    public sealed class PortalGridSelection
    {
        private readonly PortalDataGrid grid;
        internal PortalGridSelection(PortalDataGrid grid) { this.grid = grid; }
        public void SelectAll() { }
    }

    public sealed class PortalGridColumnCollection : List<GridViewColumn>
    {
        public GridViewColumn this[string name] { get { return this.FirstOrDefault(c => String.Equals(c.Caption, name, StringComparison.OrdinalIgnoreCase) || String.Equals((c as GridViewDataColumn)?.FieldName, name, StringComparison.OrdinalIgnoreCase)); } }
    }

    public sealed class CrystalViewerShim : Control
    {
        public bool HasExportButton { get; set; } public bool HasPrintButton { get; set; }
        public bool HasPageNavigationButtons { get; set; } public bool HasCrystalLogo { get; set; }
        public bool HasDrillUpButton { get; set; } public bool HasSearchButton { get; set; }
        public bool HasToggleGroupTreeButton { get; set; } public bool HasZoomFactorList { get; set; }
        public object ReportSource { get; set; }
        public Style ToolbarStyle { get; private set; } = new Style();
        public void RefreshReport() { }
        protected override void Render(HtmlTextWriter writer) { }
    }
}
