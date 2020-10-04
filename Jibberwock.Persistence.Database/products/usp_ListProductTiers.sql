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
END
