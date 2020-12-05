CREATE PROCEDURE [tenants].[usp_SyncSubscriptionsFromBillingProvider]
	@Subscription_External_Identifier nvarchar(64),
	@Status_ID int,
	@Last_Invoice_External_Identifier nvarchar(64),
	@Subscription_IDs [tenants].[udt_Subscription] readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		-- Create any first cross-links from the Subscription table to the billing provider subscription
		update [tenants].[Subscription]
		set External_Identifier = @Subscription_External_Identifier
		from [tenants].[Subscription]
		where Subscription_ID in (select Subscription_ID from @Subscription_IDs)

		-- Now, update any statuses and the latest invoice ID as applicable
		update [tenants].[Subscription]
		set Status_ID = @Status_ID,
			Last_Invoice_External_Identifier = @Last_Invoice_External_Identifier
		where External_Identifier = @Subscription_External_Identifier

	commit transaction
END