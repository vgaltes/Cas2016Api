CREATE TABLE [dbo].[SessionsSpeakers]
(
	[SessionId] INT NOT NULL,
	[SpeakerId] INT NOT NULL,

	CONSTRAINT [uq_idx_sessionsSpeakers] PRIMARY KEY NONCLUSTERED ([SessionId] ASC, [SpeakerId] ASC)
)
