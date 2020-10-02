if 1 not in (select [Characteristic_Value_Type_ID] from [products].[CharacteristicValueType])
	insert into [products].[CharacteristicValueType] ([Characteristic_Value_Type_ID], [Name])
	values (1, 'string')

if 2 not in (select [Characteristic_Value_Type_ID] from [products].[CharacteristicValueType])
	insert into [products].[CharacteristicValueType] ([Characteristic_Value_Type_ID], [Name])
	values (2, 'boolean')

if 3 not in (select [Characteristic_Value_Type_ID] from [products].[CharacteristicValueType])
	insert into [products].[CharacteristicValueType] ([Characteristic_Value_Type_ID], [Name])
	values (3, 'numeric')