/* Project 757-005 (ProjectID 400) deployment patch for InfinityBilling. */
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @Definition NVARCHAR(MAX) = OBJECT_DEFINITION(
    OBJECT_ID(N'dbo.usp_GetProjectCost3_HighestFirst_FTE_Extra', N'P'));
DECLARE @BranchStart INT;
DECLARE @BranchEnd INT;
DECLARE @Branch NVARCHAR(MAX);

IF @Definition IS NULL
    THROW 50001, 'Procedure dbo.usp_GetProjectCost3_HighestFirst_FTE_Extra was not found.', 1;

SET @BranchStart = CHARINDEX(N'ELSE IF @ProjectID = 400', @Definition);
SET @BranchEnd = CHARINDEX(N'ELSE IF @ProjectID = 385', @Definition, @BranchStart + 1);

IF @BranchStart = 0 OR @BranchEnd = 0 OR @BranchEnd <= @BranchStart
    THROW 50002, 'Expected ProjectID 400 procedure branch was not found.', 1;

SET @Branch = SUBSTRING(@Definition, @BranchStart, @BranchEnd - @BranchStart);

IF CHARINDEX(N'dbo.FTEEntry', @Branch) = 0
    THROW 50003, 'Expected ProjectID 400 FTEEntry references were not found.', 1;

/* Use the authoritative verified ERP rows consistently in this branch. */
SET @Branch = REPLACE(@Branch, N'dbo.FTEEntry', N'InfinityERP.dbo.FTEEntry');
SET @Branch = REPLACE(@Branch, N'E.isVerify IS NULL', N'E.isVerify = 1');

SET @Definition = STUFF(
    @Definition, @BranchStart, @BranchEnd - @BranchStart, @Branch);

IF UPPER(LEFT(LTRIM(@Definition), 16)) = N'CREATE PROCEDURE'
    SET @Definition = STUFF(
        @Definition,
        CHARINDEX(N'CREATE PROCEDURE', UPPER(@Definition)),
        LEN(N'CREATE PROCEDURE'),
        N'ALTER PROCEDURE');
ELSE
    THROW 50004, 'Unexpected procedure definition header.', 1;

EXEC sys.sp_executesql @Definition;
COMMIT TRANSACTION;

EXEC dbo.usp_GetProjectCost3_HighestFirst_FTE_Extra
    @ProjectID = 400,
    @BillingPeriod = N'01-Sep-2026 ~ 30-Sep-2026';
