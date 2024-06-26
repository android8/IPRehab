USE [RehabMetricsAndOutcomes]
GO
/****** Object:  Table [app].[tblPatient]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblPatient](
	[ICN] [varchar](10) NOT NULL,
	[IEN] [varchar](10) NOT NULL,
	[FirstName] [varchar](20) NOT NULL,
	[LastName] [varchar](20) NOT NULL,
	[MiddleName] [varchar](20) NULL,
	[DateOfBirth] [date] NOT NULL,
	[Last4SSN] [char](4) NOT NULL,
 CONSTRAINT [PK_tblPatient] PRIMARY KEY CLUSTERED 
(
	[ICN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblPatient_UniqueName]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblPatient_UniqueName] ON [app].[tblPatient]
(
	[LastName] ASC,
	[FirstName] ASC,
	[MiddleName] ASC,
	[Last4SSN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
