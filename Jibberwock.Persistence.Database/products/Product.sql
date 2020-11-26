CREATE TABLE [products].[Product]
(
	[Product_ID] BIGINT NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Description] NVARCHAR(256) NULL, 
    [More_Information_URL] NVARCHAR(256) NOT NULL, 
    [Visible] BIT NOT NULL, 
    [Default_Configuration_ID] BIGINT NOT NULL, 
    [Configuration_Control_Name] VARCHAR(64) NOT NULL, 
    CONSTRAINT [PK_Product] PRIMARY KEY ([Product_ID]), 
    CONSTRAINT [FK_Product_SecurableResource] FOREIGN KEY ([Product_ID]) REFERENCES [security].[SecurableResource]([Securable_Resource_ID]), 
    CONSTRAINT [FK_Product_ProductConfiguration] FOREIGN KEY ([Default_Configuration_ID]) REFERENCES [products].[ProductConfiguration]([Product_Configuration_ID]) 
)
