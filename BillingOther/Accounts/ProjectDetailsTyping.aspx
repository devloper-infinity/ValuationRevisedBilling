<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="ProjectDetailsTyping.aspx.cs" Inherits="BillingOther.Accounts.ProjectDetailsTyping" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(document).ready(function () {
            NDAChange();
            SLAChange()
        });
        function HideLabel() {
            var seconds = 5;
            setTimeout(function () {
                var e=document.getElementById("<%= dvError.ClientID %>"); if(e) e.style.display = "none";
        }, seconds * 1000);
        };
        function SLAChange() {
            var ddl = document.getElementById("<%=ddsla.ClientID %>");
        var index = ddl.selectedIndex;
        if (index == 1) {
            trsla.style.display = '';
        }

        else if (index == 3) {
            if (document.getElementById("<%= rdcardYes.ClientID %>").checked) {
                    alert('Please select either Yes or No.');
                    ddl.selectedIndex = 0;
                    trsla.style.display = 'none';
                }
                else
                    trsla.style.display = 'none';
            }
            else {
                trsla.style.display = 'none';
            }
        }

        // ****************** AADHAR Card  **************//
        function ShowHideCard(val) {
  var isNew=(String(val)==='1');
  var tr=document.getElementById('<%= trnewprocess.ClientID%>');
  var td=document.getElementById('<%= tdprojtxt.ClientID%>');
  var txt=document.getElementById('<%= txtProjectname.ClientID%>');
  if(tr) tr.style.display=isNew?'none':'';
  if(td) td.style.display=isNew?'':'none';
  if(txt) txt.style.display=isNew?'':'none';
}

        function NDAChange() {
        var ddl = document.getElementById("<%=ddnda.ClientID %>");
        var index = ddl.selectedIndex;
        if (index == 1) {
            NDA.style.display = '';
        }

        else {
            NDA.style.display = 'none';
        }
    }
    function GetLiveStopped() {
        var ddl = document.getElementById("<%= ddlProjectStatus.ClientID %>");
        var index = ddl.selectedIndex;
        var value = ddl.options[index].text;
        if (value == "Live Stopped") {
            document.getElementById("<%=trlivestopped.ClientID %>").style.display = '';
        }
        else if (value == "Live") {
            var ddsla = document.getElementById("<%= ddsla.ClientID %>");
            var indexsla = ddsla.selectedIndex;
            if (indexsla == 2) {
                alert('You cannot select LIVE status as MSA is not signed.');
                ddl.selectedIndex = 0;
                document.getElementById("<%=trlivestopped.ClientID %>").style.display = 'none';
            }
        }
        else {
            document.getElementById("<%=trlivestopped.ClientID %>").style.display = 'none';
            }
        }

    
$(function(){var p=document.getElementById('txtphonenumber');if(p)p.addEventListener('input',function(){this.value=this.value.replace(/[^0-9+() .-]/g,'');});});
</script>

