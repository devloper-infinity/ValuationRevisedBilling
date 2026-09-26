<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="SummaryReport.aspx.cs" Inherits="BillingOther.Accounts.SummaryReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/responsive/2.5.0/css/responsive.dataTables.min.css" />
    <style>
        .summary-card{background:#fff;border:1px solid #e4e8ef;border-radius:14px;padding:20px;margin-top:16px}
        .filter-row{display:grid;grid-template-columns:90px minmax(180px,1fr) 70px minmax(180px,1fr) auto;gap:14px;align-items:center;margin-bottom:20px}
        .filter-row label{font-weight:600;margin:0}
        .form-control-sm{width:100%;height:38px;padding:6px 10px;border:1px solid #cfd7e6;border-radius:7px;background:#fff}
        .table-wrap{width:100%;max-width:100%;overflow:hidden}
        #tblSummary{width:100%!important;font-size:12px;table-layout:auto}
        #tblSummary th,#tblSummary td{padding:7px 6px;vertical-align:middle;white-space:normal;word-break:break-word}
        #tblSummary_wrapper{width:100%;max-width:100%;overflow:hidden}
        #tblSummary_wrapper .dataTables_filter{margin-bottom:10px}
        #tblSummary_wrapper .dataTables_filter input{max-width:220px;width:180px}
        .action-link{border:0;background:transparent;padding:0;color:#1769d2;cursor:pointer;text-decoration:underline;font:inherit}
        .report-icon{font-size:18px;text-decoration:none}
        .portal-modal{display:none;position:fixed;z-index:10550;inset:0;background:rgba(0,0,0,.45);overflow:auto;padding:30px 15px}
        .portal-modal.show{display:block}
        .portal-modal-dialog{background:#fff;max-width:820px;margin:0 auto;border-radius:8px;box-shadow:0 12px 35px rgba(0,0,0,.25);overflow:hidden}
        .portal-modal-lg{max-width:1100px}
        .portal-modal-header{display:flex;align-items:center;justify-content:space-between;padding:12px 16px;border-bottom:1px solid #e5e7eb;font-weight:600}
        .portal-modal-body{padding:16px;max-height:78vh;overflow:auto}
        .portal-modal-footer{padding:12px 16px;border-top:1px solid #e5e7eb;text-align:right}
        .portal-close{border:0;background:transparent;font-size:24px;line-height:1;cursor:pointer}
        .form-row{display:grid;grid-template-columns:250px 1fr;gap:14px;align-items:center;margin-bottom:12px}
        .viewer-frame{width:100%;height:75vh;border:0}
        .alert-inline{display:none;margin-bottom:12px}
        .standard-loading-overlay{display:none;position:fixed;inset:0;z-index:20000;background:rgba(15,23,42,.38);align-items:center;justify-content:center;backdrop-filter:blur(1px)}
        .standard-loading-overlay.show{display:flex!important}
        .standard-loading-box{width:230px!important;min-width:230px!important;max-width:230px!important;height:auto!important;padding:20px 18px!important;margin:0!important;background:#fff;border-radius:10px;text-align:center;box-shadow:0 10px 30px rgba(0,0,0,.20);position:relative;top:auto!important;left:auto!important;right:auto!important;bottom:auto!important;transform:none!important;float:none!important}
        .standard-spinner{width:34px;height:34px;margin:0 auto 10px;border:4px solid #e5e7eb;border-top-color:#2563eb;border-radius:50%;animation:standardSpin .75s linear infinite}
        .standard-loading-title{margin:0;font-size:13px;font-weight:600;line-height:18px;color:#172554}
        .standard-loading-text{margin-top:3px;font-size:11px;line-height:16px;color:#64748b}
        @keyframes standardSpin{to{transform:rotate(360deg)}}
        .viewer-toolbar{display:flex;gap:8px;justify-content:flex-end;align-items:center}
        @media(max-width:900px){.filter-row{grid-template-columns:1fr}.filter-row label{margin-top:4px}.form-row{grid-template-columns:1fr}}
    </style>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.5.0/js/dataTables.responsive.min.js"></script>
    <script>
        var summaryTable;
        var loadingCount=0;

        function showLoading(message){
            loadingCount++;
            var overlay=document.getElementById('loadingOverlay');
            var msg=document.getElementById('loadingMessage');
            if(msg)msg.textContent=message||'Loading data';
            if(overlay)overlay.classList.add('show');
        }
        function hideLoading(){
            loadingCount=Math.max(0,loadingCount-1);
            if(loadingCount===0){var overlay=document.getElementById('loadingOverlay');if(overlay)overlay.classList.remove('show');}
        }

        function escapeHtml(v){return String(v==null?'':v).replace(/[&<>"']/g,function(c){return {'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c];});}
        function showPortalModal(id){var m=document.getElementById(id);if(m)m.classList.add('show');}
        function hidePortalModal(id){var m=document.getElementById(id);if(m)m.classList.remove('show');}
        function pageMethod(name,payload,loadingText){
            showLoading(loadingText||'Processing request...');
            return fetch('SummaryReport.aspx/'+name,{method:'POST',credentials:'same-origin',headers:{'Content-Type':'application/json; charset=utf-8'},body:JSON.stringify(payload||{})})
                .then(function(r){if(!r.ok)throw new Error('Server request failed.');return r.json();})
                .then(function(x){return x.d;})
                .finally(function(){hideLoading();});
        }
        function showMessage(text,isError){
            var e=document.getElementById('pageMessage');
            e.className='alert-inline alert '+(isError?'alert-danger':'alert-success');
            e.textContent=text;e.style.display='block';
            setTimeout(function(){e.style.display='none';},5000);
        }
        function initYears(){
            var y=document.getElementById('ddlYear');
            var current=(new Date()).getFullYear();
            for(var yr=2019;yr<=current;yr++){var o=document.createElement('option');o.value=yr;o.textContent=yr;y.appendChild(o);}
        }
        function loadSummary(){
            var month=document.getElementById('ddlMonth').value;
            var year=document.getElementById('ddlYear').value;
            document.getElementById('btnShow').disabled=true;
            pageMethod('GetSummaryRows',{month:month,year:year},'Loading summary report...').then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to load report.');
                summaryTable.clear().rows.add(d.Rows||[]).draw();
                document.getElementById('recordCount').textContent=(d.Rows||[]).length+' record(s)';
            }).catch(function(e){showMessage(e.message,true);}).finally(function(){document.getElementById('btnShow').disabled=false;});
        }
        function previewReport(group,billingPeriod){
            var frame=document.getElementById('viewerFrame');
            var download=document.getElementById('btnDownloadReport');
            frame.src='about:blank';
            download.style.display='none';
            download.removeAttribute('href');
            showPortalModal('modalViewer');
            pageMethod('PrepareReportPreview',{groupName:group,billingPeriod:billingPeriod},'Preparing PDF preview...').then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to generate report preview.');
                download.href=d.Url;
                download.setAttribute('download',d.FileName||'Billing Summary Report.pdf');
                download.style.display='inline-block';
                frame.src=d.Url+(d.Url.indexOf('?')>=0?'&':'?')+'_='+new Date().getTime();
            }).catch(function(e){
                closeViewer();
                showMessage(e.message,true);
            });
        }
        function closeViewer(){
            var frame=document.getElementById('viewerFrame');
            frame.onload=null;frame.src='about:blank';
            var download=document.getElementById('btnDownloadReport');
            download.style.display='none';download.removeAttribute('href');
            hidePortalModal('modalViewer');
        }
        function openInvoiceUpdate(group,billingPeriod){
            pageMethod('GetInvoiceForUpdate',{projectNumber:group,billingPeriod:billingPeriod},'Loading invoice details...').then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to load invoice details.');
                document.getElementById('ajaxProjectNumber').value=group;
                document.getElementById('ajaxBillingPeriod').value=billingPeriod;
                document.getElementById('lblProjectPopup').textContent=group;
                document.getElementById('lblInvoiceNoPopup').textContent=billingPeriod;
                document.getElementById('dxInvrecieved').value=d.InvoiceReceivedDate||'';
                document.getElementById('dxInvConfirm').value=d.NoDisputeDate||'';
                document.getElementById('ddlReciep').value=d.CommunicationReceipt||'Select';
                document.getElementById('txtInvRemark').value=d.Remark||'';
                document.getElementById('dxDate').value=d.CompletedDate||'';
                showPortalModal('modalInvoiceUpdate');
            }).catch(function(e){showMessage(e.message,true);});
        }
        function updateInvoiceAjax(){
            var btn=document.getElementById('btnUpdateInvoiceAjax');btn.disabled=true;
            pageMethod('UpdateInvoiceAjax',{
                projectNumber:document.getElementById('ajaxProjectNumber').value,
                billingPeriod:document.getElementById('ajaxBillingPeriod').value,
                invoiceReceivedDate:document.getElementById('dxInvrecieved').value,
                noDisputeDate:document.getElementById('dxInvConfirm').value,
                communicationReceipt:document.getElementById('ddlReciep').value,
                remark:document.getElementById('txtInvRemark').value,
                completedDate:document.getElementById('dxDate').value
            },'Updating invoice...').then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to update invoice.');
                hidePortalModal('modalInvoiceUpdate');showMessage('Details added successfully.',false);loadSummary();
            }).catch(function(e){showMessage(e.message,true);}).finally(function(){btn.disabled=false;});
        }
        function openSendToClient(group,billingPeriod){
            document.getElementById('sendGroup').value=group;document.getElementById('sendPeriod').value=billingPeriod;
            document.getElementById('emailTemplate').innerHTML='<div>Loading...</div>';
            showPortalModal('modalSend');
            pageMethod('GetSendTemplate',{groupName:group,billingPeriod:billingPeriod},'Loading email template...').then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to load email template.');
                document.getElementById('emailTemplate').innerHTML=d.Html||'';
            }).catch(function(e){document.getElementById('emailTemplate').innerHTML='<div class="alert alert-danger">'+escapeHtml(e.message)+'</div>';});
        }
        function sendToClientAjax(){
            var btn=document.getElementById('btnSendClient');btn.disabled=true;
            pageMethod('SendToClientAjax',{groupName:document.getElementById('sendGroup').value,billingPeriod:document.getElementById('sendPeriod').value},'Generating report and sending email...').then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to send email.');
                hidePortalModal('modalSend');showMessage('Email sent successfully.',false);loadSummary();
            }).catch(function(e){showMessage(e.message,true);}).finally(function(){btn.disabled=false;});
        }
        $(function(){
            initYears();
            summaryTable=$('#tblSummary').DataTable({
                data:[],pageLength:25,order:[],autoWidth:false,responsive:true,deferRender:true,
                columns:[
                    {data:null,title:'Sr. No.',orderable:false,searchable:false,render:function(d,t,r,m){return m.row+1;}},
                    {data:'GroupNumber',title:'Project Group'},
                    {data:'BillingPeriod',title:'Billing Period'},
                    {data:'InvoiceOrderCount',title:'Order Count'},
                    {data:'InvoiceTotal',title:'Total Amount'},
                    {data:null,title:'Report',orderable:false,searchable:false,render:function(d,t,r){return '<button type="button" class="action-link report-icon" title="Preview PDF" onclick="previewReport(\''+String(r.GroupNumber||'').replace(/'/g,"\\'")+'\',\''+String(r.BillingPeriod||'').replace(/'/g,"\\'")+'\')">&#128196;</button>'; }},
                    {data:'AddedDate',title:'Invoice Generated Date'},
                    {data:'InvoiceSent',title:'Is Invoice Sent To Client?'},
                    {data:'DeliveredDate',title:'Invoice Delivered Date'},
                    {data:'ReadDate',title:'Invoice Read Date'},
                    {data:null,title:'Send To Client',orderable:false,searchable:false,render:function(d,t,r){return '<button type="button" class="action-link" onclick="openSendToClient(\''+String(r.GroupNumber||'').replace(/'/g,"\\'")+'\',\''+String(r.BillingPeriod||'').replace(/'/g,"\\'")+'\')">Send To Client</button>'; }},
                    {data:null,title:'Update Invoice',orderable:false,searchable:false,render:function(d,t,r){return '<button type="button" class="action-link" onclick="openInvoiceUpdate(\''+String(r.GroupNumber||'').replace(/'/g,"\\'")+'\',\''+String(r.BillingPeriod||'').replace(/'/g,"\\'")+'\')">Update Invoice</button>'; }}
                ],
                columnDefs:[{targets:[0,3,4,5,6,7,8,9,10,11],className:'dt-center'}]
            });
            $('#tblSummary').on('draw.dt',function(){if(summaryTable&&summaryTable.responsive)summaryTable.responsive.recalc();});
            loadSummary();
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="loadingOverlay" class="standard-loading-overlay" aria-live="polite" aria-busy="true">
        <div class="standard-loading-box">
            <div class="standard-spinner"></div>
            <div class="standard-loading-title">Please wait...</div>
            <div id="loadingMessage" class="standard-loading-text">Loading data</div>
        </div>
    </div>
    <div id="pageMessage" class="alert-inline"></div>
    <div class="summary-card">
        <div class="filter-row">
            <label for="ddlMonth">Month:</label>
            <select id="ddlMonth" class="form-control-sm">
                <option value="Select">Select</option><option>January</option><option>February</option><option>March</option><option>April</option><option>May</option><option>June</option><option>July</option><option>August</option><option>September</option><option>October</option><option>November</option><option>December</option>
            </select>
            <label for="ddlYear">Year:</label>
            <select id="ddlYear" class="form-control-sm"><option value="Select">Select</option></select>
            <button id="btnShow" type="button" class="btn btn-primary" onclick="loadSummary()">Show</button>
        </div>
        <div id="recordCount" style="margin-bottom:10px">0 record(s)</div>
        <div class="table-wrap"><table id="tblSummary" class="display responsive nowrap" style="width:100%"></table></div>
    </div>

    <div id="modalViewer" class="portal-modal">
        <div class="portal-modal-dialog portal-modal-lg">
            <div class="portal-modal-header">
                <span>PDF Preview</span>
                <div class="viewer-toolbar">
                    <a id="btnDownloadReport" class="btn btn-primary btn-sm" style="display:none" href="#">Download PDF</a>
                    <button type="button" class="portal-close" onclick="closeViewer()">&times;</button>
                </div>
            </div>
            <div class="portal-modal-body" style="padding:0"><iframe id="viewerFrame" class="viewer-frame" src="about:blank"></iframe></div>
        </div>
    </div>

    <div id="modalInvoiceUpdate" class="portal-modal">
        <div class="portal-modal-dialog">
            <div class="portal-modal-header"><span>Update Invoice</span><button type="button" class="portal-close" onclick="hidePortalModal('modalInvoiceUpdate')">&times;</button></div>
            <div class="portal-modal-body">
                <input type="hidden" id="ajaxProjectNumber" /><input type="hidden" id="ajaxBillingPeriod" />
                <div class="form-row"><label>Project Group:</label><span id="lblProjectPopup"></span></div>
                <div class="form-row"><label>Billing Period:</label><span id="lblInvoiceNoPopup"></span></div>
                <div class="form-row"><label for="dxInvrecieved">Invoice received by client date:</label><input id="dxInvrecieved" type="date" class="form-control-sm" /></div>
                <div class="form-row"><label for="dxInvConfirm">No dispute confirmation date:</label><input id="dxInvConfirm" type="date" class="form-control-sm" /></div>
                <div class="form-row"><label for="ddlReciep">Communication regarding receipt:</label><select id="ddlReciep" class="form-control-sm"><option value="Select">Select</option><option value="By Mail">By Mail</option><option value="By Call">By Call</option></select></div>
                <div class="form-row"><label for="txtInvRemark">Remark:</label><textarea id="txtInvRemark" class="form-control-sm" rows="4" style="height:auto;resize:vertical"></textarea></div>
                <div class="form-row"><label for="dxDate">Completed Date:</label><input id="dxDate" type="date" class="form-control-sm" /></div>
            </div>
            <div class="portal-modal-footer"><button id="btnUpdateInvoiceAjax" type="button" class="btn btn-primary" onclick="updateInvoiceAjax()">Update Invoice</button></div>
        </div>
    </div>

    <div id="modalSend" class="portal-modal">
        <div class="portal-modal-dialog portal-modal-lg">
            <div class="portal-modal-header"><span>Send To Client</span><button type="button" class="portal-close" onclick="hidePortalModal('modalSend')">&times;</button></div>
            <div class="portal-modal-body"><input type="hidden" id="sendGroup" /><input type="hidden" id="sendPeriod" /><div id="emailTemplate"></div></div>
            <div class="portal-modal-footer"><button id="btnSendClient" type="button" class="btn btn-primary" onclick="sendToClientAjax()">Send Mail</button></div>
        </div>
    </div>
</asp:Content>
