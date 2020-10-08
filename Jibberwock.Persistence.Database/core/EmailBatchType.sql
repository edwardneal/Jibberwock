CREATE TABLE [core].[EmailBatchType]
(
	[Email_Batch_Type_ID] INT NOT NULL, 
    [Name] VARCHAR(32) NOT NULL, 
    [Sender_Address] NVARCHAR(256) NOT NULL, 
    [Sender_Alias] NVARCHAR(256) NOT NULL, 
    [External_Unsubscription_Group_ID] INT NOT NULL, 
    [External_Message_Template_ID] VARCHAR(64) NOT NULL, 
    CONSTRAINT [PK_EmailBatchType] PRIMARY KEY ([Email_Batch_Type_ID]) 
)
