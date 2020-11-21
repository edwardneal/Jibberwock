CREATE PROCEDURE [core].[usp_UpdateNotification]
	@Notification_ID bigint,
	@Status_ID int,
	@Type_ID int,
	@Priority_Name varchar(32),
	@Start_Date datetimeoffset(7),
	@End_Date datetimeoffset(7),
	@Subject nvarchar(128),
	@Message nvarchar(max),
	@Allow_Dismissal bit,
	@Send_As_Email bit,
	@New_Email_Message_Required bit output
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @priorityId int
	declare @currentDate datetimeoffset(7)
	declare @shouldRevokeDismissal bit = 0

	select @priorityId = Notification_Priority_ID from [core].[NotificationPriority] where [Name] = @Priority_Name
	select @currentDate = SYSDATETIMEOFFSET()

	if @priorityId is null
		throw 50001, 'invalid_priority_name', 1;

	begin transaction
		-- A few rules here. If the notification already has an associated email batch:
		-- * Start_Date can never be changed
		-- * If Start_Date has also passed:
		-- ** Subject, Message cannot be changed
		-- ** Send_As_Email cannot be unset
		-- Alternatively, if the notification does not have an associated email batch,
		-- Start_Date has passed and Send_As_Email is set to 1, we should create the email batch.
		-- Besides this, if Allow_Dismissal transitions to 0, or if Subject/Message change, we need to revoke any
		-- user dismissals (because the message itself has changed.)
		if exists (select 1
			from core.[Notification]
			where Notification_ID = @Notification_ID
				and Allow_Dismissal = 1
				and @Allow_Dismissal = 0)
			set @shouldRevokeDismissal = 1

		-- This means that we can always update the five fields below:
		update core.[Notification]
		set Status_ID = @Status_ID,
			[Type_ID] = @Type_ID,
			Priority_ID = @priorityId,
			End_Date = @End_Date,
			Allow_Dismissal = @Allow_Dismissal
		where Notification_ID = @Notification_ID

		-- Second-stage update to handle the Start_Date (only updating for non-email notifications)
		update core.[Notification]
		set [Start_Date] = @Start_Date
		where Notification_ID = @Notification_ID
			and Email_Batch_ID is null

		-- Third-stage update to handle Subject and Message (only updating for non-email notifications,
		-- or email notifications where the start date has not passed)
		update core.[Notification]
		set [Subject] = @Subject,
			[Message] = @Message
		where Notification_ID = @Notification_ID
			and ((Email_Batch_ID is not null and [Start_Date] >= @currentDate) or (Email_Batch_ID is null))
			and ([Subject] <> @Subject or [Message] <> @Message)

		if @@ROWCOUNT <> 0 and @shouldRevokeDismissal = 0
			set @shouldRevokeDismissal = 1

		set @New_Email_Message_Required = 0
		-- Fourth-stage update to handle Send_As_Email
		if @Send_As_Email = 0
		begin
			-- If there's already an email batch and we're unsetting Send_As_Email, delete it.
			-- This will automatically unset Email_Batch_ID
			delete eb
			from core.EmailBatch as eb
			inner join core.[Notification] as n
				on (n.Email_Batch_ID = eb.Email_Batch_ID)
			where n.Notification_ID = @Notification_ID
				and n.[Start_Date] >= @currentDate
				and n.Email_Batch_ID is not null
		end
		else if @Send_As_Email = 1
		begin
			-- If we're trying to enable emails on an existing notification, only create
			-- an email batch if the notification doesn't have one!
			insert into core.EmailBatch ([Type_ID], External_Message_ID)
				select 1, ''
				from core.[Notification]
				where Notification_ID = @Notification_ID
					and Email_Batch_ID is null

			-- No need to proceed with setting up an external batch message and updating
			-- core.Notification if nothing was created!
			if @@ROWCOUNT > 0
			begin
				declare @emailBatchId bigint = null
				declare @externalEmailBatchMessage varchar(64) = null

				set @emailBatchId = SCOPE_IDENTITY()
				set @externalEmailBatchMessage = 'EmailBatch.' + cast(@emailBatchId as varchar(64))

				update core.EmailBatch
				set External_Message_ID = @externalEmailBatchMessage
				where Email_Batch_ID = @emailBatchId
				
				update core.[Notification]
				set Email_Batch_ID = @emailBatchId
				where Notification_ID = @Notification_ID

				set @New_Email_Message_Required = 1
			end
		end

		if @shouldRevokeDismissal = 1
			delete from core.NotificationDismissal
			where Notification_ID = @Notification_ID

		exec core.usp_GetNotificationByID @Notification_ID = @Notification_ID

	commit transaction
END
