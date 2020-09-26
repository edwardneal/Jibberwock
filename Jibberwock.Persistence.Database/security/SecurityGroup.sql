CREATE TABLE [security].[SecurityGroup]
(
	[Security_Group_ID] BIGINT NOT NULL IDENTITY, 
    [Name] NVARCHAR(256) NOT NULL, 
    [Limiting_Tenant_ID] BIGINT NULL, 
    CONSTRAINT [PK_SecurityGroup] PRIMARY KEY ([Security_Group_ID]), 
    CONSTRAINT [FK_SecurityGroup_Tenant] FOREIGN KEY ([Limiting_Tenant_ID]) REFERENCES [tenants].[Tenant]([Tenant_ID]) 
)
