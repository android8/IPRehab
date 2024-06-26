USE [RehabMetricsAndOutcomes]
GO
/****** Object:  Table [app].[tblAnswer]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblAnswer](
	[EpsideOfCareIDFK] [int] NOT NULL,
	[QuestionIDFK] [int] NOT NULL,
	[AnswerCodeSetFK] [int] NOT NULL,
	[AnswerSequenceNumber] [int] NOT NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_tblAnswer] PRIMARY KEY CLUSTERED 
(
	[EpsideOfCareIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer] ON [app].[tblAnswer]
(
	[EpsideOfCareIDFK] ASC,
	[QuestionIDFK] ASC,
	[AnswerCodeSetFK] ASC,
	[AnswerSequenceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer_AnswerCodeSetFK]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer_AnswerCodeSetFK] ON [app].[tblAnswer]
(
	[AnswerCodeSetFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer_EpisodeOfCareIDFK]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer_EpisodeOfCareIDFK] ON [app].[tblAnswer]
(
	[EpsideOfCareIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer_QuestionIDFK]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer_QuestionIDFK] ON [app].[tblAnswer]
(
	[QuestionIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [app].[tblAnswer]  WITH CHECK ADD  CONSTRAINT [FK_tblAnswer_tblCodeSet] FOREIGN KEY([AnswerCodeSetFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblAnswer] CHECK CONSTRAINT [FK_tblAnswer_tblCodeSet]
GO
ALTER TABLE [app].[tblAnswer]  WITH CHECK ADD  CONSTRAINT [FK_tblAnswer_tblEpisodeOfCare] FOREIGN KEY([EpsideOfCareIDFK])
REFERENCES [app].[tblEpisodeOfCare] ([EpisodeOfCareID])
GO
ALTER TABLE [app].[tblAnswer] CHECK CONSTRAINT [FK_tblAnswer_tblEpisodeOfCare]
GO
ALTER TABLE [app].[tblAnswer]  WITH CHECK ADD  CONSTRAINT [FK_tblAnswer_tblQuestion] FOREIGN KEY([QuestionIDFK])
REFERENCES [app].[tblQuestion] ([QuestionID])
GO
ALTER TABLE [app].[tblAnswer] CHECK CONSTRAINT [FK_tblAnswer_tblQuestion]
GO
