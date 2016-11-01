CREATE TABLE [dbo].[SessionsSpeakers]
(
	[SessionId] INT NOT NULL,
	[SpeakerId] INT NOT NULL,

	CONSTRAINT [uq_idx_sessionsSpeakers] UNIQUE NONCLUSTERED ([SessionId] ASC, [SpeakerId] ASC)
)
