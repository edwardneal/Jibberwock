if 1 not in (select [Invitation_Status_ID] from [tenants].[InvitationStatus])
	insert into [tenants].[InvitationStatus] (Invitation_Status_ID, [Name])
	values (1, 'active')

if 2 not in (select [Invitation_Status_ID] from [tenants].[InvitationStatus])
	insert into [tenants].[InvitationStatus] (Invitation_Status_ID, [Name])
	values (2, 'revoked')