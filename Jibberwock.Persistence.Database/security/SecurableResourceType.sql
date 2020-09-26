CREATE TABLE [security].[SecurableResourceType]
(
	[Securable_Resource_Type_ID] INT NOT NULL, 
    [Name] VARCHAR(32) NOT NULL, 
    [Prefix] VARCHAR(5) NOT NULL, 
    CONSTRAINT [PK_SecurableResourceType] PRIMARY KEY ([Securable_Resource_Type_ID]) 
)
