CREATE PROCEDURE [core].[usp_ListBatches]
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select eb.Email_Batch_ID as Id, eb.External_Message_ID as ServiceBusMessageId,
		eb.First_Processing_Date as DateFirstProcessed, eb.Last_Processing_Date as DateLastProcessed, eb.Processed_Successfully as ProcessedSuccessfully,
		ebt.Email_Batch_Type_ID as Id, ebt.[Name]
	from core.EmailBatch as eb
	inner join core.EmailBatchType as ebt
		on (ebt.Email_Batch_Type_ID = eb.[Type_ID])
END
