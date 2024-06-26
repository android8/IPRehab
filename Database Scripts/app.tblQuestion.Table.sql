USE [RehabMetricsAndOutcomes]
GO
/****** Object:  Table [app].[tblQuestion]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblQuestion](
	[QuestionID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionKey] [varchar](20) NOT NULL,
	[Order] [int] NULL,
	[QuestionTitle] [varchar](200) NULL,
	[Question] [varchar](max) NOT NULL,
	[GroupTitle] [varchar](50) NULL,
	[FormFK] [int] NOT NULL,
	[FormSectionFK] [int] NULL,
	[AnswerCodeSetFK] [int] NOT NULL,
	[BranchingPoint] [bit] NULL,
	[MultiChoice] [bit] NULL,
 CONSTRAINT [PK_app.tblQuestion] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblQuestion_QuestionKey]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblQuestion_QuestionKey] ON [app].[tblQuestion]
(
	[QuestionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [app].[tblQuestion]  WITH CHECK ADD  CONSTRAINT [FK_app.tblQuestion_tblCodeSet] FOREIGN KEY([AnswerCodeSetFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblQuestion] CHECK CONSTRAINT [FK_app.tblQuestion_tblCodeSet]
GO
ALTER TABLE [app].[tblQuestion]  WITH CHECK ADD  CONSTRAINT [FK_tblQuestion_tblCodeSet_FomSection] FOREIGN KEY([FormSectionFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblQuestion] CHECK CONSTRAINT [FK_tblQuestion_tblCodeSet_FomSection]
GO
ALTER TABLE [app].[tblQuestion]  WITH CHECK ADD  CONSTRAINT [FK_tblQuestion_tblCodeSet_Form] FOREIGN KEY([FormFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblQuestion] CHECK CONSTRAINT [FK_tblQuestion_tblCodeSet_Form]
GO
