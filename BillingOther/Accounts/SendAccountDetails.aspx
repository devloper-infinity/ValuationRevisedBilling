<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="SendAccountDetails.aspx.cs" Inherits="BillingOther.Accounts.SendAccountDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table class="table table-border-style">
            <tr>
                <td><b>Domain:</b></td>
                <td>
                    <asp:DropDownList ID="ddldomain" runat="server" AutoPostBack="true" Width="200px" OnSelectedIndexChanged="ddldomain_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" SetFocusOnError="true" runat="server"
                        ValidationGroup="Client" ControlToValidate="ddldomain" InitialValue="Select" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Please Select Domain"></asp:RequiredFieldValidator>
                </td>
                <td><b>Project Number:</b></td>
                <td>
                    <asp:DropDownList ID="ddlProjects" runat="server" AutoPostBack="true" Width="200px" CssClass="form-control"></asp:DropDownList>
                </td>
                <td><b>Billing Period:</b></td>
                <td>
                    <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="form-control" Width="200px">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btnShow" runat="server" Text="Show" OnClick="btnShow_Click" Width="100px" ValidationGroup="Client" CssClass="btn btn-primary" /></td>
            </tr>
            <tr>
            </tr>
        </table>
    </div>
   
        <asp:Label ID="lblRecords" runat="server" CssClass="form-control" Font-Bold="true"></asp:Label>
    <div class="table-responsive portal-grid-card">
        <table class="table portal-datatable" data-datatable="true">
            <thead><tr><th>Sr. No.</th><th>Project #</th><th>Billing Period</th><th>Type</th><th>Status</th><th>Billing Sent Date</th><th>FTE Details</th><th>Action</th></tr></thead>
            <tbody>
                <asp:Repeater ID="rptBillingDetails" runat="server">
                    <ItemTemplate><tr>
                        <td><%# Container.ItemIndex + 1 %></td><td><%#: Eval("ProjectName") %></td><td><%#: Eval("BillingPeriod") %></td><td><%#: Eval("TYPE") %></td><td><%#: Eval("Status") %></td><td><%#: Eval("AddedDate") %></td>
                        <td><asp:Literal runat="server" Text='<%# GetFteDisplay(Eval("ProjectID"), Eval("ProjectName"), Eval("BillingPeriod"), Eval("TYPE")) %>' /></td>
                        <td><a class="portal-action" href='<%# GetDetailsUrl(Eval("ProjectID"), Eval("BillingPeriod"), Eval("ProjectName")) %>'><i class="fa fa-eye"></i><span>View Details</span></a></td>
                    </tr></ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</asp:Content>
