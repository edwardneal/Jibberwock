CREATE PROCEDURE [core].[usp_GetNotificationEmailBatchByID]
	@Email_Batch_ID bigint
AS
BEGIN

	declare @notificationId bigint
	declare @userId bigint
	declare @tenantId bigint
	declare @subject nvarchar(128)
	declare @message nvarchar(max)

	-- First result set contains the high-level message details: subject, message, etc.
	-- Second result set contains the list of email addresses

	select @notificationId = Notification_ID,
		@userId = [User_ID], @tenantId = Tenant_ID,
		@subject = [Subject], @message = [Message]
	from core.[Notification]
	where Email_Batch_ID = @Email_Batch_ID

	-- Result set #1!
	select @notificationId as Id, @subject as [Subject], @message as [Message]

	-- Second result set is trickier because we've got to handle the various types of notification
	if @tenantId is not null and @userId is null
	begin
		-- Look at a tenant's "tenant_members" well-known group membership,
		-- filter to the enabled users with enabled memberships, then return those
		select distinct usr.Email_Address
		from [security].[User] as usr
		inner join [security].[WellKnownGroup] as wkg
			on (wkg.Securable_Resource_ID = @tenantId)
		inner join [security].[SecurityGroupMembership] as sgm
			on (sgm.Security_Group_ID = wkg.Security_Group_ID
				and sgm.[User_ID] = usr.[User_ID])
		where wkg.Well_Known_Group_Type_ID = 6
			and sgm.[Enabled] = 1
			and usr.[Enabled] = 1
	end
	else if @userId is not null and @tenantId is null
	begin
		-- Simple: get the user's email address - as long as it's enabled
		select distinct usr.Email_Address
		from [security].[User] as usr
		where usr.[User_ID] = @userId
			and usr.[Enabled] = 1
	end
	else if @userId is null and @tenantId is null
	begin
		-- Send a message to every enabled user
		select distinct usr.Email_Address
		from [security].[User] as usr
		where usr.[Enabled] = 1
	end
END
