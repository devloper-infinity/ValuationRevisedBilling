<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="SendToClient.aspx.cs" Inherits="BillingOther.Accounts.SendToClient" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/2.4.2/css/buttons.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/responsive/2.5.0/css/responsive.dataTables.min.css" />
    <style>
        .portal-modal{display:none;position:fixed;z-index:10550;inset:0;background:rgba(0,0,0,.45);overflow:auto;padding:30px 15px}
        .portal-modal.show{display:block}
        .portal-modal-dialog{background:#fff;max-width:900px;margin:0 auto;border-radius:6px;box-shadow:0 12px 35px rgba(0,0,0,.25);overflow:hidden}
        .portal-modal-lg{max-width:1100px}
        .portal-modal-header{display:flex;align-items:center;justify-content:space-between;padding:12px 16px;border-bottom:1px solid #ddd;font-weight:600}
        .portal-modal-body{padding:16px;max-height:75vh;overflow:auto}
        .portal-close{border:0;background:transparent;font-size:24px;line-height:1;cursor:pointer}
        .form-row{display:flex;gap:14px;align-items:center;flex-wrap:wrap;margin-bottom:12px}
        .form-row label{font-weight:600;min-width:230px}
        .form-control-sm{height:32px;padding:5px 8px;border:1px solid #ccc;border-radius:4px}
        .table-wrap{width:100%;max-width:100%;overflow-x:auto}
        #tblClientDetails{width:100%!important;font-size:12px}
        #tblClientDetails th,#tblClientDetails td{padding:6px 7px;vertical-align:middle;white-space:normal;word-break:break-word}
        #tblClientDetails_wrapper{width:100%;max-width:100%;overflow:hidden}
        #tblClientDetails_wrapper .dataTables_filter{margin-bottom:10px}
        #tblClientDetails_wrapper .dataTables_filter input{max-width:230px;width:180px}
        .dataTables_wrapper .dataTables_scroll{max-width:100%}
        .report-toolbar{display:flex;gap:8px;align-items:center;margin-bottom:12px}
        .excel-preview{max-height:68vh;overflow:auto;border:1px solid #e5e7eb}
        .excel-preview table{border-collapse:collapse;width:max-content;min-width:100%;font-size:12px}
        .excel-preview th,.excel-preview td{border:1px solid #d9dee7;padding:6px 8px;white-space:nowrap}
        .excel-preview th{position:sticky;top:0;background:#f5f7fb;z-index:1}
        #tblRemarks{width:100%!important}
        .viewer-frame{width:100%;height:75vh;border:0}
        .hidden-server-button{display:none!important}
        .action-link{cursor:pointer}

        /* Standard small centered loading popup */
        .standard-loading-overlay{display:none;position:fixed;inset:0;z-index:10650;background:rgba(15,23,42,.35);align-items:center;justify-content:center}
        .standard-loading-overlay.show{display:flex!important}
        .standard-loading-box{width:230px!important;min-width:230px!important;max-width:230px!important;height:auto!important;padding:20px 18px!important;margin:0!important;background:#fff;border-radius:10px;box-shadow:0 10px 30px rgba(0,0,0,.20);text-align:center;position:relative;top:auto!important;left:auto!important;transform:none!important}
        .standard-spinner{width:34px;height:34px;margin:0 auto 10px;border:4px solid #e5e7eb;border-top-color:#2563eb;border-radius:50%;animation:standardSpin .75s linear infinite}
        .standard-loading-title{margin:0;font-size:13px;font-weight:600;line-height:18px;color:#172554}
        .standard-loading-text{margin-top:3px;font-size:11px;line-height:16px;color:#64748b}
        @keyframes standardSpin{to{transform:rotate(360deg)}}
    </style>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.5.0/js/dataTables.responsive.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.4.2/js/dataTables.buttons.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.10.1/jszip.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.4.2/js/buttons.html5.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/xlsx@0.18.5/dist/xlsx.full.min.js"></script>

    <script>
        function showPortalModal(id){var m=document.getElementById(id);if(m)m.classList.add('show');}
        function hidePortalModal(id){var m=document.getElementById(id);if(m)m.classList.remove('show');}
        function HideLabel(){setTimeout(function(){var e=document.getElementById('<%= dvError.ClientID %>');if(e)e.style.display='none';},5000);}

        function setServerKey(key){document.getElementById('<%= hfActionKey.ClientID %>').value=key;}
        function loadServerModal(type,key){
            setServerKey(key);
            var id='';
            if(type==='invoice') id='<%= btnLoadInvoiceModal.ClientID %>';
            else if(type==='send') id='<%= btnLoadSendModal.ClientID %>';
            else if(type==='remark') id='<%= btnLoadRemarkModal.ClientID %>';
            else if(type==='test') id='<%= btnLoadTestEmailModal.ClientID %>';
            else if(type==='preview') id='<%= btnLoadEmailPreviewModal.ClientID %>';
            if(id) document.getElementById(id).click();
        }
        var activeReportObjectUrl = null;
        function showLoading(message){
            var t=document.getElementById('loadingMessage');
            if(t)t.textContent=message||'Loading data...';
            var o=document.getElementById('loadingOverlay');
            if(o)o.classList.add('show');
        }
        function hideLoading(){
            var o=document.getElementById('loadingOverlay');
            if(o)o.classList.remove('show');
        }
        function revokeReportObjectUrl(){
            if(activeReportObjectUrl){
                try{URL.revokeObjectURL(activeReportObjectUrl);}catch(e){}
                activeReportObjectUrl=null;
            }
        }
        function getDownloadName(response,type){
            var cd=response.headers.get('content-disposition')||'';
            var m=cd.match(/filename\*?=(?:UTF-8''|")?([^";]+)/i);
            var name=m&&m[1]?decodeURIComponent(m[1].replace(/"/g,'').trim()):'';
            if(!name)name=type==='excel'?'Billing Report.xlsx':'Billing Report.pdf';
            return name;
        }
        function openReportPreview(type,url){
            var title=document.getElementById('reportModalTitle');
            var frame=document.getElementById('viewerFrame');
            var excel=document.getElementById('excelPreview');
            var loading=document.getElementById('reportLoading');
            var download=document.getElementById('reportDownload');

            revokeReportObjectUrl();
            frame.src='about:blank';
            frame.style.display='none';
            excel.style.display='none';
            excel.innerHTML='';
            loading.style.display='block';
            download.style.display='none';
            download.removeAttribute('href');
            download.removeAttribute('download');
            title.textContent=type==='excel'?'Excel Preview':'PDF Preview';
            showPortalModal('modalViewer');
            showLoading(type==='excel'?'Preparing Excel preview...':'Preparing PDF preview...');

            fetch(url,{credentials:'same-origin'})
                .then(function(r){
                    if(!r.ok)throw new Error('Unable to load report. HTTP '+r.status);
                    var fileName=getDownloadName(r,type);
                    if(type==='pdf'){
                        return r.blob().then(function(blob){return {blob:blob,fileName:fileName};});
                    }
                    return r.arrayBuffer().then(function(buf){return {buffer:buf,fileName:fileName};});
                })
                .then(function(result){
                    if(type==='pdf'){
                        var pdfBlob=result.blob.type==='application/pdf'?result.blob:new Blob([result.blob],{type:'application/pdf'});
                        activeReportObjectUrl=URL.createObjectURL(pdfBlob);
                        frame.src=activeReportObjectUrl;
                        frame.style.display='block';
                        download.href=activeReportObjectUrl;
                        download.download=result.fileName||'Billing Report.pdf';
                    }else{
                        var wb=XLSX.read(result.buffer,{type:'array'});
                        var ws=wb.Sheets[wb.SheetNames[0]];
                        excel.innerHTML=XLSX.utils.sheet_to_html(ws,{id:'excelPreviewTable'});
                        excel.style.display='block';
                        var excelBlob=new Blob([result.buffer],{type:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
                        activeReportObjectUrl=URL.createObjectURL(excelBlob);
                        download.href=activeReportObjectUrl;
                        download.download=result.fileName||'Billing Report.xlsx';
                    }
                    download.style.display='inline-block';
                    loading.style.display='none';
                    hideLoading();
                })
                .catch(function(err){
                    hideLoading();
                    loading.style.display='none';
                    excel.style.display='block';
                    excel.innerHTML='<div class="alert alert-danger">'+escapeHtml(err.message)+'</div>';
                });
        }
        function closeViewer(){
            document.getElementById('viewerFrame').src='about:blank';
            document.getElementById('excelPreview').innerHTML='';
            revokeReportObjectUrl();
            hideLoading();
            hidePortalModal('modalViewer');
        }
        function escapeHtml(v){return String(v==null?'':v).replace(/[&<>"']/g,function(c){return {'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c];});}
        function pageMethod(name,payload){
            return fetch('SendToClient.aspx/'+name,{method:'POST',credentials:'same-origin',headers:{'Content-Type':'application/json; charset=utf-8'},body:JSON.stringify(payload||{})})
                .then(function(r){if(!r.ok)throw new Error('Server request failed.');return r.json();})
                .then(function(x){return x.d;});
        }
        function openInvoiceUpdate(key){
            showLoading('Loading invoice details...');
            pageMethod('GetInvoiceForUpdate',{key:key}).then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to load invoice.');
                document.getElementById('ajaxInvoiceKey').value=key;
                document.getElementById('lblProjectPopup').textContent=d.ProjectNumber||'';
                document.getElementById('lblInvoiceNoPopup').textContent=d.BillingPeriod||'';
                document.getElementById('dxInvrecieved').value=d.InvoiceReceivedDate||'';
                document.getElementById('dxInvConfirm').value=d.NoDisputeDate||'';
                document.getElementById('ddlReciep').value=d.CommunicationReceipt||'Select';
                document.getElementById('txtInvRemark').value=d.Remark||'';
                document.getElementById('dxDate').value=d.CompletedDate||'';
                showPortalModal('modalInvoiceUpdate');
                hideLoading();
            }).catch(function(e){hideLoading();alert(e.message);});
        }
        function updateInvoiceAjax(){
            var btn=document.getElementById('btnUpdateInvoiceAjax');
            btn.disabled=true;
            showLoading('Updating invoice...');
            pageMethod('UpdateInvoiceAjax',{
                key:document.getElementById('ajaxInvoiceKey').value,
                invoiceReceivedDate:document.getElementById('dxInvrecieved').value,
                noDisputeDate:document.getElementById('dxInvConfirm').value,
                communicationReceipt:document.getElementById('ddlReciep').value,
                remark:document.getElementById('txtInvRemark').value,
                completedDate:document.getElementById('dxDate').value
            }).then(function(d){
                if(!d||!d.Success)throw new Error(d&&d.Message?d.Message:'Unable to update invoice.');
                hidePortalModal('modalInvoiceUpdate');
                var msg=document.getElementById('<%= dvError.ClientID %>');
                msg.className='alert alert-success background-success';
                msg.innerHTML='Details added successfully.';
                msg.style.display='block';
                HideLabel();
                hideLoading();
            }).catch(function(e){hideLoading();alert(e.message);}).then(function(){btn.disabled=false;},function(){btn.disabled=false;});
        }
        function updateInvoiceNumber(){
            var t=document.getElementById('<%= lblInvoiceNo.ClientID %>');
            var c=document.getElementById('<%= chkInvoiceNo.ClientID %>');
            if(t){t.disabled=!c.checked;if(c.checked)t.focus();}
        }
        (function(){
            var jq = window['jQuery'];
            if (!jq) { return; }
            jq(function(){
            var table=jq('#tblClientDetails').DataTable({
                pageLength:25,
                order:[],
                autoWidth:false,
                responsive:true,
                dom:'Bfrtip',
                buttons:[
                    {extend:'excelHtml5',title:'Billing Summary Report',exportOptions:{columns:[0,2,3,4,5,6,7,8,11,12,13]}},
                    {extend:'pdfHtml5',title:'Billing Summary Report',orientation:'landscape',pageSize:'A4',exportOptions:{columns:[0,2,3,4,5,6,7,8,11,12,13]}}
                ],
                columnDefs:[{targets:[1,9,10,14],orderable:false,searchable:false}]
            });
            if(jq('#tblRemarks tbody tr').length){jq('#tblRemarks').DataTable({pageLength:10,order:[]});}
            });
        })();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dvError" runat="server" style="display:none"></div>
    <input type="hidden" id="hfClient" runat="server" />
    <input type="hidden" id="hfActionKey" runat="server" />

    <div id="loadingOverlay" class="standard-loading-overlay">
        <div class="standard-loading-box">
            <div class="standard-spinner"></div>
            <div class="standard-loading-title">Please wait...</div>
            <div id="loadingMessage" class="standard-loading-text">Loading data...</div>
        </div>
    </div>

    <button type="button" id="btnLoadInvoiceModal" runat="server" class="hidden-server-button" onserverclick="btnLoadInvoiceModal_ServerClick"></button>
    <button type="button" id="btnLoadSendModal" runat="server" class="hidden-server-button" onserverclick="btnLoadSendModal_ServerClick"></button>
    <button type="button" id="btnLoadRemarkModal" runat="server" class="hidden-server-button" onserverclick="btnLoadRemarkModal_ServerClick"></button>
    <button type="button" id="btnLoadTestEmailModal" runat="server" class="hidden-server-button" onserverclick="btnLoadTestEmailModal_ServerClick"></button>
    <button type="button" id="btnLoadEmailPreviewModal" runat="server" class="hidden-server-button" onserverclick="btnLoadEmailPreviewModal_ServerClick"></button>
<div style="display:none">
        <select id="ddlProjects" runat="server" class="form-control"></select>
        <select id="ddlDatePeriod" runat="server" class="form-control"></select>
        <button type="button" runat="server" class="btn btn-primary" onserverclick="btnShow_ServerClick">Show All</button>
    </div>

    <div class="table-responsive" style="margin-bottom:12px">
        <table class="table">
            <tr>
                <td><b>Month:</b></td>
                <td>
                    <select id="ddlMonth" runat="server" class="form-control">
                        <option value="Select">Select</option><option value="January">January</option><option value="February">February</option>
                        <option value="March">March</option><option value="April">April</option><option value="May">May</option>
                        <option value="June">June</option><option value="July">July</option><option value="August">August</option>
                        <option value="September">September</option><option value="October">October</option><option value="November">November</option><option value="December">December</option>
                    </select>
                </td>
                <td><b>Year:</b></td>
                <td>
                    <select id="ddlYear" runat="server" class="form-control">
                        <option value="Select">Select</option><option value="2019">2019</option><option value="2020">2020</option><option value="2021">2021</option>
                    </select>
                </td>
                <td><button type="button" runat="server" class="btn btn-primary" onserverclick="btnShowFilter_ServerClick">Show</button></td>
            </tr>
        </table>
    </div>

    <div id="lblRecords" runat="server"></div>
    <div class="table-wrap">
        <table id="tblClientDetails" class="display compact stripe" cellspacing="0">
            <thead>
                <tr>
                    <th>Sr. No.</th><th>Update Invoice</th><th>Domain</th><th>Project Number</th><th>Billing Period</th>
                    <th>Invoice Number</th><th>Order Count</th><th>Total Amount</th><th>Invoice Generated Date</th>
                    <th>PDF</th><th>Excel</th><th>Is Invoice Sent To Client?</th><th>Invoice Delivered Date</th><th>Invoice Read Date</th><th>Send To Client</th>
                </tr>
            </thead>
            <tbody id="clientRows" runat="server"></tbody>
        </table>
    </div>

    <div id="modalViewer" class="portal-modal">
        <div class="portal-modal-dialog portal-modal-lg">
            <div class="portal-modal-header"><span id="reportModalTitle">Report Preview</span><button type="button" class="portal-close" onclick="closeViewer()">&times;</button></div>
            <div class="portal-modal-body">
                <div class="report-toolbar"><a id="reportDownload" class="btn btn-primary" href="#" style="display:none">Download</a></div>
                <div id="reportLoading">Loading preview...</div>
                <iframe id="viewerFrame" class="viewer-frame" src="about:blank" style="display:none"></iframe>
                <div id="excelPreview" class="excel-preview" style="display:none"></div>
            </div>
        </div>
    </div>

    <div id="modalSendInvoice" class="portal-modal">
        <div class="portal-modal-dialog portal-modal-lg">
            <div class="portal-modal-header"><span>Send Email</span><button type="button" class="portal-close" onclick="hidePortalModal('modalSendInvoice')">&times;</button></div>
            <div class="portal-modal-body">
                <button type="button" id="btnSendInvoice" runat="server" class="btn btn-primary" onserverclick="btnSendInvoice_ServerClick">Send Mail</button>
                <div id="dvInvocieNo" runat="server" style="display:none;margin-top:12px">
                    <b>Invoice #:</b>
                    <input type="text" id="lblInvoiceNo" runat="server" style="width:300px" disabled="disabled" />
                    <label><input type="checkbox" id="chkInvoiceNo" runat="server" onclick="updateInvoiceNumber()" /> Click to update Invoice number</label>
                </div>
                <div id="dvEmailTemplate" runat="server" class="catd table-card" style="margin-top:12px"></div>
                <span id="lblpath" runat="server" style="display:none"></span>
            </div>
        </div>
    </div>

    <div id="modalTestEmail" class="portal-modal">
        <div class="portal-modal-dialog">
            <div class="portal-modal-header"><span>Send Test Email</span><button type="button" class="portal-close" onclick="hidePortalModal('modalTestEmail')">&times;</button></div>
            <div class="portal-modal-body">
                <div class="form-row"><label>Email ID</label><input type="text" id="txttestEmailids" runat="server" class="form-control form-control-sm" /></div>
                <button type="button" id="btnsendEmail" runat="server" class="btn btn-primary" onserverclick="btnSendTestEmail_ServerClick">Send Mail</button>
            </div>
        </div>
    </div>

    <div id="modalEmailPreview" class="portal-modal">
        <div class="portal-modal-dialog portal-modal-lg">
            <div class="portal-modal-header"><span>Email Template</span><button type="button" class="portal-close" onclick="hidePortalModal('modalEmailPreview')">&times;</button></div>
            <div class="portal-modal-body">
                <div class="form-row"><label>To</label><span id="txtTo" runat="server"></span></div>
                <div class="form-row"><label>CC</label><span id="txtCC" runat="server"></span></div>
                <div class="form-row"><label>BCC</label><span id="txtBCC" runat="server"></span></div>
                <div class="form-row"><label>Date Sent</label><span id="lblDateSent" runat="server"></span></div>
                <div class="form-row"><label>Subject</label><span id="txtSubject" runat="server"></span></div>
                <div class="form-row"><label>Attachment</label><button type="button" id="lnkAttachment" runat="server" class="btn btn-link" onserverclick="btnAttachment_ServerClick">View</button></div>
                <div id="dvHtml" runat="server"></div>
            </div>
        </div>
    </div>

    <div id="modalInvoiceUpdate" class="portal-modal">
        <div class="portal-modal-dialog">
            <div class="portal-modal-header"><span>Update Invoice</span><button type="button" class="portal-close" onclick="hidePortalModal('modalInvoiceUpdate')">&times;</button></div>
            <div class="portal-modal-body">
                <input type="hidden" id="ajaxInvoiceKey" />
                <div class="form-row"><label>Project Number</label><span id="lblProjectPopup" runat="server" ClientIDMode="Static"></span></div>
                <div class="form-row"><label>Billing Period</label><span id="lblInvoiceNoPopup" runat="server" ClientIDMode="Static"></span></div>
                <div class="form-row"><label>Invoice received by client date</label><input type="date" id="dxInvrecieved" runat="server" ClientIDMode="Static" class="form-control-sm" /></div>
                <div class="form-row"><label>No dispute confirmed by client</label><input type="date" id="dxInvConfirm" runat="server" ClientIDMode="Static" class="form-control-sm" /></div>
                <div class="form-row"><label>Communication regarding receipt</label>
                    <select id="ddlReciep" runat="server" ClientIDMode="Static" class="form-control-sm"><option value="Select">Select</option><option value="By Mail">By Mail</option><option value="By Call">By Call</option></select>
                </div>
                <div class="form-row"><label>Remark</label><textarea id="txtInvRemark" runat="server" ClientIDMode="Static" rows="3" style="width:500px;max-width:100%"></textarea></div>
                <div class="form-row"><label>Completed Date</label><input type="date" id="dxDate" runat="server" ClientIDMode="Static" class="form-control-sm" /></div>
                <button type="button" id="btnUpdateInvoiceAjax" class="btn btn-primary" onclick="updateInvoiceAjax()">Update Invoice</button>
            </div>
        </div>
    </div>

    <div id="modalRemark" class="portal-modal">
        <div class="portal-modal-dialog">
            <div class="portal-modal-header"><span>Follow Up Remark</span><button type="button" class="portal-close" onclick="hidePortalModal('modalRemark')">&times;</button></div>
            <div class="portal-modal-body">
                <div id="lblerrorRemark" runat="server"></div>
                <div class="form-row"><label>Remark</label><textarea id="txtRemark" runat="server" rows="3"></textarea></div>
                <button type="button" id="btnUpdateRemark" runat="server" class="btn btn-primary" onserverclick="btnUpdateRemark_ServerClick">Update Remark</button>
                <hr />
                <table id="tblRemarks" class="display compact"><thead><tr><th>Sr. #</th><th>Remark</th><th>User</th><th>Added Date</th></tr></thead><tbody id="remarkRows" runat="server"></tbody></table>
            </div>
        </div>
    </div>
</asp:Content>
