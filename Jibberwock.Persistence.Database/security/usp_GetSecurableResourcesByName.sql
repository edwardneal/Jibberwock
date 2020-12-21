CREATE PROCEDURE [security].[usp_GetSecurableResourcesByName]
	@Name_Filter nvarchar(256),
	@User_ID bigint,
	@Tenant_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select x.ResourceId, x.ResourceIdentifier, x.ResourceType, x.ResourceName
	from
	(
		select distinct sr.Securable_Resource_ID as ResourceId, sr.Identifier as ResourceIdentifier, sr.[Type_ID] as ResourceType,
			case sr.[Type_ID]
				when 1 then ten.[Name]
				when 2 then N'(todo: API KEY)'
				when 3 then prd.[Name]
				when 4 then svc.[Name]
				when 5 then N'(todo: ALERT DEFINITION)'
				when 6 then N'(todo: ALERT DEFINITION GROUP)'
				else '(unknown type)'
			end as ResourceName
		from [security].[tvf_ListEffectivePermissions](@User_ID, 1) as perms
		inner join [security].[SecurableResource] as sr
			on (sr.[Securable_Resource_ID] = perms.[Securable_Resource_ID])
		left outer join [tenants].[Tenant] as ten
			on (ten.Tenant_ID = sr.Securable_Resource_ID and sr.[Type_ID] = 1)
		left outer join [products].[Product] as prd
			on (prd.Product_ID = sr.Securable_Resource_ID and sr.[Type_ID] = 3)
		left outer join [core].[Service] as svc
			on (svc.Service_ID = sr.Securable_Resource_ID and sr.[Type_ID] = 4)
		-- NB: deliberately not putting a specific Privilege_ID check here. "Read" is the most basic permission there is - if somebody
		-- can delete an object, they can absolutely read it!
		where (@Tenant_ID is not null and
			(
				(ten.Tenant_ID is not null and ten.Tenant_ID = @Tenant_ID)
				-- todo: as we add types of securable resources, add their criteria here (separated by ORs)
			))
			or (@Tenant_ID is null)
	) as x
	where x.ResourceName like @Name_Filter
END
