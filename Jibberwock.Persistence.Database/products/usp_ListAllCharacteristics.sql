CREATE PROCEDURE [products].[usp_ListAllCharacteristics]
AS
BEGIN
	set nocount on;
	set xact_abort on;
	
	select Characteristic_ID as Id, [Name], [Description],
		Visible, [Enabled], Value_Type_ID as ValueType
	from [products].[Characteristic]
END
