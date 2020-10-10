CREATE PROCEDURE [core].[usp_ListNotifications]
	@User_ID bigint,
	@Tenant_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select n.Notification_ID as Id, n.Status_ID as [Status], n.[Type_ID] as [Type],
		n.Creation_Date as CreationDate, n.[Start_Date] as StartDate, n.[End_Date] as EndDate,
		n.[Subject], n.[Message], n.Allow_Dismissal as AllowDismissal,
		np.Notification_Priority_ID as Id, np.[Name], np.[Order],
		usr.[User_ID] as Id,
		ten.Tenant_ID as Id,
		eb.Email_Batch_ID as Id
	from core.[Notification] as n
	inner join core.NotificationPriority as np
		on (np.Notification_Priority_ID = n.Priority_ID)
	left outer join [security].[User] as usr
		on (usr.[User_ID] = n.[User_ID])
	left outer join [tenants].[Tenant] as ten
		on (ten.Tenant_ID = n.Tenant_ID)
	left outer join core.EmailBatch as eb
		on (eb.Email_Batch_ID = n.Email_Batch_ID)
	-- Three situations:
	-- 1. We want all notifications for a tenant. User_ID = null, Tenant_ID <> null
	-- 2. We want all notifications for a user. User_ID <> null, Tenant_ID = null
	-- 3. We want all global notifications. User_ID = null, Tenant_ID = null
	where (@User_ID is null and @Tenant_ID is not null and n.Tenant_ID = @Tenant_ID)
		or (@User_ID is not null and @Tenant_ID is null and n.[User_ID] = @User_ID)
		or (@User_ID is null and @Tenant_ID is null and n.[User_ID] is null and n.Tenant_ID is null)
END
