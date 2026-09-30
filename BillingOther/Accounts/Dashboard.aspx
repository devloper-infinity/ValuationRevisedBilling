<%@ Page Title="Billing Dashboard" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="BillingOther.Accounts.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../css/accounts-dashboard.css?v=1" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="dashboard-filters dashboard-card">
        <div class="dashboard-filter-grid">
            <label>Year<select id="filterYear"></select></label><label>Month<select id="filterMonth"></select></label><label>Domain<select id="filterDomain"></select></label><label>Project<select id="filterProject"></select></label>
            <label>Billing Cycle<select id="filterCycle"><option value="">All</option>
                <option>Monthly</option>
                <option>Bi-Monthly</option>
                <option>Deal Wise</option>
            </select></label>
            <label>Billing Status<select id="filterStatus"><option value="">All</option>
                <option>Yet to Bill</option>
                <option>Invoice Generated</option>
                <option>Sent to Client</option>
                <option>Partially Billed</option>
                <option>Overdue</option>
                <option>No Activity</option>
            </select></label>
            <label>Client<select id="filterClient"></select></label><div class="dashboard-filter-actions">
                <button type="button" id="applyFilters" class="btn btn-primary">Apply</button>
                <button type="button" id="resetFilters" class="btn btn-default">Reset</button></div>
        </div>
    </section>
    <div id="dashboardMessage" class="dashboard-message" hidden></div>
    <section id="kpiGrid" class="kpi-grid"></section>
    <section class="dashboard-chart-grid">
        <article class="dashboard-card">
            <header>
                <h2>Billing Amount by Domain</h2>
            </header>
            <canvas id="domainChart"></canvas>
        </article>
        <article class="dashboard-card">
            <header>
                <h2>Yet to Bill vs Sent to Client</h2>
            </header>
            <canvas id="comparisonChart"></canvas>
        </article>
        <article class="dashboard-card">
            <header>
                <h2>Monthly Billing Trend</h2>
            </header>
            <canvas id="trendChart"></canvas>
        </article>
        <article class="dashboard-card">
            <header>
                <h2>Billing Status Distribution</h2>
            </header>
            <canvas id="statusChart"></canvas>
        </article>
    </section>
    <section class="dashboard-card dashboard-section">
        <header>
            <div><span class="section-eyebrow">Management summary</span><h2>Domain-wise Billing Summary</h2>
            </div>
        </header>
        <div class="table-responsive">
            <table id="domainTable" class="table dashboard-table">
                <thead>
                    <tr>
                        <th>Domain</th>
                        <th>Projects</th>
                        <th>Orders/Loans</th>
                        <th>Total Amount</th>
                        <th>Yet to Bill</th>
                        <th>Invoice Generated</th>
                        <th>Sent to Client</th>
                        <th>Pending Amount</th>
                        <th>Billing %</th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>
        </div>
    </section>
    <section class="dashboard-card dashboard-section">
        <header>
            <div><span class="section-eyebrow">Operational view</span><h2>Project-wise Status</h2>
            </div>
        </header>
        <div class="table-responsive">
            <table id="projectTable" class="table dashboard-table">
                <thead>
                    <tr>
                        <th>Domain</th>
                        <th>Project #</th>
                        <th>Project Name</th>
                        <th>Billing Cycle</th>
                        <th>Current Billing Period</th>
                        <th>Orders/Loans</th>
                        <th>Amount</th>
                        <th>Yet to Bill</th>
                        <th>Invoice Generated</th>
                        <th>Sent to Client</th>
                        <th>Pending</th>
                        <th>Last Billing Date</th>
                        <th>Status</th>
                        <th>View Details</th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>
        </div>
    </section>
    <section class="dashboard-card dashboard-section">
        <header>
            <div><span class="section-eyebrow">Period analysis</span><h2>Month-wise / Billing Period Trend</h2>
            </div>
        </header>
        <div class="table-responsive">
            <table id="trendTable" class="table dashboard-table">
                <thead>
                    <tr>
                        <th>Month/Billing Period</th>
                        <th>Order Count</th>
                        <th>Billing Amount</th>
                        <th>Yet to Bill</th>
                        <th>Sent to Client</th>
                        <th>Pending</th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>
        </div>
    </section>
    <section class="dashboard-card dashboard-section attention-section">
        <header>
            <div><span class="section-eyebrow">Requires action</span><h2>Management Attention</h2>
            </div>
            <span id="attentionCount" class="attention-count">0</span></header>
        <div class="table-responsive">
            <table id="attentionTable" class="table dashboard-table">
                <thead>
                    <tr>
                        <th>Priority</th>
                        <th>Issue</th>
                        <th>Domain</th>
                        <th>Project</th>
                        <th>Billing Period</th>
                        <th>Amount</th>
                        <th>Expected Date</th>
                        <th>Status</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>
        </div>
    </section>
    <div id="projectModal" class="dashboard-modal" aria-hidden="true">
        <div class="dashboard-modal-dialog">
            <header>
                <div><span class="section-eyebrow">Billing details</span><h2 id="detailTitle">Project Details</h2>
                </div>
                <button type="button" id="closeDetails">&times;</button></header>
            <div class="table-responsive">
                <table class="table dashboard-table">
                    <thead>
                        <tr>
                            <th>Billing Period</th>
                            <th>Orders/Loans</th>
                            <th>Invoice #</th>
                            <th>Invoice Date</th>
                            <th>Billing Amount</th>
                            <th>Sent Date</th>
                            <th>Status</th>
                            <th>Pending Amount</th>
                            <th>Last Action/Update</th>
                        </tr>
                    </thead>
                    <tbody id="detailRows"></tbody>
                </table>
            </div>
        </div>
    </div>
<%--    <script src="../js/accounts-dashboard.js?v=1" defer></script>--%>
</asp:Content>
