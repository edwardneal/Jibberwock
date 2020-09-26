if 1 not in (select [Securable_Resource_Type_ID] from [security].[SecurableResourceType])
	insert into [security].[SecurableResourceType] ([Securable_Resource_Type_ID], [Name], [Prefix])
	values (1, 'Tenant', 'ORG')

if 2 not in (select [Securable_Resource_Type_ID] from [security].[SecurableResourceType])
	insert into [security].[SecurableResourceType] ([Securable_Resource_Type_ID], [Name], [Prefix])
	values (2, 'API Key', 'KEY')

if 3 not in (select [Securable_Resource_Type_ID] from [security].[SecurableResourceType])
	insert into [security].[SecurableResourceType] ([Securable_Resource_Type_ID], [Name], [Prefix])
	values (3, 'Product', 'PROD')

if 4 not in (select [Securable_Resource_Type_ID] from [security].[SecurableResourceType])
	insert into [security].[SecurableResourceType] ([Securable_Resource_Type_ID], [Name], [Prefix])
	values (4, 'Service', 'SVC')

if 5 not in (select [Securable_Resource_Type_ID] from [security].[SecurableResourceType])
	insert into [security].[SecurableResourceType] ([Securable_Resource_Type_ID], [Name], [Prefix])
	values (5, 'Allert: Alert Definition', 'ALDEF')

if 6 not in (select [Securable_Resource_Type_ID] from [security].[SecurableResourceType])
	insert into [security].[SecurableResourceType] ([Securable_Resource_Type_ID], [Name], [Prefix])
	values (6, 'Allert: Alert Definition Group', 'ALDG')