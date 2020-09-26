declare @services as table
	(
		Service_ID int,
		[Name] nvarchar(128),
		[Url] nvarchar(256),
		Securable_Resource_Identifier varchar(16)
	)

insert into @services (Service_ID, [Name], [Url], Securable_Resource_Identifier)
values (1, 'Jibberwock Admin Portal', 'https://admin.jibberwock.com', 'SVC-000001'),
	(2, 'Allert', 'https://allert.jibberwock.com', 'SVC-000002')

insert into [security].[SecurableResource] ([Type_ID], [Identifier])
	select 4, Securable_Resource_Identifier
	from @services
	where Securable_Resource_Identifier not in
		(select [Identifier] from [security].[SecurableResource])

insert into core.[Service] (Service_ID, Securable_Resource_ID, [Name], [Url])
	select svc.Service_ID, sr.Securable_Resource_ID, svc.[Name], svc.[Url]
	from @services as svc
	inner join [security].[SecurableResource] as sr
		on (sr.Identifier = svc.Securable_Resource_Identifier)
	where svc.Service_ID not in
		(select Service_ID from core.[Service])