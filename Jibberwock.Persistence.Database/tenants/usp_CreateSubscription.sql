CREATE PROCEDURE [tenants].[usp_CreateSubscription]
	@Tenant_ID bigint,
	@Tier_ID bigint,
	@Product_Configuration nvarchar(max),
	@Start_Date datetimeoffset(7),
	@End_Date datetimeoffset(7)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		declare @productConfigurationId bigint = null
		declare @subscriptionStatus int = 1
		declare @subscriptionId bigint = null

		-- Convert @Product_Configuration into an appropriate ID
		insert into [products].[ProductConfiguration] (Configuration_String)
		values (@Product_Configuration)

		set @productConfigurationId = SCOPE_IDENTITY()
		-- If we're creating a subscription to a free tier, move the subscription straight to Active
		if exists (select 1 from [products].[Tier] as t where t.Tier_ID = @Tier_ID and t.External_Identifier is null)
			set @subscriptionStatus = 3

		insert into [tenants].[Subscription] (Tenant_ID, Tier_ID, Product_Configuration_ID, Status_ID,
			[Start_Date], End_Date, [Enabled])
		values (@Tenant_ID, @Tier_ID, @productConfigurationId, @subscriptionStatus,
			@Start_Date, @End_Date, 1)

		set @subscriptionId = SCOPE_IDENTITY()

		select sub.Subscription_ID as Id, sub.External_Identifier as ExternalId,
			sub.[Enabled], sub.Last_Invoice_External_Identifier as LastInvoiceExternalId,
			sub.[Start_Date] as StartDate, sub.End_Date as EndDate, sub.Status_ID as [Status],
			ten.Tenant_ID as Id, ten.[Name],
			tier.Tier_ID as Id, tier.External_Identifier as ExternalId, tier.[Name],
			tier.[Visible], tier.[Start_Date] as StartDate, tier.End_Date as EndDate
		from [tenants].[Subscription] as sub
		inner join [tenants].[Tenant] as ten
			on (ten.Tenant_ID = sub.Tenant_ID)
		inner join [products].[Tier] as tier
			on (tier.Tier_ID = sub.Tier_ID)
		where sub.Subscription_ID = @subscriptionId

	commit transaction
END
