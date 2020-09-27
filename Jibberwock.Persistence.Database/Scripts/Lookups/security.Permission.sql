if 1 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (1, 'read')

if 2 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (2, 'change')

if 3 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (3, 'change_billing_contact')

if 4 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (4, 'change_subscription_billing')

if 5 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (5, 'delete')

if 6 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (6, 'invite')

if 7 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (7, 'read_logs')

if 8 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (8, 'create_api_key')

if 9 not in (select [Permission_ID] from [security].[Permission])
	insert into [security].[Permission] ([Permission_ID], [Name])
	values (9, 'create_product')
