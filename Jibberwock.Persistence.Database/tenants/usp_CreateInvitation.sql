CREATE PROCEDURE [tenants].[usp_CreateInvitation]
	@Tenant_ID bigint,
	@Email_Address nvarchar(256),
	@Identity_Provider nvarchar(32),
	@Expiration_Date datetimeoffset(7),
	@Send_As_Email bit
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
		-- See core.usp_CreateNotification for a fuller explanation of the rationale behind
		-- creating the email batch first
		declare @notificationId bigint = null
		declare @userId bigint = null
		declare @idpId bigint = null
		declare @invitationId bigint = null
		declare @emailBatchId bigint = null
		declare @externalEmailBatchMessage as varchar(64) = null

		if @Send_As_Email = 1
		begin
			insert into core.EmailBatch ([Type_ID], External_Message_ID)
			values (2, '')

			set @emailBatchId = SCOPE_IDENTITY()
			set @externalEmailBatchMessage = 'EmailBatch.' + cast(@emailBatchId as varchar(64))

			update core.EmailBatch
			set External_Message_ID = @externalEmailBatchMessage
			where Email_Batch_ID = @emailBatchId
		end

		-- This is actually a transient user account. It's only used in order to let people
		-- control group membership in the time between invitation issuance and acceptance.
		insert into [security].[User] ([Name], [Email_Address], [Enabled], [Type_ID])
		values ('(pending invitation)', @Email_Address, 0, 2)

		set @userId = SCOPE_IDENTITY()
		-- Fold this user into the Tenant Members security group
		insert into [security].[SecurityGroupMembership] (Security_Group_ID, [User_ID], [Enabled])
			select wkg.Security_Group_ID, @userId, 1
			from [security].WellKnownGroup as wkg
			where wkg.Securable_Resource_ID = @Tenant_ID
				and wkg.Well_Known_Group_Type_ID = 6

		select @idpId = Provider_ID from [security].[ExternalIdentityProvider] where Claim_Value = @Identity_Provider

		insert into [tenants].[Invitation] (Tenant_ID, [User_ID], Email_Batch_ID,
			Expiration_Date, Email_Address, Identity_Provider_ID, [Status_ID])
		values (@Tenant_ID, @userId, @emailBatchId,
			@Expiration_Date, @Email_Address, @idpId, 1)

		set @invitationId = SCOPE_IDENTITY()

		update [security].[User]
		set [Name] = '(pending invitation #' + cast(@invitationId as nvarchar(128)) + ')'
		where [User_ID] = @userId

		select inv.Invitation_ID as Id, inv.Invitation_Date as InvitationDate, inv.Expiration_Date as ExpirationDate,
			inv.Email_Address as EmailAddress, eip.Claim_Value as ExternalIdentityProvider, inv.Status_ID as [Status],
			ten.Tenant_ID as Id, ten.[Name],
			eb.Email_Batch_ID as Id, eb.External_Message_ID as ServiceBusMessageId
		from [tenants].[Invitation] as inv
		inner join [security].[ExternalIdentityProvider] as eip
			on (eip.Provider_ID = inv.Identity_Provider_ID)
		inner join [tenants].[Tenant] as ten
			on (ten.Tenant_ID = inv.Tenant_ID)
		left outer join [core].[EmailBatch] as eb
			on (eb.Email_Batch_ID = inv.Email_Batch_ID)
		where inv.Invitation_ID = @invitationId

	commit transaction
END
