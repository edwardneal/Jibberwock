CREATE PROCEDURE [security].[usp_GetWellKnownTenantSecurityGroup]
	@Tenant_ID bigint,
	@Well_Known_Group_Type_ID int
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @groupId bigint

	select top 1 @groupId = wkg.Security_Group_ID
	from [security].WellKnownGroup as wkg
	inner join [tenants].[Tenant] as ten
		on (ten.Tenant_ID = wkg.Securable_Resource_ID)
	where wkg.Securable_Resource_ID = @Tenant_ID
		and wkg.Well_Known_Group_Type_ID = @Well_Known_Group_Type_ID

	exec [security].[usp_GetSecurityGroupById] @Tenant_ID = @Tenant_ID, @Security_Group_ID = @groupId
END
