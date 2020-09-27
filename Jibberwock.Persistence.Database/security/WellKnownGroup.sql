CREATE TABLE [security].[WellKnownGroup]
(
	[Well_Known_Group_ID] BIGINT NOT NULL IDENTITY, 
    [Securable_Resource_ID] BIGINT NOT NULL, 
    [Security_Group_ID] BIGINT NOT NULL, 
    [Well_Known_Group_Type_ID] INT NOT NULL, 
    CONSTRAINT [PK_WellKnownGroup] PRIMARY KEY ([Well_Known_Group_ID]), 
    CONSTRAINT [FK_WellKnownGroup_SecurableResource] FOREIGN KEY ([Securable_Resource_ID]) REFERENCES [security].[SecurableResource]([Securable_Resource_ID]), 
    CONSTRAINT [FK_WellKnownGroup_SecurityGroup] FOREIGN KEY ([Security_Group_ID]) REFERENCES [security].[SecurityGroup]([Security_Group_ID]), 
    CONSTRAINT [FK_WellKnownGroup_WellKnownGroupType] FOREIGN KEY ([Well_Known_Group_Type_ID]) REFERENCES [security].[WellKnownGroupType]([Well_Known_Group_Type_ID]) 
)
