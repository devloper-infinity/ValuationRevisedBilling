/* Deploy to InfinityBilling. SQL Server 2016 compatible. Returns all dashboard datasets in one call. */
IF OBJECT_ID('dbo.IB_usp_AccountsManagementDashboard','P') IS NOT NULL
    DROP PROCEDURE dbo.IB_usp_AccountsManagementDashboard;
GO
CREATE PROCEDURE dbo.IB_usp_AccountsManagementDashboard
 @Year int=NULL,@Month int=NULL,@DomainID int=NULL,@ProjectID int=NULL,
 @BillingCycle nvarchar(50)=NULL,@BillingStatus nvarchar(50)=NULL,@Client nvarchar(200)=NULL,@DetailProjectID int=NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET @Year=ISNULL(@Year,YEAR(GETDATE()));

 SELECT IM.ProjectID,IM.BillingPeriod,MAX(IM.InvoiceID) InvoiceID,
  MAX(IM.InvoiceNumber) InvoiceNumber,
  SUM(CASE WHEN ISNUMERIC(REPLACE(ISNULL(IM.Invoice_Amount,'0'),',',''))=1 THEN CONVERT(money,REPLACE(IM.Invoice_Amount,',','')) ELSE 0 END) Amount,
  SUM(CASE WHEN ISNUMERIC(REPLACE(ISNULL(IM.Invoice_OrderCount,'0'),',',''))=1 THEN CONVERT(bigint,CONVERT(money,REPLACE(IM.Invoice_OrderCount,',',''))) ELSE 0 END) OrderCount,
  MAX(IM.AddedDate) InvoiceDate,MAX(IM.InvoiceSentEmailDate) SentDate
 INTO #Invoice FROM dbo.InfinityBilling_InvoiceMaster IM WHERE ISNULL(IM.DFlag,0)=0
 GROUP BY IM.ProjectID,IM.BillingPeriod;

 SELECT OD.ProjectID,OD.BillingPeriod,COUNT_BIG(1) OrderCount INTO #Orders
 FROM dbo.InfinityBilling_OrderDetail OD GROUP BY OD.ProjectID,OD.BillingPeriod;

 SELECT B.ProjectId ProjectID,P.ProjectName ProjectNumber,ISNULL(C.Process,P.ProjectName) ProjectName,
  D.DomainID,D.DomainName,ISNULL(NULLIF(B.BillingCycle,''),C.BillingCycle) BillingCycle,B.BillingPeriod,
  ISNULL(NULLIF(B.Client,''),'Unspecified') Client,B.ClientBillingDueDate ExpectedDate,B.AddedDate BillingCreatedDate,
  I.InvoiceID,I.InvoiceNumber,ISNULL(I.Amount,0) Amount,ISNULL(I.OrderCount,O.OrderCount) OrderCount,I.InvoiceDate,I.SentDate,
  CASE WHEN I.InvoiceID IS NULL AND B.ClientBillingDueDate<CONVERT(date,GETDATE()) THEN 'Overdue'
       WHEN I.InvoiceID IS NULL THEN 'Yet to Bill' WHEN I.SentDate IS NOT NULL THEN 'Sent to Client' ELSE 'Invoice Generated' END Status,
  CASE WHEN I.InvoiceID IS NULL THEN 1 ELSE 0 END YetToBill,
  CASE WHEN I.InvoiceID IS NOT NULL THEN 1 ELSE 0 END InvoiceGenerated,
  CASE WHEN I.SentDate IS NOT NULL THEN 1 ELSE 0 END SentToClient,
  CASE WHEN I.InvoiceID IS NOT NULL AND I.SentDate IS NULL THEN I.Amount ELSE 0 END PendingAmount
 INTO #Base
 FROM dbo.ProjectBillingDetails B
 JOIN InfinityERP.dbo.Project P ON P.ProjectID=B.ProjectId
 LEFT JOIN InfinityERP.dbo.ClientFeedback C ON C.ProjectId=B.ProjectId
 LEFT JOIN InfinityERP.dbo.Domain D ON D.DomainID=C.DomainId
 LEFT JOIN #Invoice I ON I.ProjectID=B.ProjectId AND I.BillingPeriod=B.BillingPeriod
 LEFT JOIN #Orders O ON O.ProjectID=B.ProjectId AND O.BillingPeriod=B.BillingPeriod
 WHERE B.BillingPeriod IS NOT NULL
 AND B.BillingPeriod LIKE '%'+CONVERT(varchar(4),@Year)+'%'
 AND (@Month IS NULL OR B.BillingPeriod LIKE '%-'+LEFT(DATENAME(month,DATEADD(month,@Month-1,0)),3)+'-'+CONVERT(varchar(4),@Year)+'%')
 AND (@DomainID IS NULL OR D.DomainID=@DomainID) AND (@ProjectID IS NULL OR B.ProjectId=@ProjectID)
 AND (@BillingCycle IS NULL OR ISNULL(NULLIF(B.BillingCycle,''),C.BillingCycle)=@BillingCycle)
 AND (@Client IS NULL OR B.Client=@Client);

 /* Active configured projects without a billing record for the selected/current period. */
 IF ISNULL(@Month,MONTH(GETDATE()))=MONTH(GETDATE()) AND @Year=YEAR(GETDATE())
 INSERT #Base(ProjectID,ProjectNumber,ProjectName,DomainID,DomainName,BillingCycle,BillingPeriod,Client,Amount,OrderCount,Status,YetToBill,InvoiceGenerated,SentToClient,PendingAmount)
 SELECT P.ProjectID,P.ProjectName,ISNULL(C.Process,P.ProjectName),D.DomainID,D.DomainName,C.BillingCycle,DATENAME(month,GETDATE())+' '+CONVERT(varchar(4),@Year),'Unspecified',0,0,'No Activity',1,0,0,0
 FROM InfinityERP.dbo.Project P JOIN InfinityERP.dbo.ClientFeedback C ON C.ProjectId=P.ProjectID LEFT JOIN InfinityERP.dbo.Domain D ON D.DomainID=C.DomainId
 WHERE ISNULL(P.IsDelete,0)=0 AND ISNULL(P.Status,1)=1 AND (@DomainID IS NULL OR D.DomainID=@DomainID) AND (@ProjectID IS NULL OR P.ProjectID=@ProjectID)
 AND (@BillingCycle IS NULL OR C.BillingCycle=@BillingCycle) AND NOT EXISTS(SELECT 1 FROM #Base X WHERE X.ProjectID=P.ProjectID);

 IF @BillingStatus IS NOT NULL DELETE FROM #Base WHERE Status<>@BillingStatus;

 IF @DetailProjectID IS NOT NULL
 BEGIN
  SELECT ProjectID,BillingPeriod,ISNULL(OrderCount,0) OrderCount,InvoiceNumber,REPLACE(CONVERT(varchar(11),InvoiceDate,106),' ','-') InvoiceDate,Amount,REPLACE(CONVERT(varchar(11),SentDate,106),' ','-') SentDate,Status,PendingAmount,
   REPLACE(CONVERT(varchar(11),ISNULL(SentDate,ISNULL(InvoiceDate,BillingCreatedDate)),106),' ','-') LastUpdate FROM #Base WHERE ProjectID=@DetailProjectID ORDER BY ISNULL(InvoiceDate,BillingCreatedDate) DESC;
  RETURN;
 END

 /* 1: filter values */
 SELECT DISTINCT CASE WHEN ISNUMERIC(RIGHT(RTRIM(B.BillingPeriod),4))=1 THEN CONVERT(int,RIGHT(RTRIM(B.BillingPeriod),4)) ELSE @Year END [Year],D.DomainID,D.DomainName,P.ProjectID,P.ProjectName ProjectNumber,ISNULL(NULLIF(B.Client,''),'Unspecified') Client
 FROM dbo.ProjectBillingDetails B JOIN InfinityERP.dbo.Project P ON P.ProjectID=B.ProjectId LEFT JOIN InfinityERP.dbo.ClientFeedback C ON C.ProjectId=P.ProjectID LEFT JOIN InfinityERP.dbo.Domain D ON D.DomainID=C.DomainId
 WHERE B.BillingPeriod IS NOT NULL;
 /* 2: KPIs */
 SELECT COUNT(DISTINCT ProjectID) TotalProjects,SUM(ISNULL(OrderCount,0)) TotalOrders,SUM(Amount) TotalBillingAmount,
  SUM(CASE WHEN YetToBill=1 THEN Amount ELSE 0 END) YetToBillAmount,SUM(YetToBill) YetToBillCount,
  SUM(CASE WHEN InvoiceGenerated=1 THEN Amount ELSE 0 END) InvoiceGeneratedAmount,SUM(CASE WHEN SentToClient=1 THEN Amount ELSE 0 END) SentToClientAmount,
  SUM(PendingAmount) PendingToSendAmount,COUNT(DISTINCT CASE WHEN YetToBill=1 THEN ProjectID END) PendingProjects,COUNT(DISTINCT CASE WHEN InvoiceGenerated=1 THEN ProjectID END) CompletedProjects FROM #Base;
 /* 3: domain summary */
 SELECT DomainID,ISNULL(DomainName,'Unassigned') DomainName,COUNT(DISTINCT ProjectID) Projects,SUM(ISNULL(OrderCount,0)) Orders,SUM(Amount) TotalAmount,SUM(YetToBill) YetToBill,SUM(InvoiceGenerated) InvoiceGenerated,SUM(SentToClient) SentToClient,SUM(PendingAmount) PendingAmount,
  CONVERT(decimal(6,2),CASE WHEN COUNT(*)=0 THEN 0 ELSE SUM(InvoiceGenerated)*100.0/COUNT(*) END) BillingPercent FROM #Base GROUP BY DomainID,DomainName ORDER BY TotalAmount DESC;
 /* 4: project status */
 SELECT DomainID,DomainName,ProjectID,ProjectNumber,ProjectName,BillingCycle,BillingPeriod,SUM(ISNULL(OrderCount,0)) OrderCount,SUM(Amount) Amount,SUM(YetToBill) YetToBill,SUM(InvoiceGenerated) InvoiceGenerated,SUM(SentToClient) SentToClient,SUM(PendingAmount) PendingAmount,
  REPLACE(CONVERT(varchar(11),MAX(ISNULL(SentDate,ISNULL(InvoiceDate,BillingCreatedDate))),106),' ','-') LastBillingDate,
  CASE WHEN SUM(SentToClient)>0 AND SUM(YetToBill)>0 THEN 'Partially Billed' WHEN MAX(CASE Status WHEN 'Overdue' THEN 6 WHEN 'Yet to Bill' THEN 5 WHEN 'Invoice Generated' THEN 4 WHEN 'No Activity' THEN 3 ELSE 1 END)=6 THEN 'Overdue' WHEN SUM(YetToBill)>0 THEN MAX(Status) WHEN SUM(SentToClient)>0 THEN 'Sent to Client' ELSE MAX(Status) END Status
 FROM #Base GROUP BY DomainID,DomainName,ProjectID,ProjectNumber,ProjectName,BillingCycle,BillingPeriod ORDER BY DomainName,ProjectNumber;
 /* 5: trend */
 SELECT BillingPeriod Period,SUM(ISNULL(OrderCount,0)) OrderCount,SUM(Amount) BillingAmount,SUM(YetToBill) YetToBill,SUM(CASE WHEN SentToClient=1 THEN Amount ELSE 0 END) SentToClient,SUM(PendingAmount) Pending FROM #Base GROUP BY BillingPeriod ORDER BY MIN(ISNULL(InvoiceDate,BillingCreatedDate));
 /* 6: attention */
 SELECT TOP 200 CASE WHEN Status='Overdue' THEN 'High' ELSE 'Medium' END Priority,
  CASE WHEN Status='Overdue' THEN 'Billing period overdue' WHEN Status='No Activity' THEN 'No billing activity for current period' WHEN InvoiceGenerated=1 AND SentToClient=0 THEN 'Invoice generated but not sent to client' WHEN ISNULL(OrderCount,0)>0 AND Amount=0 THEN 'Orders present with zero billing amount' WHEN PendingAmount>=100000 THEN 'Large pending amount' ELSE 'Project pending billing' END Issue,
  DomainName,ProjectID,ProjectNumber,BillingPeriod,Amount,REPLACE(CONVERT(varchar(11),ExpectedDate,106),' ','-') ExpectedDate,Status
 FROM #Base WHERE Status IN('Overdue','No Activity','Yet to Bill') OR (InvoiceGenerated=1 AND SentToClient=0) OR (ISNULL(OrderCount,0)>0 AND Amount=0) OR PendingAmount>=100000 ORDER BY CASE WHEN Status='Overdue' THEN 0 ELSE 1 END,Amount DESC;
END
GO
