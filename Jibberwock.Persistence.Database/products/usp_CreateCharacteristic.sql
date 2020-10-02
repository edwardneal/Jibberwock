CREATE PROCEDURE [products].[usp_CreateCharacteristic]
	@Name nvarchar(128),
	@Description nvarchar(256),
	@Visible bit,
	@Enabled bit,
	@Value_Type_ID int
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
	
		insert into [products].[Characteristic] ([Name], [Description], Visible, [Enabled], Value_Type_ID)
		values (@Name, @Description, @Visible, @Enabled, @Value_Type_ID)

		if @@ROWCOUNT = 0
			throw 50001, 'record_not_inserted', 1;
		else
			select Characteristic_ID as Id, [Name], [Description], Visible, [Enabled], Value_Type_ID as ValueType
			from [products].[Characteristic]
			where Characteristic_ID = SCOPE_IDENTITY()

	commit transaction
END
