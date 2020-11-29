CREATE PROCEDURE [security].[usp_CreateSecurityGroup]
	@Name nvarchar(256),
	@Tenant_ID bigint,
	@Security_Group_ID bigint output
AS
BEGIN
	set nocount on;
	set xact_abort on;

	insert into [security].[SecurityGroup] ([Name], Limiting_Tenant_ID)
	values (@Name, @Tenant_ID)

	set @Security_Group_ID = SCOPE_IDENTITY()
END
