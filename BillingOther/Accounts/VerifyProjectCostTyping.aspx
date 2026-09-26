<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="VerifyProjectCostTyping.aspx.cs" Inherits="BillingOther.Accounts.VerifyProjectCostTyping" %>


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
            var trProductType = document.getElementById("<%= trProductType.ClientID %>");
            var trOrderType = document.getElementById("<%= trOrderType.ClientID %>");
            var trCharacter = document.getElementById("<%= trCharacter.ClientID %>");
            var tbl379003 = document.getElementById("<%= tbl379003.ClientID %>");
            var index = ddl.selectedIndex;
            if (index == 1) {
                trBaseRate.style.display = '';
                trProductType.style.display = 'none';
                trOrderType.style.display = 'none';
                tbl379003.style.display = 'none';
                trCharacter.style.display = 'none';
            }
            else if (index == 2) {
                trBaseRate.style.display = 'none';
                trProductType.style.display = '';
                trOrderType.style.display = 'none';
                tbl379003.style.display = 'none';
                trCharacter.style.display = 'none';
            }
            else if (index == 3) {
                trBaseRate.style.display = 'none';
                trProductType.style.display = 'none';
                trOrderType.style.display = '';
                trCharacter.style.display = 'none';
                tbl379003.style.display = 'none';
            }
            else if (index == 4) {
                trBaseRate.style.display = 'none';
                trProductType.style.display = 'none';
                trOrderType.style.display = 'none';
                trCharacter.style.display = '';
                tbl379003.style.display = 'none';
            }
            else if (index == 5) {
                trBaseRate.style.display = 'none';
                trProductType.style.display = 'none';
                trOrderType.style.display = 'none';
                trCharacter.style.display = 'none';
                tbl379003.style.display = '';
            }
        }
    </script>
    <table class="verify-form-table" style="border: solid 1px black;">
        <tr>
            <td><b>Billing Type:</b></td>
            <td>
                <select ID="ddlBillingBase" runat="server" class="verify-control" onchange="GetConfiguration(this);">
                    <option value="Select">Select</option>
                    <option value="Base Rate">Base Rate Only</option>
                    <option value="Product Type">Product Type</option>
                    <option value="Order Type">Order Type</option>
                    <option value="Character Based">Character Based</option>
                    <option value="Page Based">Page Based</option>
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
                    <tr>
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
        <tr id="trProductType" runat="server" style="display: none;">
            <td colspan="3">
                <script>
                    function GetProductInfo(ddlProduct) {
                        var lblBaseProduct = document.getElementById("<%= lblProdcutBase.ClientID %>");
                        var lblOtherProduct = document.getElementById("<%= lblProdcutOther.ClientID %>");
                        var hdProductBaseLabel = document.getElementById("<%= hdProductBaseLabel.ClientID %>");
                        var hdProductOtherLabel = document.getElementById("<%= hdProductOtherLabel.ClientID %>");
                        var index = ddlProduct.selectedIndex;
                        var ddlText = ddlProduct.options[index].text;
                        for (var i = 0; i < ddlProduct.length; i++) {
                            if (ddlProduct.options[i].text == ddlText && ddlProduct.options[i].text != "Select") {
                                lblBaseProduct.innerHTML = ddlText;
                                hdProductBaseLabel.value = ddlText;
                            }
                            if (ddlProduct.options[i].text != ddlText && ddlProduct.options[i].text != "Select") {
                                lblOtherProduct.innerHTML = ddlProduct.options[i].text;
                                hdProductOtherLabel.value = ddlProduct.options[i].text;
                            }
                        }
                    }
                </script>
                <table class="verify-form-table" style="display: none;">
                    <tr>
                        <td><b>Base Product Type:</b></td>
                        <td>
                            <select ID="ddlBaseProductType" runat="server" class="verify-control" onchange="GetProductInfo(this);">
                            </select>
                        </td>
                    </tr>
                </table>
                <table class="verify-form-table">
                    <tr>
                        <th>Parameter Name</th>
                        <th>Is Applicable?</th>
                        <th>Charge Type</th>
                        <th>Price</th>
                        <th>Billing Header</th>
                    </tr>
                    <tr>
                        <td>
                            <span ID="Label1" runat="server" class="verify-control" Text="Purchase"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicablePurchase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypePurchase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPricePurchase" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderPurchase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Product Type">Product Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span ID="Label2" runat="server" class="verify-control" Text="Refinance"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableRefinance" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeRefinance" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceRefinance" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderRefinance" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Product Type">Product Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span ID="Label3" runat="server" class="verify-control" Text="Current Owner"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableCurrentOwner" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeCurrentOwner" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceCurrentOwner" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderCurrentOwner" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Product Type">Product Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span ID="Label4" runat="server" class="verify-control" Text="Full Search"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableFullSearch" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeFullSearch" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceFullSearch" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderFullSearch" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Product Type">Product Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span ID="Label5" runat="server" class="verify-control" Text="Two Owner"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableTwoOwner" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeTwoOwner" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceTwoOwner" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderTwoOwner" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Product Type">Product Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td># Characters > 800</td>
                        <td>
                            <select ID="ddlIsApplicableCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceCharacter" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderCharacterNew" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="# of Character"># of Character</option>
                            </select>
                        </td>
                    </tr>
                </table>
                <table class="verify-form-table" style="display: none;">
                    <tr>
                        <th>Parameter Name</th>
                        <th>Is Applicable?</th>
                        <th>Charge Type</th>
                        <th>Price</th>
                        <th>Billing Header</th>
                    </tr>
                    <tr>
                        <td>
                            <span ID="lblProdcutBase" runat="server" class="verify-control"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableProductBase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeProductBase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceProductBase" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHEaderProductBase" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Product Type">Product Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span ID="lblProdcutOther" runat="server" class="verify-control"></span>
                        </td>
                        <td>
                            <select ID="ddlIsApplicableProductOther" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeProductOther" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                                <option value="Incremental">Incremental</option>
                                <option value="Percentage">Percentage</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceProductOther" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHEaderProductOther" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Product Type">Product Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td># Characters > 800</td>
                        <td>
                            <select ID="ddlIsApplicableProductCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Yes">Yes</option>
                                <option value="No">No</option>
                            </select>
                        </td>
                        <td>
                            <select ID="ddlChargeTypeProductCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="Fix Amount">Fix Amount</option>
                                <option value="Incremental">Incremental</option>
                                <option value="Percentage">Percentage</option>
                            </select>
                        </td>
                        <td>
                            <input type="text" ID="txtPriceProductCharacter" runat="server" class="verify-control" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderproductCharacter" runat="server" class="verify-control">
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
                <table class="verify-form-table">
                    <tr>
                        <td><b>Base Order Type:</b></td>
                        <td>
                            <select ID="ddlBaseOrderType" runat="server" class="verify-control" onchange="GetOrderTypeInfo(this);">
                                <option value="Select">Select</option>
                                <option value="Commercial">Commercial</option>
                                <option value="Residential">Residential</option>
                            </select>
                        </td>
                    </tr>
                </table>
                <table class="verify-form-table">
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
                                <option value="Order Type">Order Type</option>
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
                                <option value="Order Type">Order Type</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
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
        <tr id="trCharacter" runat="server" style="display: none;">
            <td colspan="3">
                <table class="verify-form-table" border="1">
                    <tr>
                        <th>Parameter Name</th>
                        <th>For Character</th>
                        <th>Price</th>
                        <th>Billing Header</th>
                    </tr>
                    <tr>
                        <td># of Character</td>
                        <td>
                            <input type="text" ID="txtPerCharacter" runat="server" />
                        </td>
                        <td>
                            <input type="text" ID="txtPricePerCharacter" runat="server" />
                        </td>
                        <td>
                            <select ID="ddlBillingHeaderCharacter" runat="server" class="verify-control">
                                <option value="Select">Select</option>
                                <option value="# of Character"># of Character</option>
                            </select></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="tbl379003" runat="server" style="display: none;">
            <td colspan="6">
                <table class="verify-form-table" border="1">
                    <tr>
                        <th>Parameter</th>
                        <th>Charge Type</th>
                        <th>Price</th>
                        <th>Billing Header</th>
                    </tr>
                    <tr>
                        <td>No of Pages</td>
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

                </table>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <button type="button" id="btnShubmitTyping" class="verify-primary-btn" onclick="approveTypingCost()">Approve Costing</button>
            </td>
            <td></td>
        </tr>
    </table>


<div id="standardLoadingOverlay" class="standard-loading-overlay show"><div class="standard-loading-box"><div class="standard-spinner"></div><div class="standard-loading-title">Please wait...</div><div id="standardLoadingMessage" class="standard-loading-text">Loading project cost...</div></div></div>

<script>
function approveTypingCost(){
 var projectId=parseInt(new URLSearchParams(window.location.search).get('ProjectId')||'0',10);
 pageMethod('VerifyProjectCostTyping.aspx/ApproveCosting',{projectId:projectId,loadingMessage:'Approving project costing...'},function(r){showVerifyMessage(r.Message,!r.Success);});
}
$(function(){hideStandardLoading();});
</script></asp:Content>
