CREATE PROCEDURE [security].[usp_GetAuditTrail]
	@Start_Time datetimeoffset(7),
	@End_Time datetimeoffset(7),
	@User_ID bigint,
	@Tenant_ID bigint,
	@Performed_By_User_ID bigint,
	@Type_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select aud.Audit_Trail_ID as Id, aud.Occurrence_Time as Occurrence, aud.Audit_Trail_Type_ID as [Type],
		aud.Originating_Connection_ID as OriginatingConnectionId, aud.Comment, aud.Metadata,
		rU.[User_ID] as Id, rU.[Name],
		rT.[Tenant_ID] as Id, rT.[Name],
		pbU.[User_ID] as Id, pbU.[Name],
		svc.Service_ID as Id, svc.[Name]
	from [security].[AuditTrail] as aud
	left outer join [security].[User] as rU
		on (rU.[User_ID] = aud.[User_ID])
	left outer join [tenants].[Tenant] as rT
		on (rT.Tenant_ID = aud.Tenant_ID)
	left outer join [security].[User] as pbU
		on (pbU.[User_ID] = aud.Performed_By_User_ID)
	left outer join [core].[Service] as svc
		on (svc.Service_ID = aud.Originating_Service_ID)
	where ((@Start_Time is not null and aud.Occurrence_Time >= @Start_Time) or (@Start_Time is null))
		and ((@End_Time is not null and aud.Occurrence_Time <= @End_Time) or (@End_Time is null))
		and ((@User_ID is not null and aud.[User_ID] = @User_ID) or (@User_ID is null))
		and ((@Tenant_ID is not null and aud.Tenant_ID = @Tenant_ID) or (@Tenant_ID is null))
		and ((@Performed_By_User_ID is not null and aud.Performed_By_User_ID = @Performed_By_User_ID) or (@Performed_By_User_ID is null))
		and ((@Type_ID is not null and aud.Audit_Trail_Type_ID = @Type_ID) or (@Type_ID is null))
END
