CREATE TABLE [security].[ExternalIdentity]
(
	[External_Identity_ID] BIGINT NOT NULL IDENTITY, 
    [External_Identifier] NVARCHAR(MAX) NOT NULL, 
    [Provider_ID] INT NOT NULL, 
    [User_ID] BIGINT NOT NULL, 
    CONSTRAINT [PK_ExternalIdentity] PRIMARY KEY ([External_Identity_ID]), 
    CONSTRAINT [FK_ExternalIdentity_User] FOREIGN KEY ([User_ID]) REFERENCES [security].[User]([User_ID]), 
    CONSTRAINT [FK_ExternalIdentity_ExternalIdentityProvider] FOREIGN KEY ([Provider_ID]) REFERENCES [security].[ExternalIdentityProvider]([Provider_ID]) 
)
