CREATE TABLE [security].[ExternalIdentityProvider]
(
	[Provider_ID] INT NOT NULL IDENTITY, 
    [Claim_Value] NVARCHAR(32) NOT NULL, 
    [Description] NVARCHAR(64) NOT NULL, 
    CONSTRAINT [PK_ExternalIdentityProvider] PRIMARY KEY ([Provider_ID]) 
)