<link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
<link rel="stylesheet" href="https://cdn.datatables.net/responsive/2.5.0/css/responsive.dataTables.min.css" />
<style>
.migrated-page .table-responsive{width:100%;overflow-x:auto}.migrated-page table.dataTable{width:100%!important;font-size:12px}.migrated-page table.dataTable th,.migrated-page table.dataTable td{padding:7px 8px;vertical-align:middle}.standard-loading-overlay{display:none;position:fixed;inset:0;z-index:99999;background:rgba(15,23,42,.35);align-items:center;justify-content:center}.standard-loading-overlay.show{display:flex!important}.standard-loading-box{width:230px!important;min-width:230px!important;max-width:230px!important;height:auto!important;padding:20px 18px!important;margin:0!important;background:#fff;border-radius:10px;box-shadow:0 10px 30px rgba(0,0,0,.20);text-align:center;position:relative;top:auto!important;left:auto!important;transform:none!important}.standard-spinner{width:34px;height:34px;margin:0 auto 10px;border:4px solid #e5e7eb;border-top-color:#2563eb;border-radius:50%;animation:standardSpin .75s linear infinite}.standard-loading-title{font-size:13px;font-weight:600;line-height:18px;color:#172554}.standard-loading-text{margin-top:3px;font-size:11px;line-height:16px;color:#64748b}@keyframes standardSpin{to{transform:rotate(360deg)}}

/* Cost Master summary + tabs */
.migrated-page .project-summary-card{
    background:#fff;
    border:1px solid #e2e8f0;
    border-radius:14px;
    box-shadow:0 4px 18px rgba(15,23,42,.06);
    overflow:hidden;
    margin-bottom:20px;
}
.migrated-page .project-summary-header{
    display:flex;
    align-items:center;
    justify-content:space-between;
    gap:16px;
    padding:14px 18px;
    background:linear-gradient(135deg,#f8fbff 0%,#f8fafc 100%);
    border-bottom:1px solid #e2e8f0;
}
.migrated-page .project-summary-title{font-size:14px;font-weight:700;color:#172554;margin:0;line-height:1.2}
.migrated-page .project-summary-subtitle{font-size:11px;color:#64748b;margin-top:3px}
.migrated-page .project-back-btn{
    display:inline-flex;align-items:center;gap:6px;
    min-width:76px;justify-content:center;
    padding:8px 14px!important;margin:0!important;float:none!important;
    border:0;border-radius:8px;background:#f59e0b;color:#fff!important;
    font-size:12px;font-weight:600;box-shadow:0 3px 8px rgba(245,158,11,.22);
}
.migrated-page .project-back-btn:hover{background:#d97706;color:#fff!important}
.migrated-page .project-summary-grid{
    display:grid;
    grid-template-columns:repeat(4,minmax(0,1fr));
    gap:0;
    padding:0;
}
.migrated-page .summary-item{
    position:relative;
    padding:14px 16px;
    min-height:68px;
    border-right:1px solid #edf2f7;
    border-bottom:1px solid #edf2f7;
    background:#fff;
}
.migrated-page .summary-item:nth-child(4n){border-right:0}
.migrated-page .summary-item:nth-last-child(-n+4){border-bottom:0}
.migrated-page .summary-label{
    display:block;font-size:10px;font-weight:700;letter-spacing:.45px;
    text-transform:uppercase;color:#64748b;margin-bottom:6px;
}
.migrated-page .summary-value{
    display:block;font-size:13px;font-weight:600;color:#0f172a;
    white-space:normal;overflow-wrap:anywhere;
}
.migrated-page .summary-item.summary-primary .summary-value{color:#2563eb;font-weight:700}

.migrated-page .cost-tabs-wrap{
    background:#fff;border:1px solid #e2e8f0;border-radius:14px;
    box-shadow:0 4px 18px rgba(15,23,42,.05);overflow:hidden;
}
.migrated-page .nav.nav-tabs{
    display:flex;gap:7px;align-items:center;flex-wrap:wrap;
    padding:10px 12px;margin:0;
    border:0!important;border-bottom:1px solid #e5e7eb!important;
    background:#f8fafc;
}
.migrated-page .nav.nav-tabs .nav-item{margin:0!important;border:0!important}
.migrated-page .nav.nav-tabs .nav-link{
    display:inline-flex;align-items:center;gap:7px;
    padding:8px 13px!important;
    border:1px solid transparent!important;border-radius:8px!important;
    background:transparent;color:#475569!important;
    font-size:12px;font-weight:600;line-height:1.2;
    transition:all .16s ease;
}
.migrated-page .nav.nav-tabs .nav-link:hover{
    background:#eef4ff;color:#2563eb!important;border-color:#dbe7ff!important;
}
.migrated-page .nav.nav-tabs .nav-link.active,
.migrated-page .nav.nav-tabs .nav-item.active .nav-link{
    color:#fff!important;border-color:#2563eb!important;
    background:linear-gradient(135deg,#2563eb,#1d4ed8)!important;
    box-shadow:0 3px 8px rgba(37,99,235,.20);
}
.migrated-page .nav.nav-tabs .nav-link i{font-size:12px}
.migrated-page .nav.nav-tabs .slide{display:none!important}
.migrated-page .tab-content.card-block{
    border:0!important;margin:0!important;padding:16px!important;background:#fff;
}
.migrated-page .tab-pane{padding:0}

@media (max-width:991px){
    .migrated-page .project-summary-grid{grid-template-columns:repeat(2,minmax(0,1fr))}
    .migrated-page .summary-item:nth-child(4n){border-right:1px solid #edf2f7}
    .migrated-page .summary-item:nth-child(2n){border-right:0}
    .migrated-page .summary-item:nth-last-child(-n+4){border-bottom:1px solid #edf2f7}
    .migrated-page .summary-item:nth-last-child(-n+2){border-bottom:0}
}
@media (max-width:575px){
    .migrated-page .project-summary-grid{grid-template-columns:1fr}
    .migrated-page .summary-item{border-right:0!important;border-bottom:1px solid #edf2f7!important}
    .migrated-page .summary-item:last-child{border-bottom:0!important}
    .migrated-page .project-summary-header{align-items:flex-start}
    .migrated-page .nav.nav-tabs{gap:5px}
    .migrated-page .nav.nav-tabs .nav-link{padding:8px 10px!important;font-size:11px}
}

</style>
<script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
<script src="https://cdn.datatables.net/responsive/2.5.0/js/dataTables.responsive.min.js"></script>
<script>
function showLoading(message){var m=document.getElementById('loadingMessage');if(m)m.textContent=message||'Loading...';var o=document.getElementById('loadingOverlay');if(o)o.classList.add('show');}
function hideLoading(){var o=document.getElementById('loadingOverlay');if(o)o.classList.remove('show');}
function initMigratedTables(){if(!window.jQuery||!$.fn.DataTable)return;$('.migrated-datatable').each(function(){if(!$.fn.DataTable.isDataTable(this))$(this).DataTable({responsive:true,autoWidth:false,pageLength:25});});}
$(function(){initMigratedTables();});
</script>
<script>
var clientValidationRules=[{"id": "ddldomain", "required": true, "initial": "Select", "message": "Please Select Domain", "pattern": "", "patternMessage": ""}, {"id": "txtProjectname", "required": true, "initial": "", "message": "Please enter Project number", "pattern": "", "patternMessage": ""}, {"id": "ddlProjects", "required": true, "initial": "Select", "message": "Please Select Project", "pattern": "", "patternMessage": ""}, {"id": "txtProcess", "required": true, "initial": "", "message": "Please enter Process", "pattern": "", "patternMessage": ""}, {"id": "txtcompanyname", "required": true, "initial": "", "message": "Please enter Company name", "pattern": "", "patternMessage": ""}, {"id": "txtContactPerson", "required": true, "initial": "", "message": "Please enter Contact person", "pattern": "", "patternMessage": ""}, {"id": "txtphonenumber", "required": true, "initial": "", "message": "Please enter Phone no.", "pattern": "", "patternMessage": ""}, {"id": "txtemail", "required": true, "initial": "", "message": "Please enter Email", "pattern": "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.[a-z]{2,3})+)$", "patternMessage": "Invalid Email Format."}, {"id": "txturl", "required": true, "initial": "", "message": "Please enter URL", "pattern": "^(?:http(s)?:\\/\\/)?[\\w.-]+(?:\\.[\\w\\.-]+)+[\\w\\-\\._~:/?#[\\]@!\\$&'\\(\\)\\*\\+,;=.]+$", "patternMessage": "Invalid URL Format."}];
var salesValidationRules=[{"id": "ddlprojNo", "required": true, "initial": "Select", "message": "Please Select Project", "pattern": "", "patternMessage": ""}, {"id": "ddlBDM", "required": true, "initial": "Select", "message": "Please Select BDM", "pattern": "", "patternMessage": ""}, {"id": "txtRequestedDate", "required": true, "initial": "", "message": "Please enter Requested Date", "pattern": "", "patternMessage": ""}, {"id": "ddnda", "required": true, "initial": "Select", "message": "Please select NDA Signed", "pattern": "", "patternMessage": ""}, {"id": "ddsla", "required": true, "initial": "Select", "message": "Please select MSA Signed", "pattern": "", "patternMessage": ""}, {"id": "ddlProjectStatus", "required": true, "initial": "Select", "message": "Please Select ProjectStatus", "pattern": "", "patternMessage": ""}, {"id": "txtExpectedVolume", "required": true, "initial": "", "message": "Please enter Expected Volume", "pattern": "", "patternMessage": ""}, {"id": "txtExpectedStartDate", "required": true, "initial": "", "message": "Please enter Expected Date", "pattern": "", "patternMessage": ""}, {"id": "ddlProjectDuration", "required": true, "initial": "Select", "message": "Please Select Project Duration", "pattern": "", "patternMessage": ""}];
function validateRules(rules){
  for(var i=0;i<rules.length;i++){
    var r=rules[i], el=document.getElementById(r.id); if(!el || el.disabled || el.offsetParent===null) continue;
    var v=(el.value||'').trim();
    if(r.required && (!v || (r.initial && v===r.initial))){ alert(r.message||('Please enter '+r.id)); el.focus(); return false; }
    if(r.pattern && v){ try{ if(!(new RegExp(r.pattern)).test(v)){ alert(r.patternMessage||'Invalid value'); el.focus(); return false; } }catch(e){} }
  } return true;
}
function validateClientInfo(){return validateRules(clientValidationRules);}
function validateSalesInfo(){return validateRules(salesValidationRules);}
function setRequiredState(id,on){var e=document.getElementById(id);if(e)e.setAttribute('data-dynamic-required',on?'1':'0');}
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="migrated-page">
<div id="loadingOverlay" class="standard-loading-overlay"><div class="standard-loading-box"><div class="standard-spinner"></div><div class="standard-loading-title">Please wait...</div><div id="loadingMessage" class="standard-loading-text">Loading...</div></div></div>

    <div id="dvError" runat="server"></div>
    <input type="hidden" id="hdndomain" runat="server" />
    <input type="hidden" id="hdnSLAPath" runat="server" />
    <input type="hidden" id="hdProductBaseLabel" runat="server" />
    <input type="hidden" id="hdProductOtherLabel" runat="server" />
    <input type="hidden" id="hdOrderBaseLabel" runat="server" />
    <input type="hidden" id="hdOrderOtherLabel" runat="server" />

    <input type="hidden" id="hdnMSAPath" runat="server" />
    <div style="width: 100%; display: none;" id="dvSummary" runat="server">
        <div class="project-summary-card">
            <div class="project-summary-header">
                <div>
                    <div class="project-summary-title">Project Overview</div>
                    <div class="project-summary-subtitle">Key project and billing information</div>
                </div>
                <button type="submit" id="btnBack" runat="server" class="project-back-btn" OnServerClick="btnBack_Click"><i class="fa fa-arrow-left"></i> Back</button>
            </div>
            <div id="projectSummaryRows" runat="server" class="project-summary-grid"></div>
        </div>
    </div>

<div style="clear: both;"></div>
    <hr />
    <div class="card-content cost-tabs-wrap">
        <div class="card-body" style="padding:0;">
            <ul class="nav nav-tabs " role="tablist">
                <li class="nav-item active" id="tbpnl1" runat="server">
                    <a class="nav-link active" data-toggle="tab" href="#tab1" role="tab"><i class="fa fa-user"></i>&nbsp;&nbsp;Client Information</a>
                    <div class="slide"></div>
                </li>
                <li class="nav-item" id="tbpnl2" runat="server">
                    <a class="nav-link" data-toggle="tab" href="#tab2" role="tab"><i class="fa fa-envelope"></i>&nbsp;&nbsp;Sales Information</a>
                    <div class="slide"></div>
                </li>
                <li class="nav-item" id="tbpnlTyping" runat="server">
                    <a class="nav-link" data-toggle="tab" href="#tab3" role="tab"><i class="fa fa-cog"></i>&nbsp;&nbsp;Set Pricing</a>
                    <div class="slide"></div>
                </li>
            </ul>
            <div class="tab-content card-block" style="border: solid 1px #6c757d; margin-left: 5px;">
                <div class="tab-pane active" id="tab1" role="tabpanel">
                    
                        
                        
                            <table class="table table-border-style">
                                <tr id="rdButtons" runat="server">
                                    <td colspan="4">
                                        <label class="form-check-label"><input type="radio" id="rdcardYes" runat="server" onclick="ShowHideCard(&#x27;1&#x27;);" name="rdaadharcard" checked="true" /> New Project</label>
                                        <label class="form-check-label"><input type="radio" id="rdcardNo" runat="server" onclick="ShowHideCard(&#x27;2&#x27;);" name="rdaadharcard" /> New Process</label></b></td>
                                </tr>
                                <tr>
                                    <td>Domain:
                                    </td>
                                    <td>

                                        <select id="ddldomain" runat="server" class="form-control" style="width:200px" OnServerChange="ddldomain_SelectedIndexChanged" onchange="this.form.submit();">
                                        </select>
                                        

                                    </td>
                                    <td id="tdprojtxt" runat="server">Project Number:
                                    </td>
                                    <td>
                                        <input type="text" id="txtProjectname" runat="server" class="form-control" style="width:200px" />
                                        
                                    </td>
                                </tr>




                                <tr id="trnewprocess" runat="server" style="display: none;">
                                    <td>Project Number:</td>
                                    <td>

                                        <select id="ddlProjects" runat="server" class="form-control" style="width:200px" OnServerChange="ddlProjects_SelectedIndexChanged" onchange="this.form.submit();"></select>
                                        

                                    </td>
                                    <td>Process:</td>
                                    <td>
                                        <input type="text" id="txtProcess" runat="server" class="form-control" maxlength="3" style="width:200px" />
                                        
                                    </td>
                                </tr>

                                <tr>
                                    <td>Company Name:                        
                                    </td>
                                    <td>
                                        <input type="text" id="txtcompanyname" runat="server" class="form-control" style="width:200px" />
                                        
                                    </td>
                                    <td>Contact Person:</td>
                                    <td>
                                        <input type="text" id="txtContactPerson" runat="server" class="form-control" style="width:200px" />
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td>Phone Number:</td>
                                    <td>
                                        <input type="text" id="txtphonenumber" runat="server" class="form-control" style="width:200px" />
                                        
                                        
                                    </td>
                                    <td>Email Id: 
                                    </td>
                                    <td>
                                        <input type="email" id="txtemail" runat="server" class="form-control" style="width:200px" />
                                        
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td>Website Url:</td>
                                    <td>
                                        <input type="url" id="txturl" runat="server" class="form-control" style="width:200px" />
                                        
                                        
                                    </td>
                                    <td>Address:</td>
                                    <td>
                                        <textarea id="txtaddress" runat="server" class="form-control" style="resize: none; width:200px"></textarea>
                                    </td>
                                </tr>

                                <tr>
                                    <td>Remark: 
                                    </td>
                                    <td colspan="3">
                                        <textarea id="txtresult" runat="server" class="form-control" style="resize: none; width:350px"></textarea>
                                    </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="3" style="text-align: left;">

                                        <button type="submit" id="btnsbmit" runat="server" class="btn btn-primary" OnServerClick="btnsbmit_Click" onclick="if(!validateClientInfo()) return false; showLoading('Saving project information...');">Submit</button>
                                        
                                </tr>
                            </table>
                        
                        
                    

                    <div id="Div1" runat="server" style="overflow: auto; display: none;">
                        <div class="table-responsive"><table id="tblProjectRequests" class="table table-striped table-bordered migrated-datatable" style="width:100%"><thead><tr><th>Sr. No</th><th>Project Number</th><th>Company Name</th><th>Contact Person</th><th>Phone Number</th><th>Email</th><th>Website Url</th><th>Address</th><th>Remark</th><th>Edit</th></tr></thead><tbody id="projectRequestRows" runat="server"></tbody></table></div>
                    </div>
                </div>
                <div class="tab-pane" id="tab2" role="tabpanel">
                    <table class="table table-border-style">
                        <tr>
                            <td>Project Number:
                            </td>
                            <td>
                                <%--   <input type="text" id="lblSalesProjectNumber" runat="server" />--%>
                                <select id="ddlprojNo" runat="server" class="form-control" style="width:200px" disabled="disabled"></select>
                                
                            </td>
                            <td>Process Name:
                            </td>
                            <td>
                                <input type="text" id="txtProcessNameSales" runat="server" class="form-control" style="width:200px" /></td>
                        </tr>
                        <tr>

                            <td>BDM:
                            </td>
                            <td>
                                <select id="ddlBDM" runat="server" class="form-control" style="width:200px">
                                </select>
                                
                            </td>
                            <td style="display: none;">Requested Date:
                            </td>
                            <td style="display: none;">
                                <input type="date" id="txtRequestedDate" runat="server" placeholder="01-Jan-2016" class="form-control" style="width:200px" autocomplete="off" />
                                
                                
                            </td>
                        </tr>
                        <tr>
                            <td>Scope of Project:
                            </td>
                            <td colspan="3">
                                <textarea id="txtProjectScope" runat="server" class="form-control" style="resize: none; width:510px"></textarea>
                            </td>


                        </tr>
                        <tr>
                            <td>NDA Signed:
                            </td>
                            <td>
                                <select id="ddnda" runat="server" class="form-control" style="width:200px" onchange="NDAChange();">
                                    <option Value="Select">Select</option>
                                    <option Value="1">Yes</option>
                                    <option Value="0">No</option>
                                    <option Value="2">Not Applicable</option>
                                </select>
                                
                            </td>
                            <td></td>
                            <td></td>

                        </tr>
                        <tr id="NDA" style="display: none;">
                            <td colspan="4">
                                <table class="table" border="1" style="border-style: solid; border-width: 1px; text-align: center">
                                    <tr>
                                        <th>Date of the Agreement
                                        </th>
                                        <th>Expiration Date of the Agreement
                                        </th>
                                        <th>Signed by (Client)
                                        </th>
                                        <th>Signed by (Infinity)
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input type="date" id="txtDateOfAgreement" runat="server" placeholder="01-Jan-2016" class="form-control" style="width:130px" autocomplete="off" />
                                            
                                        </td>
                                        <td>
                                            <input type="text" id="txtExpirationDateOfAgreement" runat="server" placeholder="01-Jan-2016" class="form-control" style="width:130px" autocomplete="off" />
                                            
                                        </td>
                                        <td>
                                            <select id="ddlNDASignedByClient" runat="server" class="form-control" style="width:100px">
                                                <option Value="NULL">Select</option>
                                                <option Value="1">Yes</option>
                                                <option Value="0">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlNDASignedByInfinity" runat="server" class="form-control" style="width:100px">
                                                <option Value="NULL">Select</option>
                                                <option Value="1">Yes</option>
                                                <option Value="0">No</option>
                                            </select>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td>Upload Document:
                                        </td>
                                        <td>
                                            <input type="file" id="fpNDA" runat="server" class="form-control" style="width:200px" /></td>
                                        <td>
                                            <button type="submit" id="LinkButtonNDA" runat="server" style="float: right; padding-right: 150px; font-size: 10px; font-weight: bold; background:none; border:0; color:#0d6efd; text-decoration:underline; cursor:pointer" OnServerClick="LinkButtonNDA_Click" >View</button>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>MSA Signed:
                            </td>
                            <td>
                                <select id="ddsla" runat="server" class="form-control" style="width:200px" onchange="SLAChange();">
                                    <option Value="Select">Select</option>
                                    <option Value="1">Yes</option>
                                    <option Value="0">No</option>
                                    <option Value="2">Not Applicable</option>
                                </select>
                                
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr id="trsla" style="display: none;">
                            <td colspan="6">
                                <table class="table" border="1" style="border-style: solid; border-width: 1px; text-align: center">
                                    <tr>
                                        <th>Date of the Agreement
                                        </th>
                                        <th>Expiration Date of the Agreement
                                        </th>
                                        <th>Signed by (Client)
                                        </th>
                                        <th>Signed by (Infinity)
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input type="text" id="txtDateOfSLAAgreement" runat="server" placeholder="01-Jan-2016" class="form-control" style="width:130px" autocomplete="off" />
                                            
                                        </td>
                                        <td>
                                            <input type="text" id="txtExpirationDateOfSLAAgreement" runat="server" placeholder="01-Jan-2016" class="form-control" style="width:130px" autocomplete="off" />
                                            
                                        </td>
                                        <td>
                                            <select id="ddlSLASignedByClient" runat="server" class="form-control" style="width:100px">
                                                <option Value="NULL">Select</option>
                                                <option Value="1">Yes</option>
                                                <option Value="0">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlSLASignedByInfinity" runat="server" class="form-control" style="width:100px">
                                                <option Value="NULL">Select</option>
                                                <option Value="1">Yes</option>
                                                <option Value="0">No</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Upload Document:
                                        </td>
                                        <td>
                                            <input type="file" id="fpSLA" runat="server" class="form-control" style="width:200px" /></td>
                                        <td>
                                            <button type="submit" id="LinkButtonSLA" runat="server" style="float: right; padding-right: 150px; font-size: 10px; font-weight: bold; background:none; border:0; color:#0d6efd; text-decoration:underline; cursor:pointer" OnServerClick="LinkButtonSLA_Click" >View</button>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>Project Status:
                            </td>
                            <td>
                                <select id="ddlProjectStatus" runat="server" class="form-control" style="width:200px" onchange="GetLiveStopped();">
                                    <option Value="Select">Select</option>
                                    <option Value="Live">Live</option>
                                    <option Value="Demo">Demo</option>
                                    <option Value="Test">Test</option>
                                    <option Value="Live Stopped">Live Stopped</option>
                                </select>
                                
                            </td>
                            <td>Expected Volume:
                            </td>
                            <td>
                                <input type="text" id="txtExpectedVolume" runat="server" class="form-control" style="width:200px" />
                                
                            </td>
                        </tr>
                        <tr id="trlivestopped" runat="server" style="display: none;">
                            <td>Stopped Date:
                            </td>
                            <td>
                                <input type="text" id="txtStoppedDate" runat="server" class="form-control" style="width:200px" />
                                
                            </td>
                            <td>Stopped Remark:</td>
                            <td>
                                <textarea id="txtStoppedRemark" runat="server" class="form-control" style="width:200px"></textarea>
                            </td>
                        </tr>
                        <tr>
                            <td>Expected Start Date:
                            </td>
                            <td>
                                <input type="date" id="txtExpectedStartDate" runat="server" placeholder="01-Jan-2016" class="form-control" style="width:200px" autocomplete="off" />
                                
                                
                            </td>
                            <td>Project Duration:
                            </td>
                            <td>
                                <select id="ddlProjectDuration" runat="server" class="form-control" style="width:200px">
                                    <option Value="Select">Select</option>
                                    <option Value="Short Term">Short Term</option>
                                    <option Value="Long Term">Long Term</option>
                                </select>
                                
                            </td>
                        </tr>
                        <tr>
                            <tr>
                                <td>Remark:
                                </td>
                                <td>
                                    <textarea id="txtSalesRemark" runat="server" resize="none" class="form-control" style="resize: none; width:310px"></textarea>
                                </td>
                                <td>Rate Revision Date:</td>
                                <td>
                                    <input type="text" id="txtRateRevisionDate" runat="server" class="form-control" style="width:200px" />
                                    
                                </td>
                            </tr>
                        </tr>

                        <tr>
                            <td></td>
                            <td colspan="3" style="text-align: left;">
                                <button type="submit" id="btnSalesInformation" runat="server" class="btn btn-primary" OnServerClick="btnSalesInformation_Click" onclick="if(!validateSalesInfo()) return false; showLoading('Saving sales information...');">Submit</button>

                            </td>
                        </tr>
                    </table>
                </div>
                <div class="tab-pane" id="tab3" role="tabpanel">
                    <script>
                        function GetConfiguration(ddl) {
                            var trBaseRate = document.getElementById("<%= trBaseRate.ClientID %>");
                            var trProductType = document.getElementById("<%= trProductType.ClientID %>");
                            var trOrderType = document.getElementById("<%= trOrderType.ClientID %>");
                            var trCharacter = document.getElementById("<%= trCharacter.ClientID %>");
                            var tbl379003 = document.getElementById("<%= tbl379003.ClientID %>");
                            var index = ddl.selectedIndex;
                            if (index == 1) {
                                trBaseRate.style.display = '';
                                trProductType.style.display = 'none';
                                trOrderType.style.display = 'none';
                                trCharacter.style.display = 'none';
                                tbl379003.style.display = 'none';
                            }
                            else if (index == 2) {
                                trBaseRate.style.display = 'none';
                                trProductType.style.display = '';
                                trOrderType.style.display = 'none';
                                trCharacter.style.display = 'none';
                                tbl379003.style.display = 'none';
                            }
                            else if (index == 3) {
                                trBaseRate.style.display = 'none';
                                trProductType.style.display = 'none';
                                trOrderType.style.display = '';
                                trCharacter.style.display = 'none';
                                tbl379003.style.display = 'none';
                            }
                            else if (index == 4) {
                                trBaseRate.style.display = 'none';
                                trProductType.style.display = 'none';
                                trOrderType.style.display = 'none';
                                trCharacter.style.display = '';
                                tbl379003.style.display = 'none';
                            }
                            else if (index == 5) {
                                trBaseRate.style.display = 'none';
                                trProductType.style.display = 'none';
                                trOrderType.style.display = 'none';
                                trCharacter.style.display = 'none';
                                tbl379003.style.display = '';
                            }
                        }
                    </script>
                    <table class="table">
                        <tr>
                            <td><b>Billing Type:</b></td>
                            <td>
                                <select id="ddlBillingBase" runat="server" class="form-control" style="width:200px" onchange="GetConfiguration(this);">
                                    <option Value="Select">Select</option>
                                    <option Value="Base Rate">Base Rate Only</option>
                                    <option Value="Product Type">Product Type</option>
                                    <option Value="Order Type">Order Type</option>
                                    <option Value="Character Based">Character Based</option>
                                    <option Value="Page Based">Page Based</option>
                                </select>
                            </td>
                            <td>
                                
                            </td>
                            <td><b>Billing Criteria:</b></td>
                            <td>
                                <select id="ddlBillingCriteria" runat="server" class="form-control" style="width:210px">
                                    <option Value="Select">Select</option>
                                    <option Value="Cancelled - Not to be billed">Cancelled - Not to be billed</option>
                                    <option Value="Cancelled - To be billed">Cancelled - To be billed</option>
                                </select>
                            </td>
                            <td>
                                
                            </td>
                        </tr>
                        <tr id="trBaseRate" runat="server" style="display: none;">
                            <td colspan="6">
                                <table class="table">
                                    <tr>
                                        <th>Parameter Name</th>
                                        <th>Is Applicable?</th>
                                        <th>Charge Type</th>
                                        <th>Price</th>
                                        <th>Billing Header</th>
                                    </tr>
                                    <tr>
                                        <td>Base Rate</td>
                                        <td>
                                            <select id="ddlIsApplicableBaseRate" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeBaseRate" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                                <option Value="Incremental">Incremental</option>
                                                <option Value="Percentage">Percentage</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceBaseRate" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderBaseRate" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Dispatched Record">Dispatched Record</option>
                                                <option Value="No Of Records">No Of Records</option>
                                                <option Value="Order#">Order#</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Rush</td>
                                        <td>
                                            <select id="ddlIsApplicableBaseRateRush" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeBaseRateRush" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                                <option Value="Incremental">Incremental</option>
                                                <option Value="Percentage">Percentage</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceBaseRateRush" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderBaseRateRush" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Order#">Order#</option>
                                                <option Value="Rush">Rush</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td># Characters > 800</td>
                                        <td>
                                            <select id="ddlIsApplicableBaseRateCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeBaseRateCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                                <option Value="Incremental">Incremental</option>
                                                <option Value="Percentage">Percentage</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceBaseRateCharacter" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderBaseRateCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="# of Character"># of Character</option>
                                            </select>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="trProductType" runat="server" style="display: none;">
                            <td colspan="6">
                                <script>
                                    function GetProductInfo(ddlProduct) {
                                        var lblBaseProduct = document.getElementById("<%= lblProdcutBase.ClientID %>");
                                        var lblOtherProduct = document.getElementById("<%= lblProdcutOther.ClientID %>");
                                        var hdProductBaseLabel = document.getElementById("<%= hdProductBaseLabel.ClientID %>");
                                        var hdProductOtherLabel = document.getElementById("<%= hdProductOtherLabel.ClientID %>");
                                        var index = ddlProduct.selectedIndex;
                                        var ddlText = ddlProduct.options[index].text;
                                        for (var i = 0; i < ddlProduct.length; i++) {
                                            if (ddlProduct.options[i].text == ddlText && ddlProduct.options[i].text != "Select") {
                                                lblBaseProduct.innerHTML = ddlText;
                                                hdProductBaseLabel.value = ddlText;
                                            }
                                            if (ddlProduct.options[i].text != ddlText && ddlProduct.options[i].text != "Select") {
                                                lblOtherProduct.innerHTML = ddlProduct.options[i].text;
                                                hdProductOtherLabel.value = ddlProduct.options[i].text;
                                            }
                                        }
                                    }
                                </script>
                                <table class="table table-border-style" style="display: none;">
                                    <tr>
                                        <td><b>Base Product Type:</b></td>
                                        <td>
                                            <select id="ddlBaseProductType" runat="server" class="form-control" style="width:200px" onchange="GetProductInfo(this);">
                                            </select>
                                        </td>
                                    </tr>
                                </table>
                                <table class="table table-border-style">
                                    <tr>
                                        <th>Parameter Name</th>
                                        <th>Is Applicable?</th>
                                        <th>Charge Type</th>
                                        <th>Price</th>
                                        <th>Billing Header</th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Label1" runat="server" class="form-control">Purchase</span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicablePurchase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypePurchase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPricePurchase" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderPurchase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Product Type">Product Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Label2" runat="server" class="form-control">Refinance</span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableRefinance" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeRefinance" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceRefinance" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderRefinance" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Product Type">Product Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Label3" runat="server" class="form-control">Current Owner</span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableCurrentOwner" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeCurrentOwner" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceCurrentOwner" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderCurrentOwner" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Product Type">Product Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Label4" runat="server" class="form-control">Full Search</span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableFullSearch" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeFullSearch" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceFullSearch" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderFullSearch" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Product Type">Product Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="Label5" runat="server" class="form-control">Two Owner</span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableTwoOwner" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeTwoOwner" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceTwoOwner" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderTwoOwner" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Product Type">Product Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td># Characters > 800</td>
                                        <td>
                                            <select id="ddlIsApplicableCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceCharacter" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderCharacterNew" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="# of Character"># of Character</option>
                                            </select>
                                        </td>
                                    </tr>
                                </table>
                                <table class="table table-border-style" style="display: none;">
                                    <tr>
                                        <th>Parameter Name</th>
                                        <th>Is Applicable?</th>
                                        <th>Charge Type</th>
                                        <th>Price</th>
                                        <th>Billing Header</th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="lblProdcutBase" runat="server" class="form-control"></span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableProductBase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeProductBase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceProductBase" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHEaderProductBase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Product Type">Product Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="lblProdcutOther" runat="server" class="form-control"></span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableProductOther" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeProductOther" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                                <option Value="Incremental">Incremental</option>
                                                <option Value="Percentage">Percentage</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceProductOther" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHEaderProductOther" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Product Type">Product Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td># Characters > 800</td>
                                        <td>
                                            <select id="ddlIsApplicableProductCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeProductCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                                <option Value="Incremental">Incremental</option>
                                                <option Value="Percentage">Percentage</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceProductCharacter" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderproductCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="# of Character"># of Character</option>
                                            </select>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="trOrderType" runat="server" style="display: none;">
                            <td colspan="6">
                                <script>
                                    function GetOrderTypeInfo(ddlOrder) {
                                        var lblBaseOrder = document.getElementById("<%= lblOrderTypeBase.ClientID %>");
                                        var lblOtherOrder = document.getElementById("<%= lblOrderTypeOther.ClientID %>");
                                        var hdOrderBaseLabel = document.getElementById("<%= hdOrderBaseLabel.ClientID %>");
                                        var hdOrderOtherLabel = document.getElementById("<%= hdOrderOtherLabel.ClientID %>");
                                        var index = ddlOrder.selectedIndex;
                                        var ddlText = ddlOrder.options[index].text;
                                        for (var i = 0; i < ddlOrder.length; i++) {
                                            if (ddlOrder.options[i].text == ddlText && ddlOrder.options[i].text != "Select") {
                                                lblBaseOrder.innerHTML = ddlText;
                                                hdOrderBaseLabel.value = ddlText;
                                            }
                                            if (ddlOrder.options[i].text != ddlText && ddlOrder.options[i].text != "Select") {
                                                lblOtherOrder.innerHTML = ddlOrder.options[i].text;
                                                hdOrderOtherLabel.value = ddlOrder.options[i].text;
                                            }
                                        }
                                    }
                                </script>
                                <table class="table table-border-style">
                                    <tr>
                                        <td><b>Base Order Type:</b></td>
                                        <td>
                                            <select id="ddlBaseOrderType" runat="server" class="form-control" style="width:200px" onchange="GetOrderTypeInfo(this);">
                                                <option Value="Select">Select</option>
                                                <option Value="Commercial">Commercial</option>
                                                <option Value="Residential">Residential</option>
                                            </select>
                                        </td>
                                    </tr>
                                </table>
                                <table class="table table-border-style">
                                    <tr>
                                        <th>Parameter Name</th>
                                        <th>Is Applicable?</th>
                                        <th>Charge Type</th>
                                        <th>Price</th>
                                        <th>Billing Header</th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="lblOrderTypeBase" runat="server" class="form-control"></span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableOrderBase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeOrderBase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceOrderBase" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderOrderBase" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Order Type">Order Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="lblOrderTypeOther" runat="server" class="form-control"></span>
                                        </td>
                                        <td>
                                            <select id="ddlIsApplicableOrderOther" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeOrderOther" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                                <option Value="Incremental">Incremental</option>
                                                <option Value="Percentage">Percentage</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceOrderOther" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderOrderOther" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Order Type">Order Type</option>
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td># Characters > 800</td>
                                        <td>
                                            <select id="ddlIsApplicableOrderCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Yes">Yes</option>
                                                <option Value="No">No</option>
                                            </select>
                                        </td>
                                        <td>
                                            <select id="ddlChargeTypeOrderCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                                <option Value="Incremental">Incremental</option>
                                                <option Value="Percentage">Percentage</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPriceOrderCharacter" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHEaderOrderCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="# of Character"># of Character</option>
                                            </select>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="trCharacter" runat="server" style="display: none;">
                            <td colspan="6">
                                <table class="table table-border-style" border="1">
                                    <tr>
                                        <th>Parameter Name</th>
                                        <th>For Character</th>
                                        <th>Price</th>
                                        <th>Billing Header</th>
                                    </tr>
                                    <tr>
                                        <td># of Character</td>
                                        <td>
                                            <input type="text" id="txtPerCharacter" runat="server" />
                                        </td>
                                        <td>
                                            <input type="text" id="txtPricePerCharacter" runat="server" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeaderCharacter" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="# of Character"># of Character</option>
                                            </select></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="tbl379003" runat="server" style="display: none;">
                            <td colspan="6">
                                <table class="table table-border-style" border="1">
                                    <tr>
                                        <th>Parameter</th>
                                        <th>Charge Type</th>
                                        <th>Price</th>
                                        <th>Billing Header</th>
                                    </tr>
                                    <tr>
                                        <td>No of Pages</td>
                                        <td>
                                            <select id="ddlChargeType771ScanPages" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="Fix Amount">Fix Amount</option>
                                            </select>
                                        </td>
                                        <td>
                                            <input type="text" id="txtPrice771ScanPages" runat="server" class="form-control" style="width:200px" />
                                        </td>
                                        <td>
                                            <select id="ddlBillingHeader771ScanPages" runat="server" class="form-control" style="width:200px">
                                                <option Value="Select">Select</option>
                                                <option Value="No of Pages">No of Pages</option>
                                            </select>
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td>
                                <button type="submit" id="btnShubmitTyping" runat="server" OnServerClick="btnShubmitTyping_Click" onclick="showLoading('Please wait...');">Submit</button>
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </div>


        </div>
    </div>

</div>
</asp:Content>