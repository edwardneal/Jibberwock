CREATE TABLE [security].[AuditTrail]
(
	[Audit_Trail_ID] BIGINT NOT NULL IDENTITY, 
    [Audit_Trail_Type_ID] INT NOT NULL, 
    [Occurrence_Time] DATETIMEOFFSET NOT NULL, 
    [User_ID] BIGINT NULL, 
    [Tenant_ID] BIGINT NULL, 
    [Performed_By_User_ID] BIGINT NULL, 
    [Originating_Connection_ID] NVARCHAR(128) NULL, 
    [Originating_Service_ID] BIGINT NOT NULL, 
    [Comment] NVARCHAR(MAX) NULL, 
    [Metadata] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_AuditTrail] PRIMARY KEY ([Audit_Trail_ID]), 
    CONSTRAINT [FK_AuditTrail_AuditTrailType] FOREIGN KEY ([Audit_Trail_Type_ID]) REFERENCES [security].[AuditTrailType]([Audit_Trail_Type_ID]), 
    CONSTRAINT [FK_AuditTrail_User] FOREIGN KEY ([User_ID]) REFERENCES [security].[User]([User_ID]), 
    CONSTRAINT [FK_AuditTrail_Tenant] FOREIGN KEY ([Tenant_ID]) REFERENCES [tenants].[Tenant]([Tenant_ID]), 
    CONSTRAINT [FK_AuditTrail_User_PerformedBy] FOREIGN KEY ([Performed_By_User_ID]) REFERENCES [security].[User]([User_ID]), 
    CONSTRAINT [FK_AuditTrail_Service] FOREIGN KEY ([Originating_Service_ID]) REFERENCES [core].[Service]([Service_ID]) 
)
