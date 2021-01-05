CREATE PROCEDURE [security].[usp_CreateSecurityGroupMembership]
	@Tenant_ID bigint,
	@Security_Group_ID bigint,
	@Member_User_ID bigint,
	@Enabled bit
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @matchingSecurityGroupId bigint

		select top 1 @matchingSecurityGroupId = [Security_Group_ID] from [security].[SecurityGroup] where [Security_Group_ID] = @Security_Group_ID and [Limiting_Tenant_ID] = @Tenant_ID

		-- Throw an exception if the security group ID is valid but the
		-- tenant ID is not. This protects against spoofed API requests
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_id', 1

		insert into [security].[SecurityGroupMembership] (Security_Group_ID, [User_ID], [Enabled])
		values (@Security_Group_ID, @Member_User_ID, @Enabled)

		select sgm.Security_Group_Membership_ID as Id, sgm.[Enabled],
			sgm.[Security_Group_ID] as Id,
			sgm.[User_ID] as Id
		from [security].[SecurityGroupMembership] as sgm
		where [Security_Group_Membership_ID] = SCOPE_IDENTITY()

	commit transaction
END