declare @externalComponents as table
	(
		External_ID nvarchar(max),
		Purpose nvarchar(128),
		Status_Provider int
	)

insert into @externalComponents (External_ID, Purpose, Status_Provider)
	-- SendGrid, monitored by Statuspage.io
values ('3tgl2vf85cht.dcvwpwy7361c', 'EmailTrackingWebHooks', 1)

insert into [components].[ExternalComponent] (External_ID, Purpose_ID)
	select ec.External_ID, p.Purpose_ID
	from @externalComponents as ec
	inner join [components].[Purpose] as p
		on (p.[Name] = ec.Purpose)
	where ec.External_ID not in (
		select External_ID from [components].[ExternalComponent] where Purpose_ID = p.Purpose_ID
	)

insert into [components].[ExternalComponentStatus] (External_Component_ID, Status_Provider_ID)
	select dbEc.External_Component_ID, ec.Status_Provider
	from [components].[ExternalComponent] as dbEc
	inner join [components].[Purpose] as dbP
		on (dbP.Purpose_ID = dbEc.Purpose_ID)
	inner join @externalComponents as ec
		on (ec.External_ID = dbEc.External_ID and ec.Purpose = dbP.[Name])
	where dbEc.External_Component_ID not in (
		select External_Component_ID from [components].[ExternalComponentStatus]
	)
