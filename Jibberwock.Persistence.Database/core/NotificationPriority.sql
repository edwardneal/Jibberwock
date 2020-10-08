CREATE TABLE [core].[NotificationPriority]
(
	[Notification_Priority_ID] INT NOT NULL, 
    [Name] VARCHAR(32) NOT NULL, 
    [Order] INT NOT NULL, 
    CONSTRAINT [PK_NotificationPriority] PRIMARY KEY ([Notification_Priority_ID]) 
)
