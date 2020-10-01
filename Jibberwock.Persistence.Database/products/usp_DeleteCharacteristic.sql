CREATE PROCEDURE [products].[usp_DeleteCharacteristic]
	@Characteristic_ID int
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
	
		delete from [products].[Characteristic]
		where Characteristic_ID = @Characteristic_ID;

		if @@ROWCOUNT = 0
			select cast(0 as bit) as Success
		else
			select cast(1 as bit) as Success

	commit transaction
END

