CREATE TABLE [core].[Notification]
(
	[Notification_ID] BIGINT NOT NULL IDENTITY, 
    [User_ID] BIGINT NULL, 
    [Tenant_ID] BIGINT NULL, 
    [Email_Batch_ID] BIGINT NULL, 
    [Status_ID] INT NOT NULL, 
    [Type_ID] INT NOT NULL, 
    [Priority_ID] INT NOT NULL, 
    [Creation_Date] DATETIMEOFFSET NOT NULL, 
    [Start_Date] DATETIMEOFFSET NULL, 
    [End_Date] DATETIMEOFFSET NULL, 
    [Subject] NVARCHAR(128) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [Allow_Dismissal] BIT NOT NULL, 
    CONSTRAINT [PK_Notification] PRIMARY KEY ([Notification_ID]), 
    CONSTRAINT [FK_Notification_User] FOREIGN KEY ([User_ID]) REFERENCES [security].[User]([User_ID]), 
    CONSTRAINT [FK_Notification_Tenant] FOREIGN KEY ([Tenant_ID]) REFERENCES [tenants].[Tenant]([Tenant_ID]), 
    CONSTRAINT [FK_Notification_EmailBatch] FOREIGN KEY ([Email_Batch_ID]) REFERENCES [core].[EmailBatch]([Email_Batch_ID]), 
    CONSTRAINT [FK_Notification_NotificationType] FOREIGN KEY ([Type_ID]) REFERENCES [core].[NotificationType]([Notification_Type_ID]), 
    CONSTRAINT [FK_Notification_NotificationStatus] FOREIGN KEY ([Status_ID]) REFERENCES [core].[NotificationStatus]([Notification_Status_ID]), 
    CONSTRAINT [FK_Notification_NotificationPriority] FOREIGN KEY ([Priority_ID]) REFERENCES [core].[NotificationPriority]([Notification_Priority_ID]) 
)
