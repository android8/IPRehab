USE [RehabMetricsAndOutcomes]
GO
/****** Object:  User [VHAMASTER\VA-NSOC-DB-Scan]    Script Date: 4/19/2021 9:31:58 AM ******/
CREATE USER [VHAMASTER\VA-NSOC-DB-Scan] FOR LOGIN [VHAMASTER\VA-NSOC-DB-Scan] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [vansocscan_role] ADD MEMBER [VHAMASTER\VA-NSOC-DB-Scan]
GO
