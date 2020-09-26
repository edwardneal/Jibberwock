CREATE TABLE [security].[SecurityGroupMembership]
(
	[Security_Group_Membership_ID] BIGINT NOT NULL IDENTITY, 
    [Security_Group_ID] BIGINT NOT NULL, 
    [User_ID] BIGINT NOT NULL, 
    [Enabled] BIT NOT NULL, 
    CONSTRAINT [PK_SecurityGroupMembership] PRIMARY KEY ([Security_Group_Membership_ID]), 
    CONSTRAINT [FK_SecurityGroupMembership_SecurityGroup] FOREIGN KEY ([Security_Group_ID]) REFERENCES [security].[SecurityGroup]([Security_Group_ID]), 
    CONSTRAINT [FK_SecurityGroupMembership_User] FOREIGN KEY ([User_ID]) REFERENCES [security].[User]([User_ID]) 
)
