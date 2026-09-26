<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ProjectwisePriceMaster.aspx.cs" Inherits="BillingOther.Accounts.ProjectwisePriceMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .pricing-page {
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

        .filter-row {
            display: grid;
            grid-template-columns: minmax(220px,360px) auto 1fr;
            gap: 12px;
            align-items: end
        }

        .field-label {
            display: block;
            font-size: 12px;
            font-weight: 700;
            color: #172554;
            margin-bottom: 6px
        }

        .form-control-modern {
            width: 100%;
            height: 40px;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            padding: 0 12px;
            background: #fff;
            color: #0f172a
        }

        .action-row {
            display: flex;
            gap: 8px;
            flex-wrap: wrap
        }

        .btn-modern {
            height: 40px;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            padding: 0 15px;
            background: #fff;
            color: #1e3a5f;
            font-weight: 600;
            font-size: 12px;
            cursor: pointer
        }

            .btn-modern.primary {
                background: #2563eb;
                border-color: #2563eb;
                color: #fff
            }

        .table-wrap {
            width: 100% !important;
            max-width: 100% !important;
            overflow: visible !important;
            margin-top: 16px;
            display: block
        }

        .pricing-table {
            display: table !important;
            width: 100% !important;
            min-width: 100% !important;
            max-width: 100% !important;
            border-collapse: collapse !important;
            table-layout: fixed !important
        }

            .pricing-table th {
                background: #f4f7fb;
                color: #102a56;
                font-size: 11px;
                font-weight: 700;
                padding: 10px 9px;
                border: 1px solid #dce3ec;
                white-space: nowrap
            }

            .pricing-table td {
                font-size: 11px;
                padding: 9px;
                border-bottom: 1px solid #e6ebf2;
                vertical-align: middle
            }

            .pricing-table tbody tr:hover {
                background: #f8fbff
            }

        .group-row td {
            background: #eaf1ff !important;
            color: #173b73;
            font-weight: 700;
            border-top: 1px solid #cddcf6;
            border-bottom: 1px solid #cddcf6
        }

        .group-toggle {
            cursor: pointer;
            user-select: none
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

            .dataTables_wrapper table.dataTable, #tblPricing_wrapper table, #tblPricing {
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

            .dataTables_wrapper .dataTables_filter, .dataTables_wrapper .dataTables_length {
                margin-bottom: 10px
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

            .dataTables_wrapper .dataTables_info {
                padding-top: 14px
            }

            .dataTables_wrapper .dataTables_paginate {
                padding-top: 8px
            }

                .dataTables_wrapper .dataTables_paginate .paginate_button {
                    border-radius: 7px !important;
                    border: 1px solid transparent !important;
                    padding: 5px 9px !important
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

        @media(max-width:800px) {
            .filter-row {
                grid-template-columns: 1fr
            }

            .action-row {
                justify-content: flex-start
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="pricing-page">
     
        <div class="panel-card">
            <div class="filter-row">
                <div>
                    <label class="field-label" for="ddlDomainClient">Domain</label><select id="ddlDomainClient" class="form-control-modern"><option value="">Select</option>
                    </select></div>
                <div>
                    <button type="button" id="btnShowClient" class="btn-modern primary">Show</button></div>
                <div class="action-row" style="justify-content: flex-end">
                    <button type="button" id="btnCollapseClient" class="btn-modern">Collapse All</button>
                    <button type="button" id="btnExpandClient" class="btn-modern">Expand All</button>
                    <button type="button" id="btnExportClient" class="btn-modern">Export To Excel</button></div>
            </div>
            <div id="pricingTableHost" class="table-wrap">
                <table id="tblPricing" class="pricing-table" style="width: 100%" width="100%">
                    <thead>
                        <tr>
                            <th>Project Number</th>
                            <th>Parameter Name</th>
                            <th>Is Applicable?</th>
                            <th>Additional Charge</th>
                            <th>Charge Type</th>
                            <th>Latest Price</th>
                            <th>Previous Price</th>
                            <th>Price Updated On</th>
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
            <div id="loadingMessage" class="standard-loading-text">Loading project pricing...</div>
        </div>
    </div>
    <script>
(function(){
  var dt=null, rows=[];
  function showLoading(m){document.getElementById('loadingMessage').textContent=m||'Loading...';document.getElementById('loadingOverlay').classList.add('show');}
  function hideLoading(){document.getElementById('loadingOverlay').classList.remove('show');}
  function post(method,data){return fetch('ProjectwisePriceMaster.aspx/'+method,{method:'POST',headers:{'Content-Type':'application/json; charset=utf-8'},body:JSON.stringify(data||{})}).then(function(r){if(!r.ok)throw new Error('HTTP '+r.status);return r.json();}).then(function(x){return x.d;});}
  function esc(v){return String(v==null?'':v).replace(/[&<>"']/g,function(c){return {'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c];});}
  function ensureDataTable(cb){if(window.jQuery&&window.jQuery.fn&&window.jQuery.fn.DataTable){cb();return;} cb();}
  function forceFullWidth(){
    var host=document.getElementById('pricingTableHost');
    var table=document.getElementById('tblPricing');
    if(!host||!table)return;
    host.style.width='100%'; host.style.maxWidth='100%';
    table.style.width='100%'; table.style.minWidth='100%'; table.style.maxWidth='100%';
    table.setAttribute('width','100%');
    var wrapper=document.getElementById('tblPricing_wrapper');
    if(wrapper){
      wrapper.style.width='100%'; wrapper.style.maxWidth='100%';
      wrapper.querySelectorAll('table,.dataTables_scroll,.dataTables_scrollHead,.dataTables_scrollHeadInner,.dataTables_scrollBody').forEach(function(el){el.style.width='100%';el.style.maxWidth='100%';});
    }
    if(dt&&dt.columns){try{dt.columns.adjust();}catch(e){}}
  }
  function render(data){rows=data||[]; if(dt){dt.destroy();dt=null;} var tb=document.querySelector('#tblPricing tbody');tb.innerHTML='';var last='';rows.forEach(function(r){var p=r.ProjectName||'';if(p!==last){var g=document.createElement('tr');g.className='group-row';g.setAttribute('data-group',p);g.innerHTML='<td colspan="8"><span class="group-toggle">&#9660; '+esc(p)+'</span></td>';tb.appendChild(g);last=p;}var tr=document.createElement('tr');tr.className='detail-row';tr.setAttribute('data-group',p);tr.innerHTML='<td>'+esc(p)+'</td><td>'+esc(r.IBP_ParameterName)+'</td><td>'+esc(r.IBV_Comment)+'</td><td>'+esc(r.IBV_Additional)+'</td><td>'+esc(r.IBV_ChargeType)+'</td><td>'+esc(r.LatestPrice)+'</td><td>'+esc(r.PreviousPrice)+'</td><td>'+esc(r.UpdatedDate)+'</td>';tb.appendChild(tr);});
    ensureDataTable(function(){if(window.jQuery&&window.jQuery.fn&&window.jQuery.fn.DataTable){dt=window.jQuery('#tblPricing').DataTable({pageLength:25,autoWidth:false,scrollX:false,responsive:false,order:[],columnDefs:[{targets:0,visible:false}],drawCallback:function(){setTimeout(forceFullWidth,0);}});setTimeout(forceFullWidth,0);setTimeout(forceFullWidth,100);setTimeout(forceFullWidth,300);}else{forceFullWidth();}});
  }
  function loadDomains(){showLoading('Loading domains...');post('GetDomains',{}).then(function(data){var s=document.getElementById('ddlDomainClient');(data||[]).forEach(function(x){var o=document.createElement('option');o.value=x.Value;o.textContent=x.Text;s.appendChild(o);});}).catch(alert).finally(hideLoading);}
  function loadPricing(){var id=document.getElementById('ddlDomainClient').value;if(!id){render([]);return;}showLoading('Loading project pricing...');post('GetPricing',{domainId:parseInt(id,10)}).then(render).catch(function(e){alert(e.message);}).finally(hideLoading);}
  document.getElementById('btnShowClient').addEventListener('click',loadPricing);
  document.getElementById('btnCollapseClient').addEventListener('click',function(){document.querySelectorAll('.detail-row').forEach(function(x){x.style.display='none';});document.querySelectorAll('.group-toggle').forEach(function(x){x.innerHTML=x.innerHTML.replace('▼','▶');});});
  document.getElementById('btnExpandClient').addEventListener('click',function(){document.querySelectorAll('.detail-row').forEach(function(x){x.style.display='';});document.querySelectorAll('.group-toggle').forEach(function(x){x.innerHTML=x.innerHTML.replace('▶','▼');});});
  document.getElementById('tblPricing').addEventListener('click',function(e){var t=e.target.closest('.group-toggle');if(!t)return;var g=t.closest('tr').getAttribute('data-group'),hidden=true;document.querySelectorAll('.detail-row').forEach(function(x){if(x.getAttribute('data-group')===g){hidden=x.style.display!=='none';x.style.display=hidden?'none':'';}});t.innerHTML=t.innerHTML.replace(hidden?'▼':'▶',hidden?'▶':'▼');});
  document.getElementById('btnExportClient').addEventListener('click',function(){var html='<table><thead>'+document.querySelector('#tblPricing thead').innerHTML+'</thead><tbody>';rows.forEach(function(r){html+='<tr><td>'+esc(r.ProjectName)+'</td><td>'+esc(r.IBP_ParameterName)+'</td><td>'+esc(r.IBV_Comment)+'</td><td>'+esc(r.IBV_Additional)+'</td><td>'+esc(r.IBV_ChargeType)+'</td><td>'+esc(r.LatestPrice)+'</td><td>'+esc(r.PreviousPrice)+'</td><td>'+esc(r.UpdatedDate)+'</td></tr>';});html+='</tbody></table>';var b=new Blob(['\ufeff'+html],{type:'application/vnd.ms-excel'}),a=document.createElement('a');a.href=URL.createObjectURL(b);a.download='Projectwise Price Details.xls';a.click();URL.revokeObjectURL(a.href);});
  window.addEventListener('resize',function(){setTimeout(forceFullWidth,50);});
  window.addEventListener('load',function(){setTimeout(forceFullWidth,100);});
  if(window.ResizeObserver){new ResizeObserver(function(){forceFullWidth();}).observe(document.getElementById('pricingTableHost'));}
  loadDomains();
})();
</script>
</asp:Content>
