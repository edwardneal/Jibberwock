if 1 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (1, 'service_administrators')

if 2 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (2, 'product_administrators')

if 3 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (3, 'service_auditors')

if 4 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (4, 'service_readers')

if 5 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (5, 'billing_administrators')

if 6 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (6, 'tenant_members')

if 7 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (7, 'tenant_administrators')

if 8 not in (select [Well_Known_Group_Type_ID] from [security].[WellKnownGroupType])
	insert into [security].[WellKnownGroupType] ([Well_Known_Group_Type_ID], [Name])
	values (8, 'api_key_administrators')
