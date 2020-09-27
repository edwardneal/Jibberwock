CREATE PROCEDURE [security].[usp_CheckUserPermissions]
	@User_ID bigint,
	@Permission_Checks [security].[udt_SecurableResourcePermissionCheck] readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @missingPermissions as table
		(
			Securable_Resource_ID bigint,
			Permission_ID int
		);

	-- Drill into the user's enabled group memberships, then into their access control entries
	with userAccessControls as (
		select ace.Access_Control_Entry_ID, ace.Securable_Resource_ID, ace.Permission_ID
		from [security].[SecurityGroupMembership] as sgm
		inner join [security].[AccessControlEntry] as ace
			on (ace.Security_Group_ID = sgm.Security_Group_ID)
		inner join [security].[User] as usr
			on (usr.[User_ID] = sgm.[User_ID])
		where usr.[User_ID] = @User_ID
			and usr.[Enabled] = 1
			and sgm.[Enabled] = 1
	)
	-- Now, look for anything in Permission_Checks which doesn't exist in the above table
	insert into @missingPermissions (Securable_Resource_ID, Permission_ID)
		select pc.Securable_Resource_ID, pc.Permission_ID
		from @Permission_Checks as pc
		left outer join userAccessControls as uac
			on (uac.Securable_Resource_ID = pc.Securable_Resource_ID
				and uac.Permission_ID = pc.Permission_ID)
		where uac.Access_Control_Entry_ID is null

	if exists (select top 1 Securable_Resource_ID from @missingPermissions)
		select cast(0 as bit) as Check_Result
	else
		select cast(1 as bit) as Check_Result
END
