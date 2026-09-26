using BillingOther.App_Code.BLL;
using BillingOther.App_Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace BillingOther.Accounts
{
    public partial class ProjectwisePriceMaster : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = Master.FindControl("lblHeader") as HtmlGenericControl;
            if (h4 != null) h4.InnerText = "Project wise Price Details";
        }

        [WebMethod]
        public static List<OptionItem> GetDomains()
        {
            DataTable dt = new bllTracking().getalldomains(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            List<OptionItem> result = new List<OptionItem>();
            foreach (DataRow r in dt.Rows)
                result.Add(new OptionItem { Value = Convert.ToString(r["DomainID"]), Text = Convert.ToString(r["DomainName"]) });
            return result;
        }

        [WebMethod]
        public static List<Dictionary<string, string>> GetPricing(int domainId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "[usp_getallbillingParametersForCosting_ForAllProject]");
            SQLHelper.AddParamToSQLCmd(cmd, "@DomainID", SqlDbType.Int, 10, ParameterDirection.Input, domainId);
            DataTable dt = SQLHelper.ExecuteDataTableCmd_billing(cmd);
            return ToRows(dt);
        }

        private static List<Dictionary<string, string>> ToRows(DataTable dt)
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            foreach (DataRow r in dt.Rows)
            {
                Dictionary<string, string> item = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn c in dt.Columns) item[c.ColumnName] = Convert.ToString(r[c]);
                rows.Add(item);
            }
            return rows;
        }

        public class OptionItem { public string Value { get; set; } public string Text { get; set; } }
    }
}
