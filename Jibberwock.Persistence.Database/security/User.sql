CREATE TABLE [security].[User]
(
	[User_ID] BIGINT NOT NULL  IDENTITY, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Email_Address] NVARCHAR(256) NULL, 
    CONSTRAINT [PK_User] PRIMARY KEY ([User_ID])
)
