using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.IO;


namespace BillingOther.Accounts
{
    public partial class MasterReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "All Domain Master Report";
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=All Domain Master Report.xls");
            using (StringWriter sw = new StringWriter())
            using (HtmlTextWriter writer = new HtmlTextWriter(sw))
            {
                grdMaster.RenderControl(writer);
                Response.Write(sw.ToString());
            }
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public override void VerifyRenderingInServerForm(Control control) { }
    }
}
