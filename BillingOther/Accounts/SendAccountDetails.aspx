<%@ Page Title="" Language="C#" MasterPageFile="~/Accounts/Accounts.Master" AutoEventWireup="true" CodeBehind="SendAccountDetails.aspx.cs" Inherits="BillingOther.Accounts.SendAccountDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            window.setTimeout(function () {
                var tableElement = document.getElementById("billingInboxTable");
                var links = document.querySelectorAll(".billing-domain-link");
                if (!tableElement || !links.length || !window.jQuery || !jQuery.fn.DataTable) return;
                var selectedDomain = document.querySelector(".billing-domain-link.active").getAttribute("data-domain-id");
                var table = jQuery(tableElement).DataTable();
                jQuery.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
                    if (settings.nTable !== tableElement) return true;
                    var row = settings.aoData[dataIndex].nTr;
                    return row && row.getAttribute("data-domain-id") === selectedDomain;
                });
                function updateCount() {
                    var count = table.rows({ search: "applied" }).count();
                    var label = document.getElementById("lblRecords");
                    if (label) label.textContent = count + " pending";
                }
                Array.prototype.forEach.call(links, function (link) {
                    link.addEventListener("click", function (event) {
                        event.preventDefault();
                        selectedDomain = link.getAttribute("data-domain-id");
                        Array.prototype.forEach.call(links, function (item) { item.classList.remove("active"); });
                        link.classList.add("active");
                        table.page("first").draw();
                        updateCount();
                    });
                });
                table.draw();
                updateCount();
            }, 0);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="billing-inbox-title"><i class="fa fa-clipboard"></i><span>Invoice to be submitted</span><asp:Label ID="lblRecords" runat="server" ClientIDMode="Static"></asp:Label></div>
    <div class="billing-inbox">
        <aside class="billing-domain-list" aria-label="Pending billing domains">
            <asp:Repeater ID="rptDomains" runat="server">
                <ItemTemplate>
                    <a href="#" class='<%# GetDomainCss(Eval("DomainID")) %>' data-domain-id='<%# Eval("DomainID") %>'><%#: Eval("DomainName") %></a>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoDomains" runat="server" CssClass="billing-domain-empty" Visible="false">No pending domains.</asp:Panel>
        </aside>
        <section class="billing-inbox-results">
            <div class="table-responsive portal-grid-card">
                <table id="billingInboxTable" class="table portal-datatable" data-datatable="true">
                    <thead>
                        <tr>
                            <th>Action</th>
                            <th>Sr. #</th>
                            <th>Project #</th>
                            <th>Billing Period</th>
                            <th>Type</th>
                            <th>Status</th>
                            <th>Billing Sent Date</th>
                            <th style="display:none;">FTE Details</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptBillingDetails" runat="server">
                            <ItemTemplate>
                                <tr data-domain-id='<%# Eval("DomainID") %>'>
                                    <td><a class="portal-action" href='<%# GetDetailsUrl(Eval("ProjectID"), Eval("BillingPeriod"), Eval("ProjectName")) %>'><i class="fa fa-pencil"></i><span>Edit</span></a></td>
                                    <td><%# Container.ItemIndex + 1 %></td>
                                    <td><%#: Eval("ProjectName") %></td>
                                    <td><%#: Eval("BillingPeriod") %></td>
                                    <td><%#: Eval("TYPE") %></td>
                                    <td><%#: Eval("Status") %></td>
                                    <td><%#: Eval("AddedDate") %></td>
                                    <td style="display:none;">
                                        <asp:Literal runat="server" Text='<%# GetFteDisplay(Eval("ProjectID"), Eval("ProjectName"), Eval("BillingPeriod"), Eval("TYPE")) %>' /></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
