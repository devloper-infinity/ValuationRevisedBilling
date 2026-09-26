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
using BillingOther.App_Code.DAL;
using BillingOther.App_Code.BLL;

namespace BillingOther.Accounts
{
    public partial class ExcelViewer : System.Web.UI.Page
    {
        public string ProjectID;
        public string BillingPeriod;
        public string InvoiceId;
        public string ProjectName;
        public string DomainId;
        bllTracking blltracking = new bllTracking();
        protected void Page_Load(object sender, EventArgs e)
        {
            ProjectID = Convert.ToString(Request.QueryString["ProjectID"]);
            BillingPeriod = Convert.ToString(Request.QueryString["BillingPeriod"]);
            InvoiceId = Convert.ToString(Request.QueryString["InvoiceId"]);
            getprojectid(int.Parse(ProjectID));
            
            DataTable dt = new DataTable();

            dt = GetCrystalReport(int.Parse(ProjectID), BillingPeriod);
            WriteExcelResponse(dt, ProjectName + "_" + BillingPeriod);
        }

        private void WriteExcelResponse(DataTable table, string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Text.RegularExpressions.Regex.Replace(fileName, "[^A-Za-z0-9_-]", "_") + ".xls");
            System.Text.StringBuilder html = new System.Text.StringBuilder("<table border='1'><thead><tr>");
            foreach (DataColumn column in table.Columns) html.Append("<th>").Append(HttpUtility.HtmlEncode(column.ColumnName)).Append("</th>");
            html.Append("</tr></thead><tbody>");
            foreach (DataRow row in table.Rows)
            {
                html.Append("<tr>");
                foreach (DataColumn column in table.Columns) html.Append("<td>").Append(HttpUtility.HtmlEncode(Convert.ToString(row[column]))).Append("</td>");
                html.Append("</tr>");
            }
            Response.Write(html.Append("</tbody></table>").ToString());
            Response.End();
        }

        public DataTable GetBillingBase(int ProjectID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_BillingBase");
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectId", System.Data.SqlDbType.BigInt, 10, System.Data.ParameterDirection.Input, ProjectID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return dt;
        }

        public DataTable GetCrystalReport(int ProjectID, string BillingPeriod)
        {
            DataTable dt = null;
            string procedureName = "";
            if (DomainId == "19")
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
            else if (DomainId == "2")
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

        public void getprojectid(int Project_Cost_Id)
        {
            DataTable dtget = blltracking.GetProjectName(Project_Cost_Id);
            if (dtget.Rows.Count > 0)
            {
                ProjectName = Convert.ToString(dtget.Rows[0]["ProjectName"]);
                DomainId = Convert.ToString(dtget.Rows[0]["DomainId"]);

            }
        }
    }
}
