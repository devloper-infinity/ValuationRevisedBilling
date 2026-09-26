<%@ Page Title="" Language="C#" MasterPageFile="~/BDM/BDM.Master" AutoEventWireup="true" CodeBehind="CostPerRecord.aspx.cs" Inherits="BillingOther.BDM.CostPerRecord" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="CC12" %>
<%@ Register Assembly="DevExpress.Web.v14.2, Version=14.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>

    <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/js/bootstrap.min.js"></script>
    <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />

    <script src="../bootstrap/js/bootstrap-multiselect.js" type="text/javascript"></script>
    <script type="text/javascript"></script>
    <link href="../bootstrap/css/TabContainer.css" rel="stylesheet" />

    <script type="text/javascript">
        function HideLabel() {
            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%=dvError.ClientID %>").style.display = "none";
            }, seconds * 1000);
        };


        function OnEndCallbackVendorCost() {

            if (gridVendor.cp_message) {
                if (gridVendor.cp_message == "1") {

                    document.getElementById("<%= dvError.ClientID%>").style.display = "";
                    document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record updated successfully.";
                    document.getElementById("<%= lblError.ClientID%>").style.color = "Green";
                    gridVendor.cp_message = "";
                }
                else if (gridVendor.cp_message == "0") {
                    document.getElementById("<%= dvError.ClientID%>").style.display = "";
                    document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record already exists!";
                    document.getElementById("<%= lblError.ClientID%>").style.color = "Red";
                    gridVendor.cp_message = "";
                }
                else if (gridVendor.cp_message == "3") {
                    document.getElementById("<%= dvError.ClientID%>").style.display = "";
                    document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record Deleted Successfully.";
                    document.getElementById("<%= lblError.ClientID%>").style.color = "Green";
                    gridVendor.cp_message = "";
                }
                else if (gridVendor.cp_message == "4") {
                    {
                        document.getElementById("<%= dvError.ClientID%>").style.display = "";
                        document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record name can't be blank.";
                        document.getElementById("<%= lblError.ClientID%>").style.color = "Green";
                        gridVendor.cp_message = "";
                    }

                }
                else {
                    document.getElementById("<%= dvError.ClientID%>").style.display = "none";
                    gridVendor.cp_message = "";
                }
                HideLabel();
            }
            else {
                document.getElementById("<%= dvError.ClientID%>").style.display = "none";
                grid.cp_message = "";
            }
        }

        function OnEndCallbackOtherCost() {

            if (gridOther.cp_message) {
                if (gridOther.cp_message == "1") {

                    document.getElementById("<%= dvError.ClientID%>").style.display = "";
                    document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record updated successfully.";
                    document.getElementById("<%= lblError.ClientID%>").style.color = "Green";
                    gridOther.cp_message = "";
                }
                else if (gridOther.cp_message == "0") {
                    document.getElementById("<%= dvError.ClientID%>").style.display = "";
                    document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record already exists!";
                    document.getElementById("<%= lblError.ClientID%>").style.color = "Red";
                    gridOther.cp_message = "";
                }
                else if (gridOther.cp_message == "3") {
                    document.getElementById("<%= dvError.ClientID%>").style.display = "";
                    document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record Deleted Successfully.";
                    document.getElementById("<%= lblError.ClientID%>").style.color = "Green";
                    gridOther.cp_message = "";
                }
                else if (gridOther.cp_message == "4") {
                    {
                        document.getElementById("<%= dvError.ClientID%>").style.display = "";
                        document.getElementById("<%= lblError.ClientID%>").innerHTML = "Record name can't be blank.";
                        document.getElementById("<%= lblError.ClientID%>").style.color = "Green";
                        gridOther.cp_message = "";
                    }

                }
                else {
                    document.getElementById("<%= dvError.ClientID%>").style.display = "none";
                    gridOther.cp_message = "";
                }
                HideLabel();
            }
            else {
                document.getElementById("<%= dvError.ClientID%>").style.display = "none";
                gridOther.cp_message = "";
            }
        }
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
                    <td style="font-weight: bold;">Month :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMonth" Height="24px" runat="server" ValidationGroup="user" Width="150px">
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
                            ControlToValidate="ddlMonth" runat="server" InitialValue="Select" Font-Size="12px" ValidationGroup="user"></asp:RequiredFieldValidator>
                    </td>
                    <td></td>

                    <td style="font-weight: bold;">Year:</td>
                    <td>
                        <asp:DropDownList ID="ddlYear" Height="24px" runat="server" ValidationGroup="user" Width="150px">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorYear" ErrorMessage="Please select year." Display="None" Style="color: Red;"
                            ControlToValidate="ddlYear" runat="server" InitialValue="Select" Font-Size="12px" ValidationGroup="user"></asp:RequiredFieldValidator>
                    </td>
                    <td></td>
                    <td>
                        <asp:Button ID="btnShow" runat="server" Text="Show" OnClick="btnShow_Click" ValidationGroup="user" Font-Bold="true" Font-Size="Larger" Width="90px" Height="27px" />
                        <asp:ValidationSummary ID="vsRecord" runat="server" ValidationGroup="user" ShowMessageBox="true" ShowSummary="false" />
                    </td>
                    <td>
                        <asp:Button ID="btnExpoetToExcel" runat="server" Text="Export To Excel" OnClick="btnExpoetToExcel_Click" ValidationGroup="user" Font-Bold="true" Font-Size="Larger" Height="27px" />
                    </td>
                </tr>
                <tr style="display:none;">
                    <td colspan="2"><b>Support Employee Count :</b> &nbsp;
                        <asp:Label ID="lblSupport" runat="server" Style="font-weight: bold;"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div style="clear: both"></div>
        <br />
        <div class="card-content">
            <div class="card-body">
                <ul class="nav nav-tabs " role="tablist">
                    <li class="nav-item" id="tbpnl1" runat="server" style="display:none;">
                        <a class="nav-link active" data-toggle="tab" href="#tab1" role="tab"><i class="fa fa-user"></i>&nbsp;&nbsp;Userwise</a>
                        <div class="slide"></div>
                    </li>
                    <li class="nav-item" id="tbpnl2" runat="server" style="display:none;">
                        <a class="nav-link" data-toggle="tab" href="#tab2" role="tab"><i class="fa fa-envelope"></i>&nbsp;&nbsp;Domainwise</a>
                        <div class="slide"></div>
                    </li>
                    <li class="nav-item" id="tbpnl3" runat="server" style="display:none;">
                        <a class="nav-link" data-toggle="tab" href="#tab3" role="tab"><i class="fa fa-paragraph"></i>&nbsp;&nbsp;Projectwise</a>
                        <div class="slide"></div>
                    </li>
                    <li class="nav-item active" id="tbpnlFTE" runat="server">
                        <a class="nav-link" data-toggle="tab" href="#tab4" role="tab"><i class="fa fa-cog"></i>&nbsp;&nbsp;Cost Master</a>
                        <div class="slide"></div>
                    </li>

                </ul>
                <div class="tab-content card-block" style="border: solid 1px #6c757d; margin-left: 5px;">
                    <div class="tab-pane" id="tab1" role="tabpanel" style="display:none;">
                        <div style="overflow: auto;">
                            <dx:ASPxGridViewExporter ID="grdUserExport" runat="server" FileName="User Wise Cost Per Report" GridViewID="grdUser" OnRenderBrick="grdUserExport_RenderBrick"></dx:ASPxGridViewExporter>
                            <dx:ASPxGridView ID="grdUser" runat="server" SettingsPager-PageSize="10" AutoGenerateColumns="false" KeyFieldName="Code" Theme="Office2010Silver" OnCustomUnboundColumnData="grdUser_CustomUnboundColumnData1" OnCustomButtonInitialize="grdUser_CustomButtonInitialize" Width="1565px" OnHtmlRowPrepared="grdUser_HtmlRowPrepared">
                                <Settings ShowFilterBar="Auto" ShowFilterRow="true" />

                                <Columns>
                                    <dx:GridViewDataTextColumn FieldName="Number" Caption="Sr. No." Width="50px" VisibleIndex="0" ReadOnly="true" UnboundType="String" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Code" Caption="Code" Name="DisplayCode" Visible="false" />
                                    <dx:GridViewDataColumn FieldName="Code" Caption="Code" VisibleIndex="0" Width="70px" Name="Code" CellStyle-HorizontalAlign="Center"></dx:GridViewDataColumn>
                                    <dx:GridViewDataTextColumn FieldName="Name" Caption="Name" Width="250px" />
                                    <dx:GridViewDataTextColumn FieldName="BranchName" Caption="Branch" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="DepartmentName" Caption="Department" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Domain" Caption="Domain" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="SubDomain" Caption="Subdomain" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Project" Caption="Project" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Process" Caption="Process" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Production" Caption="Order/ Loan Count" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Target" Caption="Target" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Productivity%" Caption="Productivity %" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Ratio" Caption="Ratio" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="GrossSalary" Caption="Gross Salary" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="DueSalary" Caption="Gross Pay" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Incentive" Caption="Incentive" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="ESI" Caption="ESI @3.75%" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="PF" Caption="PF @12%" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="LeaveEncashment" Caption="Leave Encashment" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Incentive" Caption="Incentive" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="TotalSalary" Caption="Total" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="NetOutGoing" Caption="NetOutGoing" CellStyle-HorizontalAlign="Center" />
                                </Columns>
                            </dx:ASPxGridView>
                        </div>
                    </div>
                    <div class="tab-pane" id="tab2" role="tabpanel" style="display:none;">
                        <div style="overflow: auto;">
                            <dx:ASPxGridViewExporter ID="grdDomainExport" runat="server" GridViewID="grdReport"></dx:ASPxGridViewExporter>
                            <dx:ASPxGridView ID="grdReport" runat="server" SettingsPager-PageSize="20" AutoGenerateColumns="false" Theme="Office2010Silver" Styles-Footer-BackColor="PowderBlue" OnCustomUnboundColumnData="grdReport_CustomUnboundColumnData">
                                <Settings ShowFilterBar="Auto" ShowFilterRow="true" />
                                <Styles Header-Wrap="True"></Styles>
                                <Styles>
                                    <CommandColumn BorderBottom-BorderStyle="Ridge" Border-BorderColor="Gray"></CommandColumn>
                                </Styles>
                                <Columns>
                                    <dx:GridViewDataTextColumn FieldName="Number" Caption="Sr. No." Width="30px" VisibleIndex="0" ReadOnly="true" UnboundType="String" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Domain" Caption="Domain" />
                                    <dx:GridViewDataTextColumn FieldName="Subdomain" Caption="Subdomain" />
                                    <dx:GridViewDataTextColumn FieldName="NoOfEmployees" Caption="No. of employees">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblNoOfEmployees" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblNoOfEmployees_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="RecordsBilled" Caption="Records Billed">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblRecordsBilled" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblRecordsBilled_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="AmountBilled" Caption="Billing US$">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblAmountBilled" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblAmountBilled_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Production" Caption="Production">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblProduction" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblProduction_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Marketing" Caption="Marketing">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblMarketing" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblMarketing_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Total" Caption="Total">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblTotal" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblTotal_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Total %" Caption="Total %" CellStyle-HorizontalAlign="Center" />
                                   
                                    <dx:GridViewDataTextColumn FieldName="Support Salary" Caption="Support Salary">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblSupportSalary" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblSupportSalary_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="MSEB" Caption="MSEB">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblMSEB" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblMSEB_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Internet" Caption="Internet">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblInternet" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblInternet_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Rent" Caption="Rent">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblRent" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblRent_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="General Expenses" Caption="General Expenses">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblGeneralExpenses" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblGeneralExpenses_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Repair And Maintenance" Caption="Repair And Maintenance">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblRepairAndMaintenance" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblRepairAndMaintenance_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Depreciation" Caption="Depreciation">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblDepreciation" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblDepreciation_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="VendorCost" Caption="VendorCost">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblVendorCost" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblVendorCost_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Contract Cost" Caption="Contract Cost">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblContractCost" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblContractCost_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="TotalCost" Caption="TotalCost">
                                        <HeaderCaptionTemplate>
                                            <dx:ASPxLabel ID="lblTotalCost" Font-Bold="true" Font-Names="Courier New" runat="server" OnLoad="lblTotalCost_Load"></dx:ASPxLabel>
                                        </HeaderCaptionTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True"></HeaderStyle>
                                    </dx:GridViewDataTextColumn>
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
                                <TotalSummary>
                                    <dx:ASPxSummaryItem FieldName="NoOfEmployees" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="RecordsBilled" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="AmountBilled" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Production" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Marketing" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Total" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Support Salary" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="MSEB" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Internet" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Rent" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="General Expenses" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Repair And Maintenance" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Depreciation" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="VendorCost" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Contract Cost" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="TotalCost" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="US Payroll" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="US Payroll Allocation" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Indep - Contr (review/abst plant + Scien+CE + aci/alamo Apprisers" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="US Total Indirect Exp (rent, ins, inern, prof fees, T&E, conf, gifts)" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Marketing Expenses" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Total of US Expenses" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="IndiaCost" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Conversion of India cost In US$ @75" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="India & USA Total Cost in US$" SummaryType="Sum" DisplayFormat="n2" />
                                </TotalSummary>
                            </dx:ASPxGridView>
                        </div>
                    </div>
                    <div class="tab-pane" id="tab3" role="tabpanel" style="display:none;">
                        <div style="overflow: auto">
                            <dx:ASPxGridViewExporter ID="grdProjectExport" runat="server" GridViewID="grdProjectReport"></dx:ASPxGridViewExporter>
                            <dx:ASPxGridView ID="grdProjectReport" runat="server" SettingsPager-PageSize="20" AutoGenerateColumns="false" Theme="Office2010Silver" Styles-Footer-BackColor="PowderBlue" OnCustomUnboundColumnData="grdReport_CustomUnboundColumnData">
                                <Settings ShowFilterBar="Auto" ShowFilterRow="true" />
                                <Styles Header-Wrap="True"></Styles>
                                <Styles>
                                    <CommandColumn BorderBottom-BorderStyle="Ridge" Border-BorderColor="Gray"></CommandColumn>
                                </Styles>
                                <Columns>
                                    <dx:GridViewDataTextColumn FieldName="Number" Caption="Sr. No." Width="30px" VisibleIndex="0" ReadOnly="true" UnboundType="String" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Project" Caption="Project #" />
                                    <dx:GridViewDataTextColumn FieldName="Project Start MM/YYYY" Caption="Project Start Date" />
                                    <dx:GridViewDataTextColumn FieldName="AmountBilled" Caption="Billing US$" />
                                    <dx:GridViewDataTextColumn FieldName="Domain" Caption="Domain" />
                                    <dx:GridViewDataTextColumn FieldName="Subdomain" Caption="Subdomain" />
                                    <dx:GridViewDataTextColumn FieldName="Production" Caption="Production" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Production %" Caption="Production %" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Marketing" Caption="Marketing" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Training" Caption="Training" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Other" Caption="Other" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Total Salary" Caption="Total" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Total %" Caption="Total %" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Support Salary" Caption="Support Salary" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="MSEB" Caption="MSEB" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Internet" Caption="Internet" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Rent" Caption="Rent" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="General Expenses" Caption="Adminstration & General Expenses" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Repair And Maintenance" Caption="Repair And Maintance" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Depreciation" Caption="Depreciation" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="VendorCost" Caption="Vendor Cost" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Total Cost" Caption="Total Cost" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="No of Employees" Caption="No of Employees" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Total Cost + Support Salary" Caption="Total Cost + Support Salary" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Cost/FTE" Caption="Cost/FTE (Total Cost)" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Cost/FTE (Production + Training)" Caption="Cost/FTE (Production + Training)" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Cost/FTE (Production + Training + Marketing + Support)" Caption="Cost/FTE (Production + Training + Marketing + Support)" CellStyle-HorizontalAlign="Center" CellStyle-Wrap="True" />
                                    <dx:GridViewDataTextColumn FieldName="RecordsBilled" Caption="Records Billed" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Cost/Record(Production + Training)" Caption="Cost/Record(Production + Training)" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Cost/Record (Production + Training + Marketing + Support)" Caption="Cost/Record (Production + Training + Marketing + Support)" CellStyle-HorizontalAlign="Center" />
                                    <dx:GridViewDataTextColumn FieldName="Cost/Record (Total Cost)" Caption="Cost/Record (Total Cost)" CellStyle-HorizontalAlign="Center" />
                                </Columns>
                                <Settings ShowFooter="True" GridLines="Both" />
                                <Styles>
                                    <Footer Border-BorderColor="LightGray" Border-BorderWidth="1"></Footer>
                                </Styles>
                                <TotalSummary>
                                    <dx:ASPxSummaryItem FieldName="Production" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Marketing" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Training" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Other" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Total Salary" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Support Salary" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="MSEB" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Internet" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Rent" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="GeneralExpenses" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="RepairAndMaintance" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Depreciation" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Total Cost" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="No of Employees" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Cost/FTE" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Cost/FTE (Production + Training)" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Cost/FTE (Production + Training + Marketing + Support)" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="RecordsBilled" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Cost/Record(Production + Training)" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Cost/Record (Production + Training + Marketing + Support)" SummaryType="Sum" DisplayFormat="n2" />
                                    <dx:ASPxSummaryItem FieldName="Cost/Record (Total Cost)" SummaryType="Sum" DisplayFormat="n2" />
                                </TotalSummary>
                            </dx:ASPxGridView>
                        </div>
                    </div>
                    <div class="tab-pane active" id="tab4" role="tabpanel">
                        <div class="card-content">
                            <div class="card-body">
                                <ul class="nav nav-tabs " role="tablist" style="margin-top: 5px!important;">
                                    <li class="nav-item" id="Li1" runat="server">
                                        <a class="nav-link" data-toggle="tab" href="#tab5" role="tab"><i class="fa fa-user"></i>&nbsp;&nbsp;Vendor Outsourcing Cost</a>
                                        <div class="slide"></div>
                                    </li>
                                    <li class="nav-item active" id="Li2" runat="server">
                                        <a class="nav-link" data-toggle="tab" href="#tab6" role="tab"><i class="fa fa-envelope"></i>&nbsp;&nbsp;Other Cost</a>
                                        <div class="slide"></div>
                                    </li>
                                </ul>
                                <div class="tab-content card-block" style="border: solid 1px #6c757d; margin-left: 5px;">
                                    <div class="tab-pane" id="tab5" role="tabpanel">
                                        <table class="table-condensed">
                                            <tr>
                                                <td width="30px"></td>
                                                <td><b>Month:</b></td>
                                                <td width="260px">
                                                    <asp:DropDownList ID="drpMonth" runat="server" Width="210px" Height="25px"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RFV1" runat="server" ControlToValidate="drpMonth" ErrorMessage="Please select month name." ForeColor="Red" Display="None" SetFocusOnError="True" InitialValue="Select" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                                                </td>
                                                <td><b>Year:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="drpYear" runat="server" Width="210px" Height="25px"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RFV2" runat="server" ControlToValidate="drpYear" ErrorMessage="Please select year." ForeColor="Red" Display="None" SetFocusOnError="True" InitialValue="Select" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td><b>Project Name:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="drpProjectName" runat="server" Height="25px" OnSelectedIndexChanged="drpProjectName_SelectedIndexChanged" Width="210px">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RFV3" runat="server" ControlToValidate="drpProjectName" ErrorMessage="Please select project name." ForeColor="Red" Display="None" SetFocusOnError="True" ValidationGroup="Submit" InitialValue="Select"></asp:RequiredFieldValidator>
                                                </td>
                                                <td><b>Volume Outsourced:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtvolume" runat="server" MaxLength="10" Width="210px"></asp:TextBox>
                                                    <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" Enabled="True" FilterMode="InvalidChars" InvalidChars="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&amp;*(),-+_=?/\&gt;&lt;.'`~;:()[]{}|" TargetControlID="txtvolume">
                                                    </asp:FilteredTextBoxExtender>
                                                    <asp:RequiredFieldValidator ID="RFV4" runat="server" ControlToValidate="txtvolume" Display="None" ErrorMessage="Please enter volume outsourced." ForeColor="Red" SetFocusOnError="True" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                                                </td>
                                                <td width="50px"></td>
                                                <td rowspan="5" style="vertical-align: top"></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td><b>Vendor Billing:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtVendorBilling" runat="server" MaxLength="10" Width="210px"></asp:TextBox>
                                                    <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" Enabled="True" FilterMode="InvalidChars" InvalidChars="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&amp;*(),-+_=?/\&gt;&lt;.'`~;:()[]{}|" TargetControlID="txtVendorBilling">
                                                    </asp:FilteredTextBoxExtender>
                                                    <asp:RequiredFieldValidator ID="RFV5" runat="server" ControlToValidate="txtVendorBilling" Display="None" ErrorMessage="Please enter vendor billing number." ForeColor="Red" SetFocusOnError="True" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                                                </td>
                                                <td><b>Other any charges:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtTotalCosting" runat="server" MaxLength="10" Width="210px"></asp:TextBox>
                                                    <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender7" runat="server" Enabled="True" FilterMode="InvalidChars" InvalidChars="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&amp;*(),-+_=?/\&gt;&lt;.'`~;:()[]{}|" TargetControlID="txtTotalCosting">
                                                    </asp:FilteredTextBoxExtender>
                                                    <asp:RequiredFieldValidator ID="RFV6" runat="server" ControlToValidate="txtTotalCosting" Display="None" ErrorMessage="Please enter total costing." ForeColor="Red" SetFocusOnError="True" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td>
                                                    <asp:Button ID="btnVendorCost" runat="server" Font-Bold="True" Font-Size="18px" Height="30px" OnClick="btnVendorCost_Click" Text="Submit" ValidationGroup="Submit" Width="100px" />
                                                    <asp:ValidationSummary ID="vsRemark" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="Submit" />
                                                </td>
                                            </tr>
                                        </table>
                                        <dx:ASPxGridView ID="grdVendorCost" runat="server" AutoGenerateColumns="False" EnableRowsCache="False" KeyFieldName="CostId" Theme="Office2010Silver" ClientInstanceName="gridVendor"
                                            OnCustomCallback="grdVendorCost_CustomCallback" OnCustomUnboundColumnData="grdVendorCost_CustomUnboundColumnData" OnRowUpdating="grdVendorCost_RowUpdating" Width="100%">
                                            <Settings ShowFilterRow="True" ShowFilterRowMenu="True" />
                                            <SettingsPager Mode="ShowAllRecords"></SettingsPager>
                                            <ClientSideEvents EndCallback="OnEndCallbackVendorCost" />
                                            <Columns>
                                                <dx:GridViewDataTextColumn FieldName="CostId" Caption="CostId" Visible="false"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Number" VisibleIndex="0" Caption="Sr. #" Width="40px" CellStyle-HorizontalAlign="Center" UnboundType="String" ReadOnly="True" ShowInCustomizationForm="True"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Month" VisibleIndex="1" Width="50px" ReadOnly="true" ShowInCustomizationForm="True"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Year" VisibleIndex="2" Width="50px" ReadOnly="true" CellStyle-HorizontalAlign="Center" ShowInCustomizationForm="True"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Project Name" FieldName="ProjectName" ReadOnly="True" VisibleIndex="3" Width="50px" CellStyle-HorizontalAlign="Center" ShowInCustomizationForm="True"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Volume Outsourced" FieldName="VolumeOutsourced" VisibleIndex="4" Width="50px" CellStyle-HorizontalAlign="Center" ShowInCustomizationForm="True">
                                                    <PropertiesTextEdit>
                                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" ErrorText="Please Enter Volume Outsourced" SetFocusOnError="True">
                                                            <RegularExpression ErrorText="Please Enter only Numbers." ValidationExpression="^[0-9]*$"></RegularExpression>
                                                            <RequiredField IsRequired="True" ErrorText="Please Enter Volume Outsourced."></RequiredField>
                                                        </ValidationSettings>
                                                    </PropertiesTextEdit>
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Vendor Billing" FieldName="VendorBilling" VisibleIndex="5" Width="50px" CellStyle-HorizontalAlign="Center" ShowInCustomizationForm="True">
                                                    <PropertiesTextEdit>
                                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" ErrorText="Please Enter Vendor Billing" SetFocusOnError="True">
                                                            <RegularExpression ErrorText="Please Enter only Numbers." ValidationExpression="^[0-9]*$"></RegularExpression>
                                                            <RequiredField IsRequired="True" ErrorText="Please Enter Vendor Billing."></RequiredField>
                                                        </ValidationSettings>
                                                    </PropertiesTextEdit>
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Other Charges" FieldName="TotalCosting" VisibleIndex="6" Width="50px" CellStyle-HorizontalAlign="Center" ShowInCustomizationForm="True">
                                                    <PropertiesTextEdit>
                                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" ErrorText="Please Enter Other Charges" SetFocusOnError="True">
                                                            <RegularExpression ErrorText="Please Enter only Numbers." ValidationExpression="^[0-9]*$"></RegularExpression>
                                                            <RequiredField IsRequired="True" ErrorText="Please Enter Other Charges."></RequiredField>
                                                        </ValidationSettings>
                                                    </PropertiesTextEdit>
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewCommandColumn ButtonType="Button" Caption="Edit" Width="50px" VisibleIndex="7">
                                                    <EditButton Visible="True" Text="Edit"></EditButton>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                </dx:GridViewCommandColumn>
                                            </Columns>
                                        </dx:ASPxGridView>
                                    </div>
                                    <div class="tab-pane active" id="tab6" role="tabpanel">
                                        <table class="table-condensed">
                                            <tr>
                                                <td width="30px"></td>
                                                <td><b>Month:</b></td>
                                                <td width="250px">
                                                    <asp:DropDownList ID="drpMonth1" runat="server" Width="210px" Height="25px"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RFV7" runat="server" ControlToValidate="drpMonth1" ErrorMessage="Please select month name." ForeColor="Red" Display="none" SetFocusOnError="true" InitialValue="Select" ValidationGroup="Cost"></asp:RequiredFieldValidator>
                                                </td>
                                                <td><b>Year:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="drpYear1" runat="server" Width="210px" Height="25px"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RFV8" runat="server" ControlToValidate="drpYear1" ErrorMessage="Please select year." ForeColor="Red" Display="none" SetFocusOnError="true" InitialValue="Select" ValidationGroup="Cost"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="30px"></td>
                                                <td><b>Domain:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlDomainForOther" runat="server" Width="210px" Height="25px"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td><b>Cost Type:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="drpCostType" runat="server" Width="210px" Height="25px">
                                                        <asp:ListItem Value="Select">Select</asp:ListItem>
                                                        <asp:ListItem Value="AdminstrationAndGeneralExpenses">AdminstrationAndGeneralExpenses</asp:ListItem>
                                                        <asp:ListItem Value="Electricity">ElectricityCharges</asp:ListItem>
                                                        <asp:ListItem Value="Internet">Internet Charges</asp:ListItem>
                                                        <asp:ListItem Value="PremisesRental">Premises Rental Charges</asp:ListItem>
                                                        <asp:ListItem Value="DepreciationInsurance">Depreciation & Insurance</asp:ListItem>
                                                        <asp:ListItem Value="Maintenance">Maintenance</asp:ListItem>
                                                        <asp:ListItem Value="IndiaVendorCost">India Vendor Cost</asp:ListItem>
                                                        <asp:ListItem Value="USPayrollAllocation">US Payroll Allocation</asp:ListItem>
                                                        <asp:ListItem Value="USTotalIndirectExpense">US Total Indirect Expense</asp:ListItem>
                                                        <asp:ListItem Value="IndependantContribution">Independant Contribution</asp:ListItem>
                                                        <asp:ListItem Value="USPayroll">US Payroll</asp:ListItem>
                                                        <asp:ListItem Value="USMarketingExpenses">US Marketing Expenses</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="drpCostType" ErrorMessage="Please select Cost Type." ForeColor="Red" Display="None" SetFocusOnError="True"
                                                        ValidationGroup="Cost" InitialValue="Select"></asp:RequiredFieldValidator>
                                                </td>
                                                <td><b>Amount:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtamount" MaxLength="10" runat="server" Width="210px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:FilteredTextBoxExtender runat="server" ID="FilteredTextBoxExtender1" TargetControlID="txtamount" FilterMode="InvalidChars"
                                                        InvalidChars="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*(),-+_=?/\><.'`~;:()[]{}|">
                                                    </asp:FilteredTextBoxExtender>
                                                    <asp:RequiredFieldValidator ID="RFV9" runat="server" ControlToValidate="txtamount" ErrorMessage="Please enter amount." ForeColor="Red" Display="none" SetFocusOnError="true" ValidationGroup="Cost"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td><b>Remark:</b></td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtremark" runat="server" Style="resize: none;" Height="50px" TextMode="MultiLine" Width="580px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:FilteredTextBoxExtender runat="server" ID="FilteredTextBoxExtender2" TargetControlID="txtremark" FilterMode="InvalidChars"
                                                        InvalidChars="!@#$%^&*()+_=?/\><'`~;()[]{}|">
                                                    </asp:FilteredTextBoxExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td>
                                                    <asp:Button ID="btnOtherCost" Text="Submit" runat="server" OnClick="btnOtherCost_Click" ValidationGroup="Cost" Width="100px" Height="30px" Font-Bold="true" Font-Size="18px" />
                                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Cost" ShowMessageBox="true" ShowSummary="false" />
                                                </td>
                                            </tr>
                                        </table>
                                        <dx:ASPxGridView ID="grdOtherCost" runat="server" AutoGenerateColumns="false" EnableRowsCache="False" KeyFieldName="OtherCId" OnRowUpdating="grdOtherCost_RowUpdating" Theme="Office2010Silver" ClientInstanceName="gridOther"
                                            OnCustomCallback="grdOtherCost_CustomCallback" OnCustomUnboundColumnData="grdOtherCost_CustomUnboundColumnData" Width="100%">
                                            <Settings ShowFilterRow="true" ShowFilterRowMenu="true" />
                                            <SettingsPager Mode="EndlessPaging"></SettingsPager>
                                            <ClientSideEvents EndCallback="OnEndCallbackOtherCost" />
                                            <Columns>
                                                <dx:GridViewDataTextColumn FieldName="OtherCId" Caption="OtherCId" Visible="false"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Number" VisibleIndex="0" Caption="Sr. #" Width="40px" CellStyle-HorizontalAlign="Center" UnboundType="String" ReadOnly="true"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Month" ReadOnly="True" VisibleIndex="1" Width="50px"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Year" ReadOnly="True" VisibleIndex="2" Width="50px" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Cost Type" FieldName="CostType" ReadOnly="True" VisibleIndex="3" Width="50px"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Amount" FieldName="Amount" VisibleIndex="4" Width="50px" CellStyle-HorizontalAlign="Center">
                                                    <PropertiesTextEdit>
                                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" ErrorText="Please Enter Amount" SetFocusOnError="True">
                                                            <RegularExpression ErrorText="Please enter Only Numbers." ValidationExpression="[0-9]*$"></RegularExpression>
                                                            <RequiredField IsRequired="True" ErrorText="Please Enter Amount."></RequiredField>
                                                        </ValidationSettings>
                                                    </PropertiesTextEdit>
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Remark" FieldName="Remark" VisibleIndex="5" Width="50px"></dx:GridViewDataTextColumn>
                                                <dx:GridViewCommandColumn ButtonType="Button" Caption="Edit" HeaderStyle-HorizontalAlign="Center" Width="50px">
                                                    <EditButton Visible="true" Text="Edit"></EditButton>
                                                </dx:GridViewCommandColumn>
                                            </Columns>
                                        </dx:ASPxGridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div style="display: none;">
            <CC12:TabContainer runat="server" ActiveTabIndex="0" ID="TabCost" CssClass="fancy fancy-green">
                <CC12:TabPanel ID="PnlUser" runat="server" HeaderText="Training Summary" CssClass="panel">
                    <HeaderTemplate>
                        <div class="divTab">User Wise</div>
                    </HeaderTemplate>
                    <ContentTemplate>
                    </ContentTemplate>
                </CC12:TabPanel>

                <CC12:TabPanel ID="PnlDomain" runat="server" HeaderText="Domain Wise" CssClass="panel">
                    <ContentTemplate>
                    </ContentTemplate>
                </CC12:TabPanel>

                <CC12:TabPanel ID="PnlProject" runat="server" HeaderText="Project Wise" Width="1080px" CssClass="panel" Visible="false">
                    <ContentTemplate>
                    </ContentTemplate>
                </CC12:TabPanel>

                <CC12:TabPanel ID="PnlCostMaster" runat="server" HeaderText="Cost Master" Width="1080px" CssClass="panel">
                    <ContentTemplate>
                        <asp:TabContainer runat="server" ActiveTabIndex="0" ID="TabTicket" CssClass="fancy fancy-green">
                            <asp:TabPanel ID="PnlVendorCost" runat="server" HeaderText="Cost Master" CssClass="panel">
                                <HeaderTemplate>
                                    <div class="divTab">Vendor Outsourcing Cost</div>
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <dx:ASPxRoundPanel ID="RndPanelVendorCost" ClientInstanceName="roundPanel" HeaderText="Vendor Cost" runat="server" CssClass="col-sm-10" AllowCollapsingByHeaderClick="false" ShowCollapseButton="false" EnableAnimation="true" ShowHeader="true" View="Standard" Width="100%">
                                        <PanelCollection>
                                            <dx:PanelContent>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxRoundPanel>
                                    <div style="clear: both;"></div>

                                    <br />
                                    <div style="overflow: auto">
                                        <b></b>

                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>

                            <asp:TabPanel ID="PnlOtherCost" runat="server" HeaderText="Other Cost" CssClass="panel">
                                <HeaderTemplate>
                                    <div class="divTab">Other Cost</div>
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <dx:ASPxRoundPanel ID="RndPnlOtherCost" ClientInstanceName="roundPanel" HeaderText="Other Cost" runat="server" CssClass="col-sm-10" AllowCollapsingByHeaderClick="false" ShowCollapseButton="false" EnableAnimation="true" ShowHeader="true" View="Standard" Width="100%">
                                        <PanelCollection>
                                            <dx:PanelContent>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxRoundPanel>
                                    <div style="clear: both;"></div>
                                    <br />
                                    <div style="overflow: auto">
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                        </asp:TabContainer>
                    </ContentTemplate>
                </CC12:TabPanel>
            </CC12:TabContainer>
        </div>
    </div>
</asp:Content>
