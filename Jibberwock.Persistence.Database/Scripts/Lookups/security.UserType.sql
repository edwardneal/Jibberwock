if 1 not in (select [User_Type_ID] from [security].[UserType])
	insert into [security].[UserType] ([User_Type_ID], [Name])
	values (1, 'user')

if 2 not in (select [User_Type_ID] from [security].[UserType])
	insert into [security].[UserType] ([User_Type_ID], [Name])
	values (2, 'invitation')

if 3 not in (select [User_Type_ID] from [security].[UserType])
	insert into [security].[UserType] ([User_Type_ID], [Name])
	values (3, 'api_key')