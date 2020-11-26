CREATE PROCEDURE [products].[usp_GetProductById]
	@Product_ID bigint,
	@User_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
	
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
		where p.Product_ID = @Product_ID
			and p.Product_ID in (select Securable_Resource_ID from @availableSecurableResources)

		select pc.Product_Configuration_ID as Id, pc.Configuration_String as ConfigurationString
		from [products].[ProductConfiguration] as pc
		inner join [products].[Product] as p
			on (p.Default_Configuration_ID = pc.Product_Configuration_ID)
		where p.Product_ID = @Product_ID
			and p.Product_ID in (select Securable_Resource_ID from @availableSecurableResources)

		select c.Characteristic_ID as Id, c.[Name], c.Visible, c.[Enabled], c.Value_Type_ID as ValueType
		from [products].[ApplicableProductCharacteristic] as apc
		inner join [products].[Characteristic] as c
			on (c.Characteristic_ID = apc.Characteristic_ID)
		where Product_ID = @Product_ID
			and apc.Product_ID in (select Securable_Resource_ID from @availableSecurableResources)

	commit transaction
END
