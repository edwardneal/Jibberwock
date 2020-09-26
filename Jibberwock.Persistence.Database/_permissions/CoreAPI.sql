CREATE ROLE [CoreAPI]
GO

GRANT EXECUTE ON [components].[usp_GetByPurpose] TO [CoreAPI]
GO

GRANT EXECUTE ON [components].[usp_ListAll] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_GetUserByIdentifier] TO [CoreAPI]
GO
