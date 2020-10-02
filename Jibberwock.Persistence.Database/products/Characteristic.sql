CREATE TABLE [products].[Characteristic]
(
	[Characteristic_ID] INT NOT NULL IDENTITY, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Description] NVARCHAR(256) NULL, 
    [Visible] BIT NOT NULL, 
    [Enabled] BIT NOT NULL, 
    [Value_Type_ID] INT NOT NULL, 
    CONSTRAINT [PK_Characteristic] PRIMARY KEY ([Characteristic_ID]), 
    CONSTRAINT [FK_Characteristic_CharacteristicValueType] FOREIGN KEY ([Value_Type_ID]) REFERENCES [products].[CharacteristicValueType]([Characteristic_Value_Type_ID]) 
)
