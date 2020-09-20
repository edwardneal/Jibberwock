CREATE TABLE [components].[ExternalComponentStatus]
(
	[External_Component_Status_ID] INT NOT NULL  IDENTITY, 
    [External_Component_ID] INT NOT NULL, 
	[Status_Provider_ID] INT NOT NUll, 
    [Available] BIT NOT NULL DEFAULT (1), 
    [Raw_Status] NVARCHAR(MAX) NULL, 
    [Retrieval_Date] DATETIMEOFFSET NULL, 
    CONSTRAINT [PK_ExternalComponentStatus] PRIMARY KEY ([External_Component_Status_ID]), 
    CONSTRAINT [FK_ExternalComponentStatus_ExternalComponent] FOREIGN KEY ([External_Component_ID]) REFERENCES [components].[ExternalComponent]([External_Component_ID]), 
    CONSTRAINT [FK_ExternalComponentStatus_StatusProvider] FOREIGN KEY ([Status_Provider_ID]) REFERENCES [components].[StatusProvider]([Status_Provider_ID]), 
    CONSTRAINT [UQ_ExternalComponentStatus_External_Component_ID] UNIQUE ([External_Component_ID])
)
