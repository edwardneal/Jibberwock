CREATE PROCEDURE [security].[usp_DeleteAccessControlEntry]
	@Tenant_ID bigint,
	@Security_Group_ID bigint,
	@Access_Control_Entry_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @matchingSecurityGroupId bigint
		declare @matchingAccessControlEntryId bigint

		select top 1 @matchingSecurityGroupId = [Security_Group_ID]
		from [security].[SecurityGroup]
		where [Security_Group_ID] = @Security_Group_ID
			and [Limiting_Tenant_ID] = @Tenant_ID
			and [Security_Group_ID] not in (select [Security_Group_ID] from [security].[WellKnownGroup] where [Securable_Resource_ID] = @Tenant_ID)

		-- Throw an exception if the security group membership ID is valid but the
		-- tenant ID is not, or the security group type is not null. This protects
		-- against spoofed API requests and API requests which try to change the permissions
		-- of core groups
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_id', 1

		select top 1 @matchingAccessControlEntryId = [Access_Control_Entry_ID]
		from [security].[AccessControlEntry]
		where [Security_Group_ID] = @Security_Group_ID
			and [Access_Control_Entry_ID] = @Access_Control_Entry_ID

		-- Throw an exception if the access control entry does not exist, or if the
		-- access control is for a different security group than the specified group.
		-- This protects against API requests which try to change the permissions of
		-- other groups
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_group', 2

		delete from [security].[AccessControlEntry]
		where [Access_Control_Entry_ID] = @Access_Control_Entry_ID

		if @@ROWCOUNT = 0
			select cast(0 as bit)
		else
			select cast(1 as bit)

	commit transaction
END
