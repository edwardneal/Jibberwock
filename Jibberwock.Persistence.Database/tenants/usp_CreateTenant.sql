CREATE PROCEDURE [tenants].[usp_CreateTenant]
	@Name nvarchar(256),
	@Contact_Name nvarchar(256),
	@Contact_Telephone_Number nvarchar(128),
	@Contact_Email_Address nvarchar(256),
	@Current_User_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction
		declare @tenantId bigint
		declare @contactId bigint

		-- Set up the securable resource and tenant...
		exec [security].[usp_CreateSecurableResource] @Securable_Resource_Type_ID = 1, @Created_Securable_Resource_ID = @tenantId output

		insert into [tenants].[Contact] (Full_Name, Telephone_Number, Email_Address)
		values (@Contact_Name, @Contact_Telephone_Number, @Contact_Email_Address)

		set @contactId = SCOPE_IDENTITY()

		insert into [tenants].[Tenant] (Tenant_ID, Billing_Contact_ID, [Name])
		values (@tenantId, @contactId, @Name)

		-- ...create the groups and tie them to the tenant's WellKnownGroups...
		declare @tenantAuditorsGroup bigint
		declare @billingAdministratorsGroup bigint
		declare @tenantMembersGroup bigint
		declare @tenantAdministratorsGroup bigint
		declare @apiKeyAdministratorsGroup bigint

		exec [security].[usp_CreateSecurityGroup] @Name = 'Auditors', @Tenant_ID = @tenantId, @Security_Group_ID = @tenantAuditorsGroup output
		exec [security].[usp_CreateSecurityGroup] @Name = 'Billing Admins', @Tenant_ID = @tenantId, @Security_Group_ID = @billingAdministratorsGroup output
		exec [security].[usp_CreateSecurityGroup] @Name = 'Tenant Members', @Tenant_ID = @tenantId, @Security_Group_ID = @tenantMembersGroup output
		exec [security].[usp_CreateSecurityGroup] @Name = 'Tenant Admins', @Tenant_ID = @tenantId, @Security_Group_ID = @tenantAdministratorsGroup output
		exec [security].[usp_CreateSecurityGroup] @Name = 'API Key Admins', @Tenant_ID = @tenantId, @Security_Group_ID = @apiKeyAdministratorsGroup output

		insert into [security].[WellKnownGroup] (Securable_Resource_ID, Security_Group_ID, Well_Known_Group_Type_ID)
		values (@tenantId, @tenantAuditorsGroup, 3),
			(@tenantId, @billingAdministratorsGroup, 5),
			(@tenantId, @tenantMembersGroup, 6),
			(@tenantId, @tenantAdministratorsGroup, 7),
			(@tenantId, @apiKeyAdministratorsGroup, 8)

		-- ...set up each group's ACLs...

		insert into [security].[AccessControlEntry] (Securable_Resource_ID, Security_Group_ID, Parent_Tenant_ID, Permission_ID)
		-- Auditors: Read Logs
		values (@tenantId, @tenantAuditorsGroup, @tenantId, 7),
		-- Billing Admins: Change Billing Contact, Change Subscription Billing
			(@tenantId, @billingAdministratorsGroup, @tenantId, 3),
			(@tenantId, @billingAdministratorsGroup, @tenantId, 4),
		-- Tenant Members: Read
			(@tenantId, @tenantMembersGroup, @tenantId, 1),
		-- Tenant Admins: Change, Read Logs
			(@tenantId, @tenantAdministratorsGroup, @tenantId, 2),
			(@tenantId, @tenantAdministratorsGroup, @tenantId, 7),
		-- API Key Admins: Create API Key
			(@tenantId, @apiKeyAdministratorsGroup, @tenantId, 8)

		-- ...and add the current user to all of these groups
		insert into [security].[SecurityGroupMembership] (Security_Group_ID, [User_ID], [Enabled])
		values (@tenantAuditorsGroup, @Current_User_ID, 1),
			(@billingAdministratorsGroup, @Current_User_ID, 1),
			(@tenantMembersGroup, @Current_User_ID, 1),
			(@tenantAdministratorsGroup, @Current_User_ID, 1),
			(@apiKeyAdministratorsGroup, @Current_User_ID, 1)

		select @tenantId as Id

	commit transaction
END
