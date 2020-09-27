CREATE TABLE [security].[AccessControlEntry]
(
	[Access_Control_Entry_ID] BIGINT NOT NULL IDENTITY, 
    [Security_Group_ID] BIGINT NOT NULL, 
    [Securable_Resource_ID] BIGINT NOT NULL, 
    [Permission_ID] INT NOT NULL, 
    [Parent_Tenant_ID] BIGINT NULL, 
    CONSTRAINT [PK_AccessControlEntry] PRIMARY KEY ([Access_Control_Entry_ID]), 
    CONSTRAINT [FK_AccessControlEntry_SecurityGroup] FOREIGN KEY ([Security_Group_ID]) REFERENCES [security].[SecurityGroup]([Security_Group_ID]), 
    CONSTRAINT [FK_AccessControlEntry_SecurableResource] FOREIGN KEY ([Securable_Resource_ID]) REFERENCES [security].[SecurableResource]([Securable_Resource_ID]), 
    CONSTRAINT [FK_AccessControlEntry_Permission] FOREIGN KEY ([Permission_ID]) REFERENCES [security].[Permission]([Permission_ID]), 
    CONSTRAINT [FK_AccessControlEntry_Tenant] FOREIGN KEY ([Parent_Tenant_ID]) REFERENCES [tenants].[Tenant]([Tenant_ID]) 
)
