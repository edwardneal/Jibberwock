CREATE PROCEDURE [security].[usp_GetTenantSecurityGroups]
	@Tenant_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select distinct sg.Security_Group_ID as Id, sg.[Name],
		wkg.Well_Known_Group_Type_ID as WellKnownGroupType
	from [security].[SecurityGroup] as sg
	left outer join [security].[WellKnownGroup] as wkg
		on (sg.Security_Group_ID = wkg.Security_Group_ID
			and wkg.Securable_Resource_ID = @Tenant_ID)
	where sg.Limiting_Tenant_ID = @Tenant_ID
END
