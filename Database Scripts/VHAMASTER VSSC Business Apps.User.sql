USE [RehabMetricsAndOutcomes]
GO
/****** Object:  User [VHAMASTER\VSSC Business Apps]    Script Date: 4/19/2021 9:31:58 AM ******/
CREATE USER [VHAMASTER\VSSC Business Apps] FOR LOGIN [VHAMASTER\VSSC Business Apps]
GO
ALTER ROLE [db_owner] ADD MEMBER [VHAMASTER\VSSC Business Apps]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [VHAMASTER\VSSC Business Apps]
GO
