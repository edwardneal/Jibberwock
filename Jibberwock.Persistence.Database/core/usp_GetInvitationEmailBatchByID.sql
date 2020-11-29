CREATE PROCEDURE [core].[usp_GetInvitationEmailBatchByID]
	@Email_Batch_ID bigint
AS
BEGIN
	set nocount on;
	set xact_abort on;

	select inv.Invitation_ID as Id, inv.Invitation_Date as InvitationDate, inv.Expiration_Date as ExpirationDate,
		-- NB: we pull the external identity provider's description here, since it gets fed into the SendGrid message template
		inv.Email_Address as EmailAddress, eip.[Description] as ExternalIdentityProvider, inv.Status_ID as [Status],
		ten.Tenant_ID as Id, ten.[Name],
		eb.Email_Batch_ID as Id, eb.External_Message_ID as ServiceBusMessageId
	from [tenants].[Invitation] as inv
	inner join [security].[ExternalIdentityProvider] as eip
		on (eip.Provider_ID = inv.Identity_Provider_ID)
	inner join [tenants].[Tenant] as ten
		on (ten.Tenant_ID = inv.Tenant_ID)
	inner join [core].[EmailBatch] as eb
		on (eb.Email_Batch_ID = inv.Email_Batch_ID)
	where eb.Email_Batch_ID = @Email_Batch_ID
END
