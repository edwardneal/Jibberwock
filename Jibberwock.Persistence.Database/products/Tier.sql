CREATE TABLE [products].[Tier]
(
	[Tier_ID] BIGINT NOT NULL IDENTITY, 
    [Product_ID] BIGINT NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
    [External_Identifier] NVARCHAR(MAX) NULL, 
    [Visible] BIT NOT NULL, 
    [Start_Date] DATETIMEOFFSET NULL, 
    [End_Date] DATETIMEOFFSET NULL, 
    CONSTRAINT [PK_Tier] PRIMARY KEY ([Tier_ID]), 
    CONSTRAINT [FK_Tier_Product] FOREIGN KEY ([Product_ID]) REFERENCES [products].[Product]([Product_ID]) 
)
