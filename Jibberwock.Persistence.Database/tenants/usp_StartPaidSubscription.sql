CREATE PROCEDURE [tenants].[usp_StartPaidSubscription]
	@Subscription_ID bigint,
	@External_Identifier nvarchar(64)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		update [tenants].[Subscription]
		set External_Identifier = @External_Identifier,
			Status_ID = 3
		where Subscription_ID = @Subscription_ID

	commit transaction
END
