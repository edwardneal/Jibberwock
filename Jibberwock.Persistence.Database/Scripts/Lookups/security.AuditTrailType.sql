if 1 not in (select [Audit_Trail_Type_ID] from [security].[AuditTrailType])
	insert into [security].[AuditTrailType] ([Audit_Trail_Type_ID], [Name])
	values (1, 'control_user_access')