<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ProjectDetailsMaster.aspx.cs" Inherits="BillingOther.Accounts.ProjectDetailsMaster" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function HideLabel() {

            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%# dvError.ClientID %>").style.display = "none";
                document.getElementById("<%# dvError.ClientID %>").innerHTML = "";
            }, seconds * 1000);
        };   </script>

<link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
<link rel="stylesheet" href="https://cdn.datatables.net/responsive/2.5.0/css/responsive.dataTables.min.css" />
<style>
.project-master-card{background:#fff;border:1px solid #e2e8f0;border-radius:12px;box-shadow:0 2px 8px rgba(15,23,42,.05);overflow:hidden}
.project-master-header{display:flex;align-items:center;justify-content:space-between;gap:16px;padding:16px 18px;border-bottom:1px solid #e2e8f0}
.project-master-title{font-size:15px;font-weight:700;color:#172554;margin:0}.project-master-subtitle{font-size:11px;color:#64748b;margin-top:3px}
.project-add-btn{border:0;border-radius:7px;background:#2563eb;color:#fff;font-size:12px;font-weight:600;padding:9px 14px;cursor:pointer;white-space:nowrap}.project-add-btn:hover{background:#1d4ed8}
.project-table-wrap{padding:14px 16px 16px;overflow:hidden}
#tblProjects_wrapper{width:100%}#tblProjects_wrapper .dataTables_length,#tblProjects_wrapper .dataTables_filter{margin-bottom:12px;font-size:12px;color:#475569}
#tblProjects_wrapper .dataTables_filter input{height:32px;border:1px solid #cbd5e1;border-radius:6px;padding:5px 9px;outline:none;margin-left:6px;min-width:190px;background:#fff}
#tblProjects_wrapper .dataTables_length select{height:30px;border:1px solid #cbd5e1;border-radius:6px;padding:3px 26px 3px 7px;background:#fff}
#tblProjects{width:100%!important;border-collapse:separate!important;border-spacing:0!important;font-size:12px!important}
#tblProjects thead th{background:#f8fafc!important;color:#334155!important;font-size:11px!important;font-weight:700!important;padding:9px 10px!important;border-bottom:1px solid #dbe3ee!important;white-space:nowrap!important;text-align:left!important}
#tblProjects tbody td{padding:8px 10px!important;border-bottom:1px solid #eef2f7!important;color:#1e293b!important;vertical-align:middle!important;white-space:nowrap!important}
#tblProjects tbody tr:hover td{background:#f8fbff!important}.edit-col{width:58px!important}.serial-col{width:48px!important;text-align:center!important}.address-col{min-width:260px!important;white-space:normal!important;line-height:1.35!important}
.project-edit-btn{display:inline-flex;align-items:center;justify-content:center;height:28px;min-width:46px;padding:0 10px;border:1px solid #bfdbfe;border-radius:6px;background:#eff6ff;color:#1d4ed8!important;text-decoration:none!important;font-size:11px;font-weight:600}.project-edit-btn:hover{background:#dbeafe}
#tblProjects_wrapper .dataTables_info,#tblProjects_wrapper .dataTables_paginate{font-size:11px;color:#64748b;margin-top:10px}#tblProjects_wrapper .dataTables_paginate .paginate_button{padding:4px 8px!important;border-radius:5px!important;border:1px solid transparent!important}.dataTables_scrollBody{border-bottom:0!important}
.standard-loading-overlay{display:none;position:fixed;inset:0;z-index:99999;background:rgba(15,23,42,.35);align-items:center;justify-content:center}.standard-loading-overlay.show{display:flex!important}.standard-loading-box{width:230px!important;min-width:230px!important;max-width:230px!important;padding:20px 18px!important;background:#fff;border-radius:10px;box-shadow:0 10px 30px rgba(0,0,0,.20);text-align:center}.standard-spinner{width:34px;height:34px;margin:0 auto 10px;border:4px solid #e5e7eb;border-top-color:#2563eb;border-radius:50%;animation:standardSpin .75s linear infinite}.standard-loading-title{font-size:13px;font-weight:600;color:#172554}.standard-loading-text{margin-top:3px;font-size:11px;color:#64748b}@keyframes standardSpin{to{transform:rotate(360deg)}}
</style>
<script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
<script src="https://cdn.datatables.net/responsive/2.5.0/js/dataTables.responsive.min.js"></script>
<script>
function showLoading(message){var m=document.getElementById('loadingMessage');if(m)m.textContent=message||'Loading...';var o=document.getElementById('loadingOverlay');if(o)o.classList.add('show');}
function hideLoading(){var o=document.getElementById('loadingOverlay');if(o)o.classList.remove('show');}
function initMigratedTables(){if(!window.jQuery||!$.fn.DataTable)return;var t=$('#tblProjects');if(t.length&&!$.fn.DataTable.isDataTable(t[0])){t.DataTable({responsive:false,autoWidth:false,scrollX:true,pageLength:25,lengthMenu:[[25,50,100,-1],[25,50,100,'All']],order:[],columnDefs:[{targets:0,orderable:false,width:'58px'},{targets:1,width:'48px'},{targets:8,width:'280px'}],language:{search:'Search:',lengthMenu:'Show _MENU_ entries',info:'Showing _START_ to _END_ of _TOTAL_ entries',infoEmpty:'No records found'}});}}
$(function(){initMigratedTables();});
</script></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="migrated-page">
<div id="loadingOverlay" class="standard-loading-overlay"><div class="standard-loading-box"><div class="standard-spinner"></div><div class="standard-loading-title">Please wait...</div><div id="loadingMessage" class="standard-loading-text">Loading...</div></div></div>

    <div id="dvError" runat="server"></div>
    <div class="project-master-card">
        <div class="project-master-header">
            <div><div class="project-master-title">Project Details</div><div class="project-master-subtitle">View and maintain configured projects</div></div>
            <button type="submit" id="btnAddNew" runat="server" class="project-add-btn" OnServerClick="btnAddNew_Click" onclick="showLoading('Opening project details...');">Add New Project</button>
        </div>
        <div class="project-table-wrap">
            <table id="tblProjects" class="display migrated-datatable nowrap" style="width:100%">
                <thead><tr><th>Edit</th><th>Sr. #</th><th>Domain</th><th>Project Number</th><th>Process</th><th>Company Name</th><th>Contact Person</th><th>Phone Number</th><th>Address</th><th>Remark</th></tr></thead>
                <tbody id="projectRows" runat="server"></tbody>
            </table>
        </div>
    </div>
    

</div>
</asp:Content>
