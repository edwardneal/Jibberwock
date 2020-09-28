CREATE PROCEDURE [security].[usp_CreateAuditTrailEntry]
	@Audit_Trail_Type_ID int,
	@Occurrence_Time datetimeoffset(7),
	@User_ID bigint,
	@Tenant_ID bigint,
	@Performed_By_User_ID bigint,
	@Originating_Connection_ID nvarchar(128),
	@Originating_Service_ID bigint,
	@Comment nvarchar(max),
	@Metadata nvarchar(max),
	@Result_Value_Type int = 0
AS
BEGIN
	set nocount on;
	set xact_abort on;

	insert into [security].[AuditTrail] (Audit_Trail_Type_ID, Occurrence_Time, [User_ID], Tenant_ID,
		Performed_By_User_ID, Originating_Connection_ID, Originating_Service_ID,
		Comment, Metadata)
	values (@Audit_Trail_Type_ID, @Occurrence_Time, @User_ID, @Tenant_ID,
		@Performed_By_User_ID, @Originating_Connection_ID, @Originating_Service_ID,
		@Comment, @Metadata)

	-- Result_Value_Type determines the shape of the result set
	-- 0 = nothing, 1 = ID only, 2 = full record details
	if @Result_Value_Type = 1
		select SCOPE_IDENTITY() as Id
	else if @Result_Value_Type = 2
		select Audit_Trail_ID as Id, Occurrence_Time as Occurrence,
			Originating_Connection_ID as OriginatingConnectionId, Audit_Trail_Type_ID as [Type],
			Comment, Metadata,
			-- RelatedUser
			[User_ID] as Id,
			-- RelatedTenant
			Tenant_ID as Id,
			-- PerformedBy
			Performed_By_User_ID as Id,
			-- OriginatingService
			Originating_Service_ID as Id
		from [security].[AuditTrail]
		where Audit_Trail_ID = SCOPE_IDENTITY()
END
