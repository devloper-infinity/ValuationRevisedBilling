<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceViewer.aspx.cs" Inherits="BillingOther.Accounts.InvoiceViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<link rel="stylesheet" type="text/css" href="../crystalreportviewers13/js/crviewer/images/style.css" />
<script src="../crystalreportviewers13/js/crviewer/crv.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 100%;">
            <CR:CrystalReportViewer ID="ILSReport" runat="server" Width="100%" AutoDataBind="true"
                ToolPanelView="None" SeparatePages="False" />
        </div>
    </form>
</body>
</html>
