CREATE PROCEDURE [tenants].[usp_RevokeInvitation]
	@Tenant_ID bigint,
	@Invitation_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @matchingInvitationId bigint

		select top 1 @matchingInvitationId = inv.[Invitation_ID]
		from [tenants].[Invitation] as inv
		where inv.Tenant_ID = @Tenant_ID
			and inv.Invitation_ID = @Invitation_ID

		-- Throw an exception if the invitation ID is valid but the
		-- tenant ID is not. This protects against spoofed API requests
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_id', 1

		update [tenants].[Invitation]
		set Status_ID = 2
		where Invitation_ID = @Invitation_ID

		if @@ROWCOUNT = 0
			select cast(0 as bit)
		else
			select cast(1 as bit)

	commit transaction
END