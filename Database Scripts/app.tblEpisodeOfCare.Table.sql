USE [RehabMetricsAndOutcomes]
GO
/****** Object:  Table [app].[tblEpisodeOfCare]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblEpisodeOfCare](
	[EpisodeOfCareID] [int] IDENTITY(1,1) NOT NULL,
	[OnsetDate] [date] NOT NULL,
	[AdmissionDate] [date] NOT NULL,
	[PatientICNFK] [varchar](10) NOT NULL,
 CONSTRAINT [PK_app.tblEpisodeOfCare] PRIMARY KEY CLUSTERED 
(
	[EpisodeOfCareID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [app].[tblEpisodeOfCare]  WITH CHECK ADD  CONSTRAINT [FK_app.tblEpisodeOfCare_app.tblPatient] FOREIGN KEY([PatientICNFK])
REFERENCES [app].[tblPatient] ([ICN])
GO
ALTER TABLE [app].[tblEpisodeOfCare] CHECK CONSTRAINT [FK_app.tblEpisodeOfCare_app.tblPatient]
GO
