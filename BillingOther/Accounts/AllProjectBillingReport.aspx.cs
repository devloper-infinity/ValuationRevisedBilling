using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using BillingOther.App_Code.DAL;

namespace BillingOther.Accounts
{
    public partial class AllProjectBillingReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = Master.FindControl("lblHeader") as HtmlGenericControl;
            if (h4 != null)
                h4.InnerText = "All Project Billing Report";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<Dictionary<string, object>> GetBillingReport(string fromDate, string toDate)
        {
            DataTable dt = GetAllProjectSummary(fromDate ?? string.Empty, toDate ?? string.Empty);
            return ToRows(dt);
        }

        private static DataTable GetAllProjectSummary(string fromDate, string toDate)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "[WBT_usp_GetAllProjectSummary]");
            SQLHelper.AddParamToSQLCmd(cmd, "@FromMonth", SqlDbType.NVarChar, 100, ParameterDirection.Input, fromDate);
            SQLHelper.AddParamToSQLCmd(cmd, "@ToMonth", SqlDbType.NVarChar, 100, ParameterDirection.Input, toDate);
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        private static List<Dictionary<string, object>> ToRows(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            if (dt == null) return rows;

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn col in dt.Columns)
                    row[col.ColumnName] = FormatValue(col.ColumnName, dr[col]);

                EnsureKey(row, "Domain");
                EnsureKey(row, "Subdomain");
                EnsureKey(row, "ProjectName");
                EnsureKey(row, "BillingPeriod");
                EnsureKey(row, "OrderCount");
                EnsureKey(row, "TotalAmount");
                EnsureKey(row, "AddedDate");
                EnsureKey(row, "InvocieNumber");
                EnsureKey(row, "InvoiceSent");
                EnsureKey(row, "DeliveredDate");
                EnsureKey(row, "ReadDate");
                rows.Add(row);
            }
            return rows;
        }

        private static void EnsureKey(Dictionary<string, object> row, string key)
        {
            if (!row.ContainsKey(key)) row[key] = string.Empty;
        }

        private static object FormatValue(string columnName, object value)
        {
            if (value == null || value == DBNull.Value) return string.Empty;

            DateTime dt;
            if ((columnName.IndexOf("Date", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 columnName.IndexOf("Period", StringComparison.OrdinalIgnoreCase) >= 0) &&
                DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out dt))
            {
                return dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }

            return value;
        }
    }
}
