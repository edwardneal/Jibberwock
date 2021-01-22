CREATE PROCEDURE [security].[usp_CreateAccessControlEntry]
	@Tenant_ID bigint,
	@User_ID bigint,
	@Security_Group_ID bigint,
	@Securable_Resource_ID bigint,
	@Permission_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @matchingSecurityGroupId bigint
		declare @matchingSecurableResourceId bigint
		declare @securableResourceTypeId int
		declare @securableResourceTenant bigint
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

		select top 1 @matchingSecurableResourceId = ep.[Securable_Resource_ID],
			@securableResourceTypeId = sr.[Type_ID]
		from [security].[tvf_ListEffectivePermissions](@User_ID, 1) as ep
		inner join [security].[SecurableResource] as sr
			on (sr.[Securable_Resource_ID] = ep.[Securable_Resource_ID])
		where ep.[Permission_ID] = 1
			and ep.[Securable_Resource_ID] = @Securable_Resource_ID

		-- Throw an exception if the user does not have at least Read permission over
		-- the resource being secured. This protects against malicious attempts to
		-- grant permissions over ad-hoc resource IDs
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_resource', 2

		select top 1 @matchingAccessControlEntryId = [Access_Control_Entry_ID]
		from [security].[AccessControlEntry]
		where [Security_Group_ID] = @Security_Group_ID
			and [Securable_Resource_ID] = @Securable_Resource_ID
			and [Permission_ID] = @Permission_ID

		-- Make sure that it's not possible to add the same permission for the same
		-- group to the same securable object twice
		if @@ROWCOUNT > 0
			throw 50001, 'duplicate_access_control_entry', 3

		-- Figure out the securable resource's tenant ID based upon its type
		-- 1: Tenant
		if @securableResourceTypeId = 1
			set @securableResourceTenant = @Securable_Resource_ID
		-- 2: todo: API key
		else if @securableResourceTypeId = 2
			set @securableResourceTenant = null
		-- 5: todo: Alert definition
		else if @securableResourceTypeId = 5
			set @securableResourceTenant = null
		-- 6: todo: Alert definition group
		else if @securableResourceTypeId = 6
			set @securableResourceTenant = null

		insert into [security].[AccessControlEntry] ([Security_Group_ID], [Securable_Resource_ID], [Permission_ID], [Parent_Tenant_ID])
			select @Security_Group_ID, @Securable_Resource_ID, @Permission_ID, @securableResourceTenant

		select ace.[Access_Control_Entry_ID] as Id, ace.[Permission_ID] as Permission,
			sg.Security_Group_ID as Id, sg.[Name],
			sr.[Securable_Resource_ID] as Id, sr.Identifier as ResourceIdentifier, sr.[Type_ID] as ResourceType,
			ace.[Parent_Tenant_ID] as Id
		from [security].[AccessControlEntry] as ace
		inner join [security].[SecurityGroup] as sg
			on (sg.[Security_Group_ID] = ace.[Security_Group_ID])
		inner join [security].[SecurableResource] as sr
			on (sr.[Securable_Resource_ID] = ace.[Securable_Resource_ID])
		where ace.[Access_Control_Entry_ID] = SCOPE_IDENTITY()

	commit transaction
END