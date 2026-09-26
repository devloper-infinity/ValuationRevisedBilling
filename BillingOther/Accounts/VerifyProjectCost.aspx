<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="VerifyProjectCost.aspx.cs" Inherits="BillingOther.Accounts.VerifyProjectCost" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Accounts/VerifyCostMigration.css") %>" rel="stylesheet" />
    <script src="<%= ResolveUrl("~/Accounts/VerifyCostMigration.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server" class="verify-alert" style="display:none"></div>

    <section class="verify-summary-card">
        <div class="verify-summary-header">
            <div>
                <div class="verify-eyebrow">INFINITY IPS BILLING</div>
                <h2>Verify Project Cost</h2>
                <p>Review configured billing parameters before approval.</p>
            </div>
            <a class="verify-back-btn" href="VerifyCostMaster.aspx" onclick="showStandardLoading('Returning to project list...')"><i class="fa fa-arrow-left"></i> Back</a>
        </div>
        <div id="summaryContainer" runat="server"></div>
    </section>

    <section id="dvGen" runat="server" class="verify-section" style="display:none">
        <h3 class="verify-section-title">Billing Parameters</h3>
        <div class="verify-table-wrap">
            <table class="verify-form-table" id="tblBillingParams">
                <thead><tr><th>Sr. #</th><th>Parameter Name</th><th>Is Applicable?</th><th>Additional Charge</th><th>Charge Type</th><th>Price by BDM</th><th>Latest Price</th></tr></thead>
                <tbody id="billingParamsBody" runat="server"></tbody>
            </table>
        </div>
        <div class="verify-actions"><button type="button" class="verify-primary-btn" onclick="approveBillingParameters()"><i class="fa fa-check"></i> Approve</button></div>
    </section>

    <section id="dvFTE" runat="server" class="verify-section" style="display:none">
        <h3 class="verify-section-title">FTE Costing</h3>
        <table class="verify-form-table" id="tblOtherFTE" runat="server">
            <tr><th id="tdRate" runat="server">Hourly Rate</th><td><input type="text" id="txtHourlyRate" runat="server" class="verify-control" readonly="readonly" /></td><td><button type="button" class="verify-primary-btn" onclick="approveFteCost()">Approve Costing</button></td></tr>
        </table>
        <table class="verify-form-table" id="tbl861007" runat="server" style="display:none">
            <tr><th>Rate for First 1200 ballots</th><td><input type="text" id="txtfirst1200Rate" runat="server" class="verify-control" readonly="readonly" /></td></tr>
            <tr><th>Rate for Additional ballots</th><td><input type="text" id="txtAdditionalBallotsRate" runat="server" class="verify-control" readonly="readonly" /></td></tr>
            <tr><th>Wire Transfer Charges</th><td><input type="text" id="txtWireTransferCharges" runat="server" class="verify-control" readonly="readonly" /></td></tr>
            <tr><td></td><td><button type="button" class="verify-primary-btn" onclick="approveFteCost()">Approve Costing</button></td></tr>
        </table>
    </section>

    <div id="standardLoadingOverlay" class="standard-loading-overlay show"><div class="standard-loading-box"><div class="standard-spinner"></div><div class="standard-loading-title">Please wait...</div><div id="standardLoadingMessage" class="standard-loading-text">Loading project cost...</div></div></div>

    <script>
        function collectBillingRows(){
            var rows=[];
            document.querySelectorAll('#tblBillingParams tbody tr[data-id]').forEach(function(tr){
                rows.push({
                    Id:parseInt(tr.getAttribute('data-id'),10),
                    Comment:tr.querySelector('.js-comment').value,
                    Additional:tr.querySelector('.js-additional').value,
                    ChargeType:tr.querySelector('.js-charge-type').value,
                    Remark:tr.querySelector('.js-remark').value
                });
            });
            return rows;
        }
        function approveBillingParameters(){
            var projectId=parseInt(new URLSearchParams(window.location.search).get('ProjectId')||'0',10);
            pageMethod('VerifyProjectCost.aspx/ApproveBillingParameters',{projectId:projectId,rows:collectBillingRows(),loadingMessage:'Approving billing parameters...'},function(r){showVerifyMessage(r.Message,!r.Success);});
        }
        function approveFteCost(){
            var projectId=parseInt(new URLSearchParams(window.location.search).get('ProjectId')||'0',10);
            pageMethod('VerifyProjectCost.aspx/ApproveFteCost',{projectId:projectId,loadingMessage:'Approving FTE costing...'},function(r){showVerifyMessage(r.Message,!r.Success);});
        }
        $(function(){hideStandardLoading();});
    </script>
</asp:Content>
