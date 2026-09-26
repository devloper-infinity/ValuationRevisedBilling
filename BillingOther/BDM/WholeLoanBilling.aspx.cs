using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BillingOther.BDM
{
    public partial class WholeLoanBilling : System.Web.UI.Page
    {
        static DataTable dtGen = null;
        bllTracking blltracking = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Year();
            }
        }

        private void Year()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(null);
            int year = 2000;

            for (int Y = year; Y <= DateTime.Now.Year; Y++)
            {
                ddlYear.Items.Add(new ListItem(Y.ToString(), Y.ToString()));
            }

            ddlYear.SelectedValue = DateTime.Now.Year.ToString();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string StrSource = "E:\\Securitization";
            string FileNm = Path.GetFileName(fpAttachment.PostedFile.FileName);
            string fileName = FileNm.Substring(FileNm.LastIndexOf("\\") + 1);
            string Extn = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (Extn == "xls" | Extn == "xlsx")
            {
                if (!Directory.Exists(StrSource))
                {
                    Directory.CreateDirectory(StrSource);
                }
                string strSourceFile = StrSource + "\\" + fileName;
                fpAttachment.SaveAs(strSourceFile);
                string ConExcel;
                if (Extn.Contains("xlsx"))
                {
                    ConExcel = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + strSourceFile + "; Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                }
                else
                {
                    ConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + strSourceFile + "; Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                }
                DataSet dsExcel = new DataSet();
                DataTable Dt = new DataTable("[Sheet1$]");
                using (OleDbConnection myExcelConnection = new OleDbConnection(ConExcel))
                {
                    //myExcelConnection.Open();

                    string sqlExcel = "";
                    sqlExcel = "Select * from [Sheet1$]";
                    OleDbDataAdapter daExcel = new OleDbDataAdapter(sqlExcel, myExcelConnection);
                    daExcel.Fill(dsExcel);
                    daExcel.Dispose();
                    Dt = dsExcel.Tables[0];
                    dtGen = Dt.Copy();
                    if (myExcelConnection.State == ConnectionState.Open)
                    {
                        myExcelConnection.Close();
                    }
                    if (Dt != null)
                    {
                        grdVolume.Columns.Clear();
                        grdVolume.AutoGenerateColumns = true;
                        grdVolume.DataSource = Dt;
                        grdVolume.DataBind();
                        btnAddtoDatabase.Style.Add("display", "");
                    }
                }
            }
        }

        protected void btnAddtoDatabase_Click(object sender, EventArgs e)
        {
            string BillingPeriod = "";
            string StartDate = "1-" + Convert.ToString(ddlMonth.SelectedValue).Substring(0, 3) + "-" + Convert.ToString(ddlYear.SelectedValue);
            StartDate = Convert.ToDateTime(StartDate).ToString("dd-MMM-yyyy");
            string EndDate = Convert.ToString(Convert.ToDateTime(StartDate).AddMonths(1).AddDays(-1));
            EndDate = Convert.ToDateTime(EndDate).ToString("dd-MMM-yyyy");
            BillingPeriod = StartDate + " ~ " + EndDate;
            if (dtGen.Rows.Count > 0)
            {
                dtGen.Columns.Add("Purchaser");
                dtGen.Columns.Add("Seller");
                dtGen.Columns.Add("Loan Count");
                dtGen.Columns.Add("Closing Date");
                dtGen.Columns.Add("Bill To");
                //dtGen.Columns.Add("Cut-off Date");
                dtGen.Columns.Add("isVerify");
                dtGen.Columns.Add("VerifiedBy");
                dtGen.Columns.Add("BillingPeriod");
                dtGen.Columns.Add("TradeName");
                dtGen.Columns.Add("PayingEntity");
                dtGen.AcceptChanges();
                foreach (DataRow dr in dtGen.Rows)
                {
                    dr["Purchaser"] = Convert.ToString(txtPurchaser.Text);
                    dr["Seller"] = Convert.ToString(txtSeller.Text);
                    dr["Loan Count"] = Convert.ToString(txtLoanCount.Text);
                    dr["Closing Date"] = Convert.ToString(txtClosingDate.Text);
                    dr["Bill To"] = Convert.ToString(txtBilledTo.Text);
                    //dr["Cut-off Date"] = Convert.ToString(txtCutOffDate.Text);
                    dr["isVerify"] = true;
                    dr["VerifiedBy"] = Convert.ToString(HttpContext.Current.User.Identity.Name.ToString());
                    dr["BillingPeriod"] = Convert.ToString(BillingPeriod);
                    dr["TradeName"] = Convert.ToString(txtTradeName.Text);
                    dr["PayingEntity"] = Convert.ToString(txtpayingEntity.Text);
                }
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = "Data Source=23.111.175.186;Initial Catalog=InfinityBilling;Persist Security Info=True;User ID=sa;Password=#Cl0ud^$ecure4; Pooling=true; Min Pool Size=1; Max Pool Size=10; Connect Timeout=200; Packet Size=8192";
                SqlBulkCopy objbulk = new SqlBulkCopy(sqlConnection);
                //assigning Destination table name
                objbulk.DestinationTableName = "dbo.WholeLoanBilling";
                string destTableQuery = "Select top 1 * from dbo.WholeLoanBilling";
                SqlCommand cmd = new SqlCommand(destTableQuery);
                sqlConnection.Open();
                cmd.Connection = sqlConnection;
                // i use sql helper for executing query you can use corde sw
                DataTable dtDest = SQLHelper.ExecuteDataSetCmd_Billing(cmd).Tables[0];
                //Mapping Table column
                for (int i = 0; i < dtDest.Columns.Count; i++)
                {
                    string destinationColumnName = dtDest.Columns[i].Caption.ToString();
                    if (dtGen.Columns.Contains(destinationColumnName))
                    {
                        //Once column matched get its index
                        int sourceColumnIndex = dtGen.Columns.IndexOf(destinationColumnName);

                        string sourceColumnName = dtGen.Columns[sourceColumnIndex].ToString();

                        // give column name of source table rather then destination table 
                        // so that it would avoid case sensitivity
                        objbulk.ColumnMappings.Add(sourceColumnName, sourceColumnName);
                    }
                }

                objbulk.WriteToServer(dtGen);
                sqlConnection.Close();
                int returnvalue = InsertPojectBillingDetailsWLA(620, BillingPeriod, int.Parse(HttpContext.Current.User.Identity.Name.ToString()));

                dvError.Style.Add("display", "");
                dvError.Attributes.Add("class", "alert alert-warning background-success");
                dvError.InnerHtml = "Billing added successfully.";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
        }

        public int InsertPojectBillingDetailsWLA(int ProjectID, string BillingPeriod, int AddedBy)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_InsertProjectBillingDetails_WLA");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, ProjectID);
            SQLHelper.AddParamToSQLCmd(cmd, "@BillingPeriod", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, BillingPeriod);
            SQLHelper.AddParamToSQLCmd(cmd, "@Client", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, txtBilledTo.Text.Trim());
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", System.Data.SqlDbType.NVarChar, 1000, System.Data.ParameterDirection.Input, AddedBy);

            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            return ReturnValue;
        }
    }
}