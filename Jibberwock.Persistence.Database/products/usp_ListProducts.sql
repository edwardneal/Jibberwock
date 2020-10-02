CREATE PROCEDURE [products].[usp_ListProducts]
	@Include_Hidden bit,
	@User_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	with availableSecurableResources as (
		select distinct Securable_Resource_ID
		from [security].[tvf_ListEffectivePermissions](@User_ID, 1) as ep
		where ep.Permission_ID = 1
	)
	select p.Product_ID as Id, p.[Name], p.[Description],
		p.More_Information_URL as MoreInformationUrl,
		p.Visible, sr.Identifier as ResourceIdentifier,
		sr.[Type_ID] as ResourceType
	from [products].[Product] as p
	inner join [security].[SecurableResource] as sr
		on (sr.Securable_Resource_ID = p.Product_ID)
	where sr.Securable_Resource_ID in (select Securable_Resource_ID from availableSecurableResources)
		and ((@Include_Hidden = 0 and Visible = 1) or (@Include_Hidden = 1))
end
