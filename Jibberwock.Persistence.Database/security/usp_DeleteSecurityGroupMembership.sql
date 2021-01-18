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

		delete [security].[SecurityGroupMembership] where Security_Group_Membership_ID = @Security_Group_Membership_ID

		if @@ROWCOUNT = 0
			select cast(0 as bit)
		else
			select cast(1 as bit)

	commit transaction
END
