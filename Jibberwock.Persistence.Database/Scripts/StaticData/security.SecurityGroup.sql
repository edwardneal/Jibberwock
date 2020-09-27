declare @securityGroups as table
	(
		Service_ID int,
		Group_Name nvarchar(256),
		Well_Known_Group_Type int,
		Permission_ID int
	)

insert into @securityGroups (Service_ID, Group_Name, Well_Known_Group_Type, Permission_ID)
values (1, 'Jibberwock Service Administrators', 1, 1),
	(1, 'Jibberwock Service Administrators', 1, 2),
	(1, 'Jibberwock Service Administrators', 1, 7),
	(1, 'Jibberwock Service Administrators', 1, 9),
	(1, 'Jibberwock Product Administrators', 2, 9),
	(1, 'Jibberwock Log Access', 3, 7),
	(1, 'Jibberwock Service Readers', 4, 1),
	(1, 'Jibberwock Service Readers', 4, 7),
	(1, 'Jibberwock Billing Administrators', 5, 5)

insert into [security].[SecurityGroup] ([Name])
	select distinct Group_Name
	from @securityGroups
	where Group_Name not in (select [Name] from [security].[SecurityGroup])

insert into [security].[AccessControlEntry] (Security_Group_ID, Securable_Resource_ID, Permission_ID)
	select grp.Security_Group_ID, sg.Service_ID, sg.Permission_ID
	from @securityGroups as sg
	inner join [security].[SecurityGroup] as grp
		on (grp.[Name] = sg.Group_Name)
	where not exists (
		select 1
		from [security].[AccessControlEntry] as ace
		where ace.Security_Group_ID = grp.Security_Group_ID
			and ace.Securable_Resource_ID = sg.Service_ID
			and ace.Permission_ID = sg.Permission_ID
	)

insert into [security].[WellKnownGroup] (Securable_Resource_ID, Security_Group_ID, Well_Known_Group_Type_ID)
	select distinct sg.Service_ID, grp.Security_Group_ID, sg.Well_Known_Group_Type
	from @securityGroups as sg
	inner join [security].[SecurityGroup] as grp
		on (grp.[Name] = sg.Group_Name)
	where not exists (
		select 1
		from [security].[WellKnownGroup] as wkg
		where wkg.Securable_Resource_ID = sg.Service_ID
			and wkg.Security_Group_ID = grp.Security_Group_ID
			and wkg.Well_Known_Group_Type_ID = sg.Well_Known_Group_Type
	)