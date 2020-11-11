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

GRANT EXECUTE ON [products].[usp_ListAllCharacteristics] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_UpdateCharacteristic] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_CreateCharacteristic] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_DeleteCharacteristic] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_ListProducts] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_CreateProduct] TO [CoreAPI]
GO

GRANT EXECUTE ON TYPE::[products].[udt_ProductCharacteristic] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_UpdateProduct] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_ListProductTiers] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_GetProductById] TO [CoreAPI]
GO

GRANT EXECUTE ON TYPE::[products].[udt_TierCharacteristicValue] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_CreateProductTier] TO [CoreAPI]
GO

GRANT EXECUTE ON [products].[usp_UpdateProductTier] TO [CoreAPI]
GO

GRANT EXECUTE ON [tenants].[usp_GetTenantsByName] TO [CoreAPI]
GO

GRANT EXECUTE ON [security].[usp_GetAuditTrail] TO [CoreAPI]
GO

GRANT EXECUTE ON [core].[usp_CreateNotification] TO [CoreAPI]
GO

GRANT EXECUTE ON [core].[usp_UpdateNotification] TO [CoreAPI]
GO

GRANT EXECUTE ON [core].[usp_ListNotifications] TO [CoreAPI]
GO

GRANT EXECUTE ON [tenants].[usp_GetTenantsByUserId] TO [CoreAPI]
GO

GRANT EXECUTE ON [core].[usp_ListBatches] TO [CoreAPI]
GO

GRANT EXECUTE ON TYPE::[core].[udt_ToAddressHash] TO [CoreAPI]
GO

GRANT EXECUTE ON [core].[usp_GetEmailHistory] TO [CoreAPI]
GO

GRANT EXECUTE ON [core].[usp_GetPlatformKPIs] TO [CoreAPI]
GO