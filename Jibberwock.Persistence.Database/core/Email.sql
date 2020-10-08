CREATE TABLE [core].[Email]
(
	[Email_ID] BIGINT NOT NULL IDENTITY, 
    [Source_Batch_ID] BIGINT NOT NULL, 
    [Send_Date] DATETIMEOFFSET NOT NULL, 
    [External_Email_ID] VARCHAR(256) NOT NULL, 
    [To_Address_Salt] VARBINARY(16) NOT NULL, 
    [To_Address_Hash] VARCHAR(128) NOT NULL, 
    CONSTRAINT [PK_Email] PRIMARY KEY ([Email_ID]), 
    CONSTRAINT [FK_Email_EmailBatch] FOREIGN KEY ([Source_Batch_ID]) REFERENCES [core].[EmailBatch]([Email_Batch_ID]) 
)
