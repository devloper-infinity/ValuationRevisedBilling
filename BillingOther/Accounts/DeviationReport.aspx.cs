using BillingOther.App_Code.DAL;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BillingOther.Accounts
{
    public partial class DeviationReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Year();
            }
            BindGrid();
        }

        public void BindGrid()
        {
            DataTable dtPrev = null;
            DataTable dtCurrent = null;
            DataTable ds = null;
            if (Session["Prev"] == null)
            {
                ds = GetCostingReport_Month1(Convert.ToString(ddlPreviousMonth.SelectedValue), Convert.ToString(ddlPreviousYear.SelectedValue));
                Session["Prev"] = ds;
            }
            else
                ds = (DataTable)Session["Prev"];
            if (ds != null)
            {
                lblPrevious.Text = Convert.ToString(ddlPreviousMonth.SelectedValue) + " ~ " + Convert.ToString(ddlPreviousYear.SelectedValue);
                dtPrev = ds;
                dtPrev.TableName = "TablePrevious";
                grdPreviousDomain.DataSource = ds;
                grdPreviousDomain.DataBind();
            }
            if (Session["Current"] == null)
            {
                ds = GetCostingReport_Month2(Convert.ToString(ddlCurrentMonth.SelectedValue), Convert.ToString(ddlCurrentYear.SelectedValue));
                Session["Current"] = ds;
            }
            else
                ds = (DataTable)Session["Current"];
            // ds = GetCostingReport_Month2(Convert.ToString(ddlCurrentMonth.SelectedValue), Convert.ToString(ddlCurrentYear.SelectedValue));
            if (ds != null)
            {
                lblCurrent.Text = Convert.ToString(ddlCurrentMonth.SelectedValue) + " ~ " + Convert.ToString(ddlCurrentYear.SelectedValue);
                dtCurrent = ds;
                dtCurrent.TableName = "TableCurrent";
                grdCurrentDomain.DataSource = ds;
                grdCurrentDomain.DataBind();


            }

            if (Session["Diff"] == null)
            {
                ds = GetCostingReport_Differnece();
                Session["Diff"] = ds;
            }
            else
                ds = (DataTable)Session["Diff"];
            // ds = GetCostingReport_Month2(Convert.ToString(ddlCurrentMonth.SelectedValue), Convert.ToString(ddlCurrentYear.SelectedValue));
            if (ds != null)
            {
                grdDifference.DataSource = ds;
                grdDifference.DataBind();


            }
        }

        public static DataTable DifferenceAmount(DataTable dtCurrent, DataTable dtPrev)
        {
            DataTable dtResult = new DataTable("Difference");

            dtResult = dtCurrent.Copy();
            dtResult.Rows.Clear();

            for (int i = 0; i < dtCurrent.Rows.Count; i++)
            {
                DataRow dr = dtResult.NewRow();
                for (int j = 2; j < dtCurrent.Columns.Count; j++)
                {

                }
            }

            return dtResult;
        }

        public static DataTable Difference(DataTable First, DataTable Second)
        {

            //Create Empty Table
            DataTable table = new DataTable("Difference");

            //Must use a Dataset to make use of a DataRelation object
            using (DataSet ds = new DataSet())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });

                //Get Columns for DataRelation
                DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count];

                for (int i = 0; i < firstcolumns.Length; i++)
                {
                    firstcolumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondcolumns.Length; i++)
                {
                    secondcolumns[i] = ds.Tables[1].Columns[i];
                }

                //Create DataRelation
                DataRelation r1 = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);
                ds.Relations.Add(r1);

                DataRelation r2 = new DataRelation(string.Empty, secondcolumns, firstcolumns, false);
                ds.Relations.Add(r2);

                //Create columns for return table
                table = First.Clone();

                //If First Row not in Second, Add to return table.
                table.BeginLoadData();

                foreach (DataRow parentrow in ds.Tables[0].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r1);
                    if (childrows == null || childrows.Length == 0)
                        table.LoadDataRow(parentrow.ItemArray, true);
                }

                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r2);
                    if (childrows == null || childrows.Length == 0)
                        table.LoadDataRow(parentrow.ItemArray, true);
                }

                table.EndLoadData();
            }

            return table;

        }

        public DataTable GetCostingReport_Month1(string Month, string year)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[GetCostPerRecordReport_Domain_Month1]");
            SQLHelper.AddParamToSQLCmd(cmd, "@Month", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Month);
            SQLHelper.AddParamToSQLCmd(cmd, "@Year", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, year);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        public DataTable GetCostingReport_Month2(string Month, string year)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[GetCostPerRecordReport_Domain_Month2]");
            SQLHelper.AddParamToSQLCmd(cmd, "@Month", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Month);
            SQLHelper.AddParamToSQLCmd(cmd, "@Year", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, year);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        public DataTable GetCostingReport_Differnece()
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[GetCostPerRecordDeviation]");
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        private void Year()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(null);
            int lastyear = DateTime.Now.Year - 5;
            int year = DateTime.Now.Year;

            for (int Y = year; Y >= lastyear; Y--)
            {
                ddlPreviousYear.Items.Add(new ListItem(Y.ToString(), Y.ToString()));
                ddlCurrentYear.Items.Add(new ListItem(Y.ToString(), Y.ToString()));
            }
            ddlPreviousYear.Items.Insert(0, new ListItem("Select"));
            ddlCurrentYear.Items.Insert(0, new ListItem("Select"));
        }

        protected void btnExpoetToExcel_Click(object sender, EventArgs e)
        {
            var exportOptions = new XlsExportOptionsEx();
            exportOptions.ExportType = DevExpress.Export.ExportType.DataAware;
            exportOptions.ExportHyperlinks = true;

            PrintingSystemBase ps = new PrintingSystemBase();
            ps.ExportOptions.Xlsx.SheetName = "";


            PrintableComponentLinkBase link1 = new PrintableComponentLinkBase(ps);
            link1.Component = grdPreviousExport;


            PrintableComponentLinkBase link2 = new PrintableComponentLinkBase(ps);
            link2.Component = grdCurrentExport;

            PrintableComponentLinkBase link3 = new PrintableComponentLinkBase(ps);
            link3.Component = grdDifferenceExport;

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
                Response.AppendHeader("Content-Disposition", "attachment; filename=Cost Per Record Deviation Report.xlsx");
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
            ps.Dispose();

        }

        void PrintingSystem_XlsxDocumentCreated(object sender, DevExpress.XtraPrinting.XlsxDocumentCreatedEventArgs e)
        {
            e.SheetNames[0] = Convert.ToString(ddlPreviousMonth.SelectedValue) + " - " + Convert.ToString(ddlPreviousYear.SelectedValue);
            e.SheetNames[1] = Convert.ToString(ddlCurrentMonth.SelectedValue) + " - " + Convert.ToString(ddlCurrentYear.SelectedValue);
            e.SheetNames[2] = "Difference";
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            Session["Prev"] = null;
            Session["Current"] = null;
            Session["Diff"] = null;
            BindGrid();
        }

        protected void grdPreviousDomain_CustomUnboundColumnData(object sender, DevExpress.Web.ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }

        protected void grdCurrentDomain_CustomUnboundColumnData(object sender, DevExpress.Web.ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }
    }
}