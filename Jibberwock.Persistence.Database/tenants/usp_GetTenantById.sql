CREATE PROCEDURE [tenants].[usp_GetTenantById]
	@Tenant_ID bigint
AS
BEGIN
	select ten.Tenant_ID as Id, ten.[Name], ten.External_Identifier as ExternalId,
		sr.Identifier as ResourceIdentifier, sr.[Type_ID] as ResourceType,
		cont.Contact_ID as Id, cont.[Full_Name] as FullName, cont.Email_Address as EmailAddress, cont.Telephone_Number as TelephoneNumber
	from [tenants].[Tenant] as ten
	inner join [security].[SecurableResource] as sr
		on (sr.Securable_Resource_ID = ten.Tenant_ID)
	inner join [tenants].[Contact] as cont
		on (cont.Contact_ID = ten.Billing_Contact_ID)
	where ten.Tenant_ID = @Tenant_ID
END
