CREATE PROCEDURE [products].[usp_UpdateProductTier]
	@Product_ID bigint,
	@Tier_ID bigint,
	@User_ID bigint,
	@Name nvarchar(128),
	@Visible bit,
	@Start_Date datetimeoffset(7),
	@End_Date datetimeoffset(7),
	@Characteristic_Values [products].[udt_TierCharacteristicValue] readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		update [products].[Tier]
		set [Name] = @Name,
			Visible = @Visible,
			[Start_Date] = @Start_Date,
			End_Date = @End_Date
		where Tier_ID = @Tier_ID

		if @@ROWCOUNT > 0
		begin
			delete from [products].[TierCharacteristicValue]
			where Characteristic_ID not in (select Characteristic_ID from @Characteristic_Values)
				and Tier_ID = @Tier_ID

			insert into [products].[TierCharacteristicValue] (Tier_ID, Characteristic_ID)
				select @Tier_ID, Characteristic_ID
				from @Characteristic_Values
				where Characteristic_ID not in
					(select Characteristic_ID from [products].[TierCharacteristicValue] where Tier_ID = @Tier_ID)

			update tcv
			set Numeric_Value = (case c.Value_Type_ID when 3 then cast(cv.Characteristic_Value as bigint) else null end),
				String_Value = (case c.Value_Type_ID when 1 then cast(cv.Characteristic_Value as nvarchar(max)) else null end),
				Boolean_Value = (case c.Value_Type_ID when 2 then cast(cv.Characteristic_Value as bit) else null end)
			from [products].[TierCharacteristicValue] as tcv
			inner join @Characteristic_Values as cv
				on (cv.Characteristic_ID = tcv.Characteristic_ID
					and tcv.Tier_ID = @Tier_ID)
			inner join [products].[Characteristic] as c
				on (c.Characteristic_ID = tcv.Characteristic_ID)
		end

		exec [products].[usp_GetProductTierById] @Product_ID, @Tier_ID, @User_ID

	commit transaction
END