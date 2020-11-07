CREATE PROCEDURE [products].[usp_UpdateCharacteristic]
	@Characteristic_ID int,
	@Name nvarchar(128),
	@Description nvarchar(256),
	@Visible bit,
	@Enabled bit
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
	
		update [products].[Characteristic]
		set [Name] = @Name,
			[Description] = @Description,
			Visible = @Visible,
			[Enabled] = @Enabled
		where Characteristic_ID = @Characteristic_ID;

		if @@ROWCOUNT = 0
			throw 50001, 'id_not_valid', 1;
		else
			select Characteristic_ID as Id, [Name], [Description], Visible, [Enabled], Value_Type_ID as ValueType
			from [products].[Characteristic]
			where Characteristic_ID = @Characteristic_ID

	commit transaction
END
