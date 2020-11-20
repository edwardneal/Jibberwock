CREATE PROCEDURE [core].[usp_ListClientNotifications]
	@Calling_User_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	-- Unlike core.usp_ListNotifications, this is called to get all notifications which are applicable to a particular user.
	select n.Notification_ID as Id, n.Status_ID as [Status], n.[Type_ID] as [Type],
		n.Creation_Date as CreationDate, n.[Start_Date] as StartDate, n.[End_Date] as EndDate,
		n.[Subject], n.[Message], n.Allow_Dismissal as AllowDismissal,
		np.Notification_Priority_ID as Id, np.[Name], np.[Order],
		n.[User_ID] as Id,
		ten.Tenant_ID as Id
	from core.[Notification] as n
	inner join core.NotificationPriority as np
		on (np.Notification_Priority_ID = n.Priority_ID)
	left outer join [tenants].[Tenant] as ten
		on (ten.Tenant_ID = n.Tenant_ID)
	-- Expand into the Tenant Members group
	left outer join [security].[WellKnownGroup] as tenWkg
		on (tenWkg.Securable_Resource_ID = ten.Tenant_ID and tenWkg.Well_Known_Group_Type_ID = 6)
	-- Try to find an enabled membership of the above group for the calling user
	left outer join [security].[SecurityGroupMembership] as gm
		on (gm.Security_Group_ID = tenWkg.Security_Group_ID
			and gm.[User_ID] = @Calling_User_ID
			and gm.[Enabled] = 1)
	-- Check #1: notifications must be for this user account
	where (
		-- Global notification
		(n.[User_ID] is null and n.Tenant_ID is null)
		-- Direct notification for this user
		or (n.[User_ID] is not null and n.Tenant_ID is null and n.[User_ID] = @Calling_User_ID)
		-- Notification for all users of this tenant
		or (n.[User_ID] is null and n.Tenant_ID is not null and gm.Security_Group_Membership_ID is not null)
	-- Checks #2, 3: notifications need to be for the correct date
	) and (
		(n.[Start_Date] is null)
		or (n.[Start_Date] is not null and n.[Start_Date] <= SYSDATETIMEOFFSET())
	) and (
		(n.End_Date is null)
		or (n.End_Date is not null and n.End_Date >= SYSDATETIMEOFFSET())
	-- Check #4: notification must not be cancelled
	) and (
		n.Status_ID <> 2
	)
	-- Check #5 (TODO): notification must not have been dismissed
END
