<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ProductMapping.aspx.cs" Inherits="BillingOther.Accounts.ProductMapping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function HideLabel() {
            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%=dvError.ClientID %>").style.display = "none";
             }, seconds * 1000);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server"></div>

    <table class="table table-condensed">
        <tr>
            <td><b>Project:</b></td>
            <td>
                <asp:DropDownList ID="ddlProject" runat="server" Width="500px" CssClass="form-control"></asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvProject" runat="server" ControlToValidate="ddlProject" ErrorMessage="Please select project" InitialValue="Select" Display="Dynamic" ForeColor="Red" ValidationGroup="mapping"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td><b>Billing Product Type:</b></td>
            <td>
                <asp:DropDownList ID="ddlProductBilling" runat="server" CssClass="form-control" Width="500px" AutoPostBack="true" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged">
                    <asp:ListItem Value="Select">Select</asp:ListItem>
                    <asp:ListItem Value="FHA">FHA</asp:ListItem>
                    <asp:ListItem Value="Multifamily">Multifamily</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlProductBilling" ErrorMessage="Please select product type" InitialValue="Select" Display="Dynamic" ForeColor="Red" ValidationGroup="mapping"></asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td style="vertical-align: top;"><b>ERP Product Types:</b></td>
            <td>
                <asp:ListBox ID="ddlProductERP" runat="server" SelectionMode="Multiple" CssClass="form-control" Width="500px" Height="250px"></asp:ListBox>
            </td>
            <td></td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" ValidationGroup="mapping" />
            </td>
            <td></td>
        </tr>
    </table>
</asp:Content>
