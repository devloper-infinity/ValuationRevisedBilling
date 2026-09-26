<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ProjectSummaryReport.aspx.cs" Inherits="BillingOther.Accounts.ProjectSummaryReport" %>

<%@ Register Assembly="DevExpress.Web.v14.2, Version=14.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Data.v14.2, Version=14.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Data" TagPrefix="dx" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server"></div>
    <div>


        <table class="table table-responsive">

            <tr>
                <td width="100px"><b>From Date :</b></td>
                <td width="120px" style="text-align: right;">
                    <asp:TextBox ID="txtSummaryFromDate" runat="server" placeholder="dd-MMM-yyyy" Width="120px"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender5" runat="server" TargetControlID="txtSummaryFromDate" Format="dd-MMM-yyyy"></asp:CalendarExtender>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSummaryFromDate" SetFocusOnError="true" ErrorMessage="Please Enter From Date." ForeColor="Red" Display="None" ValidationGroup="woman"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ValidationGroup="woman" ID="RegularExpressionValidator2" ValidationExpression="^(([0-9])|([0-2][0-9])|([3][0-1]))\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\-\d{4}$" ControlToValidate="txtSummaryFromDate" ForeColor="Red" runat="server" Display="Dynamic" SetFocusOnError="true" ErrorMessage="Insert Valid From Date"></asp:RegularExpressionValidator>
                </td>
                <td width="80px" style="text-align: right;"><b>To Date :</b></td>
                <td width="120px">
                    <asp:TextBox ID="txtSummaryToDate" runat="server" placeholder="dd-MMM-yyyy" Width="120px"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender6" runat="server" TargetControlID="txtSummaryToDate" Format="dd-MMM-yyyy"></asp:CalendarExtender>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtSummaryToDate" SetFocusOnError="true" ErrorMessage="Please Enter To Date." ForeColor="Red" Display="None" ValidationGroup="woman"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ValidationGroup="woman" ID="RegularExpressionValidator3" ValidationExpression="^(([0-9])|([0-2][0-9])|([3][0-1]))\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\-\d{4}$" ControlToValidate="txtSummaryToDate" ForeColor="Red" runat="server" Display="Dynamic" SetFocusOnError="true" ErrorMessage="Insert Valid To Date"></asp:RegularExpressionValidator>
                </td>

                <td>
                    <asp:Button ID="btnShowFilter" runat="server" Text="Show" OnClick="btnShowFilter_Click" CssClass="btn btn-primary" />
                </td>
                <td>
                    <asp:Button ID="btnExport" runat="server" Text="Export To Excel" OnClick="btnExport_Click" CssClass="btn btn-primary" />
                </td>


            </tr>


        </table>

            <dx:ASPxGridViewExporter runat="server" ID="grdExport" GridViewID="grdSummaryDetails" FileName="Billing Summary Report"></dx:ASPxGridViewExporter>

        <dx:ASPxGridView ID="grdSummaryDetails" runat="server" AutoGenerateColumns="false" ClientInstanceName="grid" Theme="Default" OnCustomUnboundColumnData="grdSummaryDetails_CustomUnboundColumnData">
            <SettingsPager Mode="ShowAllRecords">
            </SettingsPager>
            <Columns>
                <dx:GridViewDataTextColumn FieldName="Number" VisibleIndex="0" Width="40px" Caption="Sr. No." UnboundType="String" ReadOnly="True" ShowInCustomizationForm="True">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                    <CellStyle HorizontalAlign="Center"></CellStyle>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Domain" Caption="Domain" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="120px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Subdomain" Caption="Sub Domain" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="120px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="OrderType" Caption="Product Type" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="120px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="DealNo" Caption="Project #" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Received" Caption="Received" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="TotalReceived" Caption="Total Cost based on Received" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Dispatched" Caption="Dispatch" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="TotalDispatched" Caption="Total Cost based on Dispatch" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>
                <%--<dx:GridViewDataTextColumn FieldName="On Hold" Caption="On-Hold" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Cancelled" Caption="Cancelled" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="Pending" Caption="In-Process" CellStyle-HorizontalAlign="Center" CellStyle-ForeColor="Black" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Bold="true" Width="95px"></dx:GridViewDataTextColumn>--%>




            </Columns>
        </dx:ASPxGridView>





    </div>
</asp:Content>
