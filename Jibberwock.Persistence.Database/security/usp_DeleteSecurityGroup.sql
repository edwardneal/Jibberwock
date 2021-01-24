CREATE PROCEDURE [security].[usp_DeleteSecurityGroup]
	@Tenant_ID bigint,
	@Security_Group_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @matchingSecurityGroupId bigint

		select top 1 @matchingSecurityGroupId = [Security_Group_ID]
		from [security].[SecurityGroup] as sg
		where [Security_Group_ID] = @Security_Group_ID
			and [Limiting_Tenant_ID] = @Tenant_ID
			and [Security_Group_ID] not in (select [Security_Group_ID] from [security].[WellKnownGroup] where [Securable_Resource_ID] = @Tenant_ID)

		-- Throw an exception if the security group ID is valid but the
		-- tenant ID is not (or if the security group is a well-known group.)
		-- This protects against spoofed API requests
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_id', 1

		delete [security].[SecurityGroupMembership] where [Security_Group_ID] = @Security_Group_ID

		delete [security].[AccessControlEntry] where [Security_Group_ID] = @Security_Group_ID

		delete [security].[SecurityGroup] where [Security_Group_ID] = @Security_Group_ID

		if @@ROWCOUNT = 0
			select cast(0 as bit)
		else
			select cast(1 as bit)

	commit transaction
END