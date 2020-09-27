CREATE TABLE [core].[Service]
(
	[Service_ID] BIGINT NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Url] NVARCHAR(256) NOT NULL, 
    CONSTRAINT [PK_Service] PRIMARY KEY ([Service_ID]), 
	CONSTRAINT [FK_Service_SecurableResource] FOREIGN KEY ([Service_ID]) REFERENCES [security].[SecurableResource]([Securable_Resource_ID]) 
)
