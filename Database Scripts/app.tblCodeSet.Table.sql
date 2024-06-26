USE [RehabMetricsAndOutcomes]
GO
/****** Object:  Table [app].[tblCodeSet]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblCodeSet](
	[CodeSetID] [int] IDENTITY(1,1) NOT NULL,
	[CodeSetParent] [int] NULL,
	[CodeValue] [varchar](30) NOT NULL,
	[CodeDescription] [varchar](400) NOT NULL,
	[HierarchyType] [int] NULL,
	[Active] [bit] NULL,
	[FyConstraint] [int] NULL,
	[SortOrder] [int] NULL,
	[Comment] [varchar](200) NULL,
 CONSTRAINT [PK_tblCodeSet] PRIMARY KEY CLUSTERED 
(
	[CodeSetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblCodeSet]    Script Date: 4/19/2021 9:32:01 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblCodeSet] ON [app].[tblCodeSet]
(
	[CodeSetParent] ASC,
	[CodeValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [app].[tblCodeSet]  WITH CHECK ADD  CONSTRAINT [FK_tblCodeSet_tblCodeSet] FOREIGN KEY([CodeSetParent])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblCodeSet] CHECK CONSTRAINT [FK_tblCodeSet_tblCodeSet]
GO
