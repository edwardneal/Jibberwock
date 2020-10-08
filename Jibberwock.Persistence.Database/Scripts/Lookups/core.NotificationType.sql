if 1 not in (select [Notification_Type_ID] from [core].[NotificationType])
	insert into [core].[NotificationType] ([Notification_Type_ID], [Name])
	values (1, 'alert')

if 2 not in (select [Notification_Type_ID] from [core].[NotificationType])
	insert into [core].[NotificationType] ([Notification_Type_ID], [Name])
	values (2, 'information')

if 3 not in (select [Notification_Type_ID] from [core].[NotificationType])
	insert into [core].[NotificationType] ([Notification_Type_ID], [Name])
	values (3, 'error')

if 4 not in (select [Notification_Type_ID] from [core].[NotificationType])
	insert into [core].[NotificationType] ([Notification_Type_ID], [Name])
	values (4, 'warning')