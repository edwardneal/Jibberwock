if 1 not in (select [Email_Batch_Type_ID] from [core].[EmailBatchType])
	insert into [core].[EmailBatchType] ([Email_Batch_Type_ID], [Name], [Sender_Address], [Sender_Alias], [External_Unsubscription_Group_ID], [External_Message_Template_ID])
	values (1, 'notification', 'notifications@dev.jibberwock.com', '[Development] Jibberwock Notification', 14814, 'd-b893b60a25254ee1a95589b831017634')

if 2 not in (select [Email_Batch_Type_ID] from [core].[EmailBatchType])
	insert into [core].[EmailBatchType] ([Email_Batch_Type_ID], [Name], [Sender_Address], [Sender_Alias], [External_Unsubscription_Group_ID], [External_Message_Template_ID])
	values (2, 'invitations', 'invitations@dev.jibberwock.com', '[Development] Jibberwock Tenant Invitation', 14813, 'd-e594105eb9ff4a5ba9d6ed5bf38f6a91')
