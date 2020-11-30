if 1 not in (select [Subscription_Status_ID] from [tenants].[SubscriptionStatus])
	insert into [tenants].[SubscriptionStatus] ([Subscription_Status_ID], [Name])
	values (1, 'payment_pending')

if 2 not in (select [Subscription_Status_ID] from [tenants].[SubscriptionStatus])
	insert into [tenants].[SubscriptionStatus] ([Subscription_Status_ID], [Name])
	values (2, 'trial')

if 3 not in (select [Subscription_Status_ID] from [tenants].[SubscriptionStatus])
	insert into [tenants].[SubscriptionStatus] ([Subscription_Status_ID], [Name])
	values (3, 'active')

if 4 not in (select [Subscription_Status_ID] from [tenants].[SubscriptionStatus])
	insert into [tenants].[SubscriptionStatus] ([Subscription_Status_ID], [Name])
	values (4, 'expired')

if 5 not in (select [Subscription_Status_ID] from [tenants].[SubscriptionStatus])
	insert into [tenants].[SubscriptionStatus] ([Subscription_Status_ID], [Name])
	values (5, 'billing_details_expired')

if 6 not in (select [Subscription_Status_ID] from [tenants].[SubscriptionStatus])
	insert into [tenants].[SubscriptionStatus] ([Subscription_Status_ID], [Name])
	values (6, 'unpaid')