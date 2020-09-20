CREATE ROLE [BackgroundProcessing]
GO

GRANT EXECUTE ON [components].[usp_GetByStatusProvider] TO [BackgroundProcessing]
GO

GRANT EXECUTE ON [components].[usp_UpdateStatus] TO [BackgroundProcessing]
GO
