CREATE PROCEDURE [products].[usp_CreateProductTier]
	@Product_ID bigint,
	@Name nvarchar(128),
	@External_Identifier nvarchar(max),
	@Visible bit,
	@Start_Date datetimeoffset(7),
	@End_Date datetimeoffset(7),
	@Characteristic_Values [products].[udt_TierCharacteristicValue] readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @tierId bigint

		insert into [products].[Tier] (Product_ID, [Name], External_Identifier, Visible, [Start_Date], End_Date)
		values (@Product_ID, @Name, @External_Identifier, @Visible, @Start_Date, @End_Date)

		set @tierId = SCOPE_IDENTITY()

		insert into [products].[TierCharacteristicValue] (Tier_ID, Characteristic_ID, Numeric_Value, String_Value, Boolean_Value)
			select @tierId, cv.Characteristic_ID,
				(case c.Value_Type_ID when 3 then cast(cv.Characteristic_Value as bigint) else null end),
				(case c.Value_Type_ID when 1 then cast(cv.Characteristic_Value as nvarchar(max)) else null end),
				(case c.Value_Type_ID when 2 then cast(cv.Characteristic_Value as bit) else null end)
			from @Characteristic_Values as cv
			inner join [products].[Characteristic] as c
				on (c.Characteristic_ID = cv.Characteristic_ID)

		select @tierId as Id

	commit transaction
END
