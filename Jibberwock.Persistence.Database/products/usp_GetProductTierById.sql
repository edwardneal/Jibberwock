CREATE PROCEDURE [products].[usp_GetProductTierById]
	@Product_ID bigint,
	@Tier_ID bigint,
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

	select t.Tier_ID as Id, t.[Name], t.External_Identifier as ExternalId,
		t.Visible, t.[Start_Date], t.End_Date
	from [products].[Tier] as t
	where t.Tier_ID = @Tier_ID
		and t.Product_ID = @Product_ID
		and t.Product_ID in (select Securable_Resource_ID from @availableSecurableResources)

	select tcv.Tier_Characteristic_Value_ID as Id,
		(case c.Value_Type_ID
			when 1 then cast(cast(tcv.String_Value as nvarchar(4000)) as sql_variant)
			when 2 then cast(tcv.Boolean_Value as sql_variant)
			when 3 then cast(tcv.Numeric_Value as sql_variant)
		end) as CharacteristicValue,
		c.Characteristic_ID as CharacteristicId, c.[Name], c.[Description],
		c.Visible, c.[Enabled], c.Value_Type_ID as ValueType
	from [products].[TierCharacteristicValue] as tcv
	inner join [products].[Characteristic] as c
		on (c.Characteristic_ID = tcv.Characteristic_ID)
	inner join [products].[Tier] as t
		on (t.Tier_ID = tcv.Tier_ID)
	where t.Tier_ID = @Tier_ID
		and t.Product_ID = @Product_ID
		and t.Product_ID in (select Securable_Resource_ID from @availableSecurableResources)
END
