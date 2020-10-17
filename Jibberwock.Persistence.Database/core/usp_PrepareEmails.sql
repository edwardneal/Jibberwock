CREATE PROCEDURE [core].[usp_PrepareEmails]
	@Email_Batch_ID bigint,
	@Emails_To_Send core.udt_Email readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @emailExclusions as table
		(
			External_Email_ID varchar(128)
		)
	declare @insertedEmails as table
		(
			External_Email_ID varchar(128),
			Email_ID bigint
		)

	begin transaction
		-- If there are any already-sent emails in the same batch,
		-- sent to the same person, don't send again.
		insert into @emailExclusions (External_Email_ID)
			select distinct e2s.External_Email_ID, Send_Date
			from @Emails_To_Send as e2s
			inner join core.Email as eml
				on (eml.To_Address_Salt = e2s.To_Address_Salt
					and eml.To_Address_Hash = e2s.To_Address_Hash)
			where eml.Source_Batch_ID = @Email_Batch_ID
				and eml.Send_Date is not null

		-- Insert the records into core.Email, excluding the set above
		insert into core.Email (Source_Batch_ID, Send_Date,
			External_Email_ID, To_Address_Salt, To_Address_Hash)
			output inserted.External_Email_ID, inserted.Email_ID into @insertedEmails
			select @Email_Batch_ID, null,
				e2s.External_Email_ID, e2s.To_Address_Salt, e2s.To_Address_Hash
			from @Emails_To_Send as e2s
			where e2s.External_Email_ID not in
				(select External_Email_ID from @emailExclusions)

		select External_Email_ID from @emailExclusions

		select Email_ID as Id, External_Email_ID as ExternalEmailId from @insertedEmails

	commit transaction
END
