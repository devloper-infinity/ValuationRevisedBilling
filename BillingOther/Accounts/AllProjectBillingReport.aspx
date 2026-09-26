<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="AllProjectBillingReport.aspx.cs" Inherits="BillingOther.Accounts.AllProjectBillingReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/responsive/2.5.0/css/responsive.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/2.4.2/css/buttons.dataTables.min.css" />

    <style>
        .billing-page { padding: 16px 10px 28px; font-family: Bahnschrift, "Segoe UI", Arial, sans-serif; color: #13213c; }
        .billing-hero { display:flex; align-items:center; justify-content:space-between; gap:16px; padding:20px 26px; border-radius:18px; background:linear-gradient(110deg,#2f64ea 0%,#1b8a8c 100%); color:#fff; margin-bottom:14px; }
        .billing-eyebrow { font-size:11px; letter-spacing:1.4px; font-weight:700; text-transform:uppercase; opacity:.9; }
        .billing-title { margin:5px 0 0; font-size:20px; font-weight:700; }
        .billing-subtitle { margin-top:5px; font-size:12px; opacity:.9; }
        .billing-card { width:100%; box-sizing:border-box; background:#fff; border:1px solid #dce5f1; border-radius:16px; padding:18px 20px; box-shadow:0 6px 20px rgba(15,23,42,.04); }
        .filter-grid { display:grid; grid-template-columns:minmax(190px,1fr) minmax(190px,1fr) auto; gap:14px; align-items:end; }
        .field-label { display:block; font-size:12px; font-weight:700; margin:0 0 7px; color:#203457; }
        .form-control-modern { width:100%; height:40px; box-sizing:border-box; border:1px solid #cdd9e8; border-radius:9px; padding:7px 11px; font-family:inherit; font-size:13px; outline:none; background:#fff; }
        .form-control-modern:focus { border-color:#2f64ea; box-shadow:0 0 0 3px rgba(47,100,234,.10); }
        .action-row { display:flex; gap:8px; flex-wrap:wrap; align-items:center; }
        .btn-modern { height:40px; border:0; border-radius:9px; padding:0 16px; font-family:inherit; font-size:12px; font-weight:700; cursor:pointer; white-space:nowrap; }
        .btn-primary-modern { background:#2f64ea; color:#fff; box-shadow:0 4px 12px rgba(47,100,234,.20); }
        .btn-secondary-modern { background:#fff; color:#203457; border:1px solid #cdd9e8; }
        .table-toolbar { display:flex; justify-content:space-between; align-items:center; margin:16px 0 8px; gap:12px; flex-wrap:wrap; }
        .record-count { font-size:12px; font-weight:600; color:#566987; }
        .table-shell, .table-shell .dataTables_wrapper, .table-shell .dataTables_scroll, .table-shell .dataTables_scrollHead, .table-shell .dataTables_scrollHeadInner, .table-shell .dataTables_scrollBody { width:100% !important; max-width:100% !important; box-sizing:border-box !important; }
        #billingTable, #billingTable.dataTable { width:100% !important; max-width:100% !important; min-width:0 !important; margin:0 !important; table-layout:auto !important; border-collapse:collapse !important; }
        #billingTable thead th { background:#f4f7fb; color:#173056; border:1px solid #dce5f1; padding:9px 8px; font-size:11px; font-weight:700; white-space:normal; vertical-align:middle; }
        #billingTable tbody td { border-bottom:1px solid #e7edf5; padding:8px; font-size:11px; vertical-align:middle; white-space:normal; overflow-wrap:anywhere; }
        #billingTable tbody tr:hover { background:#f8fbff; }
        #billingTable tfoot th { background:#f8fafc; border-top:2px solid #dce5f1; padding:9px 8px; font-size:11px; font-weight:700; }
        .dataTables_wrapper { width:100% !important; }
        .dataTables_wrapper .dataTables_length, .dataTables_wrapper .dataTables_filter { margin-bottom:10px; font-size:11px; }
        .dataTables_wrapper .dataTables_length select, .dataTables_wrapper .dataTables_filter input { border:1px solid #cdd9e8 !important; border-radius:8px !important; min-height:36px; background:#fff !important; }
        .dataTables_wrapper .dataTables_info, .dataTables_wrapper .dataTables_paginate { margin-top:10px; font-size:11px; }
        .dataTables_wrapper .dataTables_paginate .paginate_button { border-radius:7px !important; }
        .dt-buttons { display:none !important; }
        .empty-state { text-align:center; color:#7b8ba5; padding:22px !important; }
        .alert-modern { display:none; margin-bottom:12px; padding:10px 13px; border-radius:9px; font-size:12px; }
        .alert-modern.show { display:block; }
        .alert-danger-modern { background:#fff1f2; color:#b42318; border:1px solid #fecdd3; }
        .alert-success-modern { background:#ecfdf3; color:#027a48; border:1px solid #abefc6; }

        .standard-loading-overlay { display:none; position:fixed; inset:0; z-index:99999; background:rgba(15,23,42,.35); align-items:center; justify-content:center; }
        .standard-loading-overlay.show { display:flex !important; }
        .standard-loading-box { width:230px !important; min-width:230px !important; max-width:230px !important; height:auto !important; padding:20px 18px !important; margin:0 !important; background:#fff; border-radius:10px; box-shadow:0 10px 30px rgba(0,0,0,.20); text-align:center; position:relative; top:auto !important; left:auto !important; transform:none !important; }
        .standard-spinner { width:34px; height:34px; margin:0 auto 10px; border:4px solid #e5e7eb; border-top-color:#2563eb; border-radius:50%; animation:standardSpin .75s linear infinite; }
        .standard-loading-title { font-size:13px; font-weight:600; line-height:18px; color:#172554; }
        .standard-loading-text { margin-top:3px; font-size:11px; line-height:16px; color:#64748b; }
        @keyframes standardSpin { to { transform:rotate(360deg); } }

        @media (max-width:900px) {
            .filter-grid { grid-template-columns:1fr 1fr; }
            .action-row { grid-column:1 / -1; }
        }
        @media (max-width:620px) {
            .filter-grid { grid-template-columns:1fr; }
            .action-row { grid-column:auto; }
            .billing-card { padding:14px; }
            .billing-hero { padding:17px 18px; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="billing-page">
      
        <div id="pageAlert" class="alert-modern"></div>

        <div class="billing-card">
            <div class="filter-grid">
                <div>
                    <label class="field-label" for="txtFromDate">From Date</label>
                    <input type="date" id="txtFromDate" class="form-control-modern" />
                </div>
                <div>
                    <label class="field-label" for="txtToDate">To Date</label>
                    <input type="date" id="txtToDate" class="form-control-modern" />
                </div>
                <div class="action-row">
                    <button type="button" id="btnShowFilter" class="btn-modern btn-primary-modern">Show</button>
                    <button type="button" id="btnExportExcel" class="btn-modern btn-secondary-modern">Export To Excel</button>
                    <button type="button" id="btnExportPdf" class="btn-modern btn-secondary-modern">Export To PDF</button>
                </div>
            </div>

            <div class="table-toolbar">
                <div id="lblRecords" class="record-count">0 record(s)</div>
            </div>

            <div class="table-shell" id="tableShell">
                <table id="billingTable" class="display" cellspacing="0">
                    <thead>
                        <tr>
                            <th>Sr. No.</th>
                            <th>Domain</th>
                            <th>Subdomain</th>
                            <th>Project Number</th>
                            <th>Billing Period</th>
                            <th>Order Count</th>
                            <th>Total Amount</th>
                            <th>Invoice Generated Date</th>
                            <th>Invoice Number</th>
                            <th>Is Invoice Sent To Client?</th>
                            <th>Invoice Delivered Date</th>
                            <th>Invoice Read Date</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                    <tfoot>
                        <tr>
                            <th colspan="5" style="text-align:right">Total</th>
                            <th id="ftOrderCount">0</th>
                            <th id="ftTotalAmount">0.00</th>
                            <th colspan="5"></th>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    </div>

    <div id="loadingOverlay" class="standard-loading-overlay">
        <div class="standard-loading-box">
            <div class="standard-spinner"></div>
            <div class="standard-loading-title">Please wait...</div>
            <div id="loadingMessage" class="standard-loading-text">Loading billing report...</div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/jquery@3.7.1/dist/jquery.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.5.0/js/dataTables.responsive.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.4.2/js/dataTables.buttons.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.10.1/jszip.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.4.2/js/buttons.html5.min.js"></script>

    <script>
        (function () {
            var jq = window.jQuery;
            var table = null;
            var resizeObserver = null;
            var watermarkImageData = null;

            function showLoading(message) {
                document.getElementById('loadingMessage').textContent = message || 'Loading data...';
                document.getElementById('loadingOverlay').classList.add('show');
            }

            function hideLoading() {
                document.getElementById('loadingOverlay').classList.remove('show');
            }

            function showAlert(message, isSuccess) {
                var el = document.getElementById('pageAlert');
                el.textContent = message || '';
                el.className = 'alert-modern show ' + (isSuccess ? 'alert-success-modern' : 'alert-danger-modern');
                window.setTimeout(function () { el.className = 'alert-modern'; }, 5000);
            }

            function formatServerDate(value) {
                if (!value) return '';
                var p = value.split('-');
                if (p.length !== 3) return value;
                var months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
                return p[2] + '-' + months[parseInt(p[1], 10) - 1] + '-' + p[0];
            }

            function escapeHtml(value) {
                var div = document.createElement('div');
                div.textContent = value == null ? '' : String(value);
                return div.innerHTML;
            }

            function parseNumber(value) {
                if (value === null || value === undefined || value === '') return 0;
                var n = parseFloat(String(value).replace(/,/g, ''));
                return isNaN(n) ? 0 : n;
            }

            function forceFullWidth() {
                var shell = document.getElementById('tableShell');
                var tbl = document.getElementById('billingTable');
                if (!shell || !tbl) return;
                var width = shell.clientWidth;
                tbl.style.width = '100%';
                tbl.style.maxWidth = '100%';
                var wrapper = document.getElementById('billingTable_wrapper');
                if (wrapper) {
                    wrapper.style.width = '100%';
                    wrapper.style.maxWidth = '100%';
                    var generated = wrapper.querySelectorAll('.dataTables_scroll,.dataTables_scrollHead,.dataTables_scrollHeadInner,.dataTables_scrollBody');
                    for (var i = 0; i < generated.length; i++) {
                        generated[i].style.width = '100%';
                        generated[i].style.maxWidth = '100%';
                    }
                    var nestedTables = wrapper.querySelectorAll('table');
                    for (var j = 0; j < nestedTables.length; j++) {
                        nestedTables[j].style.width = '100%';
                        nestedTables[j].style.maxWidth = '100%';
                    }
                }
                if (table && table.columns) {
                    table.columns.adjust();
                    if (table.responsive && table.responsive.recalc) table.responsive.recalc();
                }
                if (width > 0) tbl.style.width = width + 'px';
            }

            function createTable() {
                if (!jq || !jq.fn || !jq.fn.DataTable) {
                    showAlert('DataTable library could not be loaded.', false);
                    hideLoading();
                    return;
                }
                table = jq('#billingTable').DataTable({
                    data: [],
                    responsive: false,
                    autoWidth: false,
                    scrollX: false,
                    pageLength: 25,
                    lengthMenu: [[25,50,100,-1],[25,50,100,'All']],
                    dom: 'lfrtipB',
                    buttons: [
                        { extend:'excelHtml5', title:'Billing Summary Report', footer:true },
                        { extend:'pdfHtml5', title:'Billing Summary Report', orientation:'landscape', pageSize:'A3', footer:true, customize:function(doc) {
                            if (watermarkImageData) {
                                doc.background = function(currentPage, pageSize) {
                                    return { image:watermarkImageData, width:260, opacity:0.06, absolutePosition:{ x:(pageSize.width-260)/2, y:(pageSize.height-120)/2 } };
                                };
                            }
                            doc.defaultStyle.fontSize = 7;
                            if (doc.styles && doc.styles.tableHeader) doc.styles.tableHeader.fontSize = 7;
                        } }
                    ],
                    columns: [
                        { data:null, render:function (d,t,r,m) { return m.row + 1; } },
                        { data:'Domain', defaultContent:'' },
                        { data:'Subdomain', defaultContent:'' },
                        { data:'ProjectName', defaultContent:'' },
                        { data:'BillingPeriod', defaultContent:'' },
                        { data:'OrderCount', defaultContent:'' },
                        { data:'TotalAmount', defaultContent:'' },
                        { data:'AddedDate', defaultContent:'' },
                        { data:'InvocieNumber', defaultContent:'' },
                        { data:'InvoiceSent', defaultContent:'' },
                        { data:'DeliveredDate', defaultContent:'' },
                        { data:'ReadDate', defaultContent:'' }
                    ],
                    drawCallback: function () {
                        window.setTimeout(forceFullWidth, 0);
                    },
                    initComplete: function () {
                        forceFullWidth();
                        hideLoading();
                    }
                });
                window.setTimeout(forceFullWidth, 60);
            }

            function bindRows(rows) {
                rows = rows || [];
                var orderTotal = 0;
                var amountTotal = 0;
                for (var i=0; i<rows.length; i++) {
                    orderTotal += parseNumber(rows[i].OrderCount);
                    amountTotal += parseNumber(rows[i].TotalAmount);
                }
                document.getElementById('ftOrderCount').textContent = orderTotal.toLocaleString(undefined,{maximumFractionDigits:2});
                document.getElementById('ftTotalAmount').textContent = amountTotal.toLocaleString(undefined,{minimumFractionDigits:2,maximumFractionDigits:2});
                document.getElementById('lblRecords').textContent = rows.length + ' record(s)';
                table.clear();
                table.rows.add(rows);
                table.draw(false);
                forceFullWidth();
            }

            function preloadWatermark() {
                try {
                    var img = new Image();
                    img.onload = function () {
                        try {
                            var canvas = document.createElement('canvas');
                            canvas.width = img.naturalWidth || img.width;
                            canvas.height = img.naturalHeight || img.height;
                            var ctx = canvas.getContext('2d');
                            ctx.drawImage(img, 0, 0);
                            watermarkImageData = canvas.toDataURL('image/jpeg', 0.85);
                        } catch (e) { watermarkImageData = null; }
                    };
                    img.src = '../Images/InfinityLogo.jpg';
                } catch (e) { watermarkImageData = null; }
            }

            function loadReport() {
                var from = document.getElementById('txtFromDate').value;
                var to = document.getElementById('txtToDate').value;
                if ((from && !to) || (!from && to)) {
                    showAlert('Please select both From Date and To Date.', false);
                    return;
                }
                showLoading('Loading billing report...');
                fetch('AllProjectBillingReport.aspx/GetBillingReport', {
                    method:'POST',
                    headers:{'Content-Type':'application/json; charset=utf-8'},
                    body:JSON.stringify({ fromDate: formatServerDate(from), toDate: formatServerDate(to) })
                })
                .then(function (r) { if (!r.ok) throw new Error('HTTP ' + r.status); return r.json(); })
                .then(function (result) {
                    bindRows(result.d || []);
                    hideLoading();
                })
                .catch(function (err) {
                    hideLoading();
                    showAlert('Unable to load billing report. ' + err.message, false);
                });
            }

            document.addEventListener('DOMContentLoaded', function () {
                showLoading('Loading billing report...');
                preloadWatermark();
                createTable();
                document.getElementById('btnShowFilter').addEventListener('click', loadReport);
                document.getElementById('btnExportExcel').addEventListener('click', function () {
                    if (table) table.button('.buttons-excel').trigger();
                });
                document.getElementById('btnExportPdf').addEventListener('click', function () {
                    if (table) table.button('.buttons-pdf').trigger();
                });
                window.addEventListener('resize', function () { window.setTimeout(forceFullWidth, 80); });
                window.addEventListener('load', function () { window.setTimeout(forceFullWidth, 80); });
                if (window.ResizeObserver) {
                    resizeObserver = new ResizeObserver(function () { window.setTimeout(forceFullWidth, 20); });
                    resizeObserver.observe(document.getElementById('tableShell'));
                }
                loadReport();
            });
        })();
    </script>
</asp:Content>
