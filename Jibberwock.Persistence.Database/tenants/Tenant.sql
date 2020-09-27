CREATE TABLE [tenants].[Tenant]
(
	[Tenant_ID] BIGINT NOT NULL, 
    [Name] NVARCHAR(256) NOT NULL, 
    [Billing_Contact_ID] BIGINT NOT NULL, 
    CONSTRAINT [PK_Tenant] PRIMARY KEY ([Tenant_ID]), 
    CONSTRAINT [FK_Tenant_Contact_BillingContact] FOREIGN KEY ([Billing_Contact_ID]) REFERENCES [tenants].[Contact]([Contact_ID]), 
    CONSTRAINT [FK_Tenant_SecurableResource] FOREIGN KEY ([Tenant_ID]) REFERENCES [security].[SecurableResource]([Securable_Resource_ID]) 
)
