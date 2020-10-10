CREATE PROCEDURE [core].[usp_CreateNotification]
	@User_ID bigint,
	@Tenant_ID bigint,
	@Status_ID int,
	@Type_ID int,
	@Priority_Name varchar(32),
	@Start_Date datetimeoffset(7),
	@End_Date datetimeoffset(7),
	@Subject nvarchar(128),
	@Message nvarchar(max),
	@Allow_Dismissal bit,
	@Send_As_Email bit
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @priorityId int

	select @priorityId = Notification_Priority_ID from [core].[NotificationPriority] where [Name] = @Priority_Name

	if @priorityId is null
		throw 50001, 'invalid_priority_name', 1;

	begin transaction
		-- If sending as an email, create the email batch first. Wrapped this in a transaction to prevent
		-- race conditions where something might pick up the email batch but not have the notification details.

		declare @emailBatchId bigint = null
		declare @externalEmailBatchMessage as varchar(64) = null

		if @Send_As_Email = 1
		begin
			insert into core.EmailBatch ([Type_ID], External_Message_ID)
			values (1, '')

			set @emailBatchId = SCOPE_IDENTITY()
			set @externalEmailBatchMessage = 'EmailBatch.' + cast(@emailBatchId as varchar(64))

			update core.EmailBatch
			set External_Message_ID = @externalEmailBatchMessage
			where Email_Batch_ID = @emailBatchId
		end

		insert into core.[Notification] ([User_ID], Tenant_ID, Email_Batch_ID,
			Status_ID, [Type_ID], Priority_ID, Creation_Date,
			[Subject], [Message], Allow_Dismissal)
		select @User_ID, @Tenant_ID, @emailBatchId,
			@Status_ID, @Type_ID, @priorityId, SYSDATETIMEOFFSET(),
			@Subject, @Message, @Allow_Dismissal

		select SCOPE_IDENTITY() as Id, @emailBatchId as Id, @externalEmailBatchMessage as ServiceBusMessageId
	commit transaction
END
