CREATE PROCEDURE [core].[usp_GetEmailHistory]
	@Start_Time datetimeoffset(7),
	@End_Time datetimeoffset(7),
	@Email_Batch_ID bigint,
	@Email_Batch_Type_ID int,
	@External_Message_ID varchar(64),
	@To_Address_Hashes core.udt_ToAddressHash readonly
AS
BEGIN
	set nocount on;
	set xact_abort on;
	declare @emailIds as table (Email_ID bigint, Email_Batch_ID bigint)

	declare @filterToAddress bit = 0

	if exists (select 1 from @To_Address_Hashes)
		set @filterToAddress = 1

	begin transaction
		insert into @emailIds (Email_ID, Email_Batch_ID)
			select eml.Email_ID, eml.Source_Batch_ID
			from core.Email as eml
			inner join core.EmailBatch as ebt
				on (ebt.Email_Batch_ID = eml.Source_Batch_ID)
			where ((@Start_Time is not null and eml.Send_Date >= @Start_Time) or (@Start_Time is null))
				and ((@End_Time is not null and eml.Send_Date <= @End_Time) or (@End_Time is null))
				and ((@Email_Batch_ID is not null and eml.Source_Batch_ID = @Email_Batch_ID) or (@Email_Batch_ID is null))
				and ((@External_Message_ID is not null and ebt.External_Message_ID = @External_Message_ID) or (@External_Message_ID is null))
				and ((@filterToAddress = 1 and eml.To_Address_Hash in
						(select [Hash] from @To_Address_Hashes where [Salt] = eml.To_Address_Salt))
					or (@filterToAddress = 0))

		-- Result set 1 will be the master list of email batches.
		-- We separate this out because rolling both result sets together will
		-- generate twice as many object allocations in Dapper as we want
		select distinct ebt.Email_Batch_ID as Id, ebt.External_Message_ID as ServiceBusMessageId,
			ebt.First_Processing_Date as DateFirstProcessed, ebt.Last_Processing_Date as DateLastProcessed, ebt.Processed_Successfully as ProcessedSuccessfully,
			ebtt.Email_Batch_Type_ID as TypeId, ebtt.[Name] as TypeName
		from core.EmailBatch as ebt
		inner join @emailIds as eml
			on (eml.Email_Batch_ID = ebt.Email_Batch_ID)
		inner join core.EmailBatchType as ebtt
			on (ebtt.Email_Batch_Type_ID = ebt.[Type_ID])

		-- Result set 2 is the master list of emails: the meaningful data we care about
		select emlIds.Email_ID as Id, eml.Send_Date as SendDate, eml.External_Email_ID as ExternalEmailId,
			emlIds.Email_Batch_ID as EmailBatchId
		from @emailIds as emlIds
		inner join core.Email as eml
			on (eml.Email_ID = emlIds.Email_ID)

	commit transaction
END
