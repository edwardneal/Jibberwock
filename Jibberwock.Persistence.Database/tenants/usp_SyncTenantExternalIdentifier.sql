CREATE PROCEDURE [tenants].[usp_SyncTenantExternalIdentifier]
	@Tenant_ID bigint,
	@External_Identifier nvarchar(64)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	update [tenants].[Tenant]
	set External_Identifier = @External_Identifier
	where Tenant_ID = @Tenant_ID
END
