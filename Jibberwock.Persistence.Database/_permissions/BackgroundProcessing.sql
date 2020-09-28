CREATE ROLE [BackgroundProcessing]
GO

GRANT EXECUTE ON [components].[usp_GetByStatusProvider] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [components].[usp_UpdateStatus] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [security].[usp_CreateAuditTrailEntry] TO [CoreAPI]
GO
