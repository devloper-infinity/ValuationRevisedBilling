<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="DeviationReport.aspx.cs" Inherits="BillingOther.Accounts.DeviationReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="CC12" %>
<%@ Register Assembly="DevExpress.Web.v14.2, Version=14.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
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
    <div style="overflow: auto;">
        <center>
            <div id="dvError" runat="server" style="display: none; padding-bottom: 20px;">
                <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
            </div>
        </center>
        <br />
        <div>
            <table class="table-condensed">
                <tr>
                    <td style="font-weight: bold;">Previous Month :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPreviousMonth" Height="24px" runat="server" ValidationGroup="user" Width="150px">
                            <asp:ListItem Value="Select">Select</asp:ListItem>
                            <asp:ListItem Value="January">January</asp:ListItem>
                            <asp:ListItem Value="February">February</asp:ListItem>
                            <asp:ListItem Value="March">March</asp:ListItem>
                            <asp:ListItem Value="April">April</asp:ListItem>
                            <asp:ListItem Value="May">May</asp:ListItem>
                            <asp:ListItem Value="June">June</asp:ListItem>
                            <asp:ListItem Value="July">July</asp:ListItem>
                            <asp:ListItem Value="August">August</asp:ListItem>
                            <asp:ListItem Value="September">September</asp:ListItem>
                            <asp:ListItem Value="October">October</asp:ListItem>
                            <asp:ListItem Value="November">November</asp:ListItem>
                            <asp:ListItem Value="December">December</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorMonth" ErrorMessage="Please select month." Display="None" Style="color: Red;"
                            ControlToValidate="ddlPreviousMonth" runat="server" InitialValue="Select" Font-Size="12px" ValidationGroup="user"></asp:RequiredFieldValidator>
                    </td>
                    <td></td>

                    <td style="font-weight: bold;">Previous Year:</td>
                    <td>
                        <asp:DropDownList ID="ddlPreviousYear" Height="24px" runat="server" ValidationGroup="user" Width="150px">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorYear" ErrorMessage="Please select year." Display="None" Style="color: Red;"
                            ControlToValidate="ddlPreviousYear" runat="server" InitialValue="Select" Font-Size="12px" ValidationGroup="user"></asp:RequiredFieldValidator>
                    </td>
                    <td style="font-weight: bold;">Current Month :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCurrentMonth" Height="24px" runat="server" ValidationGroup="user" Width="150px">
                            <asp:ListItem Value="Select">Select</asp:ListItem>
                            <asp:ListItem Value="January">January</asp:ListItem>
                            <asp:ListItem Value="February">February</asp:ListItem>
                            <asp:ListItem Value="March">March</asp:ListItem>
                            <asp:ListItem Value="April">April</asp:ListItem>
                            <asp:ListItem Value="May">May</asp:ListItem>
                            <asp:ListItem Value="June">June</asp:ListItem>
                            <asp:ListItem Value="July">July</asp:ListItem>
                            <asp:ListItem Value="August">August</asp:ListItem>
                            <asp:ListItem Value="September">September</asp:ListItem>
                            <asp:ListItem Value="October">October</asp:ListItem>
                            <asp:ListItem Value="November">November</asp:ListItem>
                            <asp:ListItem Value="December">December</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ErrorMessage="Please select month." Display="None" Style="color: Red;"
                            ControlToValidate="ddlCurrentMonth" runat="server" InitialValue="Select" Font-Size="12px" ValidationGroup="user"></asp:RequiredFieldValidator>
                    </td>
                    <td></td>

                    <td style="font-weight: bold;">Current Year:</td>
                    <td>
                        <asp:DropDownList ID="ddlCurrentYear" Height="24px" runat="server" ValidationGroup="user" Width="150px">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ErrorMessage="Please select year." Display="None" Style="color: Red;"
                            ControlToValidate="ddlCurrentYear" runat="server" InitialValue="Select" Font-Size="12px" ValidationGroup="user"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <asp:Button ID="btnShow" runat="server" Text="Show" OnClick="btnShow_Click" ValidationGroup="user" Font-Bold="true" Font-Size="Larger" Width="90px" Height="27px" />
                        <asp:ValidationSummary ID="vsRecord" runat="server" ValidationGroup="user" ShowMessageBox="true" ShowSummary="false" />
                    </td>
                    <td>
                        <asp:Button ID="btnExpoetToExcel" runat="server" Text="Export To Excel" OnClick="btnExpoetToExcel_Click" ValidationGroup="user" Font-Bold="true" Font-Size="Larger" Height="27px" />
                    </td>
                </tr>

            </table>
        </div>

        <div style="clear: both"></div>
        <br />
        <div style="width=95%; overflow:auto;">
            <asp:Label ID="lblPrevious" runat="server" CssClass="form-control" Font-Bold="true"></asp:Label>
            <dx:ASPxGridViewExporter ID="grdPreviousExport" runat="server" GridViewID="grdPreviousDomain"></dx:ASPxGridViewExporter>
            <dx:ASPxGridView ID="grdPreviousDomain" runat="server" SettingsPager-PageSize="20" AutoGenerateColumns="false" Theme="Office2010Silver" Styles-Footer-BackColor="PowderBlue" OnCustomUnboundColumnData="grdPreviousDomain_CustomUnboundColumnData">
                <Settings ShowFilterBar="Auto" ShowFilterRow="true" />
                <Styles Header-Wrap="True"></Styles>
                <Styles>
                    <CommandColumn BorderBottom-BorderStyle="Ridge" Border-BorderColor="Gray"></CommandColumn>
                </Styles>
                <Columns>
                    <dx:GridViewDataTextColumn FieldName="Number" Caption="Sr. No." Width="30px" VisibleIndex="0" ReadOnly="true" UnboundType="String" FixedStyle="Left" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="Domain" Caption="Domain" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Subdomain" Caption="Subdomain" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="RecordsBilled" Caption="Records Billed" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="AmountBilled" Caption="Billing US$" />
                    <dx:GridViewDataTextColumn FieldName="Production" Caption="Production" CellStyle-HorizontalAlign="Center" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Marketing" Caption="Marketing" CellStyle-HorizontalAlign="Center" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Total" Caption="Total" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Total %" Caption="Total %" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Support Salary" Caption="Support Salary" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="MSEB" Caption="MSEB" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Internet" Caption="Internet" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Rent" Caption="Rent" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="General Expenses" Caption="Adminstration & General Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Repair And Maintenance" Caption="Repair And Maintance" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Depreciation" Caption="Depreciation" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="VendorCost" Caption="Vendor Cost" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="TotalCost" Caption="Total Cost (Support + Salary)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Payroll" Caption="US Payroll" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Payroll Allocation" Caption="US Payroll Allocation" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Indep - Contr (review/abst plant + Scien+CE + aci/alamo Apprisers" Caption="Indep - Contr (review/abst plant + Scien+CE + aci/alamo Apprisers)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Total Indirect Exp (rent, ins, inern, prof fees, T&E, conf, gifts)" Caption="US Total Indirect Exp (rent, ins, inern, prof fees, T&E, conf, gifts)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Marketing Expenses" Caption="Marketing Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Total of US Expenses" Caption="Total of US Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="IndiaCost" Caption="IndiaCost" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Conversion of India cost In US$ @75" Caption="Conversion of India cost In US$ @75" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="India & USA Total Cost in US$" Caption="India & USA Total Cost in US$" CellStyle-HorizontalAlign="Center" />
                </Columns>
                <Settings ShowFooter="True" GridLines="Both" />
                <Styles>
                    <Footer Border-BorderColor="LightGray" Border-BorderWidth="1"></Footer>
                </Styles>

            </dx:ASPxGridView>
        </div>
        <div style="width=95%; overflow:auto;">
            <asp:Label ID="lblCurrent" runat="server" CssClass="form-control" Font-Bold="true"></asp:Label>
            <dx:ASPxGridViewExporter ID="grdCurrentExport" runat="server" GridViewID="grdCurrentDomain"></dx:ASPxGridViewExporter>
            <dx:ASPxGridView ID="grdCurrentDomain" runat="server" SettingsPager-PageSize="20" AutoGenerateColumns="false" Theme="Office2010Silver" Styles-Footer-BackColor="PowderBlue" OnCustomUnboundColumnData="grdCurrentDomain_CustomUnboundColumnData">
                <Settings ShowFilterBar="Auto" ShowFilterRow="true" />
                <Styles Header-Wrap="True"></Styles>
                <Styles>
                    <CommandColumn BorderBottom-BorderStyle="Ridge" Border-BorderColor="Gray"></CommandColumn>
                </Styles>
                <Columns>
                    <dx:GridViewDataTextColumn FieldName="Number" Caption="Sr. No." Width="30px" VisibleIndex="0" ReadOnly="true" UnboundType="String" FixedStyle="Left" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="Domain" Caption="Domain" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Subdomain" Caption="Subdomain" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="RecordsBilled" Caption="Records Billed" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="AmountBilled" Caption="Billing US$" />
                    <dx:GridViewDataTextColumn FieldName="Production" Caption="Production" CellStyle-HorizontalAlign="Center" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Marketing" Caption="Marketing" CellStyle-HorizontalAlign="Center" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Total" Caption="Total" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Total %" Caption="Total %" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Support Salary" Caption="Support Salary" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="MSEB" Caption="MSEB" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Internet" Caption="Internet" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Rent" Caption="Rent" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="General Expenses" Caption="Adminstration & General Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Repair And Maintenance" Caption="Repair And Maintance" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Depreciation" Caption="Depreciation" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="VendorCost" Caption="Vendor Cost" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="TotalCost" Caption="Total Cost (Support + Salary)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Payroll" Caption="US Payroll" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Payroll Allocation" Caption="US Payroll Allocation" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Indep - Contr (review/abst plant + Scien+CE + aci/alamo Apprisers" Caption="Indep - Contr (review/abst plant + Scien+CE + aci/alamo Apprisers)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Total Indirect Exp (rent, ins, inern, prof fees, T&E, conf, gifts)" Caption="US Total Indirect Exp (rent, ins, inern, prof fees, T&E, conf, gifts)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Marketing Expenses" Caption="Marketing Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Total of US Expenses" Caption="Total of US Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="IndiaCost" Caption="IndiaCost" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Conversion of India cost In US$ @75" Caption="Conversion of India cost In US$ @75" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="India & USA Total Cost in US$" Caption="India & USA Total Cost in US$" CellStyle-HorizontalAlign="Center" />
                </Columns>
                <Settings ShowFooter="True" GridLines="Both" />
                <Styles>
                    <Footer Border-BorderColor="LightGray" Border-BorderWidth="1"></Footer>
                </Styles>

            </dx:ASPxGridView>
        </div>
         <div style="width=95%; overflow:auto;">
            <asp:Label ID="Label1" runat="server" CssClass="form-control" Font-Bold="true" Text="Difference Table"></asp:Label>
            <dx:ASPxGridViewExporter ID="grdDifferenceExport" runat="server" GridViewID="grdDifference"></dx:ASPxGridViewExporter>
            <dx:ASPxGridView ID="grdDifference" runat="server" SettingsPager-PageSize="20" AutoGenerateColumns="false" Theme="Office2010Silver" Styles-Footer-BackColor="PowderBlue" OnCustomUnboundColumnData="grdCurrentDomain_CustomUnboundColumnData">
                <Settings ShowFilterBar="Auto" ShowFilterRow="true" />
                <Styles Header-Wrap="True"></Styles>
                <Styles>
                    <CommandColumn BorderBottom-BorderStyle="Ridge" Border-BorderColor="Gray"></CommandColumn>
                </Styles>
                <Columns>
                    <dx:GridViewDataTextColumn FieldName="Number" Caption="Sr. No." Width="30px" VisibleIndex="0" ReadOnly="true" UnboundType="String" FixedStyle="Left" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="Domain" Caption="Domain" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Subdomain" Caption="Subdomain" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="RecordsBilled" Caption="Records Billed" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="AmountBilled" Caption="Billing US$" />
                    <dx:GridViewDataTextColumn FieldName="Production" Caption="Production" CellStyle-HorizontalAlign="Center" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Marketing" Caption="Marketing" CellStyle-HorizontalAlign="Center" FixedStyle="Left" />
                    <dx:GridViewDataTextColumn FieldName="Total" Caption="Total" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Total %" Caption="Total %" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Support Salary" Caption="Support Salary" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="MSEB" Caption="MSEB" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Internet" Caption="Internet" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Rent" Caption="Rent" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="General Expenses" Caption="Adminstration & General Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Repair And Maintenance" Caption="Repair And Maintance" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Depreciation" Caption="Depreciation" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="VendorCost" Caption="Vendor Cost" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="TotalCost" Caption="Total Cost (Support + Salary)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Payroll" Caption="US Payroll" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Payroll Allocation" Caption="US Payroll Allocation" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Indep - Contr (review/abst plant + Scien+CE + aci/alamo Apprisers" Caption="Indep - Contr (review/abst plant + Scien+CE + aci/alamo Apprisers)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="US Total Indirect Exp (rent, ins, inern, prof fees, T&E, conf, gifts)" Caption="US Total Indirect Exp (rent, ins, inern, prof fees, T&E, conf, gifts)" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Marketing Expenses" Caption="Marketing Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Total of US Expenses" Caption="Total of US Expenses" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="IndiaCost" Caption="IndiaCost" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="Conversion of India cost In US$ @75" Caption="Conversion of India cost In US$ @75" CellStyle-HorizontalAlign="Center" />
                    <dx:GridViewDataTextColumn FieldName="India & USA Total Cost in US$" Caption="India & USA Total Cost in US$" CellStyle-HorizontalAlign="Center" />
                </Columns>
                <Settings ShowFooter="True" GridLines="Both" />
                <Styles>
                    <Footer Border-BorderColor="LightGray" Border-BorderWidth="1"></Footer>
                </Styles>

            </dx:ASPxGridView>
        </div>
    </div>
</asp:Content>
