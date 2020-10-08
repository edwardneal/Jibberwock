if 1 not in (select [Notification_Status_ID] from [core].[NotificationStatus])
	insert into [core].[NotificationStatus] ([Notification_Status_ID], [Name])
	values (1, 'active')

if 2 not in (select [Notification_Status_ID] from [core].[NotificationStatus])
	insert into [core].[NotificationStatus] ([Notification_Status_ID], [Name])
	values (2, 'cancelled')