CREATE PROCEDURE [security].[usp_ControlUserAccess]
	@User_ID bigint,
	@Enabled bit
AS
BEGIN
	set nocount on;
	set xact_abort on;

	update [security].[User]
	set [Enabled] = @Enabled
	where [User_ID] = @User_ID

	-- @@ROWCOUNT can either be 0 or 1. If it's one, we're all good. If it's zero, the User_ID doesn't exist
	if @@ROWCOUNT = 1
		select cast(0 as int) as [Status_Code]
	else
		select cast(1 as int) as [Status_Code]
END
