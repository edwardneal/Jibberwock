CREATE PROCEDURE [products].[usp_DeleteCharacteristic]
	@Characteristic_ID int
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @errorCode int = 0

	begin transaction

		if exists (select 1 from [products].[TierCharacteristicValue] where Characteristic_ID = @Characteristic_ID)
			set @errorCode = 1
		else if exists (select 1 from [products].[ApplicableProductCharacteristic] where Characteristic_ID = @Characteristic_ID)
			set @errorCode = 2
		else
		begin
			delete from [products].[Characteristic]
			where Characteristic_ID = @Characteristic_ID;

			if @@ROWCOUNT = 0
				set @errorCode = 3
		end

		select @errorCode as Result

	commit transaction
END

