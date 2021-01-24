CREATE TABLE [security].[User]
(
	[User_ID] BIGINT NOT NULL  IDENTITY, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Email_Address] NVARCHAR(256) NULL, 
    [Enabled] BIT NOT NULL , 
    [Type_ID] INT NOT NULL, 
    CONSTRAINT [PK_User] PRIMARY KEY ([User_ID]), 
    CONSTRAINT [FK_User_UserType] FOREIGN KEY ([Type_ID]) REFERENCES [security].[UserType]([User_Type_ID])
)
