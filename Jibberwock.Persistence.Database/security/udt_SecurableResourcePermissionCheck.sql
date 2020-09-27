CREATE TYPE [security].[udt_SecurableResourcePermissionCheck] AS TABLE
(
	Securable_Resource_ID BIGINT NOT NULL,
	Securable_Resource_Type_ID INT NOT NULL,
	Permission_ID INT NOT NULL
)
