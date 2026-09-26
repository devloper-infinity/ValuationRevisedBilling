using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BillingOther.App_Code.DAL
{
    public class DALLogin
    {
        public int ValidateUser(string Username, string Password)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_ValidateUser");
            SQLHelper.AddParamToSQLCmd(cmd, "@Username", System.Data.SqlDbType.NVarChar, 50, System.Data.ParameterDirection.Input, Username);
            SQLHelper.AddParamToSQLCmd(cmd, "@Password", System.Data.SqlDbType.NVarChar, 50, System.Data.ParameterDirection.Input, Password);
            SQLHelper.AddParamToSQLCmd(cmd, "@ReturnValue", System.Data.SqlDbType.BigInt, 0, System.Data.ParameterDirection.ReturnValue, null);
            SQLHelper.ExecuteNonQueryCmd(cmd);

            int ReturnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            cmd.Dispose();
            return ReturnValue; //-1=User Not Exist, 0=Invalid Password, >0=Success

        }

        public DataTable GetUserById(int EmployeeID, string Username, string Password)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetUserById_ForOST");
            SQLHelper.AddParamToSQLCmd(cmd, "@EmployeeID", System.Data.SqlDbType.BigInt, 50, System.Data.ParameterDirection.Input, EmployeeID);
            SQLHelper.AddParamToSQLCmd(cmd, "@Username", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Username);
            SQLHelper.AddParamToSQLCmd(cmd, "@Password", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, Password);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        public DataTable BlockUserLogin(string Code)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_BlockUserLogin");
            SQLHelper.AddParamToSQLCmd(cmd, "@Code", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, Code);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }
        public DataTable GetUserInformation(int EmployeeID)
        {
            SqlCommand cmd = SQLHelper.GetCommand(System.Data.CommandType.StoredProcedure, "usp_GetUserInformation");
            SQLHelper.AddParamToSQLCmd(cmd, "@EmployeeID", System.Data.SqlDbType.BigInt, 50, System.Data.ParameterDirection.Input, EmployeeID);
            DataTable dt = SQLHelper.ExecuteDataTableCmd(cmd);
            return dt;
        }

        public string GetMfaSecret(int employeeId)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.Text, "SELECT TOP (1) ProtectedSecret FROM dbo.BillingPortalMfa WHERE EmployeeId = @EmployeeId AND IsEnabled = 1");
            SQLHelper.AddParamToSQLCmd(cmd, "@EmployeeId", SqlDbType.Int, 0, ParameterDirection.Input, employeeId);
            object value = SQLHelper.ExecuteScalarCmd(cmd);
            return value == null || value == DBNull.Value ? null : Convert.ToString(value);
        }

        public void SaveMfaSecret(int employeeId, string protectedSecret)
        {
            SqlCommand cmd = SQLHelper.GetCommand(CommandType.Text, @"MERGE dbo.BillingPortalMfa AS target
USING (SELECT @EmployeeId AS EmployeeId) AS source ON target.EmployeeId = source.EmployeeId
WHEN MATCHED THEN UPDATE SET ProtectedSecret=@ProtectedSecret, IsEnabled=1, EnrolledOnUtc=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT (EmployeeId, ProtectedSecret, IsEnabled, EnrolledOnUtc) VALUES (@EmployeeId,@ProtectedSecret,1,SYSUTCDATETIME());");
            SQLHelper.AddParamToSQLCmd(cmd, "@EmployeeId", SqlDbType.Int, 0, ParameterDirection.Input, employeeId);
            SQLHelper.AddParamToSQLCmd(cmd, "@ProtectedSecret", SqlDbType.NVarChar, 512, ParameterDirection.Input, protectedSecret);
            SQLHelper.ExecuteNonQueryCmd(cmd);
        }
    }
}
