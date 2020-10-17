CREATE TYPE [core].[udt_Email] AS TABLE
(
	External_Email_ID varchar(256) not null,
	To_Address_Salt varbinary(16) not null,
	To_Address_Hash varchar(128)
)
