<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="MasterReport.aspx.cs" Inherits="BillingOther.Accounts.MasterReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button ID="btnExport" runat="server" Text="Export To Excel" CssClass="btn btn-primary" OnClick="btnExport_Click" />
    <div style="overflow: auto; margin-top:10px; width:100%;">
        <asp:GridView ID="grdMaster" runat="server" DataSourceID="ds2" Width="100%" AutoGenerateColumns="false" CssClass="table table-striped table-bordered portal-datatable">
            <Columns>
                <asp:TemplateField HeaderText="Sr. No"><ItemTemplate><%# Container.DisplayIndex + 1 %></ItemTemplate></asp:TemplateField>
                <asp:BoundField DataField="DomainName" HeaderText="Domain" /><asp:BoundField DataField="PAI_Project_Name" HeaderText="Project Number" />
                <asp:BoundField DataField="PAI_ProcessName" HeaderText="Sub Project" /><asp:BoundField DataField="IBS_ProcessName" HeaderText="Process" />
                <asp:BoundField DataField="PAI_Company_Name" HeaderText="Company Name" /><asp:BoundField DataField="PAI_Contact_Person" HeaderText="Contact Person" />
                <asp:BoundField DataField="PAI_Phone_Number" HeaderText="Phone Number" /><asp:BoundField DataField="PAI_Email_Id" HeaderText="Email" />
                <asp:BoundField DataField="PAI_Url" HeaderText="Website Url" /><asp:BoundField DataField="PAI_Address" HeaderText="Address" />
                <asp:BoundField DataField="BDMName" HeaderText="BDM" /><asp:BoundField DataField="IBS_ScopeOfProject" HeaderText="Scope of Project" />
                <asp:BoundField DataField="IBS_RequestedDate" HeaderText="Requested Date" /><asp:BoundField DataField="IBS_ProjectStatus" HeaderText="Project Status" />
                <asp:BoundField DataField="IBS_ExpectedVolume" HeaderText="Expected Volume" /><asp:BoundField DataField="IBS_ExpectedStartDate" HeaderText="Expected Start Date" />
                <asp:BoundField DataField="IBS_ProjectDuration" HeaderText="Project Duration" /><asp:BoundField DataField="NDASigned" HeaderText="NDA Signed?" />
                <asp:BoundField DataField="IBS_DateOfNDAAgreement" HeaderText="NDA Agreement Date" /><asp:BoundField DataField="IBS_ExpirationDateofNDAAgreement" HeaderText="NDA Expiration Date" />
                <asp:BoundField DataField="IBS_NDASignedByClient" HeaderText="NDA Signed By Client?" /><asp:BoundField DataField="IBS_NDASignedBYInfinity" HeaderText="NDA Signed By Infinity?" />
                <asp:BoundField DataField="SLASigned" HeaderText="MSA Signed?" /><asp:BoundField DataField="IBS_DateOfSLAAgreement" HeaderText="MSA Agreement Date" />
                <asp:BoundField DataField="IBS_ExpirationDateofSLAAgreement" HeaderText="MSA Expiration Date" /><asp:BoundField DataField="IBS_SLASignedByClient" HeaderText="MSA Signed By Client?" />
                <asp:BoundField DataField="IBS_SLASignedByInfinity" HeaderText="MSA Signed By Infinity?" /><asp:BoundField DataField="CEC_CC" HeaderText="CC" /><asp:BoundField DataField="CEC_BCC" HeaderText="BCC" />
            </Columns>
        </asp:GridView>
         <asp:SqlDataSource ID="ds2" runat="server" SelectCommand="[usp_GetMasterReport]" SelectCommandType="StoredProcedure" ConnectionString="Data Source=23.111.175.186;Initial Catalog=InfinityBilling;Persist Security Info=True;User ID=sa;Password=#Cl0ud^$ecure4; Pooling=true; Min Pool Size=1; Max Pool Size=10; Connect Timeout=200; Packet Size=8192">
        </asp:SqlDataSource>
    </div>
</asp:Content>
