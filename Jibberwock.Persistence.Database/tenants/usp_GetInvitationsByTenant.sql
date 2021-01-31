CREATE PROCEDURE [tenants].[usp_GetInvitationsByTenant]
	@Tenant_ID bigint,
	@Include_Inapplicable bit = 0
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select inv.Invitation_ID as Id, inv.Invitation_Date as InvitationDate, inv.Expiration_Date as ExpirationDate,
		inv.Email_Address as EmailAddress, eip.Claim_Value as ExternalIdentityProvider, inv.Status_ID as [Status],
		usr.[User_ID] as Id, usr.[Name], usr.[Enabled], usr.[Type_ID] as [Type]
	from [tenants].[Invitation] as inv
	inner join [security].[ExternalIdentityProvider] as eip
		on (eip.Provider_ID = inv.Identity_Provider_ID)
	inner join [security].[User] as usr
		on (usr.[User_ID] = inv.[User_ID])
	where ((@Include_Inapplicable = 1)
		or (@Include_Inapplicable = 0 and inv.Status_ID = 1
			and ((inv.Expiration_Date is null) or (inv.Expiration_Date is not null and inv.Expiration_Date >= SYSDATETIMEOFFSET()))
		))
		and (inv.Tenant_ID = @Tenant_ID)
END