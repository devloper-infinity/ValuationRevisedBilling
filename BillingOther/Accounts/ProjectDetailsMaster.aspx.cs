using BillingOther.App_Code.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace BillingOther.Accounts
{
    public partial class ProjectDetailsMaster : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Project Details";
            if (!IsPostBack) BindProjectInformationGrid();
        }

        public void BindProjectInformationGrid()
        {
            DataTable dt = ViewAllProjectApp();
            StringBuilder sb = new StringBuilder(); int n = 1;
            foreach (DataRow r in dt.Rows)
            {
                string projectId = Convert.ToString(r["PAI_ErpProjectID"]);
                string domainId = Convert.ToString(r["PAI_DomainId"]);
                string target;
                if (projectId == "395" || projectId == "87" || domainId == "36" || projectId == "426") target = "ProjectDetails.aspx";
                else if (domainId == "19") target = "ProjectDetailsTyping.aspx";
                else if (domainId == "2" && !(projectId == "203" || projectId == "400" || projectId == "385" || projectId == "373" || projectId == "352")) target = "ProjectDetailsFreight.aspx";
                else target = "ProjectDetails.aspx";
                string url = target + "?ERPProjectId=" + HttpUtility.UrlEncode(projectId);
                sb.Append("<tr><td class='edit-col'><a class='project-edit-btn' href='").Append(url).Append("'>Edit</a></td><td class='serial-col'>").Append(n++).Append("</td><td>").Append(H(r,"DomainName")).Append("</td><td>").Append(H(r,"PAI_Project_Name")).Append("</td><td>").Append(H(r,"PAI_ProcessName")).Append("</td><td>").Append(H(r,"PAI_Company_Name")).Append("</td><td>").Append(H(r,"PAI_Contact_Person")).Append("</td><td>").Append(H(r,"PAI_Phone_Number")).Append("</td><td class='address-col'>").Append(H(r,"PAI_Address")).Append("</td><td>").Append(H(r,"PAI_Remark")).Append("</td></tr>");
            }
            projectRows.InnerHtml = sb.ToString();
        }
        private static string H(DataRow r, string c) { return HttpUtility.HtmlEncode(r.Table.Columns.Contains(c) ? Convert.ToString(r[c]) : string.Empty); }
        public DataTable ViewAllProjectApp()
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "[usp_GetAllProjectDetails_BDM]");
            SQLHelper.AddParamToSQLCmd(cmd, "@EmployeeID", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.Input, int.Parse(HttpContext.Current.User.Identity.Name));
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }
        protected void btnAddNew_Click(object sender, EventArgs e) { Response.Redirect("../Accounts/ProjectDetails.aspx"); }
    }
}
