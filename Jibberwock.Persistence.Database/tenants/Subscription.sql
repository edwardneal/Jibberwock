CREATE TABLE [tenants].[Subscription]
(
	[Subscription_ID] BIGINT NOT NULL  IDENTITY, 
    [External_Identifier] NVARCHAR(64) NULL, 
    [Tenant_ID] BIGINT NOT NULL, 
    [Tier_ID] BIGINT NOT NULL, 
    [Product_Configuration_ID] BIGINT NOT NULL, 
    [Status_ID] INT NOT NULL, 
    [Start_Date] DATETIMEOFFSET NOT NULL , 
    [End_Date] DATETIMEOFFSET NULL, 
    [Enabled] BIT NOT NULL, 
    [Last_Invoice_External_Identifier] NVARCHAR(64) NULL, 
    CONSTRAINT [PK_Subscription] PRIMARY KEY ([Subscription_ID]), 
    CONSTRAINT [FK_Subscription_Tenant] FOREIGN KEY ([Tenant_ID]) REFERENCES [tenants].[Tenant]([Tenant_ID]), 
    CONSTRAINT [FK_Subscription_Tier] FOREIGN KEY ([Tier_ID]) REFERENCES [products].[Tier]([Tier_ID]), 
    CONSTRAINT [FK_Subscription_ProductConfiguration] FOREIGN KEY ([Product_Configuration_ID]) REFERENCES [products].[ProductConfiguration]([Product_Configuration_ID]), 
    CONSTRAINT [FK_Subscription_SubscriptionStatus] FOREIGN KEY ([Status_ID]) REFERENCES [tenants].[SubscriptionStatus]([Subscription_Status_ID])
)
