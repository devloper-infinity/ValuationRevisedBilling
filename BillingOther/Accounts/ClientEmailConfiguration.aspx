<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ClientEmailConfiguration.aspx.cs" Inherits="BillingOther.Accounts.ClientEmailConfiguration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server"></div>
    <table class="table table-border-style">
            <tr>
                <td>Project Number:</td>
                <td>
                    <asp:DropDownList ID="ddlProjects" runat="server" AutoPostBack="true" Width="200px" CssClass="form-control" OnSelectedIndexChanged="ddlProjects_SelectedIndexChanged"></asp:DropDownList>
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator2" SetFocusOnError="true" runat="server"
                        ValidationGroup="user" ControlToValidate="ddlProjects" InitialValue="0" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Please Select Project"></asp:RequiredFieldValidator> </td>
            </tr>
                <tr>
                    <td>Client:</td>
                    <td>

                        <asp:DropDownList runat="server" ID="ddlClientList" Width="200px" CssClass="form-control"></asp:DropDownList>
                          <asp:RequiredFieldValidator ID="RequiredFieldValidator3" SetFocusOnError="true" runat="server"
                        ValidationGroup="user" ControlToValidate="ddlClientList" InitialValue="0" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Please Select Client Name"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            <tr>
                <td align="top">To:</td>
                <td>
                    <asp:TextBox ID="txtTo" runat="server" Width="400px" Height="40px" CssClass="form-control" TextMode="MultiLine" Style="resize: none;"></asp:TextBox>
                    <a style="font: 16px; color: red;">Note: Use comma( , ) for multiple EmailID</a>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTo"
                        ErrorMessage="Please enter To." Font-Size="12px" Display="Dynamic" Style="color: Red;"
                        vertical-align="top" SetFocusOnError="true" ValidationGroup="user"></asp:RequiredFieldValidator>
                </td>
            </tr>
             <tr>
                <td align="top">CC:</td>
                <td>
                    <asp:TextBox ID="txtCC" runat="server" Width="400px" Height="40px" CssClass="form-control" TextMode="MultiLine" Style="resize: none;"></asp:TextBox>
                    <a style="font: 16px; color: red;">Note: Use comma( , ) for multiple EmailID</a>

                </td>
            </tr>
            <tr>
                <td align="top">BCC:</td>
                <td>
                    <asp:TextBox ID="txtBCC" runat="server" Width="400px" Height="40px" CssClass="form-control" TextMode="MultiLine"  Style="resize: none;"></asp:TextBox><br />
                    <a style="font: 16px; color: red;">Note: Use comma( , ) for multiple EmailID</a>
                  
                </td>

            </tr>
       
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnSend" runat="server" Text="Save" CssClass="btn btn-primary" ValidationGroup="user" OnClick="btnSend_Click"/>
                </td>
            </tr>
        </table>
        <div id="costgriddiv" runat="server" class="table-responsive portal-grid-card">
            <table class="table portal-datatable" data-datatable="true">
                <thead><tr><th>Sr.#</th><th>Project #</th><th>Client Name</th><th>TO Address</th><th>CC Address</th><th>BCC Address</th><th>Added By</th><th>Added Date</th><th>Action</th></tr></thead>
                <tbody><asp:Repeater ID="rptEmailConfigurations" runat="server"><ItemTemplate><tr><td><%# Container.ItemIndex + 1 %></td><td><%#: Eval("Project_Name") %></td><td><%#: Eval("ClientName") %></td><td><%#: Eval("CEC_To") %></td><td><%#: Eval("CEC_CC") %></td><td><%#: Eval("CEC_BCC") %></td><td><%#: Eval("Added_By") %></td><td><%#: Eval("AddedDate") %></td><td><a class="portal-action" href='<%# ResolveUrl("~/Accounts/ClientEmailConfiguration.aspx?Edit=" + Eval("CEC_Id")) %>'><i class="fa fa-pencil"></i><span>Edit</span></a></td></tr></ItemTemplate></asp:Repeater></tbody>
            </table>
        </div>
</asp:Content>
