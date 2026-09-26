<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="RateRevisionUpdates.aspx.cs" Inherits="BillingOther.Accounts.RateRevisionUpdates" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .rate-page {
            font-family: Bahnschrift,'Segoe UI',sans-serif
        }

        .page-hero {
            background: linear-gradient(110deg,#2f67e8,#148a8d);
            color: #fff;
            border-radius: 18px;
            padding: 20px 26px;
            margin: 18px 8px 14px
        }

            .page-hero .eyebrow {
                font-size: 11px;
                font-weight: 700;
                letter-spacing: 1.4px;
                opacity: .9
            }

            .page-hero h2 {
                font-size: 20px;
                margin: 4px 0 0;
                font-weight: 700
            }

        .panel-card {
            background: #fff;
            border: 1px solid #dde5f0;
            border-radius: 16px;
            margin: 0 8px 16px;
            padding: 20px;
            box-shadow: 0 4px 18px rgba(15,23,42,.04)
        }

        .toolbar {
            display: flex;
            justify-content: flex-end;
            margin-bottom: 14px
        }

        .btn-modern {
            height: 38px;
            border: 1px solid #2563eb;
            border-radius: 8px;
            padding: 0 15px;
            background: #2563eb;
            color: #fff;
            font-weight: 600;
            font-size: 12px;
            cursor: pointer
        }

        .table-wrap {
            width: 100% !important;
            max-width: 100% !important;
            overflow: visible !important;
            display: block
        }

        .rate-table {
            display: table !important;
            width: 100% !important;
            min-width: 100% !important;
            max-width: 100% !important;
            border-collapse: collapse !important;
            table-layout: fixed !important
        }

            .rate-table th {
                background: #f4f7fb;
                color: #102a56;
                font-size: 11px;
                font-weight: 700;
                padding: 10px 9px;
                border: 1px solid #dce3ec;
                white-space: nowrap;
                text-align: center
            }

            .rate-table td {
                font-size: 11px;
                padding: 9px;
                border-bottom: 1px solid #e6ebf2;
                text-align: center;
                vertical-align: middle
            }

            .rate-table tbody tr:hover {
                background: #f8fbff
            }

        .dataTables_wrapper {
            width: 100% !important;
            max-width: 100% !important;
            display: block !important
        }

            .dataTables_wrapper .dataTables_scroll, .dataTables_wrapper .dataTables_scrollHead, .dataTables_wrapper .dataTables_scrollHeadInner, .dataTables_wrapper .dataTables_scrollBody {
                width: 100% !important;
                max-width: 100% !important
            }

            .dataTables_wrapper table.dataTable, #tblRate_wrapper table, #tblRate {
                width: 100% !important;
                min-width: 100% !important;
                max-width: 100% !important;
                margin: 0 !important
            }

            .dataTables_wrapper .dataTables_scrollHeadInner table {
                width: 100% !important
            }

            .dataTables_wrapper .dataTables_scrollBody {
                overflow-x: auto !important
            }

            .dataTables_wrapper .dataTables_length {
                float: left
            }

            .dataTables_wrapper .dataTables_filter {
                float: right
            }

                .dataTables_wrapper .dataTables_filter input, .dataTables_wrapper .dataTables_length select {
                    border: 1px solid #cbd5e1;
                    border-radius: 8px;
                    padding: 7px 10px;
                    background: #fff
                }

            .dataTables_wrapper .dataTables_paginate .paginate_button.current {
                background: #2563eb !important;
                color: #fff !important;
                border-color: #2563eb !important
            }

        .standard-loading-overlay {
            display: none;
            position: fixed;
            inset: 0;
            z-index: 99999;
            background: rgba(15,23,42,.35);
            align-items: center;
            justify-content: center
        }

            .standard-loading-overlay.show {
                display: flex !important
            }

        .standard-loading-box {
            width: 230px !important;
            min-width: 230px !important;
            max-width: 230px !important;
            padding: 20px 18px !important;
            background: #fff;
            border-radius: 10px;
            text-align: center;
            box-shadow: 0 10px 30px rgba(0,0,0,.2)
        }

        .standard-spinner {
            width: 34px;
            height: 34px;
            margin: 0 auto 10px;
            border: 4px solid #e5e7eb;
            border-top-color: #2563eb;
            border-radius: 50%;
            animation: spin .75s linear infinite
        }

        .standard-loading-title {
            font-size: 13px;
            font-weight: 700;
            color: #172554
        }

        .standard-loading-text {
            margin-top: 3px;
            font-size: 11px;
            color: #64748b
        }

        @keyframes spin {
            to {
                transform: rotate(360deg)
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="rate-page">
       
        <div class="panel-card">
            <div class="toolbar">
                <button type="button" id="btnExportRate" class="btn-modern">Export To Excel</button></div>
            <div id="rateTableHost" class="table-wrap">
                <table id="tblRate" class="rate-table" style="width: 100%" width="100%">
                    <thead>
                        <tr>
                            <th>Sr. #</th>
                            <th>Project #</th>
                            <th>Agreement Date</th>
                            <th>Rate Revision Clause</th>
                            <th>Rate Type</th>
                            <th>Latest Rate (In $)</th>
                            <th>Updated On</th>
                            <th>Previous Rate (In $)</th>
                            <th>Previous Rate 2 (In $)</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div>
    <div id="loadingOverlay" class="standard-loading-overlay">
        <div class="standard-loading-box">
            <div class="standard-spinner"></div>
            <div class="standard-loading-title">Please wait...</div>
            <div id="loadingMessage" class="standard-loading-text">Loading rate revisions...</div>
        </div>
    </div>
    <script>
(function(){var dt=null,rows=[];function show(){document.getElementById('loadingOverlay').classList.add('show')}function hide(){document.getElementById('loadingOverlay').classList.remove('show')}function esc(v){return String(v==null?'':v).replace(/[&<>"']/g,function(c){return {'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c];});}function post(){return fetch('RateRevisionUpdates.aspx/GetRateRevisionUpdatesAjax',{method:'POST',headers:{'Content-Type':'application/json; charset=utf-8'},body:'{}'}).then(function(r){if(!r.ok)throw new Error('HTTP '+r.status);return r.json()}).then(function(x){return x.d})}function forceFullWidth(){var host=document.getElementById('rateTableHost'),table=document.getElementById('tblRate');if(!host||!table)return;host.style.width='100%';host.style.maxWidth='100%';table.style.width='100%';table.style.minWidth='100%';table.style.maxWidth='100%';table.setAttribute('width','100%');var wrapper=document.getElementById('tblRate_wrapper');if(wrapper){wrapper.style.width='100%';wrapper.style.maxWidth='100%';wrapper.querySelectorAll('table,.dataTables_scroll,.dataTables_scrollHead,.dataTables_scrollHeadInner,.dataTables_scrollBody').forEach(function(el){el.style.width='100%';el.style.maxWidth='100%';});}if(dt&&dt.columns){try{dt.columns.adjust();}catch(e){}}}function render(data){rows=data||[];if(dt){dt.destroy();dt=null;}var tb=document.querySelector('#tblRate tbody');tb.innerHTML='';rows.forEach(function(r,i){var tr=document.createElement('tr');tr.innerHTML='<td>'+(i+1)+'</td><td>'+esc(r.ProjectName)+'</td><td>'+esc(r.AgreementDate)+'</td><td>'+esc(r.RateRevisionClause)+'</td><td>'+esc(r.Type)+'</td><td>'+esc(r.LatestRate)+'</td><td>'+esc(r.UpdatedOn)+'</td><td>'+esc(r.PreviousRate)+'</td><td>'+esc(r.PreviousRate2)+'</td>';tb.appendChild(tr)});if(window.jQuery&&window.jQuery.fn&&window.jQuery.fn.DataTable){dt=window.jQuery('#tblRate').DataTable({pageLength:25,autoWidth:false,scrollX:false,responsive:false,order:[],drawCallback:function(){setTimeout(forceFullWidth,0);}});setTimeout(forceFullWidth,0);setTimeout(forceFullWidth,100);setTimeout(forceFullWidth,300);}else{forceFullWidth();}}function load(){show();post().then(render).catch(function(e){alert(e.message)}).finally(hide)}document.getElementById('btnExportRate').addEventListener('click',function(){var html='<table><thead>'+document.querySelector('#tblRate thead').innerHTML+'</thead><tbody>';rows.forEach(function(r,i){html+='<tr><td>'+(i+1)+'</td><td>'+esc(r.ProjectName)+'</td><td>'+esc(r.AgreementDate)+'</td><td>'+esc(r.RateRevisionClause)+'</td><td>'+esc(r.Type)+'</td><td>'+esc(r.LatestRate)+'</td><td>'+esc(r.UpdatedOn)+'</td><td>'+esc(r.PreviousRate)+'</td><td>'+esc(r.PreviousRate2)+'</td></tr>'});html+='</tbody></table>';var b=new Blob(['\ufeff'+html],{type:'application/vnd.ms-excel'}),a=document.createElement('a');a.href=URL.createObjectURL(b);a.download='Rate Revision Updates.xls';a.click();URL.revokeObjectURL(a.href)});window.addEventListener('resize',function(){setTimeout(forceFullWidth,50);});window.addEventListener('load',function(){setTimeout(forceFullWidth,100);});if(window.ResizeObserver){new ResizeObserver(function(){forceFullWidth();}).observe(document.getElementById('rateTableHost'));}load()})();
</script>
</asp:Content>
