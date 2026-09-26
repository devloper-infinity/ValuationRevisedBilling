using BillingOther.App_Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace BillingOther.Accounts
{
    public partial class ProjectGroupMaster : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl h4 = (HtmlGenericControl)Master.FindControl("lblHeader");
            if (h4 != null) h4.InnerText = "Project Group Master";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<ProjectOptionDto> GetProjects()
        {
            DataTable dt = new DALTracking().GetAllProjectByUserRights();
            List<ProjectOptionDto> result = new List<ProjectOptionDto>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new ProjectOptionDto
                    {
                        ProjectId = Convert.ToString(row["ProjectId"]),
                        ProjectName = Convert.ToString(row["ProjectName"])
                    });
                }
            }
            return result;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<ProjectGroupDto> GetGroups()
        {
            DataTable dt = GetAllProjectGroupStatic();
            List<ProjectGroupDto> result = new List<ProjectGroupDto>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new ProjectGroupDto
                    {
                        GroupNumber = GetValue(row, "GroupNumber"),
                        ProjectName = GetValue(row, "ProjectName"),
                        AddedByName = GetValue(row, "AddedByName"),
                        AddedDate = FormatDate(GetObject(row, "AddedDate"))
                    });
                }
            }
            return result;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static SaveResult SaveConfiguration(string groupNumber, string[] projectIds)
        {
            try
            {
                groupNumber = (groupNumber ?? string.Empty).Trim();
                if (groupNumber.Length == 0)
                    return new SaveResult { Success = false, Message = "Please enter group number." };
                if (projectIds == null || projectIds.Length == 0)
                    return new SaveResult { Success = false, Message = "Please select at least one project." };

                int addedBy = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
                int saved = 0;
                foreach (string projectId in projectIds)
                {
                    int parsedProjectId;
                    if (!int.TryParse(projectId, out parsedProjectId)) continue;
                    Hashtable htParam = new Hashtable();
                    htParam.Add("GroupNumber", groupNumber);
                    htParam.Add("ProjectID", parsedProjectId);
                    htParam.Add("AddedBy", addedBy);
                    InsertProjectGroupStatic(htParam);
                    saved++;
                }

                return new SaveResult
                {
                    Success = saved > 0,
                    Message = saved > 0 ? "Configuration saved successfully." : "No valid projects were selected."
                };
            }
            catch (Exception ex)
            {
                return new SaveResult { Success = false, Message = ex.Message };
            }
        }

        private static int InsertProjectGroupStatic(Hashtable htParam)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_InsertProjectGroups");
            SQLHelper.AddParamToSQLCmd(cmd, "@GroupNumber", SqlDbType.NVarChar, 100, ParameterDirection.Input, htParam["GroupNumber"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProjectID", SqlDbType.Int, 0, ParameterDirection.Input, htParam["ProjectID"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@AddedBy", SqlDbType.Int, 0, ParameterDirection.Input, htParam["AddedBy"]);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", SqlDbType.BigInt, 0, ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmdBilling(cmd);
            return Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
        }

        private static DataTable GetAllProjectGroupStatic()
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "usp_GetAllProjectGroups");
            return SQLHelper.ExecuteDataTableCmd_billing(cmd);
        }

        private static object GetObject(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? row[column] : null;
        }

        private static string GetValue(DataRow row, string column)
        {
            object value = GetObject(row, column);
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }

        private static string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value) return string.Empty;
            DateTime date;
            return DateTime.TryParse(Convert.ToString(value), out date)
                ? date.ToString("dd-MMM-yyyy hh:mm tt")
                : Convert.ToString(value);
        }

        public class ProjectOptionDto
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
        }

        public class ProjectGroupDto
        {
            public string GroupNumber { get; set; }
            public string ProjectName { get; set; }
            public string AddedByName { get; set; }
            public string AddedDate { get; set; }
        }

        public class SaveResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}
