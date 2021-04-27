USE [RehabMetricsAndOutcomes]
GO
/****** Object:  View [app].[vQuestionStandardChoices]    Script Date: 4/19/2021 9:32:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [app].[vQuestionStandardChoices]
as
select *
from [app].[tblQuestion] a
left join [app].[vCodeSetHierarchy] b
on a.AnswerCodeSetFK = b.[Parent id]
GO
