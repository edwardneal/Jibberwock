CREATE TABLE [products].[ProductConfiguration]
(
	[Product_Configuration_ID] BIGINT NOT NULL IDENTITY, 
    [Configuration_String] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_ProductConfiguration] PRIMARY KEY ([Product_Configuration_ID]) 
)
