USE [RehabMetricsAndOutcomes]
GO
/****** Object:  User [AAC\vaoiscsocdbscan]    Script Date: 4/19/2021 9:31:58 AM ******/
CREATE USER [AAC\vaoiscsocdbscan] FOR LOGIN [AAC\vaoiscsocdbscan] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [vacsocitc_role] ADD MEMBER [AAC\vaoiscsocdbscan]
GO
