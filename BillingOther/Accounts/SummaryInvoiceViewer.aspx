<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SummaryInvoiceViewer.aspx.cs" Inherits="BillingOther.Accounts.SummaryInvoiceViewer" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
     <div>
  <table width="100%">
        <tr>
            <td>
                <CR:CrystalReportViewer ID="ILSReport" runat="server" Width="350px" AutoDataBind="true"
                    ToolPanelView="None" Height="50px" SeparatePages="False" />
               
            </td>
        </tr>
    </table>
    </div>
        <div id="divexcel" runat="server" visible="false"></div>
    </form>
</body>
</html>
