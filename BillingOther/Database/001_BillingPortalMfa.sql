IF OBJECT_ID(N'dbo.BillingPortalMfa', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BillingPortalMfa
    (
        EmployeeId INT NOT NULL CONSTRAINT PK_BillingPortalMfa PRIMARY KEY,
        ProtectedSecret NVARCHAR(512) NOT NULL,
        IsEnabled BIT NOT NULL CONSTRAINT DF_BillingPortalMfa_IsEnabled DEFAULT (1),
        EnrolledOnUtc DATETIME2(0) NOT NULL CONSTRAINT DF_BillingPortalMfa_EnrolledOnUtc DEFAULT (SYSUTCDATETIME()),
        LastVerifiedOnUtc DATETIME2(0) NULL
    );
END;
