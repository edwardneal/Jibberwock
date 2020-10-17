CREATE ROLE [BackgroundProcessing]
GO

GRANT EXECUTE ON [components].[usp_GetByStatusProvider] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [components].[usp_UpdateStatus] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [security].[usp_CreateAuditTrailEntry] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [core].[usp_GetEmailBatchByIdentifier] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [core].[usp_StartEmailBatch] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON TYPE::[core].[udt_Email] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [core].[usp_PrepareEmails] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [core].[usp_CompleteEmails] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [core].[usp_CompleteEmailBatch] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [core].[usp_GetNotificationEmailBatchByID] TO [BackgroundProcessing]
GO
