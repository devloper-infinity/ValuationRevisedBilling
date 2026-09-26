<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ProjectGroupMaster.aspx.cs" Inherits="BillingOther.Accounts.ProjectGroupMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
    <style type="text/css">
        .pgm-page { padding: 0; font-family: Bahnschrift, 'Segoe UI', Arial, sans-serif; color: #14213d; }
        .pgm-header {
            display:flex; align-items:center; justify-content:space-between; gap:16px;
            padding:20px 24px; margin-bottom:14px; border-radius:18px;
            background:linear-gradient(110deg,#2f66e8 0%,#168a8a 100%); color:#fff;
            box-shadow:0 10px 28px rgba(31,55,94,.10);
        }
        .pgm-kicker { font-size:11px; font-weight:700; letter-spacing:1.3px; text-transform:uppercase; opacity:.88; }
        .pgm-title { margin:4px 0 0; font-size:20px; font-weight:700; line-height:1.2; }
        .pgm-subtitle { margin-top:4px; font-size:12px; opacity:.9; }

        .pgm-card { background:#fff; border:1px solid #e3eaf4; border-radius:16px; box-shadow:0 8px 24px rgba(28,48,74,.05); margin-bottom:16px; overflow:hidden; }
        .pgm-card-head { display:flex; align-items:center; justify-content:space-between; padding:16px 20px 12px; border-bottom:1px solid #edf1f6; }
        .pgm-card-title { margin:0; font-size:14px; font-weight:700; color:#172554; }
        .pgm-card-note { font-size:11px; color:#64748b; margin-top:2px; }
        .pgm-card-body { padding:18px 20px; }

        .pgm-form-grid { display:grid; grid-template-columns:minmax(240px,.9fr) minmax(360px,1.55fr) auto; gap:18px; align-items:start; }
        .pgm-action-field { padding-top:24px; display:flex; align-items:flex-start; }
        .pgm-field { min-width:0; }
        .pgm-field-group-number { max-width:100%; }
        .pgm-field label { display:block; font-size:11px; font-weight:700; color:#334155; margin-bottom:7px; }
        .pgm-control {
            width:100%; min-height:38px; border:1px solid #ccd7e6; border-radius:9px;
            padding:8px 11px; font-size:12px; color:#172554; background:#fff; outline:none;
            transition:border-color .15s ease, box-shadow .15s ease;
        }
        .pgm-control:focus { border-color:#2f66e8; box-shadow:0 0 0 3px rgba(47,102,232,.11); }
        select.pgm-control[multiple] { height:126px; min-height:126px; padding:5px 6px; overflow-y:auto; }
        select.pgm-control[multiple] option { padding:7px 8px; border-radius:5px; }

        .pgm-btn { border:0; border-radius:9px; min-height:38px; padding:0 17px; font-size:12px; font-weight:700; cursor:pointer; transition:all .15s ease; }
        .pgm-btn-primary { background:#2f66e8; color:#fff; box-shadow:0 5px 14px rgba(47,102,232,.20); }
        .pgm-btn-primary:hover { background:#2459d4; transform:translateY(-1px); }
        .pgm-btn:disabled { opacity:.65; cursor:not-allowed; transform:none; }

        .pgm-alert { display:none; margin:0 0 14px; padding:11px 14px; border-radius:9px; font-size:12px; font-weight:600; }
        .pgm-alert-success { display:block; color:#166534; background:#ecfdf3; border:1px solid #bbf7d0; }
        .pgm-alert-error { display:block; color:#991b1b; background:#fef2f2; border:1px solid #fecaca; }

        .pgm-table-shell { width:100% !important; max-width:100% !important; overflow:visible; box-sizing:border-box; }
        .pgm-table-scroll { width:100% !important; max-width:100% !important; overflow-x:auto; box-sizing:border-box; }
        #projectGroupTable,
        #projectGroupTable.dataTable,
        .pgm-table-scroll table.dataTable {
            width:100% !important; min-width:100% !important; max-width:100% !important;
            display:table !important; margin:0 !important; border-collapse:separate; border-spacing:0;
            table-layout:fixed !important; box-sizing:border-box !important;
        }
        #projectGroupTable th, #projectGroupTable td { box-sizing:border-box; }
        #projectGroupTable th:nth-child(1), #projectGroupTable td:nth-child(1) { width:7% !important; }
        #projectGroupTable th:nth-child(2), #projectGroupTable td:nth-child(2) { width:18% !important; }
        #projectGroupTable th:nth-child(3), #projectGroupTable td:nth-child(3) { width:24% !important; }
        #projectGroupTable th:nth-child(4), #projectGroupTable td:nth-child(4) { width:23% !important; }
        #projectGroupTable th:nth-child(5), #projectGroupTable td:nth-child(5) { width:28% !important; }
        #projectGroupTable thead th {
            background:#f5f8fc; color:#172554; font-size:11px; font-weight:700; white-space:nowrap;
            border-bottom:1px solid #dce5f1; padding:11px 10px !important;
        }
        #projectGroupTable tbody td { font-size:11px; color:#1e293b; padding:10px !important; border-bottom:1px solid #edf1f6; vertical-align:middle; }
        #projectGroupTable tbody tr:hover td { background:#f8fbff; }
        #projectGroupTable tbody tr:last-child td { border-bottom:0; }

        div.dataTables_wrapper { width:100% !important; max-width:100% !important; display:block !important; box-sizing:border-box !important; }
        div.dataTables_wrapper::after { content:''; display:block; clear:both; }
        div.dataTables_wrapper table.dataTable { width:100% !important; min-width:100% !important; max-width:100% !important; display:table !important; }
        div.dataTables_wrapper .dataTables_length, div.dataTables_wrapper .dataTables_filter { margin:0 0 14px; font-size:11px; color:#334155; }
        div.dataTables_wrapper .dataTables_filter input, div.dataTables_wrapper .dataTables_length select {
            border:1px solid #ccd7e6 !important; border-radius:8px !important; min-height:35px; padding:6px 9px !important; background:#fff !important;
        }
        div.dataTables_wrapper .dataTables_filter input { min-width:220px; }
        div.dataTables_wrapper .dataTables_info { font-size:11px; color:#64748b; padding-top:15px; }
        div.dataTables_wrapper .dataTables_paginate { padding-top:10px; }
        div.dataTables_wrapper .dataTables_paginate .paginate_button {
            border:1px solid #d9e2ef !important; border-radius:7px !important; background:#fff !important;
            color:#334155 !important; font-size:11px; padding:5px 9px !important; margin-left:4px !important;
        }
        div.dataTables_wrapper .dataTables_paginate .paginate_button.current { background:#2f66e8 !important; color:#fff !important; border-color:#2f66e8 !important; }
        div.dataTables_wrapper .dataTables_paginate .paginate_button:hover { background:#eef4ff !important; color:#1d4ed8 !important; border-color:#bfd0ee !important; }

        .standard-loading-overlay { display:none; position:fixed; inset:0; z-index:99999; background:rgba(15,23,42,.30); align-items:center; justify-content:center; }
        .standard-loading-overlay.show { display:flex !important; }
        .standard-loading-box { width:230px; padding:20px 18px; background:#fff; border-radius:11px; text-align:center; box-shadow:0 14px 38px rgba(0,0,0,.22); }
        .standard-spinner { width:34px; height:34px; margin:0 auto 10px; border:4px solid #e5e7eb; border-top-color:#2563eb; border-radius:50%; animation:pgmSpin .75s linear infinite; }
        .standard-loading-title { font-size:13px; font-weight:700; color:#172554; }
        .standard-loading-text { margin-top:3px; font-size:11px; color:#64748b; }
        @keyframes pgmSpin { to { transform:rotate(360deg); } }

        @media (max-width:900px) {
            .pgm-form-grid { grid-template-columns:1fr; }
            .pgm-action-field { padding-top:0; }
            .pgm-btn { width:100%; }
            .pgm-header { padding:17px 18px; }
            .pgm-card-body { padding:15px; }
            div.dataTables_wrapper .dataTables_filter input { min-width:150px; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="pgm-page">
        <div id="dvError" class="pgm-alert"></div>

        <section class="pgm-header">
            <div>
                <div class="pgm-kicker">Infinity IPS Billing</div>
                <h2 class="pgm-title">Project Group Master</h2>
                <div class="pgm-subtitle">Configure project groups and review existing mappings.</div>
            </div>
        </section>

        <section class="pgm-card">
            <div class="pgm-card-head">
                <div>
                    <h3 class="pgm-card-title">Add Project Group</h3>
                    <div class="pgm-card-note">Enter the group number and select one or more projects.</div>
                </div>
            </div>
            <div class="pgm-card-body">
                <div class="pgm-form-grid">
                    <div class="pgm-field pgm-field-group-number">
                        <label for="txtGroupNumber">Group Number</label>
                        <input type="text" id="txtGroupNumber" class="pgm-control" maxlength="100" autocomplete="off" />
                    </div>
                    <div class="pgm-field">
                        <label for="ddlProjectNumber">Project Number</label>
                        <select id="ddlProjectNumber" class="pgm-control" multiple="multiple"></select>
                    </div>
                    <div class="pgm-action-field">
                        <button type="button" id="btnSubmit" class="pgm-btn pgm-btn-primary">Save Configuration</button>
                    </div>
                </div>
            </div>
        </section>

        <section class="pgm-card">
            <div class="pgm-card-head">
                <div>
                    <h3 class="pgm-card-title">Configured Project Groups</h3>
                    <div class="pgm-card-note">Existing project-to-group mappings.</div>
                </div>
            </div>
            <div class="pgm-card-body">
                <div class="pgm-table-shell">
                    <div class="pgm-table-scroll">
                        <table id="projectGroupTable" class="display pgm-datatable" cellspacing="0" style="width:100%">
                            <colgroup>
                                <col style="width:7%" />
                                <col style="width:18%" />
                                <col style="width:24%" />
                                <col style="width:23%" />
                                <col style="width:28%" />
                            </colgroup>
                            <thead>
                                <tr>
                                    <th>Sr. #</th>
                                    <th>Group Number</th>
                                    <th>Project Number</th>
                                    <th>Added By</th>
                                    <th>Added Date / Time</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div id="loadingOverlay" class="standard-loading-overlay">
        <div class="standard-loading-box">
            <div class="standard-spinner"></div>
            <div class="standard-loading-title">Please wait...</div>
            <div id="loadingMessage" class="standard-loading-text">Loading project groups...</div>
        </div>
    </div>

    <script type="text/javascript">
        (function () {
            'use strict';

            var tableInstance = null;

            function byId(id) { return document.getElementById(id); }
            function showLoading(message) {
                byId('loadingMessage').textContent = message || 'Please wait...';
                byId('loadingOverlay').classList.add('show');
            }
            function hideLoading() { byId('loadingOverlay').classList.remove('show'); }
            function showMessage(message, ok) {
                var box = byId('dvError');
                box.className = 'pgm-alert ' + (ok ? 'pgm-alert-success' : 'pgm-alert-error');
                box.textContent = message || '';
                box.style.display = 'block';
                window.setTimeout(function () { box.style.display = 'none'; }, 5000);
            }
            function htmlEncode(value) {
                var div = document.createElement('div');
                div.textContent = value == null ? '' : String(value);
                return div.innerHTML;
            }
            function postWebMethod(method, payload) {
                return fetch('ProjectGroupMaster.aspx/' + method, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: JSON.stringify(payload || {})
                }).then(function (response) {
                    if (!response.ok) throw new Error('Request failed (' + response.status + ').');
                    return response.json();
                }).then(function (json) { return json.d; });
            }
            function loadScript(src) {
                return new Promise(function (resolve, reject) {
                    var s = document.createElement('script');
                    s.src = src; s.onload = resolve; s.onerror = reject;
                    document.head.appendChild(s);
                });
            }
            function ensureDataTables() {
                var promise = Promise.resolve();
                if (!window.jQuery) promise = loadScript('https://code.jquery.com/jquery-3.7.1.min.js');
                return promise.then(function () {
                    if (window.jQuery && !window.jQuery.fn.DataTable) {
                        return loadScript('https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js');
                    }
                });
            }
            function forceTableFullWidth() {
                var table = byId('projectGroupTable');
                if (!table) return;
                var scroll = table.closest('.pgm-table-scroll');
                var shell = table.closest('.pgm-table-shell');
                var wrapper = table.closest('.dataTables_wrapper');

                [shell, scroll, wrapper].forEach(function (el) {
                    if (!el) return;
                    el.style.setProperty('width', '100%', 'important');
                    el.style.setProperty('max-width', '100%', 'important');
                    el.style.setProperty('box-sizing', 'border-box', 'important');
                });

                table.style.setProperty('display', 'table', 'important');
                table.style.setProperty('width', '100%', 'important');
                table.style.setProperty('min-width', '100%', 'important');
                table.style.setProperty('max-width', '100%', 'important');
                table.style.setProperty('table-layout', 'fixed', 'important');

                if (tableInstance) tableInstance.columns.adjust();
            }

            function initDataTable() {
                if (!window.jQuery || !window.jQuery.fn.DataTable) {
                    forceTableFullWidth();
                    return;
                }
                if (tableInstance) {
                    tableInstance.destroy();
                    tableInstance = null;
                }
                tableInstance = window.jQuery('#projectGroupTable').DataTable({
                    autoWidth: false,
                    responsive: false,
                    scrollX: false,
                    pageLength: 25,
                    lengthMenu: [[10,25,50,100],[10,25,50,100]],
                    order: [],
                    columnDefs: [
                        { targets: 0, width: '7%' },
                        { targets: 1, width: '18%' },
                        { targets: 2, width: '24%' },
                        { targets: 3, width: '23%' },
                        { targets: 4, width: '28%' }
                    ],
                    language: { search: 'Search:', lengthMenu: 'Show _MENU_ entries' },
                    initComplete: function () {
                        forceTableFullWidth();
                        window.setTimeout(forceTableFullWidth, 50);
                        window.setTimeout(forceTableFullWidth, 250);
                    },
                    drawCallback: function () {
                        forceTableFullWidth();
                    }
                });
                forceTableFullWidth();
            }
            function renderProjects(projects) {
                var select = byId('ddlProjectNumber');
                select.innerHTML = '';
                (projects || []).forEach(function (p) {
                    var option = document.createElement('option');
                    option.value = p.ProjectId;
                    option.textContent = p.ProjectName;
                    select.appendChild(option);
                });
            }
            function renderGroups(rows) {
                if (tableInstance) {
                    tableInstance.destroy();
                    tableInstance = null;
                }
                var tbody = byId('projectGroupTable').querySelector('tbody');
                var html = '';
                (rows || []).forEach(function (r, index) {
                    html += '<tr>' +
                        '<td>' + (index + 1) + '</td>' +
                        '<td>' + htmlEncode(r.GroupNumber) + '</td>' +
                        '<td>' + htmlEncode(r.ProjectName) + '</td>' +
                        '<td>' + htmlEncode(r.AddedByName) + '</td>' +
                        '<td>' + htmlEncode(r.AddedDate) + '</td>' +
                        '</tr>';
                });
                tbody.innerHTML = html;
                ensureDataTables().then(initDataTable).catch(function () { forceTableFullWidth(); });
            }
            function loadPage() {
                showLoading('Loading project groups...');
                Promise.all([postWebMethod('GetProjects', {}), postWebMethod('GetGroups', {})])
                    .then(function (result) {
                        renderProjects(result[0]);
                        renderGroups(result[1]);
                    })
                    .catch(function (err) { showMessage(err.message || 'Unable to load project groups.', false); })
                    .then(hideLoading);
            }
            function saveConfiguration() {
                var groupNumber = byId('txtGroupNumber').value.trim();
                var projectIds = Array.prototype.slice.call(byId('ddlProjectNumber').selectedOptions).map(function (o) { return o.value; });
                if (!groupNumber) { showMessage('Please enter group number.', false); byId('txtGroupNumber').focus(); return; }
                if (!projectIds.length) { showMessage('Please select at least one project.', false); byId('ddlProjectNumber').focus(); return; }

                var btn = byId('btnSubmit');
                btn.disabled = true;
                showLoading('Saving configuration...');
                postWebMethod('SaveConfiguration', { groupNumber: groupNumber, projectIds: projectIds })
                    .then(function (result) {
                        if (!result || !result.Success) throw new Error(result && result.Message ? result.Message : 'Unable to save configuration.');
                        showMessage(result.Message || 'Configuration saved successfully.', true);
                        byId('txtGroupNumber').value = '';
                        Array.prototype.forEach.call(byId('ddlProjectNumber').options, function (o) { o.selected = false; });
                        return postWebMethod('GetGroups', {});
                    })
                    .then(function (rows) { if (rows) renderGroups(rows); })
                    .catch(function (err) { showMessage(err.message || 'Unable to save configuration.', false); })
                    .then(function () { btn.disabled = false; hideLoading(); });
            }

            document.addEventListener('DOMContentLoaded', function () {
                byId('btnSubmit').addEventListener('click', saveConfiguration);
                loadPage();
            });

            window.addEventListener('resize', function () { window.setTimeout(forceTableFullWidth, 80); });
            window.addEventListener('load', function () { window.setTimeout(forceTableFullWidth, 120); });
        })();
    </script>
</asp:Content>
