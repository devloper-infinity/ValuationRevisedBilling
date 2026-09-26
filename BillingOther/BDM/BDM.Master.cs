using BillingOther.App_Code.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BillingOther.BDM
{
    public partial class BDM : System.Web.UI.MasterPage
    {
        bllLogin bllLogin = new bllLogin();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInfo();
            }
        }
        private void BindInfo()
        {
            DataTable dt = bllLogin.GetUserInformation(int.Parse(HttpContext.Current.User.Identity.Name.ToString()));
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    spnUsername.InnerHtml = "Welcome "+Convert.ToString(dt.Rows[0]["FirstName"]);
                }
            }
        }
    }
}