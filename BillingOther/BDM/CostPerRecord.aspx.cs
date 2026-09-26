using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;
using DevExpress.Web;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace BillingOther.BDM
{
    public partial class CostPerRecord : System.Web.UI.Page
    {
        bllTracking bllMaster = new bllTracking();
        string Month = "";
        string Yearvalue = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Cost Per Record Report";

            if (!IsPostBack)
            {
                Year();
                OtherCostMonth();
                VendorCostMonth();
                OtherCostYear();
                VendorCostYear();
                //BindVendorCostgrid();
                //OtherCostBindgrid();
                BindProject();
                BindDomain();
            }

            Month = ddlMonth.SelectedValue;
            Yearvalue = ddlYear.SelectedValue;

            if (ddlMonth.SelectedIndex != 0 && ddlYear.SelectedIndex != 0)
            {
                //BindGrid(ddlMonth.SelectedValue, ddlYear.SelectedValue);
                //BindDomainGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
                //BindSupportEmployee(ddlMonth.SelectedValue, ddlYear.SelectedValue);
                //BindProjectGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
              //  BindGrid(ddlMonth.SelectedValue, ddlYear.SelectedValue);
            }
            BindVendorCostgrid();
            OtherCostBindgrid();

        }

        public DataSet GetCostingReport(string Month, string year)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[GetCostPerRecordReport]");
            SQLHelper.AddParamToSQLCmd(cmd, "@Month", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Month);
            SQLHelper.AddParamToSQLCmd(cmd, "@Year", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, year);
            DataSet dt = SQLHelper.ExecuteDataSetCmd(cmd);
            return dt;
        }

        public void BindGrid(string Month, string Year)
        {
            DataSet ds = GetCostingReport(Month, Year);
            if (ds != null)
            {
                grdUser.DataSource = ds.Tables[0];
                grdUser.DataBind();

                //DataTable dtUser = ds.Tables[0];
                //DataView dv = dtUser.DefaultView;
                //dv.RowFilter = "Domain='Support'";
                //if (dv.ToTable() != null)
                //{
                //    if (dv.ToTable().Rows.Count > 0)
                //    {
                //        lblSupport.Text = Convert.ToString(dv.ToTable().Rows.Count);
                //    }
                //    else
                //    {
                //        lblSupport.Text = Convert.ToString(0);
                //    }
                //}

                grdReport.DataSource = ds.Tables[1];
                grdReport.DataBind();

                grdProjectReport.DataSource = ds.Tables[2];
                grdProjectReport.DataBind();
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex != 0 && ddlYear.SelectedIndex != 0)
            {
                BindGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
                //BindDomainGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
                //BindSupportEmployee(ddlMonth.SelectedValue, ddlYear.SelectedValue);
                //BindProjectGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
            }
        }

        public void BindDomain()
        {
            DataTable dt = GetAllDomain();
            int count = dt.Rows.Count + 1;
            if (dt.Rows.Count > 0)
            {
                ddlDomainForOther.DataSource = dt;
                ddlDomainForOther.DataValueField = "SubdomainID";
                ddlDomainForOther.DataTextField = "SubdomainName";
                ddlDomainForOther.DataBind();
            }
            ddlDomainForOther.Items.Insert(0, new ListItem("Select"));
            ddlDomainForOther.Items.Insert(1, new ListItem("All"));
        }

        public DataTable GetAllDomain()
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetAllSubDomain]");
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        protected void btnExpoetToExcel_Click(object sender, EventArgs e)
        {
            GridViewDataTextColumn exportColumn = FindColumnByName(grdUser, "DisplayCode");
            GridViewDataColumn templateColumn = FindColumnByNameData(grdUser, "Code");
            templateColumn.Visible = false;
            exportColumn.Visible = true;
            exportColumn.VisibleIndex = 1;
            var exportOptions = new XlsExportOptionsEx();
            exportOptions.ExportType = DevExpress.Export.ExportType.DataAware;
            exportOptions.ExportHyperlinks = true;

            PrintingSystemBase ps = new PrintingSystemBase();
            ps.ExportOptions.Xlsx.SheetName = "";


            PrintableComponentLinkBase link1 = new PrintableComponentLinkBase(ps);
            link1.Component = grdUserExport;


            PrintableComponentLinkBase link2 = new PrintableComponentLinkBase(ps);
            link2.Component = grdDomainExport;

            PrintableComponentLinkBase link3 = new PrintableComponentLinkBase(ps);
            link3.Component = grdProjectExport;

            CompositeLinkBase compositeLink = new CompositeLinkBase(ps);
            compositeLink.Links.AddRange(new object[] { link1, link2, link3 });

            compositeLink.CreatePageForEachLink();
            ps.XlsxDocumentCreated += PrintingSystem_XlsxDocumentCreated;
            using (MemoryStream stream = new MemoryStream())
            {
                XlsxExportOptions options = new XlsxExportOptions();
                options.ExportMode = XlsxExportMode.SingleFilePageByPage;
                compositeLink.PrintingSystemBase.ExportToXlsx(stream, options);
                Response.Clear();
                Response.Buffer = false;
                Response.AppendHeader("Content-Type", "application/xlsx");
                Response.AppendHeader("Content-Transfer-Encoding", "binary");
                Response.AppendHeader("Content-Disposition", "attachment; filename=Cost Per Record_" + ddlMonth.SelectedItem.Text + "" + ddlYear.SelectedItem.Text + " .xlsx");
                Response.BinaryWrite(stream.ToArray());
                exportColumn.Visible = false;
                templateColumn.Visible = true;
                Response.End();
            }
            ps.Dispose();

        }

        private void Year()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(null);
            int lastyear = DateTime.Now.Year - 5;
            int year = DateTime.Now.Year;

            for (int Y = year; Y >= lastyear; Y--)
            {
                ddlYear.Items.Add(new ListItem(Y.ToString(), Y.ToString()));
            }
            ddlYear.Items.Insert(0, new ListItem("Select"));
        }

        void PrintingSystem_XlsxDocumentCreated(object sender, DevExpress.XtraPrinting.XlsxDocumentCreatedEventArgs e)
        {
            e.SheetNames[0] = "Userwise";
            e.SheetNames[1] = "Domainwise";
            e.SheetNames[2] = "Projectwise";
        }

        //public void BindSupportEmployee(string Month, string Year)
        //{
        //    DataTable dt = bllMaster.GetSupportDomainEmployee(Month, Year);
        //    if (dt.Rows.Count > 0)
        //    {
        //        lblSupport.Text = Convert.ToString(dt.Rows.Count);
        //    }
        //}

        #region User Wise

        //public void BindGrid(string Month, string Year)
        //{
        //    DataTable dt = bllMaster.GetDataForAdminWiseCostPerRecord(Month, Year);
        //    if (dt.Rows.Count > 0)
        //    {
        //        grdUser.DataSource = dt;
        //        grdUser.DataBind();
        //    }
        //}

        protected void lnkSalaryBreakUp_Init(object sender, EventArgs e)
        {
            ASPxHyperLink link = (ASPxHyperLink)sender;

            Month = Convert.ToString(ViewState["Month"]);
            Yearvalue = Convert.ToString(ViewState["Yearvalue"]);

            if (link != null)
            {
                GridViewDataItemTemplateContainer tempCont = (GridViewDataItemTemplateContainer)link.NamingContainer;

                if (tempCont != null)
                {
                    string key = ((string)tempCont.KeyValue);

                    //   int rowVisibleIndex = tempCont.VisibleIndex;
                    string contentUrl = "ProductivityWiseSalaryBreakUp.aspx?Code=" + key + "&Month=" + Month + "&Yearvalue=" + Yearvalue;
                    link.NavigateUrl = "javascript:void(0);";
                    link.ClientSideEvents.Click = string.Format("function(s, e) {{ ShowSalaryBreakUp('{0}'); }}", contentUrl);
                }
            }
        }

        protected void grdUser_CustomUnboundColumnData1(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }

        protected void grdUser_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
        {

        }

        protected void grdUser_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
        {

        }

        protected void grdUserExport_RenderBrick(object sender, ASPxGridViewExportRenderingEventArgs e)
        {
            if (e.Column.Caption == "Net Out Going")
                e.TextValueFormatString = "{0:0.00}";
        }

        private GridViewDataTextColumn FindColumnByName(ASPxGridView gridView, string name)
        {
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (gridView.Columns[i].Name == name)
                    return (GridViewDataTextColumn)gridView.Columns[i];
            }
            return null;
        }

        private GridViewDataColumn FindColumnByNameData(ASPxGridView gridView, string name)
        {
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (gridView.Columns[i].Name == name)
                    return (GridViewDataColumn)gridView.Columns[i];
            }
            return null;
        }

        #endregion

        #region Domain Wise

        //public void BindDomainGrid(string Month, string Year)
        //{
        //    DataTable dt = bllMaster.GetDataForDomainWiseCostPerRecords(Month, Year);
        //    if (dt.Rows.Count > 0)
        //    {
        //        grdReport.DataSource = dt;
        //        grdReport.DataBind();
        //        lblSupport.Text = Convert.ToString(dt.Rows[0]["TotalSupportEmployees"]);
        //    }
        //}

        protected void grdReport_CustomUnboundColumnData(object sender, DevExpress.Web.ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }

        #endregion

        #region Project Wise

        //public void BindProjectGrid(string Month, string Year)
        //{
        //    DataTable dt = bllMaster.GetDataForProjectWiseCostPerRecords(Month, Year);
        //    if (dt.Rows.Count > 0)
        //    {
        //        grdProjectReport.DataSource = dt;
        //        grdProjectReport.DataBind();
        //    }
        //}

        protected void grdProjectReport_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }

        #endregion

        #region Cost Master

        #region Vendor Cost

        protected void btnVendorCost_Click(object sender, EventArgs e)
        {
            Hashtable htdailyV = new Hashtable();
            htdailyV["Month"] = drpMonth.SelectedItem.Text;
            htdailyV["Year"] = drpYear.Text.Trim();
            htdailyV["ProjectID"] = drpProjectName.SelectedValue;
            htdailyV["VolumeOutsourced"] = txtvolume.Text.Trim();
            htdailyV["VendorBilling"] = txtVendorBilling.Text.Trim();
            htdailyV["TotalCosting"] = txtTotalCosting.Text.Trim();
            htdailyV["AddedBy"] = HttpContext.Current.User.Identity.Name;
            int Returnvalue = bllMaster.InsertVendorCost(htdailyV);
            if (Returnvalue > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                lblError.Text = "Vendor Outsourcing Cost Saved successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                lblError.ForeColor = Color.Green;
                BindVendorCostgrid();
                clear();
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                lblError.Text = "Records already exists!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                lblError.ForeColor = Color.Red;
            }

            if (ddlMonth.SelectedIndex != 0 && ddlYear.SelectedIndex != 0)
            {
                BindGrid(ddlMonth.SelectedValue, ddlYear.SelectedValue);
                //BindDomainGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
                //BindProjectGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
            }
        }


        private void VendorCostMonth()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(null);
            for (int i = 1; i < 13; i++)
            {
                drpMonth.Items.Add(new ListItem(info.GetMonthName(i), i.ToString()));

            }

        }

        private void VendorCostYear()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(null);
            int year = DateTime.Now.Year - 5;
            for (int Y = year; Y <= DateTime.Now.Year; Y++)
            {
                drpYear.Items.Add(new ListItem(Y.ToString(), Y.ToString()));
            }
            drpYear.SelectedValue = DateTime.Now.Year.ToString();
        }

        public void BindVendorCostgrid()
        {
            DataTable dt = bllMaster.GetVendorCost();
            grdVendorCost.DataSource = dt;
            grdVendorCost.DataBind();

        }

        public void clear()
        {
            drpMonth.Items.Clear();
            VendorCostMonth();
            drpMonth.SelectedValue = Convert.ToString(DateTime.Now.Month);
            drpYear.SelectedValue = Convert.ToString(DateTime.Now.Year);
            drpProjectName.SelectedIndex = 0;
            txtvolume.Text = "";
            txtVendorBilling.Text = "";
            txtTotalCosting.Text = "";
        }

        public void BindProject()
        {
            DataTable dt = bllMaster.GetAllProject();
            drpProjectName.DataSource = dt;
            drpProjectName.DataTextField = "ProjectName";
            drpProjectName.DataValueField = "ProjectID";
            drpProjectName.DataBind();
            drpProjectName.Items.Insert(0, new ListItem("Select"));

        }

        protected void grdVendorCost_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            int index = -1;
            if (int.TryParse(e.Parameters, out index))
                grdVendorCost.SettingsEditing.Mode = (GridViewEditingMode)index;
        }

        protected void grdVendorCost_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }

        protected void grdVendorCost_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            ASPxGridView grid = sender as ASPxGridView;
            int CostId = Convert.ToInt32(e.Keys[0]);
            string Month = Convert.ToString(e.NewValues["Month"]).Trim();
            string Year = Convert.ToString(e.NewValues["Year"]).Trim();
            string VolumeOutsourced = Convert.ToString(e.NewValues["VolumeOutsourced"]).Trim();
            string VendorBilling = Convert.ToString(e.NewValues["VendorBilling"]).Trim();
            string TotalCosting = Convert.ToString(e.NewValues["TotalCosting"]).Trim();

            int ReturnValue = bllMaster.UpdateVendorCost(Month, CostId, Year, VolumeOutsourced, VendorBilling, TotalCosting);
            if (ReturnValue > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                ((ASPxGridView)sender).JSProperties["cp_message"] = "1";
                lblError.ForeColor = Color.Green;
                BindVendorCostgrid();
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                ((ASPxGridView)sender).JSProperties["cp_message"] = "0";
                lblError.ForeColor = Color.Red;
            }
            e.Cancel = true;
            grdVendorCost.CancelEdit();
            BindVendorCostgrid();
        }

        protected void drpProjectName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Other Cost

        protected void btnOtherCost_Click(object sender, EventArgs e)
        {
            Hashtable htdailyV1 = new Hashtable();

            htdailyV1["Month"] = drpMonth1.SelectedItem.Text;
            htdailyV1["Year"] = drpYear1.Text.Trim();
            htdailyV1["CostType"] = drpCostType.Text.Trim();
            htdailyV1["Amount"] = txtamount.Text.Trim();
            htdailyV1["Remark"] = txtremark.Text.Trim();
            htdailyV1["AddedBy"] = HttpContext.Current.User.Identity.Name;
            if (ddlDomainForOther.SelectedItem.Text == "All")
                htdailyV1["SubdomainID"] = Convert.ToInt32(9999);
            else
                htdailyV1["SubdomainID"] = Convert.ToInt32(ddlDomainForOther.SelectedValue);
            int Returnvalue = InsertOtherCost(htdailyV1);
            if (Returnvalue > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                lblError.Text = "Other Cost Saved successfully!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                lblError.ForeColor = Color.Green;
                OtherCostBindgrid();
                clear1();
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                lblError.Text = "Records already exists!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                lblError.ForeColor = Color.Red;
            }

            if (ddlMonth.SelectedIndex != 0 && ddlYear.SelectedIndex != 0)
            {
                BindGrid(ddlMonth.SelectedValue, ddlYear.SelectedValue);
                //BindDomainGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
                //BindProjectGrid(ddlMonth.SelectedValue, Convert.ToString(ddlYear.SelectedValue));
            }
        }

        public int InsertOtherCost(Hashtable htdailyV1)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_InsertOtherCost_Revised");
            SQLHelper.AddParamToSQLCmd(cmd, "@Month", System.Data.SqlDbType.NVarChar, 120, System.Data.ParameterDirection.Input, htdailyV1["Month"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Year", System.Data.SqlDbType.NVarChar, 120, System.Data.ParameterDirection.Input, htdailyV1["Year"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@CostType", System.Data.SqlDbType.NVarChar, 120, System.Data.ParameterDirection.Input, htdailyV1["CostType"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Amount", System.Data.SqlDbType.NVarChar, 120, System.Data.ParameterDirection.Input, htdailyV1["Amount"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@Remark", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, htdailyV1["Remark"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htdailyV1["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@SubdomainID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, htdailyV1["SubdomainID"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmd(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }

        private void OtherCostMonth()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(null);
            for (int i = 1; i < 13; i++)
            {
                drpMonth1.Items.Add(new ListItem(info.GetMonthName(i), i.ToString()));
            }

        }

        private void OtherCostYear()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(null);
            int year = DateTime.Now.Year - 5;
            for (int Y = year; Y <= DateTime.Now.Year; Y++)
            {
                drpYear1.Items.Add(new ListItem(Y.ToString(), Y.ToString()));
            }
            drpYear1.SelectedValue = DateTime.Now.Year.ToString();
        }

        public void clear1()
        {
            drpMonth1.Items.Clear();
            OtherCostMonth();
            drpMonth1.SelectedValue = Convert.ToString(DateTime.Now.Month);
            drpYear1.SelectedValue = Convert.ToString(DateTime.Now.Year);
            drpCostType.SelectedIndex = 0;
            txtamount.Text = "";
            txtremark.Text = "";
        }

        protected void grdOtherCost_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            ASPxGridView grid = sender as ASPxGridView;
            int OtherCId = Convert.ToInt32(e.Keys[0]);
            string Month = Convert.ToString(e.NewValues["Month"]).Trim();
            string Year = Convert.ToString(e.NewValues["Year"]).Trim();

            string CostType = Convert.ToString(e.NewValues["CostType"]).Trim();
            string Amount = Convert.ToString(e.NewValues["Amount"]).Trim();
            string Remark = Convert.ToString(e.NewValues["Remark"]).Trim();

            int ReturnValue = bllMaster.UpdateOtherCost(Month, OtherCId, Year, CostType, Amount, Remark);
            if (ReturnValue > 0)
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                ((ASPxGridView)sender).JSProperties["cp_message"] = "1";
                lblError.ForeColor = Color.Green;
                OtherCostBindgrid();
            }
            else
            {
                dvError.Style.Add("display", "");
                dvError.Visible = true;
                ((ASPxGridView)sender).JSProperties["cp_message"] = "0";
                lblError.ForeColor = Color.Red;
            }
            e.Cancel = true;
            grdOtherCost.CancelEdit();
            OtherCostBindgrid();
        }

        public void OtherCostBindgrid()
        {
            DataTable dt1 = bllMaster.GetOtherCost();
            grdOtherCost.DataSource = dt1;
            grdOtherCost.DataBind();
        }

        protected void grdOtherCost_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            int index = -1;
            if (int.TryParse(e.Parameters, out index))
                grdOtherCost.SettingsEditing.Mode = (GridViewEditingMode)index;
        }

        protected void grdOtherCost_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }

        #endregion

        #endregion

        protected void lblNoOfEmployees_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["NoOfEmployees"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["NoOfEmployees", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblRecordsBilled_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["RecordsBilled"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["RecordsBilled", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblAmountBilled_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["AmountBilled"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["AmountBilled", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblProduction_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Production"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Production", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblMarketing_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Marketing"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Marketing", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblTotal_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Total"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Total", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        

        protected void lblSupportSalary_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Support Salary"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Support Salary", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblMSEB_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["MSEB"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["MSEB", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblInternet_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Internet"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Internet", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblRent_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Rent"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Rent", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblGeneralExpenses_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["General Expenses"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["General Expenses", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblRepairAndMaintenance_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Repair And Maintenance"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Repair And Maintenance", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblDepreciation_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Depreciation"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Depreciation", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblVendorCost_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["VendorCost"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["VendorCost", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblContractCost_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["Contract Cost"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["Contract Cost", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }

        protected void lblTotalCost_Load(object sender, EventArgs e)
        {
            ASPxGridView grid = grdReport;
            GridViewDataColumn col = grid.Columns["TotalCost"] as GridViewDataColumn;
            ASPxSummaryItem summary = grid.TotalSummary["TotalCost", DevExpress.Data.SummaryItemType.Sum];
            string text = summary.GetTotalFooterDisplayText(col, grid.GetTotalSummaryValue(summary));
            ASPxLabel label = (ASPxLabel)sender;
            label.Text = string.Format("{0}\r\n({1})", col.Caption, text);
            label.Style.Add("font-weight", "bold");
        }
    }
}
