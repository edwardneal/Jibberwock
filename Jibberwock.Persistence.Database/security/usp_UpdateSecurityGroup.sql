CREATE PROCEDURE [security].[usp_UpdateSecurityGroup]
	@Security_Group_ID bigint,
	@Name nvarchar(256)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		update [security].[SecurityGroup]
		set [Name] = @Name
		where Security_Group_ID = @Security_Group_ID

		if @@ROWCOUNT = 0
			throw 50001, 'invalid_id', 1

		select distinct sg.Security_Group_ID as Id, sg.[Name],
			wkg.Well_Known_Group_Type_ID as WellKnownGroupType
		from [security].[SecurityGroup] as sg
		left outer join [security].[WellKnownGroup] as wkg
			on (sg.Security_Group_ID = wkg.Security_Group_ID)
		where sg.Security_Group_ID = @Security_Group_ID

	commit transaction
END