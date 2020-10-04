CREATE PROCEDURE [tenants].[usp_GetTenantsByName]
	@Name_Filter nvarchar(256)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select Tenant_ID as Id, [Name]
	from [tenants].[Tenant]
	where [Name] like @Name_Filter
END
