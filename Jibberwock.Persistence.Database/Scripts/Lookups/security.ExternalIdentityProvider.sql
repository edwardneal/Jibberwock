if 'google.com' not in (select [Claim_Value] from [security].[ExternalIdentityProvider])
	insert into [security].[ExternalIdentityProvider] ([Claim_Value], [Description])
	values ('google.com', 'Google')

if 'live.com' not in (select [Claim_Value] from [security].[ExternalIdentityProvider])
	insert into [security].[ExternalIdentityProvider] ([Claim_Value], [Description])
	values ('live.com', 'Microsoft Account')

if 'github.com' not in (select [Claim_Value] from [security].[ExternalIdentityProvider])
	insert into [security].[ExternalIdentityProvider] ([Claim_Value], [Description])
	values ('github.com', 'GitHub')