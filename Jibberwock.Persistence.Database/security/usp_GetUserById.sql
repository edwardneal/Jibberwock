CREATE PROCEDURE [security].[usp_GetUserById]
	@User_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select u.[User_ID] as Id, u.Name, u.[Enabled]
	from [security].[User] as u
	where u.[User_ID] = @User_ID

	select eI.External_Identity_ID as Id, eI.External_Identifier as ExternalIdentifier, eIP.[Description] as [Provider]
	from [security].[ExternalIdentity] as eI
	inner join [security].[ExternalIdentityProvider] as eIP
		on (eIP.Provider_ID = eI.Provider_ID)
	where eI.[User_ID] = @User_ID
END
