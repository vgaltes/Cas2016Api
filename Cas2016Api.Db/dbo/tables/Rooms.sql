﻿CREATE TABLE [dbo].[Rooms]
(
	[Id] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
	[Name] NVARCHAR(128) NOT NULL,
	[Capacity] INT NOT NULL
)
