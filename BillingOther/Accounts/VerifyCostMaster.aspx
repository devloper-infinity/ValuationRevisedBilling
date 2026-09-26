<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="VerifyCostMaster.aspx.cs" Inherits="BillingOther.Accounts.VerifyCostMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Accounts/VerifyCostMigration.css") %>" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
    <script src="<%= ResolveUrl("~/Accounts/VerifyCostMigration.js") %>"></script>

    <style>
        /* Verify Cost Master - full-width DataTable */
        .verify-page-card { padding: 20px 22px; width: 100%; box-sizing: border-box; }

        #tabNewPrice,
        #tabModifiedPrice,
        #tabNewPrice .verify-table-wrap,
        #tabModifiedPrice .verify-table-wrap,
        #tabNewPrice .dataTables_wrapper,
        #tabModifiedPrice .dataTables_wrapper {
            width: 100% !important;
            max-width: none !important;
            box-sizing: border-box !important;
        }

        #tabNewPrice .verify-table-wrap,
        #tabModifiedPrice .verify-table-wrap {
            padding: 14px;
            overflow-x: auto;
            border: 1px solid #dfe6f1;
            border-radius: 12px;
            background: #fff;
        }

        .dataTables_wrapper .dataTables_length,
        .dataTables_wrapper .dataTables_filter {
            margin-bottom: 14px;
            color: #334155;
            font-size: 12px;
            font-weight: 600;
        }

        .dataTables_wrapper .dataTables_length select {
            min-width: 72px;
            height: 36px;
            margin: 0 6px;
            padding: 5px 28px 5px 10px !important;
            border: 1px solid #cbd5e1 !important;
            border-radius: 8px !important;
            background: #fff !important;
        }

        .dataTables_wrapper .dataTables_filter input {
            width: 230px;
            height: 36px;
            margin-left: 8px !important;
            padding: 6px 10px !important;
            border: 1px solid #cbd5e1 !important;
            border-radius: 8px !important;
            box-shadow: none !important;
        }

        /* Critical: never allow DataTables or a hidden tab calculation to leave a pixel-width table. */
        table.verify-datatable,
        table.verify-datatable.dataTable,
        #tabNewPrice table.dataTable,
        #tabModifiedPrice table.dataTable {
            width: 100% !important;
            min-width: 100% !important;
            max-width: none !important;
            margin: 0 !important;
            table-layout: auto !important;
            border-collapse: separate !important;
            border-spacing: 0 !important;
            font-size: 11.5px !important;
        }

        table.verify-datatable thead th {
            padding: 10px 10px !important;
            background: #f4f7fb !important;
            color: #172554 !important;
            border-right: 1px solid #e5eaf2 !important;
            border-bottom: 1px solid #dbe3ef !important;
            font-size: 11px !important;
            font-weight: 700 !important;
            white-space: nowrap !important;
            vertical-align: middle !important;
        }

        table.verify-datatable tbody td {
            padding: 9px 10px !important;
            color: #273449 !important;
            border-right: 1px solid #eef2f7 !important;
            border-bottom: 1px solid #edf1f6 !important;
            white-space: normal !important;
            vertical-align: middle !important;
        }

        table.verify-datatable tbody tr:nth-child(even) td { background: #fbfcfe; }
        table.verify-datatable tbody tr:hover td { background: #f1f6ff !important; }


        table.verify-datatable .verify-action-btn {
            min-width: 64px;
            padding: 6px 10px;
            border-radius: 7px;
            font-size: 11px;
        }

        .dataTables_wrapper .dataTables_info { padding-top: 14px !important; color:#64748b !important; font-size:11px; }
        .dataTables_wrapper .dataTables_paginate { padding-top: 9px !important; }
        .dataTables_wrapper .dataTables_paginate .paginate_button {
            min-width: 32px; margin-left: 3px !important; padding: 5px 9px !important;
            border: 1px solid transparent !important; border-radius: 7px !important;
        }
        .dataTables_wrapper .dataTables_paginate .paginate_button.current,
        .dataTables_wrapper .dataTables_paginate .paginate_button.current:hover {
            color:#fff !important; border-color:#2563eb !important; background:#2563eb !important;
        }

        /* DataTables float controls must not affect table width. */
        .dataTables_wrapper { width:100% !important; max-width:100% !important; display:block !important; box-sizing:border-box !important; }
        .dataTables_wrapper::after { content:""; display:block; clear:both; }

        @media (max-width: 900px) {
            .dataTables_wrapper .dataTables_filter input { width: 170px; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server" class="verify-alert" style="display:none"></div>

    <section class="verify-page-card">
        <div class="verify-card-header">
            <div>
                <div class="verify-eyebrow">INFINITY IPS BILLING</div>
                <h2>Verify Project Price</h2>
                <p>Review new and modified project pricing before approval.</p>
            </div>
        </div>

        <div class="verify-tabs" role="tablist">
            <button type="button" class="verify-tab active" data-target="#tabNewPrice">
                <i class="fa fa-rupee"></i><span>New Price</span>
            </button>
            <button type="button" class="verify-tab" data-target="#tabModifiedPrice">
                <i class="fa fa-history"></i><span>Modified Price</span>
            </button>
        </div>

        <div id="tabNewPrice" class="verify-tab-panel active">
            <div class="verify-table-wrap">
                <table id="tblNewPrice" class="display verify-datatable" style="width:100%">
                    <thead>
                        <tr>
                            <th>View</th>
                            <th>Sr. #</th>
                            <th>Project #</th>
                            <th>Domain</th>
                            <th>Start Date</th>
                            <th>Billing Cycle</th>
                            <th>BDM</th>
                            <th>CRM</th>
                            <th>Unit Head</th>
                            <th>Added By</th>
                            <th>Added Date</th>
                        </tr>
                    </thead>
                    <tbody id="newPriceBody" runat="server"></tbody>
                </table>
            </div>
        </div>

        <div id="tabModifiedPrice" class="verify-tab-panel">
            <div class="verify-table-wrap">
                <table id="tblModifiedPrice" class="display verify-datatable" style="width:100%">
                    <thead>
                        <tr>
                            <th>View</th>
                            <th>Sr. #</th>
                            <th>Project #</th>
                            <th>Domain</th>
                            <th>Start Date</th>
                            <th>Billing Cycle</th>
                            <th>BDM</th>
                            <th>CRM</th>
                            <th>Unit Head</th>
                            <th>Added By</th>
                            <th>Added Date</th>
                        </tr>
                    </thead>
                    <tbody id="modifiedPriceBody" runat="server"></tbody>
                </table>
            </div>
        </div>
    </section>

    <div id="standardLoadingOverlay" class="standard-loading-overlay show">
        <div class="standard-loading-box">
            <div class="standard-spinner"></div>
            <div class="standard-loading-title">Please wait...</div>
            <div id="standardLoadingMessage" class="standard-loading-text">Loading project pricing...</div>
        </div>
    </div>

    <script>
        function sizeVerifyTableToContainer(selector) {
            var jq = window.jQuery;
            var tableEl = document.querySelector(selector);
            if (!tableEl) return;

            var wrap = tableEl.closest('.verify-table-wrap');
            var panel = tableEl.closest('.verify-tab-panel');
            var card = tableEl.closest('.verify-page-card');

            if (panel) {
                panel.style.width = '100%';
                panel.style.maxWidth = '100%';
            }
            if (wrap) {
                wrap.style.display = 'block';
                wrap.style.width = '100%';
                wrap.style.maxWidth = '100%';
                wrap.style.boxSizing = 'border-box';
            }

            /* Use the actual available pixel width. This avoids DataTables retaining
               a narrow width calculated from hidden/auto-sized columns. */
            var available = wrap ? wrap.clientWidth : (card ? card.clientWidth : 0);
            if (available > 0) {
                tableEl.style.setProperty('width', available + 'px', 'important');
                tableEl.style.setProperty('min-width', available + 'px', 'important');
                tableEl.style.setProperty('max-width', available + 'px', 'important');
            }

            if (jq && jq.fn && jq.fn.DataTable && jq.fn.DataTable.isDataTable(selector)) {
                var api = jq(selector).DataTable();
                var dtWrap = jq(selector).closest('.dataTables_wrapper');
                dtWrap.css({ width: '100%', maxWidth: '100%', display: 'block' });

                api.columns.adjust();

                window.setTimeout(function () {
                    var currentWidth = wrap ? wrap.clientWidth : available;
                    if (currentWidth > 0) {
                        tableEl.style.setProperty('width', currentWidth + 'px', 'important');
                        tableEl.style.setProperty('min-width', currentWidth + 'px', 'important');
                        tableEl.style.setProperty('max-width', currentWidth + 'px', 'important');
                    }
                    api.columns.adjust();
                }, 50);

                window.setTimeout(function () {
                    var currentWidth = wrap ? wrap.clientWidth : available;
                    if (currentWidth > 0) {
                        tableEl.style.setProperty('width', currentWidth + 'px', 'important');
                        tableEl.style.setProperty('min-width', currentWidth + 'px', 'important');
                        tableEl.style.setProperty('max-width', currentWidth + 'px', 'important');
                    }
                    api.columns.adjust().draw(false);
                }, 250);
            }
        }

        function initVerifyCostMasterTable(selector) {
            var jq = window.jQuery;
            if (!jq || !jq.fn || !jq.fn.DataTable || !jq(selector).length) return;

            if (!jq.fn.DataTable.isDataTable(selector)) {
                jq(selector).DataTable({
                    pageLength: 25,
                    lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'All']],
                    autoWidth: false,
                    scrollX: false,
                    responsive: false,
                    order: [],
                    columnDefs: [
                        { targets: 0, width: '6%' },
                        { targets: 1, width: '5%' },
                        { targets: 2, width: '9%' },
                        { targets: 3, width: '10%' },
                        { targets: 4, width: '11%' },
                        { targets: 5, width: '10%' },
                        { targets: 6, width: '6%' },
                        { targets: 7, width: '6%' },
                        { targets: 8, width: '8%' },
                        { targets: 9, width: '15%' },
                        { targets: 10, width: '14%' }
                    ],
                    language: {
                        search: 'Search:',
                        emptyTable: 'No records available'
                    },
                    initComplete: function () {
                        sizeVerifyTableToContainer(selector);
                    },
                    drawCallback: function () {
                        sizeVerifyTableToContainer(selector);
                    }
                });
            }

            sizeVerifyTableToContainer(selector);
        }

        function initVisibleVerifyTable() {
            var activePanel = document.querySelector('.verify-tab-panel.active');
            if (!activePanel) return;
            var table = activePanel.querySelector('table.verify-datatable');
            if (table) initVerifyCostMasterTable('#' + table.id);
        }

        document.addEventListener('DOMContentLoaded', function () {
            initVerifyTabs();

            ensureDataTables(function () {
                initVisibleVerifyTable();
                hideStandardLoading();
            });

            var tabButtons = document.querySelectorAll('.verify-tab');
            Array.prototype.forEach.call(tabButtons, function (btn) {
                btn.addEventListener('click', function () {
                    window.setTimeout(function () {
                        var target = btn.getAttribute('data-target');
                        var panel = target ? document.querySelector(target) : null;
                        var table = panel ? panel.querySelector('table.verify-datatable') : null;
                        if (table) initVerifyCostMasterTable('#' + table.id);
                    }, 30);
                });
            });

            window.addEventListener('resize', function () {
                window.setTimeout(initVisibleVerifyTable, 80);
            });

            window.addEventListener('load', function () {
                window.setTimeout(initVisibleVerifyTable, 120);
            });

            window.setTimeout(hideStandardLoading, 1500);
        });
    </script>
</asp:Content>
