<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ProductMaster.aspx.cs" Inherits="BillingOther.Accounts.ProductMaster" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server"></div>
    <div>
        <table class="table table-border-style">
            <tr>
                <td><b>Project Number</b></td>
                <td>
                    <asp:DropDownList ID="ddlProjects" runat="server" AutoPostBack="true"  CssClass="form-control" Width="200px"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" SetFocusOnError="true" runat="server"
                        ValidationGroup="Client" ControlToValidate="ddlProjects" InitialValue="0" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Please Select Project"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td><b>Product Type</b></td>
                <td>
                    <asp:TextBox runat="server" ID="txtProductType" CssClass="form-control" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator12" SetFocusOnError="true" runat="server"
                        ValidationGroup="Client" ControlToValidate="txtProductType" Display="Dynamic" ForeColor="Red"
                        ErrorMessage="Please enter Product type"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" ValidationGroup="Client" OnClick="btnSave_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div id="costgriddiv" runat="server" class="table-responsive portal-grid-card">
        <table class="table portal-datatable" data-datatable="true">
            <thead><tr><th>Sr.#</th><th>Project #</th><th>Product Type</th><th>Added By</th><th>Added Date</th><th>Action</th></tr></thead>
            <tbody><asp:Repeater ID="rptProducts" runat="server"><ItemTemplate><tr><td><%# Container.ItemIndex + 1 %></td><td><%#: Eval("Project_Name") %></td><td><%#: Eval("ProductType") %></td><td><%#: Eval("Added_By") %></td><td><%#: Eval("AddedDate") %></td><td><a class="portal-action" href='<%# ResolveUrl("~/Accounts/ProductMaster.aspx?Edit=" + Eval("IPM_Id")) %>'><i class="fa fa-pencil"></i><span>Edit</span></a></td></tr></ItemTemplate></asp:Repeater></tbody>
        </table>
    </div>
</asp:Content>
