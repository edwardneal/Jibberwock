CREATE TABLE [core].[EmailBatch]
(
	[Email_Batch_ID] BIGINT NOT NULL IDENTITY, 
    [Type_ID] INT NOT NULL, 
    [External_Message_ID] VARCHAR(64) NOT NULL, 
    [First_Processing_Date] DATETIMEOFFSET NULL, 
    [Last_Processing_Date] DATETIMEOFFSET NULL, 
    [Processed_Successfully] BIT NULL, 
    CONSTRAINT [PK_EmailBatch] PRIMARY KEY ([Email_Batch_ID]), 
    CONSTRAINT [FK_EmailBatch_EmailBatchType] FOREIGN KEY ([Type_ID]) REFERENCES [core].[EmailBatchType]([Email_Batch_Type_ID]) 
)
