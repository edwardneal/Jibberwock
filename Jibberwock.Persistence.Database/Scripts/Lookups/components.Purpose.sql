declare @componentPurposes as table ( [Name] nvarchar(max) )

insert into @componentPurposes ([Name])
values ('EmailTrackingWebHooks'),
	('DomainRegistrar'),
	('ContentDistributionNetwork'),
	('Dns'),
	('ContinuousIntegration')

insert into [components].[Purpose] ([Name])
	select [Name]
	from @componentPurposes
	where [Name] not in (select [Name] from [components].[Purpose])
