CREATE TABLE [tenants].[Contact]
(
	[Contact_ID] BIGINT NOT NULL IDENTITY, 
    [Full_Name] NVARCHAR(256) NOT NULL, 
    [Telephone_Number] NVARCHAR(128) NULL, 
    [Email_Address] NVARCHAR(256) NULL, 
    CONSTRAINT [PK_Contact] PRIMARY KEY ([Contact_ID]) 
)
