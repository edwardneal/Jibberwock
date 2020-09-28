CREATE ROLE [CoreAPI]
GO

GRANT EXECUTE ON [components].[usp_GetByPurpose] TO [CoreAPI]
GO

GRANT EXECUTE ON [components].[usp_ListAll] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_GetUserByIdentifier] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_CheckUserPermissions] TO [CoreAPI]
GO

GRANT EXECUTE ON TYPE::[security].[udt_SecurableResourcePermissionCheck] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_GetUsersByName] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_GetUserById] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_ControlUserAccess] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_CreateAuditTrailEntry] TO [CoreAPI]
GO
