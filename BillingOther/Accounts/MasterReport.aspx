<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="MasterReport.aspx.cs" Inherits="BillingOther.Accounts.MasterReport" %>
<%@ Register Assembly="DevExpress.Web.v14.2, Version=14.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button ID="btnExport" runat="server" Text="Export To Excel" CssClass="btn btn-primary" OnClick="btnExport_Click" />
    <div style="overflow: auto; margin-top:10px; width:100%;">
        <dx:ASPxGridViewExporter ID="gridExport" OnRenderBrick="gridExport_RenderBrick" runat="server" GridViewID="grdMaster" FileName="All Domain Master Report"></dx:ASPxGridViewExporter>

        <dx:ASPxGridView ID="grdMaster" runat="server" DataSourceID="ds2" Width="100%" Styles-Header-Font-Bold="true" AutoGenerateColumns="false" ClientInstanceName="grid"
            KeyFieldName="PAI_Id" EnableRowsCache="False" Theme="Default" OnCustomUnboundColumnData="grdMaster_CustomUnboundColumnData">
            <Settings ShowFilterRow="true" ShowFilterRowMenu="true" VerticalScrollBarMode="Visible" VerticalScrollableHeight="500" HorizontalScrollBarMode="Auto" />
            <SettingsPager PageSize="10"></SettingsPager>
            <Columns>
                <dx:GridViewBandColumn Caption="Project Details">
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="Number" VisibleIndex="0" Width="40px" Caption="Sr. No" UnboundType="String" ReadOnly="true"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="DomainName" Caption="Domain"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_Project_Name" Caption="Project Number"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_ProcessName" Caption="Sub Project"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ProcessName" Caption="Process"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_Company_Name" Caption="Company Name"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_Contact_Person" Caption="Contact Person"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_Phone_Number" Caption="Phone Number"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_Email_Id" Caption="Email"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_Url" Caption="Website Url"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="PAI_Address" Caption="Address"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="BDMName" Caption="BDM"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ScopeOfProject" Caption="Scope of Project"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_RequestedDate" Caption="Reqeusted Date"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ProjectStatus" Caption="Project Status"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ExpectedVolume" Caption="Expected Volume"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ExpectedStartDate" Caption="Expected Start Date"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ProjectDuration" Caption="Project Duration"></dx:GridViewDataTextColumn>
                    </Columns>
                </dx:GridViewBandColumn>
                <dx:GridViewBandColumn Caption="NDA Details">
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="NDASigned" Caption="NDA Signed?"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_DateOfNDAAgreement" Caption="Agreement Date"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ExpirationDateofNDAAgreement" Caption="Expiration Date"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_NDASignedByClient" Caption="Signed By Client?"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_NDASignedBYInfinity" Caption="Signed By Infinity?"></dx:GridViewDataTextColumn>
                    </Columns>
                </dx:GridViewBandColumn>
                <dx:GridViewBandColumn Caption="MSA Details">
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="SLASigned" Caption="MSA Signed?"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_DateOfSLAAgreement" Caption="Agreement Date"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_ExpirationDateofSLAAgreement" Caption="Expiration Date"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_SLASignedByClient" Caption="Signed By Client?"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IBS_SLASignedByInfinity" Caption="Signed By Infinity?"></dx:GridViewDataTextColumn>
                    </Columns>
                </dx:GridViewBandColumn>
                <dx:GridViewBandColumn Caption="Email Details">
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="CEC_CC" Caption="CC"></dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="CEC_BCC" Caption="BCC"></dx:GridViewDataTextColumn>
                    </Columns>
                </dx:GridViewBandColumn>
            </Columns>
        </dx:ASPxGridView>
         <asp:SqlDataSource ID="ds2" runat="server" SelectCommand="[usp_GetMasterReport]" SelectCommandType="StoredProcedure" ConnectionString="Data Source=23.111.175.186;Initial Catalog=InfinityBilling;Persist Security Info=True;User ID=sa;Password=#Cl0ud^$ecure4; Pooling=true; Min Pool Size=1; Max Pool Size=10; Connect Timeout=200; Packet Size=8192">
        </asp:SqlDataSource>
    </div>
</asp:Content>
