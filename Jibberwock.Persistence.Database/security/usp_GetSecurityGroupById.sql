CREATE PROCEDURE [security].[usp_GetSecurityGroupById]
	@Tenant_ID bigint,
	@Security_Group_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
		-- Result set 1: basic security group details
		-- Result set 2: security group members
		-- Result set 3: access control entries for this security group

		select distinct sg.Security_Group_ID as Id, sg.[Name],
			wkg.Well_Known_Group_Type_ID as WellKnownGroupType
		from [security].[SecurityGroup] as sg
		left outer join [security].[WellKnownGroup] as wkg
			on (sg.Security_Group_ID = wkg.Security_Group_ID
				and wkg.Securable_Resource_ID = @Tenant_ID)
		where sg.Security_Group_ID = @Security_Group_ID
			and sg.Limiting_Tenant_ID = @Tenant_ID

		-- This means that we throw an exception if the security group ID is valid but the
		-- tenant ID is not. It protects against spoofed API requests
		if @@ROWCOUNT = 0
			throw 50001, 'invalid_id', 1

		select sgm.Security_Group_Membership_ID as Id, sgm.[Enabled],
			u.[User_ID] as UserId, u.[Name] as UserName, u.[Type_ID] as [UserType]
		from [security].[SecurityGroupMembership] as sgm
		inner join [security].[User] as u
			on (u.[User_ID] = sgm.[User_ID])
		where sgm.Security_Group_ID = @Security_Group_ID

		select ace.Access_Control_Entry_ID as Id, ace.Permission_ID as Permission,
			sr.Securable_Resource_ID as ResourceId, sr.Identifier as ResourceIdentifier, sr.[Type_ID] as ResourceType,
			case sr.[Type_ID]
				when 1 then ten.[Name]
				when 2 then N'(todo: API KEY)'
				when 3 then prd.[Name]
				when 4 then svc.[Name]
				when 5 then N'(todo: ALERT DEFINITION)'
				when 6 then N'(todo: ALERT DEFINITION GROUP)'
				else '(unknown type)'
			end as ResourceName
		from [security].[AccessControlEntry] as ace
		inner join [security].[SecurableResource] as sr
			on (sr.Securable_Resource_ID = ace.Securable_Resource_ID)
		left outer join [tenants].[Tenant] as ten
			on (ten.Tenant_ID = sr.Securable_Resource_ID and sr.[Type_ID] = 1)
		left outer join [products].[Product] as prd
			on (prd.Product_ID = sr.Securable_Resource_ID and sr.[Type_ID] = 3)
		left outer join [core].[Service] as svc
			on (svc.Service_ID = sr.Securable_Resource_ID and sr.[Type_ID] = 4)
		where ace.[Security_Group_ID] = @Security_Group_ID

	commit transaction
END
