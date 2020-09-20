if 0 not in (select [Status_Provider_ID] from [components].[StatusProvider])
	insert into [components].[StatusProvider] ([Status_Provider_ID], [Name])
	values (0, 'Not Applicable')

if 1 not in (select [Status_Provider_ID] from [components].[StatusProvider])
	insert into [components].[StatusProvider] ([Status_Provider_ID], [Name])
	values (1, 'Statuspage.io')