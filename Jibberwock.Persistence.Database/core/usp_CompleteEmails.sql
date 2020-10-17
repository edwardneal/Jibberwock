CREATE PROCEDURE [core].[usp_CompleteEmails]
	@Email_Batch_ID bigint,
	@Date_Sent datetimeoffset,
	@Sent_Emails core.udt_Email readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	-- If these emails have been sent, we just want to mark them as such.
	-- There's separate logic in core.usp_CompleteEmailBatch to handle the batch itself
	update e
	set e.Send_Date = @Date_Sent
	from core.Email as e
	inner join @Sent_Emails as sE
		on (sE.External_Email_ID = e.External_Email_ID)
	where e.Source_Batch_ID = @Email_Batch_ID
	
	if @@ROWCOUNT > 0
		select cast(1 as bit) as Success
	else
		select cast(0 as bit) as Success
END
