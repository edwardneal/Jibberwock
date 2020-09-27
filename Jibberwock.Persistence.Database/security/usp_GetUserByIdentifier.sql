CREATE PROCEDURE [security].[usp_GetUserByIdentifier]
	@External_Identifier nvarchar(max),
	@Identity_Provider nvarchar(32),
	@External_Name nvarchar(128),
	@Email_Address nvarchar(256)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	-- Get the user by their external identifier, making sure that their cached name and email addresses are correct.
	update u
	set u.Name = @External_Name,
		u.Email_Address = @Email_Address
	from [security].[User] as u
	inner join [security].[ExternalIdentity] as eI
		on (eI.User_ID = u.User_ID)
	inner join [security].ExternalIdentityProvider as eIP
		on (eIP.Provider_ID = eI.Provider_ID)
	where eI.External_Identifier = @External_Identifier
		and eIP.Claim_Value = @Identity_Provider

	select u.User_ID as Id, u.Name, u.Email_Address as EmailAddress, u.[Enabled]
	from [security].[User] as u
	inner join [security].[ExternalIdentity] as eI
		on (eI.User_ID = u.User_ID)
	inner join [security].ExternalIdentityProvider as eIP
		on (eIP.Provider_ID = eI.Provider_ID)
	where eI.External_Identifier = @External_Identifier
		and eIP.Claim_Value = @Identity_Provider
END
