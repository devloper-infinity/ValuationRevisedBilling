using BillingOther.App_Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BillingOther.App_Code.BLL
{
    public class bllTracking
    {
        DALTracking dalTracking = new DALTracking();
        #region tracking Sheet
        public DataSet GetAllProjectSendToAccounts(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetAllProjectSendToAccounts(ProjectID, BillingPeriod);
        }

        public DataSet GetTotalProjectAmount(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetTotalProjectAmount(ProjectID, BillingPeriod);
        }

        public DataSet GetTotalProjectAmount_Freight(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetTotalProjectAmount_Freight(ProjectID, BillingPeriod);

        }

        public DataSet GetTotalProjectAmount_Typing(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetTotalProjectAmount_Typing(ProjectID, BillingPeriod);

        }

        public DataSet GetTotalProjectAmount_Freight_SummaryCount(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetTotalProjectAmount_Freight_SummaryCount(ProjectID, BillingPeriod);
        }

        public DataTable GetDomianFromProject(int ProjectID)
        {
            return dalTracking.GetDomianFromProject(ProjectID);
        }

        public DataSet GetTotalProjectAmountForReport(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetTotalProjectAmountForReport(ProjectID, BillingPeriod);
        }

        public DataTable GetAllProjectByUserRights()
        {
            return dalTracking.GetAllProjectByUserRights();
        }

        public DataTable GetAllProjectSendToAccountsDetails(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetAllProjectSendToAccountsDetails(ProjectID, BillingPeriod);
        }

        public DataTable GetAllProjectSendToAccountsDetailsBasedonDomain(int ProjectID, string BillingPeriod, int DomainId)
        {
            return dalTracking.GetAllProjectSendToAccountsDetailsBasedonDomain(ProjectID, BillingPeriod, DomainId);
        }

        public DataTable GetAllProjectBillingPeriod()
        {
            return dalTracking.GetAllProjectBillingPeriod();
        }

        public int InsertAllProjectSendToAccountsDetails(int ProjectID, string BillingPeriod, int AddedBy)
        {
            return dalTracking.InsertAllProjectSendToAccountsDetails(ProjectID, BillingPeriod, AddedBy);
        }

        //public int UpdateAllProjectSendToAccountsDetails(int ProjectID, string BillingPeriod)
        //{
        //    return dalTracking.InsertAllProjectSendToAccountsDetails(ProjectID, BillingPeriod);
        //}

        public string getActualColumnName(string HeaderName, int ProjectID)
        {
            return dalTracking.getActualColumnName(HeaderName, ProjectID);
        }



        public DataTable GetIsUniqueColumnForHeader(int ProjectID)
        {
            return dalTracking.GetIsUniqueColumnForHeader(ProjectID);
        }

        public int SetIsverifyTrueForBillingOrder(int ProjectID, string TempOrderNumberColumn, string OrderNumber, string OrderDate, string TempOrderDateColumn, string BillingPeriod, int AddedBy)
        {
            return dalTracking.SetIsverifyTrueForBillingOrder(ProjectID, TempOrderNumberColumn, OrderNumber, OrderDate, TempOrderDateColumn, BillingPeriod, AddedBy);
        }

        public DataTable GetAllProjectApprovedforClient(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetAllProjectApprovedforClient(ProjectID, BillingPeriod);
        }

        public int InsertAllProjectSendToAccountsDetailsBackToProduction(int ProjectID, string BillingPeriod, int AddedBy)
        {
            return dalTracking.InsertAllProjectSendToAccountsDetailsBackToProduction(ProjectID, BillingPeriod, AddedBy);
        }

        public int UpdateSendToclient(Hashtable ht)
        {
            return dalTracking.UpdateSendToclient(ht);
        }

        public DataTable GetAllProjectforClientApproved(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetAllProjectforClientApproved(ProjectID, BillingPeriod);
        }

        public DataTable GetInvoiceDetails(int InvoiceID)
        {
            return dalTracking.GetInvoiceDetails(InvoiceID);
        }

        public DataTable GetInvoiceDetailsGroup(int InvoiceID)
        {
            return dalTracking.GetInvoiceDetailsGroup(InvoiceID);
        }
        public int UpdateInvoiceDetails(Hashtable ht)
        {
            return dalTracking.UpdateInvoiceDetails(ht);
        }

        public int UpdateInvoiceDetailsGroup(Hashtable ht)
        {
            return dalTracking.UpdateInvoiceDetailsGroup(ht);
        }
        public DataTable GetInvoiceRemarkDetails(int InvoiceID)
        {
            return dalTracking.GetInvoiceRemarkDetails(InvoiceID);
        }

        public DataTable GetInvoiceRemarkDetailsGroup(int InvoiceID)
        {
            return dalTracking.GetInvoiceRemarkDetailsGroup(InvoiceID);
        }

        public int Insertbillingparam(Hashtable Htparam)
        {
            return dalTracking.Insertbillingparam(Htparam);
        }

        public DataTable bindGridBillingRates()
        {
            return dalTracking.bindGridBillingRates();
        }

        public int DeleteProjectcost(int Projectvalue)
        {
            return dalTracking.DeleteProjectcost(Projectvalue);
        }

        public DataTable GetbyProject_Cost_IdReq(int Project_Cost_Id)
        {
            return dalTracking.GetbyProject_Cost_IdReq(Project_Cost_Id);
        }

        public int UpdateProjectCost(Hashtable Htparam)
        {
            return dalTracking.UpdateProjectCost(Htparam);
        }

        public int Insertbillingcostdeatils(Hashtable Htparam)
        {
            return dalTracking.Insertbillingcostdeatils(Htparam);
        }

        public DataTable Viewprocostdetails(string ProjectCost_Id)
        {
            return dalTracking.Viewprocostdetails(ProjectCost_Id);
        }

        public DataTable GetProjectId_by_Cost_Id(int Project_Cost_Id)
        {
            return dalTracking.GetProjectId_by_Cost_Id(Project_Cost_Id);
        }

        public DataTable GetProjectName(int Project_Id)
        {
            return dalTracking.GetProjectName(Project_Id);
        }

        public DataTable GetColumndata(string ProjectId)
        {
            return dalTracking.GetColumndata(ProjectId);
        }

        public DataTable GetCostDetails_ById(int Cost_Id)
        {
            return dalTracking.GetCostDetails_ById(Cost_Id);
        }

        public DataTable GetProductTypeByProjectID(string ProjectId)
        {
            return dalTracking.GetProductTypeByProjectID(ProjectId);
        }

        public DataTable GetStatusByProjectID(string ProjectId)
        {
            return dalTracking.GetStatusByProjectID(ProjectId);
        }

        public int Updatebillingcostdetails(Hashtable Htparam)
        {
            return dalTracking.Updatebillingcostdetails(Htparam);
        }

        public int UpdateInvoiceRemark(Hashtable Htparam)
        {
            return dalTracking.UpdateInvoiceRemark(Htparam);
        }

        public int UpdateInvoiceRemarkGroup(Hashtable Htparam)
        {
            return dalTracking.UpdateInvoiceRemarkGroup(Htparam);
        }

        public DataTable GetClientDetails(int ClientId)
        {
            return dalTracking.GetClientDetails(ClientId);
        }

        public int UpdateClientDetails(Hashtable ht)
        {
            return dalTracking.UpdateClientDetails(ht);
        }

        public int InsertClientDetails(Hashtable ht)
        {
            return dalTracking.InsertClientDetails(ht);
        }

        public DataTable GetAllClientDetails()
        {
            return dalTracking.GetAllClientDetails();
        }

        public DataTable GetAllClientListProjectWise(int InvoiceId)
        {
            return dalTracking.GetAllClientListProjectWise(InvoiceId);
        }

        public int DeleteClientDetails(int ClientID, int userId)
        {
            return dalTracking.DeleteClientDetails(ClientID, userId);
        }

        public DataTable GetProjectClientConfiguration(int ProjectId)
        {
            return dalTracking.GetProjectClientConfiguration(ProjectId);
        }

        public int UpdateInvoicePath(int InvId, string strAttachmentPath)
        {
            return dalTracking.UpdateInvoicePath(InvId, strAttachmentPath);
        }

        public int UpdateInvoiceClient(int InvId, int strClientId)
        {
            return dalTracking.UpdateInvoiceClient(InvId, strClientId);
        }

        public DataTable GetCrystalReport(int ProjectID, string BillingPeriod)
        {
            return dalTracking.GetCrystalReport(ProjectID, BillingPeriod);
        }

        public DataTable GetAllOfficialIdsUser()
        {
            return dalTracking.GetAllOfficialIdsUser();
        }

        #region Reviewers
        public DataTable GetAllReviewers()
        {
            return dalTracking.GetAllReviewers();
        }

        public DataTable GetReviewersByID(int ReviewerId)
        {
            return dalTracking.GetReviewersByID(ReviewerId);
        }

        public int InsertReviewerDetails(Hashtable ht)
        {
            return dalTracking.InsertReviewerDetails(ht);
        }

        public int UpdateReviewerDetails(Hashtable ht)
        {
            return dalTracking.UpdateReviewerDetails(ht);
        }

        #endregion

        #region Email COnfiguration

        public DataTable GetAllEmailConfiguration()
        {
            return dalTracking.GetAllEmailConfiguration();
        }

        public DataTable GetEmailConfigurationByID(int ClientId)
        {
            return dalTracking.GetEmailConfigurationByID(ClientId);
        }

        public int InsertEmailConfiguration(Hashtable ht)
        {
            return dalTracking.InsertEmailConfiguration(ht);
        }

        public int UpdateEmailConfiguration(Hashtable ht)
        {
            return dalTracking.UpdateEmailConfiguration(ht);
        }

        #endregion

        public DataTable GetAllClientDetailsProjectWise(int ProjectId)
        {
            return dalTracking.GetAllClientDetailsProjectWise(ProjectId);
        }

        #region Product

        public DataTable GetAllProductProjectWise()
        {
            return dalTracking.GetAllProductProjectWise();
        }

        public DataTable GetAllProductProjectWiseByID(int Product_Id)
        {
            return dalTracking.GetAllProductProjectWiseByID(Product_Id);
        }

        public int InsertProduct(Hashtable ht)
        {
            return dalTracking.InsertProduct(ht);
        }

        public int UpdateProduct(Hashtable ht)
        {
            return dalTracking.UpdateProduct(ht);
        }

        #endregion

        public DataTable EmailDetailsByInvoiceID(int InvoiceID)
        {
            return dalTracking.EmailDetailsByInvoiceID(InvoiceID);
        }

        public DataTable EmailDetailsByInvoiceIDGroup(int InvoiceID)
        {
            return dalTracking.EmailDetailsByInvoiceIDGroup(InvoiceID);
        }

        public DataTable GetBillingDetailsforReport()
        {
            return dalTracking.GetBillingDetailsforReport();
        }

        public DataTable GetBillingDetailsforReportTest(int DomainId, string BillingPeriod)
        {
            return dalTracking.GetBillingDetailsforReportTest(DomainId, BillingPeriod);
        }

        public DataTable GetBillingDetailsforReportPeriodWise(string Month, int Year)
        {
            return dalTracking.GetBillingDetailsforReportPeriodWise(Month, Year);
        }

        #endregion

        public DataTable ViewprocostdetailsHistory(string Project_Id)
        {
            return dalTracking.ViewprocostdetailsHistory(Project_Id);
        }

        public DataTable BindProductTypeprojectWise(string Project_Id)
        {
            return dalTracking.BindProductTypeprojectWise(Project_Id);
        }

        #region FinalStatus

        public DataTable GetAllFinalStatusProjectWise()
        {
            return dalTracking.GetAllFinalStatusProjectWise();
        }

        public DataTable GetAllFinalStatusProjectWiseByID(int Product_Id)
        {
            return dalTracking.GetAllFinalStatusProjectWiseByID(Product_Id);
        }

        public int InsertFinalStatus(Hashtable ht)
        {
            return dalTracking.InsertFinalStatus(ht);
        }

        public int UpdateFinalStatus(Hashtable ht)
        {
            return dalTracking.UpdateFinalStatus(ht);
        }

        public DataTable BindFinalStatusprojectWise(string Project_Id)
        {
            return dalTracking.BindFinalStatusprojectWise(Project_Id);
        }

        public DataTable getalldomains(int EmployeeId)
        {
            return dalTracking.getalldomains(EmployeeId);
        }

        public DataTable GetAllProjectByDomainWise(int DomainId, int EmployeeId)
        {
            return dalTracking.GetAllProjectByDomainWise(DomainId, EmployeeId);
        }

        public DataTable BindProjectApprovalDetails(int Project_Id)
        {
            return dalTracking.BindProjectApprovalDetails(Project_Id);
        }

        public int InsertbillingcostApproval(Hashtable Htparam)
        {
            return dalTracking.InsertbillingcostApproval(Htparam);
        }

        public DataTable BindVerifyProject()
        {
            return dalTracking.BindVerifyProject();
        }

        public DataTable BindProjectForPriceDetails()
        {
            return dalTracking.BindProjectForPriceDetails();
        }

        public DataTable BindVerifyProjectById(int Project_Id)
        {
            return dalTracking.BindVerifyProjectById(Project_Id);
        }

        public DataTable BindVerifyProjectForModifiedRates()
        {
            return dalTracking.BindVerifyProjectForModifiedRates();
        }

        #endregion

        public DataTable getallMarketingEmployee()
        {
            return dalTracking.getallMarketingEmployee();
        }

        public int UpdateProjectApprovalRequest(Hashtable InsertprojectApproval)
        {
            return dalTracking.UpdateProjectApprovalRequest(InsertprojectApproval);
        }

        public int InsertProjectApprovalRequest(Hashtable InsertprojectApproval)
        {
            return dalTracking.InsertProjectApprovalRequest(InsertprojectApproval);
        }

        public DataTable ViewAllProjectApp()
        {
            return dalTracking.ViewAllProjectApp();
        }

        public DataTable GetProjectApprovalInformation(int PIA_Id)
        {
            return dalTracking.GetProjectApprovalInformation(PIA_Id);
        }

        public DataTable GetbyProjectApproval_IdReq(int ProjectApproval_Id)
        {
            return dalTracking.GetbyProjectApproval_IdReq(ProjectApproval_Id);
        }

        public int InsertSalesInformation(Hashtable InsertprojectApproval)
        {
            return dalTracking.InsertSalesInformation(InsertprojectApproval);
        }

        public int InsertBillParameter(Hashtable htParam)
        {
            return dalTracking.InsertBillParameter(htParam);
        }

        public int InsertBillParameterUW(Hashtable htParam)
        {
            return dalTracking.InsertBillParameterUW(htParam);
        }

        public int UpdateSalesInformation(Hashtable InsertprojectApproval)
        {
            return dalTracking.UpdateSalesInformation(InsertprojectApproval);
        }

        public DataTable GetBiilingValuesDomainwise(int ProjectApproval_Id)
        {
            return dalTracking.GetBiilingValuesDomainwise(ProjectApproval_Id);
        }

        public DataTable ViewAllProjectGroup()
        {
            return dalTracking.ViewAllProjectGroup();
        }

        public DataTable ViewAllGroupProjects(string Groupname)
        {
            return dalTracking.ViewAllGroupProjects(Groupname);
        }

        public DataTable GetBillingDetailsCount(string BillingPeriod)
        {
            return dalTracking.GetBillingDetailsCount(BillingPeriod);
        }

        public int ApproveBillParameter(Hashtable htParam)
        {
            return dalTracking.ApproveBillParameter(htParam);
        }

        public DataSet BindTestOrders(int DomainId, string BillingPeriod)
        {
            return dalTracking.BindTestOrders(DomainId, BillingPeriod);
        }

        public DataTable BindSummaryReport(string GroupName, string BillingPeriod)
        {
            return dalTracking.BindSummaryReport(GroupName, BillingPeriod);
        }

        public int InsertGroupAttachmentPath(string GroupName, string BillingPeriod, string strAttachmentPath)
        {
            return dalTracking.InsertGroupAttachmentPath(GroupName, BillingPeriod, strAttachmentPath);
        }

        public int InsertGroupAttachmentPath_QC(string GroupName, string BillingPeriod, string strAttachmentPath, string InvoiceNumber)
        {
            return dalTracking.InsertGroupAttachmentPath_QC(GroupName, BillingPeriod, strAttachmentPath, InvoiceNumber);
        }

        public DataTable GetSummaryReportAttachments(string GroupName, string BillingPeriod)
        {
            return dalTracking.GetSummaryReportAttachments(GroupName, BillingPeriod);
        }

        public DataTable GetClientDetails(string GroupName, string BillingPeriod)
        {
            return dalTracking.GetClientDetails(GroupName, BillingPeriod);
        }

        public DataTable GetSummaryReportInvoice(string GroupName, string BillingPeriod)
        {
            return dalTracking.GetSummaryReportInvoice(GroupName, BillingPeriod);
        }

        public int InsertVendorCost(Hashtable htdailyV)
        {
            return dalTracking.InsertVendorCost(htdailyV);
        }
        public int InsertOtherCost(Hashtable htdailyV1)
        {
            return dalTracking.InsertOtherCost(htdailyV1);
        }
        public DataTable GetVendorCost()
        {
            return dalTracking.GetVendorCost();
        }
        public DataTable GetOtherCost()
        {
            return dalTracking.GetOtherCost();
        }
        public DataTable GetAllProject()
        {
            return dalTracking.GetAllProject();
        }
        public int UpdateVendorCost(string Month, int CostId, string Year, string VolumeOutsourced, string VendorBilling, string TotalCosting)
        {
            return dalTracking.UpdateVendorCost(Month, CostId, Year, VolumeOutsourced, VendorBilling, TotalCosting);
        }
        public int UpdateOtherCost(string Month, int OtherCId, string Year, string CostType, string Amount, string Remark)
        {
            return dalTracking.UpdateOtherCost(Month, OtherCId, Year, CostType, Amount, Remark);
        }
    }
}