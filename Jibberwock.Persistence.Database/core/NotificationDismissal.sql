CREATE TABLE [core].[NotificationDismissal]
(
	[Notification_Dismissal_ID] BIGINT NOT NULL IDENTITY, 
    [Notification_ID] BIGINT NOT NULL, 
    [User_ID] BIGINT NOT NULL, 
    CONSTRAINT [PK_NotificationDismissal] PRIMARY KEY ([Notification_Dismissal_ID]), 
    CONSTRAINT [FK_NotificationDismissal_Notification] FOREIGN KEY ([Notification_ID]) REFERENCES [core].[Notification]([Notification_ID]), 
    CONSTRAINT [FK_NotificationDismissal_User] FOREIGN KEY ([User_ID]) REFERENCES [security].[User]([User_ID]) 
)

GO

CREATE UNIQUE INDEX [UQ_NotificationDismissal_NotificationUser] ON [core].[NotificationDismissal] ([Notification_ID], [User_ID])
