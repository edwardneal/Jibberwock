CREATE PROCEDURE [security].[usp_CreateSecurableResource]
	@Securable_Resource_Type_ID int,
	@Created_Securable_Resource_ID bigint output
AS
BEGIN
	set nocount on;
	set xact_abort on;

	declare @identifier varchar(16)
	declare @identifierDigitLength int = 6

	select @identifier = Prefix + '-' from [security].[SecurableResourceType] where Securable_Resource_Type_ID = @Securable_Resource_Type_ID

	begin transaction

		-- Generate the initial securable resource (so we can get its ID), then
		-- transform it after the fact
		insert into [security].[SecurableResource] ([Type_ID], [Identifier])
		values (@Securable_Resource_Type_ID, '-')

		select @identifier = @identifier + right(replicate('0', @identifierDigitLength) + cast(SCOPE_IDENTITY() as varchar(16)), @identifierDigitLength)

		update [security].[SecurableResource]
		set [Identifier] = @identifier
		where Securable_Resource_ID = SCOPE_IDENTITY()

		set @Created_Securable_Resource_ID = SCOPE_IDENTITY()

	commit transaction
END
