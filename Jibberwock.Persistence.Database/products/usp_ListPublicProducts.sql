CREATE PROCEDURE [products].[usp_ListPublicProducts]
AS
BEGIN
	set nocount on;
	set xact_abort on;

	begin transaction

		select p.Product_ID as Id, p.[Name], p.[Description], p.More_Information_URL as MoreInformationUrl,
			p.Visible, sr.Identifier as ResourceIdentifier, sr.[Type_ID] as ResourceType,
			p.Configuration_Control_Name as Configuration_Control_Name,
			pc.Product_Configuration_ID as Id, pc.Configuration_String as ConfigurationString
		from products.Product as p
		inner join [security].SecurableResource as sr
			on (sr.Securable_Resource_ID = p.Product_ID)
		inner join products.ProductConfiguration as pc
			on (pc.Product_Configuration_ID = p.Default_Configuration_ID)
		where p.[Visible] = 1

		select t.Tier_ID as Id, t.External_Identifier as ExternalId, t.[Name],
			t.[Visible], t.[Start_Date] as StartDate, t.End_Date as EndDate,
			p.Product_ID as Id
		from products.Tier as t
		inner join products.Product as p
			on (p.Product_ID = t.Product_ID)
		where p.Visible = 1
			and t.Visible = 1
			and ((t.[Start_Date] is null) or (t.[Start_Date] is not null and t.[Start_Date] <= SYSDATETIMEOFFSET()))
			and ((t.End_Date is null) or (t.End_Date is not null and t.End_Date >= SYSDATETIMEOFFSET()))

	commit transaction

END
