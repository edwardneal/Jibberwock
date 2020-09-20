CREATE TABLE [components].[ExternalComponent]
(
	[External_Component_ID] INT NOT NULL  IDENTITY, 
    [External_ID] NVARCHAR(MAX) NULL, 
    [Purpose_ID] INT NOT NULL, 
    CONSTRAINT [PK_ExternalComponent] PRIMARY KEY ([External_Component_ID]), 
    CONSTRAINT [FK_ExternalComponent_Purpose] FOREIGN KEY ([Purpose_ID]) REFERENCES [components].[Purpose]([Purpose_ID])
)
