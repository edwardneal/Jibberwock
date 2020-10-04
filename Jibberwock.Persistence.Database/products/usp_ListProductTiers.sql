CREATE PROCEDURE [products].[usp_ListProductTiers]
	@Product_ID bigint,
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

	select t.Tier_ID as Id, t.[Name], t.External_Identifier as ExternalId,
		t.Visible, t.[Start_Date] as StartDate, t.End_Date as EndDate
	from [products].[Tier] as t
	where t.Product_ID = @Product_ID
		and t.Product_ID in (select Securable_Resource_ID from @availableSecurableResources)
		and ((@Include_Hidden = 0 and t.Visible = 1) or (@Include_Hidden = 1))

	select t.Tier_ID as TierId, c.Characteristic_ID as CharacteristicId, c.[Name],
		c.[Description], c.[Visible], c.[Enabled], c.Value_Type_ID as ValueType,
		tcv.Tier_Characteristic_Value_ID as TierCharacteristicValueId,
		(case c.Value_Type_ID
			when 1 then cast(cast(tcv.String_Value as nvarchar(4000)) as sql_variant)
			when 2 then cast(tcv.Boolean_Value as sql_variant)
			when 3 then cast(tcv.Numeric_Value as sql_variant)
		end) as [Value]
	from [products].[Tier] as t
	inner join [products].[TierCharacteristicValue] as tcv
		on (tcv.Tier_ID = t.Tier_ID)
	inner join [products].[Characteristic] as c
		on (c.Characteristic_ID = tcv.Characteristic_ID)
	where t.Product_ID = @Product_ID
		and t.Product_ID in (select Securable_Resource_ID from @availableSecurableResources)
		and ((@Include_Hidden = 0 and t.Visible = 1) or (@Include_Hidden = 1))
END
