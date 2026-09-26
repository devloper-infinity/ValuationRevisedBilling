<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="VerifyProjectCostFreight.aspx.cs" Inherits="BillingOther.Accounts.VerifyProjectCostFreight" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Accounts/VerifyCostMigration.css") %>" rel="stylesheet" />
    <script src="<%= ResolveUrl("~/Accounts/VerifyCostMigration.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server"></div>
    <input type="hidden" ID="hdProductBaseLabel" runat="server" />
    <input type="hidden" ID="hdProductOtherLabel" runat="server" />
    <input type="hidden" ID="hdOrderBaseLabel" runat="server" />
    <input type="hidden" ID="hdOrderOtherLabel" runat="server" />
    <section class="verify-summary-card">
        <div class="verify-summary-header">
            <div><div class="verify-eyebrow">INFINITY IPS BILLING</div><h2>Verify Project Cost</h2><p>Review configured project pricing before approval.</p></div>
            <a class="verify-back-btn" href="VerifyCostMaster.aspx" onclick="showStandardLoading('Returning to project list...')"><i class="fa fa-arrow-left"></i> Back</a>
        </div>
        <div id="summaryContainer" runat="server"></div>
    </section>
    <script>
        function GetConfiguration(ddl) {
            var trBaseRate = document.getElementById("<%= trBaseRate.ClientID %>");
            var trOrderType = document.getElementById("<%= trOrderType.ClientID %>");
            var index = ddl.selectedIndex;
            if (index == 1) {
                trBaseRate.style.display = '';
                trOrderType.style.display = 'none';
            }
            else if (index == 2) {
                trBaseRate.style.display = 'none';
                trOrderType.style.display = '';
            }
        }
    </script>
    <table class="verify-form-table" style="border: solid 1px black; display: none;" id="tblOther" runat="server">
        <tr>
            <td><b>Billing Type:</b></td>
            <td>
                <select ID="ddlBillingBase" runat="server" class="verify-control" onchange="GetConfiguration(this);">
                    <option value="Select">Select</option>
                    <option value="Base Rate">Base Rate Only</option>
                    <option value="Product Type">Product Type</option>
                    <option value="Invoice Type">Invoice Type</option>
                </select>
            </td>
            <td>
                
            </td>
        </tr>
        <tr id="trBaseRate" runat="server" style="display: none;">
            <td colspan="3">
                <table class="verify-form-table">
                    <tr>
                        <th>Parameter Name</th>
                        <th>Is Applicable?</th>
                        <th>Charge Type</th>
                        <th>Price</th>
                        <th>Billing Header</th>
                    </tr>
                    <tr>
                        <td>Base Rate</td>
                        <td>
                            <select ID="ddlIsApplicableBaseRate" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeBaseRate" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                                <option value="Incremental">Incremental</option>
                                <option value="Percentage">Percentage</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceBaseRate" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderBaseRate" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Dispatched Record">Dispatched Record</option>
                                <option value="No Of Records">No Of Records</option>
                                <option value="Order#">Order#</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>Rush</td>
                        <td>
                            <select ID="ddlIsApplicableBaseRateRush" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeBaseRateRush" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                                <option value="Incremental">Incremental</option>
                                <option value="Percentage">Percentage</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceBaseRateRush" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderBaseRateRush" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Order#">Order#</option>
                                <option value="Rush">Rush</option>
                            </select>
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td># Characters > 800</td>
                        <td>
                            <select ID="ddlIsApplicableBaseRateCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeBaseRateCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                                <option value="Incremental">Incremental</option>
                                <option value="Percentage">Percentage</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceBaseRateCharacter" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderBaseRateCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="# of Character"># of Character</option>
                            </select>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr id="trOrderType" runat="server" style="display: none;">
            <td colspan="3">
                <script>
                    function GetOrderTypeInfo(ddlOrder) {
                        var lblBaseOrder = document.getElementById("<%= lblOrderTypeBase.ClientID %>");
                                        var lblOtherOrder = document.getElementById("<%= lblOrderTypeOther.ClientID %>");
                                        var hdOrderBaseLabel = document.getElementById("<%= hdOrderBaseLabel.ClientID %>");
                                        var hdOrderOtherLabel = document.getElementById("<%= hdOrderOtherLabel.ClientID %>");
                        var index = ddlOrder.selectedIndex;
                        var ddlText = ddlOrder.options[index].text;
                        for (var i = 0; i < ddlOrder.length; i++) {
                            if (ddlOrder.options[i].text == ddlText && ddlOrder.options[i].text != "Select") {
                                lblBaseOrder.innerHTML = ddlText;
                                hdOrderBaseLabel.value = ddlText;
                            }
                            if (ddlOrder.options[i].text != ddlText && ddlOrder.options[i].text != "Select") {
                                lblOtherOrder.innerHTML = ddlOrder.options[i].text;
                                hdOrderOtherLabel.value = ddlOrder.options[i].text;
                            }
                        }
                    }
                </script>
                <table class="verify-form-table" border="1">
                    <tr>
                        <td><b>Base Invoice Type:</b></td>
                        <td>
                            <select ID="ddlBaseOrderType" runat="server" class="verify-control" onchange="GetOrderTypeInfo(this);">
                                <option value="Select">Select</option>
                                <option value="International">International</option>
                                <option value="Standard">Standard</option>
                            </select>
                        </td>
                    </tr>
                </table>
                <table class="verify-form-table" border="1">
                    <tr>
                        <th>Parameter Name</th>
                        <th>Is Applicable?</th>
                        <th>Charge Type</th>
                        <th>Price</th>
                        <th>Billing Header</th>
                    </tr>
                    <tr>
                        <td>
                            <span ID="lblOrderTypeBase" runat="server" class="verify-control"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableOrderBase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeOrderBase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceOrderBase" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderOrderBase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Invoice Type">Invoice Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span ID="lblOrderTypeOther" runat="server" class="verify-control"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableOrderOther" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeOrderOther" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                                <option value="Incremental">Incremental</option>
                                <option value="Percentage">Percentage</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceOrderOther" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderOrderOther" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Invoice Type">Invoice Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td># Characters > 800</td>
                        <td>
                            <select ID="ddlIsApplicableOrderCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeOrderCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                                <option value="Incremental">Incremental</option>
                                <option value="Percentage">Percentage</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceOrderCharacter" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHEaderOrderCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="# of Character"># of Character</option>
                            </select>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <button type="button" id="btnShubmitTyping" class="verify-primary-btn" onclick="approveFreightCost()">Approve Costing</button>
            </td>
            <td></td>
        </tr>
    </table>
    <table class="verify-form-table" id="tbl771" runat="server" style="display: none; border: solid 1px black;">
        <tr>
            <th>Parameter</th>
            <th>Charge Type</th>
            <th>Price</th>
            <th>Billing Header</th>
        </tr>
        <tr>
            <td>No of Pages - Scan</td>
            <td>
                <select ID="ddlChargeType771ScanPages" runat="server" class="verify-control">
                    <option value="Select">Select</option>
                    <option value="Fix Amount">Fix Amount</option>
                </select>
            </td>
            <td>
                <input type="text" ID="txtPrice771ScanPages" runat="server" class="verify-control" />
            </td>
            <td>
                <select ID="ddlBillingHeader771ScanPages" runat="server" class="verify-control">
                    <option value="Select">Select</option>
                    <option value="No of Pages">No of Pages</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>No of Records - Scan</td>
            <td>
                <select ID="ddlChargeType771ScanRecords" runat="server" class="verify-control">
                    <option value="Select">Select</option>
                    <option value="Fix Amount">Fix Amount</option>
                </select>
            </td>
            <td>
                <input type="text" ID="txtPrice771ScanRecords" runat="server" class="verify-control" />
            </td>
            <td>
                <select ID="ddlBillingHeader771ScanRecords" runat="server" class="verify-control">
                    <option value="Select">Select</option>
                    <option value="No of Records">No of Records</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>No of Records - Mail</td>
            <td>
                <select ID="ddlChargeType771MailRecords" runat="server" class="verify-control">
                    <option value="Select">Select</option>
                    <option value="Fix Amount">Fix Amount</option>
                </select>
            </td>
            <td>
                <input type="text" ID="txtPrice771MailRecords" runat="server" class="verify-control" />
            </td>
            <td>
                <select ID="ddlBillingHeader771MailRecords" runat="server" class="verify-control">
                    <option value="Select">Select</option>
                    <option value="No of Records">No of Records</option>
                </select>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <button type="button" id="btnSubmit771" class="verify-primary-btn" onclick="approveFreightCost()">Approve Costing</button>
            </td>
            <td></td>
            <td></td>
        </tr>
    </table>

<div id="standardLoadingOverlay" class="standard-loading-overlay show"><div class="standard-loading-box"><div class="standard-spinner"></div><div class="standard-loading-title">Please wait...</div><div id="standardLoadingMessage" class="standard-loading-text">Loading project cost...</div></div></div>

<script>
function approveFreightCost(){
 var projectId=parseInt(new URLSearchParams(window.location.search).get('ProjectId')||'0',10);
 pageMethod('VerifyProjectCostFreight.aspx/ApproveCosting',{projectId:projectId,loadingMessage:'Approving project costing...'},function(r){showVerifyMessage(r.Message,!r.Success);});
}
$(function(){hideStandardLoading();});
</script></asp:Content>
