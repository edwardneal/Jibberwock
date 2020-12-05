CREATE PROCEDURE [tenants].[usp_SyncTenantFromBillingProvider]
	@Tenant_ID bigint,
	@Tenant_Name nvarchar(256),
	@Tenant_External_Identifier nvarchar(64),
	@Contact_Telephone_Number nvarchar(128),
	@Contact_Email_Address nvarchar(256)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		-- First of all: if Tenant_ID is non-null, update the tenant's External_Identifier to form the initial link
		if @Tenant_ID is not null
			update [tenants].[Tenant]
			set External_Identifier = @Tenant_External_Identifier
			where Tenant_ID = @Tenant_ID

		-- Next, update the top-level properties (currently just the tenant name) on the tenant itself
		update [tenants].[Tenant]
		set [Name] = @Tenant_Name
		where External_Identifier = @Tenant_External_Identifier

		if @@ROWCOUNT = 0
			throw 50001, 'no_identifier_found', 1

		-- Implicit assumption: there'll always be a billing contact, even if it's never used. The only way a tenant
		-- can be created is through usp_CreateTenant, and that always creates a record. Billing_Contact_ID is also non-nullable
		update c
		set c.Telephone_Number = @Contact_Telephone_Number,
			c.Email_Address = @Contact_Email_Address
		from [tenants].[Contact] as c
		inner join [tenants].[Tenant] as t
			on (t.Billing_Contact_ID = c.Contact_ID)
		where t.External_Identifier = @Tenant_External_Identifier

		if @@ROWCOUNT = 0
			throw 50001, 'no_contact_record', 2

	commit transaction
END