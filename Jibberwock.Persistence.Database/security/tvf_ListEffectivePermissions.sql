CREATE FUNCTION [security].[tvf_ListEffectivePermissions]
(
	@User_ID bigint,
	@Require_User_Enabled bit
)
RETURNS @accessControlEntries TABLE
(
	Access_Control_Entry_ID bigint not null,
	Securable_Resource_ID bigint not null,
	Security_Group_Membership_ID bigint not null,
	Permission_ID int not null
)
AS
BEGIN
	insert into @accessControlEntries (Access_Control_Entry_ID, Securable_Resource_ID,
		Security_Group_Membership_ID, Permission_ID)
		select ace.Access_Control_Entry_ID, ace.Securable_Resource_ID,
			sgm.Security_Group_Membership_ID, ace.Permission_ID
		from [security].[AccessControlEntry] as ace
		inner join [security].[SecurityGroup] as sg
			on (sg.Security_Group_ID = ace.Security_Group_ID)
		inner join [security].[SecurityGroupMembership] as sgm
			on (sgm.Security_Group_ID = sg.Security_Group_ID)
		inner join [security].[User] as u
			on (u.[User_ID] = sgm.[User_ID])
		where sgm.[Enabled] = 1
			and sgm.[User_ID] = @User_ID
			and ((@Require_User_Enabled = 1 and u.[Enabled] = 1) or (@Require_User_Enabled = 0))

	RETURN
END
