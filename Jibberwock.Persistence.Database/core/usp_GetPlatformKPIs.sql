CREATE PROCEDURE [core].[usp_GetPlatformKPIs]
	@KPI_Period_Days int
AS
BEGIN
	set nocount on;
	set xact_abort on;

	/*
	Result set 1: # of users, # of tenants, status of all components, # of email batches pending send
      Result set 2: last X days, # of audit trail entries
      Result set 3: last X days, # of exceptions*/

	declare @currDate date = sysutcdatetime()
	declare @kpiStartDate date = dateadd(DAY, -@KPI_Period_Days, @currDate)
	declare @userCount bigint
	declare @tenantCount bigint
	declare @componentsHealthy bit = 1
	declare @pendingEmailBatches bigint

	select @userCount = count_big(1) from [security].[User]
	select @tenantCount = count_big(1) from [tenants].[Tenant]
	select @componentsHealthy = 0 from components.ExternalComponentStatus where Available = 0
	select @pendingEmailBatches = count_big(1) from core.EmailBatch where Processed_Successfully is null

	-- Result set 1: # of users, # of tenants, whether or not all external components are healthy, # of unprocessed email batches
	select @userCount as UserCount, @tenantCount as TenantCount, @componentsHealthy as ComponentsHealthy, @pendingEmailBatches as PendingEmailBatches

	-- Result set 2: # of audit trail entries, grouped by date. Filtered by a time period
	select cast(Occurrence_Time as date) as [Date], count(1) as EntryCount
	from [security].AuditTrail
	where Occurrence_Time >= @kpiStartDate
	group by cast(Occurrence_Time as date)
END