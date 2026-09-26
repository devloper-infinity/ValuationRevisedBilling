using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace BillingOther.Accounts
{
    public partial class MasterReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "All Domain Master Report";
        }

        protected void grdMaster_CustomUnboundColumnData(object sender, DevExpress.Web.ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("{0}", e.ListSourceRowIndex + 1);
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var options = new XlsxExportOptionsEx();
            options.ExportType = DevExpress.Export.ExportType.WYSIWYG;
            gridExport.WriteXlsxToResponse(options);
        }

        protected void gridExport_RenderBrick(object sender, DevExpress.Web.ASPxGridViewExportRenderingEventArgs e)
        {
            if (e.RowType == DevExpress.Web.GridViewRowType.Header)
            {
                e.BrickStyle.ForeColor = Color.Black;
                e.BrickStyle.BackColor = Color.White;
                FontFamily fm = new FontFamily("Calibri");
                e.BrickStyle.Font = new Font(fm, 10, FontStyle.Bold);
            }
            else
            {
                e.BrickStyle.ForeColor = Color.Black;
                e.BrickStyle.BackColor = Color.White;
                FontFamily fm = new FontFamily("Calibri");
                e.BrickStyle.Font = new Font(fm, 10, FontStyle.Regular);
            }
            
        }
    }
}
