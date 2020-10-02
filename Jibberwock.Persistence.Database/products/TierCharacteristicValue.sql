CREATE TABLE [products].[TierCharacteristicValue]
(
	[Tier_Characteristic_Value_ID] BIGINT NOT NULL IDENTITY, 
    [Tier_ID] BIGINT NOT NULL, 
    [Characteristic_ID] INT NOT NULL, 
    [String_Value] NVARCHAR(MAX) NULL, 
    [Boolean_Value] BIT NULL, 
    [Numeric_Value] BIGINT NULL, 
    CONSTRAINT [PK_TierCharacteristicValue] PRIMARY KEY ([Tier_Characteristic_Value_ID]), 
    CONSTRAINT [FK_TierCharacteristicValue_Tier] FOREIGN KEY ([Tier_ID]) REFERENCES [products].[Tier]([Tier_ID]), 
    CONSTRAINT [FK_TierCharacteristicValue_Characteristic] FOREIGN KEY ([Characteristic_ID]) REFERENCES [products].[Characteristic]([Characteristic_ID]) 
)
