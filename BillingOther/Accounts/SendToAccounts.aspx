<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="SendToAccounts.aspx.cs" Inherits="BillingOther.Accounts.SendToAccounts" %>

<%@ Register Assembly="DevExpress.Web.v14.2, Version=14.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Data.v14.2, Version=14.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Data" TagPrefix="dx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="BillingOther" Namespace="BillingOther.Accounts" TagPrefix="portal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>  function HideLabel() {
            // alert("Hi");
            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%# dvError.ClientID %>").style.display = "none";
            }, seconds * 1000);
        };
        function EditOrder(element, key) {
            OrderChange.SetContentHtml("");
            popupExcelReport.ShowAtElementByID("Link_" + key);
            keyValue = key;
        }
        function popupExcelReport_Shown(s, e) {
            OrderChange.PerformCallback(keyValue);
        }

        function EditOrderFTE(element, key) {
            OrderChangeFTE.SetContentHtml("");
            popupExcelReportFTE.ShowAtElementByID("Link_" + key);
            keyValue = key;
        }
        function popupExcelReportFTE_Shown(s, e) {
            OrderChangeFTE.PerformCallback(keyValue);
        }


    </script>
    <style>
        .table td, .table th {
            padding: 3px !important;
        }

        .table th {
            background-color: #DCDCDC;
        }
    </style>
    <style>
        .modalBackground {
            background-color: gray;
            filter: blur(5px);
            opacity: 0.8;
        }

            .modalBackground:hover {
            }

        .modalPopup {
        }

        .header {
            border-radius: 5px;
            margin: 10px;
            height: 30px;
            background-color: #FFFFFF;
            text-align: center;
            padding: 4px;
        }

        .body {
            padding: 10px;
            border-radius: 5px;
            width: 500px;
            min-height: 250px;
            height:auto;
            float: left;
            font-family:Verdana;
            font-size:11px;
            background-color: White;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server"></div>

    <a id="lnkDummy" runat="server"></a>
    <asp:ModalPopupExtender ID="mdlBillingMerger" BehaviorID="mpe" runat="server"
        PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modal modal-styled fade" CancelControlID="btnHide">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">

        <div class="body">
            <span>Previous billing pending for below billing periods. If you want to merge billing then click on check box to coninue.</span><br />
            <br />
            <asp:GridView ID="grdPendingPeriod" runat="server" CssClass="dataTables_wrapper" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="#3AC0F2" HeaderStyle-ForeColor="White"
                AutoGenerateColumns="false">
                <Columns>
                    <asp:TemplateField HeaderText="Select" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkRow" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Billing Period" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="250">
                        <ItemTemplate>
                            <asp:Label ID="lblBillingPeriod" runat="server" Text='<%# Eval("BillingPeriod") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <span style="color: red; font-weight: bold;">(Note: Billing Period will be changed if you merge billing)</span><br />
            <br />
            <asp:Button ID="btnContinueMerge" runat="server" Text="Merge and Continue" CssClass="btn btn-secondary" OnClick="btnContinueMerge_Click" />
            <asp:Button ID="btnHide" runat="server" Text="Cancel" Style="float: right;" CssClass="btn btn-primary" />
        </div>
    </asp:Panel>
    <portal:CrystalViewerShim ID="ILSReport" runat="server" Visible="false" />

    <asp:HiddenField ID="hdnTotalRows" runat="server" />
    <asp:HiddenField ID="hdOrderNo" runat="server" />
    <p>
        <asp:Button ID="btnSaveVerified" runat="server" Text="Save Verified Orders" OnClick="btnSaveVerified_Click" Visible="false" CssClass="btn btn-primary"></asp:Button>
    </p>
    <div class="row">
        <div class="col-md-8 col-sm-8">
            <table class="table table-condensed table-bordered">
                <tr>
                    <td><strong>Project Number</strong></td>
                    <td><strong>Billing Period</strong></td>

                    <td><strong>Total Order/Invoice #</strong></td>
                    <td><strong>Total Amount</strong></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblProject" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblBillingPeriod" runat="server"></asp:Label></td>

                    <td>
                        <asp:Label ID="txtOrder" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="txtAmount" runat="server"></asp:Label></td>
                </tr>

            </table>
            <div style="overflow: auto; margin-top: 10px;">
                <portal:PortalDataGrid ID="grdTemplate" runat="server" CssClass="table portal-datatable" AutoGenerateColumns="true" EnableSelection="false" />
            </div>
            <div style="overflow: auto; display: none; margin-top: 10px;" id="dvStatus" runat="server">
                <portal:PortalDataGrid ID="grdStatus" runat="server" CssClass="table portal-datatable" AutoGenerateColumns="true" EnableSelection="false" />
            </div>
            <div style="margin-top: 10px;">
                <b style="font-weight: bold!important;">Invoice # :</b>
                <asp:TextBox ID="lblInvoiceNo" runat="server" Width="250px" Enabled="false"></asp:TextBox>
                <asp:CheckBox ID="chkInvoiceNo" runat="server" Text="Click to update Invoice number" onclick="update();" />
                <script>
                    function update() {
                        var lblinvoiceNo = document.getElementById("<%= lblInvoiceNo.ClientID %>");
                    var chk = document.getElementById("<%= chkInvoiceNo.ClientID %>");
                        if (chk.checked == true) {
                            lblinvoiceNo.disabled = false;
                            lblinvoiceNo.focus();
                        }
                        else {
                            lblinvoiceNo.disabled = true;
                        }
                    }
                </script>
            </div>
        </div>
        <div class="col-md-4">
            <p>
                <asp:Button ID="btnBack" runat="server" Text="<< Back" OnClick="btnBack_Click" CssClass="btn btn-warning btn-rounded"></asp:Button>
            </p>
            <p>
                <asp:Button ID="btnSendToClient" runat="server" Text="Ready To Send To Client" OnClick="btnSendToClient_Click" CssClass="btn btn-primary"></asp:Button>
            </p>
            <p>
                <asp:Button ID="btnPreviewInvoice" runat="server" Text="Preview Invoice" OnClick="btnPreviewInvoice_Click" CssClass="btn btn-info" Style="font-weight: bold;"></asp:Button>
            </p>
            <p>
                <asp:Button ID="btnExportToExcel" runat="server" Text="Export To Excel" OnClick="btnExportToExcel_Click" CssClass="btn btn-success" />
            </p>

        </div>
    </div>
    <div>

        <div class="table-responsive">
            <div style="overflow: auto;">
                <asp:Label ID="lblRecords" runat="server" Style="display: none;"></asp:Label>
                <div style="margin-bottom: 10px; margin-top: 10px;">
                    <div>
                    </div>

                </div>
                <div style="padding-top: 5px;">
                    <portal:PortalDataGrid ID="grdBilling" runat="server" CssClass="table portal-datatable" AutoGenerateColumns="true" KeyFieldName="TrackingSheetID" EnableSelection="true" />
                </div>
            </div>
        </div>
    </div>
    <div class="col-md-2" style="display:none;">


        <p id="sendback" runat="server">
            <a data-toggle="modal" href="#SendBackToProduction" class="btn btn-danger btn-round" style="width: 200px; font-weight: bold;">Send Back To Production</a>
            <%--<asp:Button ID="btnBackToProd" runat="server" Text="" Width="200px" CssClass="btn btn-primary" data-toggle="modal" href="#"></asp:Button>--%>
        </p>


    </div>
    <div class="col-md-2">
        <p id="invoice" runat="server" style="display: none;">
        </p>
    </div>
    <dx:ASPxPopupControl ID="popupExcelReportFTE" ClientInstanceName="popupExcelReportFTE" runat="server" AllowDragging="true" PopupHorizontalAlign="WindowCenter" HeaderText="Edit FTE Hours" PopupVerticalAlign="WindowCenter" Width="700px">
        <HeaderStyle Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1FTE" runat="server">
                <dx:ASPxCallbackPanel ID="OrderChangeFTE" runat="server" OnCallback="OrderChangeFTE_Callback" ClientInstanceName="OrderChangeFTE" Width="700px" Height="300px" RenderMode="Table">
                    <PanelCollection>
                        <dx:PanelContent ID="pnlViewTaxDetailsFTE" runat="server">
                            <table class="table table-condensed">
                                <tr>

                                    <td><b>Project:</b></td>
                                    <td>
                                        <asp:Label ID="lblProjectNoFTE" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><b>Employee:</b></td>
                                    <td>
                                        <asp:Label ID="lblEmployee" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><b>Process:</b></td>
                                    <td>
                                        <asp:Label ID="lblProcess" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><b>Existing FTE Hours:</b></td>
                                    <td>
                                        <asp:Label ID="lblExistingFTEHours" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><b>Enter Hours</b></td>
                                    <td>
                                        <asp:TextBox ID="txtFTEHours" runat="server" Width="400px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1FTE" SetFocusOnError="true" runat="server"
                                            ValidationGroup="testFTE" ControlToValidate="txtFTEHours" Display="None" ForeColor="Red"
                                            ErrorMessage="Please enter FTE Hours"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td><b>Remark:</b></td>
                                    <td>
                                        <asp:TextBox ID="txtRemarkFTE" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnUpdateFTE" runat="server" Text="Update" ValidationGroup="testFTE" CssClass="btn btn-primary" OnClick="btnUpdateFTE_Click" /></td>

                                </tr>
                            </table>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ClientSideEvents Shown="popupExcelReportFTE_Shown" />
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="popupExcelReport" ClientInstanceName="popupExcelReport" runat="server" AllowDragging="true" PopupHorizontalAlign="WindowCenter" HeaderText="Edit Order Number" PopupVerticalAlign="WindowCenter" Width="700px">
        <HeaderStyle Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxCallbackPanel ID="OrderChange" runat="server" OnCallback="CallbackPanelTaxDetails_Callback" ClientInstanceName="OrderChange" Width="700px" Height="300px" RenderMode="Table">
                    <PanelCollection>
                        <dx:PanelContent ID="pnlViewTaxDetails" runat="server">
                            <table class="table table-condensed">
                                <tr>

                                    <td>Project:</td>
                                    <td>
                                        <asp:Label ID="lblProjectNo" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td>Order #:</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderNo" runat="server" Width="400px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" SetFocusOnError="true" runat="server"
                                            ValidationGroup="test" ControlToValidate="txtOrderNo" Display="None" ForeColor="Red"
                                            ErrorMessage="Please enter Order Number"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Remark:</td>
                                    <td>
                                        <asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnUpdate" runat="server" Text="Update" ValidationGroup="test" CssClass="btn btn-primary" OnClick="btnUpdate_Click" /></td>

                                </tr>
                            </table>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ClientSideEvents Shown="popupExcelReport_Shown" />
    </dx:ASPxPopupControl>
    <hr />
    <div id="SendBackToProduction" class="modal modal-styled fade">
        <div class="modal-dialog">
            <div class="modal-content" style="margin-top: 70px;">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h3 class="modal-title">Send Back To Production</h3>
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="txtSendBackToProductionRemark" runat="server" TextMode="MultiLine" CssClass="form-control" Height="100px" Width="500px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSendBackToProduction" runat="server" ControlToValidate="txtSendBackToProductionRemark" ErrorMessage="Please enter remark" Display="Dynamic" ForeColor="Red" ValidationGroup="sendback"></asp:RequiredFieldValidator>
                    <br />
                    <asp:Button ID="btnSendBackToProduction" runat="server" Text="Send Back To Production" OnClick="btnSendBackToProduction_Click" ValidationGroup="sendback" CssClass="btn btn-primary" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->


    <asp:Button ID="btnShowVerified" runat="server" Visible="false" Text="Show Verified Orders" OnClick="btnShowVerified_Click" Width="195px" Style="padding: 5px 10px;" Font-Bold="True"></asp:Button>

</asp:Content>
