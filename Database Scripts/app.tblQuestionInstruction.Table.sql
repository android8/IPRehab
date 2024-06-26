USE [RehabMetricsAndOutcomes]
GO
/****** Object:  Table [app].[tblQuestionInstruction]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblQuestionInstruction](
	[InstructionID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionIDFK] [int] NOT NULL,
	[Order] [int] NULL,
	[Instruction] [varchar](max) NOT NULL,
 CONSTRAINT [PK_tblInstruction] PRIMARY KEY CLUSTERED 
(
	[InstructionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_tblInstruction]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblInstruction] ON [app].[tblQuestionInstruction]
(
	[QuestionIDFK] ASC,
	[Order] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [app].[tblQuestionInstruction]  WITH CHECK ADD  CONSTRAINT [FK_tblInstruction_tblQuestion] FOREIGN KEY([QuestionIDFK])
REFERENCES [app].[tblQuestion] ([QuestionID])
GO
ALTER TABLE [app].[tblQuestionInstruction] CHECK CONSTRAINT [FK_tblInstruction_tblQuestion]
GO
