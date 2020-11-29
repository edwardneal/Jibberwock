CREATE TABLE [tenants].[Invitation]
(
	[Invitation_ID] BIGINT NOT NULL IDENTITY, 
    [Tenant_ID] BIGINT NOT NULL, 
    [User_ID] BIGINT NOT NULL, 
    [Email_Batch_ID] BIGINT NULL, 
    [Invitation_Date] DATETIMEOFFSET NOT NULL DEFAULT (sysdatetimeoffset()), 
    [Expiration_Date] DATETIMEOFFSET NULL, 
    [Email_Address] NVARCHAR(256) NOT NULL, 
    [Identity_Provider_ID] INT NOT NULL, 
    [Status_ID] INT NOT NULL, 
    CONSTRAINT [PK_Invitation] PRIMARY KEY ([Invitation_ID]), 
    CONSTRAINT [FK_Invitation_Tenant] FOREIGN KEY ([Tenant_ID]) REFERENCES [tenants].[Tenant]([Tenant_ID]), 
    CONSTRAINT [FK_Invitation_User] FOREIGN KEY ([User_ID]) REFERENCES [security].[User]([User_ID]), 
    CONSTRAINT [FK_Invitation_EmailBatch] FOREIGN KEY ([Email_Batch_ID]) REFERENCES [core].[EmailBatch]([Email_Batch_ID]), 
    CONSTRAINT [FK_Invitation_ExternalIdentityProvider] FOREIGN KEY ([Identity_Provider_ID]) REFERENCES [security].[ExternalIdentityProvider]([Provider_ID]), 
    CONSTRAINT [FK_Invitation_InvitationStatus] FOREIGN KEY ([Status_ID]) REFERENCES [tenants].[InvitationStatus]([Invitation_Status_ID]) 
)
