CREATE TYPE [core].[udt_ToAddressHash] AS TABLE
(
	Salt VARBINARY(16) NOT NULL, 
    [Hash] VARCHAR(128) NOT NULL
)
