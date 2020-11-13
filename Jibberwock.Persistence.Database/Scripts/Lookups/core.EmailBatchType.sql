if 1 not in (select [Email_Batch_Type_ID] from [core].[EmailBatchType])
	insert into [core].[EmailBatchType] ([Email_Batch_Type_ID], [Name], [Sender_Address], [Sender_Alias], [External_Unsubscription_Group_ID], [External_Message_Template_ID])
	values (1, 'notification', 'notifications@dev.jibberwock.com', '[Development] Jibberwock Notification', 14814, 'd-b893b60a25254ee1a95589b831017634')
