CREATE ROLE [BackgroundProcessing]
GO

GRANT EXECUTE ON [components].[usp_GetByPurpose] TO [CoreAPI]
GO
