CREATE TABLE [security].[AuditTrailType]
(
	[Audit_Trail_Type_ID] INT NOT NULL, 
    [Name] NVARCHAR(32) NOT NULL, 
    CONSTRAINT [PK_AuditTrailType] PRIMARY KEY ([Audit_Trail_Type_ID]) 
)
