CREATE TABLE [core].[Service]
(
	[Service_ID] INT NOT NULL, 
	[Securable_Resource_ID] BIGINT NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Url] NVARCHAR(256) NOT NULL, 
    CONSTRAINT [PK_Service] PRIMARY KEY ([Service_ID]), 
	CONSTRAINT [FK_Service_SecurableResource] FOREIGN KEY ([Securable_Resource_ID]) REFERENCES [security].[SecurableResource]([Securable_Resource_ID]) 
)
