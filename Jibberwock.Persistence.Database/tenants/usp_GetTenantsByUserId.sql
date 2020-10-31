CREATE PROCEDURE [tenants].[usp_GetTenantsByUserId]
	@User_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	-- This looks for all associated tenants of all "Tenant Members" groups which the user is a member of.
	select ten.Tenant_ID as Id, ten.[Name],
		grp.Security_Group_ID as Id, grp.[Name],
		sgm.Security_Group_Membership_ID as Id, sgm.[Enabled]
	from tenants.Tenant as ten
	inner join [security].WellKnownGroup as tenWkg
		on (tenWkg.Securable_Resource_ID = ten.Tenant_ID)
	inner join [security].SecurityGroup as grp
		on (grp.Security_Group_ID = tenWkg.Security_Group_ID)
	inner join [security].SecurityGroupMembership as sgm
		on (sgm.Security_Group_ID = grp.Security_Group_ID)
	where tenWkg.Well_Known_Group_Type_ID = 6
		and sgm.[User_ID] = @User_ID
END
