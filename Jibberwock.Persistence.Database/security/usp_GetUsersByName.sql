CREATE PROCEDURE [security].[usp_GetUsersByName]
	@Name_Filter nvarchar(128)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select u.[User_ID] as Id, u.Name, u.[Enabled]
	from [security].[User] as u
	where u.[Name] like @Name_Filter
END
