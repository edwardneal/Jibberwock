CREATE TABLE [tenants].[SubscriptionStatus]
(
	[Subscription_Status_ID] INT NOT NULL, 
    [Name] NVARCHAR(64) NOT NULL, 
    CONSTRAINT [PK_SubscriptionStatus] PRIMARY KEY ([Subscription_Status_ID]) 
)
