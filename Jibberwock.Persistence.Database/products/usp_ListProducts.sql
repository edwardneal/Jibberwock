CREATE PROCEDURE [products].[usp_ListProducts]
	@Include_Hidden bit,
	@User_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @availableSecurableResources as table (Securable_Resource_ID bigint)

	insert into @availableSecurableResources (Securable_Resource_ID)
		select distinct Securable_Resource_ID
		from [security].[tvf_ListEffectivePermissions](@User_ID, 1) as ep
		where ep.Permission_ID = 1

	select p.Product_ID as Id, p.[Name], p.[Description],
		p.More_Information_URL as MoreInformationUrl,
		p.Visible, sr.Identifier as ResourceIdentifier,
		sr.[Type_ID] as ResourceType
	from [products].[Product] as p
	inner join [security].[SecurableResource] as sr
		on (sr.Securable_Resource_ID = p.Product_ID)
	where sr.Securable_Resource_ID in (select Securable_Resource_ID from @availableSecurableResources)
		and ((@Include_Hidden = 0 and Visible = 1) or (@Include_Hidden = 1))

	select p.Product_ID as ProductId, c.Characteristic_ID as Id, c.[Name], c.[Description], c.Visible, c.[Enabled], c.Value_Type_ID as ValueType
	from [products].[Product] as p
	inner join @availableSecurableResources as asr
		on (asr.Securable_Resource_ID = p.Product_ID)
	inner join [products].[ApplicableProductCharacteristic] as apc
		on (apc.Product_ID = p.Product_ID)
	inner join [products].[Characteristic] as c
		on (c.Characteristic_ID = apc.Characteristic_ID)
	where ((@Include_Hidden = 0 and p.Visible = 1) or (@Include_Hidden = 1))
end
