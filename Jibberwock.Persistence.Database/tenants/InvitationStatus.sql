CREATE TABLE [tenants].[InvitationStatus]
(
	[Invitation_Status_ID] INT NOT NULL , 
    [Name] NVARCHAR(64) NOT NULL, 
    CONSTRAINT [PK_InvitationStatus] PRIMARY KEY ([Invitation_Status_ID])
)
