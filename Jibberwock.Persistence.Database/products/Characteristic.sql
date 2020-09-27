CREATE TABLE [products].[Characteristic]
(
	[Characteristic_ID] INT NOT NULL IDENTITY, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Description] NVARCHAR(256) NULL, 
    [Visible] BIT NOT NULL, 
    [Enabled] BIT NOT NULL, 
    CONSTRAINT [PK_Characteristic] PRIMARY KEY ([Characteristic_ID]) 
)
