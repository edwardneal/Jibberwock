CREATE PROCEDURE [components].[usp_UpdateStatus]
	@External_Component_ID int,
	@Status_Provider_ID int,
	@Raw_Status nvarchar(max),
	@Available bit
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		if @External_Component_ID not in (select External_Component_ID from components.ExternalComponentStatus)
			insert into components.ExternalComponentStatus (External_Component_ID, Status_Provider_ID, [Available])
			values (@External_Component_ID, @Status_Provider_ID, 0)

		update components.ExternalComponentStatus
		set Status_Provider_ID = @Status_Provider_ID,
			Available = @Available,
			Raw_Status = @Raw_Status,
			Retrieval_Date = SYSDATETIMEOFFSET()
		where External_Component_ID = @External_Component_ID

		select External_Component_Status_ID as Id, Status_Provider_ID as StatusProvider,
			Available, Raw_Status as ThirdPartyStatus, Retrieval_Date as RetrievalDate
		from components.ExternalComponentStatus
		where External_Component_ID = @External_Component_ID

	commit transaction
END
