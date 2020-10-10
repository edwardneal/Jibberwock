CREATE PROCEDURE [core].[usp_GetEmailBatchByIdentifier]
	@External_Message_Identifier varchar(64)
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select eb.Email_Batch_ID as Id, eb.External_Message_ID as ServiceBusMessageId,
		eb.First_Processing_Date as DateFirstProcessed, eb.Last_Processing_Date as DateLastProcessed,
		eb.Processed_Successfully as ProcessedSuccessfully,
		ebt.Email_Batch_Type_ID as Id, ebt.[Name],
		ebt.[Sender_Address] as SenderAddress, ebt.Sender_Alias as SenderContact,
		ebt.External_Unsubscription_Group_ID as UnsubscriptionGroupId, ebt.External_Message_Template_ID as MessageTemplateId
	from core.EmailBatch as eb
	inner join core.EmailBatchType as ebt
		on (ebt.Email_Batch_Type_ID = eb.[Type_ID])
	left outer join core.[Notification] as n
		on (n.Email_Batch_ID = eb.Email_Batch_ID)
	where eb.External_Message_ID = @External_Message_Identifier
		and (
			n.Notification_ID is null
			-- Filter to only allow Active notifications
			or (n.Notification_ID is not null and n.Status_ID = 1))
END