using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace BillingOther.Accounts
{
    public partial class CheckProcedures : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportDocument rpt = new ReportDocument();
            rpt.Load(Server.MapPath("~/Reports/Valuation/SummaryReport.rpt"));

            Console.WriteLine("=== MAIN REPORT ===");

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in rpt.Database.Tables)
            {
                Console.WriteLine($"Table/SP: {table.Location}");
            }

            foreach (ReportDocument sub in rpt.Subreports)
            {
                Console.WriteLine($"=== SUBREPORT: {sub.Name} ===");

                foreach (CrystalDecisions.CrystalReports.Engine.Table table in sub.Database.Tables)
                {
                    Console.WriteLine($"Table/SP: {table.Location}");
                }
            }

            Console.WriteLine("\n=== COMMAND OBJECTS ===");
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in rpt.Database.Tables)
            {
                //if (table.ClassName == "CommandTable")
                //{
                //    Console.WriteLine(table.Name);
                //}
            }
        }
    }
}