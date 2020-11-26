CREATE PROCEDURE [products].[usp_UpdateProduct]
	@Product_ID bigint,
	@Name nvarchar(128),
	@Description nvarchar(256),
	@More_Information_URL nvarchar(256),
	@Visible bit,
	@Configuration_Control_Name varchar(64),
	@Configuration_String nvarchar(max),
	@Characteristics [products].[udt_ProductCharacteristic] readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
		-- Handle the initial work to update the top-level fields: name, description,
		-- more information URL, visible. Then, perform some inserts/deletes to handle
		-- the list of product characteristics.
		-- NB: this could easily mean that the child product tiers could have characteristic
		-- values which aren't present in the product's list of applicable characteristics.
		-- I'm fine with that!

		update pc
		set Configuration_String = @Configuration_String
		from [products].[ProductConfiguration] as pc
		inner join [products].[Product] as p
			on (p.Default_Configuration_ID = pc.Product_Configuration_ID)
		where p.Product_ID = @Product_ID

		update [products].[Product]
		set [Name] = @Name,
			[Description] = @Description,
			More_Information_URL = @More_Information_URL,
			Visible = @Visible,
			Configuration_Control_Name = @Configuration_Control_Name
		where Product_ID = @Product_ID

		-- No need to perform complex work if the product doesn't exist!
		if @@ROWCOUNT > 0
		begin
			-- Handle the deletes and inserts for applicable product characteristics
			delete from [products].[ApplicableProductCharacteristic]
			where Product_ID = @Product_ID
				and Characteristic_ID not in (select Characteristic_ID from @Characteristics)

			insert into [products].[ApplicableProductCharacteristic] (Characteristic_ID, Product_ID)
				select Characteristic_ID, @Product_ID
				from @Characteristics
				where Characteristic_ID not in
					(select Characteristic_ID from [products].[ApplicableProductCharacteristic] where Product_ID = @Product_ID)
					and Characteristic_ID in (select Characteristic_ID from [products].[Characteristic])
		end

		select p.Product_ID as Id, p.[Name], p.[Description],
			p.More_Information_URL as MoreInformationUrl,
			p.Visible, p.Configuration_Control_Name as ConfigurationControlName, sr.Identifier as ResourceIdentifier,
			sr.[Type_ID] as ResourceType
		from [products].[Product] as p
		inner join [security].[SecurableResource] as sr
			on (sr.Securable_Resource_ID = p.Product_ID)
		where p.Product_ID = @Product_ID

		select p.Default_Configuration_ID as Id, @Configuration_String as ConfigurationString
		from [products].[Product] as p
		where p.Product_ID = @Product_ID

		select c.Characteristic_ID as Id, c.[Name], c.Visible, c.[Enabled], c.Value_Type_ID as ValueType
		from [products].[ApplicableProductCharacteristic] as apc
		inner join [products].[Characteristic] as c
			on (c.Characteristic_ID = apc.Characteristic_ID)
		where Product_ID = @Product_ID
	commit transaction
END