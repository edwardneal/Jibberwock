CREATE PROCEDURE [security].[usp_DeleteSecurityGroupMembership]
	@Tenant_ID bigint,
	@Security_Group_Membership_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @matchingSecurityGroupId bigint

		select top 1 @matchingSecurityGroupId = sgm.[Security_Group_ID]
		from [security].[SecurityGroupMembership] as sgm
		inner join [security].[SecurityGroup] as sg
			on (sg.Security_Group_ID = sgm.Security_Group_ID)
		where sgm.[Security_Group_Membership_ID] = @Security_Group_Membership_ID
			and sg.[Limiting_Tenant_ID] = @Tenant_ID

		-- Throw an exception if the security group membership ID is valid but the
		-- tenant ID is not. This protects against spoofed API requests
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_id', 1

		-- If the security group is a well-known group for the tenant, there must always be
		-- at least one enabled group membership for a user account associated with the group.
		if @matchingSecurityGroupId in (select [Security_Group_ID] from [security].[WellKnownGroup] where [Securable_Resource_ID] = @Tenant_ID)
		begin
			declare @membershipCount bigint

			select @membershipCount = count_big(1)
			from [security].[SecurityGroupMembership] as sgm
			inner join [security].[User] as usr
				on (usr.[User_ID] = sgm.[User_ID])
			where sgm.[Security_Group_ID] = @matchingSecurityGroupId
				and sgm.[Enabled] = 1
				and usr.[Type_ID] = 1
				and sgm.Security_Group_Membership_ID <> @Security_Group_Membership_ID

			if @membershipCount = 0
				throw 50001, 'last_wellknown_group_membership', 2

		end

		delete [security].[SecurityGroupMembership] where Security_Group_Membership_ID = @Security_Group_Membership_ID

		if @@ROWCOUNT = 0
			select cast(0 as bit)
		else
			select cast(1 as bit)

	commit transaction
END
