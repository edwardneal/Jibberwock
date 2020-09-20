CREATE PROCEDURE [components].[usp_GetByStatusProvider]
	@Status_Provider_ID int
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select comp.External_Component_ID as Id, comp.External_ID as ExternalId,
		comp.Purpose_ID as Id, purp.[Name] as [Name],
		[status].External_Component_Status_ID as Id, [status].Status_Provider_ID as StatusProvider, [status].Available, [status].Raw_Status as ThirdPartyStatus, [status].Retrieval_Date as RetrievalDate
	from components.ExternalComponent as comp
	inner join components.Purpose as purp
		on (purp.Purpose_ID = comp.Purpose_ID)
	left outer join components.ExternalComponentStatus as [status]
		on ([status].External_Component_ID = comp.External_Component_ID)
	where [status].Status_Provider_ID = @Status_Provider_ID
		or ([status].External_Component_Status_ID is null and @Status_Provider_ID is null)
END
