USE [RehabMetricsAndOutcomes]
GO
/****** Object:  Table [dbo].[tblSignature]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSignature](
	[EpisodeCareIDFK] [int] NOT NULL,
	[Signature] [binary](50) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[DateInformationProvided] [date] NOT NULL,
	[Time] [time](7) NULL,
 CONSTRAINT [PK_tblSignature] PRIMARY KEY CLUSTERED 
(
	[EpisodeCareIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblSignature]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblSignature] ON [dbo].[tblSignature]
(
	[Signature] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblSignature]  WITH CHECK ADD  CONSTRAINT [FK_tblSignature_tblEpisodeOfCare] FOREIGN KEY([EpisodeCareIDFK])
REFERENCES [app].[tblEpisodeOfCare] ([EpisodeOfCareID])
GO
ALTER TABLE [dbo].[tblSignature] CHECK CONSTRAINT [FK_tblSignature_tblEpisodeOfCare]
GO
