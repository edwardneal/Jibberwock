CREATE PROCEDURE [products].[usp_CreateProduct]
	@Service_ID int,
	@Name nvarchar(128),
	@Description nvarchar(256),
	@More_Information_URL nvarchar(256),
	@Visible bit,
	@Characteristics [products].[udt_ProductCharacteristic] readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		-- Create the basic securable resource, then layer the Product record atop that.
		-- Then, add the list of applicable product characteristics and set up the security.
		-- We secure this by allowing service administrators, product administrators and
		-- service readers to read the product details.
		declare @productId bigint
		declare @serviceAdministratorsGroup bigint
		declare @productAdministratorsGroup bigint
		declare @serviceReadersGroup bigint

		exec [security].[usp_CreateSecurableResource] @Securable_Resource_Type_ID = 3, @Created_Securable_Resource_ID = @productId output

		insert into [products].[Product] ([Product_ID], [Name], [Description], More_Information_URL, Visible)
		values (@productId, @Name, @Description, @More_Information_URL, @Visible)

		insert into [products].[ApplicableProductCharacteristic] (Product_ID, Characteristic_ID)
			select distinct @productId, Characteristic_ID
			from @Characteristics
			where Characteristic_ID in
				(select Characteristic_ID from [products].[Characteristic])

		select @serviceAdministratorsGroup = [Security_Group_ID] from [security].[WellKnownGroup] where Securable_Resource_ID = @Service_ID and Well_Known_Group_Type_ID = 1
		select @productAdministratorsGroup = [Security_Group_ID] from [security].[WellKnownGroup] where Securable_Resource_ID = @Service_ID and Well_Known_Group_Type_ID = 2
		select @serviceReadersGroup = [Security_Group_ID] from [security].[WellKnownGroup] where Securable_Resource_ID = @Service_ID and Well_Known_Group_Type_ID = 4

		insert into [security].[AccessControlEntry] (Securable_Resource_ID, Security_Group_ID, Permission_ID) values (@productId, @serviceAdministratorsGroup, 1)
		insert into [security].[AccessControlEntry] (Securable_Resource_ID, Security_Group_ID, Permission_ID) values (@productId, @productAdministratorsGroup, 1)
		insert into [security].[AccessControlEntry] (Securable_Resource_ID, Security_Group_ID, Permission_ID) values (@productId, @serviceReadersGroup, 1)

		select p.Product_ID as Id, p.[Name], p.[Description],
			p.More_Information_URL as MoreInformationUrl,
			p.Visible, sr.Identifier as ResourceIdentifier,
			sr.[Type_ID] as ResourceType
		from [products].[Product] as p
		inner join [security].[SecurableResource] as sr
			on (sr.Securable_Resource_ID = p.Product_ID)
		where p.Product_ID = @productId

		select c.Characteristic_ID as Id, c.[Name], c.Visible, c.[Enabled], c.Value_Type_ID as ValueType
		from [products].[ApplicableProductCharacteristic] as apc
		inner join [products].[Characteristic] as c
			on (c.Characteristic_ID = apc.Characteristic_ID)
		where Product_ID = @productId

	commit transaction
END
