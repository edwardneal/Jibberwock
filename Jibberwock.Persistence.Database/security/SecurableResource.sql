CREATE TABLE [security].[SecurableResource]
(
	[Securable_Resource_ID] BIGINT NOT NULL IDENTITY, 
    [Type_ID] INT NOT NULL, 
    [Identifier] VARCHAR(16) NOT NULL, 
    CONSTRAINT [PK_SecurableResource] PRIMARY KEY ([Securable_Resource_ID]), 
    CONSTRAINT [FK_SecurableResource_SecurableResourceType] FOREIGN KEY ([Type_ID]) REFERENCES [security].[SecurableResourceType]([Securable_Resource_Type_ID]) 
)
