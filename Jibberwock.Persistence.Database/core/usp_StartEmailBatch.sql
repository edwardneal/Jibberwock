CREATE PROCEDURE [core].[usp_StartEmailBatch]
	@Email_Batch_ID bigint,
	@Date_Started datetimeoffset
AS
BEGIN
	set nocount on;
	set xact_abort on;

	update core.EmailBatch
	set First_Processing_Date = isnull(First_Processing_Date, @Date_Started),
		Last_Processing_Date = @Date_Started
	where Email_Batch_ID = @Email_Batch_ID

	if @@ROWCOUNT = 0
		select cast(0 as bit) as Success
	else
		select cast(1 as bit) as Success
END
