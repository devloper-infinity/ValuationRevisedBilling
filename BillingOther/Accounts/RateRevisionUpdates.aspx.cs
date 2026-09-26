using BillingOther.App_Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace BillingOther.Accounts
{
    public partial class RateRevisionUpdates : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = Master.FindControl("lblHeader") as HtmlGenericControl;
            if (h4 != null) h4.InnerText = "Rate Revision Updates";
        }

        [WebMethod]
        public static List<Dictionary<string, string>> GetRateRevisionUpdatesAjax()
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetRateRevisionUpdates");
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            foreach (DataRow r in dt.Rows)
            {
                Dictionary<string, string> item = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn c in dt.Columns) item[c.ColumnName] = Convert.ToString(r[c]);
                rows.Add(item);
            }
            return rows;
        }
    }
}
