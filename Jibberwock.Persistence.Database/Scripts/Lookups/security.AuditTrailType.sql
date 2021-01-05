if 1 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (1, 'control_user_access')

if 2 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (2, 'modify_product')

if 3 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (3, 'delete_product')

if 4 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (4, 'modify_product_characteristic')

if 5 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (5, 'delete_product_characteristic')

if 6 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (6, 'modify_tier')

if 7 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (7, 'modify_notification')

if 8 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (8, 'dismiss_notification')

if 9 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (9, 'modify_tenant')

if 10 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (10, 'invite_user')

if 11 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (11, 'subscription')

if 12 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (12, 'sync_subscription')

if 13 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (13, 'modify_group')