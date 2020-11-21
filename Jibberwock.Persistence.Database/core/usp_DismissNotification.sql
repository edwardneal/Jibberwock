CREATE PROCEDURE [core].[usp_DismissNotification]
	@Calling_User_ID bigint,
	@Notification_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		if exists (select 1 from core.NotificationDismissal where Notification_ID = @Notification_ID and [User_ID] = @Calling_User_ID)
			throw 50001, 'notification_already_dismissed', 1;
		if exists (select 1 from core.[Notification] where Notification_ID = @Notification_ID and Allow_Dismissal = 0)
			throw 50001, 'notification_cannot_be_dismissed', 2;

		insert into core.NotificationDismissal (Notification_ID, [User_ID])
		values (@Notification_ID, @Calling_User_ID)

		select cast(1 as bit) as [Success]

	commit transaction
END