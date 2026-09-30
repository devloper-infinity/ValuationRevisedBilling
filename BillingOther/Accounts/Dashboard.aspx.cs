using BillingOther.App_Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace BillingOther.Accounts
{
 public partial class Dashboard : Page
 {
  protected void Page_Load(object sender, EventArgs e) { HtmlGenericControl h = Master.FindControl("lblHeader") as HtmlGenericControl; if (h != null) h.InnerText = "Management Billing Dashboard"; }
  [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
  public static DashboardResult GetDashboard(DashboardFilter filter)
  {
   DashboardResult r = new DashboardResult();
   try {
    filter = filter ?? new DashboardFilter(); SqlCommand cmd = SQLHelper.GetCommand(CommandType.StoredProcedure, "IB_usp_AccountsManagementDashboard");
    Add(cmd,"@Year",SqlDbType.Int,filter.Year); Add(cmd,"@Month",SqlDbType.Int,filter.Month); Add(cmd,"@DomainID",SqlDbType.Int,filter.DomainId); Add(cmd,"@ProjectID",SqlDbType.Int,filter.ProjectId);
    Add(cmd,"@BillingCycle",SqlDbType.NVarChar,filter.BillingCycle); Add(cmd,"@BillingStatus",SqlDbType.NVarChar,filter.BillingStatus); Add(cmd,"@Client",SqlDbType.NVarChar,filter.Client);
    DataSet d=SQLHelper.ExecuteDataSetCmd_Billing(cmd); r.Success=true; r.Filters=Rows(d,0); r.Kpis=First(d,1); r.Domains=Rows(d,2); r.Projects=Rows(d,3); r.Trends=Rows(d,4); r.Attention=Rows(d,5); r.Details=new List<Dictionary<string,object>>();
   } catch(Exception ex) { r.Success=false; r.Message=ex.Message.IndexOf("IB_usp_AccountsManagementDashboard",StringComparison.OrdinalIgnoreCase)>=0 ? "Dashboard procedure is not installed. Run Database/002_AccountsManagementDashboard.sql." : "Dashboard data could not be loaded."; }
   return r;
  }
  [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
  public static List<Dictionary<string,object>> GetProjectDetails(int projectId,int year,int month)
  {
   SqlCommand cmd=SQLHelper.GetCommand(CommandType.StoredProcedure,"IB_usp_AccountsManagementDashboard");
   Add(cmd,"@Year",SqlDbType.Int,year);Add(cmd,"@Month",SqlDbType.Int,month);Add(cmd,"@DomainID",SqlDbType.Int,0);Add(cmd,"@ProjectID",SqlDbType.Int,projectId);
   Add(cmd,"@BillingCycle",SqlDbType.NVarChar,null);Add(cmd,"@BillingStatus",SqlDbType.NVarChar,null);Add(cmd,"@Client",SqlDbType.NVarChar,null);Add(cmd,"@DetailProjectID",SqlDbType.Int,projectId);
   return Rows(SQLHelper.ExecuteDataSetCmd_Billing(cmd),0);
  }
  private static void Add(SqlCommand c,string n,SqlDbType t,object v) { SqlParameter p=c.Parameters.Add(n,t); p.Value=v==null||(v is string&&string.IsNullOrWhiteSpace((string)v))||(v is int&&(int)v==0)?(object)DBNull.Value:v; }
  private static List<Dictionary<string,object>> Rows(DataSet d,int i) { var a=new List<Dictionary<string,object>>(); if(d==null||d.Tables.Count<=i)return a; foreach(DataRow row in d.Tables[i].Rows){var x=new Dictionary<string,object>(StringComparer.OrdinalIgnoreCase);foreach(DataColumn col in d.Tables[i].Columns)x[col.ColumnName]=row[col]==DBNull.Value?null:row[col];a.Add(x);}return a; }
  private static Dictionary<string,object> First(DataSet d,int i){var a=Rows(d,i);return a.Count==0?new Dictionary<string,object>():a[0];}
 }
 public class DashboardFilter { public int Year{get;set;} public int Month{get;set;} public int DomainId{get;set;} public int ProjectId{get;set;} public string BillingCycle{get;set;} public string BillingStatus{get;set;} public string Client{get;set;} }
 public class DashboardResult { public bool Success{get;set;} public string Message{get;set;} public List<Dictionary<string,object>> Filters{get;set;} public Dictionary<string,object> Kpis{get;set;} public List<Dictionary<string,object>> Domains{get;set;} public List<Dictionary<string,object>> Projects{get;set;} public List<Dictionary<string,object>> Trends{get;set;} public List<Dictionary<string,object>> Attention{get;set;} public List<Dictionary<string,object>> Details{get;set;} }
}
