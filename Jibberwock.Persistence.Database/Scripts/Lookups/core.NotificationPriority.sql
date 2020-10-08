if 1 not in (select [Notification_Priority_ID] from [core].[NotificationPriority])
	insert into [core].[NotificationPriority] ([Notification_Priority_ID], [Name], [Order])
	values (1, 'low', 1)

if 2 not in (select [Notification_Priority_ID] from [core].[NotificationPriority])
	insert into [core].[NotificationPriority] ([Notification_Priority_ID], [Name], [Order])
	values (2, 'normal', 2)

if 3 not in (select [Notification_Priority_ID] from [core].[NotificationPriority])
	insert into [core].[NotificationPriority] ([Notification_Priority_ID], [Name], [Order])
	values (3, 'high', 3)