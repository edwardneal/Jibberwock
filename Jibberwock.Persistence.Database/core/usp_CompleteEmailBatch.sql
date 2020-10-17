CREATE PROCEDURE [core].[usp_CompleteEmailBatch]
	@Email_Batch_ID bigint,
	@Processed_Successfully bit
AS
BEGIN
	set nocount on;
	set xact_abort on;

	update core.EmailBatch
	set Processed_Successfully = @Processed_Successfully
	where Email_Batch_ID = @Email_Batch_ID

	if @@ROWCOUNT > 0
		select cast(1 as bit) as Success
	else
		select cast(0 as bit) as Success
END