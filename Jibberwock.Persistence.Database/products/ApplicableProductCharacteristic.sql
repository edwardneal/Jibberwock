CREATE TABLE [products].[ApplicableProductCharacteristic]
(
	[Applicable_Product_Characteristic_ID] BIGINT NOT NULL IDENTITY, 
    [Product_ID] BIGINT NOT NULL, 
    [Characteristic_ID] INT NOT NULL, 
    CONSTRAINT [PK_ApplicableProductCharacteristic] PRIMARY KEY ([Applicable_Product_Characteristic_ID]), 
    CONSTRAINT [FK_ApplicableProductCharacteristic_Product] FOREIGN KEY ([Product_ID]) REFERENCES [products].[Product]([Product_ID]), 
    CONSTRAINT [FK_ApplicableProductCharacteristic_Characteristic] FOREIGN KEY ([Characteristic_ID]) REFERENCES [products].[Characteristic]([Characteristic_ID]) 
)
