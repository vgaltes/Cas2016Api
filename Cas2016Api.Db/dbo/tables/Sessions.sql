CREATE TABLE [dbo].[Sessions]
(
	[Id] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
	[Title] NVARCHAR(512) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL, 
    [Duration] INT NOT NULL, 
    [StartTime] DATETIME2 NULL, 
    [EndTime] DATETIME2 NULL, 
    [Tags] NVARCHAR(MAX) NULL, 
    [Room] INT NOT NULL,
	[IsPlenary] BIT NULL, 
	[Language] NVARCHAR(2) DEFAULT 'es',
	[ExtraInfo] NVARCHAR(MAX),
    CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Session_Room] FOREIGN KEY ([Room]) REFERENCES [Rooms] ([Id])
)
